using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace CinemaManagementSystem.Maildata
{
    public class Mail
    {
       public int Send(string Mailto)
        {
            MailAddress MessageFrom = new MailAddress("Yamato1220@163.com"); //发件人邮箱地址 
            string MessageTo = Mailto; //收件人邮箱地址 
            string MessageSubject = "激活验证"; //邮件主题 
            string MessageBody = "请进行邮箱验证来完成您注册的最后一步,点击下面的链接激活您的帐号：<br><a target='_blank' rel='nofollow' style='color: #0041D3; text-decoration: underline' href='https://summer-life.asia/LoginAndResinger/ActiveMail?Email=" + Mailto +"'>激活</a>"; //邮件内容 （一般是一个网址链接，生成随机数加验证id参数，点击去网站验证。）
     
            if (SendMail(MessageFrom, MessageTo, MessageSubject, MessageBody))
            {
                //发送成功返回1
                return 1;
                //Response.Write("<script type='text/javascript'>alert('发送邮件失败');</script>");
            }
            else
            {
                //发送失败返回0
                return 0;
                //Response.Write("<script type='text/javascript'>alert('发送邮件失败');</script>");
            }
        }
        public bool SendMail(MailAddress MessageFrom, string MessageTo, string MessageSubject, string MessageBody)  //发送验证邮件
        {
            MailMessage message = new MailMessage();
            message.To.Add(MessageTo);
            message.From = MessageFrom;
            message.Subject = MessageSubject;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.Body = MessageBody;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true; //是否为html格式 
            message.Priority = MailPriority.High; //发送邮件的优先等级 
            SmtpClient sc = new SmtpClient();
            sc.EnableSsl = true;//是否SSL加密
            sc.Host = "smtp.163.com"; //指定发送邮件的服务器地址或IP 
            sc.Port = 25; //指定发送邮件端口 
            sc.Credentials = new System.Net.NetworkCredential("Yamato1220@163.com", "YVVRNLIURGKBDMWL"); //指定登录服务器的用户名和密码(注意：这里的密码是开通上面的pop3/smtp服务提供给你的授权密码，不是你的qq密码)

            try
            {
                sc.Send(message); //发送邮件 
            }
            catch (Exception e)
            {
                Debug.Write(e);
                return false;
            }
            return true;

        }
    }
}