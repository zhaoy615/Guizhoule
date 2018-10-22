﻿using Aliyun.OSS;
using DAL.DAL;
using MJBLL.common;
using MJBLL.model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestMJ
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
          var list=  textBox1.Text.Split(',');
            List<MJBLL.model.ServerMaJiang> mjlist = new List<ServerMaJiang>();
            for (int i = 0; i < list.Length; i++)
            {
                mjlist.Add(  new ServerMaJiang { PaiHs = int.Parse(list[i]), PaiId = i });
            }
         //   bool is_qys = false;
            //if (mjlist[mjlist.Count - 1].PaiHs - mjlist[0].PaiHs < 9 && mjlist[0].PaiHs / 10 == mjlist[mjlist.Count - 1].PaiHs / 10)
            //{
            //    if (mjlist[0].PaiHs > 10 && mjlist[0].PaiHs < 20)
            //    {
            //        int count = 0;// thisuser.Peng.FindAll(u => u.PaiHs < 10 || u.PaiHs > 20).Count;
            //                      //if (!string.IsNullOrEmpty(thisuser.Gong))
            //                      //{


            //        //    string gang = thisuser.Gong.Remove(thisuser.Gong.Length - 1, 1);
            //        //    string[] arr = gang.Split(',');
            //        //    int panduan = 0;
            //        //    foreach (var item in arr)
            //        //    {
            //        //        string[] newarr = item.Split('|');
            //        //        if (int.Parse(newarr[0]) < 10 || int.Parse(newarr[0]) > 20)
            //        //        {
            //        //            panduan++;
            //        //            is_qys = false;
            //        //            break;
            //        //        }
            //        //    }
            //        //    if (panduan == 0 && count == 0)
            //        //    {
            //        //        is_qys = true;
            //        //    }
            //        //}
            //        //else
            //        //{
            //        if (count == 0)
            //        {
            //            is_qys = true;
            //        }
            //        //}
            //    }
            //    else if (mjlist[0].PaiHs < 10)
            //    {
            //        int count = 0;//thisuser.Peng.FindAll(u => u.PaiHs > 10).Count;
            //                      //if (!string.IsNullOrEmpty(thisuser.Gong))
            //                      //{
            //                      //    string gang = thisuser.Gong.Remove(thisuser.Gong.Length - 1, 1);
            //                      //    string[] arr = gang.Split(',');
            //                      //    int panduan = 0;
            //                      //    foreach (var item in arr)
            //                      //    {
            //                      //        string[] newarr = item.Split('|');
            //                      //        if (int.Parse(newarr[0]) > 10)
            //                      //        {
            //                      //            panduan++;
            //                      //            is_qys = false;
            //                      //            break;
            //                      //        }
            //                      //    }
            //                      //    if (panduan == 0 && count == 0)
            //                      //    {
            //                      //        is_qys = true;
            //                      //    }
            //                      //}
            //                      //else
            //                      //{
            //        if (count == 0)
            //        {
            //            is_qys = true;
            //        }
            //        //}
            //    }
            //    else
            //    {
            //        int count = 0;// thisuser.Peng.FindAll(u => u.PaiHs < 20).Count;
            //                      //if (!string.IsNullOrEmpty(thisuser.Gong))
            //                      //{
            //                      //    string gang = thisuser.Gong.Remove(thisuser.Gong.Length - 1, 1);
            //                      //    string[] arr = gang.Split(',');
            //                      //    int panduan = 0;
            //                      //    foreach (var item in arr)
            //                      //    {
            //                      //        string[] newarr = item.Split('|');
            //                      //        if (int.Parse(newarr[0]) < 20)
            //                      //        {
            //                      //            panduan++;
            //                      //            is_qys = false;
            //                      //            break;
            //                      //        }
            //                      //    }
            //                      //    if (panduan == 0 && count == 0)
            //                      //    {
            //                      //        is_qys = true;
            //                      //    }
            //                      //}
            //                      //else
            //                      //{
            //        if (count == 0)
            //        {
            //            is_qys = true;
            //        }
            //        //}
            //    }
            //}
            //if (qy == 0)
            //    {
            //        label1.Text ="10   ";
            //    }

            //}
            //  string stat = "";
            //stat = new Ting().GetHuQD(mjlist, true);
            //stat = new Ting().GetHuDD(mjlist, 0);
            label1.Text += new Ting().GetTing(mjlist);
            //var qym = mjlist.Select(w => new { Tong = (w.PaiHs < 10 ? 1 : 0), Tiao = (w.PaiHs > 10 && w.PaiHs < 20 ? 1 : 0), Wan = (w.PaiHs > 20 ? 1 : 0) });
            //var mydata = from r in qym
            //             group r by new { r.Tiao, r.Tong, r.Wan } into g
            //             select new { key = g.Key, number = g.Count() };
        }
        static string accessKeyId = "LTAIooatOLctSD7E";
        static string accessKeySecret = "4N1B12IRUdb54u8zDLyee8RkUintOA";
        static string endpoint = "oss-cn-shenzhen.aliyuncs.com";
        static OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);
        static string bucketName = "zh-imageserver";

        private void button2_Click(object sender, EventArgs e)
        {
            //bool exists = false;
            //RoomInfoDAL dal = new RoomInfoDAL();
            //string roomInfoID = string.Empty;
            //do
            //{
            //    roomInfoID = Guid.NewGuid().ToString();
            //    exists = dal.GetExistsByRoomInfoID(roomInfoID);
            //} while (exists);
            WebResponse response = null;
            Stream stream = null;
            var memoryStream = new MemoryStream();
            var timeOut = 500;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(textBox1.Text);
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
        var    fileName = Guid.NewGuid() + ".jpg"; ;
            using (Stream fileStream = new MemoryStream(memoryStream.ToArray()))//转成Stream流  
            {
                try
                {
                    client.PutObject(bucketName, fileName, fileStream);
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

            // Console.WriteLine("Put object:{0} succeeded", key);
            label1.Text= "http://zh-imageserver.oss-cn-shenzhen.aliyuncs.com/" + fileName;
        }

        private void button3_Click(object sender, EventArgs e)
        {
           
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ;
            ;
            label1.Text = Erth.GetDistance(textBox2.Text, textBox3.Text).ToString();
        }
    }
}