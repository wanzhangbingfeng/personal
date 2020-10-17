using System;
using System.Collections.Generic;

namespace Personal.Webapi.Entity
{
    public partial class CustomRecord
    {
        public Guid CustomRecordId { get; set; }
        public Guid? CustomTypeId { get; set; }
        public Guid? PayTypeId { get; set; }
        public decimal? Amount { get; set; }
        public string Describe { get; set; }
        public string Remark { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Guid? UserId { get; set; }
        public DateTime? CustomDate { get; set; }
    }

    public partial class CustomRecordDto
    {
        public Guid CustomRecordId { get; set; }
        public string payName { get; set; }
        public string customName { get; set; }
        public string Amount { get; set; }
        public string Describe { get; set; }
        public string UserId { get; set; }
        public string CustomDate { get; set; }
        public string CreatedOn { get; set; }
    }

    public class CustomRecordReport
    {
        public string payName { get; set; }
        public string customName { get; set; }
        public double Amount { get; set; }

        /// <summary>
        /// 消费次数
        /// </summary>
        public int Cout { get; set; }
    }
}
