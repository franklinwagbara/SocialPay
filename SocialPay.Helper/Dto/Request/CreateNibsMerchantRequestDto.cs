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

    public class CreateNibbsSubMerchantDto
    {
        public string mchNo { get; set; }
        public string merchantName { get; set; }
        public string merchantEmail { get; set; }
        public string merchantPhoneNumber { get; set; }
        public string subFixed { get; set; }
        public string subAmount { get; set; }
    }

    public class BindMerchantRequestDto
    {
        public string mchNo { get; set; }
    }
}
