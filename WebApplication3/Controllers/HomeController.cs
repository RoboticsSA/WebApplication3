using combit.ListLabel24.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication3.Models;
using WebReporting;

namespace WebApplication3.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View(new IndexViewModel()
            {
                // DE:  Auflisten der vorhandenen Projekte sowohl im Dateisystem als auch im Repository (hier eine SQLite-Datenbank).
                // EN:  Enumerate the existing projects both in the file system and the repository (a SQLite database in this example).
                ProjectsFromFileSystem = Directory.GetFiles(Server.MapPath("~/App_Data"), "*.lst").Select(x => Path.GetFileNameWithoutExtension(x)),
                ProjectsFromRepository = GetCurrentRepository().GetAllItems()
                                                                    // get only the project files from the repository
                                                                    .Where(item => RepositoryItemType.IsProjectType(item.Type))
                                                                    // get the project name for the UI, and the hash of the repository item Id (the ID itself is not suitable for URLs)
                                                                    .Select(item => new Tuple<int, string>(item.InternalID.GetHashCode(), item.ExtractDisplayName()))
            });
        }

        private IRepository GetCurrentRepository()
        {
            if (_repository == null)
            {
                _repository = new SQLiteFileRepository(Server.MapPath("~/App_Data/repository.db"), "en" /* language for the reports */);
            }
            return _repository;
        }
        private static IRepository _repository;

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}