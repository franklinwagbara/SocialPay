using SocialPay.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocialPay.Domain
{
    public class DataSeeder
    {
        public static void Seed(SocialPayDbContext _context)
        {
              if (!_context.ClientAuthentication.Any(x => x.Email == "festus.patrick@sterling.ng"))
                {
                        var createuser = new ClientAuthentication[]
                        {
                         new ClientAuthentication { FullName = "Festus Patrick", Email= "festus.patrick@sterling.ng",
                        IsDeleted = false, PhoneNumber = "08030523654", LastDateModified = DateTime.Now, UserName = "patricksf",
                        RoleName ="Super Administrator", StatusCode = "00"
                        },
                        };
                        _context.ClientAuthentication.AddRange(createuser);
                        _context.SaveChanges();
                }
        }
    }
}
