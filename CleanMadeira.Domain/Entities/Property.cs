using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMadeira.Domain.Entities
{
    public class Property
    {
        public Guid Id { get; set; }

        public Guid ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; } = null!;

        public string Name { get; set; }

        public string Address { get; set; }

        public string PostalCode { get; set; }
        public string Freguesia { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public int Rooms { get; set; }

        public int Bathrooms { get; set; }
        public int? NumberGuests { get; set; }
        public string? Description { get; set; }

        public Company? Company { get; set; }

        public ICollection<CleaningTask> CleaningTasks { get; set; }
        = new List<CleaningTask>();

        public ICollection<InventoryItem> InventoryItems { get; set; }
        = new List<InventoryItem>();

        public ICollection<CalendarIntegration> CalendarIntegrations { get; set; }
        = new List<CalendarIntegration>();
        public bool Active { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
