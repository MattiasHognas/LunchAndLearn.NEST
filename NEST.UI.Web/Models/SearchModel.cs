using System.Collections.Generic;
using Nest;

namespace NEST.UI.Web.Models
{
    public class SearchModel
    {
        public string Search { get; set; }
        public IEnumerable<Product> Documents { get; set; }
        public IEnumerable<Highlight> Highlights { get; set; }
        public double MaxScore { get; set; }
        public int Total { get; set; }
    }
}