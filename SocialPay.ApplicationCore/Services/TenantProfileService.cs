using AutoMapper;
using SocialPay.ApplicationCore.Interfaces.Repositories;
using SocialPay.ApplicationCore.Interfaces.Service;
using SocialPay.Domain.Entities;
using SocialPay.Helper.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.ApplicationCore.Services
{

    public class TenantProfileService : ITenantProfileService
    {
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<TenantProfile> _tenantProfile;

        public TenantProfileService(IAsyncRepository<TenantProfile> tenantProfile)
        {
            _tenantProfile = tenantProfile;

            var config = new MapperConfiguration(cfg => cfg.CreateMap<TenantProfile, TenantProfileViewModel>());

            _mapper = config.CreateMapper();
        }

        public async Task<List<TenantProfileViewModel>> GetAllAsync()
        {
            var tenants = await _tenantProfile.GetAllAsync();

            return _mapper.Map<List<TenantProfile>, List<TenantProfileViewModel>>(tenants);
        }

        public async Task<TenantProfileViewModel> GetProfileEmail(string email)
        {
            var tenant = await _tenantProfile.GetSingleAsync(x => x.Email == email);

            return _mapper.Map<TenantProfile, TenantProfileViewModel>(tenant);
        }

        public async Task<bool> ExistsAsync(long tenantId)
        {
            return await _tenantProfile.ExistsAsync(x => x.TenantProfileId == tenantId);
        }

        public async Task<bool> ExistsByEmailAsync(string email, string phoneNumber)
        {
            return await _tenantProfile.ExistsAsync(x => x.Email == email || x.PhoneNumber == phoneNumber);
        }

        public async Task<TenantProfileViewModel> AddAsync(TenantProfileViewModel model)
        {
            var entity = new TenantProfile
            {
               Address = model.Address,
               IsDeleted = false,
               Status = false,
               LastDateModified = DateTime.Now,
               ClientAuthenticationId = model.ClientAuthenticationId,
               Email = model.Email,
               PhoneNumber = model.PhoneNumber,
               WebSiteUrl = model.WebSiteUrl,
               TenantName = model.TenantName
            };

            await _tenantProfile.AddAsync(entity);

            return _mapper.Map<TenantProfile, TenantProfileViewModel>(entity);
        }

        public async Task UpdateAsync(TenantProfileViewModel model)
        {
            var entity = await _tenantProfile.GetSingleAsync(x => x.TenantProfileId == model.TenantProfileId);

            entity.TenantName = model.TenantName;
            entity.LastDateModified = DateTime.Now;

            await _tenantProfile.UpdateAsync(entity);
        }

        public async Task<int> CountTotalProfilesAsync()
        {
            return 1;
            // return await _clientAuthentication.CountAsync(x => x.AvailableFlag == true);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _tenantProfile.GetByIdAsync(id);

            await _tenantProfile.DeleteAsync(entity);
        }

    }

}
