using System;
using Shashlik.Pager;

// ReSharper disable CheckNamespace

namespace Jinkong.Payment
{
    /// <summary>
    /// 预付款订单查询
    /// </summary>
    public class PrepayOrderQueryInput : PageInput
    {
        /// <summary>
        /// 人员id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 查询关键字(姓名/SN/名称/手机号)
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public PrepayOrderStatus? Status { get; set; }

        /// <summary>
        /// 订单创建日期查询-开始
        /// </summary>
        public DateTime? BeginDate { get; set; }

        /// <summary>
        /// 订单创建日期查询-结束
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 业务单来源
        /// </summary>
        public string SourceType { get; set; }
    }
}