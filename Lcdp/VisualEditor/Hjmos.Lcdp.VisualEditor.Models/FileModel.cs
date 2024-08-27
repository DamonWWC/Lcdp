using Hjmos.Lcdp.VisualEditorServer.Entities.Enums;
using Prism.Commands;
using System.Windows;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Models
{
    public class FileModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 父ID
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 文件层级（深度）
        /// </summary>
        public int? Level { get; set; }

        /// <summary>
        /// 所有父ID（逗号分隔）
        /// </summary>
        public string ParentIds { get; set; }

        /// <summary>
        /// 文件后缀
        /// </summary>
        public string Suffix { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public FileType FileType { get; set; }

        /// <summary>
        /// 页面文件Guid
        /// </summary>
        public string Guid { get; set; }

        #region Command

        /// <summary>
        /// 复制Guid命令
        /// </summary>
        public ICommand CopyCommand
        {
            get
            {
                if (_copyCommand == null) _copyCommand = new DelegateCommand<FileModel>(f => Clipboard.SetText(f.Guid.ToString()));
                return _copyCommand;
            }
        }
        private ICommand _copyCommand;

        /// <summary>
        /// 重命名命令
        /// </summary>
        public ICommand RenameCommand
        {
            get
            {
                if (_renameCommand == null) _renameCommand = new DelegateCommand<FileModel>(f => Clipboard.SetText(f.Guid.ToString()));
                return _renameCommand;
            }
        }
        private ICommand _renameCommand;

        /// <summary>
        /// 删除命令
        /// </summary>
        public ICommand DeleteCommand { get; set; }
        

        #endregion
    }
}