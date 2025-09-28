namespace Compago.API.ExceptionHandling
{
    public class ErrorDTO
    {
        public string? Type { get; set; }

        public string? Title { get; set; }

        public int? Status { get; set; }

        public List<string> Details { get; set; } = [];
    }
}
