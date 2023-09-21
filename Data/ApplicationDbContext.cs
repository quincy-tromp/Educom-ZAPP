using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Zapp.Models;

namespace Zapp.Data;

public class ApplicationDbContext : IdentityDbContext<Employee, IdentityRole, string>
{
    private readonly string connectionString = "Server=localhost;Database=Zapp;Uid=ZappUser;Pwd=xhXNl)Lel)FKRT7];";

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>()
            .HasMany(e => e.Appointments)
            .WithOne(e => e.Employee)
            .HasForeignKey(e => e.EmployeeId)
            .IsRequired();

        modelBuilder.Entity<Customer>()
            .HasMany(e => e.Appointments)
            .WithOne(e => e.Customer)
            .HasForeignKey(e => e.CustomerId)
            .IsRequired();

        modelBuilder.Entity<Customer>()
            .HasMany(e => e.CustomerTasks)
            .WithOne(e => e.Customer)
            .HasForeignKey(e => e.CustomerId)
            .IsRequired();

        modelBuilder.Entity<CareTask>()
            .HasMany(e => e.CustomerTasks)
            .WithOne(e => e.Task)
            .HasForeignKey(e => e.TaskId)
            .IsRequired();

        modelBuilder.Entity<CareTask>()
            .HasMany(e => e.AppointmentTasks)
            .WithOne(e => e.Task)
            .HasForeignKey(e => e.TaskId)
            .IsRequired();

        modelBuilder.Entity<Appointment>()
            .HasMany(e => e.AppointmentTasks)
            .WithOne(e => e.Appointment)
            .HasForeignKey(e => e.AppointmentId)
            .IsRequired();
    }
}

