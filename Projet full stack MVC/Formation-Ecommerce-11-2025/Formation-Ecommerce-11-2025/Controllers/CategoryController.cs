using AutoMapper;
using Formation_Ecommerce_11_2025.Application.Categories.Dtos;
using Formation_Ecommerce_11_2025.Application.Categories.Interfaces;
using Formation_Ecommerce_11_2025.Models.Category;
using Microsoft.AspNetCore.Mvc;

namespace Formation_Ecommerce_11_2025.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        public CategoryController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }
        public async Task<IActionResult> CategoryIndex()
        {
            IEnumerable<CategoryDto> categories = await _categoryService.ReadAllAsync();
            IEnumerable<CategoryViewModel> viewModels = _mapper.Map<IEnumerable<CategoryViewModel>>(categories);
            TempData["success"] = "Category successfully Loaded!";
            TempData["error"] = "Test Temp Data Error!";
            return View(viewModels);
        }

        public IActionResult CreateCategory()
        {
            return View(new CreateCategoryViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryViewModel createCategoryViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(createCategoryViewModel);
            }

            try
            {
                var createCategoryDto = _mapper.Map<CreateCategoryDto>(createCategoryViewModel);
                var result = await _categoryService.AddAsync(createCategoryDto);

                if (result != null)
                {
                    TempData["success"] = "Category created successfully!";
                    return RedirectToAction(nameof(CategoryIndex));
                }
                TempData["error"] = "Failed to create category.";
                return View(createCategoryViewModel);
            }
            catch (Exception ex)
            {
                // Consider logging the exception
                ModelState.AddModelError("", "An error occurred while creating the category.");
                TempData["error"] = ex.Message;
                return View(createCategoryViewModel);
            }
        }

        public async Task<IActionResult> EditCategory(Guid id)
        {
            try
            {
                var categoryDto = await _categoryService.ReadByIdAsync(id);
                if (categoryDto == null)
                {
                    return RedirectToAction(nameof(CategoryIndex));
                }
                return View(_mapper.Map<EditCatgoryViewModel>(categoryDto));
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(CategoryIndex));
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(EditCatgoryViewModel editCatgoryViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editCatgoryViewModel);
            }

            try
            {
                var updateCategoryDto = _mapper.Map<UpdateCategoryDto>(editCatgoryViewModel);
                await _categoryService.UpdateAsync(updateCategoryDto);
                return RedirectToAction(nameof(CategoryIndex));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the category.");
                return View(editCatgoryViewModel);
            }
        }

        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            try
            {
                var categoryDto = await _categoryService.ReadByIdAsync(id);
                if (categoryDto == null)
                {
                    TempData["error"] = "Category not found.";
                    return RedirectToAction(nameof(CategoryIndex));
                }
                return View(_mapper.Map<DeleteCategoryViewModel>(categoryDto));
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error loading category for deletion.";
                return RedirectToAction(nameof(CategoryIndex));
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategoryConfirmed(DeleteCategoryViewModel deleteCategoryViewModel)
        {
            try
            {
                await _categoryService.DeleteAsync(deleteCategoryViewModel.Id);
                TempData["success"] = "Category deleted successfully!";
                return RedirectToAction(nameof(CategoryIndex));
            }
            catch (Exception ex)
            {
                TempData["error"] = "Failed to delete category.";
                return RedirectToAction(nameof(CategoryIndex));
            }
        }
    }
}
