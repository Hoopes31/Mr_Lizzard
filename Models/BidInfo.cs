using System;
using System.Collections.Generic;
namespace scaffold.Models
{
    public class BidInfo : BaseEntity
    {
        public string product_name { get; set; }
        public string description { get; set; }
        public float starting_bid { get; set; }
        public string first_name { get; set; }
        public int users_id { get; set; }
    }
}