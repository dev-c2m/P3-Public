using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityServer.Data
{
    public interface IDataWithId
    {
        int Id { get; set; }
    }
}
