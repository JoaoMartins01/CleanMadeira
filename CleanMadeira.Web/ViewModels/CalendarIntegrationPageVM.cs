namespace CleanMadeira.Web.ViewModels
{
    public class CalendarIntegrationsPageVM
    {
        public Guid PropriedadeId { get; set; }

        public string PropriedadeNome { get; set; } = string.Empty;

        public CalendarIntegrationVM NovaIntegracao { get; set; }
            = new();

        public List<CalendarIntegrationVM> Integracoes { get; set; }
            = new();
    }
}
