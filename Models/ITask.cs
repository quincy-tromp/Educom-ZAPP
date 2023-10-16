using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zapp.Models
{
	public interface ITask 
	{
        [Required]
        public int TaskId { get; set; }
        public TaskItem Task { get; set; }

        [Column(TypeName = "varchar(256)")]
        public string? AdditionalInfo { get; set; }

        [NotMapped]
        public bool IsDeleted { get; set; }
    }
}

