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
    [Route("api/CustomRecord")]
    [ApiController]
    public class CustomRecordController : ControllerBase
    {
        readonly PersonalContext _personalContext;
        readonly IHttpContextAccessor _httpContextAccessor;
        readonly IMapper _mapper;
        public CustomRecordController(PersonalContext personalContext, IHttpContextAccessor accessor, IMapper mapper)
        {
            _personalContext = personalContext ?? throw new ArgumentNullException(nameof(personalContext));
            _httpContextAccessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet(template: "list"), Authorize]
        public IActionResult getCustomRecordList(string filter)
        {
            List<CustomRecord> recordlist = new List<CustomRecord>();
            recordlist = (from record in _personalContext.CustomRecord
                        select record).ToList();

            if (!string.IsNullOrEmpty(filter))
            {
                string[] filters = filter.Trim(',').Split(",");
                DateTime customDate = DateTime.Now;
                foreach (string item in filters)
                {
                    if (item.Contains("customDate"))
                    {
                        customDate = Convert.ToDateTime(item.Split(':')[1]);
                    }
                }
                foreach (string item in filters)
                {
                    if (item.Contains("dateType"))
                    {
                        switch (item.Split(':')[1])
                        {
                            case "年":
                                recordlist = recordlist.Where(x => x.CustomDate.Value.Year == customDate.Year).ToList();
                                break;
                            case "月":
                                recordlist = recordlist.Where(x => x.CustomDate.Value.Year == customDate.Year& x.CustomDate.Value.Month == customDate.Month).ToList();
                                break;
                            case "日":
                                recordlist = recordlist.Where(x => x.CustomDate.Value.Date == customDate.Date).ToList();
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            recordlist = recordlist.OrderByDescending(x => x.CustomDate).ThenByDescending(x => x.CreatedOn).ToList();

            List<CustomRecordDto> listdto = new List<CustomRecordDto>();
            if (recordlist!=null&& recordlist.Count>0)
            {
                foreach (CustomRecord item in recordlist)
                {
                    CustomRecordDto dto = new CustomRecordDto();
                    dto = _mapper.Map<CustomRecordDto>(item);
                    dto.payName = (from type in _personalContext.PayType
                                   where type.PayTypeId == item.PayTypeId
                                   select type).FirstOrDefault().Name;
                    dto.customName = (from type in _personalContext.CustomType
                                   where type.CustomTypeId == item.CustomTypeId
                                   select type).FirstOrDefault().Name;
                    listdto.Add(dto);
                }
            }

            return Ok(listdto);
        }

        [HttpGet(template: "{CustomRecordId}"), Authorize]
        public IActionResult getCustomRecordById(string CustomRecordId)
        {
            CustomRecord customrecord = new CustomRecord();
            customrecord = (from record in _personalContext.CustomRecord
                       where record.CustomRecordId == new Guid(CustomRecordId)
                       select record).FirstOrDefault();
            if (customrecord!=null)
            {
                return Ok(customrecord);
            }
            else
            {
                return BadRequest("该数据不存在，请重试");
            }
        }

        [HttpPost(template:"save"),Authorize]
        public IActionResult saveCustomRecord(CustomRecord record)
        {
            if (_personalContext.CustomRecord.Contains(record))
            {
                _personalContext.Update(record);
            }
            else
            {
                record.CreatedOn = DateTime.Now;
                record.UserId = new Guid(_httpContextAccessor.HttpContext.User.FindFirst("userid").Value);
                _personalContext.CustomRecord.Add(record);
            }
            _personalContext.SaveChanges();
            return Ok();
        }

        [HttpPost(template: "delete"), Authorize]
        public IActionResult deleteCustomRecord(CustomRecord record)
        {
            if (_personalContext.CustomRecord.Contains(record))
            {
                _personalContext.CustomRecord.Remove(record);
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