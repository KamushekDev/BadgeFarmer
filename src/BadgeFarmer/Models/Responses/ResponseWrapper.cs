using Newtonsoft.Json;

namespace BadgeFarmer.Models.Responses
{
    public record ResponseWrapper<TResponse>([property: JsonProperty("response")] TResponse Response);
}