using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Filters
{
    public class WishListFullFilter : IFilter
    {
        public string UserId { get; set; }
        public string BookId { get; set; }
        public string Name { get; set; }
    }
}
