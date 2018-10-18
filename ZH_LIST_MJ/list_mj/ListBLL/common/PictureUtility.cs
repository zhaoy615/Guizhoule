using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ListBLL.common
{
   public class PictureUtility
    {
        /// <summary>
        /// 根据图片URL保存至本地
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <returns></returns>
        public static string SavePicture(string imageUrl,out string fileName)
        {
            #region 微信图片下载调用
            string imgurl = string.Empty;
            fileName = string.Empty;
            // 保存微信图片
            string m_fileName = DateTime.Now.ToFileTime().ToString() + ".jpg";
            string m_saveName = "//CodeImage/" + m_fileName;
            string m_savePath = "";
            fileName = m_fileName;
            if (HttpContext.Current != null)
            {
                m_savePath = HttpContext.Current.Server.MapPath(m_saveName);
            }
            else //非web程序引用
            {
                m_saveName = m_saveName.Replace("/", "\\");
                if (m_saveName.StartsWith("\\") || m_saveName.StartsWith("~"))
                {
                    m_saveName = m_saveName.Substring(m_saveName.IndexOf('\\', 1)).TrimStart('\\');
                }
                m_savePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, m_saveName);
            }
            if (DownloadPicture(imageUrl, m_savePath, -1))
            {
               return m_savePath;
             
            }
            return "";//当图片保存到本地失败 不做任何操作返回空值
            #endregion
        }
        #region 微信图片下载 2017-5-3

        /// <summary>
        /// 根据图片Url保存至本地
        /// </summary>
        /// <param name="picUrl">图片URL</param>
        /// <param name="savePath">保存本地地址</param>
        /// <param name="timeOut">超时时间</param>
        /// <returns></returns>
        private static bool DownloadPicture(string picUrl, string savePath, int timeOut)
        {
            bool value = false;
            WebResponse response = null;
            Stream stream = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(picUrl);
                if (timeOut != -1) request.Timeout = timeOut;
                response = request.GetResponse();
                stream = response.GetResponseStream();
                if (!response.ContentType.ToLower().StartsWith("text/"))
                    value = SaveBinaryFile(response, savePath);
            }
            finally
            {
                if (stream != null) stream.Close();
                if (response != null) response.Close();
            }
            return value;
        }

        /// <summary>
        /// 将请求保存至路径
        /// </summary>
        /// <param name="response"></param>
        /// <param name="savePath"></param>
        /// <returns></returns>
        private static bool SaveBinaryFile(WebResponse response, string savePath)
        {
            bool value = false;
            byte[] buffer = new byte[1024];
            Stream outStream = null;
            Stream inStream = null;
            try
            {
                if (File.Exists(savePath)) File.Delete(savePath);
                outStream = System.IO.File.Create(savePath);
                inStream = response.GetResponseStream();
                int l;
                do
                {
                    l = inStream.Read(buffer, 0, buffer.Length);
                    if (l > 0) outStream.Write(buffer, 0, l);
                } while (l > 0);
                value = true;
            }
            finally
            {
                if (outStream != null) outStream.Close();
                if (inStream != null) inStream.Close();
            }
            return value;
        }
        #endregion
    }
}
