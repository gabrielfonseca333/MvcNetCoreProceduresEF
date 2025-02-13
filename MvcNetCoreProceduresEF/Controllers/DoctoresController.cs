using Microsoft.AspNetCore.Mvc;
using MvcNetCoreProceduresEF.Models;
using MvcNetCoreProceduresEF.Repositories;

namespace MvcNetCoreProceduresEF.Controllers
{
    
    public class DoctoresController : Controller
    {
        RepositoryDoctores repo;
        public DoctoresController(RepositoryDoctores repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            List<Doctor> doctores = await this.repo.GetDoctoresAsync();
            ViewData["ESPECIALIDADES"] = await this.repo.GetEspecialidadesAsync();
            return View(doctores);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string especialidad, int incremento)
        {
            
            ViewData["ESPECIALIDADES"] = await this.repo.GetEspecialidadesAsync();
            List<Doctor> doctores = await this.repo.AumentarSalarioYObtenerDoctoresAsync(especialidad, incremento);
            return View(doctores);
        }
    }
}
