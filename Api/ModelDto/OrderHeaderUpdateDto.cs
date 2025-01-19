namespace Api.ModelDto
{
    public class OrderHeaderUpdateDto
    {
        public int OrderHeaderId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }

        public string Status { get; set; }
    }
}
