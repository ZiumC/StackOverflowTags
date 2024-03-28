namespace StackOverflowTags.Models.DatabaseModels
{
    public class TagModel
    {
        public int IdTag { get; set; }
        public bool Has_synonyms { get; set; }
        public bool Is_moderator_only { get; set; }
        public bool Is_required { get; set; }
        public int Count { get; set; }
        public string Name { get; set; }
    }
}
