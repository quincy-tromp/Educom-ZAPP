using System;
namespace Zapp.Models
{
	public class Customer
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }
		public string PostalCode { get; set; }
		public string Residence { get; set; }
		public ICollection<Appointment> Appointments { get; } = new List<Appointment>();
		public ICollection<CustomerTask> CustomerTasks { get; } = new List<CustomerTask>();
	}
}

