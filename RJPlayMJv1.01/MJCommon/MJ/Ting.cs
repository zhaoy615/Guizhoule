using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MJModel.BllModel;

namespace MJCommon.common
{
    public class Ting
    {

        public string GetTing(List< MJModel.BllModel.MaJiang> Pai)
        {
            Pai.Sort((a, b) => -b.PaiHs.CompareTo(a.PaiHs));
            List< MJModel.BllModel.MaJiang> paitwo = new List<MJModel.BllModel.MaJiang>();
            paitwo.AddRange(Pai.ToArray());
            List< MJModel.BllModel.MaJiang> qidui = new List<MJModel.BllModel.MaJiang>();
            qidui.AddRange(Pai.ToArray());
            List<MJModel.BllModel.MaJiang> paisan = new List<MJModel.BllModel.MaJiang>();

            paisan.AddRange(Pai.ToArray());

            List<MJModel.BllModel.MaJiang> paisfour = new List<MJModel.BllModel.MaJiang>();

            paisfour.AddRange(Pai.ToArray());

            List<MJModel.BllModel.MaJiang> paidui = new List<MJModel.BllModel.MaJiang>();
            paidui.AddRange(Pai.ToArray());
            List<MJModel.BllModel.MaJiang> paisfive = new List<MJModel.BllModel.MaJiang>();
            paisfive.AddRange(Pai.ToArray());
            string ddstring = GetTingDD(paidui);

            //string HJDL = GetTingJDL(Pai);
            //if (!string.IsNullOrEmpty(HJDL))
            //{
            //    return HJDL;
            //}


            if (ddstring != "MJ")
            {
                if (Pai.Count == 13|| ddstring=="H")
                    return ddstring;
            }


           MJModel.BllModel.MaJiang mjone = new MJModel.BllModel.MaJiang();
           MJModel.BllModel.MaJiang mjtwo = new MJModel.BllModel.MaJiang();
            string stat = "MJ";//没叫
            int shun = 0;
            int dui = 0;
            int ke = 0;
            List<int> shan = new List<int>();//散牌
            if (Pai.Count == 1)
            {
                stat = "DDDD";//单调大队
            }
            stat = getQD(qidui);
            if (stat != "MJ")//没叫
            {
                if (Pai.Count == 13 || stat == "H")
                    return stat;
            }


            while (Pai.Count > 0)
            {
                if (Pai.Count >= 3)
                {
                    mjone = Pai.FindLast(u => u.PaiHs == Pai[0].PaiHs + 1);
                    mjtwo = Pai.FindLast(u => u.PaiHs == Pai[0].PaiHs + 2 && Pai[0].PaiHs + 2 != 11 && Pai[0].PaiHs + 2 != 21);


                    if (mjone != null && mjtwo != null)
                    {
                        if (Pai[0].PaiHs == Pai[0 + 1].PaiHs)
                        {
                            dui++;
                            Pai.RemoveRange(0, 2);
                        }
                        else
                        {
                            shun++;
                            Pai.Remove(Pai[0]);
                            Pai.Remove(mjone);
                            Pai.Remove(mjtwo);
                        }

                    }
                    else if (Pai[0].PaiHs == Pai[2].PaiHs)
                    {
                        ke++;
                        Pai.RemoveRange(0, 3);
                    }


                    else if (Pai[0].PaiHs == Pai[1].PaiHs)
                    {
                        dui++;
                        Pai.RemoveRange(0, 2);
                    }
                    else
                    {
                        shan.Add(Pai[0].PaiHs);
                        Pai.Remove(Pai[0]);
                    }
                }
                else if (Pai.Count >= 2)
                {
                    if (Pai[0].PaiHs == Pai[1].PaiHs)
                    {
                        dui++;
                        Pai.RemoveRange(0, 2);
                    }
                    else
                    {
                        shan.Add(Pai[0].PaiHs);
                        Pai.Remove(Pai[0]);
                    }
                }
                else
                {
                    shan.Add(Pai[0].PaiHs);
                    Pai.Remove(Pai[0]);
                }
            }
            if (shan.Count <= 3)
            {
                if (shan.Count == 1)
                {
                    if (dui == 6)
                    {
                        ///七对
                        stat = "QD";
                    }

                    else
                    {

                        if (dui == 0)
                        {
                            stat = "SP";//散牌
                        }
                    }



                    if (shun == 0 && dui == 0)
                    {
                        stat = "DD";//大队
                    }
                }
                else if (shan.Count == 0)
                {
                    if (shun == 0)
                    {
                        if (dui < 3)
                        {
                            stat = "DD";//大队
                        }

                    }
                    else if (dui == 5 && ke == 1)
                    {
                        stat = "LQD";//龙七对
                    }
                    else
                    {
                        if (dui == 0)
                        {
                            stat = "SP";//散牌
                        }

                        if (dui == 2)
                        {
                            stat = "SP";//散牌
                        }

                    }
                }
                else
                {
                    if (shan[0] == shan[1] - 1 || (shan[0] == shan[1] - 2 && shan[1] != 11 && shan[1] != 21))
                    {

                        if (dui == 1)
                        {
                            stat = "SP";//散牌
                        }

                    }

                }
            }

            if ((shan.Count == 0 && dui == 1) || dui == 7)
            {
                stat = "H";//胡牌
            }
           
            string getstring = GetTingTwoo(paitwo);
            string sanstring = GetTingSan(paisan);
            string fourstring = GetTingFour(paisfour);
            string fivestring = GetTingFive(paisfive);

            if (stat == "H")
            {
                return stat;
            }
            if (getstring == "H")
            {
                return getstring;
            }
            if (sanstring == "H")
            {
                return sanstring;
            }

            if (fourstring == "H")
            {
                return fourstring;
            }
            if (stat == "MJ")
            {

                if (fourstring != "MJ")
                {
                    return fourstring;
                }

                if (getstring != "MJ")
                {
                    return getstring;
                }
                if (sanstring != "MJ")
                {
                    return sanstring;
                }
                if (fivestring != "MJ")
                {
                    return fivestring;
                }
            }



            return stat;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pai"></param>
        /// <returns></returns>
        private string GetTingFive(List<MJModel.BllModel.MaJiang> pai)
        {
            if (pai.Count == 0)
                return "MJ";
            List< MJModel.BllModel.MaJiang> listpai = new List<MJModel.BllModel.MaJiang>();
            listpai.AddRange(pai);

            int dui = 0;

            int n = 0;
            while (listpai.Count >= n + 3)//先扣除顺子
            {

                MJModel.BllModel.MaJiang mjone = listpai.FindLast(u => u.PaiHs == listpai[n].PaiHs + 1);
                MJModel.BllModel.MaJiang mjtwo = listpai.FindLast(u => u.PaiHs == listpai[n].PaiHs + 2 && listpai[n].PaiHs + 2 != 11 && listpai[n].PaiHs + 2 != 21);

                int shun = 0;
                if (listpai[n].PaiHs == listpai[n + 1].PaiHs)
                {
                    int i = 0;
                    while (true)
                    {
                        if (listpai.Any(w => w.PaiHs == listpai[n].PaiHs + i && listpai[n].PaiHs + i != 11 && listpai[n].PaiHs + i != 21))
                        {
                            shun++;
                        }
                        else
                            break;
                        i++;
                    }
                }

                if (shun > 1 && shun % 3 != 0)//如果加上成对的牌存在单张 那么就扣除当前的对子
                {
                    dui++;
                    listpai.RemoveRange(n, listpai.Where(w => w.PaiHs == listpai[n].PaiHs).Count());
                    n = 0;
                    continue;
                }
                if (mjone != null && mjtwo != null)
                {   if (listpai.Where(w => w.PaiHs == listpai[n].PaiHs).Count() == 2)
                    {
                        if (listpai.Where(w => w.PaiHs == mjone.PaiHs).Count() >= 2 && listpai.Where(w => w.PaiHs == mjtwo.PaiHs).Count() >= 2)
                        {
                            listpai.Remove(listpai[n]);
                            listpai.Remove(mjone);
                            listpai.Remove(mjtwo);
                        }
                        else
                        {
                            dui++;
                            listpai.RemoveRange(n, listpai.Where(w => w.PaiHs == listpai[n].PaiHs).Count());
                            n = 0;
                            continue;
                        }
                    }
                    else
                    {
                        listpai.Remove(listpai[n]);
                        listpai.Remove(mjone);
                        listpai.Remove(mjtwo);
                    }
                    n = 0;
                }
                else
                    n++;
            }
            n = 0;
            while (listpai.Count >= n + 4)
            {
                if (listpai[0].PaiHs == listpai[3].PaiHs)
                {
                    listpai.RemoveRange(0, 4);
                }
                else
                    n++;
            }
            n = 0;
            while (listpai.Count >= n + 3)
            {
                if (listpai[0].PaiHs == listpai[2].PaiHs)
                {
                    listpai.RemoveRange(0, 3);
                }
                else
                    n++;
            }
            n = 0;
            while (listpai.Count >= n + 2)
            {
                if (listpai[0].PaiHs == listpai[1].PaiHs)
                {
                    dui++;
                    listpai.RemoveRange(0, 2);
                }
                else
                    n++;
            }
            if (listpai.Count > 0)
                return "MJ";//没叫
            else
            {
                return dui > 1 ? "MJ" : "H";//这里不判断七对等特殊牌型，如果手上超过一个对子则为没叫牌
            }

        }


        /// <summary>
        /// 蒋登磊这SB找出的SB牌型判断
        /// </summary>
        /// <param name="Pai"></param>
        /// <returns></returns>
        public string GetTingJDL(List<MJModel.BllModel.MaJiang> Pai)
        {
            List<MJModel.BllModel.MaJiang> listpai = new List<MJModel.BllModel.MaJiang>();
            listpai.AddRange(Pai);
            List<int> Kelist = new List<int>();
            int dui = 0;
            int shun = 0;

            List<int> shan = new List<int>();


            for (int i = 0; i < listpai.Count; i = 0)
            {


                if (listpai.Count >= 3)
                {
                    MJModel.BllModel.MaJiang mjone = listpai.FindLast(u => u.PaiHs == listpai[i].PaiHs + 1);
                    MJModel.BllModel.MaJiang mjtwo = listpai.FindLast(u => u.PaiHs == listpai[i].PaiHs + 2 && listpai[i].PaiHs + 2 != 11 && listpai[i].PaiHs + 2 != 21);
                    if (listpai[i].PaiHs == listpai[i + 2].PaiHs)
                    {
                        Kelist.Add(listpai[i].PaiHs);

                        listpai.RemoveRange(0, 3);
                    }
                    else if (mjone != null && mjtwo != null)
                    {
                        listpai.Remove(listpai[i]);
                        listpai.Remove(mjone);
                        listpai.Remove(mjtwo);
                        shun++;
                    }
                    else if (listpai[i].PaiHs == listpai[i + 1].PaiHs)
                    {
                        dui++;

                        listpai.RemoveRange(0, 2);
                    }
                    else
                    {
                        shan.Add(listpai[i].PaiHs);
                        listpai.Remove(listpai[i]);
                    }
                }
                else if (listpai.Count >= 2)
                {
                    if (listpai[i].PaiHs == listpai[i + 1].PaiHs)
                    {
                        dui++;

                        listpai.RemoveRange(0, 2);
                    }
                    else
                    {
                        shan.Add(listpai[i].PaiHs);
                        listpai.Remove(listpai[i]);
                    }
                }
                else
                {
                    shan.Add(listpai[i].PaiHs);
                    listpai.Remove(listpai[i]);
                }



            }
            if (shan.Count == 2)
            {
                shan.Sort();
                if (shan[1] - shan[0] <= 2 && shan[1] != 11 && shan[1] != 21)
                {
                    for (int x = 0; x < Kelist.Count; x++)
                    {
                        if (shan[1] - shan[0] == 2)
                        {
                            shun++;
                            dui++;
                            shan.Clear();
                            break;

                        }
                    }
                }

            }
            if (dui == 1 && shan.Count == 0)
            {
                return "H";
            }
            else
            {
                return "";
            }

        }


        public string GetTingTwoo(List<MJModel.BllModel.MaJiang> Pai)
        {


            Pai.Sort((a, b) => -b.PaiHs.CompareTo(a.PaiHs));
           MJModel.BllModel.MaJiang mjone = new MJModel.BllModel.MaJiang();
           MJModel.BllModel.MaJiang mjtwo = new MJModel.BllModel.MaJiang();
            string stat = "MJ";
            int shun = 0;
            int dui = 0;
            int ke = 0;
            List<int> shan = new List<int>();

            stat = getQD(Pai);
            if (stat != "MJ")
            {
                if (Pai.Count == 13 || stat == "H")
                    return stat;
            }

          while(Pai.Count>1)
            {
                if (Pai.Count >= 3)
                {
                    mjone = Pai.FindLast(u => u.PaiHs == Pai[0].PaiHs + 1);
                    mjtwo = Pai.FindLast(u => u.PaiHs == Pai[0].PaiHs + 2 && Pai[0].PaiHs + 2 != 11 && Pai[0].PaiHs + 2 != 21);

                    if (Pai[0].PaiHs == Pai[1].PaiHs)
                    {
                        dui++;
                        Pai.RemoveRange(0, 2);
                    }
                    else if (Pai[0].PaiHs == Pai[2].PaiHs)
                    {
                        ke++;
                        Pai.RemoveRange(0, 3);
                    }

                    else if (mjone != null && mjtwo != null)
                    {
                        shun++;
                        Pai.Remove(Pai[0]);
                        Pai.Remove(mjone);
                        Pai.Remove(mjtwo);
                    }
                    else
                    {
                        shan.Add(Pai[0].PaiHs);
                        Pai.Remove(Pai[0]);
                    }
                }
                else if (Pai.Count >= 2)
                {
                    if (Pai[0].PaiHs == Pai[1].PaiHs)
                    {
                        dui++;
                        Pai.RemoveRange(0, 2);
                    }
                    else
                    {
                        shan.Add(Pai[0].PaiHs);
                        Pai.Remove(Pai[0]);
                    }
                }
                else
                {
                    shan.Add(Pai[0].PaiHs);
                    Pai.Remove(Pai[0]);
                }
            }
            if (shan.Count <= 3)
            {
                if (shan.Count == 1)
                {
                    if (dui == 6)
                    {
                        ///七对
                        stat = "QD";
                    }

                    else
                    {

                        if (dui == 0)
                        {
                            stat = "SP";
                        }
                    }
                }
                else if (shan.Count == 0)
                {
                    if (shun == 0)
                    {
                        if (dui < 3 && shun == 0)
                        {
                            stat = "DD";
                        }

                    }
                    else if (dui == 5 && ke == 1)
                    {
                        stat = "LQD";
                    }
                    else if (dui == 2)
                    {
                        stat = "SP";
                    }
                    else
                    {
                        if (dui == 0)
                        {
                            stat = "SP";
                        }

                    }
                }
                else
                {
                    if (shan[0] == shan[1] - 1 || (shan[0] == shan[1] - 2 && shan[1] != 11 && shan[1] != 21))
                    {

                        if (dui == 1)
                        {
                            stat = "SP";
                        }

                    }

                }
            }

            if ((shan.Count == 0 && dui == 1) || dui == 7)
            {
                stat = "H";
            }
            return stat;
        }

        /// <summary>
        /// 判断是否叫牌牌型为大对子
        /// </summary>
        /// <param name="Pai"></param>
        /// <returns></returns>
        public string GetTingDD(List< MJModel.BllModel.MaJiang> Pai)
        {
            int ke = 0;
            int dui = 0;
            int shan = 0;//单数量

            if (Pai.Count == 0)
            {
                return "手牌为空";
            }


            while (Pai.Count > 0)
            {

                if (Pai.Count >= 3)
                {
                    if (Pai[0].PaiHs == Pai[0 + 2].PaiHs)
                    {
                        ke++;
                        Pai.RemoveRange(0, 3);
                    }
                    else if (Pai[0].PaiHs == Pai[0 + 1].PaiHs)
                    {
                        dui++;
                        Pai.RemoveRange(0, 2);
                    }
                    else
                    {
                        shan++;
                        Pai.Remove(Pai[0]);
                    }
                }
                else if (Pai.Count >= 2)
                {
                    if (Pai[0].PaiHs == Pai[0 + 1].PaiHs)
                    {
                        dui++;
                        Pai.RemoveRange(0, 2);
                    }
                    else
                    {
                        shan++;
                        Pai.Remove(Pai[0]);
                    }
                }
                else
                {
                    shan++;
                    Pai.Remove(Pai[0]);
                }

            }

            if (dui == 0 && shan == 1)
            {
                return "DD";//大队
            }
            else if (dui == 2 && shan == 0)
            {
                return "DD";//大队
            }
            else if (dui == 1 && shan == 0)
            {
                return "H";//胡
            }
            else
            {
                return "MJ";//没叫牌
            }


        }


        public string GetTingSan(List< MJModel.BllModel.MaJiang> Pai)
        {


            Pai.Sort((a, b) => -b.PaiHs.CompareTo(a.PaiHs));

           MJModel.BllModel.MaJiang mjone = new MJModel.BllModel.MaJiang();
           MJModel.BllModel.MaJiang mjtwo = new MJModel.BllModel.MaJiang();
            string stat = "MJ";
            int shun = 0;
            int dui = 0;
            int ke = 0;
            List<int> shan = new List<int>();

            stat = getQD(Pai);
            if (stat != "MJ")
            {
                if (Pai.Count == 13 || stat == "H")
                    return stat;
            }

           while(Pai.Count>1)
            {
                if (Pai.Count >= 3)
                {
                    mjone = Pai.FindLast(u => u.PaiHs == Pai[0].PaiHs + 1);
                    mjtwo = Pai.FindLast(u => u.PaiHs == Pai[0].PaiHs + 2 && Pai[0].PaiHs + 2 != 11 && Pai[0].PaiHs + 2 != 21);
                    if (Pai[0].PaiHs == Pai[ 2].PaiHs)
                    {
                        ke++;
                        Pai.RemoveRange(0, 3);
                    }
                    else if (Pai[0].PaiHs == Pai[1].PaiHs)
                    {
                        dui++;
                        Pai.RemoveRange(0, 2);
                    }
                    else if (mjone != null && mjtwo != null)
                    {


                        shun++;
                        Pai.Remove(Pai[0]);
                        Pai.Remove(mjone);
                        Pai.Remove(mjtwo);


                    }
                    else
                    {
                        shan.Add(Pai[0].PaiHs);
                        Pai.Remove(Pai[0]);
                    }
                }
                else if (Pai.Count >= 2)
                {
                    if (Pai[0].PaiHs == Pai[ 1].PaiHs)
                    {
                        dui++;
                        Pai.RemoveRange(0, 2);
                    }
                    else
                    {
                        shan.Add(Pai[0].PaiHs);
                        Pai.Remove(Pai[0]);
                    }
                }
                else
                {
                    shan.Add(Pai[0].PaiHs);
                    Pai.Remove(Pai[0]);
                }
            }
            if (shan.Count <= 3)
            {
                if (shan.Count == 1)
                {
                    if (dui == 6)
                    {
                        ///七对
                        stat = "QD";
                    }

                    else
                    {

                        if (dui == 0)
                        {
                            stat = "SP";
                        }
                    }
                }
                else if (shan.Count == 0)
                {
                    if (shun == 0)
                    {
                        if (dui < 3 && shun == 0)
                        {
                            stat = "DD";
                        }

                    }
                    else if (dui == 5 && ke == 1)
                    {
                        stat = "LQD";
                    }
                    else if (dui == 2)
                    {
                        stat = "SP";
                    }
                    else
                    {
                        if (dui == 0)
                        {
                            stat = "SP";
                        }

                    }
                }
                else
                {
                    if (shan[0] == shan[1] - 1 || (shan[0] == shan[1] - 2 && shan[1] != 11 && shan[1] != 21))
                    {



                    }
                }

                if ((shan.Count == 0 && dui == 1) || dui == 7)
                {
                    stat = "H";
                }
            }
            return stat;


        }


        public string GetTingFour(List< MJModel.BllModel.MaJiang> Pai)
        {


            Pai.Sort((a, b) => -b.PaiHs.CompareTo(a.PaiHs));

           MJModel.BllModel.MaJiang mjone = new MJModel.BllModel.MaJiang();
           MJModel.BllModel.MaJiang mjtwo = new MJModel.BllModel.MaJiang();
            string stat = "MJ";
            int shun = 0;
            int dui = 0;
            int ke = 0;
            List<int> shan = new List<int>();

            stat = getQD(Pai);
            if (stat != "MJ")
            {
                if (Pai.Count == 13 || stat == "H")
                    return stat;
            }

           while(Pai.Count>0)
            {
                if (Pai.Count >= 3)
                {
                    mjone = Pai.FindLast(u => u.PaiHs == Pai[0].PaiHs + 1);
                    mjtwo = Pai.FindLast(u => u.PaiHs == Pai[0].PaiHs + 2 && Pai[0].PaiHs + 2 != 11 && Pai[0].PaiHs + 2 != 21);
                    if (Pai[0].PaiHs == Pai[2].PaiHs)
                    {
                        ke++;
                        Pai.RemoveRange(0, 3);
                    }
                    else if (mjone != null && mjtwo != null)
                    {


                        shun++;
                        Pai.Remove(Pai[0]);
                        Pai.Remove(mjone);
                        Pai.Remove(mjtwo);


                    }
                    else if (Pai[0].PaiHs == Pai[1].PaiHs)
                    {
                        dui++;
                        Pai.RemoveRange(0, 2);
                    }

                    else
                    {
                        shan.Add(Pai[0].PaiHs);
                        Pai.Remove(Pai[0]);
                    }
                }
                else if (Pai.Count >= 2)
                {
                    if (Pai[0].PaiHs == Pai[ 1].PaiHs)
                    {
                        dui++;
                        Pai.RemoveRange(0, 2);
                    }
                    else
                    {
                        shan.Add(Pai[0].PaiHs);
                        Pai.Remove(Pai[0]);
                    }
                }
                else
                {
                    shan.Add(Pai[0].PaiHs);
                    Pai.Remove(Pai[0]);
                }
                
            }
            if (shan.Count <= 3)
            {
                if (shan.Count == 1)
                {
                    if (dui == 6)
                    {
                        ///七对
                        stat = "QD";
                    }

                    else
                    {

                        if (dui == 0)
                        {
                            stat = "SP";
                        }
                    }
                }
                else if (shan.Count == 0)
                {
                    if (shun == 0)
                    {
                        if (dui < 3 && shun == 0)
                        {
                            stat = "DD";
                        }

                    }
                    else if (dui == 5 && ke == 1)
                    {
                        stat = "LQD";
                    }
                    else if (dui == 2)
                    {
                        stat = "SP";
                    }
                    else
                    {
                        if (dui == 0)
                        {
                            stat = "SP";
                        }

                    }
                }
                else
                {
                    if (shan[0] == shan[1] - 1 || (shan[0] == shan[1] - 2 && shan[1] != 11 && shan[1] != 21))
                    {

                        if (dui == 1)
                        {
                            stat = "SP";
                        }

                    }

                }
            }

            if ((shan.Count == 0 && dui == 1) || dui == 7)
            {
                stat = "H";
            }
            return stat;
        }

        /// <summary>
        /// 获取七对
        /// </summary>
        /// <param name="Pai"></param>
        /// <returns></returns>
        public string getQD(List< MJModel.BllModel.MaJiang> Pai)
        {

            List< MJModel.BllModel.MaJiang> list = new List< MJModel.BllModel.MaJiang>();
            list.AddRange(Pai.ToArray());
            int dui = 0;
            int ke = 0;
            while (list.Count >= 2)
            {
                if (list.Count >= 4)
                {
                    if (list[0].PaiHs == list[0 + 3].PaiHs)
                    {
                        dui += 2;
                        list.RemoveRange(0, 4);
                    }
                    else if (list[0].PaiHs == list[0 + 2].PaiHs)
                    {
                        ke++;
                        list.RemoveRange(0, 3);
                    }
                    else if (list[0].PaiHs == list[0 + 1].PaiHs)
                    {
                        dui++;
                        list.RemoveRange(0, 2);
                    }
                    else
                    {
                        list.Remove(list[0]);
                    }
                }
                else if (list.Count >= 3)
                {
                    if (list[0].PaiHs == list[0 + 2].PaiHs)
                    {
                        ke++;
                        list.RemoveRange(0, 3);
                    }
                    else if (list[0].PaiHs == list[0 + 1].PaiHs)
                    {
                        dui++;
                        list.RemoveRange(0, 2);
                    }
                    else
                    {
                        list.Remove(list[0]);
                    }
                }
                else
                {
                    if (list[0].PaiHs == list[0 + 1].PaiHs)
                    {
                        dui++;
                        list.RemoveRange(0, 2);
                    }
                    else
                    {
                        list.Remove(list[0]);
                    }
                }


                //}
                //else
                //{
                //    break;
                //}

            }
            if (dui == 6)
            {
                return "QD";//七对
            }
            else if (dui == 5 && ke == 1)
            {
                return "LQD";//龙七对
            }
            else if (dui == 7)
            {
                return "H";//胡
            }
            else
            {
                return "MJ";
            }
        }

        /// <summary>
        /// 返回听牌
        /// </summary>
        /// <param name="Pai"></param>
        /// <returns></returns>
        public List< MJModel.BllModel.MaJiang> ReturnJMJ(List< MJModel.BllModel.MaJiang> Pai)
        {
           MJModel.BllModel.MaJiang model = new MJModel.BllModel.MaJiang();
            List< MJModel.BllModel.MaJiang> RL = new List< MJModel.BllModel.MaJiang>();

            foreach (var item in Pai)
            {
                List< MJModel.BllModel.MaJiang> sMJ = new List< MJModel.BllModel.MaJiang>();
                sMJ.AddRange(Pai.ToArray());
                model = item;
                sMJ.Remove(item);
                if (GetTing(sMJ) != "MJ")
                {
                    RL.Add(model);
                }

            }
            return RL;
        }



        /// <summary>
        /// 返回对应鸡牌和牌型分值
        /// </summary>
        /// <param name="isyikou3">是否一扣3</param>
        /// <returns></returns>
        public JIpai GetJIpai(bool isyikou3, bool is_lianz)
        {
            JIpai jipai = new JIpai();
            if (isyikou3)
            {
                jipai.cfj = 3;
                jipai.cfwuguji = 6;
                jipai.wuguji = 2;
                jipai.tt = 20;
                jipai.xqj = 3;
                jipai.yaoji = 1;
                jipai.zimo = 3;
                jipai.th = 20;
                jipai.bt = 10;
                jipai.dduizi = 5;
                jipai.longqidui = 20;
                jipai.qys = 10;
                jipai.xqidui = 10;
                jipai.zr_yao = 2;
                jipai.zr_wugu = 4;
            }
            else if (is_lianz)
            {
                jipai.cfj = 2;
                jipai.cfwuguji = 4;
                jipai.wuguji = 2;
                jipai.tt = 20;
                jipai.xqj = 3;
                jipai.yaoji = 1;
                jipai.zimo = 1;
                jipai.th = 20;
                jipai.bt = 10;
                jipai.dduizi = 5;
                jipai.longqidui = 20;
                jipai.qys = 10;
                jipai.xqidui = 10;
                jipai.zr_yao = 1;
                jipai.zr_wugu = 2;
            }
            else
            {
                jipai.cfj = 2;
                jipai.cfwuguji = 4;
                jipai.wuguji = 2;
                jipai.tt = 20;
                jipai.xqj = 3;
                jipai.yaoji = 1;
                jipai.zimo = 2;
                jipai.th = 20;
                jipai.bt = 10;
                jipai.dduizi = 5;
                jipai.longqidui = 20;
                jipai.qys = 10;
                jipai.xqidui = 10;
                jipai.zr_yao = 1;
                jipai.zr_wugu = 2;
            }




            return jipai;

        }



        List<int> lianZhuangScore = new List<int>() { 0, 0, 0, 0 };//单独结算的得分
        List<int> danduScore = new List<int>() { 0, 0, 0, 0 };//单独结算的得分
        List<int> zrwgjlist = new List<int>() { 0, 0, 0, 0 };//责任乌骨鸡特殊算法
        List<int> zryaojilist = new List<int>() { 0, 0, 0, 0 };//责任幺鸡特殊算法
        List<UserSettle> userList = new List<UserSettle>();
        /// <summary>
        /// 结算
        /// </summary>
        /// <param name="roomid">房间号</param>
        /// <param name="majiang">翻鸡牌</param>
        /// <returns></returns>
        public List<UserSettle> Settle(int roomid, MJModel.BllModel.MaJiang majiang, SendHu huInfo)
        {
            Room r = Gongyong.roomlist.Find(u => u.RoomID == roomid);
            ///返回各鸡牌对应分数
            JIpai jipai = GetJIpai(r.is_yikousan, r.is_lianz);
            ///根据房间号找到对应用户
            List<mjuser> mjlist = Gongyong.mulist.FindAll(u => u.RoomID == roomid);
            List<UserSettle> returnList = new List<UserSettle>();
            CardUser roomzhuang = Gongyong.FKUser.Find(u => u.roomid == r.RoomID);

            ///默认实例化4个
            UserSettle user1 = new UserSettle();
            UserSettle user2 = new UserSettle();
            UserSettle user3 = new UserSettle();
            UserSettle user4 = new UserSettle();
            userList.Add(user1);
            userList.Add(user2);
            userList.Add(user3);
            userList.Add(user4);
            int lianzhuangZIMOScore = 0;
            if (r.is_lianz)
            {


                if (huInfo.Type == 1 || huInfo.Type == 5)
                {

                    //if (huInfo.Type == 5)
                    //{
                    //    lianzhuangZIMOScore += 1;
                    //}
                    if (huInfo.FWZ == roomzhuang.win)
                    {
                        lianzhuangZIMOScore = roomzhuang.lianzCount;
                    }
                    else
                    {
                        switch (roomzhuang.win)
                        {
                            case 1:
                                lianZhuangScore[0] -= roomzhuang.lianzCount;

                                break;
                            case 2:
                                lianZhuangScore[1] -= roomzhuang.lianzCount;
                                break;
                            case 3:
                                lianZhuangScore[2] -= roomzhuang.lianzCount;
                                break;
                            case 4:
                                lianZhuangScore[3] -= roomzhuang.lianzCount;
                                break;
                            default:
                                break;
                        }

                        switch (huInfo.FWZ)
                        {
                            case 1:
                                lianZhuangScore[0] += roomzhuang.lianzCount;
                                break;
                            case 2:
                                lianZhuangScore[1] += roomzhuang.lianzCount;
                                break;
                            case 3:
                                lianZhuangScore[2] += roomzhuang.lianzCount;
                                break;
                            case 4:
                                lianZhuangScore[3] += roomzhuang.lianzCount;
                                break;
                            default:
                                break;
                        }


                    }
                }
                else
                {
                    if (roomzhuang.win == huInfo.FWZ || roomzhuang.win == huInfo.FWB)
                    {
                        switch (huInfo.FWZ)
                        {
                            case 1:

                                lianZhuangScore[0] += roomzhuang.lianzCount;
                                break;
                            case 2:
                                lianZhuangScore[1] += roomzhuang.lianzCount;
                                break;
                            case 3:
                                lianZhuangScore[2] += roomzhuang.lianzCount;
                                break;
                            case 4:
                                lianZhuangScore[3] += roomzhuang.lianzCount;
                                break;
                            default:
                                break;
                        }

                        switch (huInfo.FWB)
                        {
                            case 1:

                                lianZhuangScore[0] -= roomzhuang.lianzCount;
                                break;
                            case 2:
                                lianZhuangScore[1] -= roomzhuang.lianzCount;
                                break;
                            case 3:
                                lianZhuangScore[2] -= roomzhuang.lianzCount;
                                break;
                            case 4:
                                lianZhuangScore[3] -= roomzhuang.lianzCount;
                                break;
                            default:
                                break;
                        }
                    }
                    #region 旧代码
                    //if (huInfo.FWZ == roomzhuang.win)
                    //{
                    //    switch (huInfo.FWZ)
                    //    {
                    //        case 1:
                    //            lianZhuangScore[0] += roomzhuang.lianzCount;
                    //            break;
                    //        case 2:
                    //            lianZhuangScore[1] += roomzhuang.lianzCount;
                    //            break;
                    //        case 3:
                    //            lianZhuangScore[2] += roomzhuang.lianzCount;
                    //            break;
                    //        case 4:
                    //            lianZhuangScore[3] += roomzhuang.lianzCount;
                    //            break;
                    //        default:
                    //            break;
                    //    }


                    //    switch (huInfo.FWB)
                    //    {
                    //        case 1:
                    //            lianZhuangScore[0] -= roomzhuang.lianzCount;
                    //            break;
                    //        case 2:
                    //            lianZhuangScore[1] -= roomzhuang.lianzCount;
                    //            break;
                    //        case 3:
                    //            lianZhuangScore[2] -= roomzhuang.lianzCount;
                    //            break;
                    //        case 4:
                    //            lianZhuangScore[3] -= roomzhuang.lianzCount;
                    //            break;
                    //        default:
                    //            break;
                    //    }
                    //}
                    //else if (huInfo.FWB == roomzhuang.win)
                    //{
                    //    switch (huInfo.FWZ)
                    //    {
                    //        case 1:
                    //            lianZhuangScore[0] += roomzhuang.lianzCount;
                    //            break;
                    //        case 2:
                    //            lianZhuangScore[1] += roomzhuang.lianzCount;
                    //            break;
                    //        case 3:
                    //            lianZhuangScore[2] += roomzhuang.lianzCount;
                    //            break;
                    //        case 4:
                    //            lianZhuangScore[3] += roomzhuang.lianzCount;
                    //            break;
                    //        default:
                    //            break;
                    //    }


                    //    switch (huInfo.FWB)
                    //    {
                    //        case 1:
                    //            lianZhuangScore[0] -= roomzhuang.lianzCount;
                    //            break;
                    //        case 2:
                    //            lianZhuangScore[1] -= roomzhuang.lianzCount;
                    //            break;
                    //        case 3:
                    //            lianZhuangScore[2] -= roomzhuang.lianzCount;
                    //            break;
                    //        case 4:
                    //            lianZhuangScore[3] -= roomzhuang.lianzCount;
                    //            break;
                    //        default:
                    //            break;
                    //    }
                    //} 
                    #endregion
                }

            }



            bool is_xia = false;
            bool is_ben = false;
            bool iswgj = false;
            switch (majiang.PaiHs)
            {
                case 8:

                    if (r.is_benji && r.is_wgj)
                    {
                        jipai.wuguji = jipai.wuguji * 2;
                        jipai.zr_wugu = jipai.zr_wugu * 2;
                        jipai.cfwuguji = jipai.cfwuguji * 2;
                        is_ben = true;
                    }

                    break;
                case 11:
                    if (r.is_benji)
                    {
                        jipai.yaoji = jipai.yaoji * 2;
                        jipai.zr_yao = jipai.zr_yao * 2;
                        jipai.cfj = jipai.cfj * 2;
                        is_ben = true;
                    }
                    break;
                case 7:
                    if (r.is_wgj)
                    {
                        jipai.wuguji = jipai.wuguji * 2;
                        jipai.zr_wugu = jipai.zr_wugu * 2;
                        jipai.cfwuguji = jipai.cfwuguji * 2;
                        iswgj = true;
                    }

                    break;
                case 19:
                    jipai.yaoji = jipai.yaoji * 2;
                    jipai.zr_yao = jipai.zr_yao * 2;
                    jipai.cfj = jipai.cfj * 2;
                    iswgj = true;

                    break;
                case 9:
                    if (r.is_shangxiaji && r.is_wgj)
                    {
                        jipai.wuguji = jipai.wuguji * 2;
                        jipai.zr_wugu = jipai.zr_wugu * 2;
                        jipai.cfwuguji = jipai.cfwuguji * 2;
                        is_xia = true;
                    }

                    break;
                case 12:
                    if (r.is_shangxiaji)
                    {
                        jipai.yaoji = jipai.yaoji * 2;
                        jipai.zr_yao = jipai.zr_yao * 2;
                        jipai.cfj = jipai.cfj * 2;
                        is_xia = true;

                    }
                    break;
                default:
                    break;
            }

            int user1j = 0;//玩家得分
            int user2j = 0;
            int user3j = 0;
            int user4j = 0;


            foreach (var item in mjlist)
            {
                ///结算信息
                UserSettle userT = new UserSettle();
                int userj = 0;
                userT.openid = item.Openid;
                var majianglist = new List<MJModel.BllModel.MaJiang>();
                majianglist.AddRange(item.majiangs.ToArray());
                string hxx = new Ting().GetTing(majianglist);
                //bool is_hu = false;
                if (hxx.Equals("H"))
                {
                    //is_hu = true;
                    var newmajianglist = new List<MJModel.BllModel.MaJiang>();
                    newmajianglist.AddRange(item.majiangs.ToArray());
                    //newmajianglist.AddRange(ToArray());
                    //if (!string.IsNullOrEmpty(item.Gong))
                    //{
                    //  var GangPailist=  item.Gong.Split(',').Where(w => !string.IsNullOrEmpty(w)).Select(w=> new model.MaJiang { PaiHs=int.Parse( w.Split('|')[0].Length>4? w.Split('|')[0].Substring(0,2): w.Split('|')[0].Substring(0, 1)), PaiId=0 } );
                    //    newmajianglist.AddRange(GangPailist.ToArray());
                    //}
                    hxx = GetHPaiPaiXin(newmajianglist, item.Peng.GroupBy(w => w.PaiHs).Count(), string.IsNullOrEmpty(item.Gong) ? 0 : item.Gong.Split(',').Length, huInfo.MJ.PaiHS);
                }
                GetPaiXin(hxx, item);
                ///判断是否叫牌
                if (!item.Is_jiao)
                {
                    //为叫牌赋值唯0
                    userT.is_jiao = 0;
                    if (r.is_wgj)
                    {
                        #region 判断包的乌骨鸡
                        //存在冲锋鸡，判断打出去的8筒数
                        if (item.is_cfwg == true)
                        {
                            ///鸡牌复制
                            UserJPOne cfwg = new UserJPOne();
                            cfwg.PaiHS = 8;
                            cfwg.PaiScare = 0 - jipai.cfwuguji;
                            cfwg.type = 4;
                            userT.jp.Add(cfwg);
                            userj += cfwg.PaiScare;
                            ///查找出的8筒
                            int wuCount = item.chuda.FindAll(u => u.PaiHs == 8).Count - 1;


                            ///查找碰的8筒
                            int wuP = item.Peng.FindAll(u => u.PaiHs == 8).Count;


                            int ACount = wuP + wuCount;
                            if (ACount > 0)
                            {
                                UserJPOne wgp = new UserJPOne();
                                wgp.PaiHS = 8;
                                wgp.PaiScare = 0 - (ACount * jipai.wuguji);
                                userj += wgp.PaiScare;
                                wgp.type = 3;
                                userT.jp.Add(wgp);

                            }
                        }
                        ///不存在冲锋鸡，判断打出的8筒
                        else
                        {
                            ////存在责任鸡

                            if (item.is_zrwg != 0)
                            {
                                if (item.is_zrwg > 0 && mjlist.Any(w => w.is_zrwg == -1 && w.Is_jiao))
                                {
                                    UserJPOne zrcfwg = new UserJPOne();
                                    zrcfwg.PaiHS = 8;
                                    zrcfwg.PaiScare = 0 - jipai.zr_wugu;
                                    zrcfwg.type = 8;//责任鸡
                                    userT.jp.Add(zrcfwg);
                                    zrwgjlist[item.ZS_Fw - 1] += zrcfwg.PaiScare;//如果有责任鸡但是没下叫需要反过来给对方负责
                                }
                                else if (mjlist.Any(w => w.is_zrwg == 1 && w.Is_jiao))
                                {
                                    UserJPOne zrcfwg = new UserJPOne();
                                    zrcfwg.PaiHS = 8;
                                    zrcfwg.PaiScare = 0 - jipai.zr_wugu;
                                    zrcfwg.type = 8;//责任鸡
                                    userT.jp.Add(zrcfwg);
                                    //var zrwuInfo = mjlist.Find(w => w.is_zrwg == 1);//获取有责任乌骨鸡的玩家是否下叫
                                    //if (zrwuInfo.Is_jiao)//如果需要负责的那一家也没有下叫则不需要向对方负责
                                    zrwgjlist[item.ZS_Fw - 1] += zrcfwg.PaiScare;
                                    //else
                                    //    zrwgjlist = new List<int> { 0, 0, 0, 0 };
                                }

                                #region 查找打出的8筒
                                //int zrCount = item.chuda.FindAll(u => u.PaiHs == 8).Count;

                                /////查找碰的8筒
                                //int zrpCount = item.Peng.FindAll(u => u.PaiHs == 8).Count;


                                //int ACount = zrCount + zrpCount;
                                //if (!string.IsNullOrEmpty(item.Gong))
                                //{
                                //    string gang = string.Copy(item.Gong);
                                //    //替换相似字符，防止找错
                                //    gang = gang.Replace("18", "八条");
                                //    gang = gang.Replace("28", "八万");
                                //    gang = gang.Replace("8", "八筒");
                                //    if (gang.Contains("八筒"))
                                //    {
                                //        ACount = ACount + 4;
                                //        UserJPOne wg = new UserJPOne();
                                //        wg.PaiHS = 8;
                                //        wg.PaiScare = 0 - (ACount * jipai.wuguji);
                                //        wg.type = 3;

                                //        userT.jp.Add(wg);
                                //        userj += wg.PaiScare;
                                //    }
                                //}

                                //else
                                //{
                                //    if (ACount > 0)
                                //    {
                                //        UserJPOne wg = new UserJPOne();
                                //        wg.PaiHS = 8;
                                //        wg.PaiScare = 0 - (ACount * jipai.wuguji);
                                //        wg.type = 3;
                                //        userT.jp.Add(wg);
                                //        userj += wg.PaiScare;
                                //    }
                                //} 
                                #endregion



                            }


                            ///不存在责任鸡
                            //else
                            //{
                            int CCount = item.chuda.FindAll(u => u.PaiHs == 8).Count;


                            int PCount = item.Peng.FindAll(u => u.PaiHs == 8).Count;



                            int ACount = CCount + PCount;
                            if (!string.IsNullOrEmpty(item.Gong))
                            {
                                string gang = string.Copy(item.Gong);
                                //替换相似字符，防止找错
                                gang = gang.Replace("18", "八条");
                                gang = gang.Replace("28", "八万");
                                gang = gang.Replace("8", "八筒");
                                if (gang.Contains("八筒"))
                                {
                                    ACount = 4 + ACount;
                                    UserJPOne wg = new UserJPOne();
                                    wg.PaiHS = 8;
                                    wg.PaiScare = 0 - (ACount * jipai.wuguji);
                                    wg.type = 3;
                                    userT.jp.Add(wg);

                                }
                            }


                            if (ACount > 0)
                            {
                                UserJPOne wg = new UserJPOne();
                                wg.PaiHS = 8;
                                wg.PaiScare = 0 - (ACount * jipai.wuguji);
                                wg.type = 3;
                                userT.jp.Add(wg);
                                userj += wg.PaiScare;
                            }



                        }

                        #endregion
                    }

                    #region 未叫牌，判断幺鸡

                    ///判断是否冲锋鸡
                    if (item.Is_cfj == true)
                    {
                        UserJPOne cfg = new UserJPOne();
                        cfg.PaiHS = 11;
                        cfg.PaiScare = 0 - jipai.cfj;
                        cfg.type = 2;
                        userT.jp.Add(cfg);
                        userj += cfg.PaiScare;
                        ///查找打出的幺鸡
                        int CYJ = item.chuda.FindAll(u => u.PaiHs == 11).Count - 1;

                        ///查找碰的幺鸡
                        int PYJ = item.Peng.FindAll(u => u.PaiHs == 11).Count;
                        int Acount = CYJ + PYJ;
                        if (Acount > 0)
                        {
                            UserJPOne Pj = new UserJPOne();
                            Pj.PaiHS = 11;
                            Pj.PaiScare = 0 - (jipai.yaoji * Acount);
                            Pj.type = 1;
                            userT.jp.Add(Pj);
                            userj += Pj.PaiScare;
                        }
                    }
                    ///不存在冲锋鸡，判断打出的幺鸡
                    else
                    {
                        ///判断责任鸡

                        if (item.Is_zrj != 0)
                        {
                            if (item.Is_zrj > 0 && mjlist.Any(w => w.Is_zrj == -1 && w.Is_jiao))
                            {
                                UserJPOne zrcfg = new UserJPOne();
                                zrcfg.PaiHS = 11;
                                zrcfg.PaiScare = 0 - jipai.zr_yao;
                                zrcfg.type = 9;
                                userT.jp.Add(zrcfg);
                                //userj += zrcfg.PaiScare;
                                zryaojilist[item.ZS_Fw - 1] += zrcfg.PaiScare;
                            }
                            else if (mjlist.Any(w => w.Is_zrj == 1 && w.Is_jiao))
                            {
                                UserJPOne zrcfg = new UserJPOne();
                                zrcfg.PaiHS = 11;
                                zrcfg.PaiScare = 0 - jipai.zr_yao;
                                zrcfg.type = 9;
                                userT.jp.Add(zrcfg);
                                //userj += zrcfg.PaiScare;
                                //var zrwuInfo = mjlist.Find(w => w.Is_zrj == 1);//获取有责任乌骨鸡的玩家是否下叫
                                //if (zrwuInfo.Is_jiao)//如果需要负责的那一家也没有下叫则不需要向对方负责
                                zryaojilist[item.ZS_Fw - 1] += zrcfg.PaiScare;
                                //else
                                //    zryaojilist = new List<int> { 0, 0, 0, 0 };
                            }
                        }
                        //    ///查找打出的幺鸡
                        //    int zrcyj = item.chuda.FindAll(u => u.PaiHs == 11).Count;


                        //    ///查找碰的幺鸡
                        //    int zrpyj = item.Peng.FindAll(u => u.PaiHs == 11).Count;

                        //    int ACount = zrcyj + zrpyj;
                        //    string gang = item.Gong;

                        //    if (!string.IsNullOrEmpty(gang))
                        //    {



                        //        if (gang.Contains("11"))
                        //        {
                        //            ACount = ACount + 4;
                        //            UserJPOne yji = new UserJPOne();
                        //            yji.PaiHS = 11;
                        //            yji.PaiScare = 0 - (ACount * jipai.yaoji);
                        //            yji.type = 1;
                        //            userT.jp.Add(yji);
                        //            userj += yji.PaiScare;
                        //        }
                        //    }
                        //    else
                        //    {
                        //        if (ACount > 0)
                        //        {
                        //            UserJPOne yji = new UserJPOne();
                        //            yji.PaiHS = 11;
                        //            yji.PaiScare = 0 - (ACount * jipai.yaoji);
                        //            yji.type = 1;
                        //            userT.jp.Add(yji);
                        //            userj += yji.PaiScare;
                        //        }
                        //    }
                        //}

                        /////不存在责任鸡
                        //else
                        //{
                        ///查找打出的幺鸡
                        int cyj = item.chuda.FindAll(u => u.PaiHs == 11).Count;

                        ///查找碰的幺鸡
                        int pyj = item.Peng.FindAll(u => u.PaiHs == 11).Count;
                        int Acount = cyj + pyj;

                        if (!string.IsNullOrEmpty(item.Gong))
                        {

                            if (item.Gong.Contains("11"))
                            {
                                Acount = Acount + 4;
                                UserJPOne pj = new UserJPOne();
                                pj.PaiHS = 11;
                                pj.PaiScare = 0 - (Acount * jipai.yaoji);
                                pj.type = 1;
                                userT.jp.Add(pj);
                                userj += pj.PaiScare;
                            }
                            else
                            {
                                if (Acount > 0)
                                {
                                    UserJPOne pj = new UserJPOne();
                                    pj.PaiHS = 11;
                                    pj.PaiScare = 0 - (Acount * jipai.yaoji);
                                    pj.type = 1;
                                    userT.jp.Add(pj);
                                    userj += pj.PaiScare;
                                }
                            }
                        }
                        else
                        {
                            if (Acount > 0)
                            {
                                UserJPOne pj = new UserJPOne();
                                pj.PaiHS = 11;
                                pj.PaiScare = 0 - (Acount * jipai.yaoji);
                                pj.type = 1;
                                userT.jp.Add(pj);
                                userj += pj.PaiScare;
                            }
                        }
                        //}
                    }

                    #endregion
                }
                ///叫牌
                else
                {

                    userT.is_jiao = 1;
                    if (!(huInfo.Type >= 3 && huInfo.FWB == item.ZS_Fw))
                    {

                        #region 叫牌判断乌骨鸡
                        if (r.is_wgj)
                        {
                            ///添加冲锋乌骨鸡
                            if (item.is_cfwg)
                            {
                                UserJPOne pai = new UserJPOne();
                                pai.PaiHS = 8;
                                pai.PaiScare = jipai.cfwuguji;
                                pai.type = 4;
                                userT.jp.Add(pai);
                                userj += pai.PaiScare;
                                int Ccount = item.chuda.FindAll(u => u.PaiHs == 8).Count - 1;

                                int SCount = item.majiangs.FindAll(u => u.PaiHs == 8).Count;
                                int Pcount = item.Peng.FindAll(u => u.PaiHs == 8).Count;
                                int ACount = Ccount + Pcount + SCount;
                                if (ACount > 0)
                                {
                                    UserJPOne Ppai = new UserJPOne();
                                    Ppai.PaiHS = 8;
                                    Ppai.PaiScare = ACount * jipai.wuguji;
                                    Ppai.type = 3;
                                    userT.jp.Add(Ppai);
                                    userj += Ppai.PaiScare;
                                }
                            }
                            else
                            {
                                if (item.is_zrwg != 0)
                                {
                                    if (item.is_zrwg > 0)
                                    {
                                        UserJPOne zr = new UserJPOne();
                                        zr.PaiHS = 8;
                                        zr.PaiScare = jipai.zr_wugu;
                                        zr.type = 8;
                                        userT.jp.Add(zr);
                                        zrwgjlist[item.ZS_Fw - 1] += zr.PaiScare;
                                        //userj += zr.PaiScare;
                                    }
                                    else
                                    {
                                        UserJPOne zr = new UserJPOne();
                                        zr.PaiHS = 8;
                                        var zrwuInfo = mjlist.Find(w => w.is_zrwg == 1);//获取有责任乌骨鸡的玩家是否下叫
                                        if (!zrwuInfo.Is_jiao)
                                            zr.PaiScare = jipai.zr_wugu;
                                        else
                                            zr.PaiScare = 0 - jipai.zr_wugu;
                                        zr.type = 8;
                                        userT.jp.Add(zr);
                                        //userj += zr.PaiScare;
                                        zrwgjlist[item.ZS_Fw - 1] += zr.PaiScare;
                                    }
                                }

                                int Ccount = item.chuda.FindAll(u => u.PaiHs == 8).Count;
                                int Pcount = item.Peng.FindAll(u => u.PaiHs == 8).Count;
                                int SCount = item.majiangs.FindAll(u => u.PaiHs == 8).Count;
                                int ACount = Ccount + Pcount + SCount;
                                if (!string.IsNullOrEmpty(item.Gong))
                                {
                                    string gang = string.Copy(item.Gong);
                                    //替换相似字符，防止找错
                                    gang = gang.Replace("18", "八条");
                                    gang = gang.Replace("28", "八万");
                                    gang = gang.Replace("8", "八筒");
                                    if (gang.Contains("八筒"))
                                    {
                                        ACount = ACount + 4;

                                    }
                                }

                                if (ACount > 0)
                                {
                                    UserJPOne wg = new UserJPOne();
                                    wg.PaiHS = 8;
                                    wg.PaiScare = ACount * jipai.wuguji;
                                    wg.type = 3;
                                    userT.jp.Add(wg);
                                    userj += wg.PaiScare;
                                }

                            }
                        }
                        #endregion
                        #region 判断幺鸡
                        int YJcount = 0;
                        if (item.Is_cfj)
                        {
                            UserJPOne CFJ = new UserJPOne();
                            CFJ.PaiHS = 11;
                            CFJ.PaiScare = jipai.cfj;
                            CFJ.type = 2;
                            userT.jp.Add(CFJ);
                            userj += CFJ.PaiScare;
                            YJcount = item.chuda.FindAll(u => u.PaiHs == 11).Count - 1;

                        }
                        else
                        {
                            YJcount = item.chuda.FindAll(u => u.PaiHs == 11).Count;
                        }

                        if (item.Is_zrj != 0)
                        {
                            if (item.Is_zrj > 0)
                            {
                                UserJPOne zryj = new UserJPOne();
                                zryj.PaiHS = 11;
                                zryj.PaiScare = jipai.zr_yao;
                                zryj.type = 9;
                                userT.jp.Add(zryj);
                                // userj += zryj.PaiScare;
                                zryaojilist[item.ZS_Fw - 1] += zryj.PaiScare;
                            }
                            else
                            {
                                UserJPOne zryj = new UserJPOne();
                                zryj.PaiHS = 11;

                                var zrjInfo = mjlist.Find(w => w.Is_zrj == 1);//获取有责任乌骨鸡的玩家是否下叫
                                if (!zrjInfo.Is_jiao)
                                    zryj.PaiScare = jipai.zr_yao;
                                else
                                    zryj.PaiScare = 0 - jipai.zr_yao;

                                zryj.type = 9;
                                userT.jp.Add(zryj);
                                // userj += zryj.PaiScare;
                                zryaojilist[item.ZS_Fw - 1] += zryj.PaiScare;
                            }
                        }
                        int PCount = item.Peng.FindAll(u => u.PaiHs == 11).Count;
                        int Scount = item.majiangs.FindAll(u => u.PaiHs == 11).Count;
                        int Acount = PCount + Scount + YJcount;
                        if (!string.IsNullOrEmpty(item.Gong))
                        {
                            if (item.Gong.Contains("11"))
                            {
                                Acount = Acount + 4;
                            }
                        }
                        if (Acount > 0)
                        {
                            UserJPOne GYJ = new UserJPOne();
                            GYJ.PaiHS = 11;
                            GYJ.PaiScare = Acount * jipai.yaoji;
                            GYJ.type = 1;
                            userT.jp.Add(GYJ);
                            userj += GYJ.PaiScare;
                        }
                        #endregion
                        #region 判断本鸡
                        if (r.is_benji && !is_ben)
                        {
                            int Ccount = item.chuda.FindAll(u => u.PaiHs == majiang.PaiHs).Count;
                            int Pcount = item.Peng.FindAll(u => u.PaiHs == majiang.PaiHs).Count;
                            int SBcount = item.majiangs.FindAll(u => u.PaiHs == majiang.PaiHs).Count;
                            int ALLCount = Ccount + Pcount + SBcount;

                            if (!string.IsNullOrEmpty(item.Gong))
                            {
                                string GangBJ = string.Copy(item.Gong);
                                string[] arrG = GangBJ.Split(',');
                                foreach (var gang in arrG)
                                {
                                    if (gang.Contains(majiang.PaiHs.ToString()))
                                    {
                                        string[] itemarr = gang.Split('|');
                                        if (itemarr[0] == majiang.PaiHs.ToString())
                                        {
                                            ALLCount = ALLCount + 4;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (ALLCount > 0)
                            {
                                UserJPOne BJA = new UserJPOne();
                                BJA.PaiHS = majiang.PaiHs;
                                BJA.PaiScare = ALLCount;
                                BJA.type = 6;
                                userT.jp.Add(BJA);
                                userj += BJA.PaiScare;
                            }

                        }
                        #endregion
                        #region 判断普通鸡(上鸡)

                        ///上鸡花色
                        if (!iswgj)
                        {
                            int SJHS = 0;
                            if (majiang.PaiHs != 9 && majiang.PaiHs != 19 && majiang.PaiHs != 29)
                            {
                                SJHS = majiang.PaiHs + 1;
                            }
                            else
                            {
                                SJHS = majiang.PaiHs - 8;
                            }

                            int CSCount = item.chuda.FindAll(u => u.PaiHs == SJHS).Count;
                            int PSCount = item.Peng.FindAll(u => u.PaiHs == SJHS).Count;
                            int SSCount = item.majiangs.FindAll(u => u.PaiHs == SJHS).Count;
                            int ASCount = CSCount + PSCount + SSCount;
                            if (!string.IsNullOrEmpty(item.Gong))
                            {
                                string GangSJ = string.Copy(item.Gong);
                                string[] arrSJ = GangSJ.Split(',');
                                foreach (var gang in arrSJ)
                                {
                                    if (gang.Contains(majiang.PaiHs.ToString()))
                                    {
                                        string[] itemarr = gang.Split('|');
                                        if (itemarr[0] == majiang.PaiHs.ToString())
                                        {
                                            ASCount = ASCount + 4;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (ASCount > 0)
                            {
                                UserJPOne BJA = new UserJPOne();
                                BJA.PaiHS = SJHS;
                                BJA.PaiScare = ASCount;
                                BJA.type = 7;
                                userT.jp.Add(BJA);
                                userj += BJA.PaiScare;
                            }
                        }
                        #endregion
                        #region 上下鸡判断下鸡
                        if (r.is_shangxiaji && !is_xia)
                        {
                            int XJHS = 0;

                            if (majiang.PaiHs != 1 && majiang.PaiHs != 11 && majiang.PaiHs != 21)
                            {
                                XJHS = majiang.PaiHs - 1;
                            }
                            else
                            {
                                XJHS = majiang.PaiHs + 8;
                            }

                            int CXCount = item.chuda.FindAll(u => u.PaiHs == XJHS).Count;
                            int PXCount = item.Peng.FindAll(u => u.PaiHs == XJHS).Count;
                            int SXCount = item.majiangs.FindAll(u => u.PaiHs == XJHS).Count;
                            int AXCount = CXCount + PXCount + SXCount;
                            if (!string.IsNullOrEmpty(item.Gong))
                            {
                                string GangXJ = string.Copy(item.Gong);
                                string[] arrXJ = GangXJ.Split(',');
                                foreach (var gang in arrXJ)
                                {
                                    if (gang.Contains(majiang.PaiHs.ToString()))
                                    {
                                        string[] itemarr = gang.Split('|');
                                        if (itemarr[0] == majiang.PaiHs.ToString())
                                        {
                                            AXCount = AXCount + 4;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (AXCount > 0)
                            {
                                UserJPOne BJA = new UserJPOne();
                                BJA.PaiHS = XJHS;
                                BJA.PaiScare = AXCount;
                                BJA.type = 7;
                                userT.jp.Add(BJA);
                                userj += BJA.PaiScare;
                            }
                        }
                        #endregion
                        #region 星期鸡

                        if (r.is_xinqiji)
                        {


                            ///判断星期几
                            int week = Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"));
                            ///定义筒条万花色
                            if (week == 0)
                            {
                                week = 7;
                            }
                            int TiaoHS = week + 10;
                            int WHS = week + 20;

                            ///定义添加筒条万实体
                            ///筒
                            UserJPOne Tjp = new UserJPOne();
                            //条
                            UserJPOne TIAOJP = new UserJPOne();
                            //万
                            UserJPOne WJP = new UserJPOne();

                            int tongSCount = item.majiangs.FindAll(u => u.PaiHs == week).Count;
                            int tiaoSCount = item.majiangs.FindAll(u => u.PaiHs == TiaoHS).Count;
                            int wanSCount = item.majiangs.FindAll(u => u.PaiHs == WHS).Count;


                            int tongPCount = item.Peng.FindAll(u => u.PaiHs == week).Count;
                            int tiaoPCount = item.Peng.FindAll(u => u.PaiHs == TiaoHS).Count;
                            int wanPCount = item.Peng.FindAll(u => u.PaiHs == WHS).Count;

                            int tongCCount = item.chuda.FindAll(u => u.PaiHs == week).Count;
                            int tiaoCCount = item.chuda.FindAll(u => u.PaiHs == TiaoHS).Count;
                            int wanCCount = item.chuda.FindAll(u => u.PaiHs == WHS).Count;


                            ///筒总分
                            int AXQToCount = tongSCount + tongPCount + tongCCount;
                            ///条总分
                            int AXQTiCount = tiaoSCount + tiaoPCount + tiaoCCount;
                            ///万总分
                            int AXQWCount = wanSCount + wanPCount + wanCCount;
                            if (!string.IsNullOrEmpty(item.Gong))
                            {
                                string GangXq = string.Copy(item.Gong);
                                string[] arrXq = GangXq.Split(',');
                                foreach (var gang in arrXq)
                                {
                                    if (gang.Contains(majiang.PaiHs.ToString()))
                                    {
                                        string[] itemarr = gang.Split('|');
                                        if (itemarr[0] == majiang.PaiHs.ToString())
                                        {
                                            AXQToCount = AXQToCount + 4;
                                            break;
                                        }
                                    }
                                }
                            }


                            if (AXQToCount > 0)
                            {
                                Tjp.PaiHS = week;
                                Tjp.PaiScare = AXQToCount;
                                Tjp.type = 5;
                                userT.jp.Add(Tjp);
                                userj += Tjp.PaiScare;
                            }

                            if (!string.IsNullOrEmpty(item.Gong))
                            {
                                if (item.Gong.Contains(TiaoHS.ToString()))
                                {
                                    AXQTiCount = AXQTiCount + 4;
                                }
                            }

                            if (AXQTiCount > 0)
                            {
                                TIAOJP.PaiHS = TiaoHS;

                                TIAOJP.PaiScare = AXQTiCount;
                                TIAOJP.type = 5;
                                userT.jp.Add(TIAOJP);
                                userj += TIAOJP.PaiScare;
                            }
                            if (!string.IsNullOrEmpty(item.Gong))
                            {
                                if (item.Gong.Contains(WHS.ToString()))
                                {
                                    AXQWCount = AXQWCount + 4;
                                }
                            }

                            if (AXQWCount > 0)
                            {
                                WJP.PaiHS = WHS;
                                WJP.PaiScare = AXQWCount;
                                WJP.type = 5;
                                userT.jp.Add(WJP);
                                userj += WJP.PaiScare;
                            }
                        }
                        #endregion
                        #region 判断杠
                        if (!string.IsNullOrEmpty(item.Gong))
                        {


                            string[] GangARR = item.Gong.Split(',');
                            foreach (var itemGang in GangARR)
                            {
                                UserGangJS GangModel = new UserGangJS();
                                UserGangJS GangModelone = new UserGangJS();
                                if (itemGang.Contains("M"))
                                {
                                    string[] userGang = itemGang.Split('|');
                                    switch (userGang[1])
                                    {
                                        case "1":
                                            GangModelone.DScare = -3;
                                            GangModelone.DType = 1;
                                            user1.gang.Add(GangModelone);
                                            // user1.scare += GangModelone.DScare;
                                            danduScore[0] += GangModelone.DScare;
                                            break;
                                        case "2":
                                            GangModelone.DScare = -3;
                                            GangModelone.DType = 1;
                                            user2.gang.Add(GangModelone);
                                            // user2.scare += GangModelone.DScare;
                                            danduScore[1] += GangModelone.DScare;
                                            break;
                                        case "3":
                                            GangModelone.DScare = -3;
                                            GangModelone.DType = 1;
                                            user3.gang.Add(GangModelone);
                                            // user3.scare += GangModelone.DScare;
                                            danduScore[2] += GangModelone.DScare;
                                            break;
                                        case "4":
                                            GangModelone.DScare = -3;
                                            GangModelone.DType = 1;
                                            user4.gang.Add(GangModelone);
                                            //user4.scare += GangModelone.DScare;
                                            danduScore[3] += GangModelone.DScare;
                                            break;
                                    }
                                    GangModel.DScare = 3;
                                    GangModel.DType = 1;
                                    userT.gang.Add(GangModel);
                                    //userT.scare += GangModel.DScare;
                                    danduScore[item.ZS_Fw - 1] += GangModel.DScare;
                                }

                                if (itemGang.Contains("A"))
                                {
                                    GangModel.DScare = 3;
                                    GangModel.DType = 2;
                                    userT.gang.Add(GangModel);
                                    userj += GangModel.DScare;
                                }
                                if (itemGang.Contains("Z"))
                                {
                                    GangModel.DScare = 3;
                                    GangModel.DType = 3;
                                    userT.gang.Add(GangModel);
                                    userj += GangModel.DScare;
                                }
                            }
                        }


                        #endregion

                        if (item.ZS_Fw == huInfo.FWZ || huInfo.DXInfoList.Any(w => w.DXFW == item.ZS_Fw))
                        {  
                            switch (huInfo.Type)
                            {
                                case 1:
                                    userj += GetPaixinFen(jipai, item.paixinfs);
                                    userj += lianzhuangZIMOScore;
                                    userT.pai_type = item.paixinfs;
                                    userT.dy_fs = GetPaixinFen(jipai, item.paixinfs) + lianzhuangZIMOScore;

                                 
                                    #region 旧代码
                                    /* switch ()
                                                              {
                                                                  case 1:

                                                                      break;
                                                                  case 2:
                                                                      userj += jipai.qys;
                                                                      userj += jipai.zimo;
                                                                      userT.pai_type = 2;
                                                                      userT.dy_fs = jipai.qys + jipai.zimo;
                                                                      break;
                                                                  case 3:
                                                                      userj += jipai.dduizi;
                                                                      userj += jipai.zimo;
                                                                      userT.pai_type = item.paixinfs;
                                                                      userT.dy_fs = jipai.dduizi + jipai.zimo;
                                                                      break;
                                                                  case 4:
                                                                      userj += jipai.longqidui;
                                                                      userj += jipai.zimo;
                                                                      userT.pai_type = item.paixinfs;
                                                                      userT.dy_fs = jipai.longqidui + jipai.zimo;
                                                                      break;
                                                                  case 5:

                                                                      userj += jipai.zimo;
                                                                      userT.pai_type = item.paixinfs;
                                                                      userT.dy_fs = jipai.zimo;
                                                                      break;
                                                                  case 6:
                                                                      userj += jipai.xqidui;
                                                                      userj += jipai.qys;
                                                                      userj += jipai.zimo;
                                                                      userT.pai_type = item.paixinfs;
                                                                      userT.dy_fs = jipai.zimo + jipai.xqidui + jipai.qys;
                                                                      break;
                                                                  case 7:
                                                                      userj += jipai.qys * 2;
                                                                      userj += jipai.zimo;
                                                                      userT.pai_type = item.paixinfs;
                                                                      userT.dy_fs = jipai.qys * 2 + jipai.zimo;
                                                                      break;
                                                                  case 8:
                                                                      userj += jipai.dduizi;
                                                                      userj += jipai.qys;
                                                                      userj += jipai.zimo;
                                                                      userT.pai_type = item.paixinfs;
                                                                      userT.dy_fs = jipai.dduizi + jipai.qys + jipai.zimo;

                                                                      break;
                                                                  case 9:
                                                                      userj += jipai.longqidui;
                                                                      userj += jipai.qys;
                                                                      userj += jipai.zimo;
                                                                      userT.pai_type = item.paixinfs;


                                                                      userT.dy_fs = jipai.longqidui + jipai.qys + jipai.zimo;
                                                                      break;
                                                                  case 10:
                                                                      userj += jipai.qys;
                                                                      userj += jipai.zimo;
                                                                      userT.pai_type = item.paixinfs;

                                                                      userT.dy_fs = jipai.qys + jipai.zimo;
                                                                      break;
                                                                  default:
                                                                      break;
                                                              }*/
                                    #endregion
                                    break;
                                case 2:
                                    userT.dy_fs = GetPaixinFen(jipai, item.paixinfs);
                                    userT.scare += userT.dy_fs;
                                    userT.pai_type = item.paixinfs;
                                

                                    userList[huInfo.FWB-1].scare -= userT.dy_fs;
                                    userList[huInfo.FWB - 1].pai_type = 11;
                                    userList[huInfo.FWB - 1].dy_fs = (0 - userT.dy_fs);

                                    #region 旧代码
                                    //switch (huInfo.FWB)
                                    //{
                                    //    case 1:
                                    //        user1.scare -= GetPaixinFen(jipai, item.paixinfs);
                                    //        //user1.scare -= lianZhuangScore[0];
                                    //        user1.pai_type = 11;
                                    //        // user1.dy_fs = (0 - GetPaixinFen(jipai, item.paixinfs) + lianZhuangScore[0]);
                                    //        user1.dy_fs = (0 - GetPaixinFen(jipai, item.paixinfs));

                                    //        break;
                                    //    case 2:
                                    //        user2.scare -= GetPaixinFen(jipai, item.paixinfs);
                                    //        // user2.scare -= lianZhuangScore[1];
                                    //        user2.pai_type = 11;
                                    //        user2.dy_fs = (0 - GetPaixinFen(jipai, item.paixinfs));

                                    //        break;
                                    //    case 3:
                                    //        user3.scare -= GetPaixinFen(jipai, item.paixinfs);
                                    //        // user3.scare -= lianZhuangScore[2]; ;
                                    //        user3.dy_fs = (0 - GetPaixinFen(jipai, item.paixinfs)); ;
                                    //        user3.pai_type = 11;
                                    //        if (item.Is_tiant)
                                    //        {
                                    //            UserJPOne BJA = new UserJPOne();
                                    //            BJA.PaiHS = 100;
                                    //            BJA.PaiScare = -20;
                                    //            BJA.type = 10;
                                    //            user3.jp.Add(BJA);
                                    //            user3.scare += BJA.PaiScare;
                                    //        }
                                    //        if (item.Is_baotin)
                                    //        {
                                    //            UserJPOne BJA = new UserJPOne();
                                    //            BJA.PaiHS = 101;
                                    //            BJA.PaiScare = -10;
                                    //            BJA.type = 10;
                                    //            user3.jp.Add(BJA);
                                    //            user3.scare += BJA.PaiScare;
                                    //        }
                                    //        break;
                                    //    case 4:
                                    //        user4.scare -= GetPaixinFen(jipai, item.paixinfs);
                                    //        // user4.scare -= lianZhuangScore[3];
                                    //        user4.pai_type = 11;
                                    //        user4.dy_fs = (0 - GetPaixinFen(jipai, item.paixinfs)); ;
                                    //        if (item.Is_tiant)
                                    //        {
                                    //            UserJPOne BJA = new UserJPOne();
                                    //            BJA.PaiHS = 100;
                                    //            BJA.PaiScare = -20;
                                    //            BJA.type = 10;
                                    //            user4.jp.Add(BJA);
                                    //            user4.scare += BJA.PaiScare;
                                    //        }
                                    //        if (item.Is_baotin)
                                    //        {
                                    //            UserJPOne BJA = new UserJPOne();
                                    //            BJA.PaiHS = 101;
                                    //            BJA.PaiScare = -10;
                                    //            BJA.type = 10;
                                    //            user4.jp.Add(BJA);
                                    //            user4.scare += BJA.PaiScare;
                                    //        }
                                    //        break;
                                    //}
                                    //switch (item.paixinfs)
                                    //{
                                    //    case 1:
                                    //        userT.scare += jipai.xqidui;
                                    //        userT.pai_type = 1;

                                    //        userT.dy_fs = jipai.xqidui;
                                    //        switch (huInfo.Type)
                                    //        {
                                    //            case 1:
                                    //                user1.scare -= jipai.xqidui;
                                    //                user1.pai_type = 11;
                                    //                user1.dy_fs = 0 - jipai.xqidui;
                                    //                break;
                                    //            case 2:
                                    //                user2.scare -= jipai.xqidui;
                                    //                user2.pai_type = 11;
                                    //                user2.dy_fs = 0 - jipai.xqidui;
                                    //                break;
                                    //            case 3:
                                    //                user3.scare -= jipai.xqidui;
                                    //                user3.dy_fs = 0 - jipai.xqidui;
                                    //                user3.pai_type = 11;
                                    //                break;
                                    //            case 4:
                                    //                user4.scare -= jipai.xqidui;
                                    //                user4.pai_type = 11;

                                    //                user4.dy_fs = 0 - jipai.xqidui;
                                    //                break;
                                    //        }
                                    //        break;
                                    //    case 2:
                                    //        userT.scare += jipai.qys;
                                    //        userT.pai_type = 2;
                                    //        userT.dy_fs = jipai.qys;
                                    //        switch (huInfo.FWB)
                                    //        {
                                    //            case 1:
                                    //                user1.scare -= jipai.qys;
                                    //                user1.pai_type = 11;
                                    //                user1.dy_fs = 0 - jipai.qys;
                                    //                break;
                                    //            case 2:
                                    //                user2.scare -= jipai.qys;
                                    //                user2.pai_type = 11;
                                    //                user2.dy_fs = 0 - jipai.qys;
                                    //                break;
                                    //            case 3:
                                    //                user3.scare -= jipai.qys;
                                    //                user3.dy_fs = 0 - jipai.qys;
                                    //                user3.pai_type = 11;
                                    //                break;
                                    //            case 4:
                                    //                user4.scare -= jipai.qys;
                                    //                user4.dy_fs = 0 - jipai.qys;
                                    //                user4.pai_type = 11;
                                    //                break;
                                    //        }
                                    //        break;
                                    //    case 3:
                                    //        userT.scare += jipai.dduizi;

                                    //        userT.pai_type = item.paixinfs;
                                    //        userT.dy_fs = jipai.dduizi;
                                    //        switch (huInfo.FWB)
                                    //        {
                                    //            case 1:
                                    //                user1.scare -= jipai.dduizi;
                                    //                user1.pai_type = 11;
                                    //                user1.dy_fs = 0 - jipai.dduizi;
                                    //                break;
                                    //            case 2:
                                    //                user2.scare -= jipai.dduizi;
                                    //                user2.pai_type = 11;
                                    //                user2.dy_fs = 0 - jipai.dduizi;
                                    //                break;
                                    //            case 3:
                                    //                user3.scare -= jipai.dduizi;
                                    //                user3.pai_type = 11;
                                    //                user3.dy_fs = 0 - jipai.dduizi;
                                    //                break;
                                    //            case 4:
                                    //                user4.scare -= jipai.dduizi;
                                    //                user4.pai_type = 11;
                                    //                user4.dy_fs = 0 - jipai.dduizi;
                                    //                break;
                                    //        }
                                    //        break;
                                    //    case 4:
                                    //        userT.scare += jipai.longqidui;
                                    //        userT.pai_type = item.paixinfs;
                                    //        userT.dy_fs = jipai.longqidui;
                                    //        switch (huInfo.FWB)
                                    //        {
                                    //            case 1:
                                    //                user1.scare -= jipai.longqidui;
                                    //                user1.pai_type = 11;
                                    //                user1.dy_fs = 0 - jipai.longqidui;
                                    //                break;
                                    //            case 2:
                                    //                user2.scare -= jipai.longqidui;
                                    //                user2.pai_type = 11;
                                    //                user2.dy_fs = 0 - jipai.longqidui;
                                    //                break;
                                    //            case 3:
                                    //                user3.scare -= jipai.longqidui;
                                    //                user3.pai_type = 11;
                                    //                user3.dy_fs = 0 - jipai.longqidui;
                                    //                break;
                                    //            case 4:
                                    //                user4.scare -= jipai.longqidui;
                                    //                user4.pai_type = 11;
                                    //                user4.dy_fs = 0 - jipai.longqidui;
                                    //                break;
                                    //        }
                                    //        break;
                                    //    case 5:

                                    //        userT.scare += jipai.zimo;
                                    //        userT.pai_type = item.paixinfs;
                                    //        userT.dy_fs = jipai.zimo;
                                    //        switch (huInfo.FWB)
                                    //        {
                                    //            case 1:
                                    //                user1.scare -= jipai.zimo;
                                    //                user1.pai_type = 11;
                                    //                user1.dy_fs = 0 - jipai.zimo;
                                    //                break;
                                    //            case 2:
                                    //                user2.scare -= jipai.zimo;
                                    //                user2.pai_type = 11;
                                    //                user2.dy_fs = 0 - jipai.zimo;
                                    //                break;
                                    //            case 3:
                                    //                user3.scare -= jipai.zimo;
                                    //                user3.pai_type = 11;
                                    //                user3.dy_fs = 0 - jipai.zimo;
                                    //                break;
                                    //            case 4:
                                    //                user4.scare -= jipai.zimo;
                                    //                user4.pai_type = 11;
                                    //                user4.dy_fs = 0 - jipai.zimo;
                                    //                break;
                                    //        }
                                    //        break;
                                    //    case 6:
                                    //        userT.scare += jipai.xqidui;
                                    //        userT.scare += jipai.qys;

                                    //        userT.pai_type = item.paixinfs;
                                    //        userT.dy_fs = jipai.xqidui + jipai.qys;


                                    //        switch (huInfo.FWB)
                                    //        {
                                    //            case 1:
                                    //                user1.scare -= jipai.xqidui;
                                    //                user1.scare -= jipai.qys;
                                    //                user1.pai_type = 11;
                                    //                user1.dy_fs = 0 - (jipai.xqidui + jipai.qys);
                                    //                break;
                                    //            case 2:
                                    //                user2.scare -= jipai.xqidui;
                                    //                user2.scare -= jipai.qys;
                                    //                user2.pai_type = 11;
                                    //                user2.dy_fs = 0 - (jipai.xqidui + jipai.qys);
                                    //                break;
                                    //            case 3:
                                    //                user3.scare -= jipai.xqidui;
                                    //                user3.scare -= jipai.qys;
                                    //                user3.pai_type = 11;
                                    //                user3.dy_fs = 0 - (jipai.xqidui + jipai.qys);
                                    //                break;
                                    //            case 4:
                                    //                user4.scare -= jipai.xqidui;
                                    //                user4.scare -= jipai.qys;
                                    //                user4.pai_type = 11;
                                    //                user4.dy_fs = 0 - (jipai.xqidui + jipai.qys);
                                    //                break;
                                    //        }
                                    //        break;
                                    //    case 7:
                                    //        userT.scare += jipai.qys * 2;

                                    //        userT.pai_type = item.paixinfs;
                                    //        userT.dy_fs = jipai.qys * 2;
                                    //        switch (huInfo.FWB)
                                    //        {
                                    //            case 1:

                                    //                user1.scare -= jipai.qys * 2;
                                    //                user1.pai_type = 11;
                                    //                user1.dy_fs = 0 - jipai.qys * 2;
                                    //                break;
                                    //            case 2:

                                    //                user2.scare -= jipai.qys * 2;
                                    //                user2.pai_type = 11;
                                    //                user2.dy_fs = 0 - jipai.qys * 2;
                                    //                break;
                                    //            case 3:

                                    //                user3.scare -= jipai.qys * 2;
                                    //                user3.pai_type = 11;
                                    //                user3.dy_fs = 0 - jipai.qys * 2;
                                    //                break;
                                    //            case 4:

                                    //                user4.scare -= jipai.qys * 2;
                                    //                user4.pai_type = 11;
                                    //                user4.dy_fs = 0 - jipai.qys * 2;
                                    //                break;
                                    //        }
                                    //        break;
                                    //    case 8:
                                    //        userT.scare += jipai.dduizi;
                                    //        userT.scare += jipai.qys;

                                    //        userT.pai_type = item.paixinfs;
                                    //        userT.dy_fs = jipai.dduizi + jipai.qys;
                                    //        switch (huInfo.FWB)
                                    //        {
                                    //            case 1:
                                    //                user1.scare -= jipai.dduizi;
                                    //                user1.scare -= jipai.qys;
                                    //                user1.pai_type = 11;
                                    //                user1.dy_fs = 0 - (jipai.dduizi + jipai.qys);

                                    //                break;
                                    //            case 2:
                                    //                user2.scare -= jipai.dduizi;
                                    //                user2.scare -= jipai.qys;
                                    //                user2.pai_type = 11;
                                    //                user2.dy_fs = 0 - (jipai.dduizi + jipai.qys);
                                    //                break;
                                    //            case 3:
                                    //                user3.scare -= jipai.dduizi;
                                    //                user3.scare -= jipai.qys;
                                    //                user3.pai_type = 11;
                                    //                user3.dy_fs = 0 - (jipai.dduizi + jipai.qys);
                                    //                break;
                                    //            case 4:
                                    //                user4.scare -= jipai.dduizi;
                                    //                user4.scare -= jipai.qys;
                                    //                user4.pai_type = 11;
                                    //                user4.dy_fs = 0 - (jipai.dduizi + jipai.qys);
                                    //                break;
                                    //        }

                                    //        break;
                                    //    case 9:
                                    //        userT.scare += jipai.longqidui;
                                    //        userT.scare += jipai.qys;

                                    //        userT.pai_type = item.paixinfs;
                                    //        userT.dy_fs = jipai.longqidui + jipai.qys;
                                    //        switch (huInfo.FWB)
                                    //        {
                                    //            case 1:
                                    //                user1.scare -= jipai.longqidui;
                                    //                user1.scare -= jipai.qys;
                                    //                user1.pai_type = 11;

                                    //                user1.dy_fs = 0 - (jipai.longqidui + jipai.qys);
                                    //                break;
                                    //            case 2:
                                    //                user2.scare -= jipai.longqidui;
                                    //                user2.scare -= jipai.qys;
                                    //                user2.pai_type = 11;
                                    //                user2.dy_fs = 0 - (jipai.longqidui + jipai.qys);
                                    //                break;
                                    //            case 3:
                                    //                user3.scare -= jipai.longqidui;
                                    //                user3.scare -= jipai.qys;
                                    //                user3.pai_type = 11;
                                    //                user3.dy_fs = 0 - (jipai.longqidui + jipai.qys);
                                    //                break;
                                    //            case 4:
                                    //                user4.scare -= jipai.longqidui;
                                    //                user4.scare -= jipai.qys;
                                    //                user4.pai_type = 11;
                                    //                user4.dy_fs = 0 - (jipai.longqidui + jipai.qys);
                                    //                break;
                                    //        }

                                    //        break;
                                    //    case 10:
                                    //        userT.scare += jipai.qys;
                                    //        userT.pai_type = item.paixinfs;
                                    //        userT.dy_fs = jipai.qys;
                                    //        switch (huInfo.FWB)
                                    //        {
                                    //            case 1:

                                    //                user1.scare -= jipai.qys;
                                    //                user1.pai_type = 11;
                                    //                user1.dy_fs = 0 - jipai.qys;
                                    //                break;
                                    //            case 2:

                                    //                user2.scare -= jipai.qys;
                                    //                user2.pai_type = 11;
                                    //                user2.dy_fs = 0 - jipai.qys;
                                    //                break;
                                    //            case 3:

                                    //                user3.scare -= jipai.qys;
                                    //                user3.pai_type = 11;
                                    //                user3.dy_fs = 0 - jipai.qys;
                                    //                break;
                                    //            case 4:

                                    //                user4.scare -= jipai.qys;
                                    //                user4.pai_type = 11;
                                    //                user4.dy_fs = 0 - jipai.qys;
                                    //                break;
                                    //        }
                                    //        break;
                                    //    default:
                                    //        break;
                                    //} 
                                    #endregion
                                    break;
                                case 3:
                                    int scare = 0;
                                 
                                    scare += (scare + (GetPaixinFen(jipai, item.paixinfs) + jipai.zimo) * (mjlist.Count - 1));
                                    userT.scare += scare;
                                    userT.pai_type = item.paixinfs;
                                    userT.dy_fs = scare;// GetPaixinFen(jipai, item.paixinfs) + 9;
                                    
                                    userList[huInfo.FWB - 1].scare -= scare;
                                    userList[huInfo.FWB - 1].pai_type = 11;
                                    userList[huInfo.FWB - 1].dy_fs = (0 - scare);
                                    #region 旧代码
                                    //switch (huInfo.FWB)
                                    //{
                                    //    case 1:
                                    //        user1.scare -= scare;
                                    //        //user1.scare -= GetPaixinFen(jipai, item.paixinfs);
                                    //        //user1.scare -= 9;
                                    //        // user1.scare += lianZhuangScore[0];

                                    //        user1.pai_type = 11;
                                    //        user1.dy_fs = 0 - scare;//(GetPaixinFen(jipai, item.paixinfs) + 9);// + lianZhuangScore[0];
                                    //        if (item.Is_tiant)
                                    //        {
                                    //            UserJPOne BJA = new UserJPOne();
                                    //            BJA.PaiHS = 100;
                                    //            BJA.PaiScare = -20;
                                    //            BJA.type = 10;
                                    //            user1.jp.Add(BJA);
                                    //            user1.scare += BJA.PaiScare;
                                    //        }
                                    //        if (item.Is_baotin)
                                    //        {
                                    //            UserJPOne BJA = new UserJPOne();
                                    //            BJA.PaiHS = 101;
                                    //            BJA.PaiScare = -10;
                                    //            BJA.type = 10;
                                    //            user1.jp.Add(BJA);
                                    //            user1.scare += BJA.PaiScare;
                                    //        }
                                    //        break;
                                    //    case 2:

                                    //        //user2.scare -= GetPaixinFen(jipai, item.paixinfs);
                                    //        //user2.scare -= 9;
                                    //        //user2.scare += lianZhuangScore[1];
                                    //        user2.scare -= scare;
                                    //        user2.pai_type = 11;
                                    //        user2.dy_fs = 0 - scare;// (GetPaixinFen(jipai, item.paixinfs) + 9);// + lianZhuangScore[1];
                                    //        if (item.Is_tiant)
                                    //        {
                                    //            UserJPOne BJA = new UserJPOne();
                                    //            BJA.PaiHS = 100;
                                    //            BJA.PaiScare = -20;
                                    //            BJA.type = 10;
                                    //            user2.jp.Add(BJA);
                                    //            user2.scare += BJA.PaiScare;
                                    //        }
                                    //        if (item.Is_baotin)
                                    //        {
                                    //            UserJPOne BJA = new UserJPOne();
                                    //            BJA.PaiHS = 101;
                                    //            BJA.PaiScare = -10;
                                    //            BJA.type = 10;
                                    //            user2.jp.Add(BJA);
                                    //            user2.scare += BJA.PaiScare;
                                    //        }
                                    //        break;
                                    //    case 3:

                                    //        //user3.scare -= GetPaixinFen(jipai, item.paixinfs);
                                    //        //user3.scare -= 9;
                                    //        //user3.scare += lianZhuangScore[2];
                                    //        user3.scare -= scare;
                                    //        user3.pai_type = 11;
                                    //        user3.dy_fs = 0 - scare;// (GetPaixinFen(jipai, item.paixinfs) + 9);// + lianZhuangScore[2];
                                    //        if (item.Is_tiant)
                                    //        {
                                    //            UserJPOne BJA = new UserJPOne();
                                    //            BJA.PaiHS = 100;
                                    //            BJA.PaiScare = -20;
                                    //            BJA.type = 10;
                                    //            user2.jp.Add(BJA);
                                    //            user2.scare += BJA.PaiScare;
                                    //        }
                                    //        if (item.Is_baotin)
                                    //        {
                                    //            UserJPOne BJA = new UserJPOne();
                                    //            BJA.PaiHS = 101;
                                    //            BJA.PaiScare = -10;
                                    //            BJA.type = 10;
                                    //            user2.jp.Add(BJA);
                                    //            user2.scare += BJA.PaiScare;
                                    //        }
                                    //        break;
                                    //    case 4:

                                    //        //user4.scare -= GetPaixinFen(jipai, item.paixinfs);
                                    //        //user4.scare -= 9;
                                    //        // user4.scare += lianZhuangScore[3];
                                    //        user4.scare -= scare;
                                    //        user4.pai_type = 11;
                                    //        user4.dy_fs = 0 - scare;// (GetPaixinFen(jipai, item.paixinfs) + 9);// + lianZhuangScore[3];
                                    //        if (item.Is_tiant)
                                    //        {
                                    //            UserJPOne BJA = new UserJPOne();
                                    //            BJA.PaiHS = 100;
                                    //            BJA.PaiScare = -20;
                                    //            BJA.type = 10;
                                    //            user4.jp.Add(BJA);
                                    //            user4.scare += BJA.PaiScare;
                                    //        }
                                    //        if (item.Is_baotin)
                                    //        {
                                    //            UserJPOne BJA = new UserJPOne();
                                    //            BJA.PaiHS = 101;
                                    //            BJA.PaiScare = -10;
                                    //            BJA.type = 10;
                                    //            user4.jp.Add(BJA);
                                    //            user4.scare += BJA.PaiScare;
                                    //        }
                                    //        break;
                                    //}
                                    //switch (item.paixinfs)
                                    //{
                                    //    case 1:
                                    //        userT.scare += jipai.xqidui;
                                    //        userT.scare += 9;
                                    //        userT.pai_type = 1;
                                    //        userT.dy_fs = jipai.xqidui + 9;
                                    //        switch (huInfo.FWB)
                                    //        {
                                    //            case 1:

                                    //                user1.scare -= jipai.xqidui;
                                    //                user1.scare -= 9;
                                    //                user1.pai_type = 11;
                                    //                user1.dy_fs = 0 - (jipai.xqidui + 9);
                                    //                break;
                                    //            case 2:

                                    //                user2.scare -= jipai.xqidui;
                                    //                user2.scare -= 9;
                                    //                user2.pai_type = 11;
                                    //                user2.dy_fs = 0 - (jipai.xqidui + 9);
                                    //                break;
                                    //            case 3:

                                    //                user3.scare -= jipai.xqidui;
                                    //                user3.scare -= 9;
                                    //                user3.pai_type = 11;
                                    //                user3.dy_fs = 0 - (jipai.xqidui + 9);
                                    //                break;
                                    //            case 4:

                                    //                user4.scare -= jipai.xqidui;
                                    //                user4.scare -= 9;
                                    //                user4.pai_type = 11;
                                    //                user4.dy_fs = 0 - (jipai.xqidui + 9);
                                    //                break;
                                    //        }

                                    //        break;
                                    //    case 2:
                                    //        userT.scare += jipai.qys;
                                    //        userT.scare += 9;
                                    //        userT.pai_type = 2;
                                    //        userT.dy_fs = jipai.qys + 9;
                                    //        switch (huInfo.FWB)
                                    //        {
                                    //            case 1:

                                    //                user1.scare -= jipai.qys;
                                    //                user1.scare -= 9;
                                    //                user1.pai_type = 11;
                                    //                user1.dy_fs = 0 - (jipai.qys + 9);
                                    //                break;
                                    //            case 2:

                                    //                user2.scare -= jipai.qys;
                                    //                user2.scare -= 9;
                                    //                user2.pai_type = 11;
                                    //                user2.dy_fs = 0 - (jipai.qys + 9);
                                    //                break;
                                    //            case 3:

                                    //                user3.scare -= jipai.qys;
                                    //                user3.scare -= 9;
                                    //                user3.pai_type = 11;
                                    //                user3.dy_fs = 0 - (jipai.qys + 9);
                                    //                break;
                                    //            case 4:

                                    //                user4.scare -= jipai.qys;
                                    //                user4.scare -= 9;
                                    //                user4.pai_type = 11;
                                    //                user4.dy_fs = 0 - (jipai.qys + 9);
                                    //                break;
                                    //        }
                                    //        break;
                                    //    case 3:
                                    //        userT.scare += jipai.dduizi;
                                    //        userT.scare += 9;
                                    //        userT.pai_type = item.paixinfs;
                                    //        userT.dy_fs = jipai.dduizi + 9;
                                    //        switch (huInfo.FWB)
                                    //        {
                                    //            case 1:

                                    //                user1.scare -= jipai.dduizi;
                                    //                user1.scare -= 9;
                                    //                user1.pai_type = 11;
                                    //                user1.dy_fs = 0 - (jipai.dduizi + 9);
                                    //                break;
                                    //            case 2:

                                    //                user2.scare -= jipai.dduizi;
                                    //                user2.scare -= 9;
                                    //                user2.pai_type = 11;
                                    //                user2.dy_fs = 0 - (jipai.dduizi + 9);
                                    //                break;
                                    //            case 3:

                                    //                user3.scare -= jipai.dduizi;
                                    //                user3.scare -= 9;
                                    //                user3.pai_type = 11;
                                    //                user3.dy_fs = 0 - (jipai.dduizi + 9);
                                    //                break;
                                    //            case 4:

                                    //                user4.scare -= jipai.dduizi;
                                    //                user4.scare -= 9;
                                    //                user4.pai_type = 11;
                                    //                user4.dy_fs = 0 - (jipai.dduizi + 9);
                                    //                break;
                                    //        }

                                    //        break;
                                    //    case 4:
                                    //        userT.scare += jipai.longqidui;
                                    //        userT.scare += 9;
                                    //        userT.pai_type = item.paixinfs;
                                    //        userT.dy_fs = jipai.longqidui + 9;
                                    //        switch (huInfo.FWB)
                                    //        {
                                    //            case 1:

                                    //                user1.scare -= jipai.longqidui;
                                    //                user1.scare -= 9;
                                    //                user1.pai_type = 11;
                                    //                user1.dy_fs = 0 - (jipai.longqidui + 9);
                                    //                break;
                                    //            case 2:

                                    //                user2.scare -= jipai.longqidui;
                                    //                user2.scare -= 9;
                                    //                user2.pai_type = 11;
                                    //                user2.dy_fs = 0 - (jipai.longqidui + 9);
                                    //                break;
                                    //            case 3:

                                    //                user3.scare -= jipai.longqidui;
                                    //                user3.scare -= 9;
                                    //                user3.pai_type = 11;
                                    //                user3.dy_fs = 0 - (jipai.longqidui + 9);
                                    //                break;
                                    //            case 4:

                                    //                user4.scare -= jipai.longqidui;
                                    //                user4.scare -= 9;
                                    //                user4.pai_type = 11;
                                    //                user4.dy_fs = 0 - (jipai.longqidui + 9);
                                    //                break;
                                    //        }

                                    //        break;
                                    //    case 5:
                                    //        userT.scare += 9;

                                    //        userT.pai_type = item.paixinfs;
                                    //        userT.dy_fs = 9;

                                    //        switch (huInfo.FWB)
                                    //        {
                                    //            case 1:


                                    //                user1.scare -= 9;
                                    //                user1.pai_type = 11;
                                    //                user1.dy_fs = -9;
                                    //                break;
                                    //            case 2:

                                    //                user2.scare -= 9;
                                    //                user2.pai_type = 11;
                                    //                user2.dy_fs = -9;
                                    //                break;
                                    //            case 3:


                                    //                user3.scare -= 9;
                                    //                user3.pai_type = 11;
                                    //                user3.dy_fs = -9;
                                    //                break;
                                    //            case 4:


                                    //                user4.scare -= 9;
                                    //                user4.pai_type = 11;
                                    //                user4.dy_fs = -9;
                                    //                break;
                                    //        }
                                    //        break;
                                    //    case 6:
                                    //        userT.scare += jipai.xqidui;
                                    //        userT.scare += jipai.qys;
                                    //        userT.scare += 9;
                                    //        userT.pai_type = item.paixinfs;
                                    //        userT.dy_fs = jipai.xqidui + jipai.qys + 9;

                                    //        switch (huInfo.FWB)
                                    //        {
                                    //            case 1:
                                    //                user1.scare -= jipai.xqidui;
                                    //                user1.scare -= jipai.qys;
                                    //                user1.scare -= 9;
                                    //                user1.pai_type = 11;

                                    //                user1.dy_fs = 0 - (jipai.xqidui + jipai.qys + 9);

                                    //                break;
                                    //            case 2:
                                    //                user2.scare -= jipai.xqidui;
                                    //                user2.scare -= jipai.qys;
                                    //                user2.scare -= 9;
                                    //                user2.pai_type = 11;
                                    //                user2.dy_fs = 0 - (jipai.xqidui + jipai.qys + 9);
                                    //                break;
                                    //            case 3:

                                    //                user3.scare -= jipai.xqidui;
                                    //                user3.scare -= jipai.qys;
                                    //                user3.scare -= 9;
                                    //                user3.pai_type = 11;
                                    //                user3.dy_fs = 0 - (jipai.xqidui + jipai.qys + 9);
                                    //                break;
                                    //            case 4:

                                    //                user4.scare -= jipai.xqidui;
                                    //                user4.scare -= jipai.qys;
                                    //                user4.scare -= 9;
                                    //                user4.pai_type = 11;
                                    //                user4.dy_fs = 0 - (jipai.xqidui + jipai.qys + 9);
                                    //                break;
                                    //        }
                                    //        break;
                                    //    case 7:
                                    //        userT.scare += jipai.qys * 2;
                                    //        userT.scare += 9;
                                    //        userT.pai_type = item.paixinfs;
                                    //        userT.dy_fs = jipai.qys * 2 + 9;

                                    //        switch (huInfo.FWB)
                                    //        {
                                    //            case 1:

                                    //                user1.scare -= jipai.qys * 2;
                                    //                user1.scare -= 9;
                                    //                user1.pai_type = 11;
                                    //                user1.dy_fs = 0 - (jipai.qys * 2 + 9);
                                    //                break;
                                    //            case 2:

                                    //                user2.scare -= jipai.qys * 2;
                                    //                user2.scare -= 9;
                                    //                user2.pai_type = 11;
                                    //                user2.dy_fs = 0 - (jipai.qys * 2 + 9);
                                    //                break;
                                    //            case 3:


                                    //                user3.scare -= jipai.qys * 2;
                                    //                user3.scare -= 9;
                                    //                user3.pai_type = 11;
                                    //                user3.dy_fs = 0 - (jipai.qys * 2 + 9);
                                    //                break;
                                    //            case 4:


                                    //                user4.scare -= jipai.qys * 2;
                                    //                user4.scare -= 9;
                                    //                user4.pai_type = 11;
                                    //                user4.dy_fs = 0 - (jipai.qys * 2 + 9);
                                    //                break;
                                    //        }

                                    //        break;
                                    //    case 8:
                                    //        userT.scare += jipai.dduizi;
                                    //        userT.scare += jipai.qys;
                                    //        userT.scare += 9;
                                    //        userT.pai_type = item.paixinfs;
                                    //        userT.dy_fs = jipai.dduizi + jipai.qys + 9;

                                    //        switch (huInfo.FWB)
                                    //        {
                                    //            case 1:
                                    //                user1.scare -= jipai.dduizi;
                                    //                user1.scare -= jipai.qys;
                                    //                user1.scare -= 9;
                                    //                user1.pai_type = 11;
                                    //                user1.dy_fs = 0 - (jipai.dduizi + jipai.qys + 9);
                                    //                break;
                                    //            case 2:
                                    //                user2.scare -= jipai.dduizi;
                                    //                user2.scare -= jipai.qys;
                                    //                user2.scare -= 9;
                                    //                user2.pai_type = 11;
                                    //                user2.dy_fs = 0 - (jipai.dduizi + jipai.qys + 9);
                                    //                break;
                                    //            case 3:

                                    //                user3.scare -= jipai.dduizi;
                                    //                user3.scare -= jipai.qys;
                                    //                user3.scare -= 9;
                                    //                user3.pai_type = 11;
                                    //                user3.dy_fs = 0 - (jipai.dduizi + jipai.qys + 9);
                                    //                break;
                                    //            case 4:

                                    //                user4.scare -= jipai.dduizi;
                                    //                user4.scare -= jipai.qys;
                                    //                user4.scare -= 9;
                                    //                user4.pai_type = 11;
                                    //                user4.dy_fs = 0 - (jipai.dduizi + jipai.qys + 9);
                                    //                break;
                                    //        }

                                    //        break;
                                    //    case 9:
                                    //        userT.scare += jipai.longqidui;
                                    //        userT.scare += jipai.qys;
                                    //        userT.scare += 9;
                                    //        userT.pai_type = item.paixinfs;
                                    //        userT.dy_fs = jipai.longqidui + jipai.qys + 9;
                                    //        switch (huInfo.FWB)
                                    //        {
                                    //            case 1:
                                    //                user1.scare -= jipai.longqidui;
                                    //                user1.scare -= jipai.qys;
                                    //                user1.scare -= 9;
                                    //                user1.pai_type = 11;
                                    //                user1.dy_fs = 0 - (jipai.longqidui + jipai.qys + 9);
                                    //                break;
                                    //            case 2:
                                    //                user2.scare -= jipai.longqidui;
                                    //                user2.scare -= jipai.qys;
                                    //                user2.scare -= 9;
                                    //                user2.pai_type = 11;
                                    //                user2.dy_fs = 0 - (jipai.longqidui + jipai.qys + 9);
                                    //                break;
                                    //            case 3:

                                    //                user3.scare -= jipai.longqidui;
                                    //                user3.scare -= jipai.qys;
                                    //                user3.scare -= 9;
                                    //                user3.pai_type = 11;
                                    //                user3.dy_fs = 0 - (jipai.longqidui + jipai.qys + 9);
                                    //                break;
                                    //            case 4:

                                    //                user4.scare -= jipai.longqidui;
                                    //                user4.scare -= jipai.qys;
                                    //                user4.scare -= 9;
                                    //                user4.pai_type = 11;
                                    //                user4.dy_fs = 0 - (jipai.longqidui + jipai.qys + 9);
                                    //                break;
                                    //        }
                                    //        break;
                                    //    case 10:
                                    //        userT.scare += jipai.qys;
                                    //        userT.scare += 9;
                                    //        userT.pai_type = item.paixinfs;
                                    //        userT.dy_fs = jipai.qys + 9;
                                    //        switch (huInfo.FWB)
                                    //        {
                                    //            case 1:
                                    //                user1.scare -= jipai.qys;
                                    //                user1.scare -= 9;
                                    //                user1.pai_type = 11;
                                    //                user1.dy_fs = 0 - (jipai.qys + 9);
                                    //                break;
                                    //            case 2:
                                    //                user2.scare -= jipai.qys;
                                    //                user2.scare -= 9;
                                    //                user2.pai_type = 11;
                                    //                user2.dy_fs = 0 - (jipai.qys + 9);
                                    //                break;
                                    //            case 3:
                                    //                user3.scare -= jipai.qys;
                                    //                user3.scare -= 9;
                                    //                user3.pai_type = 11;
                                    //                user3.dy_fs = 0 - (jipai.qys + 9);
                                    //                break;
                                    //            case 4:
                                    //                user4.scare -= jipai.qys;
                                    //                user4.scare -= 9;
                                    //                user4.pai_type = 11;
                                    //                user4.dy_fs = 0 - (jipai.qys + 9);
                                    //                break;
                                    //        }

                                    //        break;
                                    //    default:
                                    //        break;
                                    //} 
                                    #endregion

                                    break;
                                case 4:
                                    userT.dy_fs = GetPaixinFen(jipai, item.paixinfs) + 3;
                                    userT.scare += userT.dy_fs;
                                    userT.pai_type = item.paixinfs;
                                 
                                    userList[huInfo.FWB - 1].scare -= userT.dy_fs;
                                    userList[huInfo.FWB - 1].pai_type = 11;
                                    userList[huInfo.FWB - 1].dy_fs = userT.dy_fs;
                                    #region 旧代码
                                    //switch (huInfo.FWB)
                                    //{
                                    //    case 1:
                                    //        user1.scare -= GetPaixinFen(jipai, item.paixinfs);
                                    //        user1.scare -= 3;
                                    //        // user1.scare += lianZhuangScore[0];
                                    //        user1.pai_type = 11;
                                    //        user1.dy_fs += 0 - (GetPaixinFen(jipai, item.paixinfs) + 3);
                                    //        if (item.Is_tiant)
                                    //        {
                                    //            UserJPOne BJA = new UserJPOne();
                                    //            BJA.PaiHS = 100;
                                    //            BJA.PaiScare = -20;
                                    //            BJA.type = 10;
                                    //            user1.jp.Add(BJA);
                                    //            user1.scare += BJA.PaiScare;
                                    //        }
                                    //        if (item.Is_baotin)
                                    //        {
                                    //            UserJPOne BJA = new UserJPOne();
                                    //            BJA.PaiHS = 101;
                                    //            BJA.PaiScare = -10;
                                    //            BJA.type = 10;
                                    //            user1.jp.Add(BJA);
                                    //            user1.scare += BJA.PaiScare;
                                    //        }
                                    //        break;

                                    //    case 2:
                                    //        user2.scare -= GetPaixinFen(jipai, item.paixinfs);
                                    //        user2.scare -= 3;
                                    //        // user2.scare += lianZhuangScore[1];
                                    //        user2.pai_type = 11;
                                    //        user2.dy_fs += 0 - (GetPaixinFen(jipai, item.paixinfs) + 3);
                                    //        if (item.Is_tiant)
                                    //        {
                                    //            UserJPOne BJA = new UserJPOne();
                                    //            BJA.PaiHS = 100;
                                    //            BJA.PaiScare = -20;
                                    //            BJA.type = 10;
                                    //            user2.jp.Add(BJA);
                                    //            user2.scare += BJA.PaiScare;
                                    //        }
                                    //        if (item.Is_baotin)
                                    //        {
                                    //            UserJPOne BJA = new UserJPOne();
                                    //            BJA.PaiHS = 101;
                                    //            BJA.PaiScare = -10;
                                    //            BJA.type = 10;
                                    //            user2.jp.Add(BJA);
                                    //            user2.scare += BJA.PaiScare;
                                    //        }
                                    //        break;
                                    //    case 3:
                                    //        user3.scare -= GetPaixinFen(jipai, item.paixinfs);
                                    //        user3.scare -= 3;
                                    //        //user3.scare += lianZhuangScore[2];
                                    //        user3.pai_type = 11;
                                    //        user3.dy_fs += 0 - (GetPaixinFen(jipai, item.paixinfs) + 3);
                                    //        if (item.Is_tiant)
                                    //        {
                                    //            UserJPOne BJA = new UserJPOne();
                                    //            BJA.PaiHS = 100;
                                    //            BJA.PaiScare = -20;
                                    //            BJA.type = 10;
                                    //            user2.jp.Add(BJA);
                                    //            user2.scare += BJA.PaiScare;
                                    //        }
                                    //        if (item.Is_baotin)
                                    //        {
                                    //            UserJPOne BJA = new UserJPOne();
                                    //            BJA.PaiHS = 101;
                                    //            BJA.PaiScare = -10;
                                    //            BJA.type = 10;
                                    //            user2.jp.Add(BJA);
                                    //            user2.scare += BJA.PaiScare;
                                    //        }
                                    //        break;
                                    //    case 4:
                                    //        user4.scare -= GetPaixinFen(jipai, item.paixinfs);
                                    //        user4.scare -= 3;
                                    //        // user4.scare += lianZhuangScore[3];
                                    //        user4.pai_type = 11;
                                    //        user4.dy_fs += 0 - (GetPaixinFen(jipai, item.paixinfs) + 3);
                                    //        if (item.Is_tiant)
                                    //        {
                                    //            UserJPOne BJA = new UserJPOne();
                                    //            BJA.PaiHS = 100;
                                    //            BJA.PaiScare = -20;
                                    //            BJA.type = 10;
                                    //            user2.jp.Add(BJA);
                                    //            user2.scare += BJA.PaiScare;
                                    //        }
                                    //        if (item.Is_baotin)
                                    //        {
                                    //            UserJPOne BJA = new UserJPOne();
                                    //            BJA.PaiHS = 101;
                                    //            BJA.PaiScare = -10;
                                    //            BJA.type = 10;
                                    //            user2.jp.Add(BJA);
                                    //            user2.scare += BJA.PaiScare;
                                    //        }
                                    //        break;
                                    //    default:
                                    //        break;
                                    //}
                                    //switch (item.paixinfs)
                                    //{
                                    //    case 1:
                                    //        userT.scare += jipai.xqidui;
                                    //        userT.scare += 3;
                                    //        userT.pai_type = 1;
                                    //        userT.dy_fs = jipai.xqidui + 3;
                                    //        switch (huInfo.FWB)
                                    //        {
                                    //            case 1:
                                    //                user1.scare -= 3;
                                    //                user1.scare -= jipai.xqidui;
                                    //                user1.pai_type = 11;
                                    //                user1.dy_fs = 0 - (jipai.xqidui + 3);
                                    //                break;

                                    //            case 2:
                                    //                user2.scare -= 3;
                                    //                user2.scare -= jipai.xqidui;
                                    //                user2.pai_type = 11;
                                    //                user2.dy_fs = 0 - (jipai.xqidui + 3);
                                    //                break;
                                    //            case 3:
                                    //                user3.scare -= 3;
                                    //                user3.scare -= jipai.xqidui;
                                    //                user3.pai_type = 11;
                                    //                user3.dy_fs = 0 - (jipai.xqidui + 3);
                                    //                break;
                                    //            case 4:
                                    //                user4.scare -= 3;
                                    //                user4.scare -= jipai.xqidui;
                                    //                user4.pai_type = 11;
                                    //                user4.dy_fs = 0 - (jipai.xqidui + 3);
                                    //                break;
                                    //            default:
                                    //                break;
                                    //        }
                                    //        break;
                                    //    case 2:
                                    //        userT.scare += jipai.qys;
                                    //        userT.scare += 3;
                                    //        userT.pai_type = 2;
                                    //        userT.dy_fs = jipai.qys + 3;


                                    //        switch (huInfo.FWB)
                                    //        {
                                    //            case 1:
                                    //                user1.scare -= 3;
                                    //                user1.scare -= jipai.qys;
                                    //                user1.pai_type = 11;
                                    //                user1.dy_fs = 0 - (jipai.qys + 3);
                                    //                break;

                                    //            case 2:
                                    //                user2.scare -= 3;
                                    //                user2.scare -= jipai.qys;
                                    //                user2.pai_type = 11;
                                    //                user2.dy_fs = 0 - (jipai.qys + 3);
                                    //                break;
                                    //            case 3:
                                    //                user3.scare -= 3;
                                    //                user3.scare -= jipai.qys;
                                    //                user3.pai_type = 11;
                                    //                user3.dy_fs = 0 - (jipai.qys + 3);
                                    //                break;
                                    //            case 4:
                                    //                user4.scare -= 3;
                                    //                user4.scare -= jipai.qys;
                                    //                user4.pai_type = 11;
                                    //                user4.dy_fs = 0 - (jipai.qys + 3);
                                    //                break;
                                    //            default:
                                    //                break;
                                    //        }
                                    //        break;
                                    //    case 3:
                                    //        userT.scare += jipai.dduizi;
                                    //        userT.scare += 3;
                                    //        userT.pai_type = item.paixinfs;
                                    //        userT.dy_fs = jipai.dduizi + 3;


                                    //        switch (huInfo.FWB)
                                    //        {
                                    //            case 1:
                                    //                user1.scare -= 3;
                                    //                user1.scare -= jipai.dduizi;
                                    //                user1.pai_type = 11;
                                    //                user1.dy_fs = 0 - (jipai.dduizi + 3);
                                    //                break;

                                    //            case 2:
                                    //                user2.scare -= 3;
                                    //                user2.scare -= jipai.dduizi;
                                    //                user2.pai_type = 11;
                                    //                user2.dy_fs = 0 - (jipai.dduizi + 3);
                                    //                break;
                                    //            case 3:
                                    //                user3.scare -= 3;
                                    //                user3.scare -= jipai.dduizi;
                                    //                user3.pai_type = 11;
                                    //                user3.dy_fs = 0 - (jipai.dduizi + 3);
                                    //                break;
                                    //            case 4:
                                    //                user4.scare -= 3;
                                    //                user4.scare -= jipai.dduizi;
                                    //                user4.pai_type = 11;
                                    //                user4.dy_fs = 0 - (jipai.dduizi + 3);
                                    //                break;
                                    //            default:
                                    //                break;
                                    //        }

                                    //        break;
                                    //    case 4:
                                    //        userT.scare += jipai.longqidui;
                                    //        userT.scare += 3;
                                    //        userT.pai_type = item.paixinfs;

                                    //        userT.dy_fs = jipai.longqidui + 3;


                                    //        switch (huInfo.FWB)
                                    //        {
                                    //            case 1:
                                    //                user1.scare -= 3;
                                    //                user1.scare -= jipai.longqidui;
                                    //                user1.pai_type = 11;
                                    //                user1.dy_fs = 0 - (jipai.longqidui + 3);
                                    //                break;

                                    //            case 2:
                                    //                user2.scare -= 3;
                                    //                user2.scare -= jipai.longqidui;
                                    //                user2.pai_type = 11;
                                    //                user2.dy_fs = 0 - (jipai.longqidui + 3);
                                    //                break;
                                    //            case 3:
                                    //                user3.scare -= 3;
                                    //                user3.scare -= jipai.longqidui;
                                    //                user3.pai_type = 11;
                                    //                user3.dy_fs = 0 - (jipai.longqidui + 3);
                                    //                break;
                                    //            case 4:
                                    //                user4.scare -= 3;
                                    //                user4.scare -= jipai.longqidui;
                                    //                user4.pai_type = 11;
                                    //                user4.dy_fs = 0 - (jipai.longqidui + 3);
                                    //                break;
                                    //            default:
                                    //                break;
                                    //        }
                                    //        break;
                                    //    case 5:
                                    //        userT.scare += 3;
                                    //        userT.scare += jipai.zimo;
                                    //        userT.pai_type = item.paixinfs;

                                    //        userT.dy_fs = jipai.zimo + 3;

                                    //        switch (huInfo.FWB)
                                    //        {
                                    //            case 1:
                                    //                user1.scare -= 3;
                                    //                user1.scare -= jipai.zimo;
                                    //                user1.pai_type = 11;
                                    //                user1.dy_fs = 0 - (jipai.zimo + 3);
                                    //                break;

                                    //            case 2:
                                    //                user2.scare -= 3;
                                    //                user2.scare -= jipai.zimo;
                                    //                user2.pai_type = 11;
                                    //                user2.dy_fs = 0 - (jipai.zimo + 3);
                                    //                break;
                                    //            case 3:
                                    //                user3.scare -= 3;
                                    //                user3.scare -= jipai.zimo;
                                    //                user3.pai_type = 11;
                                    //                user3.dy_fs = 0 - (jipai.zimo + 3);
                                    //                break;
                                    //            case 4:
                                    //                user4.scare -= 3;
                                    //                user4.scare -= jipai.zimo;
                                    //                user4.pai_type = 11;
                                    //                user4.dy_fs = 0 - (jipai.zimo + 3);
                                    //                break;
                                    //            default:
                                    //                break;
                                    //        }

                                    //        break;
                                    //    case 6:
                                    //        userT.scare += jipai.xqidui;
                                    //        userT.scare += jipai.qys;
                                    //        userT.scare += 3;
                                    //        userT.pai_type = item.paixinfs;

                                    //        userT.dy_fs = jipai.xqidui + 3 + jipai.qys;




                                    //        switch (huInfo.FWB)
                                    //        {
                                    //            case 1:
                                    //                user1.scare -= 3;
                                    //                user1.scare -= jipai.xqidui;
                                    //                user1.scare -= jipai.qys;
                                    //                user1.pai_type = 11;
                                    //                user1.dy_fs = 0 - (jipai.xqidui + jipai.qys + 3);
                                    //                break;

                                    //            case 2:


                                    //                user2.scare -= 3;
                                    //                user2.scare -= jipai.xqidui;
                                    //                user2.scare -= jipai.qys;
                                    //                user2.pai_type = 11;
                                    //                user2.dy_fs = 0 - (jipai.xqidui + jipai.qys + 3);
                                    //                break;
                                    //            case 3:

                                    //                user3.scare -= 3;
                                    //                user3.scare -= jipai.xqidui;
                                    //                user3.scare -= jipai.qys;
                                    //                user3.pai_type = 11;
                                    //                user3.dy_fs = 0 - (jipai.xqidui + jipai.qys + 3);


                                    //                break;
                                    //            case 4:


                                    //                user4.scare -= 3;
                                    //                user4.scare -= jipai.xqidui;
                                    //                user4.scare -= jipai.qys;
                                    //                user4.pai_type = 11;
                                    //                user4.dy_fs = 0 - (jipai.xqidui + jipai.qys + 3);

                                    //                break;
                                    //            default:
                                    //                break;
                                    //        }

                                    //        break;
                                    //    case 7:
                                    //        userT.scare += jipai.qys * 2;
                                    //        userT.scare += 3;
                                    //        userT.pai_type = item.paixinfs;

                                    //        userT.dy_fs = jipai.qys * 2 + 3;

                                    //        switch (huInfo.FWB)
                                    //        {
                                    //            case 1:
                                    //                user1.scare -= 3;

                                    //                user1.scare -= jipai.qys * 2;
                                    //                user1.pai_type = 11;
                                    //                user1.dy_fs = 0 - (jipai.qys * 2 + 3);
                                    //                break;

                                    //            case 2:

                                    //                user2.scare -= 3;

                                    //                user2.scare -= jipai.qys * 2;
                                    //                user2.pai_type = 11;
                                    //                user2.dy_fs = 0 - (jipai.qys * 2 + 3);

                                    //                break;
                                    //            case 3:

                                    //                user3.scare -= 3;

                                    //                user3.scare -= jipai.qys * 2;
                                    //                user3.pai_type = 11;
                                    //                user3.dy_fs = 0 - (jipai.qys * 2 + 3);




                                    //                break;
                                    //            case 4:

                                    //                user4.scare -= 3;

                                    //                user4.scare -= jipai.qys * 2;
                                    //                user4.pai_type = 11;
                                    //                user4.dy_fs = 0 - (jipai.qys * 2 + 3);



                                    //                break;
                                    //            default:
                                    //                break;
                                    //        }

                                    //        break;
                                    //    case 8:
                                    //        userT.scare += jipai.dduizi;
                                    //        userT.scare += jipai.qys;
                                    //        userT.scare += 3;
                                    //        userT.pai_type = item.paixinfs;

                                    //        userT.dy_fs = jipai.qys + jipai.dduizi + 3;

                                    //        switch (huInfo.FWB)
                                    //        {
                                    //            case 1:
                                    //                user1.scare -= 3;
                                    //                user1.scare -= jipai.dduizi;
                                    //                user1.scare -= jipai.qys;
                                    //                user1.pai_type = 11;
                                    //                user1.dy_fs = 0 - (jipai.dduizi + jipai.qys + 3);
                                    //                break;

                                    //            case 2:


                                    //                user2.scare -= 3;
                                    //                user2.scare -= jipai.dduizi;
                                    //                user2.scare -= jipai.qys;
                                    //                user2.pai_type = 11;
                                    //                user2.dy_fs = 0 - (jipai.dduizi + jipai.qys + 3);
                                    //                break;
                                    //            case 3:

                                    //                user3.scare -= 3;
                                    //                user3.scare -= jipai.dduizi;
                                    //                user3.scare -= jipai.qys;
                                    //                user3.pai_type = 11;
                                    //                user3.dy_fs = 0 - (jipai.dduizi + jipai.qys + 3);


                                    //                break;
                                    //            case 4:


                                    //                user4.scare -= 3;
                                    //                user4.scare -= jipai.dduizi;
                                    //                user4.scare -= jipai.qys;
                                    //                user4.pai_type = 11;
                                    //                user4.dy_fs = 0 - (jipai.dduizi + jipai.qys + 3);

                                    //                break;
                                    //            default:
                                    //                break;
                                    //        }

                                    //        break;
                                    //    case 9:
                                    //        userT.scare += jipai.longqidui;
                                    //        userT.scare += jipai.qys;
                                    //        userT.scare += 3;
                                    //        userT.pai_type = item.paixinfs;

                                    //        userT.dy_fs = jipai.longqidui + jipai.qys + 3;


                                    //        switch (huInfo.FWB)
                                    //        {
                                    //            case 1:
                                    //                user1.scare -= 3;
                                    //                user1.scare -= jipai.longqidui;
                                    //                user1.scare -= jipai.qys;
                                    //                user1.pai_type = 11;
                                    //                user1.dy_fs = 0 - (jipai.longqidui + jipai.qys + 3);
                                    //                break;

                                    //            case 2:


                                    //                user2.scare -= 3;
                                    //                user2.scare -= jipai.longqidui;
                                    //                user2.scare -= jipai.qys;
                                    //                user2.pai_type = 11;
                                    //                user2.dy_fs = 0 - (jipai.longqidui + jipai.qys + 3);
                                    //                break;
                                    //            case 3:

                                    //                user3.scare -= 3;
                                    //                user3.scare -= jipai.longqidui;
                                    //                user3.scare -= jipai.qys;
                                    //                user3.pai_type = 11;
                                    //                user3.dy_fs = 0 - (jipai.longqidui + jipai.qys + 3);


                                    //                break;
                                    //            case 4:


                                    //                user4.scare -= 3;
                                    //                user4.scare -= jipai.longqidui;
                                    //                user4.scare -= jipai.qys;
                                    //                user4.pai_type = 11;
                                    //                user4.dy_fs = 0 - (jipai.longqidui + jipai.qys + 3);

                                    //                break;
                                    //            default:
                                    //                break;
                                    //        }

                                    //        break;
                                    //    case 10:
                                    //        userT.scare += 3;
                                    //        userT.scare += jipai.qys;
                                    //        userT.pai_type = item.paixinfs;

                                    //        userT.dy_fs = jipai.qys + 3;

                                    //        switch (huInfo.FWB)
                                    //        {
                                    //            case 1:
                                    //                user1.scare -= 3;

                                    //                user1.scare -= jipai.qys;
                                    //                user1.pai_type = 11;
                                    //                user1.dy_fs = 0 - (jipai.qys + 3);
                                    //                break;

                                    //            case 2:


                                    //                user2.scare -= 3;

                                    //                user2.scare -= jipai.qys;
                                    //                user2.pai_type = 11;
                                    //                user2.dy_fs = 0 - (jipai.qys + 3);
                                    //                break;
                                    //            case 3:

                                    //                user3.scare -= 3;

                                    //                user3.scare -= jipai.qys;
                                    //                user3.pai_type = 11;
                                    //                user3.dy_fs = 0 - (jipai.qys + 3);


                                    //                break;
                                    //            case 4:


                                    //                user4.scare -= 3;

                                    //                user4.scare -= jipai.qys;
                                    //                user4.pai_type = 11;
                                    //                user4.dy_fs = 0 - (jipai.qys + 3);

                                    //                break;
                                    //            default:
                                    //                break;
                                    //        }

                                    //        break;
                                    //    default:
                                    //        break;
                                    //} 
                                    #endregion
                                    break;
                                case 5:
                                    userj += GetPaixinFen(jipai, item.paixinfs);
                                    userj += 1;
                                    userj += lianzhuangZIMOScore;
                                    userT.pai_type = item.paixinfs;
                                    userT.dy_fs = GetPaixinFen(jipai, item.paixinfs) + 1 + lianzhuangZIMOScore;
                                    #region 旧代码
                                    //switch (item.paixinfs)
                                    //{
                                    //    case 1:
                                    //        userj += jipai.xqidui;
                                    //        userj += jipai.zimo;
                                    //        userj += 1;
                                    //        userT.pai_type = 1;
                                    //        userT.dy_fs = jipai.xqidui + jipai.zimo + 1;
                                    //        break;
                                    //    case 2:
                                    //        userj += jipai.qys;
                                    //        userj += jipai.zimo;
                                    //        userj += 1;
                                    //        userT.pai_type = 2;
                                    //        userT.dy_fs = jipai.qys + jipai.zimo + 1;
                                    //        break;
                                    //    case 3:
                                    //        userj += jipai.dduizi;
                                    //        userj += jipai.zimo;
                                    //        userT.pai_type = item.paixinfs;
                                    //        userj += 1;
                                    //        userT.dy_fs = jipai.dduizi + jipai.zimo + 1;
                                    //        break;
                                    //    case 4:
                                    //        userj += jipai.longqidui;
                                    //        userj += jipai.zimo;
                                    //        userT.pai_type = item.paixinfs;
                                    //        userj += 1;
                                    //        userT.dy_fs = jipai.longqidui + jipai.zimo + 1;
                                    //        break;
                                    //    case 5:

                                    //        userj += jipai.zimo;
                                    //        userT.pai_type = item.paixinfs;
                                    //        userj += 1;
                                    //        userT.dy_fs = jipai.zimo + 1;
                                    //        break;
                                    //    case 6:
                                    //        userj += jipai.xqidui;
                                    //        userj += jipai.qys;
                                    //        userj += jipai.zimo;
                                    //        userT.pai_type = item.paixinfs;
                                    //        userj += 1;
                                    //        userT.dy_fs = jipai.zimo + jipai.xqidui + jipai.qys + 1;
                                    //        break;
                                    //    case 7:
                                    //        userj += jipai.qys * 2;
                                    //        userj += jipai.zimo;
                                    //        userT.pai_type = item.paixinfs;
                                    //        userj += 1;
                                    //        userT.dy_fs = jipai.qys * 2 + jipai.zimo + 1;
                                    //        break;
                                    //    case 8:
                                    //        userj += jipai.dduizi;
                                    //        userj += jipai.qys;
                                    //        userj += jipai.zimo;
                                    //        userT.pai_type = item.paixinfs;
                                    //        userj += 1;
                                    //        userT.dy_fs = jipai.dduizi + jipai.qys + jipai.zimo + 1;

                                    //        break;
                                    //    case 9:
                                    //        userj += jipai.longqidui;
                                    //        userj += jipai.qys;
                                    //        userj += jipai.zimo;
                                    //        userT.pai_type = item.paixinfs;

                                    //        userj += 1;
                                    //        userT.dy_fs = jipai.longqidui + jipai.qys + jipai.zimo + 1;
                                    //        break;
                                    //    case 10:
                                    //        userj += jipai.qys;
                                    //        userj += jipai.zimo;
                                    //        userT.pai_type = item.paixinfs;
                                    //        userj += 1;
                                    //        userT.dy_fs = jipai.qys + jipai.zimo + 1;
                                    //        break;
                                    //    default:
                                    //        break;
                                    //} 
                                    #endregion

                                    break;
                                default:
                                    break;
                            }
                        }

                    }

                }
                CountTianTingOrBaoting(userT.jp, ref userj, item, mjlist, huInfo);



                ///根据方位给不同的model赋值并添加list
                switch (item.ZS_Fw)
                {
                    case 1:

                        user1.is_jiao = userT.is_jiao;
                        user1.jp.AddRange(userT.jp);
                        user1.gang.AddRange(userT.gang);
                        user1.openid = userT.openid;

                        user1.scare += userT.scare;
                        //user1.dy_fs = userT.dy_fs;
                        if (r.is_lianz && lianZhuangScore[0] != 0)
                            user1.jp.Add(new UserJPOne { type = 10, PaiHS = 99, PaiScare = lianZhuangScore[0] });
                        user1j = userj;

                        //if (huInfo.FWZ == 1)
                        //{}
                        if (user1.pai_type != 11)
                        {
                            user1.dy_fs = userT.dy_fs;
                            user1.pai_type = userT.pai_type;
                        }

                        returnList.Add(user1);
                        break;
                    case 2:

                        user2.is_jiao = userT.is_jiao;
                        user2.jp.AddRange(userT.jp);
                        user2.gang.AddRange(userT.gang);
                        user2.openid = userT.openid;
                        if (r.is_lianz && lianZhuangScore[1] != 0)
                            user2.jp.Add(new UserJPOne { type = 10, PaiHS = 99, PaiScare = lianZhuangScore[1] });
                        user2.scare += userT.scare;
                        if (user2.pai_type != 11)
                        {
                            user2.dy_fs = userT.dy_fs;
                            user2.pai_type = userT.pai_type;
                        }

                        user2j = userj;
                        returnList.Add(user2);
                        break;
                    case 3:
                        user3.is_jiao = userT.is_jiao;
                        user3.jp.AddRange(userT.jp);
                        user3.gang.AddRange(userT.gang);
                        user3.openid = userT.openid;
                        // user3.pai_type = userT.pai_type;
                        user3.scare += userT.scare;

                        if (r.is_lianz && lianZhuangScore[2] != 0)
                            user3.jp.Add(new UserJPOne { type = 10, PaiHS = 99, PaiScare = lianZhuangScore[2] });
                        //if (huInfo.FWZ == 3)
                        //{
                        //    user3.pai_type = userT.pai_type;
                        //}
                        if (user3.pai_type != 11)
                        {
                            user3.dy_fs = userT.dy_fs;
                            user3.pai_type = userT.pai_type;
                        }
                        user3j = userj;
                        returnList.Add(user3);
                        break;
                    case 4:


                        user4.is_jiao = userT.is_jiao;
                        user4.jp.AddRange(userT.jp);
                        user4.gang.AddRange(userT.gang);
                        user4.openid = userT.openid;
                        //user4.pai_type = userT.pai_type;
                        user4.scare += userT.scare;
                        //user4.dy_fs = userT.dy_fs;
                        if (r.is_lianz && lianZhuangScore[3] != 0)
                            user4.jp.Add(new UserJPOne { type = 10, PaiHS = 99, PaiScare = lianZhuangScore[3] });
                        user4j = userj;
                      //  user4 = userT;
                        if (user4.pai_type != 11)
                        {
                            user4.dy_fs = userT.dy_fs;
                            user4.pai_type = userT.pai_type;
                        }

                        //if (huInfo.FWZ == 4)
                        //{
                        //    user4.pai_type = userT.pai_type;


                        //}
                        returnList.Add(user4);
                        break;
                    default:
                        break;
                }
            }
            if (huInfo.HasFWB && huInfo.Type != 1 && huInfo.Type != 5)
            {
                if (!mjlist.Any(w => w.paixinfs == 11))
                { }
            }
            var roomUser1 = mjlist.Find(u => u.Openid == user1.openid);
            var roomUser2 = mjlist.Find(u => u.Openid == user2.openid);
            var roomUser3 = mjlist.Find(u => u.Openid == user3.openid);
            var roomUser4 = mjlist.Find(u => u.Openid == user4.openid);
            var jiesuanUser1 = returnList.Find(u => u.openid == user1.openid);
            var jiesuanUser2 = returnList.Find(u => u.openid == user2.openid);
            var jiesuanUser3 = returnList.Find(u => u.openid == user3.openid);
            var jiesuanUser4 = returnList.Find(u => u.openid == user4.openid);

            ///剩余分值计算
            switch (returnList.Count)
            {
                case 2:
                    jiesuanUser1.scare += (user1j - user2j) + danduScore[0] + zrwgjlist[0] + zryaojilist[0] + lianZhuangScore[0];
                    jiesuanUser2.scare += (user2j - user1j) + danduScore[1] + zrwgjlist[1] + zryaojilist[1] + lianZhuangScore[1];

                    break;
                case 3:

                    jiesuanUser1.scare += ((!roomUser1.Is_jiao && !roomUser2.Is_jiao) ? 0 : (user1j - user2j)) + ((!roomUser1.Is_jiao && !roomUser3.Is_jiao) ? 0 : (user1j - user3j)) + danduScore[0] + zrwgjlist[0] + zryaojilist[0] + lianZhuangScore[0];
                    jiesuanUser2.scare += ((!roomUser2.Is_jiao && !roomUser1.Is_jiao) ? 0 : (user2j - user1j)) + ((!roomUser2.Is_jiao && !roomUser3.Is_jiao) ? 0 : (user2j - user3j)) + danduScore[1] + zrwgjlist[1] + zryaojilist[1] + lianZhuangScore[1];
                    jiesuanUser3.scare += ((!roomUser3.Is_jiao && !roomUser1.Is_jiao) ? 0 : (user3j - user1j)) + ((!roomUser3.Is_jiao && !roomUser2.Is_jiao) ? 0 : (user3j - user2j)) + danduScore[2] + zrwgjlist[2] + zryaojilist[2] + lianZhuangScore[2];
                    break;
                case 4:
                    ///方位1

                    if (roomUser1.Is_jiao == false)
                    {
                        if (roomUser2.Is_jiao == true)
                        {
                            jiesuanUser1.scare += (user1j - user2j);
                        }

                        if (roomUser3.Is_jiao == true)
                        {
                            jiesuanUser1.scare += (user1j - user3j);
                        }
                        if (roomUser4.Is_jiao == true)
                        {
                            jiesuanUser1.scare += (user1j - user4j);
                        }
                    }
                    else
                    {
                        jiesuanUser1.scare += ((user1j - user2j) + (user1j - user3j) + (user1j - user4j));
                    }




                    ///方位2
                    if (roomUser2.Is_jiao == false)
                    {
                        if (roomUser1.Is_jiao == true)
                        {
                            jiesuanUser2.scare += (user2j - user1j);
                        }

                        if (roomUser3.Is_jiao == true)
                        {
                            jiesuanUser2.scare += (user2j - user3j);
                        }
                        if (roomUser4.Is_jiao == true)
                        {
                            jiesuanUser2.scare += (user2j - user4j);
                        }
                    }
                    else
                    {
                        jiesuanUser2.scare += ((user2j - user1j) + (user2j - user3j) + (user2j - user4j));
                    }



                    ///方位3
                    if (roomUser3.Is_jiao == false)
                    {
                        if (roomUser1.Is_jiao == true)
                        {
                            jiesuanUser3.scare += (user3j - user1j);
                        }

                        if (roomUser2.Is_jiao == true)
                        {
                            jiesuanUser3.scare += (user3j - user2j);
                        }
                        if (roomUser4.Is_jiao == true)
                        {
                            jiesuanUser3.scare += (user3j - user4j);
                        }
                    }
                    else
                    {
                        jiesuanUser3.scare += ((user3j - user2j) + (user3j - user1j) + (user3j - user4j));
                    }



                    ///方位4
                    if (roomUser4.Is_jiao == false)
                    {
                        if (roomUser1.Is_jiao == true)
                        {
                            jiesuanUser4.scare += (user4j - user1j);
                        }

                        if (roomUser2.Is_jiao == true)
                        {
                            jiesuanUser4.scare += (user4j - user2j);
                        }
                        if (roomUser3.Is_jiao == true)
                        {
                            jiesuanUser4.scare += (user4j - user3j);
                        }
                    }
                    else
                    {
                        jiesuanUser4.scare += ((user4j - user2j) + (user4j - user3j) + (user4j - user1j));
                    }

                    jiesuanUser1.scare += danduScore[0] + zrwgjlist[0] + zryaojilist[0] + lianZhuangScore[0];
                    jiesuanUser2.scare += danduScore[1] + zrwgjlist[1] + zryaojilist[1] + lianZhuangScore[1];
                    jiesuanUser3.scare += danduScore[2] + zrwgjlist[2] + zryaojilist[2] + lianZhuangScore[2];
                    jiesuanUser4.scare += danduScore[3] + zrwgjlist[3] + zryaojilist[3] + lianZhuangScore[3];
                    break;
                default:
                    break;
            }

            return returnList;
        }
        /// <summary>
        /// 计算天听天胡报听杀报
        /// </summary>
        /// <param name="jp"></param>
        /// <param name="userj"></param>
        /// <param name="item"></param>
        /// <param name="mjlist"></param>
        /// <param name="huInfo"></param>
        private void CountTianTingOrBaoting(List<UserJPOne> jp, ref int userj, mjuser item, List<mjuser> mjlist, SendHu huInfo)
        {
            if (huInfo.FWZ == item.ZS_Fw || huInfo.DXInfoList.Any(w => w.DXFW == item.ZS_Fw))
            {

                switch (huInfo.Type)
                {
                    case 1:
                    case 5:
                        if (item.Is_tianHu)
                        {
                            UserJPOne BJA = new UserJPOne();
                            BJA.PaiHS = 102;
                            BJA.PaiScare = 20;
                            BJA.type = 10;
                            jp.Add(BJA);
                            userj += BJA.PaiScare;
                        }
                        else if (item.Is_tiant)
                        {
                            UserJPOne BJA = new UserJPOne();
                            BJA.PaiHS = 100;
                            BJA.PaiScare = 20;
                            BJA.type = 10;
                            jp.Add(BJA);
                            userj += BJA.PaiScare;
                        }
                        else if (item.Is_baotin)
                        {
                            UserJPOne BJA = new UserJPOne();
                            BJA.PaiHS = 101;
                            BJA.PaiScare = 10;
                            BJA.type = 10;
                            jp.Add(BJA);
                            userj += BJA.PaiScare;
                        }
                        break;
                    case 2:
                    case 3:
                    case 4:
                        if (item.Is_tianHu)
                        {
                            UserJPOne BJAH = new UserJPOne();
                            BJAH.PaiHS = 102;
                            BJAH.PaiScare = 20;
                            BJAH.type = 10;
                            jp.Add(BJAH);
                            danduScore[item.ZS_Fw - 1] += BJAH.PaiScare;
                            UserJPOne BJA = new UserJPOne();
                            BJA.PaiHS = 102;
                            BJA.PaiScare = -20;
                            BJA.type = 10;
                            userList[huInfo.FWB - 1].jp.Add(BJA);
                            danduScore[huInfo.FWB - 1] += BJA.PaiScare;
                        }
                        else if (item.Is_tiant)
                        {
                            UserJPOne BJAH = new UserJPOne();
                            BJAH.PaiHS = 100;
                            BJAH.PaiScare = 20;
                            BJAH.type = 10;
                            jp.Add(BJAH);
                            danduScore[item.ZS_Fw - 1] += BJAH.PaiScare;
                            UserJPOne BJA = new UserJPOne();
                            BJA.PaiHS = 100;
                            BJA.PaiScare = -20;
                            BJA.type = 10;
                            userList[huInfo.FWB - 1].jp.Add(BJA);
                            danduScore[huInfo.FWB - 1] += BJA.PaiScare;
                        }
                        else if (item.Is_baotin)
                        {
                            UserJPOne BJAH = new UserJPOne();
                            BJAH.PaiHS = 101;
                            BJAH.PaiScare = 10;
                            BJAH.type = 10;
                            jp.Add(BJAH);
                            danduScore[item.ZS_Fw - 1] += BJAH.PaiScare;
                            UserJPOne BJA = new UserJPOne();
                            BJA.PaiHS = 101;
                            BJA.PaiScare = -10;
                            BJA.type = 10;
                            userList[huInfo.FWB - 1].jp.Add(BJA);
                            danduScore[huInfo.FWB - 1] += BJA.PaiScare;
                        }
                        break;

                }

            }
            else //if (item.ZS_Fw == huInfo.FWB&&)
            {
                switch (huInfo.Type)
                {
                    case 1:
                    case 5:
                        if (item.Is_tiant)
                        {
                            UserJPOne BJA = new UserJPOne();
                            BJA.PaiHS = 100;
                            BJA.PaiScare = -20;
                            BJA.type = 10;
                            jp.Add(BJA);
                            danduScore[item.ZS_Fw - 1] += BJA.PaiScare;
                            danduScore[huInfo.FWZ - 1] += 0 - BJA.PaiScare;
                        }
                        else if (item.Is_baotin)
                        {
                            UserJPOne BJA = new UserJPOne();
                            BJA.PaiHS = 101;
                            BJA.PaiScare = -10;
                            BJA.type = 10;
                            jp.Add(BJA);
                            danduScore[item.ZS_Fw - 1] += BJA.PaiScare;
                            danduScore[huInfo.FWZ - 1] += 0 - BJA.PaiScare;
                        }
                        break;
                    case 2:
                    case 3:
                    case 4:
                        if (item.ZS_Fw == huInfo.FWB)
                        {
                            if (item.Is_tiant)
                            {
                                UserJPOne BJA = new UserJPOne();
                                BJA.PaiHS = 100;
                                BJA.PaiScare = -20;
                                BJA.type = 10;
                                userList[huInfo.FWB - 1].jp.Add(BJA);
                                danduScore[huInfo.FWB - 1] += BJA.PaiScare;
                                danduScore[huInfo.FWZ - 1] += 0 - BJA.PaiScare;
                            }
                            else if (item.Is_baotin)
                            {
                                UserJPOne BJA = new UserJPOne();
                                BJA.PaiHS = 101;
                                BJA.PaiScare = -10;
                                BJA.type = 10;
                                userList[huInfo.FWB - 1].jp.Add(BJA);
                                danduScore[huInfo.FWB - 1] += BJA.PaiScare;
                                danduScore[huInfo.FWZ - 1] += 0 - BJA.PaiScare;
                            }
                        }
                        break;

                }
            }
        }


        /// <summary>
        /// 获取牌型分
        /// </summary>
        /// <param name="jipai"></param>
        /// <param name="paixinfs"></param>
        /// <returns></returns>
        public int GetPaixinFen(JIpai jipai, int paixinfs)
        {
            switch (paixinfs)
            {
                case 1:
                    return jipai.xqidui;
                case 2:
                    return jipai.qys;
                case 3:
                    return jipai.dduizi;
                case 4:
                    return jipai.longqidui; 
                case 5:
                    return jipai.zimo;
                case 6:
                    return jipai.xqidui + jipai.qys;
                case 7:
                    return jipai.qys * 2;
                  
                case 8:
                    return jipai.dduizi + jipai.qys;
                case 9:
                    return jipai.longqidui;
                case 10:
                    return jipai.qys;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// 叫牌牌型赋值
        /// </summary>
        /// <param name="is_paixin"></param>
        /// <param name="thisuser"></param>
        /// <param name="is_qys"></param>
        public void GetPaiXin(string is_paixin, mjuser thisuser)
        {
            thisuser.majiangs.Sort((a, b) => -b.PaiHs.CompareTo(a.PaiHs));
            bool is_qys = false;
            ///判断牌型
            if (thisuser.majiangs[thisuser.majiangs.Count - 1].PaiHs - thisuser.majiangs[0].PaiHs < 9 && thisuser.majiangs[0].PaiHs / 10 == thisuser.majiangs[thisuser.majiangs.Count - 1].PaiHs / 10)
            {
                if (thisuser.majiangs[0].PaiHs > 10 && thisuser.majiangs[0].PaiHs < 20)
                {
                    int count = thisuser.Peng.FindAll(u => u.PaiHs < 10 || u.PaiHs > 20).Count;
                    if (!string.IsNullOrEmpty(thisuser.Gong))
                    {


                        string gang = thisuser.Gong.Remove(thisuser.Gong.Length - 1, 1);
                        string[] arr = gang.Split(',');
                        int panduan = 0;
                        foreach (var item in arr)
                        {
                            string[] newarr = item.Split('|');
                            if (int.Parse(newarr[0]) < 10 || int.Parse(newarr[0]) > 20)
                            {
                                panduan++;
                                is_qys = false;
                                break;
                            }
                        }
                        if (panduan == 0 && count == 0)
                        {
                            is_qys = true;
                        }
                    }
                    else
                    {
                        if (count == 0)
                        {
                            is_qys = true;
                        }
                    }
                }
                else if (thisuser.majiangs[0].PaiHs < 10)
                {
                    int count = thisuser.Peng.FindAll(u => u.PaiHs > 10).Count;
                    if (!string.IsNullOrEmpty(thisuser.Gong))
                    {
                        string gang = thisuser.Gong.Remove(thisuser.Gong.Length - 1, 1);
                        string[] arr = gang.Split(',');
                        int panduan = 0;
                        foreach (var item in arr)
                        {
                            string[] newarr = item.Split('|');
                            if (int.Parse(newarr[0]) > 10)
                            {
                                panduan++;
                                is_qys = false;
                                break;
                            }
                        }
                        if (panduan == 0 && count == 0)
                        {
                            is_qys = true;
                        }
                    }
                    else
                    {
                        if (count == 0)
                        {
                            is_qys = true;
                        }
                    }
                }
                else
                {
                    int count = thisuser.Peng.FindAll(u => u.PaiHs < 20).Count;
                    if (!string.IsNullOrEmpty(thisuser.Gong))
                    {
                        string gang = thisuser.Gong.Remove(thisuser.Gong.Length - 1, 1);
                        string[] arr = gang.Split(',');
                        int panduan = 0;
                        foreach (var item in arr)
                        {
                            string[] newarr = item.Split('|');
                            if (int.Parse(newarr[0]) < 20)
                            {
                                panduan++;
                                is_qys = false;
                                break;
                            }
                        }
                        if (panduan == 0 && count == 0)
                        {
                            is_qys = true;
                        }
                    }
                    else
                    {
                        if (count == 0)
                        {
                            is_qys = true;
                        }
                    }
                }
            }
            if (is_paixin != "MJ")
            {
                thisuser.Is_jiao = true;
                switch (is_paixin)
                {
                    case "QD":
                        if (is_qys)
                        {
                            thisuser.paixinfs = 6;
                        }
                        else
                        {
                            thisuser.paixinfs = 1;
                        }

                        break;
                    case "DDDD":
                        if (is_qys)
                        {
                            thisuser.paixinfs = 7;
                        }
                        else
                        {
                            thisuser.paixinfs = 2;
                        }

                        break;
                    case "DD":
                        if (is_qys)
                        {
                            thisuser.paixinfs = 8;
                        }
                        else
                        {
                            thisuser.paixinfs = 3;
                        }

                        break;
                    case "LQD":
                        if (is_qys)
                        {
                            thisuser.paixinfs = 9;
                        }
                        else
                        {
                            thisuser.paixinfs = 4;
                        }

                        break;
                    case "SP":
                        if (is_qys)
                        {
                            thisuser.paixinfs = 10;
                        }
                        else
                        {
                            thisuser.paixinfs = 5;
                        }

                        break;
                    default:
                        break;
                }



            }
            else
            {
                thisuser.Is_tiant = false;
                thisuser.Is_jiao = false;
                thisuser.paixinfs = 0;
            }
        }

        /// <summary>
        /// 获取胡牌的牌型
        /// </summary>
        /// <param name="Pai"></param>
        /// <param name="pengNumber"></param>
        /// <param name="gangNumber"></param>
        /// <param name="paiHS"></param>
        /// <returns></returns>
        public string GetHPaiPaiXin(List< MJModel.BllModel.MaJiang> Pai, int pengNumber, int gangNumber, int paiHS)
        {
            Pai.Sort((a, b) => -b.PaiHs.CompareTo(a.PaiHs));
            List< MJModel.BllModel.MaJiang> ddPai = new List<MJModel.BllModel.MaJiang>();
            ddPai.AddRange(Pai.ToArray());
            List< MJModel.BllModel.MaJiang> qdPai = new List<MJModel.BllModel.MaJiang>();
            qdPai.AddRange(Pai.ToArray());
            bool isLQD = false;

         
            if (Pai.Where(w => w.PaiHs == paiHS).Count() == 4)
            {
                isLQD = true;
            }
           MJModel.BllModel.MaJiang mjone = new MJModel.BllModel.MaJiang();
           MJModel.BllModel.MaJiang mjtwo = new MJModel.BllModel.MaJiang();
            string stat = "SP";//没叫
            int shun = 0;
            int dui = 0;
            int ke = 0;
            int si = 0;
            List<int> shan = new List<int>();//散牌
            #region  旧代码
            //while (Pai.Count > 0)
            //{
            //    if (Pai.Count >= 3)
            //    {
            //        mjone = Pai.FindLast(u => u.PaiHs == Pai[0].PaiHs + 1);
            //        mjtwo = Pai.FindLast(u => u.PaiHs == Pai[0].PaiHs + 2 && Pai[0].PaiHs + 2 != 11 && Pai[0].PaiHs + 2 != 21);


            //        if (mjone != null && mjtwo != null)
            //        {
            //            if (Pai[0].PaiHs == Pai[0 + 1].PaiHs)
            //            {
            //                dui++;
            //                Pai.RemoveRange(0, 2);
            //            }
            //            else
            //            {
            //                shun++;
            //                Pai.Remove(Pai[0]);
            //                Pai.Remove(mjone);
            //                Pai.Remove(mjtwo);
            //            }

            //        }
            //        else if ((Pai.Count >= 4 && Pai[0].PaiHs == Pai[2].PaiHs))
            //        {
            //            si++;
            //            Pai.RemoveRange(0, 4);
            //        }
            //        else if (Pai[0].PaiHs == Pai[2].PaiHs)
            //        {
            //            ke++;
            //            Pai.RemoveRange(0, 3);
            //        }


            //        else if (Pai[0].PaiHs == Pai[1].PaiHs)
            //        {
            //            dui++;
            //            Pai.RemoveRange(0, 2);
            //        }
            //        else
            //        {
            //            shan.Add(Pai[0].PaiHs);
            //            Pai.Remove(Pai[0]);
            //        }
            //    }
            //    else if (Pai.Count >= 2)
            //    {
            //        if (Pai[0].PaiHs == Pai[1].PaiHs)
            //        {
            //            dui++;
            //            Pai.RemoveRange(0, 2);
            //        }
            //        else
            //        {
            //            shan.Add(Pai[0].PaiHs);
            //            Pai.Remove(Pai[0]);
            //        }
            //    }
            //    else
            //    {
            //        shan.Add(Pai[0].PaiHs);
            //        Pai.Remove(Pai[0]);
            //    }
            //} 
            #endregion
            stat = GetHuQD(qdPai, isLQD);
            if (string.IsNullOrEmpty(stat))
                stat = GetHuDD(ddPai, pengNumber);


            if(string.IsNullOrEmpty(stat))
            stat = "SP";//散牌

            return stat;

        }

        /// <summary>
        /// 判断是否叫牌牌型为大对子
        /// </summary>
        /// <param name="Pai"></param>
        /// <returns></returns>
        public string GetHuDD(List< MJModel.BllModel.MaJiang> Pai, int pengNumber)
        {
            int ke = 0;
            int dui = 0;
            int shan = 0;//单数量

            if (Pai.Count == 0)
            {
                return "手牌为空";
            }


            while (Pai.Count > 0)
            {

                if (Pai.Count >= 3)
                {
                    if (Pai[0].PaiHs == Pai[0 + 2].PaiHs)
                    {
                        ke++;
                        Pai.RemoveRange(0, 3);
                    }
                    else if (Pai[0].PaiHs == Pai[0 + 1].PaiHs)
                    {
                        dui++;
                        Pai.RemoveRange(0, 2);
                    }
                    else
                    {
                        shan++;
                        Pai.Remove(Pai[0]);
                    }
                }
                else if (Pai.Count >= 2)
                {
                    if (Pai[0].PaiHs == Pai[0 + 1].PaiHs)
                    {
                        dui++;
                        Pai.RemoveRange(0, 2);
                    }
                    else
                    {
                        shan++;
                        Pai.Remove(Pai[0]);
                    }
                }
                else
                {
                    shan++;
                    Pai.Remove(Pai[0]);
                }

            }
            if (dui == 1 && shan == 0 && ke == 0 )
            {
                return "DDDD";//单调大队
            }
            if (dui == 1 && shan == 0)
            {
                return "DD";//大队
            }

            return "";



        }

        /// <summary>
        /// 获取七对
        /// </summary>
        /// <param name="Pai"></param>
        /// <returns></returns>
        public string GetHuQD(List< MJModel.BllModel.MaJiang> Pai, bool isLQD)
        {

            List< MJModel.BllModel.MaJiang> list = new List< MJModel.BllModel.MaJiang>();
            list.AddRange(Pai.ToArray());
            int dui = 0;
            int ke = 0;
            while (list.Count >= 2)
            {
                if (list.Count >= 4)
                {
                    if (list[0].PaiHs == list[0 + 3].PaiHs)
                    {
                        dui += 2;
                        list.RemoveRange(0, 4);
                    }
                    else if (list[0].PaiHs == list[0 + 2].PaiHs)
                    {
                        ke++;
                        list.RemoveRange(0, 3);
                    }
                    else if (list[0].PaiHs == list[0 + 1].PaiHs)
                    {
                        dui++;
                        list.RemoveRange(0, 2);
                    }
                    else
                    {
                        list.Remove(list[0]);
                    }
                }
                else if (list.Count >= 3)
                {
                    if (list[0].PaiHs == list[0 + 2].PaiHs)
                    {
                        ke++;
                        list.RemoveRange(0, 3);
                    }
                    else if (list[0].PaiHs == list[0 + 1].PaiHs)
                    {
                        dui++;
                        list.RemoveRange(0, 2);
                    }
                    else
                    {
                        list.Remove(list[0]);
                    }
                }
                else
                {
                    if (list[0].PaiHs == list[0 + 1].PaiHs)
                    {
                        dui++;
                        list.RemoveRange(0, 2);
                    }
                    else
                    {
                        list.Remove(list[0]);
                    }
                }


                //}
                //else
                //{
                //    break;
                //}


            }
            if (dui == 7)
            {
                if (isLQD)
                    return "LQD";
                return "QD";//胡
            }
            return "";
        }
    }
}
