using System.Collections.Generic;
using Formation_Ecommerce_11_2025.Application.Cart.Dtos;

namespace Formation_Ecommerce_11_2025.Models.Cart
{
    public class CartIndexViewModel
    {
        public CartHeaderDto CartHeader { get; set; } = new CartHeaderDto();
        public List<CartDetailsDto> CartDetails { get; set; } = new List<CartDetailsDto>();
    }
}
