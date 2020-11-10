using Microsoft.Extensions.DependencyInjection;
using SocialPay.Domain.Entities;
using System;
using System.Linq;

namespace SocialPay.Domain.SeedData
{
    public static class Seeding
    {
        public static async void Initialize(IServiceProvider serviceProvider)
        {
            var _context = serviceProvider.GetRequiredService<SocialPayDbContext>();
            _context.Database.EnsureCreated();

            using (var transaction = _context.Database.BeginTransaction())
            {


                ////if (!_context.tblADLookups.Any())
                ////{
                ////    var addNewUser = new tblADLookup[]
                ////    {
                ////    new tblADLookup { Status = true, UserId = "adm_patricksf", tblRoleId = 1, CreatedBy = "adm_patricksf" },

                ////    };
                ////    _context.tblADLookups.AddRange(addNewUser);
                ////    _context.SaveChanges();
                ////    transaction.Commit();
                ////}


                if (!_context.ClientAuthentication.Any(x => x.Email == "festus.patrick@sterling.ng"))
                {
                    
                        var createuser = new ClientAuthentication[]
                        {
                         new ClientAuthentication { FullName = "Festus Patrick", Email= "festus.patrick@sterling.ng",
                        IsDeleted = false, PhoneNumber = "08030523654", LastDateModified = DateTime.Now,
                        RoleName ="Super Administrator", StatusCode = "00"
                        },
                        };
                        _context.ClientAuthentication.AddRange(createuser);
                        _context.SaveChanges();
                        transaction.Commit();
                    

                }
                // _context.tblAuths.RemoveRange(_context.tblAuths.ToArray());
                //await  _context.SaveChangesAsync();


            }
            ////if (!context.Items.Any())
            ////{
            ////    context.Items.Add(entity: new Item() { Name = "Green Thunder" });
            ////    context.Items.Add(entity: new Item() { Name = "Berry Pomegranate" });
            ////    context.Items.Add(entity: new Item() { Name = "Betty Crocker" });
            ////    context.Items.Add(entity: new Item() { Name = "Pizza Crust Mix" });

            ////    context.SaveChanges();
            ////}

            ////if (!context.Shoppings.Any())
            ////{
            ////    context.Shoppings.Add(entity: new Shopping() { Name = "Defualt" });
            ////}
        }
    }
}
