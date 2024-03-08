using System.ComponentModel.DataAnnotations;

namespace EcommerceWebApp.Models
{
    public class Category
    {
        [Key]
        public int CatId { get; set; }
        [Required]
        public String CatName { get; set; }

        
    }
}
