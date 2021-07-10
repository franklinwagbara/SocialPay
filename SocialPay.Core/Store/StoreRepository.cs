using SocialPay.ApplicationCore.Interfaces.Service;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Store
{
    public class StoreRepository
    {
        private readonly IStoreService _storeService;

        public StoreRepository(IStoreService storeService)
        {
            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));
        }

        public async Task<WebApiResponse> CreateNewStoreAsync(StoreRequestDto request, long clentId)
        {
            try
            {
                var model = new StoreViewModel
                {
                    Description = request.Description,
                    ClientAuthenticationId = clentId,
                    StoreName = request.StoreName,
                    FileLocation = "",
                };

                await _storeService.AddAsync(model);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Store was successfully created" };
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
    }
}
