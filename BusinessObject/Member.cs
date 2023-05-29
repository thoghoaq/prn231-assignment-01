using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace BusinessObject
{
    public partial class Member
    {
        public Member()
        {
            Orders = new HashSet<Order>();
        }

        [Required]
        public int MemberId { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Company Name must be between 2 and 100 characters", MinimumLength = 2)]
        public string CompanyName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "City must be between 2 and 50 characters", MinimumLength = 2)]
        public string City { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Country must be between 2 and 50 characters", MinimumLength = 2)]
        public string Country { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Password must be between 6 and 100 characters", MinimumLength = 6)]
        public string Password { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
