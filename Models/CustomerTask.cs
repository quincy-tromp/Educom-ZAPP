using System;
namespace Zapp.Models
{
	public class CustomerTask
	{
		public int Id { get; set; }
		public int CustomerId { get; set; }
		public Customer Customer { get; } = null!;
		public int TaskId { get; set; }
		public CareTask Task { get; } = null!;
		public string? AdditionalInfo { get; set; }
	}
}

