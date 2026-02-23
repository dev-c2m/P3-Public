using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityServer.Player
{
    public class PlayerAttendance
    {
        public int AttendanceId { get; private set; }
        public int DayCount { get; private set; }
        public bool CanCheckIn { get; private set; }

        public PlayerAttendance(int attendanceId, int dayCount, bool canCheckIn)
        {
            AttendanceId = attendanceId;
            DayCount = dayCount;
            CanCheckIn = canCheckIn;
        }

        public void CheckIn()
        {
            if (CanCheckIn)
            {
                DayCount++;
                CanCheckIn = false;
            }
        }
    }
}
