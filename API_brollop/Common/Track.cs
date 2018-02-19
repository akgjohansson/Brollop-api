using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_brollop.Common
{
    public class Track
    {
        public string Href { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public int Duration_ms { get; set; }
        public string Type { get; set; }
        public string Album { get; set; }
        public string Artist { get; set; }
    }
}