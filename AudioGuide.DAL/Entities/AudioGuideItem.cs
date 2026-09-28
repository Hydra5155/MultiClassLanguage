namespace AudioGuide.DAL.Entities;

public class AudioGuideItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string LanguageCode { get; set; } = "vi";
    public string AudioUrl { get; set; } = string.Empty;
    public string Transcript { get; set; } = string.Empty;
}