using System.ComponentModel.DataAnnotations;

namespace EcommerceWebApp.Models
{
    public class Category
    {
        [Key]
        public int CatId { get; set; }
        [Required (ErrorMessage = "Category name is required")]
        public String CatName { get; set; }

    }
}
