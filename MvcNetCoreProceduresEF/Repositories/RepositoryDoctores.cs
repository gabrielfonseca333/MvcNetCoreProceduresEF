using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcNetCoreProceduresEF.Data;
using MvcNetCoreProceduresEF.Models;
using System.Data.Common;

#region STORED PROCEDURES
/*
1.
CREATE PROCEDURE SP_TODOS_DOCTORES
AS
BEGIN
    SELECT * FROM DOCTOR;
END;
GO

2.
CREATE PROCEDURE SP_ESPECIALIDADES
AS
BEGIN
    SELECT DISTINCT ESPECIALIDAD FROM DOCTOR;
END;
GO

3. aumentar salario
CREATE PROCEDURE SP_AUMENTAR_SALARIO_ESPECIALIDAD
(
    @especialidad NVARCHAR(50),  
    @incremento INT              
)
AS
BEGIN
    UPDATE DOCTOR
    SET SALARIO = SALARIO + @incremento
    WHERE ESPECIALIDAD = @especialidad;
END;
GO

4. 
CREATE PROCEDURE SP_DOCTOR_ESPECIALIDAD
(
    @especialidad NVARCHAR(50)  
)
AS
BEGIN
    SELECT * FROM DOCTOR WHERE ESPECIALIDAD = @especialidad;
END;
GO
 */
#endregion

namespace MvcNetCoreProceduresEF.Repositories
{
    public class RepositoryDoctores
    {
        private DoctoresContext context;
        public RepositoryDoctores(DoctoresContext context)
        {
            this.context = context;
        }

        //METODOS
        public async Task<List<Doctor>> GetDoctoresAsync()
        {
            using (DbCommand com = this.context.Database.GetDbConnection().CreateCommand())
            {
                string sql = "SP_TODOS_DOCTORES";
                com.CommandType = System.Data.CommandType.StoredProcedure;
                com.CommandText = sql;
                await com.Connection.OpenAsync();
                DbDataReader reader = await com.ExecuteReaderAsync();
                List<Doctor> doctores = new List<Doctor>();
                while (await reader.ReadAsync())
                {
                    Doctor doctor = new Doctor()
                    {
                        IdHospital = Convert.ToInt32(reader["HOSPITAL_COD"]),
                        IdDoctor = Convert.ToInt32(reader["DOCTOR_NO"]),
                        Apellido = reader["APELLIDO"].ToString(),
                        Especialidad = reader["ESPECIALIDAD"].ToString(),
                        Salario = Convert.ToInt32(reader["SALARIO"])
                    };
                    doctores.Add(doctor);
                }
                await reader.CloseAsync();
                await com.Connection.CloseAsync();
                return doctores;
            }
        }

        public async Task<List<string>> GetEspecialidadesAsync()
        {
            using (DbCommand com = this.context.Database.GetDbConnection().CreateCommand())
            {
                string sql = "SP_ESPECIALIDADES";
                com.CommandType = System.Data.CommandType.StoredProcedure;
                com.CommandText = sql;
                await com.Connection.OpenAsync();
                DbDataReader reader = await com.ExecuteReaderAsync();
                List<string> especialidades = new List<string>();
                while (await reader.ReadAsync())
                {
                    especialidades.Add(reader["ESPECIALIDAD"].ToString());
                }
                await reader.CloseAsync();
                await com.Connection.CloseAsync();
                return especialidades;
            }
        }

        public async Task AumentarSalarioPorEspecialidad(string especialidad, int incremento)
        {
            using (DbCommand com = this.context.Database.GetDbConnection().CreateCommand())
            {
                string sql = "SP_AUMENTAR_SALARIO_ESPECIALIDAD";
                com.CommandType = System.Data.CommandType.StoredProcedure;
                com.CommandText = sql;

                // Parámetro para la especialidad
                DbParameter paramEspecialidad = com.CreateParameter();
                paramEspecialidad.ParameterName = "@especialidad";
                paramEspecialidad.DbType = System.Data.DbType.String;
                paramEspecialidad.Value = especialidad;
                com.Parameters.Add(paramEspecialidad);

                // Parámetro para el incremento de salario
                DbParameter paramIncremento = com.CreateParameter();
                paramIncremento.ParameterName = "@incremento";
                paramIncremento.DbType = System.Data.DbType.Int32;
                paramIncremento.Value = incremento;
                com.Parameters.Add(paramIncremento);

                await com.Connection.OpenAsync();
                await com.ExecuteNonQueryAsync(); // Ejecutar la actualización
                await com.Connection.CloseAsync();
            }
        }

        public async Task<List<Doctor>> GetDoctoresPorEspecialidadAsync(string especialidad)
        {
            using (DbCommand com = this.context.Database.GetDbConnection().CreateCommand())
            {
                string sql = "SP_DOCTOR_ESPECIALIDAD";
                com.CommandType = System.Data.CommandType.StoredProcedure;
                com.CommandText = sql;

                // Parámetro para la especialidad
                DbParameter paramEspecialidad = com.CreateParameter();
                paramEspecialidad.ParameterName = "@especialidad";
                paramEspecialidad.DbType = System.Data.DbType.String;
                paramEspecialidad.Value = especialidad;
                com.Parameters.Add(paramEspecialidad);

                await com.Connection.OpenAsync();
                DbDataReader reader = await com.ExecuteReaderAsync();
                List<Doctor> doctores = new List<Doctor>();
                while (await reader.ReadAsync())
                {
                    Doctor doctor = new Doctor()
                    {
                        IdHospital = Convert.ToInt32(reader["HOSPITAL_COD"]),
                        IdDoctor = Convert.ToInt32(reader["DOCTOR_NO"]),
                        Apellido = reader["APELLIDO"].ToString(),
                        Especialidad = reader["ESPECIALIDAD"].ToString(),
                        Salario = Convert.ToInt32(reader["SALARIO"])
                    };
                    doctores.Add(doctor);
                }
                await reader.CloseAsync();
                await com.Connection.CloseAsync();
                return doctores;
            }
        }

        public async Task<List<Doctor>> AumentarSalarioYObtenerDoctoresAsync(string especialidad, int incremento)
        {
            using (DbConnection connection = this.context.Database.GetDbConnection())
            {
                await connection.OpenAsync();

                // 1. Aumentar el salario
                using (DbCommand com = connection.CreateCommand())
                {
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.CommandText = "SP_AUMENTAR_SALARIO_ESPECIALIDAD";

                    com.Parameters.Add(new SqlParameter("@especialidad", especialidad));
                    com.Parameters.Add(new SqlParameter("@incremento", incremento));

                    await com.ExecuteNonQueryAsync();
                }

                // 2. Obtener los doctores de la especialidad
                using (DbCommand com = connection.CreateCommand())
                {
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.CommandText = "SP_DOCTOR_ESPECIALIDAD";

                    com.Parameters.Add(new SqlParameter("@especialidad", especialidad));

                    DbDataReader reader = await com.ExecuteReaderAsync();
                    List<Doctor> doctores = new List<Doctor>();

                    while (await reader.ReadAsync())
                    {
                        Doctor doctor = new Doctor()
                        {
                            IdHospital = Convert.ToInt32(reader["HOSPITAL_COD"]),
                            IdDoctor = Convert.ToInt32(reader["DOCTOR_NO"]),
                            Apellido = reader["APELLIDO"].ToString(),
                            Especialidad = reader["ESPECIALIDAD"].ToString(),
                            Salario = Convert.ToInt32(reader["SALARIO"])
                        };
                        doctores.Add(doctor);
                    }

                    await reader.CloseAsync();
                    return doctores;
                }
            }
        }








    }
}
