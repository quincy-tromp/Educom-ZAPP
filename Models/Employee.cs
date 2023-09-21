using System;
using Microsoft.AspNetCore.Identity;

namespace Zapp.Models
{
	public class Employee : IdentityUser
	{
		//public new int Id { get; set; }
		public string Name { get; set; }
		public ICollection<Appointment> Appointments { get; } = new List<Appointment>();
	}
}

