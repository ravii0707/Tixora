using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tixora.Core.DTOs
{
    public class MovieWithShowTimesDTO
    {
        public MovieCreateDTO Movie { get; set; }
        public List<ShowTimeCreateDTO> Shows { get; set; }
    }
    public class MovieWithShowTimesResponseDTO
    {
        public MovieResponseDTO Movie { get; set; }
        public List<ShowTimeResponseDTO> Shows { get; set; }
    }
}
