namespace CarboxylicLithium;

public static class NullUtil
{
    public static Result<TY?> ToNullableResultOr<T, TY>(this T? obj, Func<T, Result<TY>> act)
        where T : struct
        where TY : struct
    {
        if (obj is null)
            return (TY?)null;
        var o = obj.Value;
        return act(o).Then(x => (TY?)x, Errors.MapNone);
    }

    public static Result<TY?> ToNullableResultOr<T, TY>(this T? obj, Func<T, Result<TY>> act)
        where T : class
        where TY : class
    {
        if (obj is null)
            return (TY?)null;
        return act(obj).Then(TY? (x) => x, Errors.MapNone);
    }

    public static async Task<Result<TY?>> ToNullableTaskResultOr<T, TY>(
        this T? obj,
        Func<T, Task<Result<TY>>> act
    )
        where T : struct
        where TY : struct
    {
        if (obj is null)
            return (TY?)null;
        var o = obj.Value;
        return (await act(o)).Then(x => (TY?)x, Errors.MapNone);
    }

    public static async Task<Result<TY?>> ToNullableTaskResultOr<T, TY>(
        this T? obj,
        Func<T, Task<Result<TY>>> act
    )
        where T : class
        where TY : class
    {
        if (obj is null)
            return (TY?)null;
        return (await act(obj)).Then(TY? (x) => x, Errors.MapNone);
    }

    public static Result<T?> ToNullableResult<T>(this T? obj)
    {
        return obj;
    }

    public static Task<Result<T?>> ToNullableTaskResult<T>(this T? obj)
    {
        var r = new Result<T?>(obj);
        return Task.FromResult(r);
    }
}
