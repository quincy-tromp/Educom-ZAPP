using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace Zapp.Models
{
	public class CustomerTask
	{
		public int Id { get; set; }
        [Required]
        public int CustomerId { get; set; }
		public Customer Customer { get; } = null!;
        [Required]
        public int TaskId { get; set; }
		public CareTask Task { get; } = null!;
        [Column(TypeName = "varchar(256)")]
        public string? AdditionalInfo { get; set; }
	}
}

