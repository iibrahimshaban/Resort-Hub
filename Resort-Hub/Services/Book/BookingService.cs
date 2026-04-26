using Resort_Hub.DTOs.Booking;
using Resort_Hub.Interfaces;
using Resort_Hub.ViewModels.Booking;
using System.Linq.Expressions;

namespace Resort_Hub.Services.Book
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Booking>> GetDraftBookingAsync(int villaId, string userId)
        {
            var result = await _unitOfWork.Bookings.GetDraftBookingAsync(villaId, userId);
            
            if(result is null)
                return Result.Failure<Booking>(BookingErrors.NotFound);

            return Result.Success(result);
        }

        public async Task<Result<bool>> IsBookOverlapping(Expression<Func<Booking, bool>> predicate)
        {
            var result =  await _unitOfWork.Bookings.AnyAsync(predicate);

            if (result == true)
                return Result.Failure<bool>(BookingErrors.Conflict);
            else
                return Result.Success(false);
        }

        public async Task<Result<Booking>> GetBookingAllDataByIdAsync(int id, string userId, VillaStatus status)
        {
            var result = await _unitOfWork.Bookings.GetBookedAllDataByIdAsync(id, userId, status);

            if (result is null)
                return Result.Failure<Booking>(BookingErrors.NotFound);

            return Result.Success(result); ;
        }

        public async Task<Result<List<BookedDateDTO>>> GetVillaBookedDatesAsync(int villId)
        {
            var result = await _unitOfWork.Bookings.GetBookedDatesByVillaIdAsync(villId);

            if(result is null)
                return Result.Failure<List<BookedDateDTO>>(BookingErrors.NotFound);

            return Result.Success(result);
        }

        public Result<Villa> GetVillaById(int id)
        {
            var result = _unitOfWork.Villas.GetById(id);

            if (result is null)
                return Result.Failure<Villa>(VillaErrors.NotFound);

            return Result.Success(result);
        }

        public async Task<Result<Booking>> CheckConfirmed(int bookingId, string userID, VillaStatus villaStatus)
        {
            var result = await _unitOfWork.Bookings.GetBookedAllDataByIdAsync(bookingId, userID, villaStatus);

            if (result is null)
                return Result.Failure<Booking>(BookingErrors.Failure);

            return Result.Success(result);
        }


        public async Task<Result<Villa>> GetVillaByIdWithPicsAsync(int id)
        {
            var result = await _unitOfWork.Villas.GetVillaByIdWithPics(id);

            if (result is null)
                return Result.Failure<Villa>(VillaErrors.NotFound);

            return Result.Success(result);
        }

        public void AddBooking(Booking booking)
        {
            _unitOfWork.Bookings.Add(booking);
        }

        public async Task SaveChangesAsync()
        {
            await _unitOfWork.SaveAsync();
        }

        public void Update(Booking booking)
        {
            _unitOfWork.Bookings.Update(booking);
        }

        //private bool IsOverlapping(BookedDateDTO currentBookingDate, List<BookedDateDTO> listOfBookedDates)
        //{
        //    return listOfBookedDates.Any(b => currentBookingDate.CheckInDate < b.CheckOutDate && currentBookingDate.CheckOutDate >= b.CheckInDate);
        //}
    }
}
