namespace SocialPay.Helper.Dto.Request
{
    public class DefaultMerchantRequestDto
    {
        public string Contact { get; set; }
        public string Address { get; set; }
        public double Fee { get; set; }
    }

    public class CreateNibsMerchantRequestDto : DefaultMerchantRequestDto
    {
        public string Name { get; set; }
        public string Tin { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

    }

    public class DefaultSubMerchantRequestDto
    {
        public string subFixed { get; set; }
        public string subAmount { get; set; }
    }

    public class CreateNibbsSubMerchantDto : DefaultSubMerchantRequestDto
    {
        public string mchNo { get; set; }
        public string merchantName { get; set; }
        public string merchantEmail { get; set; }
        public string merchantPhoneNumber { get; set; }
    }

    public class BindMerchantRequestDto
    {
        public string mchNo { get; set; }
        public string bankNo { get; set; }
        public string accountName { get; set; }
        public string accountNumber { get; set; }
    }


    //NIP API update

    public class CreateSubMerchant
    {
        public string mchNo { get; set; }
        public string merchantName { get; set; }
        public string merchantEmail { get; set; }
        public string merchantPhoneNumber { get; set; }
        public string subFixed { get; set; }
        public string subAmount { get; set; }
    }

    public class NewCreateNibsMerchantRequestDto
    {
        public string name { get; set; }
        public string tin { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string bankCode { get; set; }
        public string accountName { get; set; }
        public string accountNumber { get; set; }
        public string feeBearer { get; set; }
        public string contact { get; set; }
    }


    public class QueryAccountRequestDto
    {
        public string bankNumber { get; set; }
        public string accountNumber { get; set; }
    }

    public class createMerchantRequestPayload
    {
        public QueryAccountRequestDto QueryAccountRequestDto { get; set; }
        public NewCreateNibsMerchantRequestDto NewCreateNibsMerchantRequestDto { get; set; }
    }



}
