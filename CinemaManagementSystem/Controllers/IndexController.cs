using CinemaManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Helpers;
using System.Web.Mvc;
using X.PagedList;

namespace CinemaManagementSystem.Controllers
{
    public class IndexController : Controller
    {
        CinemaManageSystemEntities db = new CinemaManageSystemEntities();
        // GET: Index
        public int IfLogin()
        {
            HttpCookie cookie = Request.Cookies["User"];
          
                if (cookie==null)
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
        public ActionResult Index()
        {
            ViewBag.code = IfLogin();
          
            //未上映电影
            ViewBag.NotSalemovies = db.MovieInfoes.Where(c=>c.OnSale==1).Take(8);
            //正在上映的电影
            ViewBag.nowmovie = db.MovieInfoes.Where(c=>c.OnSale==0).Count();
            //即将上映
            ViewBag.notmovie = db.MovieInfoes.Where(c => c.OnSale == 1).Count();
            var movies = db.MovieInfoes.Where(c => c.pic != null).Take(8);
            //票房榜
            string sArguments = @"sss.py";//这里是python的文件名字
            RunPythonScript(sArguments, "-u");
            //获得名字
            ViewBag.name = sp3;
            //获得价格
            ViewBag.nowprice = sp4;
            ViewBag.allprice = allprice;
            //最受期待
            var want = db.MovieInfoes.Where(c => c.OnSale==1 ).OrderByDescending(c=>c.WantSee);
            //第一名
          ViewBag.first= want.Take(1);
            //第二名
            ViewBag.second = want.Skip(1).Take(1);
            //第三名
            ViewBag.third = want.Skip(2).Take(1);
            //后面
            ViewBag.others = want.Skip(3).Take(7);
            return View("index",movies);
        }
        [HttpPost]
        public ActionResult Test(string order_no,float money)
        {
           
            return View();
        }
        public ActionResult MovieDetail(int?id)
        {
            HttpCookie cookie = Request.Cookies["User"];
            int uid = Convert.ToInt32(cookie.Value);
            ViewBag.code = IfLogin();
            //找到演员
            var movies = db.MovieInfoes.SingleOrDefault(c => c.movieid == id);
          
            string act=movies.actor;
            if (movies.other==1)
            {
                string[] Array = act.Split(',');
                 var m=Array.Take(3);
                string actors="";
                foreach (var item in m)
                {
                    actors += (item+" ");
                }
                ViewBag.actor = actors;
            }
            else
            {
            string[] strArray = act.Split(' ');
                var m = strArray.Take(3);
                string actors = "";
                foreach (var item in m)
                {
                    actors += (item + " ");
                }
                ViewBag.actor = actors;
            }
            
            //找到导演
            string dir = movies.director;
            string[] dirtor = dir.Split(',');
            ViewBag.dirtor = dirtor.Take(1);
            //相关电影的查询
            string[] type = movies.typeclass.Split(' ');
            int[] movieid = new int[10];
            string[] moviename = new string[10];
            string[] pic = new string[10];
            int i = -1;
            int a = 0;
            //拿到分类
            foreach (var item in type)
            {
                if (a==1)
                {
                    break;
                }
                else
                {
                    //对每个分类进行查找
             var types= db.MovieInfoes.Where(c => c.typeclass.Contains(item)&&c.moviename!=movies.moviename);
                foreach (var movie in types)
                {
                        //只要11个，循环10次
                    i++;
                    if (i>=10)
                    {
                        a = 1;
                        break;
                    }
                    else
                        {
                            int c = 1;
                            //去重
                            foreach (var sameitem in movieid)
                            {
                                if (movie.movieid==sameitem)
                                {
                                    c = 0;
                                    break;
                                }
                               
                            }
                            if (c==1)
                            {
                                movieid[i] = movie.movieid;
                                moviename[i] = movie.moviename;
                                pic[i] = movie.pic;
                            }
                            else
                            {
                                i--;
                            }
                          
                        }
                }
            }
            }
            ViewBag.Sameid = movieid;
            ViewBag.Samename = moviename;
            ViewBag.Samepic = pic;
            //最近上映
             ViewBag.newmovie = db.MovieInfoes.OrderByDescending(c => c.releasedate<=DateTime.Now).Take(6);



            string d = movies.typeclass;
            string newtype =" ";
            string[] ad = d.Split(' ');
            foreach (var item in ad)
            {
                if (item.ToString() == "全部")
                {
                }
                else
                {
                    newtype += item + " ";
                }
            }
            movies.typeclass = newtype;


            string cou = movies.country;
            string newcoun = " ";
            string[] county = cou.Split(' ');
            foreach (var item in county)
            {
                if (item.ToString() == "全部")
                {
                }
                else
                {
                    newcoun += item + " ";
                }
            }
            movies.country = newcoun;
          var want=  db.WantSees.SingleOrDefault(c => c.MovieId == id && c.UserId == uid);
            if (want==null)
            {
                ViewBag.imgs = 1;
            }
            else
            {
                ViewBag.imgs = 2;
            }
            return View("MovieDetail",movies);
        }
        static string[] spl;
        static  string[] sp2 = new string[10];
        static string[] sp3 = new string[10];
        static string[] sp4 = new string[10];
        static string[] sp5 = new string[10];
        static string[] sp6 = new string[10];
        static string[] sp7 = new string[10];
       static double allprice = 0;
        public ActionResult Piaofang()
        {
            i = 0;
            string sArguments = @"sss.py";//这里是python的文件名字
            RunPythonScript(sArguments, "-u");
           
            
            ViewBag.all = allprice;
            ViewBag.no = sp2;
            ViewBag.name = sp3;
            ViewBag.nowprice = sp4;
            ViewBag.bai = sp5;
            ViewBag.days = sp6;
            ViewBag.total = sp7;
            return View();
        }
        string path = "";
        public  void RunPythonScript(string sArgName, string args = "", params string[] teps)
        {
             path = this.Server.MapPath("~/Scripts/") + "sss.py";
            Process p = new Process();

            // 获得python文件的绝对路径（将文件放在c#的debug文件夹中可以这样操作）
            //path = @"C:\Users\19239\Desktop\test\" + sArgName;//(因为我没放debug下，所以直接写的绝对路径,替换掉上面的路径了)
            //p.StartInfo.FileName = @"C:\Users\Administrator\AppData\Local\Programs\Python\Python311\python.exe";
            p.StartInfo.FileName = @"python.exe"/*@" */;//(注意：用的话需要换成自己的)没有配环境变量的话，可以像我这样写python.exe的绝对路径(用的话需要换成自己的)。如果配了，直接写"python.exe"即可
         
           	// 会输出到 Output

            p.StartInfo.Arguments = path;

            p.StartInfo.UseShellExecute = false;

            p.StartInfo.RedirectStandardOutput = true;

            p.StartInfo.RedirectStandardInput = true;

            p.StartInfo.RedirectStandardError = true;

            p.StartInfo.CreateNoWindow = true;

            p.Start();
            p.BeginOutputReadLine();
            p.OutputDataReceived += new DataReceivedEventHandler(p_OutputDataReceived);
            System.Threading.Thread.Sleep(6000);
            int n = 0;
            allprice = 0;
            foreach (var item in r)
            {
                string s = item.Remove(0,5);
                spl = s.Split(' ').ToArray();
                spl = spl.Where(d => !string.IsNullOrEmpty(d)).ToArray();
                sp2[n] = spl[0];
                sp3[n] = spl[1];
                sp4[n] = spl[2];
                sp5[n] = spl[3];
                sp6[n] = spl[4];
                sp7[n] = spl[5];
                allprice += Convert.ToDouble(spl[2]);
                n++;
            }
            p.WaitForExit();
        }
        //输出打印的信息
        public static string[] r = new string[10];
        public static int i;
        static void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {

            if (!string.IsNullOrEmpty(e.Data))
            {
                string s = e.Data;

                AppendText(e.Data + Environment.NewLine);


            }

        }
        public static void AppendText(string text)
        {
            if (i == 0)
            {
                i++;
                return;
            }
            else
            {
                if (i == 11)
                {
                    return;
                }
                else
                {



                    r[i - 1] = text;
                    string s = text;
                    i++;
                }
                //此处在控制台输出.py文件print的结果
            }
        }
       static int pages;
        public ActionResult Movie(int page=1)
        {
            ViewBag.code = IfLogin();
            HttpCookie cookie = new HttpCookie("id");
            Response.Cookies.Add(cookie);
            ViewBag.movietype = db.MovieTypes.ToList();
            ViewBag.years = db.Years.ToList();
            ViewBag.area = db.areas.ToList();
            pages = page;
            return View();
        }
        
        [HttpPost]
        public ActionResult Movie(string Type, string Country, int?Date,int range)
        {
            ViewBag.code = IfLogin();
            HttpCookie cookie = Request.Cookies["id"];
            const int pagesize = 12;
            IPagedList movies;
            //全部已经上映
            if (cookie.Value != "2" )
            {
                //按照热门排序
                if (range==1)
              {
                if (Date == null )
                {
                    movies = db.MovieInfoes.Where(c => c.typeclass.Contains(Type) && c.country.Contains(Country) && c.OnSale == 0).OrderBy(c => c.movieid).ToPagedList(pages, pagesize);
                }
                else
                {
                    movies = db.MovieInfoes.Where(c => c.typeclass.Contains(Type) && c.country.Contains(Country) && c.years == Date && c.OnSale == 0).OrderBy(c => c.movieid).ToPagedList(pages, pagesize);
                }
              }
                //按照时间排序
                else
                {
                    if (Date == null)
                    {
                        movies = db.MovieInfoes.Where(c => c.typeclass.Contains(Type) && c.country.Contains(Country) && c.OnSale == 0).OrderBy(c => c.releasedate).ToPagedList(pages, pagesize);
                    }
                    else
                    {
                        movies = db.MovieInfoes.Where(c => c.typeclass.Contains(Type) && c.country.Contains(Country) && c.years == Date && c.OnSale == 0).OrderBy(c => c.releasedate).ToPagedList(pages, pagesize);
                    }
                }
            }
            //全部未上映
            else
            {
                if (Date == null)
                {
                    movies = db.MovieInfoes.Where(c => c.typeclass.Contains(Type) && c.country.Contains(Country) && c.OnSale == 1).OrderBy(c => c.movieid).ToPagedList(pages, pagesize);
                }
                else
                {
                    movies = db.MovieInfoes.Where(c => c.typeclass.Contains(Type) && c.country.Contains(Country) && c.years == Date && c.OnSale == 1).OrderBy(c => c.movieid).ToPagedList(pages, pagesize);
                }
            }
            return PartialView("_movie", movies);
        }
        public ActionResult Cinema(int?id)
        {
            ViewBag.code = IfLogin();
            if (id!=null)
            {
                ViewBag.change = 1;
            var movies=db.MovieInfoes.SingleOrDefault(c => c.movieid == id);
                ViewBag.moviename = movies.moviename;
                ViewBag.enname = movies.enname;
                ViewBag.director = movies.director;
               string sp=movies.actor;
                string[] mo;
                string actors = "";
                if (sp.Contains(','))
                {
                    mo = sp.Split(',');
                    var m = mo.Take(3);
                    foreach (var item in m)
                    {
                        actors += (item + " ");
                    }
                   
                }
                else
                {
                    mo = sp.Split(' ');
                    var m = mo.Take(3);
                    foreach (var item in m)
                    {
                        actors += (item + " ");
                    }

                }

                ViewBag.actor = actors;
                ViewBag.typeclass = movies.typeclass;
                ViewBag.country = movies.country;
                ViewBag.duration = movies.duration;
                ViewBag.releasedate = movies.releasedate;
                ViewBag.language = movies.language;
                ViewBag.movieid = movies.movieid;
                ViewBag.pic = movies.pic;
            }
            else
            {
                ViewBag.change = 0;
            }
            return View();
        }
        public ActionResult CinemaDetail(string id,int?movieid)
        {
            ViewBag.code = IfLogin();
            if (movieid==null)
            {
                ViewBag.movies = db.MovieInfoes.Take(7).ToList();
               
            }
            else
            {
                var select = db.MovieInfoes.SingleOrDefault(c => c.movieid == movieid);
              var all=  db.MovieInfoes.Where(c=>c.movieid!=movieid).OrderBy(c=>c.typeclass).Take(6).ToList();
                List<MovieInfo> allmovie=new List<MovieInfo>();
                allmovie.Add(select);
                foreach (var item in all)
                {
                    allmovie.Add(item);
                }
                ViewBag.movies = allmovie;
            }
            ViewBag.cinemaid = id;
            return View();
        }
        [HttpPost]
        public ActionResult CinemaDetail(int id)
        {
            ViewBag.code = IfLogin();
            var mo= db.MovieInfoes.SingleOrDefault(c => c.movieid == id);
            ViewBag.movies = mo;
            ViewBag.ids = mo.movieid;
            ViewBag.lang = mo.language;
            ViewBag.ticket = mo.Ticket;
            string a = mo.actor;
            string[] s = a.Split(' ');
            string d="";
            int i = 0;
            foreach (var item in s)
            {
                if (i>1)
                {
                    break;
                }
                else
                {
                    d += (item+" ");
                    i++;
                }
               
            }
            ViewBag.act = d;
            var halls = db.FilmInfors.ToList();
            return PartialView("_cinema",halls);
        }

        public ActionResult bangdan()
        {
            ViewBag.code = IfLogin();
            ViewBag.movies=db.MovieInfoes.OrderByDescending(c => c.releasedate).Where(c => c.OnSale == 0).Take(10);
            return View();
        }
        public ActionResult selectmovie(string moviename)
        {
            ViewBag.code = IfLogin();
            ViewBag.select=db.MovieInfoes.Where(c => c.moviename.Contains(moviename));
            return View();
        }
        public ActionResult WantSee(int id)
        {
            HttpCookie cookie = Request.Cookies["User"];
            int uid = Convert.ToInt32(cookie.Value);
            var list=db.WantSees.SingleOrDefault(c => c.MovieId == id && c.UserId == uid);
            if (list==null)
            {
                WantSee w = new WantSee();
                w.MovieId = id;
                w.UserId = uid;
                db.WantSees.Add(w);
                db.SaveChanges();
                ViewBag.num = 1;
                return PartialView("_WantSee");
            }
            else
            {

                db.WantSees.Remove(list);
                ViewBag.num = 2;
                db.SaveChanges();
                return PartialView("_WantSee");

            }
           
        }
    }

}