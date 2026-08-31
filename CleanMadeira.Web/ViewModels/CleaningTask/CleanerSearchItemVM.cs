namespace CleanMadeira.Web.ViewModels.CleaningTask
{
    public class CleanerSearchItemVM
    {
        public Guid Id { get; set; }

        public string LimpadorCodigo { get; set; } = string.Empty;

        public int? LimpadorNumero { get; set; }

        public string NomeCompleto { get; set; } = string.Empty;

        public string? CompanyName { get; set; }

        public Guid? CompanyId { get; set; }

        public string Email { get; set; } = string.Empty;

        public string? Telemovel { get; set; }

        public bool Active { get; set; }
    }
}
