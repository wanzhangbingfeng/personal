using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Personal.Webapi.Entity;

namespace Personal.Webapi.Controllers
{
    [Route("api/PayType")]
    [ApiController]
    public class PayTypeController : ControllerBase
    {
        readonly PersonalContext _personalContext;
        readonly IHttpContextAccessor _httpContextAccessor;
        public PayTypeController(PersonalContext personalContext, IHttpContextAccessor accessor)
        {
            _personalContext = personalContext ?? throw new ArgumentNullException(nameof(personalContext));
            _httpContextAccessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        }

        [HttpGet(template: "list"), Authorize]
        public IActionResult getPayType()
        {
            List<PayType> typelist = new List<PayType>();
            typelist = (from type in _personalContext.PayType
                        select type).ToList();
            return Ok(typelist);
        }

        [HttpGet(template: "{PayTypeID}"), Authorize]
        public IActionResult getPayType(string PayTypeID)
        {
            PayType paytype = new PayType();
            paytype = (from type in _personalContext.PayType
                          where type.PayTypeId == new Guid(PayTypeID)
                          select type).FirstOrDefault();
            return Ok(paytype);
        }

        [HttpPost(template: "save"), Authorize]
        public IActionResult savePayType(PayType type)
        {
            if (_personalContext.PayType.Contains(type))
            {
                _personalContext.Update(type);
            }
            else
            {
                _personalContext.Add(type);
            }
            _personalContext.SaveChanges();
            return Ok();
        }

        [HttpPost(template: "delete"), Authorize]
        public IActionResult deletePayType(PayType type)
        {
            if (_personalContext.PayType.Contains(type))
            {
                _personalContext.PayType.Remove(type);
                _personalContext.SaveChanges();
                return Ok("删除成功");
            }
            else
            {
                return BadRequest("该数据不存在，请重试");
            }
        }
    }
}