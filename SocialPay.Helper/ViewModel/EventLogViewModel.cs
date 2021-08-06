using System;

namespace SocialPay.Helper.ViewModel
{
    public class EventLogViewModel
    {
        public long EventLogId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string UserId { get; set; }
        public string IpAddress { get; set; }
        public string ModuleAccessed { get; set; }
        public string Description { get; set; }
        public DateTime DateEntered { get; set; }
    }
}
