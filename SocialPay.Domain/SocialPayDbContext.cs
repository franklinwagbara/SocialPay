using Microsoft.EntityFrameworkCore;
using SocialPay.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain
{
    public class SocialPayDbContext : DbContext
    {
        public SocialPayDbContext(DbContextOptions<SocialPayDbContext> options) : base(options)
        { }
        public DbSet<ClientAuthentication> ClientAuthentication { get; set; }
        public DbSet<PinRequest> PinRequest { get; set; }
    }
}
