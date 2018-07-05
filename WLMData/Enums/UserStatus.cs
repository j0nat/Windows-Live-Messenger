using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLMData.Enums
{
    public enum UserStatus : ushort
    {
        Offline = 0,
        Busy = 1,
        Away = 2,
        Available = 3
    };
}
