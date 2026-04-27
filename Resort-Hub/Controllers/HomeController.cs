
using Microsoft.AspNetCore.Mvc;
using Resort_Hub.Interfaces;
using Resort_Hub.Models;
using Resort_Hub.Persistence.Migrations;
using Resort_Hub.ViewModels.Home;
using System.Diagnostics;


namespace Resort_Hub.Controllers;

public class HomeController(IUnitOfWork unitOfWork) : Controller
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IActionResult> Index()
    {

        var villas = await _unitOfWork.Villas.GetVillasForHmePage();

        var homeVM = new HomeVM
        {
            MostPickedVillas = villas.Take(5).ToList(),
            PopularVillas = villas,
            TotalVillas = villas.Count,
        };

        return View(homeVM);
    }
    public async Task<IActionResult> SearchVilla(
    DateOnly? checkInDate,
    int? personCount,
    string? location)
    {
        var villas = await _unitOfWork.Villas
                           .SearchVillas(checkInDate, personCount, location);

        ViewBag.CheckInDate = checkInDate?.ToString("yyyy-MM-dd");
        ViewBag.PersonCount = personCount;
        ViewBag.Location = location;
        ViewBag.HasFilters = checkInDate.HasValue ||
                             personCount.HasValue ||
                             !string.IsNullOrWhiteSpace(location);

        return View(villas);
    }
    [HttpGet]
    public async Task<IActionResult> GetLocations(string term)
    {
        var locaations = await _unitOfWork.Villas.GetLocationsAsync(term);

        return Json(locaations);
    }
    public IActionResult About()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var errorViewModel = new ErrorVM
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            ExceptionMessage = HttpContext.Items["ExceptionMessage"]?.ToString(),
            ExceptionType = HttpContext.Items["ExceptionType"]?.ToString()
        };

        return View(errorViewModel);
    }
}
