using System;
namespace Zapp.Models.ViewModels
{
	public class CustomerViewModel
	{
        public required Customer Customer { get; set; }

        public CustomerTask[] CustomerTasks { get; set; } = new CustomerTask[] { new CustomerTask() };

        public List<TaskItem> AllTasks { get; set; } = new();

        public List<string> AllTaskNames
        {
            get
            {
                return AllTasks.Select(e => e.Name).ToList();
            }
        }
    }
}

