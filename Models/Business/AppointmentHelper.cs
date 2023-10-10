using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Zapp.Data;

namespace Zapp.Models.Business
{
	public static class AppointmentHelper
	{
        /// <summary>
        /// Creates a new AppointmentViewModel with empty appointment
        /// </summary>
        /// <returns>The new AppointmentViewModel instance</returns>
        public static AppointmentViewModel CreateNewViewModel()
        {
            return new AppointmentViewModel() { Appointment = new Appointment() };
        }

        /// <summary>
        /// Fills the appointment viewModel with the necessary data
        /// </summary>
        /// <param name="context">The ApplicationDbContext</param>
        /// <param name="model">The AppointmentViewModel to fill</param>
        /// <param name="fillCurrentTime">True to fill current local time or False if not</param>
        /// <returns>The filled AppointmentViewModel</returns>
        public static AppointmentViewModel InitializeViewModel(ApplicationDbContext context, AppointmentViewModel model, bool fillCurrentTime)
        {
            if (fillCurrentTime)
            {
                model.Appointment.Scheduled = GetCurrentDateTime();
            }
            model.AllCustomers = context.Customer.ToList();
            model.AllEmployees = context.Users.ToList();
            model.AllTasks = context.TaskItem.ToList();
            return model;
        }

        /// <summary>
        /// Gets the current datetime excluding seconds and milliseconds
        /// </summary>
        /// <returns>The current datetime excluding seconds and milliseconds</returns>
        public static DateTime GetCurrentDateTime()
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

        /// <summary>
        /// Checks if employee has another appointment scheduled for the selected datetime
        /// </summary>
        /// <param name="context"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        public static bool IsEmployeeUnavailable(ApplicationDbContext context, AppointmentViewModel vm)
        {
            return (context.Appointment
                    .Where(e => e.EmployeeId == vm.Appointment.EmployeeId)
                    .Any(e => e.Scheduled == vm.Appointment.Scheduled));
        }

        /// <summary>
        /// Removes empty appointment tasks
        /// </summary>
        /// <param name="appointmentTasks">The array of appointment tasks (with empty tasks)</param>
        /// <returns>The array of appointment tasks without empty tasks</returns>
        public static AppointmentTask[] removeEmptyAppointmentTasks(AppointmentTask[] appointmentTasks)
        {
            return appointmentTasks.Where(e => e.Task.Name != null).ToArray();
        }

        /// <summary>
        /// Compares appointment tasks
        /// </summary>
        private class TaskEqualityComparer : IEqualityComparer<AppointmentTask>
        {
            public bool Equals(AppointmentTask x, AppointmentTask y)
            {
                if (x == null && y == null)
                    return true;
                if (x == null || y == null)
                    return false;

                return x.Task.Name == y.Task.Name;
            }

            public int GetHashCode(AppointmentTask obj)
            {
                return obj.Task.Name.GetHashCode();
            }
        }

        /// <summary>
        /// Removes duplicate appointment tasks
        /// </summary>
        /// <param name="appointmentTasks">The array of appointment tasks (with duplicate tasks)</param>
        /// <returns>The array of appointment tasks without duplicate tasks</returns>
        public static AppointmentTask[] removeDuplicateAppointmentTasks(AppointmentTask[] appointmentTasks)
        {
            return appointmentTasks.Distinct(new TaskEqualityComparer()).ToArray();
        }

        /// <summary>
        /// Adds an appointment, including appointment tasks and customer tasks, to a given AppointmentViewModel
        /// </summary>
        /// <param name="context">The ApplicationDbContext</param>
        /// <param name="model">The AppointmentViewModel to add the appointment to</param>
        /// <param name="id">The Id of the appointment</param>
        /// <returns>The up-to-date AppointmentViewModel</returns>
        /// <exception cref="Exception">A custom generic exception</exception>
        public static AppointmentViewModel AddAppointmentToViewModel(ApplicationDbContext context, AppointmentViewModel model, int id)
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

