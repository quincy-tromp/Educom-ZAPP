using System;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace Zapp.Models
{
	[Table("Employee")]
	public class Employee : IdentityUser
	{
        [Required]
        public string Name { get; set; }

		public ICollection<Appointment> Appointments { get; } = new List<Appointment>();
	}
}

