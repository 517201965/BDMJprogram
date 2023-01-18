using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDMJprogram
{
    class Game
    {
        //万 1111,2222,3333,4444,5555,6666,7777,8888,9999 (36)
        //条 1111,2222,3333,4444,5555,6666,7777,8888,9999 (36)
        //筒 1111,2222,3333,4444,5555,6666,7777,8888,9999 (36)
        //风 东东东东,南南南南,西西西西,北北北北 (16)
        //字 中中中中,发发发发,白白白白 (12)
        //花 春夏秋冬,梅兰竹菊 (8)
        //总共 144 张牌

        List<int> PaiKu = new List<int>();
        List<int> Touzi = new List<int>();
        public void Initial()
        {
            //生成麻将&打乱牌组
            PaiKu.Clear();
            PaiKu = GetRandom(0, 143, 144, false);
            //掷色子
            Touzi.Clear();
            Touzi = GetRandom(1, 6, 2, true);
            //发牌
        }

        public void FapaiBaida()
        {
            //发牌
            //百搭
        }

        /// <summary>
        /// 根据随机数范围获取一定数量的随机数
        /// </summary>
        /// <param name="minNum">随机数最小值</param>
        /// <param name="maxNum">随机数最大值</param>
        /// <param name="ResultCount">随机结果数量</param>
        /// <param name="isSame">结果是否允许重复</param>
        /// <returns></returns>
        private static List<int> GetRandom(int minNum, int maxNum, int ResultCount, bool isSame)
        {
            List<int> randomList = new List<int>();
            int nValue = 0;
            Random rm = new Random();

            if (isSame) 
            {
                for (int i = 0; randomList.Count < ResultCount; i++)
                {
                    nValue = rm.Next(minNum, maxNum + 1);
                    randomList.Add(nValue);
                }
            }
            else
            {
                for (int i = 0; randomList.Count < ResultCount; i++)
                {
                    nValue = rm.Next(minNum, maxNum + 1);
                    //重复判断
                    if (!randomList.Contains(nValue))
                    {
                        randomList.Add(nValue);
                    }
                }
            }

            return randomList;
        }
    }

    class ShouPai
    {
        int Zuowei = ZUOWEI.Kong;
        int[] info = new int[14];
        bool[] isShow = new bool[14];
    }
    public static class ZUOWEI
    {
        static public int Kong = 0;
        static public int Dong = 1;
        static public int Nan = 2;
        static public int Xi = 3;
        static public int Bei = 4;
    }
}
