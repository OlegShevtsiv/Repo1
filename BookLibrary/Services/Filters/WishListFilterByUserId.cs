using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Filters
{
    public class WishListFilterByUserId: IFilter
    {
        public string UserId { get; set; }
    }
}
