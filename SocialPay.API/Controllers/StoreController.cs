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
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly IStore _service;
        public StoreController(IStore service)
        {
            _service = service;
        }

        //public async Task<ActionResult> AddStore(MerchantStoreDto merchant)
        //{
        //    var result = await _service.CreateStore(merchant);
        //    if (result.StatusCode.Equals("00"))
        //        return Ok(result);
        //    return NotFound(result);
        //}

        //[HttpPost]
        //public async Task<ActionResult> AddCategory(StoreCategoryDto storeCategory)
        //{
        //    var result = await _service.CreateCategory(storeCategory);
        //    if (result.StatusCode.Equals("00"))
        //        return Ok(result);
        //    return NotFound(result);
        //}
    }
}
