using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Filters
{
    public class WishListFilterByUserIdAndName : IFilter
    {
        public string Name { get; set; }
        public string UserId { get; set; }
    }
}
