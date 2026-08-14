namespace CleanMadeira.Domain.Entities
{
    public class InventoryItem
    {
        public Guid Id { get; set; }

        public Guid? PropertyId { get; set; }

        public Property Property { get; set; }

        public string Name { get; set; }

        public string Unity { get; set; }

        public int? Quantity { get; set; }

        public int? MinimumQuantity { get; set; }

        public bool Active { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
