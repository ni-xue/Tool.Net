using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebTestApp.Test
{
    public class Class2 : EndpointDataSource
    {
        public override IReadOnlyList<Endpoint> Endpoints { get; }

        public Class2() 
        {
            Endpoints = new List<Endpoint>();
        }

        public override IChangeToken GetChangeToken()
        {
            return null;
        }
    }
}
