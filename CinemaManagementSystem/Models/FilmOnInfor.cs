//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace CinemaManagementSystem.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class FilmOnInfor
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FilmOnInfor()
        {
            this.SeatInfors = new HashSet<SeatInfor>();
            this.Tickets = new HashSet<Ticket>();
        }
    
        public int Fshow { get; set; }
        public Nullable<int> Fno { get; set; }
        public string Hname { get; set; }
        public Nullable<System.DateTime> Fdate { get; set; }
        public Nullable<double> Fprice { get; set; }
        public Nullable<int> Remainnum { get; set; }
    
        public virtual FilmInfor FilmInfor { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SeatInfor> SeatInfors { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}
