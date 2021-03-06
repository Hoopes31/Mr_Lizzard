using System.ComponentModel.DataAnnotations;

namespace scaffold.Models
{
    public class LoginViewModel : BaseEntity
    {
        public string username {get;set;}
        [Required]
        [DataType(DataType.Password)]
        public string password {get;set;}
    }
}