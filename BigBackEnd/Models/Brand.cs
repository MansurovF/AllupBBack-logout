using System.ComponentModel.DataAnnotations;

namespace BigBackEnd.Models
{
    public class Brand : BaseEntity
    {
        //[StringLength(255,ErrorMessage ="Qaqa 255-i vurub kecme")]
        //[Required(ErrorMessage ="Qaqa bos gorub kecme")]
        public string Name { get; set; }
        public IEnumerable<Product>? Products { get; set; }    

    }
}
