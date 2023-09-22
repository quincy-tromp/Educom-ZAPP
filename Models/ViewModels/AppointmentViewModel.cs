using System;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace Zapp.Models
{
	public class AppointmentViewModel
	{
		public required Appointment Appointment { get; set; }

		public List<Customer> AllCustomers { get; set; } = new();

		public List<Employee> AllEmployees { get; set; } = new();

		public List<SelectListItem> AllCustomersListItems
		{
			get
			{
				return AllCustomers.Select(e => new SelectListItem
				{
					Text = e.Name,
					Value = e.Id.ToString()
				}
				).ToList();
			}
		}

		public List<SelectListItem> AllEmployeesListItems
		{
			get
			{
				return AllEmployees.Select(e => new SelectListItem
				{
					Text = e.Name,
					Value = e.Id.ToString()
				}
				).ToList();
			}
		}
	}
}

