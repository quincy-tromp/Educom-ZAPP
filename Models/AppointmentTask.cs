using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace Zapp.Models
{
	public class AppointmentTask
	{
		public int Id { get; set; }
        [Required]
        public int AppointmentId { get; set; }
		public Appointment Appointment { get; } = null!;
        [Required]
        public int TaskId { get; set; }
		public TaskItem Task { get; } = null!;
        [Column(TypeName = "varchar(256)")]
        public string? AdditionalInfo { get; set; }
		public bool IsDone { get; set; } = false;
	}
}

