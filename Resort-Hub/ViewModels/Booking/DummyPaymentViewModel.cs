using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace Resort_Hub.ViewModels.Booking
{
    public class DummyPaymentViewModel
    {  
        [Required]
        [RegularExpression(@"^[A-Za-z\s]{3,50}$",
        ErrorMessage = "Enter valid card holder name.")]
        public string CardHolderName { get; set; }

        [Required]
        [RegularExpression(@"^\d{4}\s?\d{4}\s?\d{4}\s?\d{4}$",
        ErrorMessage = "Enter valid 16-digit card number.")]
        public string CardNumber { get; set; }

        [Required]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/(\d{2}|\d{4})$",
        ErrorMessage = "Use MM/YY or MM/YYYY.")]
        public string ValidationDate { get; set; }

        [Required]
        [RegularExpression(@"^\d{3,4}$",
        ErrorMessage = "CVV must be 3 or 4 digits.")]
        public string CVV { get; set; }
    }
}
