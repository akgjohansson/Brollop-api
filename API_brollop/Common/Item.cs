using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_brollop.Common
{
    public class Item
    {
        //public List<External_Url> External_Urls { get; set; }
        //public List<Image> Images { get; set; }
        public string Href { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Uri { get; set; }
    }
}