using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wp12_finedustCheck.Models
{
    public class DustSensor
    {
        public int Id { get; set; }
        public string Dev_id { get; set; }
        public string Name { get; set; }
        public string Loc { get; set;}
        public double Coordx { get; set;}
        public double Coordy { get; set;}
        public bool Ison { get; set;}
        public int Pm10_After { get; set;}  // 미세
        public int Pm25_After { get; set; } // 초미세
        public int State { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Company_Id { get; set; }
        public string Company_Name { get; set; }


    }
}
