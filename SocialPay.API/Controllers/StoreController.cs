using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialPay.Core.Interface;
using SocialPay.Helper.Dto.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialPay.API.Controllers
{
    [Route("api/socialpay/store")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly IStore _service;
        public StoreController(IStore service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("add-to-store")]
        public async Task<ActionResult> AddStore([FromBody] MerchantStoreDto merchant)
        {
            var result = await _service.CreateStore(merchant);

            if (result.StatusCode.Equals("00"))
                return Ok(result);

            return NotFound(result);
        }

        [HttpPost]
        [Route("add-store-category")]
        public async Task<ActionResult> AddCategory([FromBody] StoreCategoryDto storeCategory)
        {
            var result = await _service.CreateCategory(storeCategory);

            if (result.StatusCode.Equals("00"))
                return Ok(result);

            return NotFound(result);
        }
    }
}
