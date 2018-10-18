using System;
using System.IO;
using System.Threading;
using Aliyun.OSS.Common;
using System.Text;
using Aliyun.OSS.Util;
using Aliyun.OSS;
using log4net.Repository.Hierarchy;
using System.Net;
using System.Configuration;

namespace ListBLL.common
{
    /// <summary>
    /// Sample for putting object.
    /// </summary>
    public static class PutObjectSample
    {
        static string accessKeyId = ConfigurationManager.AppSettings["AccessKeyId"];
        static string accessKeySecret = ConfigurationManager.AppSettings["AccessKeySecret"];// "4N1B12IRUdb54u8zDLyee8RkUintOA";
        static string endpoint = ConfigurationManager.AppSettings["endpoint"];//"oss-cn-shenzhen.aliyuncs.com";
        static OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);
        static string bucketName = ConfigurationManager.AppSettings["bucketName"];// "zh-imageserver";
                                                                                //  static string fileToUpload = Config.FileToUpload;

        static AutoResetEvent _event = new AutoResetEvent(false);

        /// <summary>
        /// sample for put object to oss
        /// </summary>
        //public static void PutObject(string bucketName)
        //{
        //  //  PutObjectFromFile(bucketName);

        //    PutObjectFromString(bucketName);

        //    PutObjectWithDir(bucketName);

        //    PutObjectWithMd5(bucketName);

        //    PutObjectWithHeader(bucketName);

        //    AsyncPutObject(bucketName);
        //}
        ///上传至图片服务器，并返回图片外网地址
        public static string PutObjectFromFile(string picUrl, string fileName, int timeOut )
        {
          //  bool value = false;
            WebResponse response = null;
            Stream stream = null;
            var memoryStream = new MemoryStream();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(picUrl);
                if (timeOut != -1) request.Timeout = timeOut;
                response = request.GetResponse();
                stream = response.GetResponseStream();


                //将基础流写入内存流
                const int bufferLength = 1024;
                byte[] buffer = new byte[bufferLength];
                while (true)
                {
                    int actual = stream.Read(buffer, 0, bufferLength);
                    if (actual > 0)
                    {
                        memoryStream.Write(buffer, 0, actual);
                    }
                    else
                        break;
                }

                //byte[] buffer = new byte[stream.Length];
                //stream.Read(buffer, 0, buffer.Length);             
                using (Stream fileStream = new MemoryStream(memoryStream.ToArray()))//转成Stream流  
                {

                    client.PutObject(bucketName, fileName, fileStream);
                }

                // Console.WriteLine("Put object:{0} succeeded", key);
                return "http://qy-imageserver.oss-cn-shenzhen.aliyuncs.com/" + fileName;
            }
            catch (OssException ex)
            {
                //  Console.WriteLine("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}",
                //      ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
                MyLogger.Logger.Error(string.Format("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}",
                    ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId));
                return "";
            }
            catch (Exception ex)
            {
                MyLogger.Logger.Error(string.Format("Failed with error info: {0}", ex.Message));
                return "http://qy-imageserver.oss-cn-shenzhen.aliyuncs.com/20170807151456017.jpg";
            }
            finally
            {
                memoryStream.Close();
                stream.Close();
            }
        }

        public static void PutObjectFromString(string bucketName)
        {
            const string key = "PutObjectFromString";
            const string str = "Aliyun OSS SDK for C#";

            try
            {
                byte[] binaryData = Encoding.ASCII.GetBytes(str);
                var stream = new MemoryStream(binaryData);

                client.PutObject(bucketName, key, stream);
                Console.WriteLine("Put object:{0} succeeded", key);
            }
            catch (OssException ex)
            {
                Console.WriteLine("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}",
                    ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed with error info: {0}", ex.Message);
            }
        }

        //public static void PutObjectWithDir(string bucketName)
        //{
        //    const string key = "folder/sub_folder/PutObjectFromFile";

        //    try
        //    {
        //        client.PutObject(bucketName, key, fileToUpload);
        //        Console.WriteLine("Put object:{0} succeeded", key);
        //    }
        //    catch (OssException ex)
        //    {
        //        Console.WriteLine("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}",
        //            ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Failed with error info: {0}", ex.Message);
        //    }
        //}

        //public static void PutObjectWithMd5(string bucketName)
        //{
        //    const string key = "PutObjectWithMd5";

        //    string md5;
        //    using (var fs = File.Open(fileToUpload, FileMode.Open))
        //    {
        //        md5 = OssUtils.ComputeContentMd5(fs, fs.Length);
        //    }

        //    var meta = new ObjectMetadata() { ContentMd5 = md5 };
        //    try
        //    {
        //        client.PutObject(bucketName, key, fileToUpload, meta);

        //        Console.WriteLine("Put object:{0} succeeded", key);
        //    }
        //    catch (OssException ex)
        //    {
        //        Console.WriteLine("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}",
        //            ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Failed with error info: {0}", ex.Message);
        //    }
        //}

        //public static void PutObjectWithHeader(string bucketName)
        //{
        //    const string key = "PutObjectWithHeader";
        //    try
        //    {
        //        using (var content = File.Open(fileToUpload, FileMode.Open))
        //        {
        //            var metadata = new ObjectMetadata();
        //            metadata.ContentLength = content.Length;

        //            metadata.UserMetadata.Add("github-account", "qiyuewuyi");

        //            client.PutObject(bucketName, key, content, metadata);

        //            Console.WriteLine("Put object:{0} succeeded", key);
        //        }
        //    }
        //    catch (OssException ex)
        //    {
        //        Console.WriteLine("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}",
        //            ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Failed with error info: {0}", ex.Message);
        //    }
        //}

        //public static void AsyncPutObject(string bucketName)
        //{
        //    const string key = "AsyncPutObject";
        //    try
        //    {
        //        // 1. put object to specified output stream
        //        using (var fs = File.Open(fileToUpload, FileMode.Open))
        //        {
        //            var metadata = new ObjectMetadata();
        //            metadata.UserMetadata.Add("mykey1", "myval1");
        //            metadata.UserMetadata.Add("mykey2", "myval2");
        //            metadata.CacheControl = "No-Cache";
        //            metadata.ContentType = "text/html";

        //            string result = "Notice user: put object finish";
        //            client.BeginPutObject(bucketName, key, fs, metadata, PutObjectCallback, result.ToCharArray());

        //            _event.WaitOne();
        //        }
        //    }
        //    catch (OssException ex)
        //    {
        //        Console.WriteLine("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}",
        //            ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Failed with error info: {0}", ex.Message);
        //    }
        //}

        private static void PutObjectCallback(IAsyncResult ar)
        {
            try
            {
                client.EndPutObject(ar);

                Console.WriteLine(ar.AsyncState as string);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _event.Set();
            }
        }
    }
}