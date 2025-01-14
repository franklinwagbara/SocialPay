﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class OnboardingNotiification : BaseEntity
    {
        public long OnboardingNotiificationId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string notificationType { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
