using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Model
{
    public class CartItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int ShoppingCartID { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}
