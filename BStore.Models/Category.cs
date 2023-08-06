
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BStore.Models
{
    public class Category
    {
        
        public int CategoryId { get; set; }
        [Required]
        [DisplayName("Category Name")]
        [MaxLength(30)]
        public string CategorName { get; set; }
        [DisplayName("Display Order")]
        [Range(1, 50,ErrorMessage ="Display Order Must be between 1 - 50")]
        public int DisplayOrder { get; set; }

    }
}
