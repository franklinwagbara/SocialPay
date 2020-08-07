﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class ClientAuthentication
    {
        public long ClientAuthenticationId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public byte[] ClientSecretHash { get; set; }
        public byte[] ClientSecretSalt { get; set; }
        public string StatusCode { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; }
        public virtual ICollection<PinRequest> PinRequest { get; set; }
    }
}
