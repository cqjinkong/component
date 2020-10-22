// ReSharper disable CheckNamespace
namespace Jinkong.Payment
{
    public class AddPrepayOrderInput
    {
        /// <summary>
        /// 业务单编号
        /// </summary>
        public string SourceSn { get; set; }

        /// <summary>
        /// 业务单来源类型
        /// </summary>
        public string SourceType { get; set; }
        /// <summary>
        /// 下单人员id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 下单人员姓名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 下单人员姓名
        /// </summary>
        public string UserPhone { get; set; }

        /// <summary>
        /// 下单人员扩展属性
        /// </summary>
        public string UserExtends { get; set; }

        /// <summary>
        /// 订单备注
        /// </summary>
        public string Remark { get; set; }
    }
}
