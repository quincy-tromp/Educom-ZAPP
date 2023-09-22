using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace Zapp.Models
{
	public class Appointment
	{
		public int Id { get; set; }
        [Required]
        public int CustomerId { get; set; }
		public Customer Customer { get; set; } = null!;
        [Required]
        public string EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;
        [Required]
        public DateTime Scheduled { get; set; }
		public DateTime? CheckedIn { get; set; }
		public DateTime? CheckedOut { get; set; }

		public ICollection<AppointmentTask> AppointmentTasks = new List<AppointmentTask>();
	}
}

