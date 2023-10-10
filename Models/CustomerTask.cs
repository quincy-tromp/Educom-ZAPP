using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace Zapp.Models
{
	public class CustomerTask : IBase<int>
	{
		public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }
		public Customer Customer { get; set; } = null!;

        [Required]
        public int TaskId { get; set; }
		public TaskItem Task { get; set; } = null!;

        [Column(TypeName = "varchar(256)")]
        public string? AdditionalInfo { get; set; }

        [NotMapped]
        public bool IsDeleted { get; set; } = false;
    }
}

