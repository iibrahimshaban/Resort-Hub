using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resort_Hub.DTOs.Booking;
using Resort_Hub.Services.Book;
using Resort_Hub.ViewModels.Booking;

namespace Resort_Hub.Controllers
{
    [Authorize]
    [Route("book")]
    public class BookController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly UserManager<ApplicationUser> _userManager;
        public BookController(IBookingService bookingService, UserManager<ApplicationUser> userManager)
        {
            _bookingService = bookingService;
            _userManager = userManager;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/Booking/{id:int}")]
        public async Task<IActionResult> Index(int id)
        {
            if (ModelState.IsValid)
            {
                //TO DO
                //MAKE IT SESSION FOR SECURITY OR DB DRAFT
                HttpContext.Session.SetInt32("draftVillaId", id);

                TempData["IsPreview"] = true;

                return RedirectToAction("BookingPreview", new { id });
            }
            else
                return RedirectToAction("Details", "Villa", new { id });
        }


        [HttpGet]
        [Route("/Booking/Checkout/{id:int}")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> BookingPreview(int id)
        {
            var data = TempData["IsPreview"] as bool?;

            if ((data is null || data == false) || (HttpContext.Session.GetInt32("draftVillaId") != id || HttpContext.Session.GetInt32("draftVillaId") is null))
            {
                return RedirectToAction("Details", "Villa", new { id });
            }

            var result = await _bookingService.GetVillaByIdWithPicsAsync(id);

            if(!result.IsSuccess)
            {
                TempData.SetError(result.Error);
                return RedirectToAction("Index", "Home");
            }

            return View(result.Value.Adapt<BookingPreviewViewModel>());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Payment(BookingPaymentViewModel bookingPaymentViewModel)
        {
            if(ModelState.IsValid)
            {
                int? sessionId = HttpContext.Session.GetInt32("draftVillaId");

                if (sessionId is not null && sessionId != bookingPaymentViewModel.Id)
                {
                    TempData.SetError(BookingErrors.IdError);
                    return RedirectToAction("Details", "Villa", new { id = sessionId });
                }

                var result = await _bookingService.IsBookOverlapping(b => b.VillaId == bookingPaymentViewModel.Id &&
                                                                               (b.Status == VillaStatus.CheckedIn || b.Status == VillaStatus.Approved) &&
                                                                               bookingPaymentViewModel.DateStart < b.CheckOutDate &&
                                                                               bookingPaymentViewModel.DateEnd >= b.CheckInDate);
                if(!result.IsSuccess)
                {
                    TempData.SetError(result.Error);
                    return RedirectToAction("Details", "Villa", new { id = bookingPaymentViewModel.Id });
                }


                var villaResult = _bookingService.GetVillaById(bookingPaymentViewModel.Id);

                if(!villaResult.IsSuccess)
                {
                    TempData.SetError(villaResult.Error);
                    return RedirectToAction("Index", "Home");
                }

                var user = await _userManager.GetUserAsync(User);
                if (user is null)
                    return RedirectToAction("Index", "Home");


                var checkExistDraft = await _bookingService.GetDraftBookingAsync(bookingPaymentViewModel.Id,user.Id);

                if (checkExistDraft.IsSuccess && checkExistDraft.Value != null)
                {

                    checkExistDraft.Value.CheckInDate = bookingPaymentViewModel.DateStart;
                    checkExistDraft.Value.CheckOutDate = bookingPaymentViewModel.DateEnd;
                    checkExistDraft.Value.TotalCost = (decimal)(checkExistDraft.Value.Nights * villaResult.Value.PricePerNight);

                    _bookingService.Update(checkExistDraft.Value);
                    await _bookingService.SaveChangesAsync();

                    HttpContext.Session.SetInt32("draftBookingId", checkExistDraft.Value.Id);
                }
                else
                {
                    var draft = new Booking
                    {
                        VillaId = bookingPaymentViewModel.Id,
                        UserId =user.Id,
                        BookingDate = DateTime.UtcNow,
                        CheckInDate = bookingPaymentViewModel.DateStart,
                        CheckOutDate = bookingPaymentViewModel.DateEnd,
                        TotalCost = (decimal)(villaResult.Value.PricePerNight * (bookingPaymentViewModel.DateEnd.ToDateTime(TimeOnly.MinValue) - bookingPaymentViewModel.DateStart.ToDateTime(TimeOnly.MinValue)).Days),
                        Status = VillaStatus.Draft
                    };

                    _bookingService.AddBooking(draft);
                    await _bookingService.SaveChangesAsync();

                    HttpContext.Session.SetInt32("draftBookingId", draft.Id);
                }


                TempData["IsPreview"] = true;

                return RedirectToAction("PaymentPreview", new { id = bookingPaymentViewModel.Id});
            }
            
            return RedirectToAction("Details", "Villa", new { id= bookingPaymentViewModel.Id });
        }


        [HttpGet]
        [Route("/Payment/{id}")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> PaymentPreview(int id)
        {
            var data = TempData["IsPreview"] as bool?;
            int? draftBookingId = HttpContext.Session.GetInt32("draftBookingId");

            if ((data is null || data == false) || (HttpContext.Session.GetInt32("draftVillaId") != id || HttpContext.Session.GetInt32("draftVillaId") is null) || draftBookingId is null)
            {
                return RedirectToAction("Details", "Villa", new { id });
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var result = await _bookingService.GetBookingAllDataByIdAsync( (int)draftBookingId, user.Id, VillaStatus.Draft);

            if (!result.IsSuccess)
            {
                TempData.SetError(result.Error);
                return RedirectToAction("Details", "Villa", new { id });
            }

            return View(result.Value.Adapt<BookingPaymentPageViewModel>());
        }


        [HttpPost("Payment/Process")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(BookingPaymentPageViewModel viewModel)
        {
            if (!ModelState.IsValid )
            {
                TempData.SetError(BookingErrors.PaymentFailed);
                return RedirectToAction("Details", "Villa", new { id = viewModel.VillaId });
            }

            int? draftBookingId = HttpContext.Session.GetInt32("draftBookingId");

            if(draftBookingId is null)
            {
                TempData.SetError(BookingErrors.NotFound);
                return RedirectToAction("Details", "Villa", new { id = viewModel.VillaId });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return RedirectToAction("Index", "Home");

            var result = await _bookingService.GetBookingAllDataByIdAsync((int)draftBookingId,user.Id , VillaStatus.Draft);

            if (!result.IsSuccess)
            {
                TempData.SetError(result.Error);
                return RedirectToAction("Index", "Home");
            }

            var overlap = await _bookingService.IsBookOverlapping(b => b.Id != result.Value.Id &&
                                                                  b.VillaId == result.Value.VillaId &&
                                                                  (b.Status == VillaStatus.CheckedIn || b.Status == VillaStatus.Approved) &&
                                                                  result.Value.CheckInDate < b.CheckOutDate &&
                                                                  result.Value.CheckOutDate > b.CheckInDate);

            if (!overlap.IsSuccess) 
            {
                TempData.SetError(overlap.Error);
                return RedirectToAction("Details", "Villa", new { id = result.Value.VillaId });
            }

            result.Value.Status = VillaStatus.Confirmed;
            result.Value.PaymentStatus = PaymentStatus.Paid;

            await _bookingService.SaveChangesAsync();

            return RedirectToAction("Success", new { id = result.Value.Id });

        }

        [HttpGet]
        [Route("/Booking/Success/{id:int}")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Success(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return RedirectToAction("Index", "Home");

            var booking = await _bookingService.CheckConfirmed(id, user.Id, VillaStatus.Confirmed);

            if (!booking.IsSuccess)
            {
                TempData.SetError(booking.Error);
                return RedirectToAction("Index", "Home");
            }

            return View();
        }


        [HttpGet("booked-dates/{villaId:int}")]
        public async Task<IActionResult> GetBookedDates(int villaId)
        {
            var result = await _bookingService.GetVillaBookedDatesAsync(villaId);

            if(!result.IsSuccess)
            {
                return Json(new List<BookedDateDTO>());
            }

            return Json(result.Value);
        }
    }
}