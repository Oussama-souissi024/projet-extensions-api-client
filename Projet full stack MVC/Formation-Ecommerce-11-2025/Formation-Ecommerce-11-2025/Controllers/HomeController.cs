using System.Diagnostics;
using AutoMapper;
using Formation_Ecommerce_11_2025.Application.Cart.Dtos;
using Formation_Ecommerce_11_2025.Application.Cart.Interfaces;
using Formation_Ecommerce_11_2025.Application.Products.Dtos;
using Formation_Ecommerce_11_2025.Application.Products.Interfaces;
using Formation_Ecommerce_11_2025.Core.Entities.Identity;
using Formation_Ecommerce_11_2025.Models;
using Formation_Ecommerce_11_2025.Models.Home;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Formation_Ecommerce_11_2025.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductServices _productServices;
        private readonly ICartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public HomeController(IProductServices productServices,
                              IMapper mapper,
                              ICartService cartService,
                              UserManager<ApplicationUser> userManager)
        {
            _productServices = productServices;
            _mapper = mapper;
            _cartService = cartService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["error"] = "Please Login First";
                return RedirectToAction("Login", "Auth");
            }
            IEnumerable<ProductDto> productDtos = await _productServices.ReadAllAsync();
            if (productDtos == null || !productDtos.Any())
            {
                TempData["error"] = "Product not found";
                return View(new List<HomeProductViewModel>()); // Return an empty view model list
            }

            var productViewModelDtoList = _mapper.Map<List<HomeProductViewModel>>(productDtos); // Map to a list            

            return View(productViewModelDtoList); // Pass the list to the view
        }

        [HttpGet]
        public async Task<IActionResult> ProductDetails(Guid productId)
        {
            var productDto = await _productServices.ReadByIdAsync(productId);
            if (productDto == null)
            {
                TempData["error"] = "Product not found";
                return RedirectToAction(nameof(Index));
            }
            var vm = _mapper.Map<HomeProductViewModel>(productDto);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ProductDetails(HomeProductViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["error"] = "Please Login First";
                return RedirectToAction("Login", "Auth");
            }

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["error"] = "Please Login First";
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var cartDto = new CartDto
                {
                    CartHeader = new CartHeaderDto { UserID = userId },
                    CartDetails = new List<CartDetailsDto>
                    {
                        new CartDetailsDto
                        {
                            ProductId = model.Id,
                            Count = model.Count
                        }
                    }
                };

                var result = await _cartService.UpsertCartAsync(cartDto);
                if (result != null)
                {
                    TempData["success"] = "Product added to cart";
                }
                else
                {
                    TempData["error"] = "Unable to add product to cart";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }         
}
