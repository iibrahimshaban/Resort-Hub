namespace Resort_Hub.Abstraction
{
    public record ModalMessage(string Title, string Message, ModalType Type)
    {
        public static readonly ModalMessage None = new(string.Empty, string.Empty, ModalType.None);
    }
}
