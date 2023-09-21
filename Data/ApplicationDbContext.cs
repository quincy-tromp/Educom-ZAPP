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

    //protected override void OnModelCreating(ModelBuilder builder)
    //{
    //    base.OnModelCreating(builder);
    //}
}

