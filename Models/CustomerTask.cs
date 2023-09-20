using System;
namespace Zapp.Models
{
	public class CustomerTask
	{
		public int Id { get; set; }
		public int CustomerId { get; set; }
		public int TaskId { get; set; }
		public string? AdditionalInfo { get; set; }
	}
}

