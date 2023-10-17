using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Zapp.Data;
using Zapp.Models.ViewModels;

namespace Zapp.Models.BusinessLogic
{
	public class AppointmentControllerHelper : ControllerHelper<AppointmentTask>
    {
        private SystemDateTime _datetime;

		public AppointmentControllerHelper(SystemDateTime datetime)
		{
            _datetime = datetime;
		}

        public DateTime GetNow()
        {
            return _datetime.Now;
        }

        public AppointmentViewModel CreateViewModel()
		{
            return new AppointmentViewModel() { Appointment = new Appointment() };
        }

        public AppointmentViewModel InitializeViewModel(
            ApplicationDbContext context, AppointmentViewModel model)
		{
            model.AllCustomers = context.Customer.ToList();
            model.AllEmployees = context.Users.ToList();
            model.AllTasks = context.TaskItem.ToList();
            return model;
        }

        public AppointmentViewModel AddAppointmentToViewModel(
            ApplicationDbContext context, AppointmentViewModel model, int id)
        {
            var appointment = context.Appointment.Find(id);
            if (appointment == null)
            {
                throw new Exception("Something went wrong while retrieving the appointment from the database.");
            }

            model.Appointment = appointment;

            var customer = context.Customer.Find(appointment.CustomerId);
            if (customer == null)
            {
                throw new Exception("Something went wrong while retrieving the customer from the database.");
            }
            model.Appointment.Customer = customer;

            var employee = context.Users.Find(appointment.EmployeeId);
            if (employee == null)
            {
                throw new Exception("Something went wrong while retrieving the employee from the database.");
            }
            model.Appointment.Employee = employee;

            var appointmentTasks = context.AppointmentTask
                .Where(e => e.AppointmentId == appointment.Id)
                .ToList();
            if (appointmentTasks == null)
            {
                throw new Exception("Something went wrong while retrieving the appointmentTasks from the database.");
            }
            model.AppointmentTasks = appointmentTasks.ToArray();

            var customerTasks = context.CustomerTask
                .Where(e => e.CustomerId == customer.Id)
                .ToList();
            if (customerTasks != null)
            {
                model.CustomerTasks = customerTasks.ToArray();
            }

            return model;
        }
    }
}

