using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityServer.Data
{
    public enum ItemType
    {
        Consumable = 0,
        Etc = 1,
    }

    public class ItemData : IDataWithId
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ItemType Type { get; set; }
        public int Value { get; set; }
    }
}
