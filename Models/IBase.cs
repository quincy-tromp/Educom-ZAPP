using System;
namespace Zapp.Models
{
	public interface IBase <T>
	{
		public T Id { get; set; }
	}
}

