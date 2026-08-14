namespace WhiteLagoon.Domain.Entities
{
    public class Reserva
    {
        public Guid Id { get; set; }

        public Guid PropertyId { get; set; }

        public string GuestNome { get; set; }

        public DateTime CheckIn { get; set; }

        public DateTime CheckOut { get; set; }

        public int Guests { get; set; }

        public string Platform { get; set; }
    }
}
