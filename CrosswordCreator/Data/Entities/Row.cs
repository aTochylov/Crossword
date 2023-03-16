namespace CrosswordCreator.Data.Entities
{
    public class Row
    {
        public int Id { get; set; }
        public string Chars { get; set; }

        public int CrosswordId { get; set; }
        public Crossword Crossword { get; set; }
    }
}
