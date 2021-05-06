using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tool.SqlCore;

namespace WebTestApp.Test
{
    public abstract class TSqlAttribute : Attribute
    {
        public delegate dynamic OnAction(dynamic dic);

        public OnAction OnSqlAction { get; set; }

        public virtual object OnStart(DbHelper dbHelper) 
        {
            return null;
        }
    }

    public class SelectAttribute : TSqlAttribute
    {

        public SelectAttribute(string sql) 
        {
            
        }

        public override object OnStart(DbHelper dbHelper)
        {
            return null;
        }

    }
}
