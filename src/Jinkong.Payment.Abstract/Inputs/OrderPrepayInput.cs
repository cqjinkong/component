using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Jinkong.Utils;
using Shashlik.Utils;
using Shashlik.Utils.Extensions;
// ReSharper disable CheckNamespace

namespace Jinkong.Payment
{
    public class OrderPrepayInput : IValidatableObject
    {
        /// <summary>
        /// 预付款单的编号
        /// </summary>
        [Required(ErrorMessage = "参数错误")]
        [MinLength(10, ErrorMessage = "参数错误")]
        [MaxLength(64, ErrorMessage = "参数错误")]
        public string PrepayOrderSn { get; set; }

        /// <summary>
        /// 支付通道,枚举PrePayChannel
        /// </summary>
        public PrepayChannel Channel { get; set; }

        /// <summary>
        /// 支付平台对应的appId
        /// </summary>
        [Required(ErrorMessage = "参数错误")]
        [MinLength(2, ErrorMessage = "参数错误")]
        [MaxLength(32, ErrorMessage = "参数错误")]
        public string AppId { get; set; }

        /// <summary>
        /// 前端各应用的用户id,微信支付为微信的openid(app/小程序/h5各不一样)
        /// </summary>
        [MinLength(5, ErrorMessage = "参数错误")]
        [MaxLength(128, ErrorMessage = "参数错误")]
        public string TraderId { get; set; }

        /// <summary>
        /// H5跳转URL(h5支付时必须)
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// ip地址(微信H5支付必须),前端通过功能的获取ip地址的接口获取自己的公网ip
        /// </summary>
        public string ClientIp { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Channel == PrepayChannel.WxJs && TraderId.IsNullOrWhiteSpace())
                // jsapi必须传openid
                yield return new ValidationResult("参数错误");
            //if ((Channel == Enums.PrePayChannel.WxApp || Channel == Enums.PrePayChannel.AliPayH5) && ClientIp.IsNullOrWhiteSpace())
            //    yield return new ValidationResult("参数错误");
            if (Channel == PrepayChannel.WxH5 &&
                (!ClientIp.IsMatch(Consts.Regexs.Ipv4) || RedirectUrl.IsNullOrWhiteSpace()))
            {
                yield return new ValidationResult("参数错误");
            }
        }
    }
}