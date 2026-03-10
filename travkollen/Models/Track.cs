using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace travkollen.Models
{
    public class Track
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Length { get; set; }
        public int Straight { get; set; }
    }
}
