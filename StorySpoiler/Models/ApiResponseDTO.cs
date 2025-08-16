using System.Text.Json.Serialization;

namespace StorySpoiler.Models
{
    internal class ApiResponseDTO
    {
        //check if you need something here
        [JsonPropertyName("msg")]
        public string? Msg { get; set; }

        [JsonPropertyName("storyId")]
        public string? storyId { get; set; }
    }
}