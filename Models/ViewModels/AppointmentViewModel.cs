using System;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;
using Zapp.Models;
using Zapp.Models.ViewModels;

namespace Zapp.Models
{
	public class AppointmentViewModel : IViewModel
	{
		public required Appointment Appointment { get; set; }

		public AppointmentTask[] AppointmentTasks { get; set; } = new AppointmentTask[] { new AppointmentTask() };

		public CustomerTask[] CustomerTasks { get; set; } = new CustomerTask[] { };

		public AppointmentTask[] DeletedTasks { get; set; } = null!;


		public List<Customer> AllCustomers { get; set; } = new();

		public List<Employee> AllEmployees { get; set; } = new();

		public List<TaskItem> AllTasks { get; set; } = new();

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
		public List<SelectListItem> AllTasksListItems
		{
			get
			{
				return AllTasks.Select(e => new SelectListItem
				{
					Text = e.Name,
					Value = e.Id.ToString()
				}
				).ToList();
			}
		}
		public List<string> AllTaskNames
		{
			get
			{
				return AllTasks.Select(e => e.Name).ToList();
			}
		}
    }
}