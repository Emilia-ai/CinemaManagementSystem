using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CinemaManagementSystem.Models;
using CinemaManagementSystem.Message;
using Newtonsoft.Json;
using CinemaManagementSystem.Maildata;
using static System.Net.WebRequestMethods;

namespace CinemaManagementSystem.Controllers
{

    public class LoginAndResingerController : Controller
    {
        
        CinemaManageSystemEntities db = new CinemaManageSystemEntities();
        // GET: LoginAndResinger
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Resigner()
        {
            return View();
        }
        [HttpPost]
        public string Login(string Email,string PassWord,string phonenum)
        {
            if (Email == null)
            {
                if (db.MemberInfors.SingleOrDefault(c => c.Mphone == phonenum && c.Mpassword == PassWord) != null)
                {
                    var info = db.MemberInfors.SingleOrDefault(c => c.Mphone == phonenum && c.Mpassword == PassWord);
                    HttpCookie cookie = new HttpCookie("User");
                    cookie.Value = info.Mno.ToString();
                    var millisecond = DateTime.Now.AddMinutes(60);
                    cookie.Expires = millisecond;
                    Response.Cookies.Add(cookie);
                    //1表示登录成功
                    return "1";
                  
                }
                else
                {
                    //0表示失败
                    return "0";
                }
            }
            else
            {
                if (db.MemberInfors.SingleOrDefault(c =>c.Memail == Email && c.Mpassword == PassWord) != null)
                {
                    var info = db.MemberInfors.SingleOrDefault(c => c.Memail == Email && c.Mpassword == PassWord);
                    HttpCookie cookie = new HttpCookie("User");
                    cookie.Value = info.Mno.ToString();
                    var millisecond = DateTime.Now.AddMinutes(60);
                    cookie.Expires = millisecond;
                    Response.Cookies.Add(cookie);
                    return "1";
                }
                else
                {
                    return "0";
                }
            }
        }
        //验证码是否匹配
        public JsonResult checkcodes(string tel,string check,string telpass)
        {
            string code;
            if (TempData["code"]!=null)
            {
                code= TempData["code"].ToString();
            }
            else
            {
                code = TempData["recode"].ToString();
            }
            if (check == code)
            {
                MemberInfor member = new MemberInfor();
                member.Mphone = tel;
                member.Mpassword = telpass;
                member.createtime = DateTime.Now;
                member.img= "https://image.summer-life.asia/User/default.png";
                db.MemberInfors.Add(member);
                db.SaveChanges();
                return Json("<script>alert('注册成功!');location.href='/LoginAndResinger/Login/'</script>");
            }
            else
            {
                TempData["recode"] = code;
                return Json("验证码错误!");
            }

          
        }
        //手机验证，发送激活码
        public string Sendcode(string tel)
        {
            string data = tel;
            string tels = data;
            if (db.MemberInfors.Where(c => c.Mphone.Contains(tels)).Count() > 0)
            {
                return ("该账号已经被注册!");
            }
            else
            {
                string mobile = tels;//自己要验证收短信的手机号
                string appkey = "785bc99b3da4983db209c5ccc687bb38";//自己在腾讯云上申请的App Key
                string random = StaticClass.GenerateRandomCode(6);
              TempData["code"]= Convert.ToInt32(random);
                string time = StaticClass.GetTimeStamp(10).ToString();
                string sig = StaticClass.Sha256($"appkey={appkey}&random={random}&time={time}&mobile={mobile}");
                var postData = new SendCode
                {
                    Ext = "",
                    Extend = "",
                    Params = new string[] { random },
                    Sig = sig,
                    Sign = "毕业设计",//自己审核通过的短信签名
                    Tel = new Phone { Mobile = tels,/*自己要验证收短信的手机号*/ Nationcode = "86"/*国家标识*/ },
                    Time = time,
                    Tpl_id = "1437892"//自己审核通过的短信模版id
                };
                string url = $"https://yun.tim.qq.com/v5/tlssmssvr/sendsms?sdkappid=1400685216&random={random}";//sdkappid=自己在腾讯云上申请的SDK AppID
                string postDataStr = JsonConvert.SerializeObject(postData).ToLower();
                string result = StaticClass.HttpPost(url, postDataStr);
                return ("发送成功");
            }
        }
        
        public ActionResult test()
        {
     
            return View();
        }
        public ActionResult change()
        {
            return PartialView("ResignerMethods");
        }
        //检查邮箱号是否使用过
        public string checkMail(string Email,string PassWord)
        {
            if (db.MemberInfors.Where(c=>c.Memail==Email && c.MailActive==1).Count()>0)
            {
                //邮箱已经注册过的情况
                return "0";
            }
            else
            {
                if (db.MemberInfors.Where(c => c.Memail == Email).Count() > 0)
                {
                    //发过激活邮件但是没有激活成功，重新发送
                    Mail sendmail = new Mail();
                    sendmail.Send(Email);
                }
                else
                {
                    //新用户就重新创建账号
                    MemberInfor member = new MemberInfor();
                    member.Memail = Email;
                    member.Mpassword = PassWord;
                    member.SendMailTIme = DateTime.Now;
                    member.img = "https://image.summer-life.asia/User/default.png";
                    db.MemberInfors.Add(member);
                    db.SaveChanges();
                    Mail sendmail = new Mail();
                    sendmail.Send(Email);

                }
                return "1";
            }
        }
        //收到激活邮件后来激活
        public ActionResult ActiveMail(string Email)
        {
            DateTime Nowtime = DateTime.Now;
            MemberInfor SelectMember = db.MemberInfors.SingleOrDefault(c => c.Memail == Email);
            DateTime Sendtime = Convert.ToDateTime(SelectMember.SendMailTIme);
            TimeSpan times = Nowtime - Sendtime;
            if (times.Minutes >= 10)
            {
                //超过激活时间
                ViewBag.code = 1;
            }
            else
            {
                MemberInfor member = db.MemberInfors.SingleOrDefault(c => c.Memail == Email);
                member.MailActive = 1;
                db.SaveChanges();
                ViewBag.code = 2;
            }
            
            return View();
        }
        
    }
    
}