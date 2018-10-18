using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script_me
{
    public class VoiceHelp
    {


        /// <summary>
        /// 声音类赋值
        /// </summary>
        /// <returns></returns>
        public List<VoicePlay> Returnlist()
        {

            List<VoicePlay> list = new List<VoicePlay>();
            for (int i = 0; i < 30; i++)
            {
                if (i % 10 != 0)
                {
                    VoicePlay voice = new VoicePlay()
                    {
                        Paihs = i,
                        Fvoice = i + "VF",
                        Pvoice = i + "VP"
                    };

                    list.Add(voice);

                }
            }

            return list;

        }


        /// <summary>
        /// 返回声音
        /// </summary>
        /// <param name="sex">性别</param>
        /// <param name="paiHS">牌HS</param>
        /// <param name="type">方言还是普通话</param>
        /// <returns></returns>
        public string VoiceSoure(int sex, int paiHS, int type)
        {

            string VoiceSoure = "";
            switch (type)
            {
                case 1:
                    VoiceSoure = Returnlist().Find(u => u.Paihs == paiHS).Pvoice;

                    break;
                case 2:
                    VoiceSoure = Returnlist().Find(u => u.Paihs == paiHS).Fvoice;
                    break;
                default:
                    break;
            }

            switch (sex)
            {
                case 1:
                    VoiceSoure += "XY";
                    break;
                case 2:
                    VoiceSoure += "XX";
                    break;
                default:
                    break;
            }

            return VoiceSoure;
        }

    }

    /// <summary>
    /// 声音类
    /// </summary>
    public class VoicePlay
    {

        /// <summary>
        /// 牌HS
        /// </summary>
        public int Paihs { get; set; }


        /// <summary>
        /// 普通话
        /// </summary>
        public string Pvoice { get; set; }

        /// <summary>
        /// 方言
        /// </summary>
        public string Fvoice { get; set; }

    }
}
