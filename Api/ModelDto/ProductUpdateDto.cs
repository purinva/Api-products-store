using System.ComponentModel.DataAnnotations;

namespace Api.ModelDto
{
    public class ProductUpdateDto
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public string SpecialTag { get; set; }
        public string Category { get; set; }
        [Range(1, 1000)]
        public double Price { get; set; }
        public IFormFile Image { get; set; }
    }
}
