using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcNetCoreProceduresEF.Data;
using MvcNetCoreProceduresEF.Models;
using System.Data.Common;

#region PROCEDIMIENTOS ALMACENADOS

/*
 1. 

alter procedure SP_TODOS_ENFERMOS
as
	select * from ENFERMO
go

2.
alter procedure SP_FIND_ENFERMO
(@inscripcion nvarchar(50))
as 
	select * from ENFERMO where INSCRIPCION=@inscripcion
go

3.
alter procedure SP_DELETE_ENFERMO
(@inscripcion  nvarchar(50))
as
	delete from ENFERMO where INSCRIPCION=@inscripcion
go
 */


#endregion

namespace MvcNetCoreProceduresEF.Repositories
{
    public class RepositoryEnfermos
    {
        private EnfermosContext context;
        public RepositoryEnfermos(EnfermosContext context)
        {
            this.context = context;
        }

        //METODOS 

        public async Task< List<Enfermo>> GetEnfermos()
        {
            using (DbCommand com = this.context.Database.GetDbConnection().CreateCommand())
            {
                string sql = "SP_TODOS_ENFERMOS";
                com.CommandType = System.Data.CommandType.StoredProcedure;
                com.CommandText = sql;
                await com.Connection.OpenAsync();
                DbDataReader reader = await com.ExecuteReaderAsync();
                List<Enfermo> enfermos = new List<Enfermo>();
                while(await reader.ReadAsync())
                {
                    Enfermo enfermo = new Enfermo()
                    {
                        Inscripcion = reader["INSCRIPCION"].ToString(),
                        Apellido  = reader["APELLIDO"].ToString(),
                        Direccion = reader["DIRECCION"].ToString(),
                        FechaNacimiento  = DateTime.Parse(reader["FECHA_NAC"].ToString()),
                        Genero = reader["S"].ToString()
                    };
                    enfermos.Add(enfermo);
                }
                reader.CloseAsync();
                com.Connection.CloseAsync();
                return enfermos;
            }
        }

        public Enfermo GetEnfermo(string inscripcion)
        {
            string sql = "SP_FIND_ENFERMO @inscripcion";
            SqlParameter pamInscripcion = new SqlParameter("@INSCRIPCION", inscripcion);
            var consulta = this.context.Enfermos.FromSqlRaw(sql, pamInscripcion);
            Enfermo enfermo = consulta.AsEnumerable().FirstOrDefault();
            return enfermo;
        }

        public async Task DeleteEnfermoAsync(string inscripcion)
        {
            string sql = "SP_DELETE_ENFERMO";
            SqlParameter pamInscripcion = new SqlParameter("@INSCRIPCION", inscripcion);
            using (DbCommand com = this.context.Database.GetDbConnection().CreateCommand())
            {
                com.CommandType = System.Data.CommandType.StoredProcedure;
                com.CommandText = sql;
                com.Parameters.Add(pamInscripcion);
               await com.Connection.OpenAsync();
                await com.ExecuteNonQueryAsync();
                await com.Connection.CloseAsync();
                com.Parameters.Clear();
            }
        }

        public async Task DeleteEnfermoRawAsync (string inscripcion)
        {
            string sql = "SP_DELETE_ENFERMO @inscripcion";
            SqlParameter pamInscripcion = new SqlParameter("@INSCRIPCION", inscripcion);
            await this.context.Database.ExecuteSqlRawAsync(sql, pamInscripcion);
        }

        public async Task InsertEnfermoAsync(string apellido, string direccion, DateTime fechaNacimiento, string genero)
        {
            string sql = "SP_INSERT_ENFERMO";

            using (DbCommand com = this.context.Database.GetDbConnection().CreateCommand())
            {
                com.CommandType = System.Data.CommandType.StoredProcedure;
                com.CommandText = sql;

                com.Parameters.Add(new SqlParameter("@APELLIDO", apellido));
                com.Parameters.Add(new SqlParameter("@DIRECCION", direccion));
                com.Parameters.Add(new SqlParameter("@FECHA_NAC",fechaNacimiento));
                com.Parameters.Add(new SqlParameter("@S", genero));

                await com.Connection.OpenAsync();
                await com.ExecuteNonQueryAsync();
                await com.Connection.CloseAsync();
                com.Parameters.Clear();
            }
        }


        public async Task InsertEnfermoRawAsync(string apellido, string direccion, DateTime fechaNacimiento, string genero)
        {
            string sql = "EXEC SP_INSERT_ENFERMO @APELLIDO, @DIRECCION, @FECHA_NAC, @S";

            await this.context.Database.ExecuteSqlRawAsync(sql,
                new SqlParameter("@APELLIDO", apellido),
                new SqlParameter("@DIRECCION", direccion),
                new SqlParameter("@FECHA_NAC", fechaNacimiento),
                new SqlParameter("@S", genero)
            );
        }



    }
}
