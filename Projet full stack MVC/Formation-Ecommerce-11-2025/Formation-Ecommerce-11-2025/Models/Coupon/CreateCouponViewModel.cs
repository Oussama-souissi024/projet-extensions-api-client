using System.ComponentModel.DataAnnotations;

namespace Formation_Ecommerce_11_2025.Models.Coupon
{
    public class CreateCouponViewModel
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string CouponCode { get; set; }

        [Required]
        [Range(0.01, 10000)]
        public decimal DiscountAmount { get; set; }

        [Required]
        [Range(0, 10000)]
        public decimal MinimumAmount { get; set; }
    }
}
