using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace travkollen.ViewModels
{
    public class HorseDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Age { get; set; }
        public string TrainerName { get; set; }
        public int TrainerId { get; set; }
        public string? TrackName { get; set; }
        public string? SireName { get; set; }
        public int? SireId { get; set; }
        public string? DamName { get; set; }
        public int? DamId { get; set; }
        public string? ImageUrl { get; set; }
    }
}
