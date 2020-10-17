using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Personal.Webapi.Entity;

namespace Personal.Webapi.Controllers
{
    [Route("api/report")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        readonly PersonalContext _personalContext;
        readonly IHttpContextAccessor _httpContextAccessor;
        readonly IMapper _mapper;
        public ReportController(PersonalContext personalContext, IHttpContextAccessor accessor, IMapper mapper)
        {
            _personalContext = personalContext ?? throw new ArgumentNullException(nameof(personalContext));
            _httpContextAccessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet(template: "CustomReport"), AllowAnonymous, Description("消费记录报表")]
        public IActionResult getCustomReport(int groupType, int dattetype, string date)
        {
            //查询所有的消费记录
            List<CustomRecord> recordlist = new List<CustomRecord>();
            recordlist = (from record in _personalContext.CustomRecord
                          select record).ToList();
            DateTime customDate = DateTime.Now;
            DateTime.TryParse(date, out customDate);
            //筛选特定时间数据
            switch (dattetype)
            {
                case 1://年
                    recordlist = recordlist.Where(x => x.CustomDate.Value.Year == customDate.Year).ToList();
                    break;
                case 2://月
                    recordlist = recordlist.Where(x => x.CustomDate.Value.Year == customDate.Year & x.CustomDate.Value.Month == customDate.Month).ToList();
                    break;
                case 3://日
                    recordlist = recordlist.Where(x => x.CustomDate.Value.Date == customDate.Date).ToList();
                    break;
                default:
                    break;
            }
            List<CustomRecordReport> reportlist = new List<CustomRecordReport>();

            //数据分组
            switch (groupType)
            {
                case 1://完整
                    var list = recordlist.GroupBy(x => new { x.PayTypeId, x.CustomTypeId })
                         .Select(p => new
                         {
                             p.Key.PayTypeId,
                             p.Key.CustomTypeId,
                             total = p.Sum(w => w.Amount),
                             count = p.Count()
                         }).ToList();
                    if (list != null && list.Count > 0)
                    {
                        foreach (var item in list)
                        {
                            CustomRecordReport report = new CustomRecordReport();
                            report.Amount = double.Parse(item.total.Value.ToString("0.00"));
                            report.payName = (from type in _personalContext.PayType
                                              where type.PayTypeId == item.PayTypeId
                                              select type).FirstOrDefault().Name;
                            report.customName = (from type in _personalContext.CustomType
                                                 where type.CustomTypeId == item.CustomTypeId
                                                 select type).FirstOrDefault().Name;
                            report.Cout = item.count;
                            reportlist.Add(report);
                        }
                    }
                    break;
                case 2://消费类型
                    var list2 = recordlist.GroupBy(x => new { x.CustomTypeId })
                         .Select(p => new
                         {
                             p.Key.CustomTypeId,
                             total = p.Sum(w => w.Amount),
                             count = p.Count()
                         }).ToList();
                    if (list2 != null && list2.Count > 0)
                    {
                        foreach (var item in list2)
                        {
                            CustomRecordReport report = new CustomRecordReport();
                            report.Amount = double.Parse(item.total.Value.ToString("0.00"));
                            report.customName = (from type in _personalContext.CustomType
                                                 where type.CustomTypeId == item.CustomTypeId
                                                 select type).FirstOrDefault().Name;
                            report.Cout = item.count;
                            reportlist.Add(report);
                        }
                    }
                    break;
                case 3://支付方式
                    var list3 = recordlist.GroupBy(x => new { x.PayTypeId })
                         .Select(p => new
                         {
                             p.Key.PayTypeId,
                             total = p.Sum(w => w.Amount),
                             count = p.Count()
                         }).ToList();
                    if (list3 != null && list3.Count > 0)
                    {
                        foreach (var item in list3)
                        {
                            CustomRecordReport report = new CustomRecordReport();
                            report.Amount = double.Parse(item.total.Value.ToString("0.00"));
                            report.payName = (from type in _personalContext.PayType
                                              where type.PayTypeId == item.PayTypeId
                                              select type).FirstOrDefault().Name;
                            report.Cout = item.count;
                            reportlist.Add(report);
                        }
                    }
                    break;
                default:
                    break;
            }
            //统计
            CustomRecordReport total = new CustomRecordReport();
            total.payName = "总计";
            total.customName = "总计";
            total.Amount = double.Parse(reportlist.Sum(x => x.Amount).ToString("0.00"));
            total.Cout = reportlist.Sum(x => x.Cout);
            reportlist.Add(total);

            return Ok(reportlist);
        }

        [HttpGet(template: "WagesReport"), Authorize,Description("工资报表")]
        public IActionResult getWagesReport(string date)
        {
            DateTime wageDate = DateTime.Now;
            DateTime.TryParse(date, out wageDate);
            List<Wages> wagelist = new List<Wages>();
            wagelist = (from wage in _personalContext.Wages
                        where wage.WagesDate.Year == wageDate.Year
                        orderby wage.WagesDate
                        select wage).ToList();

            List<WagesReport> listdto = new List<WagesReport>();
            if (wagelist != null && wagelist.Count > 0)
            {
                foreach (Wages item in wagelist)
                {
                    WagesReport dto = new WagesReport();
                    dto = _mapper.Map<WagesReport>(item);
                    listdto.Add(dto);
                }
            }

            //统计
            WagesReport total = new WagesReport();
            total.WagesDate = "总计";
            total.BaseWages = listdto.Sum(x => x.BaseWages);
            total.WorkDays = listdto.Sum(x => x.WorkDays);
            total.Subsidy = listdto.Sum(x => x.Subsidy);
            total.SocialSecurity = listdto.Sum(x => x.SocialSecurity);
            total.Tax = listdto.Sum(x => x.Tax);
            total.ReceiveWages = listdto.Sum(x => x.ReceiveWages);
            listdto.Add(total);

            return Ok(listdto);
        }
    }
}