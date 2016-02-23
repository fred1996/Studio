using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportUserData
{
    public enum OldRole
    {

        超管 = 7,
        频道管理 = 6,
        巡管 = 5,
        讲师 = 13,
        大亨VIP = 12,
        至尊VIP = 11,
        钻石VIP = 10,
        铂金VIP = 4,
        黄金VIP = 3,
        白银VIP = 2,
        伯爵 = 9,
        子爵 = 8,
        会员 = 1
    }
}
