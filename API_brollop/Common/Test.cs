using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_brollop.Common
{
    public class Test
    {
        public List<TestContent> data { get; set; }
    }

    public class TestContent
    { 
        public string Name { get; set; }
    }
}