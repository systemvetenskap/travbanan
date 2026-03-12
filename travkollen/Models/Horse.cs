using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace travkollen.Models
{
    public class Horse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public DateOnly? DateOfDeath { get; set; }
        public int? SireId { get; set; }
        public int? DamId { get; set; }
        public int TrainerId { get; set; }
    }
}
