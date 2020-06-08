using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoronaAppAPI.ViewModel
{
    public class PasswordResetViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
