using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Tool.SqlCore
{
    /// <summary>
    /// 存储过程操作类（返回的消息对象）
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    [Serializable]
    public class Message : IMessage
    {
        /// <summary>
        /// 返回的状态ID
        /// </summary>
        public int MessageID
        {
            get
            {
                return this.m_messageID;
            }
            set
            {
                this.m_messageID = value;
                this.m_success = (this.m_messageID == 0);
            }
        }

        /// <summary>
        /// 执行成功与否（状态）
        /// </summary>
        public bool Success
        {
            get
            {
                return this.m_success;
            }
            set
            {
                this.m_success = value;
                if (this.m_success)
                {
                    this.m_messageID = 0;
                    return;
                }
                this.m_messageID = -1;
            }
        }

        /// <summary>
        /// 获取当前存储过程的参数信息
        /// </summary>
        public List<DbParameter> Prams
        {
            get
            {
                return this.m_prams;
            }
        }

        /// <summary>
        /// 根据参数名称获取参数值
        /// </summary>
        /// <param name="Name">参数名称,模糊查询，尽量精确变量名称</param>
        /// <returns>返回参数值</returns>
        public object GetPramsName(string Name)
        {
            object obj = null;
            foreach (DbParameter _prams in Prams)
            {
                if (_prams.ParameterName.Contains(Name))
                {
                    obj = _prams.Value;
                    break;
                }
            }
            return obj;
        }

        /// <summary>
        /// 初始化参数
        /// </summary>
        public Message()
        {
            this.MessageID = 0;
            this.Success = true;
            this.Content = string.Empty;
            this.EntityList = new ArrayList();
            this.m_prams = null;
        }

        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <param name="isSuccess">执行结果</param>
        public Message(bool isSuccess) : this(isSuccess, "")
        {
        }

        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <param name="isSuccess">执行结果</param>
        /// <param name="content">输出内容</param>
        public Message(bool isSuccess, string content) : this()
        {
            this.MessageID = (isSuccess ? 0 : -1);
            this.Content = content;
        }

        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <param name="messageID">消息ID</param>
        /// <param name="content">输出内容</param>
        public Message(int messageID, string content) : this()
        {
            this.MessageID = messageID;
            this.Content = content;
        }

        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <param name="prams">SQL数据对象</param>
        public Message(List<DbParameter> prams) : this()
        {
            this.m_prams = prams;
            this.MessageID = Utils.TypeParse.StrToInt(prams[prams.Count - 1].Value, -1);
            if (prams.Count > 1)
            {
                DbParameter db = prams[prams.Count - 2];
                if (db.Direction == System.Data.ParameterDirection.Output && db.DbType == System.Data.DbType.String)
                {
                    this.Content = db.Value.ToString();
                }
            }
        }

        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <param name="isSuccess">执行结果</param>
        /// <param name="content">输出内容</param>
        /// <param name="entityList">数据集</param>
        public Message(bool isSuccess, string content, ArrayList entityList) : this(isSuccess, content)
        {
            this.EntityList = entityList;
        }

        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <param name="messageID">消息ID</param>
        /// <param name="content">输出内容</param>
        /// <param name="entityList">数据集</param>
        public Message(int messageID, string content, ArrayList entityList) : this(messageID, content)
        {
            this.EntityList = entityList;
        }

        /// <summary>
        /// 给数据集合赋值
        /// </summary>
        /// <param name="entityList">一个数据集合</param>
        public void AddEntity(ArrayList entityList)
        {
            this.EntityList = entityList;
        }

        /// <summary>
        /// 添加数据到集合
        /// </summary>
        /// <param name="entity">数据源</param>
        public void AddEntity(object entity)
        {
            this.EntityList.Add(entity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetEntityList<T>(int index) 
        {
            return EntityList[index].ToVar<T>();
        }

        /// <summary>
        /// 清除所有返回的数据集合
        /// </summary>
        public void ResetEntityList()
        {
            if (this.EntityList != null)
            {
                this.EntityList.Clear();
            }
        }

        /// <summary>
        /// 存储过程返回信息
        /// </summary>
        public string Content
        {
            get;
            set;
        }

        /// <summary>
        /// 返回的数据集合
        /// </summary>
        public ArrayList EntityList
        {
            get;
            set;
        }

        private int m_messageID;

        private bool m_success;

        private readonly List<DbParameter> m_prams;
    }
}
