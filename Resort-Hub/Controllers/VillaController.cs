using Microsoft.AspNetCore.Mvc;
using Resort_Hub.Services;
using Resort_Hub.ViewModels.Villa;

namespace Resort_Hub.Controllers
{
    public class VillaController : Controller
    {
        private readonly IVillaService _villaService;

        public VillaController(IVillaService villaService)
        {
            _villaService = villaService;
        }


        public async Task<IActionResult> Details([FromRoute] int id)
        {
            var villaResult = await _villaService.GetAllVillaData(id);

            if (!villaResult.IsSuccess)
            {
                TempData.SetError(villaResult.Error);
                return RedirectToAction("Index", "Home");
            }

            return View(villaResult.Value.Adapt<VillaDetailsViewModel>());
        }
    }
}
