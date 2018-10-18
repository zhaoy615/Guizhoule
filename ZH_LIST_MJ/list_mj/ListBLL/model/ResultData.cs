using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListBLL.model
{
    public class ResultData
    {
        /// <summary>
        ///结果标识
        /// </summary>
        private bool _success = false;
        public bool Success
        {
            get { return _success; }
            set { _success = value; }
        }

        /// <summary>
        /// 提示文字
        /// </summary>
        private string _message;
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        /// <summary>
        /// 返回数据 任何类型
        /// </summary>
        private object _data;
        public object Data
        {

            get { return _data; }
            set { _data = value; }
        }
    }
}
