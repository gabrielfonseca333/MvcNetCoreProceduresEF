using Microsoft.EntityFrameworkCore;
using MvcNetCoreProceduresEF.Models;

namespace MvcNetCoreProceduresEF.Data
{
    public class DoctoresContext:DbContext
    {
        public DoctoresContext(DbContextOptions<DoctoresContext> options)
        :base(options) { }

        public DbSet<Doctor> Doctores { get; set; }
    }
}
