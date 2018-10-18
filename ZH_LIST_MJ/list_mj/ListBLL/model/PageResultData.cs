using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListBLL.model
{
    public class PageResultData : ResultData
    {
        /// 当前页
        /// <summary>
        /// 当前页
        /// </summary>
        private int _pageindex;
        public int PageIndex
        {
            set { _pageindex = value; }
            get { return _pageindex; }
        }

        /// 分页大小
        /// <summary>
        /// 分页大小
        /// </summary>
        private int _pagesize;
        public int PageSize
        {
            set { _pagesize = value; }
            get { return _pagesize; }
        }


        /// 总页数
        /// <summary>
        /// 总页数
        /// </summary>
        private int _pagecount;
        public int PageCount
        {
            set { _pagecount = value; }
            get { return _pagecount; }
        }
    }
}
