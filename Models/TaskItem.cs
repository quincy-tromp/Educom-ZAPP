using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace Zapp.Models
{
	public class TaskItem : IBase<int>
	{
		public int Id { get; set; }
        [Required]
        [Column(TypeName = "varchar(256)")]
        public string Name { get; set; }

		public ICollection<AppointmentTask> AppointmentTasks = new List<AppointmentTask>();
		public ICollection<CustomerTask> CustomerTasks = new List<CustomerTask>();
	}
}

