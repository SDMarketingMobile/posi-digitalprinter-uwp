using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSIDigitalPrinter.Model
{
    public enum ViewMode : int
    {
        LIST = 1,
        GRID = 2
    }

    public enum ScreenType : int
    {
        PRODUCTION  = 1,
        CONFERENCE  = 2,
        DELIVERY    = 3
    }

    public class Settings
    {
        public ViewMode ViewMode { get; set; }
        public ScreenType ScreenType { get; set; }
        public String ApiIp { get; set; }
        public int ApiPort { get; set; }
    }
}
