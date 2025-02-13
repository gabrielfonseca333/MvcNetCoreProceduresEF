using Microsoft.AspNetCore.Mvc;
using MvcNetCoreProceduresEF.Models;
using MvcNetCoreProceduresEF.Repositories;

namespace MvcNetCoreProceduresEF.Controllers
{
    public class EnfermosController : Controller
    {
        RepositoryEnfermos repo;

        public EnfermosController(RepositoryEnfermos repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            List<Enfermo> enfermos = await this.repo.GetEnfermos();
            return View(enfermos);
        }

        public IActionResult Details(string inscripcion)
        {
            Enfermo enfermo = this.repo.GetEnfermo(inscripcion);
            return View(enfermo);
        }

        public async Task<IActionResult> Delete(string inscripcion)
        {
            await this.repo.DeleteEnfermoRawAsync(inscripcion);
            return RedirectToAction("Index");
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Enfermo enfermo)
        {
            await this.repo.InsertEnfermoRawAsync(
                enfermo.Apellido,
                enfermo.Direccion,
                enfermo.FechaNacimiento,
                enfermo.Genero
                );
            return RedirectToAction("Index");
        }
    }
}
