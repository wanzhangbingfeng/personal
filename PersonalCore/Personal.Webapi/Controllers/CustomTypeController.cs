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
    [Route("api/CustomType")]
    [ApiController]
    public class CustomTypeController : ControllerBase
    {
        readonly PersonalContext _personalContext;
        readonly IHttpContextAccessor _httpContextAccessor;
        public CustomTypeController(PersonalContext personalContext, IHttpContextAccessor accessor )
        {
            _personalContext = personalContext ?? throw new ArgumentNullException(nameof(personalContext));
            _httpContextAccessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        }     

        [HttpGet(template: "list"), Authorize]
        public IActionResult getCustomType()
        {
            List<CustomType> typelist = new List<CustomType>();
            typelist =(from type in _personalContext.CustomType
                       where type.State==false
                       select type).ToList();
            return Ok(typelist);
        }

        [HttpGet(template: "{CustomTypeID}"), Authorize]
        public IActionResult getCustomType(string CustomTypeID)
        {
            CustomType customType = new CustomType();
            customType = (from type in _personalContext.CustomType
                        where type.CustomTypeId==new Guid(CustomTypeID)
                        select type).FirstOrDefault();
            return Ok(customType);
        }

        [HttpPost(template:"save"), Authorize]
        public IActionResult saveCustomType(CustomType type)
        {
            if (_personalContext.CustomType.Contains(type))
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

        [HttpPost(template:"delete"), Authorize]
        public IActionResult deleteCustomType(CustomType type)
        {
            if (_personalContext.CustomType.Contains(type))
            {
                _personalContext.CustomType.Remove(type);
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