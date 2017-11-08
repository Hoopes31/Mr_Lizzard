using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace scaffold.Models
{
    public class AuctionView : BaseEntity
    {
        [Required(ErrorMessage = "Please enter a product name!")]
        [MinLength(3, ErrorMessage = "Description must be greater than 3 characters.")]
        public string product_name { get; set; }

        [Required(ErrorMessage = "Please enter a brief description.")]
        [MinLength(10, ErrorMessage = "Description must be greater than 10 characters.")]
        public string description { get; set; }

        [Required(ErrorMessage = "Please enter a bid amount.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public float starting_bid { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DateValidator]
        public DateTime end_date { get; set; }
    }
    public class DateValidator : ValidationAttribute
    {
        private DateTime _today = DateTime.Now;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            AuctionView item = (AuctionView)validationContext.ObjectInstance;
            if(_today <= item.end_date)
            {
                return ValidationResult.Success;
            }
            else{
                return new ValidationResult("Date must not be in the past.");
            }
        }
    }
}