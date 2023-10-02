using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Zapp.Data;

namespace Zapp.Models.Business
{
	public static class BuAppointment
	{
        public static AppointmentViewModel FillAppointmentViewModel(ApplicationDbContext context, AppointmentViewModel viewModel)
        {
            viewModel.Appointment.Scheduled = DateTime.Today;
            viewModel.AllCustomers = context.Customer.ToList();
            viewModel.AllEmployees = context.Users.ToList();
            viewModel.AllTasks = context.TaskItem.ToList();
            return viewModel;
        }

		public static bool IsAppointmentEmpty(AppointmentViewModel viewModel)
		{
            bool customerEmpty = viewModel.Appointment.CustomerId == 0;
            bool employeeEmpty = viewModel.Appointment.EmployeeId == null;
            return customerEmpty || employeeEmpty;
        }

        public static bool IsEmployeeUnavailable(ApplicationDbContext context, AppointmentViewModel vm)
        {
            return (context.Appointment
                    .Where(e => e.EmployeeId == vm.Appointment.EmployeeId)
                    .Any(e => e.Scheduled == vm.Appointment.Scheduled));
        }

        /// <summary>
        /// Filters out empty appointmentTasks
        /// </summary>
        /// <param name="appointmentTasks"></param>
        /// <returns></returns>
        public static AppointmentTask[] filterEmptyAppointmentTasks(AppointmentTask[] appointmentTasks)
        {
            return appointmentTasks.Where(e => e.Task.Name != null).ToArray();
        }

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
        /// Filters out duplicate appointmentTasks
        /// </summary>
        /// <param name="appointmentTasks"></param>
        /// <returns></returns>
        public static AppointmentTask[] filterDuplicateAppointmentTasks(AppointmentTask[] appointmentTasks)
        {
            return appointmentTasks.Distinct(new TaskEqualityComparer()).ToArray();
        }
    }
}

