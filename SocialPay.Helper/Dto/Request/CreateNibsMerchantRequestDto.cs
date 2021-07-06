namespace SocialPay.Helper.Dto.Request
{
    public class CreateNibsMerchantRequestDto
    {
        public string Name { get; set; }
        public string Tin { get; set; }
        public string Contact { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public decimal Fee { get; set; }
    }
}
