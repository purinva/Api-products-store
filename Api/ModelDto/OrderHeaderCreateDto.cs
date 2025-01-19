using Api.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Api.ModelDto
{
    public class OrderHeaderCreateDto
    {
        [Required]
        public string CustomerName { get; set; }
        [Required]
        public string CustomerEmail { get; set; }

        public string AppUserId { get; set; }
        public double OrderTotalAmount { get; set; }

        public string Status { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<OrderDetailsCreateDto> OrderDetailsDto { get; set; }
    }
}
