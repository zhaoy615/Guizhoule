using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
   public  class CreateRoomRecord
    {
        public long GroupID { get; set; }


        public int RoomID { get; set; }

        public long CreateUserID { get; set; }

        public DateTime CreateDate { get; set; }

        public int UseRoomCard { get; set; }
    }
}
