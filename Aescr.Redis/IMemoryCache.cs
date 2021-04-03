using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aescr.Redis
{
    public interface IMemoryCache
    {
        /// <summary>
        /// 设置缓存的值
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns></returns>
        string AddCache(string value);
        /// <summary>
        /// 设置缓存的值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string AddCache(IEnumerable<string> value);
        /// <summary>
        /// 设置缓存的值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string AddCache(Dictionary<string, string> value);
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetCache(string key);
    }
}
