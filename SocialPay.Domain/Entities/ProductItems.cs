﻿using System;

namespace SocialPay.Domain.Entities
{
    public class ProductItems : BaseEntity
    {
        public long ProductItemsId { get; set; }
        public long ProductId { get; set; }
        public string FileLocation { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public Product Product { get; set; }
    }
}
