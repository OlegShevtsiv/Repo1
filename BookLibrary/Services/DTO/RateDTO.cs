using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTO
{
    public class RateDTO
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string BookId { get; set; }
        public decimal Value { get; set; }
    }
}
