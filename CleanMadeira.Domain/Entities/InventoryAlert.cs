namespace WhiteLagoon.Domain.Entities
{
    public class InventoryAlert
    {
        public Guid Id { get; set; }

        public Guid InventoryItemId { get; set; }

        public int Quantity { get; set; }

        public bool Resolved { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
