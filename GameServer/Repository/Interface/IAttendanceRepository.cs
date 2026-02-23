using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityServer.Models;

namespace UnityServer.Repository.Interface
{
    public interface IAttendanceRepository
    {
        int GetLastAttendanceDayCount(int accountId, int attendanceId);
        bool CanCheckInToday(int accountId, int attendanceId);
        void CheckAttendance(int accountId, int attendanceId, int day);
        void TryCheckAttendance(int accountId, int attendanceId, int day);
        AttendanceResult GetAttendanceDayCountAndCanCheckIn(int accountId, int attendanceId);
    }
}
