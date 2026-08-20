using System.ComponentModel.DataAnnotations;

namespace CleanMadeira.Web.Views.Propriedade
{
    public class PropertyCleaningSettingsVM
    {
        public Guid PropertyId { get; set; }

        public string? PropertyName { get; set; }

        public bool AutoIntermediateCleaning { get; set; }


        [Range(2, 30, ErrorMessage = "O intervalo deve estar entre 2 e 30 dias.")]
        public int IntermediateCleaningIntervalDays { get; set; }
    }
}
