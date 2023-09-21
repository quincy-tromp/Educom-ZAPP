using System;
namespace Zapp.Models
{
	public class CareTask
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public ICollection<AppointmentTask> AppointmentTasks = new List<AppointmentTask>();
		public ICollection<CustomerTask> CustomerTasks = new List<CustomerTask>();
	}
}

