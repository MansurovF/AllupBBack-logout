using System.ComponentModel.DataAnnotations;

namespace BigBackEnd.Models
{
    public class Tag : BaseEntity
    {
        [StringLength(255)]
        public string Name { get; set; }
        public IEnumerable<ProductTag> ProductTags { get; set; }

    }
}
