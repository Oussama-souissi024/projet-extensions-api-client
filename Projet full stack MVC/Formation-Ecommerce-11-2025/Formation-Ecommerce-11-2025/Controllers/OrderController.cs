using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Formation_Ecommerce_11_2025.Application.Orders.Dtos;
using Formation_Ecommerce_11_2025.Application.Orders.Interfaces;
using Formation_Ecommerce_11_2025.Core.Entities.Identity;
using Formation_Ecommerce_11_2025.Core.Entities.Orders;
using Formation_Ecommerce_11_2025.Core.Utility;
using Formation_Ecommerce_11_2025.Models.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Formation_Ecommerce_11_2025.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderServices _orderServices;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(IOrderServices orderServices,
                               UserManager<ApplicationUser> userManager)
        {
            _orderServices = orderServices;
            _userManager = userManager;
        }

        public IActionResult OrderIndex()
        {
            return View();
        }

        public async Task<IActionResult> OrderConfirmation(Guid orderId)
        {
            try
            {
                var order = await _orderServices.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    TempData["error"] = "Order not found";
                    return RedirectToAction(nameof(OrderDetail), new { orderId });
                }

                var response = await _orderServices.UpdateOrderStatusAsync(orderId, StaticDetails.Status_Approved);
                if (response.HasValue && response.Value)
                {
                    TempData["success"] = "Status updated successfully";
                    return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
                }
                return View();
            }
            catch (Exception ex)
            {
                TempData["error"] = "An error occurred while updating the order status";
                return RedirectToAction(nameof(OrderDetail), new { orderId });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string status)
        {
            try
            {
                IEnumerable<OrderHeaderDto> list;

                if (User.IsInRole("Admin"))
                {
                    // Get all orders based on user role
                    list = _orderServices.GetAllOrdersAsync();
                }
                else
                {
                    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    list = _orderServices.GetOrdersByUserIdAsync(userId);
                }

                if (list != null)
                {
                    // Filter based on status if provided
                    switch (status?.ToLower())
                    {
                        case "approved":
                            list = list.Where(u => u.Status == StaticDetails.Status_Approved);
                            break;
                        case "readyforpickup":
                            list = list.Where(u => u.Status == StaticDetails.Status_ReadyForPickup);
                            break;
                        case "cancelled":
                            list = list.Where(u => u.Status == StaticDetails.Status_Cancelled ||
                                                 u.Status == StaticDetails.Status_Refunded);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    list = Enumerable.Empty<OrderHeaderDto>();
                }

                return Json(new { data = list.OrderByDescending(u => u.Id) });
            }
            catch (Exception ex)
            {
                // Return empty list in case of error
                return Json(new { data = Enumerable.Empty<OrderHeaderDto>() });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> OrderDetail(Guid orderId)
        {
            try
            {
                // Récupérer les détails de la commande avec ses lignes
                var orderHeader = await _orderServices.GetOrderWithDetailsByIdAsync(orderId);

                if (orderHeader == null)
                {
                    TempData["error"] = "Commande introuvable";
                    return RedirectToAction(nameof(OrderIndex));
                }

                // Vérifier si l'utilisateur est autorisé à voir cette commande (admin ou propriétaire)
                if (!User.IsInRole("Admin") && orderHeader.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                {
                    TempData["error"] = "Vous n'êtes pas autorisé à voir cette commande";
                    return RedirectToAction(nameof(OrderIndex));
                }

                // Mapper les données vers le ViewModel
                var orderViewModel = new OrderViewModel
                {
                    Id = orderHeader.Id,
                    OrderHeaderId = orderHeader.Id,  // Ajout de cette propriété pour correspondre à la vue
                    UserId = orderHeader.UserId,
                    CouponCode = orderHeader.CouponCode,
                    Discount = (decimal)(orderHeader.Discount),
                    OrderTotal = (decimal)(orderHeader.OrderTotal ?? 0),
                    Name = orderHeader.Name,
                    Phone = orderHeader.Phone,
                    Email = orderHeader.Email,
                    OrderTime = orderHeader.OrderTime,
                    Status = orderHeader.Status,
                    OrderDetails = orderHeader.OrderDetails.Select(od => new Models.Order.OrderDetailsViewModel
                    {
                        Id = od.Id,
                        OrderHeaderId = od.OrderHeaderId,
                        ProductName = od.ProductName ?? "Produit inconnu",
                        Price = (decimal)od.Price,
                        Count = od.Count,
                        ImageUrl = od.ProductUrl
                    }).ToList()
                };

                return View(orderViewModel);
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Erreur lors de la récupération des détails de la commande: {ex.Message}";
                return RedirectToAction(nameof(OrderIndex));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> OrderReadyForPickup(Guid orderId)
        {
            try
            {
                var order = await _orderServices.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    TempData["error"] = "Order not found";
                    return RedirectToAction(nameof(OrderDetail), new { orderId });
                }

                var response = await _orderServices.UpdateOrderStatusAsync(orderId, StaticDetails.Status_ReadyForPickup);
                if (response.HasValue && response.Value)
                {
                    TempData["success"] = "Status updated successfully";
                    return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
                }
                return View();
            }
            catch (Exception ex)
            {
                TempData["error"] = "An error occurred while updating the order status";
                return RedirectToAction(nameof(OrderDetail), new { orderId });
            }
        }

        [HttpPost("CompleteOrder")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CompleteOrder(Guid orderId)
        {
            try
            {
                var order = await _orderServices.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    TempData["error"] = "Order not found";
                    return RedirectToAction(nameof(OrderDetail), new { orderId });
                }

                var response = await _orderServices.UpdateOrderStatusAsync(orderId, StaticDetails.Status_Completed);
                if (response.HasValue && response.Value)
                {
                    TempData["success"] = "Status updated successfully";
                    return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
                }
                return View();
            }
            catch (Exception ex)
            {
                TempData["error"] = "An error occurred while updating the order status";
                return RedirectToAction(nameof(OrderDetail), new { orderId });
            }
        }


        [HttpPost("CancelOrder")]
        public async Task<IActionResult> CancelOrder(Guid orderId)
        {
            try
            {
                var order = await _orderServices.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    TempData["error"] = "Order not found";
                    return RedirectToAction(nameof(OrderDetail), new { orderId });
                }

                // Only allow cancellation of approved orders               
                    var response = await _orderServices.UpdateOrderStatusAsync(orderId, StaticDetails.Status_Cancelled);
                    if (response != null)
                    {
                        TempData["success"] = "Order cancelled and refunded successfully";
                    }
                    else
                    {
                        TempData["error"] = "Failed to update order status";
                    }

                return RedirectToAction(nameof(OrderDetail), new { orderId });
            }
            catch (Exception ex)
            {
                TempData["error"] = "An error occurred while cancelling the order";
                return RedirectToAction(nameof(OrderDetail), new { orderId });
            }
        }
    }
}
