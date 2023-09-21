using System;
namespace Zapp.Models
{
	public class Appointment
	{
		public int Id { get; set; }
		public int CustomerId { get; set; }
		public Customer Customer { get; set; } = null!;
		public string EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;
        public DateTime Scheduled { get; set; }
		public DateTime? CheckedIn { get; set; }
		public DateTime? CheckedOut { get; set; }

		public ICollection<AppointmentTask> AppointmentTasks = new List<AppointmentTask>();
	}
}

