using System;
using System.Collections.Generic;
using Zapp.Data;
using Zapp.Models.ViewModels;

namespace Zapp.Models.BusinessLogic
{
	public abstract class ControllerHelper<T> where T : ITask
	{
        public ControllerHelper()
		{
        }

        public T[] RemoveEmptyTasks(T[] tasks)
		{
            return (tasks.Where(e => e.Task.Name != null).ToArray());
        }

        public T[] RemoveDuplicateTasks(T[] tasks)
        {
            return tasks.Distinct(new TaskEqualityComparer()).ToArray();
        }

        private class TaskEqualityComparer : IEqualityComparer<T>
        {
            public bool Equals(T x, T y)
            {
                if (x == null && y == null)
                    return true;
                if (x == null || y == null)
                    return false;

                return x.Task.Name == y.Task.Name;
            }

            public int GetHashCode(T obj)
            {
                return obj.Task.Name.GetHashCode();
            }
        }
    }
}

