using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDMJprogram
{
    class Game
    {
        //总共 144 张牌
        int[] PaiZu = new int[144] { 01, 01, 01, 01, 02, 02, 02, 02, 03, 03, 03, 03, 04, 04, 04, 04, 05, 05, 05, 05, 06, 06, 06, 06, 07, 07, 07, 07, 08, 08, 08, 08, 09, 09, 09, 09, //万 1111,2222,3333,4444,5555,6666,7777,8888,9999 (36)
                                     11, 11, 11, 11, 12, 12, 12, 12, 13, 13, 13, 13, 14, 14, 14, 14, 15, 15, 15, 15, 16, 16, 16, 16, 17, 17, 17, 17, 18, 18, 18, 18, 19, 19, 19, 19, //条 1111,2222,3333,4444,5555,6666,7777,8888,9999 (36)
                                     21, 21, 21, 21, 22, 22, 22, 22, 23, 23, 23, 23, 24, 24, 24, 24, 25, 25, 25, 25, 26, 26, 26, 26, 27, 27, 27, 27, 28, 28, 28, 28, 29, 29, 29, 29, //筒 1111,2222,3333,4444,5555,6666,7777,8888,9999 (36)
                                     31, 31, 31, 31, 32, 32, 32, 32, 33, 33, 33, 33, 34, 34, 34, 34, //风 东东东东,南南南南,西西西西,北北北北 (16)
                                     41, 41, 41, 41, 42, 42, 42, 42, 43, 43, 43, 43, //字 中中中中,发发发发,白白白白 (12)
                                     51, 52, 53, 54, 55, 56, 57, 58 }; //花 春夏秋冬,梅兰竹菊 (8)

        Queue<int> PaiKu = new Queue<int>();
        List<int> Touzi = new List<int>();
        int BaiDa = 0;
        int LastChuPai = 0;

        ShouPai shouPai_Zhuang = new ShouPai();
        ShouPai shouPai_Shun1 = new ShouPai();
        ShouPai shouPai_Shun2 = new ShouPai();
        ShouPai shouPai_Shun3 = new ShouPai();
        public void Initial()
        {
            //记录
            string RandomList = "【随机序号】";
            string PaikuList = "【牌库列表】";

            //开始游戏
            Recording.WriteLine("【开始游戏】==============================");

            //生成麻将&打乱牌组
            List<int> templist = new List<int>();
            Random rm = new Random();
            templist = GetRandom(0, 143, 144, rm, false);
            PaiKu.Clear();
            foreach (int i in templist)
            {
                #region 记录
                RandomList += i + ",";
                PaikuList += PaiZu[i].ToString("00") + ",";
                #endregion
                PaiKu.Enqueue(PaiZu[i]);
            }

            Recording.WriteLine(RandomList);
            Recording.WriteLine(PaikuList);
            
            //掷色子
            Touzi.Clear();
            rm = new Random();
            Touzi = GetRandom(1, 6, 2, rm, true);
            Recording.WriteLine("【骰子结果】" + Touzi.First() + "," + Touzi.Last());

            //手牌初始化
            shouPai_Zhuang = new ShouPai();
            shouPai_Shun1 = new ShouPai();
            shouPai_Shun2 = new ShouPai();
            shouPai_Shun3 = new ShouPai();
        }

        public void FapaiBaida()
        {
            Recording.WriteLine("【开始发牌】");

            //百搭，简化算法默认第一张为百搭
            do
            {
                BaiDa = PaiKu.Dequeue();
            } while (BaiDa > 50);
            Recording.WriteLine("【百搭麻将】"+BaiDa.ToString("00"));

            //发牌
            int LastPai = 0;

            for (int i = 0; i < 13; i++)
            {
                LastPai = PaiKu.Dequeue();
                shouPai_Zhuang.info[i] = LastPai;
                LastPai = PaiKu.Dequeue();
                shouPai_Shun1.info[i] = LastPai;
                LastPai = PaiKu.Dequeue();
                shouPai_Shun2.info[i] = LastPai;
                LastPai = PaiKu.Dequeue();
                shouPai_Shun3.info[i] = LastPai;
            }

            //LastPai = PaiKu.Dequeue();
            //shouPai_Zhuang.info[13] = LastPai;

            #region 记录
            string ShouPaiList = string.Empty;
            ShouPaiList = "【庄家手牌】";
            foreach(int i in shouPai_Zhuang.info)
                ShouPaiList += i.ToString("00") + ",";
            Recording.WriteLine(ShouPaiList);

            ShouPaiList = "【顺一手牌】";
            foreach (int i in shouPai_Shun1.info)
                ShouPaiList += i.ToString("00") + ",";
            Recording.WriteLine(ShouPaiList);

            ShouPaiList = "【顺二手牌】";
            foreach (int i in shouPai_Shun2.info)
                ShouPaiList += i.ToString("00") + ",";
            Recording.WriteLine(ShouPaiList);

            ShouPaiList = "【顺三手牌】";
            foreach (int i in shouPai_Shun3.info)
                ShouPaiList += i.ToString("00") + ",";
            Recording.WriteLine(ShouPaiList);

            UpdatePaikuInfo();

            Recording.WriteLine("【最近出牌】" + LastChuPai.ToString("00"));
            #endregion
        }

        public void StartLoop()
        {

        }


        /// <summary>
        /// 根据随机数范围获取一定数量的随机数
        /// </summary>
        /// <param name="minNum">随机数最小值</param>
        /// <param name="maxNum">随机数最大值</param>
        /// <param name="ResultCount">随机结果数量</param>
        /// <param name="isSame">结果是否允许重复</param>
        /// <returns></returns>
        private static List<int> GetRandom(int minNum, int maxNum, int ResultCount, Random rm, bool isSame)
        {
            List<int> randomList = new List<int>();
            int nValue = 0;

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

        void UpdatePaikuInfo()
        {
            string PaiKuInfo = "【剩余牌库】";
            string PaiKuSum = "【牌库数量】";

            foreach (int i in PaiKu)
            {
                PaiKuInfo += i.ToString("00") + ",";
            }
            PaiKuSum += PaiKu.Count;
            Recording.WriteLine(PaiKuInfo);
            Recording.WriteLine(PaiKuSum);
        }
    }

    class ShouPai
    {
        public int Zuowei = ZUOWEI.Kong;
        public int[] info = new int[14] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 99 };
        public bool[] isShow = new bool[14];
    }
    public static class ZUOWEI
    {
        static public int Kong = 0;
        static public int Dong = 1;
        static public int Nan = 2;
        static public int Xi = 3;
        static public int Bei = 4;
    }

    public static class Recording
    {
        public static void WriteLine(string msg)
        {
            Console.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]" + msg);
        }
    }
}
