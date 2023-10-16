using System;
using Microsoft.EntityFrameworkCore;
using Zapp.Data;
using Zapp.Models.ViewModels;

namespace Zapp.Models.BusinessLogic
{
	public class AppointmentControllerHelper : ControllerHelper
	{
		public AppointmentControllerHelper()
		{
		}

        public override IViewModel CreateViewModel()
		{
            return new AppointmentViewModel() { Appointment = new Appointment() };
        }

        public IViewModel InitializeViewModel(
            ApplicationDbContext context, AppointmentViewModel model, bool fillCurrentTime)
		{
            model.AllCustomers = context.Customer.ToList();
            model.AllEmployees = context.Users.ToList();
            model.AllTasks = context.TaskItem.ToList();
            if (fillCurrentTime)
            {
                model.Appointment.Scheduled = GetCurrentDateTime();
            }
            return model;
        }

        public DateTime GetCurrentDateTime()
        {
            DateTime currentDateTime = DateTime.Now;
            return new DateTime(
                currentDateTime.Year,
                currentDateTime.Month,
                currentDateTime.Day,
                currentDateTime.Hour,
                currentDateTime.Minute,
                0,
                0
            );
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

