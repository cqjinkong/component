﻿using System;
using System.IO;
using Jinkong.Wx.AspNetCore.Notifies;
using Microsoft.AspNetCore.Http;
using Senparc.NeuChar;
using Senparc.NeuChar.App.AppStore;
using Senparc.NeuChar.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MessageContexts;
using Senparc.Weixin.MP.MessageHandlers;

namespace Jinkong.Wx.AspNetCore
{
    //TODO: 使用条件属性进行筛选

    public class WxMessageHandler : MessageHandler<DefaultMpMessageContext>
    {
        HttpContext httpContext { get; }
        public WxMessageHandler(
            HttpContext httpContext,
            Stream requestStream,
            PostModel postModel,
            int maxRecordCount = 0,
            DeveloperInfo developerInfo = null) : base(requestStream, postModel, maxRecordCount, developerInfo: developerInfo)
        {
            this.httpContext = httpContext;
        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            Type interfaceType;
            if (requestMessage.MsgType != RequestMsgType.Event)
                interfaceType = typeof(IWxMsgNotify<>).MakeGenericType(requestMessage.GetType());
            else
                interfaceType = typeof(IWxEventNotify<>).MakeGenericType(requestMessage.GetType());

            // 回调处理
            var callback = httpContext
                                .RequestServices
                                .GetService(interfaceType);
            ResponseMessageBase res;
            if (callback == null)
                res = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageNoResponse>(requestMessage);
            else
                res = interfaceType.GetMethod("Handle").Invoke(callback, new[] { requestMessage }) as ResponseMessageBase;

            if (res == null)
                res = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageNoResponse>(requestMessage);
            return res;
        }
    }
}
