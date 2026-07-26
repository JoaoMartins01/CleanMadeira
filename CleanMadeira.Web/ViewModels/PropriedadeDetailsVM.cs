using CleanMadeira.Web.ViewModels.CleaningTask;
using CleanMadeira.Web.ViewModels.Propriedade;

namespace CleanMadeira.Web.ViewModels
{
    public class PropriedadeDetailsVM
    {
        public PropriedadeVM Propriedade { get; set; }

        public int CalendarCount { get; set; }

        public int InventoryCount { get; set; }

        public int PhotoCount { get; set; }

        public CleaningTaskVM? NextCleaningTask { get; set; }

        public List<CalendarIntegrationVM> CalendarIntegrations { get; set; }
            = new();
    }
}
