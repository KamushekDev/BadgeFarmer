namespace BadgeFarmer.Responses
{
    public class ResponseWrapper<TResponse>
    {
        public TResponse Response { get; init; }

        public ResponseWrapper(TResponse response)
        {
            Response = response;
        }
    }
}