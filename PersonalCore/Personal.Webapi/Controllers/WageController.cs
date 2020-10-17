using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Personal.Webapi.Entity;

namespace Personal.Webapi.Controllers
{
    [Route("api/wages")]
    [ApiController]
    public class WageController : ControllerBase
    {
        readonly PersonalContext _personalContext;
        readonly IHttpContextAccessor _httpContextAccessor;
        readonly IMapper _mapper;
        public WageController(PersonalContext personalContext, IHttpContextAccessor accessor, IMapper mapper)
        {
            _personalContext = personalContext ?? throw new ArgumentNullException(nameof(personalContext));
            _httpContextAccessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet(template: "list"), Authorize]
        public IActionResult getWageList(string filter)
        {
            List<Wages> wagesList = new List<Wages>();
            wagesList = (from wage in _personalContext.Wages
                          select wage).ToList();

            if (!string.IsNullOrEmpty(filter))
            {
                string[] filters = filter.Trim(',').Split(",");
                DateTime customDate = DateTime.Now;
                foreach (string item in filters)
                {
                    if (item.Contains("wageDate"))
                    {
                        customDate = Convert.ToDateTime(item.Split(':')[1]);
                    }
                }
                wagesList = wagesList.Where(x => x.WagesDate.Year == customDate.Year).OrderBy(x=>x.WagesDate).ToList();
            }
            List<WagesDto> dtolist = new List<WagesDto>();
            if (wagesList!=null&&wagesList.Count>0)
            {
                foreach (Wages item in wagesList)
                {
                    WagesDto dto = new WagesDto();
                    dto = _mapper.Map<WagesDto>(item);
                    dtolist.Add(dto);
                }
            }
            return Ok(dtolist);
        }

        [HttpGet(template: "{WagesId}"), Authorize]
        public IActionResult getWagesById(string WagesId)
        {
            Wages wages = new Wages();
            wages = (from wage in _personalContext.Wages
                            where wage.WagesId == new Guid(WagesId)
                            select wage).FirstOrDefault();
            if (wages != null)
            {
                return Ok(wages);
            }
            else
            {
                return BadRequest("该数据不存在，请重试");
            }
        }


        [HttpPost(template: "save"), Authorize]
        public IActionResult saveWages(Wages wage)
        {
            if (_personalContext.Wages.Contains(wage))
            {
                _personalContext.Update(wage);
            }
            else
            {
                wage.CreatedOn = DateTime.Now;
                wage.UserId = new Guid(_httpContextAccessor.HttpContext.User.FindFirst("userid").Value);
                _personalContext.Wages.Add(wage);
            }
            _personalContext.SaveChanges();
            return Ok();
        }

        [HttpPost(template: "delete"), Authorize]
        public IActionResult deleteWages(Wages wages)
        {
            if (_personalContext.Wages.Contains(wages))
            {
                _personalContext.Wages.Remove(wages);
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