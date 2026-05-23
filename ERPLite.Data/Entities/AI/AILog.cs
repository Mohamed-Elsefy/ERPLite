namespace ERPLite.Data.Entities.AI
{
    public class AILog
    {
        public int Id { get; set; }

        public string Prompt { get; set; } = null!;

        public string Response { get; set; } = null!;

        public int TokensUsed { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}