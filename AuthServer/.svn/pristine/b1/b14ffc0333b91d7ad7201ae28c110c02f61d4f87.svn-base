using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJBLL.common
{
    public class CreateMj
    {

        /// <summary>
        /// 麻将洗牌方法
        /// </summary>
        /// <returns></returns>
        public List<model.ServerMaJiang> CreateMJ(bool isYuanQue)
        {
            List<model.ServerMaJiang> UserMj = new List<model.ServerMaJiang>();
            List<int> tempMj = new List<int>();

            for (int i = 1; i <= 120; i++)
            {
                if (i % 10 != 0)
                {
                    tempMj.Add(i);
                }
            }
            List<int> AllMj =  RandomSortList(tempMj);

            UserMj.Clear();
            int paiCount = 108;
            //if (isYuanQue)
            //    paiCount = 72;
            for (int i = 0; i < paiCount; i++)
            {

               model.ServerMaJiang mj = new model.ServerMaJiang();
              
                if (AllMj[i] > 0 && AllMj[i] < 40)
                {
                    mj.PaiHs = AllMj[i] % 10;
                    mj.PaiId = AllMj[i];
                }
                else if (AllMj[i] > 40 && AllMj[i] < 80)
                {
                    mj.PaiHs = 10 + AllMj[i] % 10;
                    mj.PaiId = AllMj[i];
                }
                else if (!isYuanQue)
                {
                    mj.PaiHs = 20 + AllMj[i] % 10;
                    mj.PaiId = AllMj[i];
                }
                if (mj.PaiHs != 0)
                    UserMj.Add(mj);


            }
           
            return UserMj;
        }

        public List<T> RandomSortList<T>(List<T> ListT)
        {
            System.Random random = new System.Random();
            List<T> newList = new List<T>();
            foreach (T item in ListT)
            {
                newList.Insert(random.Next(newList.Count), item);
            }
            return newList;
        }
    }
}
