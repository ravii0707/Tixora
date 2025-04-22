using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tixora.Core.DTOs
{
    public class BulkShowtimeRequest
    {
        public int MovieId { get; set; }
        public required List<ShowTimeDTO> ShowTimes { get; set; }
    }

    public class ShowTimeDTO
    {
        public int ShowTimeId { get; set; }
        public TimeOnly ShowTime { get; set; }  // TimeOnly data type
        public int TicketCount { get; set; }
    }

}
