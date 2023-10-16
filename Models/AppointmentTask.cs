using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace Zapp.Models
{
	public class AppointmentTask : IBase<int>, ITask
	{
		public int Id { get; set; }

        [Required]
        public int AppointmentId { get; set; }
		public Appointment Appointment { get; set; } = null!;

        [Required]
        public int TaskId { get; set; }
		public TaskItem Task { get; set; } = null!;

        [Column(TypeName = "varchar(256)")]
        public string? AdditionalInfo { get; set; }

		public bool IsDone { get; set; } = false;

		[NotMapped]
		public bool IsDeleted { get; set; } = false;
	}
}

