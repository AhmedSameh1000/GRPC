using GrpcService1.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcService1.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ToDoItem> toDoItems => Set<ToDoItem>();
    }
}