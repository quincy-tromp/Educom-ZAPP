using System;
using Zapp.Data;
using Zapp.Models.ViewModels;

namespace Zapp.Models.BusinessLogic
{
	public abstract class ControllerHelper
	{
		public ControllerHelper()
		{
		}

        public abstract IViewModel CreateViewModel();

        public ITask[] RemoveEmptyTasks(ITask[] tasks)
		{
            return (tasks.Where(e => e.Task.Name != null).ToArray());
        }

        public static ITask[] RemoveDuplicateTasks(ITask[] tasks)
        {
            return tasks.Distinct(new TaskEqualityComparer()).ToArray();
        }

        private class TaskEqualityComparer : IEqualityComparer<ITask>
        {
            public bool Equals(ITask x, ITask y)
            {
                if (x == null && y == null)
                    return true;
                if (x == null || y == null)
                    return false;

                return x.Task.Name == y.Task.Name;
            }

            public int GetHashCode(ITask obj)
            {
                return obj.Task.Name.GetHashCode();
            }
        }
    }
}

