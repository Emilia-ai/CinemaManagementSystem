using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CinemaManagementSystem.Message
{
     class SendCode
    {
   
            public string Ext { get; set; }
            public string Extend { get; set; }
            public string[] Params { get; set; }
            public string Sig { get; set; }
            public string Sign { get; set; }
            public Phone Tel { get; set; }
            public string Time { get; set; }
            public string Tpl_id { get; set; }
        }
        /// <summary>
        /// 电话参数
        /// </summary>
        class Phone
        {
            public string Mobile { get; set; }
            public string Nationcode { get; set; }
        
    }
}
