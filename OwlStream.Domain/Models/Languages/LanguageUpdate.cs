namespace OwlStream.Domain.Models.Languages;

public class LanguageUpdate : Language
{
    public int Id { get; set; }
    public bool Active { get; set; }
    public bool Main { get; set; }
}