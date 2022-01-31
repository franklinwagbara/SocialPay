using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class SpectaStageVerificationPinRequest : BaseEntity
    {
        public long SpectaStageVerificationPinRequestId { get; set; }
        public string Email { get; set; }
        public string Pin { get; set; }
        public bool Status { get; set; }
        public DateTime EnterDate { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; } = DateTime.Now;
    }
}
