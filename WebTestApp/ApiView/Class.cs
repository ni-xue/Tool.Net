using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tool.Web.Api;

namespace WebTestApp.ApiView
{
    public class Class : MinApi
    {
        [CrossDomain(Origin = "*", Headers = "content-Type")]
        [Ashx(State = AshxState.Post)]
        public IApiOut As() { return ApiOut.Json(new { a = 5 }); }
    }
}
