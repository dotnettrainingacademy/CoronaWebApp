﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoronaAppAPI.Models
{
    public class AppUser : IdentityUser
    {
        [Column(TypeName = "nvarchar(50)")]
        public string FullName { get; set; }
    }
}
