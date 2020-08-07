namespace NMediator.Result
{
    public interface IRequestResult
    {
        bool IsSuccess { get; set; }
        object Data { get; }
        Error Error { get; set; }
    }
}
