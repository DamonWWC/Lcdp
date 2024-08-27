using Hjmos.Lcdp.VisualEditorServer.ICommon;
using Hjmos.Lcdp.VisualEditorServer.IService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Hjmos.Lcdp.VisualEditorServer.Service
{
    public class BaseService : IBaseService
    {
        protected DbContext Context { get; private set; }

        public BaseService(IConnectionFactory contextFactory)
        {
            Context = contextFactory.CreateDbContext();
            //Context.Database.EnsureCreated();
        }

        #region Query

        /// <summary>
        /// 根据id查询实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public T Find<T>(int id) where T : class
        {
            return this.Context.Set<T>().Find(id);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="funcWhere"></param>
        /// <returns></returns>
        public IQueryable<T> Query<T>(Expression<Func<T, bool>> funcWhere) where T : class
        {
            return this.Context.Set<T>().Where<T>(funcWhere);
        }

        /// <summary>
        /// 查询全部
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IQueryable<T> Query<T>() where T : class
        {
            return this.Context.Set<T>();
        }

        #endregion

        #region Insert

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns>带主键的实体</returns>
        public T Insert<T>(T t) where T : class
        {
            this.Context.Set<T>().Add(t);
            this.Commit();
            return t;
        }

        /// <summary>
        /// 多条SQL事务插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tList"></param>
        /// <returns></returns>
        public IEnumerable<T> Insert<T>(IEnumerable<T> tList) where T : class
        {
            this.Context.Set<T>().AddRange(tList);
            this.Commit();
            return tList;
        }

        #endregion

        #region Update

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public void Update<T>(T t) where T : class
        {
            if (t == null) throw new Exception("t is null");

            this.Context.Set<T>().Attach(t);
            this.Context.Entry<T>(t).State = EntityState.Modified;
            this.Commit();
        }

        /// <summary>
        /// 多条SQL事务更新数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tList"></param>
        public void Update<T>(IEnumerable<T> tList) where T : class
        {
            foreach (T t in tList)
            {
                this.Context.Set<T>().Attach(t);
                this.Context.Entry<T>(t).State = EntityState.Modified;
            }
            this.Commit();
        }

        #endregion

        #region Delete

        /// <summary>
        /// 根据主键删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Id"></param>
        public void Delete<T>(int Id) where T : class
        {
            T t = this.Find<T>(Id);
            if (t == null) throw new Exception("t is null");
            this.Context.Set<T>().Remove(t);
            this.Commit();
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public void Delete<T>(T t) where T : class
        {
            if (t == null) throw new Exception("t is null");
            this.Context.Set<T>().Attach(t);
            this.Context.Set<T>().Remove(t);
            this.Commit();
        }

        /// <summary>
        /// 多条SQL事务删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tList"></param>
        public void Delete<T>(IEnumerable<T> tList) where T : class
        {
            foreach (T t in tList)
            {
                this.Context.Set<T>().Attach(t);
            }
            this.Context.Set<T>().RemoveRange(tList);
            this.Commit();
        }

        #endregion

        #region Other

        /// <summary>
        /// 立即保存全部修改
        /// </summary>
        public void Commit()
        {
            this.Context.SaveChanges();
        }

        /// <summary>
        /// 销毁连接
        /// </summary>
        public virtual void Dispose()
        {
            if (this.Context != null)
            {
                this.Context.Dispose();
            }
        }

        #endregion
    }
}
