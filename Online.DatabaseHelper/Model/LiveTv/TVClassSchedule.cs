using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online.DbHelper.Model
{
    public class TVClassSchedule
    {
        public int SCId { get; set; }

        public int? LiveRoomId { get; set; }

        public string Teacher { get; set; }

        public string TNickName { get; set; }

        public string HomeUrl { get; set; }

        public string liveStartTime { get; set; }

        public string liveEndTime { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime EffectiveStartTime { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime EffectiveEndTime { get; set; }

        public virtual LiveRooms LiveRooms { get; set; }
    }
}
