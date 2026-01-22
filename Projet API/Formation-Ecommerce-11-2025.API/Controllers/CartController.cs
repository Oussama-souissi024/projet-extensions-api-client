using Formation_Ecommerce_11_2025.Application.Cart.Dtos;
using Formation_Ecommerce_11_2025.Application.Cart.Interfaces;
using Formation_Ecommerce_11_2025.Application.Coupons.Interfaces;
using Formation_Ecommerce_11_2025.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Formation_Ecommerce_11_2025.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ICouponService _couponService;

        public CartController(ICartService cartService, ICouponService couponService)
        {
            _cartService = cartService;
            _couponService = couponService;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

        /// <summary>
        /// Récupère le panier de l'utilisateur connecté
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<CartDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new CartDto { CartHeader = new CartHeaderDto { UserID = userId }, CartDetails = new List<CartDetailsDto>() };
            }

            return Ok(new ApiResponse<CartDto>
            {
                Success = true,
                Data = cart
            });
        }

        /// <summary>
        /// Ajoute ou met à jour le panier
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CartDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpsertCart([FromBody] CartDto cartDto)
        {
            cartDto.CartHeader ??= new CartHeaderDto();
            cartDto.CartHeader.UserID = GetUserId();

            var result = await _cartService.UpsertCartAsync(cartDto);
            return Ok(new ApiResponse<CartDto>
            {
                Success = true,
                Data = result
            });
        }

        /// <summary>
        /// Supprime un article du panier
        /// </summary>
        [HttpDelete("items/{cartDetailsId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveItem(Guid cartDetailsId)
        {
            var removed = await _cartService.RemoveCartItemAsync(cartDetailsId);
            if (!removed)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Impossible de supprimer l'article"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true
            });
        }

        /// <summary>
        /// Applique un coupon au panier
        /// </summary>
        [HttpPost("apply-coupon")]
        [ProducesResponseType(typeof(ApiResponse<CartDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ApplyCoupon([FromBody] ApplyCouponRequest request)
        {
            if (string.IsNullOrEmpty(request.CouponCode))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Code coupon requis"
                });
            }

            var existingCoupon = await _couponService.GetCouponByCodeAsync(request.CouponCode);
            if (existingCoupon == null)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Coupon invalide"
                });
            }

            var userId = GetUserId();
            var cartDto = await _cartService.ApplyCouponAsync(userId, request.CouponCode);
            if (cartDto == null)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Erreur lors de l'application du coupon"
                });
            }

            return Ok(new ApiResponse<CartDto>
            {
                Success = true,
                Data = cartDto
            });
        }

        /// <summary>
        /// Retire le coupon du panier
        /// </summary>
        [HttpPost("remove-coupon")]
        [ProducesResponseType(typeof(ApiResponse<CartDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveCoupon()
        {
            var userId = GetUserId();
            var cartDto = await _cartService.ApplyCouponAsync(userId, "");
            return Ok(new ApiResponse<CartDto>
            {
                Success = true,
                Data = cartDto
            });
        }

        /// <summary>
        /// Vide le panier
        /// </summary>
        [HttpDelete]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();
            await _cartService.ClearCartAsync(userId);
            return Ok(new ApiResponse<object>
            {
                Success = true
            });
        }
    }

    public class ApplyCouponRequest
    {
        public string CouponCode { get; set; } = "";
    }
}
