namespace ZenithSky.Business
{
    public class BusinessResult
    {
        public bool Success { get; private set; }
        public string? ErrorMessage { get; private set; }
        public bool IsConflict { get; private set; }
        public bool IsNotFound { get; private set; }

        public static BusinessResult Ok() => new BusinessResult { Success = true };

        public static BusinessResult BadRequest(string message) =>
            new BusinessResult { Success = false, ErrorMessage = message };

        public static BusinessResult Conflict(string message) =>
            new BusinessResult { Success = false, ErrorMessage = message, IsConflict = true };

        public static BusinessResult NotFound(string message) =>
            new BusinessResult { Success = false, ErrorMessage = message, IsNotFound = true };
    }
}
