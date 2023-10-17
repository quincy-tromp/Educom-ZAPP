using System;
namespace Zapp.Models.BusinessLogic
{
	public class SystemDateTime : IDateTime
	{
		public SystemDateTime()
		{
		}

		public DateTime Now
		{
			get
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
		}
	}
}

