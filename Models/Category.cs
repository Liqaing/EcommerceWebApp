using System.ComponentModel.DataAnnotations;

namespace EcommerceWebAppProject.Models
{
    public class Category
    {
        [Key]
        public int CatId { get; set; }
        [Required(ErrorMessage = "Category name is required")]
        public string? CatName { get; set; }

        public ICollection<Product>? Products { get; set; }

    }
}
