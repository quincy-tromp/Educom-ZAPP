using System;
using Microsoft.EntityFrameworkCore;
using Zapp.Data;

namespace Zapp.Models
{
	public static class SeedData
	{
		public static void Initialize(IServiceProvider serviceProvider)
		{
			using (var context = new ApplicationDbContext())
			{
				if (context.CareTask.Any())
				{
					return;
				}
				context.CareTask.AddRange(
					new CareTask
					{
						Name = "Koffie zetten"
					},
					new CareTask
					{
						Name = "Medicatie toedienen"
					},
					new CareTask
					{
						Name = "Afwassen"
					},
					new CareTask
					{
						Name = "Stofzuigen"
					},
					new CareTask
					{
						Name = "Dweilen"
					},
					new CareTask
					{
						Name = "Steunkousen aantrekken"
					}
				);
				if (context.Customer.Any())
				{
					return;
				}
				context.Customer.AddRange(
					new Customer
					{
						Name = "Mevrouw Pietersen",
						Address = "Straatweg 19",
						PostalCode = "6556 AB",
						Residence = "Ergenshuizen"
					},
					new Customer
					{
						Name = "John Bravo",
						Address = "Korte Veerstraat 1",
						PostalCode = "2011 CL",
						Residence = "Haarlem"
					},
					new Customer
					{
						Name = "Karel Edelgast",
						Address = "Grote Kerkstraat 46",
						PostalCode = "7902 CK",
						Residence = "Hoogeveen"
                    },
					new Customer
					{
						Name = "Jules Keizer",
						Address = "Koninginneweg 101",
						PostalCode = "1211 AP",
						Residence = "Hilversum"
                    },
					new Customer
					{
						Name = "Mika Martens",
						Address = "Akerkhof 2",
						PostalCode = "9711 JB",
						Residence = "Groningen"
					}
				);
				context.SaveChanges();
			}
		}
	}
}

