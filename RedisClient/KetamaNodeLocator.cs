using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace RedisClient
{
    public class HashAlgorithm
    {
        public static byte[] ComputeMd5(string k)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            byte[] keyBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(k));
            md5.Clear();
            //md5.update(keyBytes);
            //return md5.digest();
            return keyBytes;
        }

        public static long Hash(byte[] digest, int nTime)
        {
            long rv = ((long)(digest[3 + nTime * 4] & 0xFF) << 24)
                      | ((long)(digest[2 + nTime * 4] & 0xFF) << 16)
                      | ((long)(digest[1 + nTime * 4] & 0xFF) << 8)
                      | ((long)digest[0 + nTime * 4] & 0xFF);

            return rv & 0xffffffffL; /* Truncate to 32-bits */
        }
    }
    public class KetamaNodeLocator
    {
        private SortedList<long, MasterServer> ketamaNodes = new SortedList<long, MasterServer>();

        /// <summary>
        /// 一致性哈希的实现
        /// </summary>
        /// <param name="nodes">实际节点</param>
        /// <param name="nodeCopies">虚拟节点的个数</param>
        public KetamaNodeLocator(List<MasterServer> nodes, int nodeCopies)
        {
            //对所有节点，生成nCopies个虚拟结点
            foreach (MasterServer node in nodes)
            {
                //每四个虚拟结点为一组
                for (int i = 0; i < nodeCopies / 4; i++)
                {
                    //getKeyForNode方法为这组虚拟结点得到惟一名称
                    byte[] digest = HashAlgorithm.ComputeMd5(node.HostPort + i);
                    //Md5是一个16字节长度的数组，将16字节的数组每四个字节一组，分别对应一个虚拟结点，这就是为什么上面把虚拟结点四个划分一组的原因
                    for (int h = 0; h < 4; h++)
                    {
                        long m = HashAlgorithm.Hash(digest, h);
                        ketamaNodes[m] = node;
                    }
                }
            }
        }

        /// <summary>
        /// 一致性哈希的实现
        /// </summary>
        /// <param name="nodes">实际节点</param>
        public KetamaNodeLocator(List<MasterServer> nodes)
            : this(nodes, 10)
        {
        }

        /// <summary>
        /// 通过Key值获取节点
        /// </summary>
        /// <param name="hash"></param>
        private MasterServer GetNodeForKey(long hash)
        {
            MasterServer rv;
            long key = hash;
            //如果找到这个节点，直接取节点，返回
            if (!ketamaNodes.ContainsKey(key))
            {
                //得到大于当前key的那个子Map，然后从中取出第一个key，就是大于且离它最近的那个key 说明详见: http://www.javaeye.com/topic/684087
                var tailMap = from coll in ketamaNodes
                    where coll.Key > hash
                    select new { coll.Key };
                if (tailMap == null || tailMap.Count() == 0)
                    key = ketamaNodes.FirstOrDefault().Key;
                else
                    key = tailMap.FirstOrDefault().Key;
            }
            rv = ketamaNodes[key];
            return rv;
        }

        /// <summary>
        /// 查找Key值所在的节点
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public MasterServer GetNodes(string key)
        {
            byte[] digest = HashAlgorithm.ComputeMd5(key);
            MasterServer rv = GetNodeForKey(HashAlgorithm.Hash(digest, 0));
            return rv;
        }
    }
}