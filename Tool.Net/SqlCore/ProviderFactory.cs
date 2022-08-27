using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Text;
using Tool.Utils.ActionDelegate;

namespace Tool.SqlCore
{
    /// <summary>
    /// DbProviderFactory工厂类
    /// </summary>
    public class ProviderFactory
    {
        private static readonly Dictionary<DbProviderType, string> providerInvariantNames = new();
        //private static readonly Dictionary<DbProviderType, DbProviderFactory> providerFactoies = new Dictionary<DbProviderType, DbProviderFactory>(20);

        static ProviderFactory()
        {
            //加载已知的数据库访问类的程序集
            providerInvariantNames.Add(DbProviderType.SqlServer, "System.Data.SqlClient.SqlClientFactory, System.Data.SqlClient");
            providerInvariantNames.Add(DbProviderType.SqlServer1, "Microsoft.Data.SqlClient.SqlClientFactory, Microsoft.Data.SqlClient");
            providerInvariantNames.Add(DbProviderType.MySql, "MySql.Data.MySqlClient.MySqlClientFactory, MySql.Data");
            providerInvariantNames.Add(DbProviderType.Oracle, "Oracle.ManagedDataAccess.Client.OracleClientFactory, Oracle.ManagedDataAccess");//"Oracle.DataAccess.Client"
            providerInvariantNames.Add(DbProviderType.SQLite, "System.Data.SQLite.SQLiteFactory, System.Data.SQLite");
            providerInvariantNames.Add(DbProviderType.OleDb, "System.Data.OleDb.OleDbFactory, System.Data.OleDb");

            //providerInvariantNames.Add(DbProviderType.ODBC, "System.Data.ODBC");
            //providerInvariantNames.Add(DbProviderType.Firebird, "FirebirdSql.Data.Firebird");
            //providerInvariantNames.Add(DbProviderType.PostgreSql, "Npgsql");
            //providerInvariantNames.Add(DbProviderType.DB2, "IBM.Data.DB2.iSeries");
            //providerInvariantNames.Add(DbProviderType.Informix, "IBM.Data.Informix");
            //providerInvariantNames.Add(DbProviderType.SqlServerCe, "System.Data.SqlServerCe");
        }

        /// <summary>
        /// 获取指定数据库类型对应的程序集名称
        /// </summary>
        /// <param name="providerType">数据库类型枚举</param>
        /// <returns></returns>
        public static string GetProviderInvariantName(DbProviderType providerType)
        {
            return providerInvariantNames[providerType];
        }

        /// <summary>
        /// 获取指定类型的数据库对应的DbProviderFactory
        /// </summary>
        /// <param name="providerType">数据库类型枚举</param>
        /// <returns></returns>
        public static DbProviderFactory GetDbProviderFactory(DbProviderType providerType)
        {
            //如果还没有加载，则加载该DbProviderFactory
            //if (!providerFactoies.ContainsKey(providerType))
            //{
            //    providerFactoies.Add(providerType, ImportDbProviderFactory(providerType));
            //}
            //return providerFactoies[providerType];

            return ImportDbProviderFactory(providerType);
        }

        /// <summary>
        /// 加载指定数据库类型的DbProviderFactory
        /// </summary>
        /// <param name="providerType">数据库类型枚举</param>
        /// <returns></returns>
        private static DbProviderFactory ImportDbProviderFactory(DbProviderType providerType)
        {
            string providerName = providerInvariantNames[providerType];

            A:
            if (!DbProviderFactories.TryGetFactory(providerType.ToString(), out DbProviderFactory factory))
            {
                //var wsd = DbProviderFactories.GetProviderInvariantNames();

                //Assembly assembly = Assembly.Load(providerName);
                //Type type = assembly.GetType("System.Data.SqlClient.SqlClientFactory"); //System.Data.SqlClient.SqlClientFactory

                //AddFactory(providerType, type);//type//"System.Data.SqlClient.SqlClientFactory"

                AddFactory(providerType.ToString(), providerName, true);

                //AddFactory<System.Data.SqlClient.SqlClientFactory>("asd");
                goto A;
            }
            
            //DbProviderFactory factory;
            //try
            //{
            //    //从全局程序集中查找
            //    factory = DbProviderFactories.GetFactory(providerName);
            //}
            //catch //(ArgumentException e)
            //{
            //    factory = null;
            //}
            return factory;
        }

        /// <summary>
        /// 将现有数据库对象注入实现
        /// </summary>
        /// <param name="providerType">注册数据库类型</param>
        public static void AddFactory<DbProviderFactory>(DbProviderType providerType) where DbProviderFactory : System.Data.Common.DbProviderFactory
        {
            AddFactory(providerType, typeof(DbProviderFactory));
        }

        /// <summary>
        /// 将现有数据库对象注入实现
        /// </summary>
        /// <param name="providerInvariantName">注册的名称</param>
        public static void AddFactory<DbProviderFactory>(string providerInvariantName) where DbProviderFactory : System.Data.Common.DbProviderFactory
        {
            AddFactory(providerInvariantName, typeof(DbProviderFactory));
        }

        /// <summary>
        /// 将现有数据库对象注入实现
        /// </summary>
        /// <param name="providerInvariantName">注册的名称</param>
        /// <param name="strType">"System.Data.SqlClient.SqlClientFactory, System.Data.SqlClient"</param>
        /// <param name="throwOnError">如果为 true，则在找不到该类型时引发异常；如果为 false，则返回 null。 指定 false 还会取消某些其他异常条件，但并不取消所有条件。 请参见“异常”部分。</param>
        public static void AddFactory(string providerInvariantName, string strType, bool throwOnError)
        {
            Type type = Type.GetType(strType, throwOnError, true);
            if (type == null)//                Type.GetType("Game.Kernel.SqlServerProvider, Game.Kernel", false, true);
            {
                throw new Exception("您注册的对象未存在！");
            }
            AddFactory(providerInvariantName, type);
        }

        ///// <summary>
        ///// 将现有数据库对象注入实现
        ///// </summary>
        ///// <param name="providerInvariantName">注册的名称</param>
        ///// <param name="typeInvoke">使用<see cref="TypeInvoke"/>对象实现注入</param>
        //public static void AddFactory(string providerInvariantName, TypeInvoke typeInvoke)
        //{
        //    AddFactory(providerInvariantName, typeInvoke.GetType());
        //}

        /// <summary>
        /// 将现有数据库对象注入实现
        /// </summary>
        /// <param name="providerType">注册数据库类型</param>
        /// <param name="providerFactoryClass">对应的数据库<see cref="Type"/></param>
        public static void AddFactory(DbProviderType providerType, Type providerFactoryClass)
        {
            AddFactory(providerType.ToString(), providerFactoryClass);
        }


        /// <summary>
        /// 将现有数据库对象注入实现
        /// </summary>
        /// <param name="providerInvariantName">注册的名称</param>
        /// <param name="providerFactoryClass">对应的数据库<see cref="Type"/></param>
        public static void AddFactory(string providerInvariantName, Type providerFactoryClass)
        {
            DbProviderFactories.RegisterFactory(providerInvariantName, providerFactoryClass);
        }

        /// <summary>
        /// 加载指定数据库类型的DbProviderFactory
        /// </summary>
        /// <param name="providerName">数据库类型引用信息</param>
        /// <returns></returns>
        public static DbProviderFactory GetFactory(string providerName)
        {
            DbProviderFactory factory;
            try
            {
                //从全局程序集中查找
                factory = DbProviderFactories.GetFactory(providerName);
            }
            catch //(ArgumentException e)
            {
                factory = null;
            }
            return factory;
        }
    }
}
