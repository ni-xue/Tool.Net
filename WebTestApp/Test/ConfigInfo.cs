/*
 * 版本： 4.5
 * 日期：2020/5/18 15:51:52
 * 
 * 描述：实体类
 * 
 */

using System;
using System.Collections.Generic;

namespace WebTestApp.Test
{

    /// <summary>
    /// 实体类 ConfigInfo  (属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class ConfigInfo
    {

        #region 常量 

        /// <summary>
        /// 表名
        /// </summary>
        public const string Tablename = "ConfigInfo";

        /// <summary>
        /// ConfigID，描述:配置标识
        /// </summary>
        public const string _ConfigID = "ConfigID";

        /// <summary>
        /// ConfigKey，描述:配置KEY
        /// </summary>
        public const string _ConfigKey = "ConfigKey";

        /// <summary>
        /// ConfigName，描述:配置名称
        /// </summary>
        public const string _ConfigName = "ConfigName";

        /// <summary>
        /// ConfigString，描述:配置说明
        /// </summary>
        public const string _ConfigString = "ConfigString";

        /// <summary>
        /// Field1，描述:
        /// </summary>
        public const string _Field1 = "Field1";

        /// <summary>
        /// Field2，描述:
        /// </summary>
        public const string _Field2 = "Field2";

        /// <summary>
        /// Field3，描述:
        /// </summary>
        public const string _Field3 = "Field3";

        /// <summary>
        /// Field4，描述:
        /// </summary>
        public const string _Field4 = "Field4";

        /// <summary>
        /// Field5，描述:
        /// </summary>
        public const string _Field5 = "Field5";

        /// <summary>
        /// Field6，描述:
        /// </summary>
        public const string _Field6 = "Field6";

        /// <summary>
        /// Field7，描述:
        /// </summary>
        public const string _Field7 = "Field7";

        /// <summary>
        /// Field8，描述:
        /// </summary>
        public const string _Field8 = "Field8";

        /// <summary>
        /// SortID，描述:排序ID
        /// </summary>
        public const string _SortID = "SortID";

        #endregion

        #region 私有变量

        private int p_configid;
        private string p_configkey;
        private string p_configname;
        private string p_configstring;
        private string p_field1;
        private string p_field2;
        private string p_field3;
        private string p_field4;
        private string p_field5;
        private string p_field6;
        private string p_field7;
        private string p_field8;
        private int p_sortid;

        #endregion

        #region 构造函数 

        /// <summary>
        /// 初始化ConfigInfo
        /// </summary>
        public ConfigInfo() 
        {
            p_configid = 0;
            p_configkey = string.Empty;
            p_configname = string.Empty;
            p_configstring = string.Empty;
            p_field1 = string.Empty;
            p_field2 = string.Empty;
            p_field3 = string.Empty;
            p_field4 = string.Empty;
            p_field5 = string.Empty;
            p_field6 = string.Empty;
            p_field7 = string.Empty;
            p_field8 = string.Empty;
            p_sortid = 0;
        }

        #endregion

        #region 公共属性 

        /// <summary>
        /// ConfigID，描述:配置标识
        /// </summary>
        public int ConfigID
        {
            set
            {
                p_configid=value;
            }
            get
            {
                return p_configid;
            }
        }

        /// <summary>
        /// ConfigKey，描述:配置KEY
        /// </summary>
        public string ConfigKey
        {
            set
            {
                p_configkey=value;
            }
            get
            {
                return p_configkey;
            }
        }

        /// <summary>
        /// ConfigName，描述:配置名称
        /// </summary>
        public string ConfigName
        {
            set
            {
                p_configname=value;
            }
            get
            {
                return p_configname;
            }
        }

        /// <summary>
        /// ConfigString，描述:配置说明
        /// </summary>
        public string ConfigString
        {
            set
            {
                p_configstring=value;
            }
            get
            {
                return p_configstring;
            }
        }

        /// <summary>
        /// Field1，描述:
        /// </summary>
        public string Field1
        {
            set
            {
                p_field1=value;
            }
            get
            {
                return p_field1;
            }
        }

        /// <summary>
        /// Field2，描述:
        /// </summary>
        public string Field2
        {
            set
            {
                p_field2=value;
            }
            get
            {
                return p_field2;
            }
        }

        /// <summary>
        /// Field3，描述:
        /// </summary>
        public string Field3
        {
            set
            {
                p_field3=value;
            }
            get
            {
                return p_field3;
            }
        }

        /// <summary>
        /// Field4，描述:
        /// </summary>
        public string Field4
        {
            set
            {
                p_field4=value;
            }
            get
            {
                return p_field4;
            }
        }

        /// <summary>
        /// Field5，描述:
        /// </summary>
        public string Field5
        {
            set
            {
                p_field5=value;
            }
            get
            {
                return p_field5;
            }
        }

        /// <summary>
        /// Field6，描述:
        /// </summary>
        public string Field6
        {
            set
            {
                p_field6=value;
            }
            get
            {
                return p_field6;
            }
        }

        /// <summary>
        /// Field7，描述:
        /// </summary>
        public string Field7
        {
            set
            {
                p_field7=value;
            }
            get
            {
                return p_field7;
            }
        }

        /// <summary>
        /// Field8，描述:
        /// </summary>
        public string Field8
        {
            set
            {
                p_field8=value;
            }
            get
            {
                return p_field8;
            }
        }

        /// <summary>
        /// SortID，描述:排序ID
        /// </summary>
        public int SortID
        {
            set
            {
                p_sortid=value;
            }
            get
            {
                return p_sortid;
            }
        }

        #endregion
    }
}

