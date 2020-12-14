namespace BadgeFarmer.Responses
{
    public class ResponseWrapper<TResponse> where TResponse : IResponse
    {
        public TResponse Response { get; init; }

        public ResponseWrapper(TResponse response)
        {
            Response = response;
        }
    }
}