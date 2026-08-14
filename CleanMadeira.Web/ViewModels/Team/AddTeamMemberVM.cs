using System.ComponentModel.DataAnnotations;

namespace CleanMadeira.Web.ViewModels.Team
{
    public class AddTeamMemberVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
