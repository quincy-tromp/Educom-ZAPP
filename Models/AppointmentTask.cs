using System;
namespace Zapp.Models
{
	public class AppointmentTask
	{

		public int Id { get; set; }
		public int AppointmentId { get; set; }
		public int TaskId { get; set; }
		public string? AdditionalInfo { get; set; }
		public bool IsDone { get; set; }
	}
}

