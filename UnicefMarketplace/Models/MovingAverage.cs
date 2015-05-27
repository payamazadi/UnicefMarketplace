using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UnicefMarketplace.Models
{
    public class MovingAverage
    {
        public int Days { get; set; }
        public int Orders { get; set; }
        public decimal Price { get; set; }
    }
}