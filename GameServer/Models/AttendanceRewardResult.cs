using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityServer.Models
{
    public struct AttendanceRewardResult
    {
        public int ItemId { get; }
        public int Quantity { get; }
        
        public AttendanceRewardResult(int itemId, int quantity)
        {
            ItemId = itemId;
            Quantity = quantity;
        }
    }
}
