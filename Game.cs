using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Pipes;

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
                                     51, 52, 53, 54, 55, 56, 57, 58 }; //花 春夏秋冬,梅兰菊竹 (8)

        Queue<int> PaiKu = new Queue<int>();
        List<int> Touzi = new List<int>();
        int ZhuangJia = 0;
        int BaiDa = 0;
        int LastChuPai = 0;

        ShouPai[] shouPai = new ShouPai[4];
        public void Initial(int Zhuangjia)
        {
            //记录
            string RandomList = "【随机序号】";
            string PaikuList = "【牌库列表】";

            //开始游戏
            ZhuangJia = Zhuangjia;
            Recording.WriteLine("【开始游戏】庄家为：" + ZUOWEI.FengXiang[ZhuangJia]);

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
            Recording.Last_TouziResult = string.Format("【骰子结果】" + Touzi.First() + "," + Touzi.Last());
            Recording.WriteLine(Recording.Last_TouziResult);

            //手牌初始化
            shouPai[0] = new ShouPai() { Zuowei = ZUOWEI.Dong }; 
            shouPai[1] = new ShouPai() { Zuowei = ZUOWEI.Nan };
            shouPai[2] = new ShouPai() { Zuowei = ZUOWEI.Xi };
            shouPai[3] = new ShouPai() { Zuowei = ZUOWEI.Bei };
        }

        public void FapaiBaida()
        {
            Recording.WriteLine("【发牌阶段】");

            //百搭，简化算法默认第一张为百搭
            do
            {
                BaiDa = PaiKu.Dequeue();
            } while (BaiDa > 50);
            Recording.Last_Baida = string.Format("【百搭麻将】" + BaiDa.ToString("00"));
            Recording.WriteLine(Recording.Last_Baida);

            //发牌
            int LastPai = 0;

            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    LastPai = PaiKu.Dequeue();
                    shouPai[j].info[i] = LastPai;
                }
            }

            shouPai[ZhuangJia].isZhuang = true;

            #region 记录
            UpdateShouPaiInfo(); 
            UpdatePaikuInfo();
            #endregion

            //理牌
            Recording.WriteLine("【升序理牌】OK");
            for (int j = 0; j < 4; j++)
            {
                shouPai[j].LiPai1();
            }
            UpdateShouPaiInfo();
        }

        public void StartLoop()
        {
            Recording.WriteLine("【出牌阶段】");

            //初始信息
            int whosTurn = ZhuangJia;
            ShouPai whosShouPai = new ShouPai();
            bool iCGP = false;
            string command = string.Empty;
            int iLoop = 1;
            do 
            {
                Recording.WriteLine("【回合数量】" + iLoop.ToString("00")); iLoop++;
                Recording.WriteLine("【最近出牌】" + LastChuPai.ToString("00"));

                //吃碰杠抓确认
                int[] iFlagCmd = new int[4] { 0, 0, 0, 0 };
                string[] strCmdRecord = new string[4];
                iFlagCmd[whosTurn] = CPGZ_Level.Pass;
                if (LastChuPai != 0)
                {
                    bool isCPGZover = false;
                    do
                    {
                        //检查每个玩家的状态
                        string strCommand = string.Format("【等待指令】吃碰杠抓;东,{0};南,{1};西,{2};北,{3}", CPGZ_Level.Level_Zh[iFlagCmd[0]], CPGZ_Level.Level_Zh[iFlagCmd[1]], CPGZ_Level.Level_Zh[iFlagCmd[2]], CPGZ_Level.Level_Zh[iFlagCmd[3]]);
                        Recording.WriteLine(strCommand);
                        command = UpdateTransmitInfo_ReturnCommand(strCommand);
                        //command = Console.ReadLine();
                        Recording.WriteLine("【返回指令】" + command);
                        string[] cmd1 = command.Split(';');
                        if (cmd1.Length == 3)
                        {
                            int iWhosCmd = 5;
                            //检查谁的指令
                            switch (cmd1[0])
                            {
                                case "东": strCmdRecord[0] = command; iWhosCmd = 0; break;
                                case "南": strCmdRecord[1] = command; iWhosCmd = 1; break;
                                case "西": strCmdRecord[2] = command; iWhosCmd = 2; break;
                                case "北": strCmdRecord[3] = command; iWhosCmd = 3; break;
                                default: break;
                            }
                            //判定指令优先级，以及是否有效
                            if (iWhosCmd < 5)
                            {
                                switch (cmd1[1])
                                {
                                    case "过":
                                        iFlagCmd[iWhosCmd] = CPGZ_Level.Pass;
                                        break;
                                    case "吃":
                                        int[] iChi = new int[3];
                                        string[] iOrderC = cmd1[2].Split(',');
                                        if (iOrderC.Length == 2)
                                        {
                                            iChi[0] = LastChuPai;
                                            iChi[1] = shouPai[iWhosCmd].info[Convert.ToInt32(iOrderC[0])];
                                            iChi[2] = shouPai[iWhosCmd].info[Convert.ToInt32(iOrderC[1])];
                                            Array.Sort(iChi);
                                            if ((iChi[2] - iChi[1] == 1) && (iChi[1] - iChi[0] == 1))
                                            {
                                                iFlagCmd[iWhosCmd] = CPGZ_Level.Chi;
                                            }
                                        }
                                        break;
                                    case "碰":
                                        int[] iPeng = new int[3];
                                        string[] iOrderP = cmd1[2].Split(',');
                                        if (iOrderP.Length == 2)
                                        {
                                            iPeng[0] = LastChuPai;
                                            iPeng[1] = shouPai[iWhosCmd].info[Convert.ToInt32(iOrderP[0])];
                                            iPeng[2] = shouPai[iWhosCmd].info[Convert.ToInt32(iOrderP[1])];
                                            Array.Sort(iPeng);
                                            if ((iPeng[0] == iPeng[1]) && (iPeng[0] == iPeng[2]) && (iPeng[1] == iPeng[2])) 
                                            {
                                                iFlagCmd[iWhosCmd] = CPGZ_Level.Peng;
                                            }
                                        }
                                        break;
                                    case "杠": break;
                                    case "抓": break;
                                    default: break;
                                }
                            }
                        }
                        //检查每个玩家状态，是否已经完成指令
                        isCPGZover = (iFlagCmd[0] != 0) && (iFlagCmd[1] != 0) && (iFlagCmd[2] != 0) && (iFlagCmd[3] != 0);
                    } while (isCPGZover == false);

                    //如果没有人吃碰杠抓，则轮到下家
                    if (iFlagCmd.Sum() == 4)
                        whosTurn = ZUOWEI.Next(whosTurn);
                    else //有人吃碰杠抓
                    {
                        whosTurn = ArrarMaxIndex(iFlagCmd);
                        whosShouPai = shouPai[whosTurn];
                        //执行吃碰杠抓指令
                        int icmd = iFlagCmd[whosTurn];
                        if (icmd == CPGZ_Level.Chi)
                        {

                        }
                        else if(icmd == CPGZ_Level.Peng)
                        {

                        }
                        else if (icmd == CPGZ_Level.Gang)
                        {

                        }
                        else if (icmd == CPGZ_Level.Zhua)
                        {

                        }
                    }
                }
                else
                    Recording.WriteLine("【吃碰杠抓】庄家首轮跳过");

                //确认回合
                whosShouPai = shouPai[whosTurn];
                Recording.WriteLine("【谁的回合】" + ZUOWEI.FengXiang[whosShouPai.Zuowei]);

                //摸花
                UpdateShouPaiHua(whosShouPai);

                //摸牌&摸花
                if (iCGP == false) //如果没有吃碰杠，则摸牌
                {
                    whosShouPai.info[13] = PaiKu.Dequeue();
                    UpdateShouPaiHua(whosShouPai);
                }

                UpdateShouPaiInfo();

                //等待出杠胡牌指令
                bool isOver = false;
                do
                {
                    string strCommand = string.Format("【等待指令】出杠胡,{0},等", ZUOWEI.FengXiang[whosShouPai.Zuowei]);
                    Recording.WriteLine(strCommand);
                    command = UpdateTransmitInfo_ReturnCommand(strCommand);
                    //command = Console.ReadLine();
                    Recording.WriteLine("【返回指令】" + command);
                    string[] cmd2 = command.Split(';');
                    if (cmd2.Length == 3)
                    {
                        switch (cmd2[1])
                        {
                            case "出":
                                int iChuPaiOrder = Convert.ToInt32(cmd2[2]);
                                LastChuPai = whosShouPai.info[iChuPaiOrder];
                                whosShouPai.info[iChuPaiOrder] = 99;
                                whosShouPai.LiPai2();
                                whosShouPai.HistoryPai.Enqueue(LastChuPai);
                                //whosTurn = ZUOWEI.Next(whosTurn);
                                isOver = true;
                                break;
                            case "杠": break;
                            case "胡": break;
                            default: break;
                        }
                    }
                } while (isOver == false);

                UpdateShouPaiInfo();
            } while (true);       
        }


        /// <summary>
        /// 摸手牌内的花
        /// </summary>
        /// <param name="shouPai"></param>
        void UpdateShouPaiHua(ShouPai shouPai)
        {
            for (int i = 0; i < 14; i++)
            {
                int iHua = shouPai.info[i];
                while ((iHua >= 51) && (iHua <= 58))
                {
                    shouPai.AddHua(iHua);
                    shouPai.info[i] = PaiKu.Dequeue();
                    iHua = shouPai.info[i];
                }
            }
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

        /// <summary>
        /// 返回数组中最大值的索引
        /// </summary>
        /// <param name="arr">需要计算的数组</param>
        /// <returns></returns>
        private static int ArrarMaxIndex(int[] arr)
        {
            int index = 0;
            int value = arr[0];

            for (int i = 0; i < arr.Length; i++) 
            {
                int _value = arr[i];
                if (_value.CompareTo(value) > 0)
                {
                    value = _value;
                    index = i;
                }
            }

            return index;
        }

        /// <summary>
        /// 文字更新牌库信息
        /// </summary>
        void UpdatePaikuInfo()
        {
            string PaiKuInfo = "【剩余牌库】";
            string PaiKuSum = "【牌库数量】";

            foreach (int i in PaiKu)
            {
                PaiKuInfo += i.ToString("00") + ",";
            }
            PaiKuSum += PaiKu.Count;

            Recording.Last_PaikuSum = PaiKuSum;
            Recording.Last_PaikuInfo = PaiKuInfo;
            Recording.WriteLine(PaiKuInfo);
            Recording.WriteLine(PaiKuSum);
        }

        /// <summary>
        /// 文字更新手牌信息
        /// </summary>
        void UpdateShouPaiInfo()
        {
            for (int j = 0; j < 4; j++)
            {
                string shouPaiMsg = string.Empty;
                //手牌信息
                shouPaiMsg += "【" + ZUOWEI.FengXiang[shouPai[j].Zuowei] + "家手牌】";
                foreach (int i in shouPai[j].info)
                    shouPaiMsg += i.ToString("00") + ",";
                //吃碰杠信息
                shouPaiMsg += "【吃碰杠】";
                foreach (int i in shouPai[j].CPG)
                    shouPaiMsg += i.ToString("00") + ",";
                //花信息
                shouPaiMsg += "【花】";
                foreach (int i in shouPai[j].hua)
                    shouPaiMsg += i.ToString("00") + ",";
                //庄信息
                shouPaiMsg += "【庄】";
                if (shouPai[j].isZhuang) shouPaiMsg += "是";
                else shouPaiMsg += "否";
                Recording.Last_ShoupaiInfo[j] = shouPaiMsg;
                Recording.WriteLine(shouPaiMsg);
            }
        }

        /// <summary>
        /// 更新传输数据并发送,等待指令的返回
        /// </summary>
        /// <param name="command"></param>
        string  UpdateTransmitInfo_ReturnCommand(string command)
        {
            string msg = string.Empty;
            msg += Recording.Last_PaikuSum + ";";
            msg += Recording.Last_PaikuInfo + ";";
            msg += Recording.Last_TouziResult + ";";
            msg += Recording.Last_Baida + ";";
            msg += Recording.Last_ShoupaiInfo[0] + ";";
            msg += Recording.Last_ShoupaiInfo[1] + ";";
            msg += Recording.Last_ShoupaiInfo[2] + ";";
            msg += Recording.Last_ShoupaiInfo[3] + ";";
            msg += command;
            Recording.WriteLine(msg);
            PipeSendToServer(msg);
            return Console.ReadLine();
        }

        #region NamePipe 

        bool pipeClientConnected = false;
        NamedPipeServerStream pipeServer = null;
        StreamString streamServer = null;
        public bool CreateServer()
        {
            pipeServer = new NamedPipeServerStream("testpipe", PipeDirection.InOut);

            pipeServer.WaitForConnection();

            try
            {
                streamServer = new StreamString(pipeServer); 
                streamServer.WriteString("BDMJServer");
                string msg = streamServer.ReadString();
                if (msg == "BDMJClient")
                {
                    pipeClientConnected = true;
                    Console.WriteLine("BDMJClient Connected");
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("ERROR: {0}", e.Message);
            }
            return pipeClientConnected;
        }

        void PipeSendToServer(string msg)
        {
            bool isCommandCorrect = false;
            do
            {
                string ClientMsg = streamServer.ReadString();
                if(ClientMsg == "Request")
                {
                    streamServer.WriteString(msg);
                }
                else
                {
                    isCommandCorrect = true;
                }
            } while (isCommandCorrect == false);
        }
        #endregion
    }

    class ShouPai
    {
        public int Zuowei = ZUOWEI.Kong;
        public int[] info = new int[14] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 99 };
        public int[,] CPG = new int[4, 4] { { 99, 99, 99, 99 }, { 99, 99, 99, 99 }, { 99, 99, 99, 99 }, { 99, 99, 99, 99 } };
        public int[] hua = new int[8] { 99, 99, 99, 99, 99, 99, 99, 99 };
        public bool isZhuang = false;
        public Queue<int> HistoryPai = new Queue<int>();

        /// <summary>
        /// 升序理牌
        /// </summary>
        public void LiPai1()
        {
            Array.Sort(info);
        }

        /// <summary>
        /// 将手牌中的所有99移到最后
        /// </summary>
        public void LiPai2()
        {
            int[] info99 = new int[14] { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 };
            int j = 0;
            for (int i = 0; i < 14; i++)
            {
                if(info[i]!=99)
                {
                    info99[j] = info[i];
                    j++;
                }
            }
            info = info99;
        }
        public void AddHua(int iHua)
        {
            for (int i = 0; i < 8; i++)
            {
                if (hua[i] == 99)
                {
                    hua[i] = iHua;
                    break;
                }
            }
        }
    }
    public static class ZUOWEI
    {
        static public int Kong = 4;
        static public int Dong = 0;
        static public int Nan = 1;
        static public int Xi = 2;
        static public int Bei = 3;
        static public string[] FengXiang = new string[5] { "东", "南", "西", "北", "空" };
        static public int Next(int iCurrent)
        {
            iCurrent++;
            if (iCurrent > 4) iCurrent = 1;
            return iCurrent;
        }
    }

    public static class CPGZ_Level
    {
        static public int Wait = 0;
        static public int Pass = 1;
        static public int Chi = 2;
        static public int Peng = 3;
        static public int Gang = 4;
        static public int Zhua = 5;
        static public string[] Level_Zh = new string[6] { "等", "过", "吃", "碰", "杠", "抓" };
    }

    public static class Recording
    {
        public static string Last_PaikuSum = string.Empty;
        public static string Last_PaikuInfo = string.Empty;
        public static string Last_TouziResult = string.Empty;
        public static string Last_Baida = string.Empty;
        public static string[] Last_ShoupaiInfo = new string[4];
        public static void WriteLine(string msg)
        {
            Console.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]" + msg);
        }
    }
}
