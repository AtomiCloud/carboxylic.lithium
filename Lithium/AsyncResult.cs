using static CarboxylicLithium.Errors;

#pragma warning disable CS1998

namespace CarboxylicLithium;

/// <summary>
/// Static class with extension methods to support Async Result processing
/// </summary>
public static class AsyncResult
{
    /// <summary>
    /// Converts an object into an Async Success
    /// </summary>
    /// <param name="obj">Object to convert</param>
    /// <typeparam name="TSucc">Type of the object</typeparam>
    /// <returns>An Async Success containing the object</returns>
    public static async Task<Result<TSucc>> ToAsyncResult<TSucc>(this TSucc obj)
    {
        return obj;
    }

    /// <summary>
    /// Converts a Result into an Async Result
    /// </summary>
    /// <param name="obj">Result to convert</param>
    /// <typeparam name="TSucc">Type of the Success</typeparam>
    /// <returns>An Async Result containing the object</returns>
    public static async Task<Result<TSucc>> ToAsyncResult<TSucc>(this Result<TSucc> obj)
    {
        return obj;
    }

    /// <summary>
    /// Converts a normal single argument function into a new function that outputs an Async Result.
    /// </summary>
    /// <param name="func">Function to execute</param>
    /// <param name="mapException">Exceptions to catch and map to Failure</param>
    /// <typeparam name="TInput">Input type</typeparam>
    /// <typeparam name="TSucc">Output type</typeparam>
    /// <returns>Async Function that outputs a Success if successful, converting relevant Exceptions into Failure</returns>
    public static Func<TInput, Task<Result<TSucc>>> ToAsyncResultFunc<TInput, TSucc>(
        this Func<TInput, Task<TSucc>> func,
        ExceptionFilter mapException
    )
    {
        return async x =>
        {
            try
            {
                return await func(x);
            }
            catch (Exception e)
            {
                if (!mapException(e))
                    throw;
                return e;
            }
        };
    }

    /// <summary>
    /// Converts a normal single argument void function into a new function that outputs an Async Result.
    /// </summary>
    /// <param name="func">Function to execute</param>
    /// <param name="mapException">Exceptions to catch and map to Failure</param>
    /// <typeparam name="TInput">Input type</typeparam>
    /// <returns>Async Function that outputs a Success of Unit if successful, converting relevant Exceptions into Failure</returns>
    public static Func<TInput, Task<Result<Unit>>> ToAsyncResultFunc<TInput>(
        this Func<TInput, Task> func,
        ExceptionFilter mapException
    )
    {
        return async x =>
        {
            try
            {
                await func(x);
                return new Unit();
            }
            catch (Exception e)
            {
                if (!mapException(e))
                    throw;
                return e;
            }
        };
    }

    /// <summary>
    /// Converts a normal single argument function into a new function that outputs an Async Result.
    /// </summary>
    /// <param name="func">Function to execute</param>
    /// <param name="mapException">Exceptions to catch and map to Failure</param>
    /// <typeparam name="TInput">Input type</typeparam>
    /// <typeparam name="TSucc">Output type</typeparam>
    /// <returns>Async Function that outputs a Success if successful, converting relevant Exceptions into Failure</returns>
    public static Func<TInput, Task<Result<TSucc>>> ToAsyncResultFunc<TInput, TSucc>(
        this Func<TInput, TSucc> func,
        ExceptionFilter mapException
    )
    {
        return async x =>
        {
            try
            {
                return func(x);
            }
            catch (Exception e)
            {
                if (!mapException(e))
                    throw;
                return e;
            }
        };
    }

    /// <summary>
    /// Converts a normal single argument void function into a new function that outputs an Async Result.
    /// </summary>
    /// <param name="func">Function to execute</param>
    /// <param name="mapException">Exceptions to catch and map to Failure</param>
    /// <typeparam name="TInput">Input type</typeparam>
    /// <returns>Async Function that outputs a Success of Unit if successful, converting relevant Exceptions into Failure</returns>
    public static Func<TInput, Task<Result<Unit>>> ToAsyncResultFunc<TInput>(
        this Action<TInput> func,
        ExceptionFilter mapException
    )
    {
        return async x =>
        {
            try
            {
                func(x);
                return new Unit();
            }
            catch (Exception e)
            {
                if (!mapException(e))
                    throw;
                return e;
            }
        };
    }

    /// <summary>
    /// Converts a normal single argument result function into a new function that outputs an Async Result.
    /// </summary>
    /// <param name="func">Function to execute</param>
    /// <typeparam name="TInput">Input type</typeparam>
    /// <typeparam name="TSucc">Output type</typeparam>
    /// <returns>Async Function that outputs a Success if successful, converting relevant Exceptions into Failure</returns>
    public static Func<TInput, Task<Result<TSucc>>> ToAsyncResultFunc<TInput, TSucc>(
        this Func<TInput, Result<TSucc>> func
    )
    {
        return async x => func(x);
    }

    /// <summary>
    /// Checks if value contained in Async Result is not null
    /// </summary>
    /// <param name="result">Result to check</param>
    /// <typeparam name="TSucc">Type of the value contained</typeparam>
    /// <returns>If value in Result is null, returns a NulLReferenceException as Failure</returns>
    public static async Task<Result<TSucc>> AssertNotNull<TSucc>(this Task<Result<TSucc>> result)
    {
        return (await result).AssertNotNull();
    }

    /// <summary>
    /// Checks if Async Result is a Failure
    /// </summary>
    /// <returns>True if Async Result is a Failure</returns>
    public static Task<bool> IsFailure<TSucc>(this Task<Result<TSucc>> result)
    {
        return result.Match(_ => false, _ => true);
    }

    /// <summary>
    /// Checks if Async Result contains a Success
    /// </summary>
    /// <returns>True if Async Result is contains a Success</returns>
    public static Task<bool> IsSuccess<TSucc>(this Task<Result<TSucc>> result)
    {
        return result.Match(_ => true, _ => false);
    }

    /// <summary>
    /// Returns the contents of the Async Result if successful, or throws the exception if it failed.
    /// </summary>
    /// <returns></returns>
    public static Task<TSucc> Get<TSucc>(this Task<Result<TSucc>> result)
    {
        return result.Match(x => x, err => throw err);
    }

    /// <summary>
    /// Unwraps and returns the Success if it exists, otherwise returns a default value
    /// </summary>
    /// <param name="result">Result to execute on</param>
    /// <param name="defaultImpl">Optional default value to return</param>
    /// <returns>Success or default value</returns>
    public static Task<TSucc> SuccessOrDefault<TSucc>(
        this Task<Result<TSucc>> result,
        TSucc defaultImpl = default!
    )
    {
        return result.Match(s => s, _ => defaultImpl);
    }

    /// <summary>
    /// Unwraps and returns the Failure if it exists, otherwise returns a default value
    /// </summary>
    /// <param name="result">Result to execute on</param>
    /// <param name="defaultImpl">Optional default value to return</param>
    /// <returns>Failure or default value</returns>
    public static Task<Exception> FailureOrDefault<TSucc>(
        this Task<Result<TSucc>> result,
        Exception defaultImpl = default!
    )
    {
        return result.Match(_ => defaultImpl, e => e);
    }
}
