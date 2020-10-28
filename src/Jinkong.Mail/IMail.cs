namespace Jinkong.Mail
{
    public interface IMail
    {
        /// <summary>
        /// 发送普通HTML邮件
        /// </summary>
        /// <param name="address"></param>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        void Send(string address, string subject, string content);

        /// <summary>
        /// 发送频率限制邮件
        /// </summary>
        /// <param name="address"></param>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        void LimitSend(string address, string subject, string content);

        /// <summary>
        /// 频率限制，判断是否可以发送
        /// </summary>
        /// <param name="address"></param>
        /// <param name="subject"></param>
        /// <returns></returns>
        bool LimitCheck(string address, string subject);

    }
}
