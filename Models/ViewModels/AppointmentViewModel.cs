using System;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;
using Zapp.Models;

namespace Zapp.Models
{
	public class AppointmentViewModel
	{
		//private List<int>? taskIds = null;

		public required Appointment Appointment { get; set; }

		public AppointmentTask[] AppointmentTasks { get; set; } = new AppointmentTask[] { new AppointmentTask() };

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
    }
}

//public class AppointmentTasks : IEnumerable<AppointmentTask>
//{
//	private List<AppointmentTask> appointmentTasks = new List<AppointmentTask>();

//	public void AddTask(AppointmentTask task)
//	{
//		appointmentTasks.Add(task);
//	}

//	public IEnumerator<AppointmentTask> GetEnumerator()
//	{
//		return appointmentTasks.GetEnumerator();
//	}

//	IEnumerator IEnumerable.GetEnumerator()
//	{
//		return GetEnumerator();
//	}
//}