using Microsoft.EntityFrameworkCore;
using MvcNetCoreProceduresEF.Data;
using MvcNetCoreProceduresEF.Models;

namespace MvcNetCoreProceduresEF.Repositories
{
    public class RepositoryEmpleados
    {
        private HospitalContext context;
        public RepositoryEmpleados(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<List<VistaEmpleado>> GetVistaEmpledosAsync()
        {
            var consulta = from datos in this.context.VistaEmpleado
                           select datos;
            return await consulta.ToListAsync();
        }
    }
}
