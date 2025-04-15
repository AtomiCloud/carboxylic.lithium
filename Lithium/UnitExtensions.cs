namespace CarboxylicLithium;

/// <summary>
/// Extension method converting void methods with up to 15 arguments to methods returning Unit.
/// </summary>
public static class UnitExtensions
{
    /// <summary>
    /// Extension method that converts a void method to one that returns Unit.
    /// </summary>
    /// <param name="func">Function to convert</param>
    /// <typeparam name="T1"></typeparam>
    /// <returns></returns>
    public static Func<T1, Unit> Unit<T1>(this Action<T1> func)
    {
        return t1 =>
        {
            func(t1);
            return new Unit();
        };
    }

    /// <summary>
    /// Extension method that converts a void method to one that returns Unit.
    /// </summary>
    /// <param name="func">Function to convert</param>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <returns></returns>
    public static Func<T1, T2, Unit> Unit<T1, T2>(this Action<T1, T2> func)
    {
        return (t1, t2) =>
        {
            func(t1, t2);
            return new Unit();
        };
    }

    /// <summary>
    /// Extension method that converts a void method to one that returns Unit.
    /// </summary>
    /// <param name="func">Function to convert</param>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <returns></returns>
    public static Func<T1, T2, T3, Unit> Unit<T1, T2, T3>(this Action<T1, T2, T3> func)
    {
        return (t1, t2, t3) =>
        {
            func(t1, t2, t3);
            return new Unit();
        };
    }

    /// <summary>
    /// Extension method that converts a void method to one that returns Unit.
    /// </summary>
    /// <param name="func">Function to convert</param>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <returns></returns>
    public static Func<T1, T2, T3, T4, Unit> Unit<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> func)
    {
        return (t1, t2, t3, t4) =>
        {
            func(t1, t2, t3, t4);
            return new Unit();
        };
    }

    /// <summary>
    /// Extension method that converts a void method to one that returns Unit.
    /// </summary>
    /// <param name="func">Function to convert</param>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <returns></returns>
    public static Func<T1, T2, T3, T4, T5, Unit> Unit<T1, T2, T3, T4, T5>(
        this Action<T1, T2, T3, T4, T5> func
    )
    {
        return (t1, t2, t3, t4, t5) =>
        {
            func(t1, t2, t3, t4, t5);
            return new Unit();
        };
    }

    /// <summary>
    /// Extension method that converts a void method to one that returns Unit.
    /// </summary>
    /// <param name="func">Function to convert</param>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <returns></returns>
    public static Func<T1, T2, T3, T4, T5, T6, Unit> Unit<T1, T2, T3, T4, T5, T6>(
        this Action<T1, T2, T3, T4, T5, T6> func
    )
    {
        return (t1, t2, t3, t4, t5, t6) =>
        {
            func(t1, t2, t3, t4, t5, t6);
            return new Unit();
        };
    }

    /// <summary>
    /// Extension method that converts a void method to one that returns Unit.
    /// </summary>
    /// <param name="func">Function to convert</param>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <returns></returns>
    public static Func<T1, T2, T3, T4, T5, T6, T7, Unit> Unit<T1, T2, T3, T4, T5, T6, T7>(
        this Action<T1, T2, T3, T4, T5, T6, T7> func
    )
    {
        return (t1, t2, t3, t4, t5, t6, t7) =>
        {
            func(t1, t2, t3, t4, t5, t6, t7);
            return new Unit();
        };
    }

    /// <summary>
    /// Extension method that converts a void method to one that returns Unit.
    /// </summary>
    /// <param name="func">Function to convert</param>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <typeparam name="T8"></typeparam>
    /// <returns></returns>
    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, Unit> Unit<T1, T2, T3, T4, T5, T6, T7, T8>(
        this Action<T1, T2, T3, T4, T5, T6, T7, T8> func
    )
    {
        return (t1, t2, t3, t4, t5, t6, t7, t8) =>
        {
            func(t1, t2, t3, t4, t5, t6, t7, t8);
            return new Unit();
        };
    }

    /// <summary>
    /// Extension method that converts a void method to one that returns Unit.
    /// </summary>
    /// <param name="func">Function to convert</param>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <typeparam name="T8"></typeparam>
    /// <typeparam name="T9"></typeparam>
    /// <returns></returns>
    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Unit> Unit<
        T1,
        T2,
        T3,
        T4,
        T5,
        T6,
        T7,
        T8,
        T9
    >(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> func)
    {
        return (t1, t2, t3, t4, t5, t6, t7, t8, t9) =>
        {
            func(t1, t2, t3, t4, t5, t6, t7, t8, t9);
            return new Unit();
        };
    }

    /// <summary>
    /// Extension method that converts a void method to one that returns Unit.
    /// </summary>
    /// <param name="func">Function to convert</param>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <typeparam name="T8"></typeparam>
    /// <typeparam name="T9"></typeparam>
    /// <typeparam name="T10"></typeparam>
    /// <returns></returns>
    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Unit> Unit<
        T1,
        T2,
        T3,
        T4,
        T5,
        T6,
        T7,
        T8,
        T9,
        T10
    >(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> func)
    {
        return (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10) =>
        {
            func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
            return new Unit();
        };
    }

    /// <summary>
    /// Extension method that converts a void method to one that returns Unit.
    /// </summary>
    /// <param name="func">Function to convert</param>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <typeparam name="T8"></typeparam>
    /// <typeparam name="T9"></typeparam>
    /// <typeparam name="T10"></typeparam>
    /// <typeparam name="T11"></typeparam>
    /// <returns></returns>
    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Unit> Unit<
        T1,
        T2,
        T3,
        T4,
        T5,
        T6,
        T7,
        T8,
        T9,
        T10,
        T11
    >(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> func)
    {
        return (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11) =>
        {
            func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);
            return new Unit();
        };
    }

    /// <summary>
    /// Extension method that converts a void method to one that returns Unit.
    /// </summary>
    /// <param name="func">Function to convert</param>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <typeparam name="T8"></typeparam>
    /// <typeparam name="T9"></typeparam>
    /// <typeparam name="T10"></typeparam>
    /// <typeparam name="T11"></typeparam>
    /// <typeparam name="T12"></typeparam>
    /// <returns></returns>
    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Unit> Unit<
        T1,
        T2,
        T3,
        T4,
        T5,
        T6,
        T7,
        T8,
        T9,
        T10,
        T11,
        T12
    >(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> func)
    {
        return (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12) =>
        {
            func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);
            return new Unit();
        };
    }

    /// <summary>
    /// Extension method that converts a void method to one that returns Unit.
    /// </summary>
    /// <param name="func">Function to convert</param>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <typeparam name="T8"></typeparam>
    /// <typeparam name="T9"></typeparam>
    /// <typeparam name="T10"></typeparam>
    /// <typeparam name="T11"></typeparam>
    /// <typeparam name="T12"></typeparam>
    /// <typeparam name="T13"></typeparam>
    /// <returns></returns>
    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, Unit> Unit<
        T1,
        T2,
        T3,
        T4,
        T5,
        T6,
        T7,
        T8,
        T9,
        T10,
        T11,
        T12,
        T13
    >(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> func)
    {
        return (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13) =>
        {
            func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13);
            return new Unit();
        };
    }

    /// <summary>
    /// Extension method that converts a void method to one that returns Unit.
    /// </summary>
    /// <param name="func">Function to convert</param>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <typeparam name="T8"></typeparam>
    /// <typeparam name="T9"></typeparam>
    /// <typeparam name="T10"></typeparam>
    /// <typeparam name="T11"></typeparam>
    /// <typeparam name="T12"></typeparam>
    /// <typeparam name="T13"></typeparam>
    /// <typeparam name="T14"></typeparam>
    /// <returns></returns>
    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, Unit> Unit<
        T1,
        T2,
        T3,
        T4,
        T5,
        T6,
        T7,
        T8,
        T9,
        T10,
        T11,
        T12,
        T13,
        T14
    >(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> func)
    {
        return (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14) =>
        {
            func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
            return new Unit();
        };
    }

    /// <summary>
    /// Extension method that converts a void method to one that returns Unit.
    /// </summary>
    /// <param name="func">Function to convert</param>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <typeparam name="T8"></typeparam>
    /// <typeparam name="T9"></typeparam>
    /// <typeparam name="T10"></typeparam>
    /// <typeparam name="T11"></typeparam>
    /// <typeparam name="T12"></typeparam>
    /// <typeparam name="T13"></typeparam>
    /// <typeparam name="T14"></typeparam>
    /// <typeparam name="T15"></typeparam>
    /// <returns></returns>
    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, Unit> Unit<
        T1,
        T2,
        T3,
        T4,
        T5,
        T6,
        T7,
        T8,
        T9,
        T10,
        T11,
        T12,
        T13,
        T14,
        T15
    >(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> func)
    {
        return (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) =>
        {
            func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15);
            return new Unit();
        };
    }
}
