namespace CrosswordCreator.Data.Entities
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public int CrosswordId { get; set; }
        public Crossword Crossword { get; set; }
        public Answer Answer { get; set; }
    }
}
