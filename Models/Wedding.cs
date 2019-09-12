using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Add this to use [NotMapped]
using System.Collections.Generic;

namespace WeddingPlanner.Models
{
    public class Wedding
    {
        [Key]
        public int WeddingId {get;set;}
        [Required]
        [MinLength(2)]
        [Display(Name = "Wedder One:")]
        public string WedderOne {get;set;}
        [Required]
        [MinLength(2)]
        [Display(Name = "Wedder Two:")]
        public string WedderTwo {get;set;}
        [Required]
        [FutureDate]
        [DataType(DataType.Date)]
        [Display(Name = "Date:")]
        public DateTime Date {get;set;}
        [Required]
        [Display(Name = "Wedding Address:")]
        public string Address {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
        public int PlannerId {get;set;}
        public List<Association> Attendees {get;set;}
    }
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if ((DateTime)value <= DateTime.Now){
                return new ValidationResult("Date must be in the future");
            }
            return ValidationResult.Success;
        }
    }
}