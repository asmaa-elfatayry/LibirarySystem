using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common;


public class Result<T> : Result
{
    public T? Data { get; init; }

    public static Result<T> Success(T data, string message = "") =>
        new()
        {
            IsSuccess = true,
            Status = eResultStatus.Success,
            Data = data,
            Message = message
        };

    public static Result<T> Failure(eResultStatus status, string message) =>
        new()
        {
            IsSuccess = false,
            Status = status,
            Message = message
        };
}

public class Result
{
    public bool IsSuccess { get; init; }

    public eResultStatus Status { get; init; }

    public string Message { get; init; } = string.Empty;

    public static Result Success(string message = "") =>
        new()
        {
            IsSuccess = true,
            Status = eResultStatus.Success,
            Message = message
        };

    public static Result Failure(eResultStatus status, string message) =>
        new()
        {
            IsSuccess = false,
            Status = status,
            Message = message
        };
}