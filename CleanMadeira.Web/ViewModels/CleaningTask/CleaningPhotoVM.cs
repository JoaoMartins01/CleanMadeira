namespace CleanMadeira.Web.ViewModels.CleaningTask
{
    public class CleaningPhotoVM
    {
        public Guid Id { get; set; }

        public string FileUrl { get; set; } = string.Empty;

        public string? FileName { get; set; }
    }
}
