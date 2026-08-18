using CleanMadeira.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CleanMadeira.Web.ViewModels
{
    public class CalendarIntegrationVM
    {
        public Guid? Id { get; set; }

        public Guid PropriedadeId { get; set; }

        [Display(Name = "Plataforma")]
        [Required(ErrorMessage = "Selecione a plataforma.")]
        public CalendarProvider Provider { get; set; }

        [Display(Name = "URL do calendário iCal")]
        [Required(ErrorMessage = "Introduza o URL do calendário.")]
        [Url(ErrorMessage = "Introduza um URL válido.")]
        public string CalendarUrl { get; set; } = string.Empty;

        [Display(Name = "Integração ativa")]
        public bool IsEnabled { get; set; } = true;

        public DateTime? LastSync { get; set; }
    }
}
