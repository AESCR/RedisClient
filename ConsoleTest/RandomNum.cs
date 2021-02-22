using System;
using System.Linq;

namespace XLibrary.Random
{
    public class RandomNum
    {
        private readonly System.Random _ra;

        public RandomNum()
        {
            _ra = new System.Random();
        }

        private int GetNum(int[] arrNum, int tmp, int minValue, int maxValue, System.Random ra)
        {
            int n = 0;
            while (n <= arrNum.Length - 1)
            {
                if (arrNum[n] == tmp) //利用循环判断是否有重复
                {
                    tmp = ra.Next(minValue, maxValue); //重新随机获取。
                    GetNum(arrNum, tmp, minValue, maxValue, ra); //递归:如果取出来的数字和已取得的数字有重复就重新随机获取。
                }
            }

            return tmp;
        }

        /// <summary>
        /// 生成具有唯一编号的字符串
        /// </summary>
        /// <returns> The check code number. </returns>
        /// <param name="codeCount"> Code count. Max 10 </param>
        public string GenerateCheckCodeNum(int codeCount = 10)
        {
            codeCount = codeCount > 10 ? 10 : codeCount; // unable to return unique number list longer than 10
            int[] arrInt = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            arrInt = arrInt.OrderBy(c => Guid.NewGuid()).ToArray(); // make the array in random order

            var str = string.Empty;

            for (var i = 0; i < codeCount; i++) str += arrInt[i];
            return str;
        }

        /// <summary>
        /// 对一个数组进行随机排序
        /// </summary>
        /// <typeparam name="T"> 数组的类型 </typeparam>
        /// <param name="arr"> 需要随机排序的数组 </param>
        public void GetRandomArray<T>(T[] arr)
        {
            //对数组进行随机排序的算法:随机选择两个位置，将两个位置上的值交换

            //交换的次数,这里使用数组的长度作为交换次数
            var count = arr.Length;

            //开始交换
            for (var i = 0; i < count; i++)
            {
                //生成两个随机数位置
                var targetIndex1 = GetRandomInt(0, arr.Length);
                var targetIndex2 = GetRandomInt(0, arr.Length);

                //定义临时变量
                T temp;

                //交换两个随机数位置的值
                temp = arr[targetIndex1];
                arr[targetIndex1] = arr[targetIndex2];
                arr[targetIndex2] = temp;
            }
        }

        /// <summary>
        /// 生成一个0.0到1.0的随机小数
        /// </summary>
        public double GetRandomDouble()
        {
            return _ra.NextDouble();
        }

        /// <summary>
        /// 生成一个指定范围的随机整数，该随机数范围包括最小值，但不包括最大值
        /// </summary>
        /// <param name="minNum"> 最小值 </param>
        /// <param name="maxNum"> 最大值 </param>
        public int GetRandomInt(int minNum, int maxNum)
        {
            return _ra.Next(minNum, maxNum);
        }

        /// <summary>
        /// 获取一个随机数
        /// </summary>
        /// <param name="minValue"> </param>
        /// <param name="maxValue"> </param>
        /// <returns> </returns>
        public int GetRandomNum(int minValue, int maxValue)
        {
            return _ra.Next(minValue, maxValue);
        }

        /// <summary>
        /// 获取一组随机数 不重复
        /// </summary>
        /// <param name="num"> 随机数个数 </param>
        /// <param name="minValue"> 最小值 </param>
        /// <param name="maxValue"> 最大值 </param>
        /// <returns> </returns>
        public int[] GetRandomNums(int num, int minValue, int maxValue)
        {
            System.Random ra = new System.Random(unchecked((int)DateTime.Now.Ticks));
            int[] arrNum = new int[num];
            int tmp = 0;
            for (int i = 0; i <= num - 1;)
            {
                tmp = ra.Next(minValue, maxValue); //随机取数
                arrNum[i] = GetNum(arrNum, tmp, minValue, maxValue, ra); //取出值赋到数组中
            }
            return arrNum;
        }

        /// <summary>
        /// 随机字符串
        /// </summary>
        /// <param name="stringLength"> </param>
        /// <param name="randomString"> </param>
        /// <returns> </returns>
        public string GetRandomString(int stringLength, string randomString = "0123456789ABCDEFGHIJKMLNOPQRSTUVWXYZ")
        {
            var returnValue = string.Empty;
            for (var i = 0; i < stringLength; i++)
            {
                var r = _ra.Next(0, randomString.Length - 1);
                returnValue += randomString[r];
            }
            return returnValue;
        }
    }
}