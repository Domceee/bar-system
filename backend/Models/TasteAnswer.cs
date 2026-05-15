namespace backend.Models;

public class TasteAnswer
{
    public int Id { get; set; }

    public string QuestionKey { get; set; } = "";
    public string Answer { get; set; } = "";

    public int TasteProfileId { get; set; }
    public TasteProfile TasteProfile { get; set; } = null!;
}
