using System;
using System.Collections.Generic;

namespace Common.Utility.Memory.Redis2
{
    internal interface IRedisCache : IDisposable
    {
        #region 添加字符串

        /// <summary>
        ///  获取字符串 默认过期时间永久
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="obj">值</param>
        /// <param name="overwrite">存在就重写</param>
        /// <returns></returns>
        bool Add<T>(string key, T obj, bool overwrite = false);

        /// <summary>
        /// 获取字符串 设置缓存时间
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="obj">值</param>
        /// <param name="timeSpan">缓存时长</param>
        /// <param name="relative">是否是相对时间缓存</param>
        /// <param name="overwrite">存在就重写</param>
        /// <returns></returns>
        bool Add<T>(string key, T obj, TimeSpan timeSpan, bool relative = false, bool overwrite = false);

        /// <summary>
        /// 添加字符串 生成随机Key返回
        /// </summary>
        /// <param name="obj">值</param>
        /// <param name="timeSpan">缓存时长</param>
        /// <param name="relative">是否是相对时间缓存</param>
        /// <param name="overwrite">存在就重写</param>
        /// <returns>返回键</returns>
        string Add<T>(T obj, TimeSpan timeSpan, bool relative = false, bool overwrite = false);

        /// <summary>
        ///  获取字符串  生成随机Key返回 默认过期时间永久
        /// </summary>
        /// <param name="obj">值</param>
        /// <param name="overwrite">存在就重写</param>
        /// <returns>返回键</returns>
        string Add<T>(T obj, bool overwrite = false);

        #endregion 添加字符串

        #region 添加哈希

        /// <summary>
        /// 将哈希表 key 中的字段 field 的值设为 value 。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="filed">字段</param>
        /// <param name="value">值</param>
        /// <param name="overwrite">存在就重写</param>
        /// <returns></returns>
        bool AddHash<T>(string key, string filed, T value, bool overwrite = false);

        /// <summary>
        /// 同时将多个 field-value (域-值)对设置到哈希表 key 中。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="kv">field-value集合</param>
        /// <param name="overwrite">存在就重写</param>
        /// <returns></returns>
        bool AddHash<T>(string key, Dictionary<string, T> kv, bool overwrite = false);

        #endregion 添加哈希

        #region 添加List

        /// <summary>
        /// 在List列表中添加一个或多个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool AddList<T>(string key, params T[] value);

        /// <summary>
        /// 为已存在的列表添加值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool AddList<T>(string key, T value);

        #endregion 添加List

        #region 添加集合

        /// <summary>
        /// 在无序集合中添加一个或多个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool AddSet<T>(string key, params T[] value);

        /// <summary>
        /// 在有序集合中添加一个或多个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool AddSortSet<T>(string key, params T[] value);

        #endregion 添加集合

        #region 判断是否存在

        /// <summary>
        /// 判断是否存在Key
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        bool Exists(string key);

        #endregion 判断是否存在

        #region 删除数据

        /// <summary>
        /// 删除redis数据
        /// </summary>
        /// <param name="keys">键集合</param>
        /// <returns></returns>
        bool DelKey(params string[] keys);

        /// <summary>
        /// 删除redis数据
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        bool DelKey(string key);

        /// <summary>
        /// 删除Hash字段
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="field">字段</param>
        /// <returns></returns>
        bool DelHashField(string key, string field);

        #endregion 删除数据

        #region 修改Key

        /// <summary>
        /// 修改 key 的名称
        /// </summary>
        /// <param name="key">要修改的键</param>
        /// <param name="newKey">修改后的键名称</param>
        /// <returns></returns>
        bool ReName(string key, string newKey);

        #endregion 修改Key

        #region 过期时间维护

        /// <summary>
        /// 设置过期时间
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="timeSpan">缓存时长</param>
        /// <returns></returns>
        bool Expire(string key, TimeSpan timeSpan);

        /// <summary>
        /// 设置过期时间
        /// </summary>
        /// <param name="timeSpan">缓存时长</param>
        /// <param name="keys">键集合</param>
        /// <returns></returns>
        bool Expire(string[] keys, TimeSpan timeSpan);

        /// <summary>
        /// 移除 key 的过期时间，key 将持久保持。
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        bool Persist(string[] keys);

        /// <summary>
        /// 移除 key 的过期时间，key 将持久保持。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Persist(string key);

        #endregion 过期时间维护

        #region 自动获取值

        /// <summary>
        /// 自动设别建值类型转化
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        T Get<T>(string key);

        #endregion 自动获取值

        #region 获取字符串

        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        T GetString<T>(string key);

        /// <summary>
        /// 获取字符串集合
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        T[] GetStrings<T>(params string[] keys);

        #endregion 获取字符串

        #region 获取哈希

        /// <summary>
        /// 获取全部字段
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        string[] GetHashFileds(string key);

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        string GetHashValue(string key, string files);

        /// <summary>
        /// 获取所有给定字段的值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="files">字段集合</param>
        /// <returns></returns>
        string[] GetHashValues(string key, params string[] files);

        #endregion 获取哈希
    }
}