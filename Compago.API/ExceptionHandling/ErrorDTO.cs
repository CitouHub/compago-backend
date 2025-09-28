namespace Compago.API.ExceptionHandling
{
    public class ErrorDTO
    {
        public string? Type { get; set; }

        public string? Title { get; set; }

        public int? Status { get; set; }

        public object Errors { get; set; } = new object();
    }
}
