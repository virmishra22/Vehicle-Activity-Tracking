using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle_Activity_Tracking
{
    public class VehicleDashboardResponse
    {
        public string msg { get; set; }
        public VehicleData data { get; set; }
        public DateTime timeStamp { get; set; }
        public string debugMessage { get; set; }
        public string success { get; set; }
    }

}
