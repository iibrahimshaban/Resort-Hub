namespace Resort_Hub.Errors
{
    public static class BookingErrors
    {
        public static readonly Error Conflict =
            new("Booking Date Conflict", "The specified date is conflicted with another", ErrorType.Conflict);

        public static readonly Error PaymentFailed =
            new("Payment has failed", "Payment process has been declined", ErrorType.Failure);

        public static readonly Error IdError =
            new("Error 404", "Error has occured", ErrorType.NotFound);

        public static readonly Error NotFound =
            new("Booking not found", "Booking does not exist", ErrorType.NotFound);

        public static readonly Error Failure =
            new("Failed", "Something went wrong", ErrorType.Failure);

    }
}
