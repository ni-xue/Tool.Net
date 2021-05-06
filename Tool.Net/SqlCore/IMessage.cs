using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Tool.SqlCore
{
    /// <summary>
    /// 存储过程操作类（返回的消息对象）接口
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
	public interface IMessage
    {
        /// <summary>
        /// 给数据集合赋值
        /// </summary>
        /// <param name="entityList">一个数据集合</param>
        void AddEntity(ArrayList entityList);

        /// <summary>
        /// 添加数据到集合
        /// </summary>
        /// <param name="entity">数据源</param>
        void AddEntity(object entity);

        /// <summary>
        /// 清除所有返回的数据集合
        /// </summary>
        void ResetEntityList();

        /// <summary>
        /// 存储过程返回信息
        /// </summary>
        string Content
        {
            get;
            set;
        }

        /// <summary>
        /// 返回的数据集合
        /// </summary>
        ArrayList EntityList
        {
            get;
            set;
        }

        /// <summary>
        /// 返回的状态ID
        /// </summary>
        int MessageID
        {
            get;
            set;
        }

        /// <summary>
        /// 执行成功与否（状态）
        /// </summary>
        bool Success
        {
            get;
            set;
        }
    }
}
