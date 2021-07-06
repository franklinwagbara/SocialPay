using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialPay.Core.Interface;
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
      

    }
}
