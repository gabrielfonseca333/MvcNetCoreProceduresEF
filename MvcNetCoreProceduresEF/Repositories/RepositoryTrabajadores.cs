using Microsoft.EntityFrameworkCore;
using MvcNetCoreProceduresEF.Data;
using MvcNetCoreProceduresEF.Models;

namespace MvcNetCoreProceduresEF.Repositories
{
    public class RepositoryTrabajadores
    {
        private HospitalContext context;
        public RepositoryTrabajadores(HospitalContext context)
        {
            this.context = context;
        }

        //METODOS
        public async Task<TrabajadoresModel> GetTrabajadoresAsync()
        {
            var consulta = from datos in this.context.Trabajadores
                           select datos;
            
            TrabajadoresModel model = new TrabajadoresModel();
            model.Trabajadores = await consulta.ToListAsync();
            model.Personas = await consulta.CountAsync();
            model.SumaSalarial = await consulta.SumAsync(x => x.Salario);
            model.MediaSalarial = (int) await consulta.AverageAsync(z => z.Salario);
            return model;
        }

        public async Task<List<string>> GetOficiosAsync()
        {
            var consulta = (from datos in this.context.Trabajadores
                            select datos.Oficio).Distinct();
            return await consulta.ToListAsync();
        }

        //ayudame a acer un Trabajador model por oficio
        public async Task<TrabajadoresModel> GetTrabajadoresOficioAsync(string oficio)
        {
            var consulta = from datos in this.context.Trabajadores
                           where datos.Oficio == oficio
                           select datos;
            TrabajadoresModel model = new TrabajadoresModel();
            model.Trabajadores = await consulta.ToListAsync();
            model.Personas = await consulta.CountAsync();
            model.SumaSalarial = await consulta.SumAsync(x => x.Salario);
            model.MediaSalarial = (int)await consulta.AverageAsync(z => z.Salario);
            return model;
        }


    }
}
