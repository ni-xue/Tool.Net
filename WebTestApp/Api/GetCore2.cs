using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tool.Web.Api;

namespace WebTestApp.Api
{
    public class GetCore2 : ApiAshx
    {
        public GetCore2()//HttpClient httpClient
        {

        }

        public async Task Async()
        {
            await JsonAsync("{\"i\":10}");
        }
    }
}
