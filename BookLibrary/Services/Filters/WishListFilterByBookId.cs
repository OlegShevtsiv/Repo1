using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Filters
{
    public class WishListFilterByBookId: IFilter
    {
        public string BookId { get; set; }
    }
}
