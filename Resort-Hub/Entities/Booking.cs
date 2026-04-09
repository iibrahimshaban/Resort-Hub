using Resort_Hub.Abstraction;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Resort_Hub.Entities;

public class Booking
{
    public int Id { get; set; }

    public DateTime BookingDate { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public int Nights
    {
        get
        {
            return (CheckOutDate.ToDateTime(TimeOnly.MinValue) - CheckInDate.ToDateTime(TimeOnly.MinValue)).Days;
        }
    }

    public decimal TotalCost { get; set; }
    public VillaStatus Status { get; set; } 

    public int VillaId { get; set; }
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser User { get; set; } = default!;
    public Villa Villa { get; set; } = default!;

}
