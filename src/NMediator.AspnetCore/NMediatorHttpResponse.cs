namespace NMediator.AspnetCore
{
    /// <summary>
    /// NMediator HttpResponse structure for aspcore HttpContext.HttpResponse
    /// </summary>
    public class NMediatorHttpResponse
    {
        public string Body { get; set; }
        public int StatusCode { get; set; }
    }
}
