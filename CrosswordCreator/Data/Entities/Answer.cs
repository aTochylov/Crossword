namespace CrosswordCreator.Data.Entities
{
    public class Answer
    {
        public int Id { get; set; }
        public string Word { get; set; }
        public int WordStartRow { get; set; }
        public int WordStartCol { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
    }
}
