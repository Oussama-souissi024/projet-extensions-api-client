using AutoMapper;
using Formation_Ecommerce_11_2025.Application.Coupons.Dtos;
using Formation_Ecommerce_11_2025.Application.Coupons.Interfaces;
using Formation_Ecommerce_11_2025.Models.Coupon;
using Microsoft.AspNetCore.Mvc;

namespace Formation_Ecommerce_11_2025.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;
        private readonly IMapper _mapper;

        public CouponController(ICouponService couponService, IMapper mapper)
        {
            _couponService = couponService;
            _mapper = mapper;
        }

        public async Task<IActionResult> CouponIndex()
        {
            try
            {
                IEnumerable<CouponDto> couponDtos = await _couponService.ReadAllAsync();
                IEnumerable<CouponViewModel> viewModels = _mapper.Map<IEnumerable<CouponViewModel>>(couponDtos);
                return View(viewModels);
            }
            catch (Exception)
            {
                TempData["error"] = "Error loading coupons.";
                return View(Enumerable.Empty<CouponViewModel>());
            }
        }

        public IActionResult CreateCoupon()
        {
            return View(new CreateCouponViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateCoupon(CreateCouponViewModel createCouponViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(createCouponViewModel);
            }
            try
            {
                var couponDto = _mapper.Map<CouponDto>(createCouponViewModel);
                var result = await _couponService.AddAsync(couponDto);

                if (result != null)
                {
                    TempData["success"] = "Coupon created successfully!";
                    return RedirectToAction(nameof(CouponIndex));
                }

                return View(createCouponViewModel);
            }
            catch (InvalidOperationException ex)
            {
                TempData["error"] = ex.Message;
                return View(createCouponViewModel);
            }
            catch (Exception)
            {
                TempData["error"] = "Failed to create coupon.";
                return View(createCouponViewModel);
            }
        }

        public async Task<IActionResult> DeleteCoupon(Guid couponId)
        {
            try
            {
                var couponDto = await _couponService.ReadByIdAsync(couponId);
                if (couponDto == null)
                {
                    TempData["error"] = "Coupon not found.";
                    return RedirectToAction(nameof(CouponIndex));
                }
                var couponToDelete = _mapper.Map<DeleteCouponViewModel>(couponDto);
                return View(couponToDelete);
            }
            catch (Exception)
            {
                TempData["error"] = "Error loading coupon for deletion.";
                return RedirectToAction(nameof(CouponIndex));
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCouponConfirmed(DeleteCouponViewModel deleteCouponViewModel)
        {
            try
            {
                await _couponService.DeleteAsync(deleteCouponViewModel.Id);
                TempData["success"] = "Coupon deleted successfully!";
                return RedirectToAction(nameof(CouponIndex));
            }
            catch (InvalidOperationException ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction(nameof(CouponIndex));
            }
            catch (Exception)
            {
                TempData["error"] = "Failed to delete coupon.";
                return RedirectToAction(nameof(CouponIndex));
            }
        }
    }
}
