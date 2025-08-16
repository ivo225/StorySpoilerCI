using System.Text.Json.Serialization;

namespace StorySpoiler.Models
{
    //Adapt name to match the project
    internal class StoryDTO
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }
}
