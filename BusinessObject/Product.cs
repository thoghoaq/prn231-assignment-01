using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace BusinessObject
{
    public partial class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Product name must be between 2 and 100 characters", MinimumLength = 2)]
        public string ProductName { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Weight must be between 1 and 20 characters", MinimumLength = 1)]
        public string Weight { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit Price must be greater than 0")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Units in Stock must be greater than or equal to 0")]
        public int UnitsInStock { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
