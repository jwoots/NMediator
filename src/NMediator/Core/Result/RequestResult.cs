namespace NMediator.Core.Result
{
    public class RequestResult<T> : IRequestResult
    {
        public bool IsSuccess { get; set; }
        public Error Error { get; set; }
        public T Data { get; set; }
        object IRequestResult.Data => Data;

        public RequestResult(T data)
        {
            IsSuccess = true;
            Data = data;
        }

        public RequestResult(Error error)
        {
            IsSuccess = false;
            Error = error;
        }
    }

    public static class RequestResult
    {
        public static RequestResult<TData> Success<TData>(TData data) => new RequestResult<TData>(data);
        public static RequestResult<TData> Fail<TData>(Error error) => new RequestResult<TData>(error);
    }
}
