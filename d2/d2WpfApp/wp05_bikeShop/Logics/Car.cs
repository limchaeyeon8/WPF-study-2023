using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace wp05_bikeShop.Logics
{

    internal class Car
    {
        public string Names { get; set; }    // 오토 프로퍼티(매개변수 따로 안 만듦)
        public double Speed { get; set; }
        public Color Colour { get; set; }
        public Human Driver { get; set; }   
    }
}
