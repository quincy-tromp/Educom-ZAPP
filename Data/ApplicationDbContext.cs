using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Zapp.Data;

public class ApplicationDbContext : IdentityDbContext
{
    private readonly string connectionString = "Server=localhost;Database=Zapp;Uid=ZappUser;Pwd=xhXNl)Lel)FKRT7];";

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }
}

