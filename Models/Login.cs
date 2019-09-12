using System.ComponentModel.DataAnnotations;
namespace WeddingPlanner.Models
{
    public class Login
    {
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email:")]
        public string LoginEmail {get;set;}
        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password:")]
        public string LoginPassword {get;set;}
    }
}