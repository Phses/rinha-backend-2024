namespace RinhaBackendApi;

public enum ResultStatus
{
    Success,
    HasValidation,
    EntityNotFound,
    NotProcessed,
    NoContent,
    WithError
}

public interface IResult
{
    ResultStatus Status { get; }
}

public interface IResult<out T> : IResult
{
    T? Data { get; }
}


public class Result : IResult
{
    public static Result Success()
        => new Result { Status = ResultStatus.Success };

    public static Result WithNoContent()
        => new Result { Status = ResultStatus.NoContent };

    public static Result EntityNotFound()
        => new()
        {
            Status = ResultStatus.EntityNotFound,
        };

    public static Result NotProcessed()
        => new()
        {
            Status = ResultStatus.NotProcessed,
        };

    public static Result WithError()
        => new()
        {
            Status = ResultStatus.WithError,
        };


    public ResultStatus Status { get; protected init; }

}

public class Result<T> : Result, IResult<T>
{
    public static Result<T> Success(T data)
        => new()
        {
            Data = data,
            Status = ResultStatus.Success
        };

    public new static Result<T> WithNoContent()
        => new()
        {
            Status = ResultStatus.NoContent
        };

    public new static Result<T> EntityNotFound()
        => new()
        {
            Status = ResultStatus.EntityNotFound,
        };

    public new static Result<T> EntityNotProcessed()
        => new()
        {
            Status = ResultStatus.NotProcessed,
        };

    public new static Result<T> WithError()
        => new()
        {
            Status = ResultStatus.WithError,
        };


    public T? Data { get; private init; }

    public static implicit operator Result<T>(T data) => Success(data);

}
