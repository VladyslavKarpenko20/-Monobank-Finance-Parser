namespace Parsing.DtoModels
{
    public class TransactionsReadDto
    {
        public int Id { get; set; }
        public string? CurentName { get; set; }

        public string? Descriptions { get; set; }

        public decimal Ammount { get; set; }

        public long Balance { get; set; }

        public DateTimeOffset Time { get; set; }
    }
}
