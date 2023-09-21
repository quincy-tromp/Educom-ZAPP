using System;
using Microsoft.AspNetCore.Identity;

namespace Zapp.Models
{
	public class Employee : IdentityUser
	{
		public string Name { get; set; }
		public ICollection<Appointment> Appointments { get; } = new List<Appointment>();
	}
}

