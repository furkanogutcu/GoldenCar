namespace Core.Utilities.Results
{
    //This type can be used for void
    public interface IResult
    {
        bool Success { get; }
        string Message { get; }
    }
}
