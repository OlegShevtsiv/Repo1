using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Models
{
    public class WishList
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public string BookId { get; set; }

        public WishList()
        {

        }
    }
}
