﻿using SocialPay.Domain.Entities;
using System;
using System.Linq;

namespace SocialPay.Domain
{
    public class DataSeeder
    {
        public static void Seed(SocialPayDbContext _context)
        {
            //if (!_context.ClientAuthentication.Any(x => x.Email == "festus.patrick@sterling.ng"))
            //{
            //    var createuser = new ClientAuthentication[]
            //    {
            //           new ClientAuthentication { FullName = "Festus Patrick", Email= "festus.patrick@sterling.ng",
            //          IsDeleted = false, PhoneNumber = "08030523654", LastDateModified = DateTime.Now, UserName = "patricksf",
            //          RoleName ="Super Administrator", StatusCode = "00"
            //          },
            //    };
            //    _context.ClientAuthentication.AddRange(createuser);
            //    _context.SaveChanges();
            //}

            if (!_context.ClientAuthentication.Any(x => x.Email == "oluwasegun.olaleye@sterling.ng"))
            {
                var createuser = new ClientAuthentication[]
                {
                       new ClientAuthentication { FullName = "oluwasegun olaleye", Email= "oluwasegun.olaleye@sterling.ng",
                      IsDeleted = false, PhoneNumber = "07059999663", LastDateModified = DateTime.Now, UserName = "olaleyeo",
                      RoleName ="Super Administrator", StatusCode = "00"
                      },
                };
                _context.ClientAuthentication.AddRange(createuser);
                _context.SaveChanges();
            }


            if (!_context.ClientAuthentication.Any(x => x.Email == "abayomi.sawyerr@sterling.ng"))
            {
                var createuser = new ClientAuthentication[]
                {
                       new ClientAuthentication { FullName = "abayomi sawyerr", Email= "abayomi.sawyerr@sterling.ng",
                      IsDeleted = false, PhoneNumber = "08078272823", LastDateModified = DateTime.Now, UserName = "sawyerra",
                      RoleName ="Super Administrator", StatusCode = "00"
                      },
                };
                _context.ClientAuthentication.AddRange(createuser);
                _context.SaveChanges();
            }

            if (!_context.ClientAuthentication.Any(x => x.Email == "Pelumi.Majolagbe@sterling.ng"))
            {
                var createuser = new ClientAuthentication[]
                {
                         new ClientAuthentication { FullName = "Pelumi Majolagbe", Email= "Pelumi.Majolagbe@sterling.ng",
                        IsDeleted = false, PhoneNumber = "08045646334", LastDateModified = DateTime.Now, UserName = "majolagbeip",
                        RoleName ="Super Administrator", StatusCode = "00"
                        },
                };
                _context.ClientAuthentication.AddRange(createuser);
                _context.SaveChanges();
            }
        }
    }
}
