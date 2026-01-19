using AutoMapper;
using Formation_Ecommerce_11_2025.Application.Categories.Interfaces;
using Formation_Ecommerce_11_2025.Application.Products.Dtos;
using Formation_Ecommerce_11_2025.Application.Products.Interfaces;
using Formation_Ecommerce_11_2025.Models.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Formation_Ecommerce_11_2025.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductServices _productServices;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public ProductController(IProductServices productServices,
                                ICategoryService categoryService,
                                IMapper mapper)
        {
            _productServices = productServices;
            _categoryService = categoryService;
            _mapper = mapper;
        }
        public async Task<IActionResult> ProductIndex()
        {
            try
            {
                IEnumerable<ProductDto> productDtos = await _productServices.ReadAllAsync();
                var viewModels = _mapper.Map<IEnumerable<ProductViewModel>>(productDtos);
                return View(viewModels);
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Erreur lors du chargement des produits : {ex.Message}";
                return View(new List<ProductViewModel>());
            }
        }

        // GET: Formulaire de création de produit
        public async Task<IActionResult> CreateProduct()
        {
            try
            {
                // Récupérer les catégories pour le dropdown
                var categories = await _categoryService.ReadAllAsync();

                // Préparer les catégories au format SelectListItem pour le ViewBag
                ViewBag.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(), // Id de la catégorie comme valeur
                    Text = c.Name, // Nom de la catégorie comme texte
                }).ToList();

                return View();
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Erreur lors du chargement du formulaire : {ex.Message}";
                return RedirectToAction(nameof(ProductIndex));
            }
        }

        // POST: Traitement de la création de produit
        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductViewModel createProductViewModel)
        {

            if (!ModelState.IsValid)
            {
                // Récupérer les catégories pour le dropdown si le modèle est invalide
                // Récupérer les catégories pour le dropdown
                var categories = await _categoryService.ReadAllAsync();

                // Préparer les catégories au format SelectListItem pour le ViewBag
                ViewBag.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(), // Id de la catégorie comme valeur
                    Text = c.Name, // Nom de la catégorie comme texte
                }).ToList();

                return View();
            }

            try
            {
                // La gestion du fichier image sera faite dans le service
                // Nous passons simplement le fichier image au DTO via le mapping
                var productDto = _mapper.Map<CreateProductDto>(createProductViewModel);
                var result = await _productServices.AddAsync(productDto);

                if (result != null)
                {
                    TempData["success"] = "Produit créé avec succès!";
                    return RedirectToAction(nameof(ProductIndex));
                }

                // Si result est null, c'est une erreur inattendue
                ModelState.AddModelError("", "Erreur lors de la création du produit. Veuillez réessayer.");
                TempData["error"] = "Une erreur inattendue s'est produite lors de la création du produit.";

                // Récupérer les catégories à nouveau
                var categories = await _categoryService.ReadAllAsync();

                // Préparer les catégories au format SelectListItem pour le ViewBag
                ViewBag.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                }).ToList();

                return View(createProductViewModel);
            }

            catch (Exception ex)
            {
                // Gérer les autres exceptions
                ModelState.AddModelError("", "Une erreur est survenue lors de la création du produit.");
                TempData["error"] = $"Erreur lors de la création du produit : {ex.Message}";

                // Récupérer les catégories pour le dropdown si le modèle est invalide
                // Récupérer les catégories pour le dropdown
                var categories = await _categoryService.ReadAllAsync();

                // Préparer les catégories au format SelectListItem pour le ViewBag
                ViewBag.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(), // Id de la catégorie comme valeur
                    Text = c.Name, // Nom de la catégorie comme texte
                }).ToList();

                return View();
            }
        }

        // GET: Formulaire de modification
        public async Task<IActionResult> EditProduct(Guid id)
        {
            try
            {
                // Récupérer le produit par son ID
                var productDto = await _productServices.ReadByIdAsync(id);

                if (productDto == null)
                {
                    TempData["error"] = "Produit non trouvé.";
                    return RedirectToAction(nameof(ProductIndex));
                }

                var updateProductViewModel = _mapper.Map<UpdateProductViewModel>(productDto);

                // Récupérer les catégories pour le dropdown
                var categories = await _categoryService.ReadAllAsync();

                // Préparer les catégories au format SelectListItem pour le ViewBag
                ViewBag.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(), // Id de la catégorie comme valeur
                    Text = c.Name, // Nom de la catégorie comme texte
                }).ToList();

                return View(updateProductViewModel);
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Erreur lors du chargement du formulaire : {ex.Message}";
                return RedirectToAction(nameof(ProductIndex));
            }
        }

        // POST: Traitement de la modification
        [HttpPost]
        public async Task<IActionResult> EditProduct(UpdateProductViewModel updateProductViewModel)
        {
            if (!ModelState.IsValid)
            {
                // Récupérer les catégories pour le dropdown en cas d'erreur
                var categories = await _categoryService.ReadAllAsync();
                ViewBag.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                }).ToList();

                return View(updateProductViewModel);
            }

            try
            {
                // Mapper le ViewModel vers le DTO
                var updateProductDto = _mapper.Map<UpdateProductDto>(updateProductViewModel);

                // Mettre à jour le produit
                await _productServices.UpdateAsync(updateProductDto);

                TempData["success"] = "Produit modifié avec succès!";
                return RedirectToAction(nameof(ProductIndex));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Une erreur est survenue lors de la mise à jour du produit.");
                TempData["error"] = $"Erreur lors de la mise à jour du produit : {ex.Message}";

                // Récupérer les catégories pour le dropdown en cas d'erreur
                var categories = await _categoryService.ReadAllAsync();
                ViewBag.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                }).ToList();

                return View(updateProductViewModel);
            }
        }

        // GET: Confirmation de suppression
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            try
            {
                var productDto = await _productServices.ReadByIdAsync(id);
                if (productDto == null)
                {
                    TempData["error"] = "Produit non trouvé.";
                    return RedirectToAction(nameof(ProductIndex));
                }

                var deleteViewModel = _mapper.Map<DeleteProductViewModel>(productDto);
                return View(deleteViewModel);
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Erreur lors du chargement du produit : {ex.Message}";
                return RedirectToAction(nameof(ProductIndex));
            }
        }

        // POST: Traitement de la suppression
        [HttpPost]
        public async Task<IActionResult> DeleteProductConfirmed(DeleteProductViewModel deleteProductViewModel)
        {
            try
            {
                await _productServices.DeleteAsync(new Guid(deleteProductViewModel.Id.ToString()));
                TempData["success"] = "Produit supprimé avec succès!";
                return RedirectToAction(nameof(ProductIndex));
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Erreur lors de la suppression du produit : {ex.Message}";
                return RedirectToAction(nameof(ProductIndex));
            }
        }
    }
}
