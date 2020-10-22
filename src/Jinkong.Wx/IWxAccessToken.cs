namespace Jinkong.Wx
{
    /// <summary>
    /// 微信数据获取接口
    /// </summary>
    public interface IWxAccessToken
    {
        /// <summary>
        /// 获取默认公众号api调用token
        /// </summary>
        string DefaultMpToken { get; }

        /// <summary>
        /// 根据应用名称过去api调用token
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        string MpToken(string appName);
    }
}