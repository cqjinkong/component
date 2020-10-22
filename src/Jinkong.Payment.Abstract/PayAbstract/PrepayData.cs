using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Payment.PayAbstract
{
    public class PrepayData
    {
        public PrepayData()
        {
        }

        public PrepayData(string payData, string prepayOriginData)
        {
            PayData = payData;
            PrepayOriginData = prepayOriginData;
        }

        /// <summary>
        /// 用于支付的数据,根据不同的支付通道,格式不一样
        /// </summary>
        public string PayData { get; set; }

        /// <summary>
        /// 原始的预支付请求数据
        /// </summary>
        public string PrepayOriginData { get; set; }
    }
}
