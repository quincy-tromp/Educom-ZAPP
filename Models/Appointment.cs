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

        private List<int>? taskIds = null; // NotMapped

		[NotMapped]
        public List<int> TaskIds
        {
            get
            {
                if (taskIds == null)
                {
                    taskIds = AppointmentTasks.Select(t => t.Id).ToList();
                }
                return taskIds;
            }
            set
            {
                taskIds = value;
            }
        }
        public Appointment()
        {
            AppointmentTasks = new List<AppointmentTask>();
        }
    }
}

