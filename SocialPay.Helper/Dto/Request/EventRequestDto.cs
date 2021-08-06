namespace SocialPay.Helper.Dto.Request
{
    public class EventRequestDto
    {
        public long ClientAuthenticationId { get; set; }
        public string UserId { get; set; }
        public string IpAddress { get; set; }
        public string ModuleAccessed { get; set; }
        public string Description { get; set; }
    }
}
