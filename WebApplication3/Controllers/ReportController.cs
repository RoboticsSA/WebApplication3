using combit.ListLabel24.AdhocDesign;
using combit.ListLabel24.AdhocDesign.SessionHandling;
using combit.ListLabel24.AdhocDesign.Web;
using combit.ListLabel24.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using WebReporting;

namespace WebApplication3.Controllers
{
    public class ReportController : Controller
    {
        // GET api/<controller>
        public ActionResult Create(string projectType, string reportTitle, bool repositoryMode)
        {
            var options = GetDesignerOptions(reportTitle, projectType);

            
            // EN:  Create a new Ad-hoc Designer session. The Ad-hoc Designer supports loading projects from any source.
            //      For project files in the file system or in a repository (see IRepository) you may use the integrated methods:

            IAdhocDesignerSession designerSession;
            if (repositoryMode)
            {
                
                // EN:  To create a new item in the repository, null must be passed as the item ID.
                string repositoryItemId = null;
                designerSession = AdhocDesignerSession.FromRepositoryItem(repositoryItemId, GetCurrentRepository(), options);

                
                // EN:  After the session was created, we can get the ID of the new repository item:
                repositoryItemId = (designerSession as AdhocDesignerSessionRepositoryBased).RepositoryItemId;
            }
            else
            {
                
                // EN: For a project file that is saved in the file system we only need to pass the file path. If the file does not exist yet it will be created.
                designerSession = AdhocDesignerSession.FromFile(GetProjectFilePathFromName(reportTitle), options);
            }

            
            // EN:  The designer is opened by redirecting to a special (internal) URL.
            return this.RedirectToAdhocDesigner(designerSession);
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

        private AdhocWebDesignerSessionOptions GetDesignerOptions(string reportTitle, string projectType)
        {
            return new AdhocWebDesignerSessionOptions()
            {

                // EN:  You can find more information on the available options in the online help of List & Label.

                DataSource = ExampleDatasources.GetOleDbProviderNorthwindDb(),
                // ListLabelLicensingInfo = "<insert your license key here>",

                DefaultProjectTitle = reportTitle,
                DesignerTitle = reportTitle + " [Design]",
                DefaultProjectType = projectType == null ? AdhocProjectType.SimpleTable : (AdhocProjectType)Enum.Parse(typeof(AdhocProjectType), projectType),
                Language = new System.Globalization.CultureInfo("en"),

                // ...
            };
        }
        private string GetProjectFilePathFromName(string projectName)
        {
            return Path.Combine(Server.MapPath("~/App_Data"), Path.GetFileNameWithoutExtension(projectName) + ".lst");
        }
    
    public ActionResult OpenDesignerForFile(string projectName)
    {
        // DE:  Für Beschreibungen der Befehle siehe CreateNewProject().
        // EN:  For descriptions of the operations see CreateNewProject().
        var options = GetDesignerOptions(projectName, null);
        var designerSession = AdhocDesignerSession.FromFile(GetProjectFilePathFromName(projectName), options);
        return this.RedirectToAdhocDesigner(designerSession);
    }


    // DE:  Diese Aktion öffnet ein Projekt, das in einem Repository gespeichert ist (vgl. IRepository).
    // EN:  This action opens a project which is saved in a repository (see IRepository).
    public ActionResult OpenDesignerForRepositoryItem(int id)
    {
        // DE:  Ermitteln des zu öffnenden Projekts. Siehe Index(): Um gültige URLs zu erhalten wurde statt der ID des Repository-Elements dessen Hash-Code verwendet.
        // EN:  Get the project to open. See Index(): To get valid URL the item ID of the repository item was replaced with it's hash code.
        RepositoryItem projectToOpen = GetCurrentRepository().GetAllItems().First(item => item.InternalID.GetHashCode() == id);

        // DE:  Für Beschreibungen der Befehle siehe CreateNewProject().
        // EN:  For descriptions of the operations see CreateNewProject().
        var options = GetDesignerOptions(projectToOpen.ExtractDisplayName(), null);
        var designerSession = AdhocDesignerSession.FromRepositoryItem(projectToOpen.InternalID, GetCurrentRepository(), options);
        return this.RedirectToAdhocDesigner(designerSession);
    }              


   

}


}
