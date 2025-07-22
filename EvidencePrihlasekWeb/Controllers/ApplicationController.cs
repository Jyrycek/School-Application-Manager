using EvidencePrihlasekWeb.Models;
using EvidencePrihlasekWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EvidencePrihlasekWeb.Controllers
{
    [Authorize]
    public class ApplicationController : Controller
    {
        private readonly CreateApplicationService _createApplicationService;

        public ApplicationController(CreateApplicationService createApplicationService)
        {
            _createApplicationService = createApplicationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPrograms(int schoolId)
        {
            var programs = await _createApplicationService.GetPrograms(schoolId);
			ViewBag.Programs = new SelectList(programs, nameof(StudyProgram.Id), nameof(StudyProgram.Name));
			
            return Json(programs);
        }
        public async Task<IActionResult> CreateApplication()
        {
            if (User.Identity?.Name == null)
            {
                return RedirectToAction("Dashboard", "Dashboard");
            }
            string username = User.Identity.Name;
            bool alreadyCreated = await _createApplicationService.CheckIfApplicationExists(username);

            if (alreadyCreated)
            {
                return RedirectToAction("Dashboard", "Dashboard");
            }

            var schools = await _createApplicationService.GetSchools();
			ViewBag.Schools = new SelectList(schools, nameof(HighSchool.Id), nameof(HighSchool.Name));

			return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewApplication(CreateApplicationModel model)
        {
            if (!ModelState.IsValid) return View("CreateApplication");

			if (model.StudyProgramId1 == model.StudyProgramId2 && model.StudyProgramId1 != null 
                || model.StudyProgramId1 == model.StudyProgramId3 && model.StudyProgramId1 != null
                || model.StudyProgramId2 == model.StudyProgramId3 && model.StudyProgramId2 != null)
			{
                var schools = await _createApplicationService.GetSchools();

                ViewBag.Schools = new SelectList(schools, nameof(HighSchool.Id), nameof(HighSchool.Name));

                ModelState.AddModelError("StudyProgramId2", "Nemůžete vybrat stejný studijní obor vícekrát.");
				return View("CreateApplication");
			}

            if (User.Identity?.Name == null)
            {
                return RedirectToAction("Dashboard", "Dashboard");
            }

            await _createApplicationService.CreateApplication(model, User.Identity.Name);

			return RedirectToAction("Dashboard", "Dashboard");
        }

    }
}
