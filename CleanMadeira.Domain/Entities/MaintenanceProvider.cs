namespace CleanMadeira.Domain.Entities
{
    public class MaintenanceProvider
    {
        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty; // Canalizador, Eletricista...

        public string Phone { get; set; } = string.Empty;

        public string? Email { get; set; }

        public ApplicationUser Owner { get; set; } = null!;

        public bool Active { get; set; }

        public string? Notes { get; set; }


    }
}
