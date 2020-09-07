namespace NMediator.Result
{
    public interface IRequestResult
    {
        bool IsSuccess { get; }
        object Data { get; }
        Error Error { get; }
    }
}
