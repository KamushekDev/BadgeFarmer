using System.Text.Json.Serialization;

namespace BadgeFarmer.Models.Responses
{
    public record ResponseWrapper<TResponse>([property: JsonPropertyName("response")] TResponse Response);
}