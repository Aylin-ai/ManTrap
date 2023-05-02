namespace ManTrap.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string DateOfCreation { get; set; }
        public string OrderStatus { get; set; }
        public string Customer { get; set; }
        public string TransalteTeam { get; set; }
        public float Cost { get; set; }
        public string? TargetDate { get; set; }
        public bool IsPaid { get; set; }
    }
}
