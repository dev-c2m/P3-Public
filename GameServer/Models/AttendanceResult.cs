using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityServer.Models
{
    public struct AttendanceResult
    {
        public int DayCount { get; }
        public bool CanCheckIn { get; }

        public AttendanceResult(int dayCount, bool CanCheckIn)
        {
            DayCount = dayCount;
            this.CanCheckIn = CanCheckIn;
        }
    }
}
