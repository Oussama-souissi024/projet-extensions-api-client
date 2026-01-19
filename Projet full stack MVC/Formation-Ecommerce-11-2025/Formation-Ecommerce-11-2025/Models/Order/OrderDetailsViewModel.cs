using System.ComponentModel.DataAnnotations;
using Formation_Ecommerce_11_2025.Models.Product;

namespace Formation_Ecommerce_11_2025.Models.Order
{
    /// <summary>
    /// Modèle de vue pour les détails d'une commande (lignes de commande)
    /// </summary>
    public class OrderDetailsViewModel
    {
        public Guid Id { get; set; }
        
        [Display(Name = "ID Commande")]
        public Guid OrderHeaderId { get; set; }
        
        [Display(Name = "ID Produit")]
        public Guid ProductId { get; set; }
        
        [Display(Name = "Nom du produit")]
        public string ProductName { get; set; }
        
        [Display(Name = "Prix")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Price { get; set; }
        
        [Display(Name = "Quantité")]
        public int Count { get; set; }
        
        // Propriété calculée pour le total de la ligne
        [Display(Name = "Total")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal TotalPrice => Price * Count;
        
        // Image du produit (optionnelle)
        public string ImageUrl { get; set; }
    }
}
