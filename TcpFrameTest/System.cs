using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace TcpFrameTest
{
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public class System
    {
        public int Id { get; set; }

        public string Key_en { get; set; }

        public string Key_cn { get; set; }

        public long Value { get; set; }


        private string GetDebuggerDisplay()
        {
            return ToString();
        }
    }

    public class Abc 
    {
        public int Aid { get; set; }

        public string Bkey_en { get; set; }

        public string Ckey_cn { get; set; }

        public string Dvalue { get; set; }
    }
}
