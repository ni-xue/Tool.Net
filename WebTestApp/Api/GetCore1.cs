using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tool.Utils.Data;
using Tool.Web.Api;

namespace WebTestApp.Api
{
    public class GetCore1 : ApiAshx
    {
        private readonly Tool.SqlCore.DbHelper dbHelper;

        public GetCore1(Tool.SqlCore.DbHelper dbHelper,
            Microsoft.Extensions.Logging.ILoggerFactory loggerFactory,
            Microsoft.AspNetCore.Hosting.IWebHostEnvironment env,
            IHttpContextAccessor context)
        {
            this.dbHelper = dbHelper;
        }

        public async Task Async([ApiVal(Val.Service)] IHttpContextAccessor context)
        {
            System.Data.DataSet ds = await dbHelper.ExecuteDataSetAsync("SELECT * FROM system OrDER by id desc", new { a = 1, b = 2, c = 3, d = 4 });

            await JsonAsync(ds.Tables[0].ToDictionary()); //JsonAsync(new { i = 10 });
        }
    }
}
