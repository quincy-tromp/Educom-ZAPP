using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace Zapp.Models
{
	public class Customer : IBase<int>
	{
		public int Id { get; set; }
		[Required]
		[Column(TypeName = "varchar(256)")]
		public string Name { get; set; }
        [Required]
        [Column(TypeName = "varchar(256)")]
        public string Address { get; set; }
        [Required]
        [Column(TypeName = "char(7)")]
        public string PostalCode { get; set; }
        [Required]
        [Column(TypeName = "varchar(128)")]
        public string Residence { get; set; }

		public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
		public ICollection<CustomerTask> CustomerTasks { get; set; } = new List<CustomerTask>();
	}
}

