namespace CrosswordCreator.Data.Entities
{
    public class Crossword
    {
        public int Id { get; set; }
        public string Code { get; set; }

        public List<Row> Rows { get; set; }
        public List<Question> Questions { get; set; }
    }
}
