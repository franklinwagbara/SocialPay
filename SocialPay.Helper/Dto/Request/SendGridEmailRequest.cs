using Newtonsoft.Json;
using System.Collections.Generic;

namespace SocialPay.Helper.Dto.Request
{
    public class SendGridEmailRequest
    {
        public List<Personalization> personalizations { get; set; }
        public List<Content> content { get; set; }
        public From from { get; set; }
        public ReplyTo reply_to { get; set; }
        public List<Attachments> attachments { get; set; }
        [JsonProperty("cc")]
        public List<CC> cc { get; set; }
    }

    public class To
    {
        public string email { get; set; }
        public string name { get; set; }
    }

    public class Personalization
    {
        public List<To> to { get; set; }
        public string subject { get; set; }
    }

    public class Content
    {
        public string type { get; set; }
        public string value { get; set; }
    }

    public class From
    {
        public string email { get; set; }
        public string name { get; set; }
    }

    public class ReplyTo
    {
        public string email { get; set; }
        public string name { get; set; }
    }

    public class CC
    {
        public string email { get; set; }
        public string name { get; set; }
    }


    public class Attachments
    {
        public string content { get; set; }
        public string type { get; set; }
        public string filename { get; set; }
        public string disposition { get; set; }
        public string content_id { get; set; }
    }
}
