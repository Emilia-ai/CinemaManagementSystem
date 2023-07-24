using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CinemaManagementSystem.Models;
using XddPay;

namespace CinemaManagementSystem.Controllers
{
    public class TicketDetailController :Controller
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

        public ActionResult Ticket(int?id,int?no,string cinemas,string address,string tel)
        {
            IfLogin();
            if (Request.Cookies["User"] != null)
            { 
                TempData["address"] = address;
                TempData["tel"] = tel;
                ViewBag.movie = db.MovieInfoes.SingleOrDefault(c => c.movieid == id);
                var detail = db.MovieInfoes.SingleOrDefault(c => c.movieid == id);
                string alltype = detail.typeclass;
                string[] type = alltype.Split(' ');
                var m = type.Take(3);
                string types = "";
                foreach (var item in m)
                {
                    types += (item + " ");
                }
                ViewBag.tag = types;
                var halls = db.FilmInfors.SingleOrDefault(c => c.Fno == no);
                ViewBag.hall = halls;
                if (halls.FOrder!= null)
                {
                    ViewBag.seat = halls.FOrder.Split(',');
                }
               
                ViewBag.cinema = cinemas;
                return View();
            }
            else
            {
                return Content("<script>alert('请先登录');window.location.href='/LoginAndResinger/Login';</script>");
                //return RedirectToAction("Index","Index");
            }
        }

        public ActionResult Pay(string mname,int fileid, string cinema,double money)
        {
            ViewBag.code = IfLogin();
            ViewBag.mname = mname;
          var time=  db.FilmInfors.SingleOrDefault(c => c.Fno == fileid);
            ViewBag.File = fileid;
            ViewBag.time = time.FStart;
            ViewBag.cinema = cinema;
          string s= Request.QueryString["seats"];
            string[] d = s.Split(',');
            string seat = "";
            foreach (var item in d)
            {
                seat += (item + " ");
            }
            ViewBag.seat = seat;
            ViewBag.money = money;
            return View();
        }
        [HttpPost]
        public string Pay(string mname, int fileno, string cinema, double?money,string seat)
        {
            ViewBag.code = IfLogin();
            var all= db.FilmInfors.SingleOrDefault(c => c.Fno == fileno);
            DateTime time = all.FStart;
            string w = seat;
            string[] s = w.Trim().Split(' ');
            string orderseat = "";
            int i = 0;
            foreach (var item in s)
            {
                string row = item.Substring(0, 1);
                string list = item.Substring(2, 1);
                orderseat += row + "_" + list+",";
                i++;
            }
            all.FOrder += orderseat;
            HttpCookie cookie = Request.Cookies["User"];
            int uid = Convert.ToInt32(cookie.Value);
            var ss = db.MovieInfoes.SingleOrDefault(c => c.moviename == mname);
            string address = TempData["address"].ToString();
            string tel = TempData["tel"].ToString();
            OrderDetail order = new OrderDetail();
            order.moviename = mname;
            order.Orderid = DateTime.Now.ToString("yyyyMMddHHmmss");
            order.Userid = uid;
            DateTime times =time;
            order.time = times.ToString("ddd") + " " + times.ToString("M") + " " + times.ToString("T");
            order.cinema = cinema;
            order.Seat = seat;
            order.address = address;
            order.tel = tel;
            order.price = (decimal)money;
            order.state = 1;
            order.year = time;
            order.pic = ss.pic;
            order.creattime = DateTime.Now;
            TempData["id"] = order.Orderid;
            TempData["money"] = (decimal)money;
            db.OrderDetails.Add(order);
            db.SaveChanges();
            return "2";
            
        }
        public ActionResult AirPay()
        
        {
           
            return View();
        }
        public string Pays()
        {
            IfLogin();
            decimal m = (decimal)TempData["money"];
            //接收参数
            string subject = "电影票";
            int pay_type = 43;
            decimal money = m;
            string extra = "您的电影票";

            //创建订单
            string order_no = TempData["id"].ToString(); //商户的订单号

            //构造请求函数，无需修改
            XddpayService xddservice = new XddpayService(order_no, subject, pay_type.ToString(), money.ToString("f2"), extra);
            string sHtmlText = xddservice.Build_Form();
            //打印页面
            //ViewBag.no = order_no;
            //ViewBag.money = money.ToString("f2");
            //ViewBag.html = sHtmlText;
            return sHtmlText;
        } 
    }
}