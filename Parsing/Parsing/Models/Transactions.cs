namespace Parsing.Models
{
    public class Transactions
    {
        public int Id { get; set; }

        public string? CurentName { get; set; }

        public string? TransactionId { get; set; }

        public string? Descriptions { get; set; }

        public decimal Ammount { get; set; }

        public long Balance { get; set; }

        public DateTimeOffset Time { get; set; }
    }
}
