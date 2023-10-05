using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Zapp.Data;

namespace Zapp.Models.Business
{
	public static class AppointmentHelper
	{
        /// <summary>
        /// Fills the appointment viewModel with necessary data
        /// </summary>
        /// <param name="context">The ApplicationDbContext</param>
        /// <param name="viewModel">The AppointmentViewModel to fill</param>
        /// <param name="fillCurrentTime">True to fill current local time or False if not</param>
        /// <returns>The filled AppointmentViewModel</returns>
        public static AppointmentViewModel FillViewModel(ApplicationDbContext context, AppointmentViewModel viewModel, bool fillCurrentTime)
        {
            if (fillCurrentTime)
            {
                viewModel.Appointment.Scheduled = DateTime.Today;
            }
            viewModel.AllCustomers = context.Customer.ToList();
            viewModel.AllEmployees = context.Users.ToList();
            viewModel.AllTasks = context.TaskItem.ToList();
            return viewModel;
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
    }
}

