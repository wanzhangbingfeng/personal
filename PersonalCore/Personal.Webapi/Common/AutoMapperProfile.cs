using AutoMapper;
using Personal.Webapi.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Personal.Webapi.Common
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            //消费记录
            CreateMap<CustomRecordDto, CustomRecord>().ReverseMap()
                .ForMember(dto => dto.CustomDate, m => m.MapFrom(p => p.CustomDate.Value.ToShortDateString()))
                .ForMember(dto => dto.CreatedOn, m => m.MapFrom(p => p.CreatedOn.Value.ToString("yyyy-MM-dd hh:MM:ss")))
                .ForMember(dto=>dto.Amount,m=>m.MapFrom(p=>p.Amount));

            //消费记录报表
            CreateMap<CustomRecordReport, CustomRecord>().ReverseMap()
                .ForMember(dto => dto.Amount, m => m.MapFrom(p => p.Amount));

            //支付类型
            CreateMap<PickModel, PayType>().ReverseMap()
                .ForMember(dto => dto.name, m => m.MapFrom(p => p.Name))
                .ForMember(dto => dto.value, m => m.MapFrom(p => p.PayTypeId));

            //消费类型
            CreateMap<PickModel, CustomType>().ReverseMap()
                .ForMember(dto => dto.name, m => m.MapFrom(p => p.Name))
                .ForMember(dto => dto.value, m => m.MapFrom(p => p.CustomTypeId));

            //工资记录
            CreateMap<WagesDto, Wages>().ReverseMap()
                .ForMember(dto => dto.WagesDate, m => m.MapFrom(p => p.WagesDate.ToShortDateString()));

            //工资报表
            CreateMap<WagesReport, Wages>().ReverseMap()
                .ForMember(dto => dto.WagesDate, m => m.MapFrom(p => p.WagesDate.ToShortDateString()))
                .ForMember(dto => dto.Subsidy, m => m.MapFrom(p => p.Subsidy + p.Bonus))
                .ForMember(dto => dto.SocialSecurity, m => m.MapFrom(p => p.SocialSecurity + p.AccumulationFund));
        }
    }
}
