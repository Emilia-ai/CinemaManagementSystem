using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CinemaManagementSystem.Models;
using System.IO;
using System.Net;

namespace CinemaManagementSystem.Controllers
{
    public class UserInfoController : Controller
    {
        CinemaManageSystemEntities db = new CinemaManageSystemEntities();
        public int IfLogin()
        {
            HttpCookie cookie = Request.Cookies["User"];
            if (cookie == null)
            {
                return 1;
            }
            else
            {
                int no = Convert.ToInt32(cookie.Value);
                var mem = db.MemberInfors.SingleOrDefault(c => c.Mno == no);
                ViewBag.picture = mem.img;
                return 2;
            }
        }
        // GET: UserInfo
        public ActionResult Infoes()
        {
            ViewBag.code = IfLogin();
            if (Request.Cookies["User"].Value=="null")
            {
            return RedirectToAction("LoginAndResinger", "Login");
            }
            else
            {
                int uid = Convert.ToInt32(Request.Cookies["User"].Value);
                ViewBag.infoes = db.OrderDetails.Include("MemberInfor").Where(c => c.Userid == uid).OrderByDescending(c=>c.creattime);
                var num = db.OrderDetails.Include("MemberInfor").Where(c => c.Userid == uid).ToList();
                if (num.Count == 0)
                {
                    ViewBag.noresult = 1;
                }
                ViewBag.refund = db.refunds.ToList();
                return View();
            }
            
        }
        [HttpPost]
        public ActionResult Infoes(int model)
        {
            int uid = Convert.ToInt32(Request.Cookies["User"].Value);
            if (model == 1)
            {
                var num = db.OrderDetails.Include("MemberInfor").Where(c => c.Userid == uid).ToList();
                if (num.Count==0)
                {
                    ViewBag.noresult = 1;
                }
                ViewBag.infoes = db.OrderDetails.Include("MemberInfor").Where(c => c.Userid == uid);
                ViewBag.model = 1;
                return PartialView("_UserInfo");
            }
            else
            {
                ViewBag.model = 2;
                var tel = db.MemberInfors.SingleOrDefault(c => c.Mno == uid);
                if (tel.money==0)
                {
                    ViewBag.money =0;
                }
                else
                {
                    ViewBag.money = tel.money;
                }
                ViewBag.pic = tel.img;
                if (tel.Mphone == null)
                {
                    ViewBag.tel = "未设置";
                }
                else
                {
                    ViewBag.tel = tel.Mphone;
                }
                string m = tel.Memail;
                if (m == null)
                {
                    ViewBag.mail = "未设置";
                }
                else
                {
                    ViewBag.mail = m;
                }
                ViewBag.name = tel.Mname;
                return PartialView("_UserInfo");
            }
          
        }
        [HttpPost]
        public ContentResult Uploadpic(HttpPostedFileBase file)
        {
            int uid = Convert.ToInt32(Request.Cookies["User"].Value);
            if (file!=null)
            {
            var fileName = file.FileName;
            string last = fileName.Substring(fileName.LastIndexOf(".") + 1);
            if (last != "jpg" && last != "png" && last != "JPEG")
            {
                return Content("<script>alert('上传类型不正确!');history.go(-1);</script>");
            }
                var infoes = db.MemberInfors.SingleOrDefault(c => c.Mno == uid);
                string newname = infoes.Mno + "." + last;
                string image = Server.MapPath("~/img/") + newname;
             file.SaveAs(image);
             infoes.img = "https://image.summer-life.asia/User/" + newname;
                UploadFile(image);
                db.SaveChanges();
            }
          
            return Content("<script>alert('修改成功!');history.go(-1);</script>");


        }
        private static string ftpServerIP = "43.133.32.239:21";//服务器ip
        private static string ftpUserID = "Emilia";//用户名
        private static string ftpPassword = "Maria131";//密码
        public static bool UploadFile(string filename)
        {
            FileInfo fileInf = new FileInfo(filename);
            string uri = "ftp://" + ftpServerIP + "/" + "image/" + "User/" + fileInf.Name;

            FtpWebRequest reqFTP;


            // 根据uri创建FtpWebRequest对象 
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));

            // ftp用户名和密码
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);

            // 默认为true，连接不会被关闭
            // 在一个命令之后被执行
            reqFTP.KeepAlive = false;

            // 指定执行什么命令
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;

            // 指定数据传输类型
            reqFTP.UseBinary = true;

            // 上传文件时通知服务器文件的大小
            reqFTP.ContentLength = fileInf.Length;

            // 缓冲大小设置为2kb
            int buffLength = 2048;

            byte[] buff = new byte[buffLength];
            int contentLen;


            // 打开一个文件流 (System.IO.FileStream) 去读上传的文件
            FileStream fs = fileInf.OpenRead();
            try
            {
                // 把上传的文件写入流
                Stream strm = reqFTP.GetRequestStream();


                // 每次读文件流的2kb
                contentLen = fs.Read(buff, 0, buffLength);


                // 流内容没有结束
                while (contentLen != 0)
                {
                    // 把内容从file stream 写入 upload stream
                    strm.Write(buff, 0, contentLen);


                    contentLen = fs.Read(buff, 0, buffLength);
                }


                // 关闭两个流
                strm.Close();
                fs.Close();
                return true;
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message, "Upload Error");
                return false;
            }
        }
        [HttpPost]
        public string Upload(string name)
        {
            int uid = Convert.ToInt32(Request.Cookies["User"].Value);
            if (name!="")
            {
                var infoes = db.MemberInfors.SingleOrDefault(c => c.Mno == uid);
                infoes.Mname = name;
                db.SaveChanges();
            }
            return "1";
        }
        [HttpPost]
        public string charge(string code)
        {
            int id = Convert.ToInt32(Request.Cookies["User"].Value);
             var mem = db.MemberInfors.SingleOrDefault(c => c.Mno == id); 
            mem.money = mem.money + 100;
            db.SaveChanges();
            return "1";
        }
        public string refund(string code)
        {
            var list = db.OrderDetails.SingleOrDefault(c => c.Orderid == code);
            refund re = new refund();
            re.orderid = list.Orderid;
            re.uid = list.Userid;
            re.moviename = list.moviename;
            re.time = list.time;
            re.cinema = list.cinema;
            re.seat = list.Seat;
            re.price = list.price;
            re.State = 1;
            db.refunds.Add(re);
            db.SaveChanges();
            return "<script>alert('申请退票成功,客服人员将尽快处理');location.href='/UserInfo/Infoes/'</script>";
        }
        public string DeleteDetail(string code)
        {
            var list = db.OrderDetails.SingleOrDefault(c => c.Orderid == code);
            db.OrderDetails.Remove(list);
            db.SaveChanges();
            return "<script>location.href='/UserInfo/Infoes/'</script>";
        }
        //public ActionResult dodelete()
        //{
        //    var moview = db.MovieInfoes.ToList();

        //    foreach (var item in moview)
        //    {
        //        string newtype = "全部"+" ";
        //        string[] a = item.country.Split(' ');
        //        foreach (var it in a)
        //        {
        //            //if (it.ToString() == "全部")
        //            //{
        //            //    break;
        //            //}
        //            //else
        //            //{

        //              newtype += it + " ";
        //            //}
        //        }
        //        item.country = newtype;
        //        db.SaveChanges();
        //    }
        //    return View();
        //}
    }
}