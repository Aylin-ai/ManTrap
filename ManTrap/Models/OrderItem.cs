namespace ManTrap.Models
{
    public class OrderItem
    {
        public int MangaId { get; set; }
        public string RussianName { get; set; }
        public string SourceOfChapterToTranslate { get; set; }
        public int ChapterId { get; set; }
    }
}
