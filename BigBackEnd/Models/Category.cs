using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BigBackEnd.Models
{
    public class Category : BaseEntity
    {
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(255)]
        public string? Image { get; set; }
        public bool isMain { get; set; }
        public Nullable<int> ParentId { get; set; }
        public Category? Parent { get; set; }
        [NotMapped]
        public IFormFile? File { get; set; }
        public IEnumerable<Category>? Children { get; set; }
        public IEnumerable<Product>? Products { get; set; }
    }
}
