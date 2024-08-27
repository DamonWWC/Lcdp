using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Hjmos.Lcdp.VisualEditorServer.IService
{
    public interface IBaseService
    {
        #region Query

        /// <summary>
        /// 根据id查询实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T Find<T>(int id) where T : class;

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="funcWhere"></param>
        /// <returns></returns>
        IQueryable<T> Query<T>(Expression<Func<T, bool>> funcWhere) where T : class;

        /// <summary>
        /// 查询全部
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IQueryable<T> Query<T>() where T : class;

        #endregion

        #region Insert

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="t"></param>
        /// <returns>带主键的实体</returns>
        T Insert<T>(T t) where T : class;

        /// <summary>
        /// 多条SQL事务新增数据
        /// </summary>
        /// <param name="tList"></param>
        IEnumerable<T> Insert<T>(IEnumerable<T> tList) where T : class;

        #endregion

        #region Update

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="t"></param>
        void Update<T>(T t) where T : class;

        /// <summary>
        /// 多条SQL事务更新数据
        /// </summary>
        /// <param name="tList"></param>
        void Update<T>(IEnumerable<T> tList) where T : class;

        #endregion

        #region Delete

        /// <summary>
        /// 根据主键删除数据
        /// </summary>
        /// <param name="t"></param>
        void Delete<T>(int Id) where T : class;

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        void Delete<T>(T t) where T : class;

        /// <summary>
        /// 多条SQL事务删除数据
        /// </summary>
        /// <param name="tList"></param>
        void Delete<T>(IEnumerable<T> tList) where T : class;

        #endregion

        #region Other

        /// <summary>
        /// 立即保存全部修改
        /// </summary>
        void Commit();

        #endregion
    }
}
