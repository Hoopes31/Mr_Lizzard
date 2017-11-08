using System;
using System.Collections.Generic;
namespace scaffold.Models
{
    public class CompleteItem : BaseEntity
    {
        public int id { get; set; } 
        public string product_name { get; set; }
        public string description { get; set; }
        public float starting_bid { get; set; }
        public DateTime end_date { get; set; }
        public float top_bid { get; set; }
        public string first_name { get; set; }
    }
}