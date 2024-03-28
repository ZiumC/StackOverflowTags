namespace StackOverflowTags.Models.DatabaseModels
{
    public class TagModel
    {
        public int Id { get; set; }
        public bool HasSynonyms { get; set; }
        public bool IsModeratorOnly { get; set; }
        public bool IsRequired { get; set; }
        public int Count { get; set; }
        public string Name { get; set; }
        public double Share { get; set; }
    }
}
