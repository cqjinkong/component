using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Cap;

namespace Jinkong.Payment
{
    public static class Extensions
    {
        /// <summary>
        /// 统一支付回调，根据请求参数channel路由到不同的支付类处理
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static void UsePayNotify(this IApplicationBuilder app)
        {
            app.Map("/pay/notify", builder =>
            {
                builder.Run(async context =>
                {
                    //获取是哪个通道的回调
                    var channelStr = context.Request.Path.ToString().Trim('/');
                    if (string.IsNullOrEmpty(channelStr) || !Enum.TryParse(channelStr, true, out PrepayChannel channel))
                    {
                        //格式不对就直接返回404
                        context.Response.StatusCode = 404;
                        await context.Response.WriteAsync("");
                        return;
                    }

                    var payment = context.RequestServices.GetServices<IPayment>()
                        .FirstOrDefault(p => p.Channel == channel);
                    if (payment == null)
                    {
                        //没有这个通道也直接返回404
                        context.Response.StatusCode = 404;
                        await context.Response.WriteAsync("");
                        return;
                    }

                    Dictionary<string, string> form = null;
                    try
                    {
                        // form表单内容不一定有,还可能报错
                        form = context.Request.Form.ToDictionary(r => r.Key, r => r.Value.ToString());
                    }
                    catch
                    {
                        // ignored
                    }

                    //调用对应通道的回调方法处理请求数据
                    var result = await payment.PayResultNotify(
                        context.Request.Query?.Select(r => new KeyValuePair<string, string>(r.Key, r.Value)),
                        context.Request.Headers?.ToDictionary(r => r.Key, r => r.Value.ToString()),
                        form,
                        context.Request.Body
                    );
                    result.Channel = channel;
                    if (result.Success)
                    {
                        //成功支付则发送成功支付事件
                        var eventPublisher = context.RequestServices.GetService<IEventPublisher>();
                        await eventPublisher.PublishAsync(new PaySuccessInnerNotifyEvent(result));
                    }

                    //根据不同的通道可能给回调方返回不同的html结果
                    await context.Response.WriteAsync(await payment.GetPayResultResponse(result));
                });
            });

            app.Map("/refund/notify", builder =>
            {
                builder.Run(async context =>
                {
                    //获取是哪个通道的回调
                    var channelStr = context.Request.Path.ToString().Trim('/');
                    if (string.IsNullOrEmpty(channelStr) ||
                        !Enum.TryParse(channelStr, true, out PrepayChannel channel))
                    {
                        //格式不对就直接返回404
                        context.Response.StatusCode = 404;
                        await context.Response.WriteAsync("");
                        return;
                    }

                    var payment = context.RequestServices.GetServices<IPayment>()
                        .FirstOrDefault(p => p.Channel == channel);
                    if (payment == null)
                    {
                        //没有这个通道也直接返回404
                        context.Response.StatusCode = 404;
                        await context.Response.WriteAsync("");
                        return;
                    }

                    Dictionary<string, string> form = null;
                    try
                    {
                        // form表单内容不一定有,还可能报错
                        form = context.Request.Form.ToDictionary(r => r.Key, r => r.Value.ToString());
                    }
                    catch
                    {
                        // ignored
                    }


                    //调用对应通道的回调方法处理请求数据
                    var refundResult = await payment.RefundResultNotify(
                        context.Request.Query?.Select(r => new KeyValuePair<string, string>(r.Key, r.Value)),
                        context.Request.Headers?.ToDictionary(r => r.Key, r => r.Value.ToString()),
                        form,
                        context.Request.Body
                    );
                    if (refundResult.success)
                    {
                        //成功支付则发送成功支付事件
                        var eventPublisher = context.RequestServices.GetService<IEventPublisher>();
                        await eventPublisher.PublishAsync(new RefundResultInnerNotifyEvent(refundResult.refundDetail));
                    }

                    //根据不同的通道可能给回调方返回不同的html结果
                    await context.Response.WriteAsync(
                        await payment.GetRefundResultResponse(refundResult.success, refundResult.refundDetail));
                });
            });
        }
    }
}