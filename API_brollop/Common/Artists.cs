using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_brollop.Common
{
    public class Artists
    {
        public string Href { get; set; }
        public List<Item> Items { get; set; }
        public dynamic Limit { get; set; }
        public int Offset { get; set; }
        public int Total { get; set; }
    }
}