namespace ManTrap.Models
{
    public class Manga
    {
        public int Id { get; set; }
        public string ImageSource { get; set; }
        public string OriginalName { get; set; }
        public string RussianName { get; set; }
        public string EnglishName { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string TranslateStatus { get; set; }
        public List<string> Genres { get; set; } = new List<string>();
        public List<string> Authors { get; set; } = new List<string>();
        public List<string> Artsits { get; set; } = new List<string>();
        public List<string> Publishers { get; set; } = new List<string>();
        public List<string> ReleaseFormats { get; set; } = new List<string>();
        public List<string> TranslateTeams { get; set; } = new List<string>();
        public int YearOfRelease { get; set; }
        public string SourceOfOriginal { get; set; }
        public string Overview { get; set; }
    }
}
