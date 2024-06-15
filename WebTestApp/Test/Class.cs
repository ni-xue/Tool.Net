using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tool.Web.Session;

namespace WebTestApp.Test
{
    public class Class : DiySession
    {
        public Class() { }
        //public override void Initialize()
        //{
        //    //throw new NotImplementedException();
        //}

        public override async Task Initialize()
        {
            await base.Initialize();
        }


        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> GetKeys()
        {
            throw new NotImplementedException();
        }

        public override void Remove(string key)
        {
            throw new NotImplementedException();
        }

        public override void Set(string key, byte[] value)
        {
            throw new NotImplementedException();
        }

        public override bool TryGetValue(string key, out byte[] value)
        {
            throw new NotImplementedException();
        }
    }
}
