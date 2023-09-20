using System;
namespace Zapp.Models
{
	public class Appointment
	{
		public int Id { get; set; }
		public int CustomerId { get; set; }
		public int EmployeeId { get; set; }
		public DateTime Scheduled { get; set; }
		public DateTime? CheckedIn { get; set; }
		public DateTime? CheckedOut { get; set; }
	}
}

