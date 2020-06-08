using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoronaAppAPI.Models
{
    public class CoronaTrack
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string PatientReport { get; set; }
        public string PatientStatus { get; set; }
        public int Age { get; set; }
    }
}
