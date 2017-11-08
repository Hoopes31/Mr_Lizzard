using System.ComponentModel.DataAnnotations;

namespace scaffold.Models
{
    public class UserViewModel : BaseEntity
    {
        [Required]
        [MinLength(2, ErrorMessage = "Name must be greater than 2 characters")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage="Non-Letter Characters are not permitted")]
        public string first_name {get;set;}
        [Required]
        [MinLength(2, ErrorMessage = "Name must be greater than 2 characters")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage="Non-Letter Characters are not permitted")]
        public string last_name {get;set;}
        [Required]
        [MinLength(4, ErrorMessage = "Username must be greater than 3 characters")]
        [MaxLength(20, ErrorMessage = "Username must be less than 20 characters")]
        public string username {get;set;}
        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string password {get;set;}
        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        [Compare("password", ErrorMessage = "Password and confirmation must match")]
        public string passwordConfirm{get;set;}

    }
}