using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Model
{
    public class OrderHeader
    {
        [Key]
        public int OrderHeaderId { get; set; }
        [Required]
        public string CustomerName { get; set; }
        [Required]
        public string CustomerEmail { get; set; }

        public string AppUserId { get; set; }
        [ForeignKey("AppUserId")]
        public AppUser User { get; set; }
        public double OrderTotalAmount { get; set; }

        public DateTime OrderDateTime { get; set; }
        public string Status { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<OrderDetails> OrderDetailItems { get; set; }
    }
}
