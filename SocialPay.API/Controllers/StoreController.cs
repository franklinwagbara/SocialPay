using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialPay.Core.Extensions.Common;
using SocialPay.Core.Store;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Notification;
using System;
using System.Threading.Tasks;

namespace SocialPay.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Merchant")]
    [Route("api/socialpay/store")]
    [ApiController]
    public class StoreController : BaseController
    {
        private readonly StoreRepository _storeRepository;

        public StoreController(StoreRepository storeRepository, INotification notification) : base(notification)
        {
            _storeRepository = storeRepository ?? throw new ArgumentNullException(nameof(storeRepository));
        }

        [HttpPost]
        [Route("create-store")]
        public async Task<IActionResult> CreateStore([FromForm] StoreRequestDto request) => Response(await _storeRepository.CreateNewStoreAsync(request, User.GetSessionDetails()).ConfigureAwait(false));

        //[AllowAnonymous]
        [HttpGet]
        [Route("get-store")]
        public async Task<IActionResult> GetStores() => Response(await _storeRepository.GetStoreInfoAsync(User.GetSessionDetails()).ConfigureAwait(false));

        [HttpPost]
        [Route("create-product-category")]
        public async Task<IActionResult> CreateProductCategoryAsync([FromBody] ProductcategoryDto request) => Response(await _storeRepository.CreateProductCategoryAsync(request, User.GetSessionDetails()).ConfigureAwait(false));

        [HttpGet]
        [Route("get-product-categories")]
        public async Task<IActionResult> GetProductCategories() => Response(await _storeRepository.GetProductCategoryAsync(User.GetSessionDetails()).ConfigureAwait(false));

        [HttpPost]
        [Route("create-product")]
        public async Task<IActionResult> CreateProducts([FromForm] ProductRequestDto request) => Response(await _storeRepository.CreateNewProductAsync(request, User.GetSessionDetails()).ConfigureAwait(false));

        [HttpGet]
        [Route("get-products")]
        public async Task<IActionResult> GetProducts([FromQuery] long storeId) => Response(await _storeRepository.GetProductsAsync(User.GetSessionDetails(), storeId).ConfigureAwait(false));

        [HttpGet]
        [Route("get-products-by-Id")]
        public async Task<IActionResult> GetProductsById([FromQuery] long productId) => Response(await _storeRepository.GetProductsByIdAsync(User.GetSessionDetails(), productId).ConfigureAwait(false));
    }
}
