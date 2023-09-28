﻿using System;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;
using Zapp.Models;

namespace Zapp.Models
{
	public class AppointmentViewModel
	{
		public required Appointment Appointment { get; set; }

		public AppointmentTask[] AppointmentTasks { get; set; } = new AppointmentTask[] { new AppointmentTask() };

		public CustomerTask[] CustomerTasks { get; set; } = new CustomerTask[] { };


		public List<Customer> AllCustomers { get; set; } = new();

		public List<Employee> AllEmployees { get; set; } = new();

		public List<TaskItem> AllTasks { get; set; } = new();


		//public SelectListItem AppointmentEmployee
  //      {
		//	get
		//	{
		//		return new SelectListItem()
		//		{
		//			Text = Appointment.Employee.Name,
		//			Value = Appointment.Employee.Id.ToString()
		//		};
		//	}
		//}

		//public SelectListItem AppointmentCustomer
  //      {
		//	get
		//	{
  //              return new SelectListItem()
		//		{
		//			Text = Appointment.Customer.Name,
		//			Value = Appointment.Customer.Id.ToString()
		//		};
		//	}
		//}

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