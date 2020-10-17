using System;
using System.Collections.Generic;

namespace Personal.Webapi.Entity
{
    public partial class Wages
    {
        public Guid WagesId { get; set; }
        public DateTime WagesDate { get; set; }
        public decimal? BaseWages { get; set; }
        public decimal? WorkDays { get; set; }
        public decimal? PreTaxWages { get; set; }
        public decimal? LeaveDays { get; set; }
        public decimal? Subsidy { get; set; }
        public decimal? Bonus { get; set; }
        public decimal? LeaveAmont { get; set; }
        public decimal? SocialSecurity { get; set; }
        public decimal? AccumulationFund { get; set; }
        public decimal? TaxBase { get; set; }
        public decimal? Tax { get; set; }
        public decimal? ReceiveWages { get; set; }
        public Guid? UserId { get; set; }
        public DateTime? CreatedOn { get; set; }
    }

    public class WagesDto
    {
        public Guid WagesId { get; set; }
        public string WagesDate { get; set; }
        public decimal? ReceiveWages { get; set; }
    }

    public class WagesReport
    {
        public string WagesDate { get; set; }
        public decimal? BaseWages { get; set; }
        public decimal? WorkDays { get; set; }
        public decimal? Subsidy { get; set; }
        public decimal? SocialSecurity { get; set; }
        public decimal? Tax { get; set; }
        public decimal? ReceiveWages { get; set; }
    }
}
