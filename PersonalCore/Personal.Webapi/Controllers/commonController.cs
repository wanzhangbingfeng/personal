using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Personal.Webapi.Common;
using Personal.Webapi.Entity;

namespace Personal.Webapi.Controllers
{
    [Route("api/common")]
    [ApiController]
    public class commonController : ControllerBase
    {
        readonly PersonalContext _personalContext;
        readonly IHttpContextAccessor _httpContextAccessor;
        readonly IMapper _mapper;
        public commonController(PersonalContext personalContext, IHttpContextAccessor accessor, IMapper mapper)
        {
            _personalContext = personalContext ?? throw new ArgumentNullException(nameof(personalContext));
            _httpContextAccessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet(template: "{type}"), Authorize]
        public IActionResult getPicklist(string type)
        {
            List<PickModel> list = new List<PickModel>();
            switch (type)
            {
                case "paytype":
                    List<PayType> typelist = new List<PayType>();
                    typelist = (from pay in _personalContext.PayType
                                where pay.State == false
                                select pay).ToList();
                    if (typelist.Count>0)
                    {
                        foreach (PayType item in typelist)
                        {
                            PickModel model = new PickModel();
                            model = _mapper.Map<PickModel>(item);
                            list.Add(model);
                        }
                    }
                    break;
                case "customtype":
                    List<CustomType> customlist = new List<CustomType>();
                    customlist = (from custom in _personalContext.CustomType
                                where custom.State == false
                                select custom).ToList();
                    if (customlist.Count > 0)
                    {
                        foreach (CustomType item in customlist)
                        {
                            PickModel model = new PickModel();
                            model = _mapper.Map<PickModel>(item);
                            list.Add(model);
                        }
                    }
                    break;
                default:
                    break;
            }
            return Ok(list);
        }
    }
}