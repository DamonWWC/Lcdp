using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Hjmos.Lcdp.VisualEditor.Controls.Services
{
    internal interface ITransactionItem : IUndoAction
    {
        void Do();
        void Undo();
        bool MergeWith(ITransactionItem other);
    }


    /// <summary>描述撤消或重做堆栈上可用的操作</summary>
    public interface IUndoAction
    {
        /// <summary>受操作影响的元素列表</summary>
        ICollection<DesignItem> AffectedElements { get; }

        /// <summary>操作的标题</summary>
        string Title { get; }
    }

    #region UndoTransaction

    /// <summary>
    /// 支持ChangeGroup事务和撤消行为
    /// </summary>
    internal sealed class UndoTransaction : ChangeGroup, ITransactionItem
    {
        internal UndoTransaction(ICollection<DesignItem> affectedElements) => this.AffectedElements = affectedElements;

        public ICollection<DesignItem> AffectedElements { get; }

        public enum TransactionState
        {
            Open,
            Completed,
            Undone,
            Failed
        }

        public TransactionState State { get; private set; }

        private readonly List<ITransactionItem> items = new();

        public void Execute(ITransactionItem item)
        {
            AssertState(TransactionState.Open);
            item.Do();

            foreach (ITransactionItem existingItem in items)
            {
                if (existingItem.MergeWith(item))
                    return;
            }

            items.Add(item);
        }

        private void AssertState(TransactionState expectedState)
        {
            if (State != expectedState)
                throw new InvalidOperationException("Expected state " + expectedState + ", but state is " + State);
        }

        public event EventHandler Committed;
        public event EventHandler RolledBack;

        public override void Commit()
        {
            AssertState(TransactionState.Open);
            State = TransactionState.Completed;
            Committed?.Invoke(this, EventArgs.Empty);
        }

        public override void Abort()
        {
            AssertState(TransactionState.Open);
            State = TransactionState.Failed;
            InternalRollback();
            RolledBack?.Invoke(this, EventArgs.Empty);
        }

        public void Undo()
        {
            AssertState(TransactionState.Completed);
            State = TransactionState.Undone;
            InternalRollback();
        }

        private void InternalRollback()
        {
            try
            {
                for (int i = items.Count - 1; i >= 0; i--)
                {
                    items[i].Undo();
                }
            }
            catch
            {
                State = TransactionState.Failed;
                throw;
            }
        }

        public void Redo()
        {
            AssertState(TransactionState.Undone);
            try
            {
                for (int i = 0; i < items.Count; i++)
                {
                    items[i].Do();
                }
                State = TransactionState.Completed;
            }
            catch
            {
                State = TransactionState.Failed;
                try
                {
                    InternalRollback();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception rolling back after Redo error:\n" + ex.ToString());
                }
                throw;
            }
        }

        void ITransactionItem.Do()
        {
            if (State != TransactionState.Completed)
            {
                Redo();
            }
        }

        protected override void Dispose()
        {
            if (State == TransactionState.Open)
            {
                try
                {
                    Abort();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception rolling back after failure:\n" + ex.ToString());
                }
            }
        }

        bool ITransactionItem.MergeWith(ITransactionItem other) => false;
    }
    #endregion

    #region UndoService

    /// <summary>
    /// 在设计图面上支持撤消/重做操作的服务。  
    /// </summary>
    public sealed class UndoService
    {
        private readonly Stack<UndoTransaction> _transactionStack = new();
        private readonly Stack<ITransactionItem> _undoStack = new();
        private readonly Stack<ITransactionItem> _redoStack = new();

        internal UndoTransaction StartTransaction(ICollection<DesignItem> affectedItems)
        {
            UndoTransaction t = new(affectedItems);
            _transactionStack.Push(t);
            t.Committed += TransactionFinished;
            t.RolledBack += TransactionFinished;
            t.Committed += (s, e) => Execute((UndoTransaction)s);
            return t;
        }

        private void TransactionFinished(object sender, EventArgs e)
        {
            if (sender != _transactionStack.Pop())
            {
                throw new Exception("Invalid transaction finish, nested transactions must finish first");
            }
        }

        internal void Execute(ITransactionItem item)
        {
            if (_transactionStack.Count == 0)
            {
                item.Do();
                _undoStack.Push(item);
                _redoStack.Clear();
                OnUndoStackChanged(EventArgs.Empty);
            }
            else
            {
                _transactionStack.Peek().Execute(item);
            }
        }

        /// <summary>
        /// 撤消操作是否可用
        /// </summary>
        public bool CanUndo => _undoStack.Count > 0;

        /// <summary>
        /// 撤消栈改变事件
        /// </summary>
        public event EventHandler UndoStackChanged;

        private void OnUndoStackChanged(EventArgs e) => UndoStackChanged?.Invoke(this, e);

        /// <summary>
        /// 撤消最后一个行为
        /// </summary>
        public void Undo()
        {
            if (!CanUndo)
                throw new InvalidOperationException("Cannot Undo: undo stack is empty");
            if (_transactionStack.Count != 0)
                throw new InvalidOperationException("Cannot Undo while transaction is running");
            ITransactionItem item = _undoStack.Pop();
            try
            {
                item.Undo();
                _redoStack.Push(item);
                OnUndoStackChanged(EventArgs.Empty);
            }
            catch
            {
                // state might be invalid now, clear stacks to prevent getting more inconsistencies
                Clear();
                throw;
            }
        }

        /// <summary>
        /// Gets the list of names of the available actions on the undo stack.
        /// </summary>
        public IEnumerable<IUndoAction> UndoActions => GetActions(_undoStack);

        /// <summary>
        /// Gets the list of names of the available actions on the undo stack.
        /// </summary>
        public IEnumerable<IUndoAction> RedoActions => GetActions(_redoStack);

        private static IEnumerable<IUndoAction> GetActions(Stack<ITransactionItem> stack)
        {
            foreach (ITransactionItem item in stack)
                yield return item;
        }

        /// <summary>
        /// Gets if there are redo actions available.
        /// </summary>
        public bool CanRedo { get { return _redoStack.Count > 0; } }

        /// <summary>
        /// Redoes a previously undone action.
        /// </summary>
        public void Redo()
        {
            if (!CanRedo)
                throw new InvalidOperationException("Cannot Redo: redo stack is empty");
            if (_transactionStack.Count != 0)
                throw new InvalidOperationException("Cannot Redo while transaction is running");
            ITransactionItem item = _redoStack.Pop();
            try
            {
                item.Do();
                _undoStack.Push(item);
                OnUndoStackChanged(EventArgs.Empty);
            }
            catch
            {
                // state might be invalid now, clear stacks to prevent getting more inconsistencies
                Clear();
                throw;
            }
        }

        /// <summary>
        /// Clears saved actions (both undo and redo stack).
        /// </summary>
        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
            OnUndoStackChanged(EventArgs.Empty);
        }
    }
    #endregion
}
