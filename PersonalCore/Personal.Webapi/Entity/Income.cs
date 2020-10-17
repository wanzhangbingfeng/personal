using System;
using System.Collections.Generic;

namespace Personal.Webapi.Entity
{
    public partial class Income
    {
        public Guid IncomeId { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public double? OldAmount { get; set; }
        public Guid? OperateId { get; set; }
        public Guid? PayTypeId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Guid? UserId { get; set; }
    }
}
