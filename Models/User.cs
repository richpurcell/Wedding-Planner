using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Add this to use [NotMapped]
using System.Collections.Generic;

namespace WeddingPlanner.Models
{
    public class User
    {
        // Add Id and use [Key] as the validator
        [Key]
        public int UserId {get;set;}
        [Required]
        [MinLength(2)]
        [Display(Name = "First Name:")]
        public string FirstName {get;set;}
        [Required]
        [MinLength(2)]
        [Display(Name = "Last Name:")]
        public string LastName {get;set;}
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email:")]
        public string Email {get;set;}
        [Required]
        [MinLength(8, ErrorMessage="Password must be 8 characters or longer!")]
        [DataType(DataType.Password)]
        [Display(Name = "Password:")]
        // [Compare("Confirm")]
        public string Password {get;set;}
        [Required]
        [NotMapped] // This field is not mapped in the Model but is required to compare the password and Confirm fields match
        [MinLength(8)]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password:")]
        [Compare("Password")]
        public string Confirm {get;set;}
        // Add CreatedAt as a DateTime type
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        // Add UpdatedAt as a DateTime type
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
        public List<Association> Events {get;set;}
    }
}