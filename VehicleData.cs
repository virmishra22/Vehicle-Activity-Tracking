using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle_Activity_Tracking
{
    public class VehicleData
    {
        public string totalDelayTime { get; set; }
        public string assignedTags { get; set; }
        public string address { get; set; }
        public string mediumHaltSinceDuration { get; set; }
        public string showStartTimeFirst { get; set; }
        public string showVehicle { get; set; }
        public int? distLeft { get; set; }
        public string remark { get; set; }
        public int? yesterdayKms { get; set; }
        public int? dailyKms { get; set; }
        public string showTripStartName { get; set; }
        public string lrEwayExpiryTime { get; set; }
        public string etaStatus { get; set; }
        public int? sourceDistanceInKm { get; set; }
        public string eta { get; set; }
        public string etaInitial { get; set; }
        public string endName { get; set; }
        public string shareTripLink { get; set; }
        public string driverMobile { get; set; }
        public string driverName { get; set; }
        public string vehicleType { get; set; }
        public string showPrimStatusV1 { get; set; }
        public string dttimePretty { get; set; }
        public string startName { get; set; }
    }

}
