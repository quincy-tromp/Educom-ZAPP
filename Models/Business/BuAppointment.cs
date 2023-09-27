using System;
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
            return customerEmpty && employeeEmpty && IsAppointmentTasksEmpty(viewModel.AppointmentTasks);
        }

        public static bool IsAppointmentTasksEmpty(AppointmentTask[] appointmentTasks)
        {
            foreach (var appointmentTask in appointmentTasks)
            {
                if (appointmentTask.Task.Name != null)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsEmployeeUnavailable(ApplicationDbContext context, AppointmentViewModel vm)
        {
            return (context.Appointment
                    .Where(e => e.EmployeeId == vm.Appointment.EmployeeId)
                    .Any(e => e.Scheduled == vm.Appointment.Scheduled));
        }
	}
}

