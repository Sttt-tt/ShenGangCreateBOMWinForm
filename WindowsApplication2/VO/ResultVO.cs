using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsApplication2.VO
{
    class ResultVO
    {
        public int Record { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
        public Array[] Attach { get; set; }
    }
}
