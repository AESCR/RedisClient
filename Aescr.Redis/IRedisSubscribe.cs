namespace Aescr.Redis;

public interface IRedisSubscribe
{
    #region Redis 发布订阅 命令

    /// <summary>
    /// 订阅频道
    /// </summary>
    /// <param name="channel"></param>
    void AddChannel(params string[] channel);
    /// <summary>
    /// 取消订阅
    /// </summary>
    /// <param name="channel"></param>
    void RemoveChannel(params string[] channel);
    /// <summary>
    /// 用于退订给定的一个或多个频道的信息。
    /// </summary>
    /// <param name="channel">频道</param>
    /// <returns>这个命令在不同的客户端中有不同的表现。</returns>
    string[] Unsubscribe(params string[] channel);

    /// <summary>
    /// 用于订阅给定的一个或多个频道的信息。
    /// </summary>
    /// <param name="channel">频道</param>
    /// <returns>接收到的信息</returns>
    string[] Subscribe(params string[] channel);

    /// <summary>
    /// 用于查看订阅与发布系统状态，它由数个不同格式的子命令组成。
    /// </summary>
    /// <returns>由活跃频道组成的列表。</returns>
    string[] PubSub(string subCommand, params string[] argument);

    /// <summary>
    /// 用于退订所有给定模式的频道。
    /// </summary>
    /// <param name="pattern">频道</param>
    /// <returns>这个命令在不同的客户端中有不同的表现。</returns>
    string[] PunSubscribe(string pattern);

    /// <summary>
    /// 用于将信息发送到指定的频道。
    /// </summary>
    /// <param name="channel">频道</param>
    /// <param name="message">信息</param>
    /// <returns>接收到信息的订阅者数量。</returns>
    int Publish(string channel, string message);

    /// <summary>
    /// 订阅一个或多个符合给定模式的频道。
    /// </summary>
    /// <param name="pattern">每个模式以 * 作为匹配符，比如 it* 匹配所有以 it 开头的频道( it.news 、 it.blog 、 it.tweets 等等)。 news.* 匹配所有以 news. 开头的频道( news.it 、 news.global.today 等等)，诸如此类。</param>
    /// <returns>接收到的信息。</returns>
    string[] PSubscribe(params string[] pattern);

    #endregion Redis 发布订阅 命令
}