using CarboxylicLithium;
using FluentAssertions;

namespace UnitTest;

public class AsyncResultTests
{
    // ========================================
    // 1) ToAsyncResult<TSucc>(this TSucc obj)
    // ========================================

    private class ToAsyncResult_TSucc_should_ReturnAsyncSuccess_Data
        : TheoryData<object, Task<Result<object>>>
    {
        public ToAsyncResult_TSucc_should_ReturnAsyncSuccess_Data()
        {
            Add(0, Task.FromResult((Result<object>)0));
            Add(5, Task.FromResult((Result<object>)5));
            Add(-7, Task.FromResult((Result<object>)(-7)));
            Add("hello", Task.FromResult((Result<object>)"hello"));
            Add(string.Empty, Task.FromResult((Result<object>)string.Empty));
        }
    }

    [Theory]
    [ClassData(typeof(ToAsyncResult_TSucc_should_ReturnAsyncSuccess_Data))]
    public async Task ToAsyncResult_TSucc_should_ReturnAsyncSuccess(
        object input,
        Task<Result<object>> expectedTask
    )
    {
        // Arrange
        var expected = await expectedTask;

        // Act
        var actual = await input.ToAsyncResult();

        // Assert
        actual.Should().BeEquivalentTo(expected, "the result should match the expected value");
    }

    // =================================================
    // 2) ToAsyncResult<TSucc>(this Result<TSucc> obj)
    // =================================================

    // ---------------------------------
    //  Success Data
    // ---------------------------------
    private class ToAsyncResult_ResultTSucc_Should_ReturnAsyncEquivalent_SuccessData
        : TheoryData<Result<object>, Task<Result<object>>>
    {
        public ToAsyncResult_ResultTSucc_Should_ReturnAsyncEquivalent_SuccessData()
        {
            // A range of possible successes
            Add(new Result<object>(42), Task.FromResult((Result<object>)42));
            Add(new Result<object>("hello"), Task.FromResult((Result<object>)"hello"));
            Add(new Result<object>(true), Task.FromResult((Result<object>)true));
            Add(
                new Result<object>(DateTime.MinValue),
                Task.FromResult((Result<object>)DateTime.MinValue)
            );
        }
    }

    // ---------------------------------
    //  Failure Data
    // ---------------------------------
    private class ToAsyncResult_ResultTSucc_Should_ReturnAsyncEquivalent_FailureData
        : TheoryData<Result<object>, Task<Result<object>>>
    {
        public ToAsyncResult_ResultTSucc_Should_ReturnAsyncEquivalent_FailureData()
        {
            // Different exception types
            Add(
                (Result<object>)new InvalidOperationException("Test exception 1"),
                Task.FromResult((Result<object>)new InvalidOperationException("Test exception 1"))
            );
            Add(
                (Result<object>)new NullReferenceException("Test exception 2"),
                Task.FromResult((Result<object>)new NullReferenceException("Test exception 2"))
            );
            Add(
                (Result<object>)new FormatException("Test exception 3"),
                Task.FromResult((Result<object>)new FormatException("Test exception 3"))
            );
        }
    }

    // ---------------------------------
    //  Success Tests
    // ---------------------------------
    [Theory]
    [ClassData(typeof(ToAsyncResult_ResultTSucc_Should_ReturnAsyncEquivalent_SuccessData))]
    public async Task ToAsyncResult_ResultTSucc_should_ReturnAsyncEquivalent_SuccessCases(
        Result<object> input,
        Task<Result<object>> expectedTask
    )
    {
        // Arrange
        var expected = await expectedTask;

        // Act
        var actual = await input.ToAsyncResult();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    // ---------------------------------
    //  Failure Tests
    // ---------------------------------
    [Theory]
    [ClassData(typeof(ToAsyncResult_ResultTSucc_Should_ReturnAsyncEquivalent_FailureData))]
    public async Task ToAsyncResult_ResultTSucc_should_ReturnAsyncEquivalent_FailureCases(
        Result<object> input,
        Task<Result<object>> expectedTask
    )
    {
        // Arrange
        var expected = await expectedTask;

        // Act
        var actual = await input.ToAsyncResult();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    // ========================================================================================================
    // 3) ToAsyncResultFunc<TInput, TSucc>(this Func<TInput, Task<TSucc>> func, ExceptionFilter mapException)
    // ========================================================================================================

    // Happy Path Data
    private class ToAsyncResultFunc_Task_Should_ReturnSuccess_Data
        : TheoryData<Func<int, Task<int>>, int, Result<int>>
    {
        public ToAsyncResultFunc_Task_Should_ReturnSuccess_Data()
        {
            // Simple addition
            Add(async x => await Task.FromResult(x + 10), 5, 15);
            // Multiplication
            Add(async x => await Task.FromResult(x * 2), 4, 8);
            // Return negative
            Add(async x => await Task.FromResult(-x), 5, -5);
        }
    }

    [Theory]
    [ClassData(typeof(ToAsyncResultFunc_Task_Should_ReturnSuccess_Data))]
    public async Task ToAsyncResultFunc_Task_should_ReturnSuccess(
        Func<int, Task<int>> func,
        int input,
        Result<int> expected
    )
    {
        // Arrange

        // Act
        // Convert the original function into an async result function
        var asyncResultFunc = func.ToAsyncResultFunc(Errors.MapNone);
        var actual = await asyncResultFunc(input);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    // Failure Path Data
    private class ToAsyncResultFunc_Task_Should_CatchExceptions_Data
        : TheoryData<Func<int, Task<int>>, int, Result<int>>
    {
        public ToAsyncResultFunc_Task_Should_CatchExceptions_Data()
        {
            // 1) Throwing InvalidOperationException
            Add(
                async x =>
                {
                    await Task.Yield();
                    throw new InvalidOperationException("Invalid operation occurred");
                },
                10,
                new InvalidOperationException("Invalid operation occurred")
            );

            // 2) Throwing ArgumentNullException
            Add(
                async x =>
                {
                    await Task.Yield();
                    throw new NullReferenceException("ArgumentNull occurred");
                },
                5,
                new NullReferenceException("ArgumentNull occurred")
            );
        }
    }

    [Theory]
    [ClassData(typeof(ToAsyncResultFunc_Task_Should_CatchExceptions_Data))]
    public async Task ToAsyncResultFunc_Task_should_CatchExceptions(
        Func<int, Task<int>> func,
        int input,
        Result<int> expected
    )
    {
        // Arrange

        // Act
        var asyncResultFunc = func.ToAsyncResultFunc(Errors.MapAll);
        var actual = await asyncResultFunc(input);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task ToAsyncResultFunc_Task_should_PropagateExceptions_If_NotMapped()
    {
        // Arrange

        // Original function always throws
        Func<int, Task<int>> throwingFunc = async x =>
        {
            await Task.Yield();
            throw new FormatException("Should be rethrown");
        };

        var asyncResultFunc = throwingFunc.ToAsyncResultFunc(Errors.MapNone);

        // Act
        var act = async () => await asyncResultFunc(123);

        // Assert - expecting unwrapped exception
        await act.Should().ThrowAsync<FormatException>().WithMessage("Should be rethrown");
    }

    // ===========================================================================================
    // 4) ToAsyncResultFunc<TInput>(this Func<TInput, Task> func, ExceptionFilter mapException)
    // ===========================================================================================

    // ---------------------------
    // Happy Path Data
    // ---------------------------
    private class ToAsyncResultFunc_TaskVoid_Should_ReturnSuccess_Data
        : TheoryData<Func<int, Task>, int, Result<Unit>>
    {
        public ToAsyncResultFunc_TaskVoid_Should_ReturnSuccess_Data()
        {
            // 1) Simple no-op function
            Add(
                async x =>
                {
                    await Task.Yield();
                },
                0,
                new Unit()
            );

            // 2) Fake "logic" function that awaits a small task (still returns void)
            Add(
                async x =>
                {
                    await Task.Delay(10);
                },
                1,
                new Unit()
            );

            // 3) Another variant
            Add(
                async x =>
                {
                    await Task.Delay(1);
                },
                999,
                new Unit()
            );
        }
    }

    [Theory]
    [ClassData(typeof(ToAsyncResultFunc_TaskVoid_Should_ReturnSuccess_Data))]
    public async Task ToAsyncResultFunc_TaskVoid_should_ReturnSuccess(
        Func<int, Task> func,
        int input,
        Result<Unit> expected
    )
    {
        // Arrange

        // Act
        // Convert the original function into an async-result function
        var asyncResultFunc = func.ToAsyncResultFunc(Errors.MapNone);
        var actual = await asyncResultFunc(input);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    // ---------------------------
    // Failure (Exception) Data
    // ---------------------------
    private class ToAsyncResultFunc_TaskVoid_Should_CatchExceptions_Data
        : TheoryData<Func<int, Task>, int, Result<Unit>>
    {
        public ToAsyncResultFunc_TaskVoid_Should_CatchExceptions_Data()
        {
            // 1) InvalidOperationException
            Add(
                async x =>
                {
                    await Task.Yield();
                    throw new InvalidOperationException("Operation is invalid");
                },
                10,
                new InvalidOperationException("Operation is invalid")
            );

            // 2) ArgumentOutOfRangeException
            Add(
                async x =>
                {
                    await Task.Delay(1);
                    throw new NullReferenceException("Value out of range");
                },
                99,
                new NullReferenceException("Value out of range")
            );
        }
    }

    [Theory]
    [ClassData(typeof(ToAsyncResultFunc_TaskVoid_Should_CatchExceptions_Data))]
    public async Task ToAsyncResultFunc_TaskVoid_should_CatchExceptions(
        Func<int, Task> func,
        int input,
        Result<Unit> expected
    )
    {
        // Arrange

        // Act
        var asyncResultFunc = func.ToAsyncResultFunc(Errors.MapAll);
        var actual = await asyncResultFunc(input);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task ToAsyncResultFunc_TaskVoid_Should_PropagateExceptions_IfNotMapped()
    {
        // Arrange
        // No mapping => exceptions are rethrown
        Func<int, Task> throwingFunc = async x =>
        {
            await Task.Yield();
            throw new FormatException("Unmapped exception should be thrown");
        };

        var asyncResultFunc = throwingFunc.ToAsyncResultFunc(Errors.MapNone);

        // Act
        var act = async () => _ = await asyncResultFunc(5);

        // Assert
        await act.Should()
            .ThrowAsync<FormatException>()
            .WithMessage("Unmapped exception should be thrown");
    }

    // ==================================================================================================
    // 5) ToAsyncResultFunc<TInput, TSucc>(this Func<TInput, TSucc> func, ExceptionFilter mapException)
    // ==================================================================================================

    // ---------------------------
    // Happy Path Data
    // ---------------------------
    private class ToAsyncResultFunc_Sync_Should_ReturnSuccess_Data
        : TheoryData<Func<int, int>, int, Result<int>>
    {
        public ToAsyncResultFunc_Sync_Should_ReturnSuccess_Data()
        {
            // 1) Simple addition
            Add(x => x + 10, 5, 15);
            // 2) Multiplication
            Add(x => x * 2, 4, 8);
            // 3) Negation
            Add(x => -x, 5, -5);
            // 4) Return zero, ignoring input
            Add(x => 0, 100, 0);
            // 5) Identity
            Add(x => x, -999, -999);
        }
    }

    [Theory]
    [ClassData(typeof(ToAsyncResultFunc_Sync_Should_ReturnSuccess_Data))]
    public async Task ToAsyncResultFunc_Sync_should_ReturnSuccess(
        Func<int, int> func,
        int input,
        Result<int> expected
    )
    {
        // Arrange

        // Act
        var asyncResultFunc = func.ToAsyncResultFunc(Errors.MapNone);
        var actual = await asyncResultFunc(input);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    // ---------------------------
    // Failure (Exception) Data
    // ---------------------------
    private class ToAsyncResultFunc_Sync_Should_CatchExceptions_Data
        : TheoryData<Func<int, int>, int, Result<int>>
    {
        public ToAsyncResultFunc_Sync_Should_CatchExceptions_Data()
        {
            // 1) InvalidOperationException
            Add(
                x => throw new InvalidOperationException("Invalid operation occurred"),
                10,
                new InvalidOperationException("Invalid operation occurred")
            );

            // 2) ArgumentOutOfRangeException
            Add(
                x => throw new ArgumentOutOfRangeException("testParam", "Value out of range"),
                99,
                new ArgumentOutOfRangeException("testParam", "Value out of range")
            );
        }
    }

    [Theory]
    [ClassData(typeof(ToAsyncResultFunc_Sync_Should_CatchExceptions_Data))]
    public async Task ToAsyncResultFunc_Sync_should_CatchExceptions(
        Func<int, int> func,
        int input,
        Result<int> expected
    )
    {
        // Arrange

        // Act
        var asyncResultFunc = func.ToAsyncResultFunc(Errors.MapAll);
        var actual = await asyncResultFunc(input);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task ToAsyncResultFunc_Sync_should_PropagateExceptions_If_NotMapped()
    {
        // Arrange
        Func<int, int> throwingFunc = x => throw new FormatException("Should be rethrown");

        var asyncResultFunc = throwingFunc.ToAsyncResultFunc(Errors.MapNone);

        // Act
        var act = async () => await asyncResultFunc(123);

        // Assert
        await act.Should().ThrowAsync<FormatException>().WithMessage("Should be rethrown");
    }

    // =====================================================================================
    // 6) ToAsyncResultFunc<TInput>(this Action<TInput> func, ExceptionFilter mapException)
    // =====================================================================================

    // --------------------------------------
    // Happy Path Data
    // --------------------------------------
    private class ToAsyncResultFunc_Action_Should_ReturnSuccess_Data
        : TheoryData<Action<int>, int, Result<Unit>>
    {
        public ToAsyncResultFunc_Action_Should_ReturnSuccess_Data()
        {
            // 1) No-op action
            Add(
                x =>
                {
                    /* do nothing */
                },
                0,
                new Unit()
            );

            // 2) Simple action that does some "computation" but returns nothing
            Add(
                x =>
                {
                    var y = x + 10;
                    _ = y; // ignoring result
                },
                5,
                new Unit()
            );

            // 3) Another variant that does some logic with negative numbers
            Add(
                x =>
                {
                    if (x < 0)
                    {
                        // but do nothing
                    }
                },
                -5,
                new Unit()
            );
        }
    }

    [Theory]
    [ClassData(typeof(ToAsyncResultFunc_Action_Should_ReturnSuccess_Data))]
    public async Task ToAsyncResultFunc_Action_should_ReturnSuccess(
        Action<int> action,
        int input,
        Result<Unit> expected
    )
    {
        // Arrange

        // Act
        var asyncResultFunc = action.ToAsyncResultFunc(Errors.MapNone);
        var actual = await asyncResultFunc(input);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    // --------------------------------------
    // Failure (Exception) Data
    // --------------------------------------
    private class ToAsyncResultFunc_Action_Should_CatchExceptions_Data
        : TheoryData<Action<int>, int, Result<Unit>>
    {
        public ToAsyncResultFunc_Action_Should_CatchExceptions_Data()
        {
            // 1) Throw InvalidOperationException
            Add(
                x => throw new InvalidOperationException("Invalid operation occurred"),
                10,
                new InvalidOperationException("Invalid operation occurred")
            );

            // 2) Throw ArgumentNullException
            Add(
                x => throw new NullReferenceException("ArgumentNull occurred"),
                5,
                new NullReferenceException("ArgumentNull occurred")
            );

            // 3) Another exception type
            Add(
                x => throw new FormatException("Bad format"),
                123,
                new FormatException("Bad format")
            );
        }
    }

    [Theory]
    [ClassData(typeof(ToAsyncResultFunc_Action_Should_CatchExceptions_Data))]
    public async Task ToAsyncResultFunc_Action_should_CatchExceptions(
        Action<int> action,
        int input,
        Result<Unit> expected
    )
    {
        // Arrange

        // Act
        var asyncResultFunc = action.ToAsyncResultFunc(Errors.MapAll);
        var actual = await asyncResultFunc(input);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task ToAsyncResultFunc_Action_should_PropagateExceptions_If_NotMapped()
    {
        // Arrange
        Action<int> throwingAction = x => throw new FormatException("Should be rethrown");
        var asyncResultFunc = throwingAction.ToAsyncResultFunc(Errors.MapNone);

        // Act
        var act = async () => await asyncResultFunc(123);

        // Assert
        await act.Should().ThrowAsync<FormatException>().WithMessage("Should be rethrown");
    }

    // =============================================================================
    // 7) ToAsyncResultFunc<TInput, TSucc>(this Func<TInput, Result<TSucc>> func)
    // =============================================================================
    //
    // This overload includes no ExceptionFilter, because it's a specific overload
    // that directly returns Task<Result<TSucc>> from a synchronous Result<TSucc>.

    // --------------------------------------
    // Happy Path Data
    // --------------------------------------
    private class ToAsyncResultFunc_Result_Should_ReturnAsyncSuccess_Data
        : TheoryData<Func<int, Result<int>>, int, Result<int>>
    {
        public ToAsyncResultFunc_Result_Should_ReturnAsyncSuccess_Data()
        {
            // 1) Return success
            Add(x => new Result<int>(x + 10), 5, new Result<int>(15));

            // 2) Return success from an identity
            Add(x => new Result<int>(x), -5, new Result<int>(-5));

            // 3) Always return zero
            Add(x => new Result<int>(0), 999, new Result<int>(0));

            // 4) Return success with negative transformation
            Add(x => new Result<int>(-x), 10, new Result<int>(-10));
        }
    }

    [Theory]
    [ClassData(typeof(ToAsyncResultFunc_Result_Should_ReturnAsyncSuccess_Data))]
    public async Task ToAsyncResultFunc_Result_should_ReturnAsyncSuccess(
        Func<int, Result<int>> func,
        int input,
        Result<int> expected
    )
    {
        // Arrange

        // Act
        // Convert the synchronous "Result<TSucc>" function into an async version
        var asyncResultFunc = func.ToAsyncResultFunc();
        var actual = await asyncResultFunc(input);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    // --------------------------------------
    // Failure (Exception) Data
    // --------------------------------------
    // In this overload, if the underlying func returns a Failure result, it should remain as is.
    // If func throws an exception, there's no built-in ExceptionFilter to catch it, so it should propagate.
    private class ToAsyncResultFunc_Result_Should_ReturnAsyncFailure_Data
        : TheoryData<Func<int, Result<int>>, int, Result<int>>
    {
        public ToAsyncResultFunc_Result_Should_ReturnAsyncFailure_Data()
        {
            // 1) Return an explicit failure
            var ex1 = new InvalidOperationException("Operation invalid");
            Add(
                x => ex1, // returning an Exception means a failure result
                0,
                (Result<int>)ex1
            );

            // 2) Another explicit failure
            var ex2 = new NullReferenceException("is null");
            Add(x => ex2, 10, (Result<int>)ex2);
        }
    }

    [Theory]
    [ClassData(typeof(ToAsyncResultFunc_Result_Should_ReturnAsyncFailure_Data))]
    public async Task ToAsyncResultFunc_Result_should_ReturnAsyncFailure(
        Func<int, Result<int>> func,
        int input,
        Result<int> expected
    )
    {
        // Arrange

        // Act
        var asyncResultFunc = func.ToAsyncResultFunc();
        var actual = await asyncResultFunc(input);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task ToAsyncResultFunc_Result_should_PropagateThrownExceptions()
    {
        // Arrange
        // If the function itself throws an exception (instead of returning a failure),
        // there's no filter to catch it, so it must propagate.
        Func<int, Result<int>> throwingFunc = x => throw new FormatException("Uncaught exception");

        var asyncResultFunc = throwingFunc.ToAsyncResultFunc();

        // Act
        var act = async () => await asyncResultFunc(123);

        // Assert
        await act.Should().ThrowAsync<FormatException>().WithMessage("Uncaught exception");
    }

    // ==========================================================
    // 8) AssertNotNull<TSucc>(this Task<Result<TSucc>> result)
    // ==========================================================

    // ------------------------------------------------
    // Non-null success values → remain successes
    // ------------------------------------------------
    private class AssertNotNull_Should_PassForNonNullSuccess_Data
        : TheoryData<Task<Result<object>>, Result<object>>
    {
        public AssertNotNull_Should_PassForNonNullSuccess_Data()
        {
            Add(Task.FromResult((Result<object>)"hello"), "hello");
            Add(Task.FromResult((Result<object>)42), 42);
            Add(Task.FromResult((Result<object>)true), true);
        }
    }

    [Theory]
    [ClassData(typeof(AssertNotNull_Should_PassForNonNullSuccess_Data))]
    public async Task AssertNotNull_should_PassForNonNullSuccess(
        Task<Result<object>> input,
        Result<object> expected
    )
    {
        // Act
        var actual = await input.AssertNotNull();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    // -----------------------------------------------------------------
    // Null success values → become AssertionException (failure)
    // -----------------------------------------------------------------


    [Fact]
    public async Task AssertNotNull_should_FailForNullSuccess()
    {
        // Arrange
        var input = Task.FromResult((Result<object>)null!);
        Result<object> expected = new AssertionException("Expected Result to not be null.");

        // Act
        var actual = await input.AssertNotNull();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    // ---------------------------------------------------------
    // Pre-existing failures → remain as failures (unchanged)
    // ---------------------------------------------------------
    private class AssertNotNull_Should_NotAffectExistingFailures_Data
        : TheoryData<Task<Result<object>>, Result<object>>
    {
        public AssertNotNull_Should_NotAffectExistingFailures_Data()
        {
            Add(
                Task.FromResult(
                    (Result<object>)new InvalidOperationException("Something went wrong")
                ),
                new InvalidOperationException("Something went wrong")
            );
            Add(
                Task.FromResult((Result<object>)new NullReferenceException("Param was null")),
                new NullReferenceException("Param was null")
            );
            Add(
                Task.FromResult((Result<object>)new FormatException("Bad format encountered")),
                new FormatException("Bad format encountered")
            );
        }
    }

    [Theory]
    [ClassData(typeof(AssertNotNull_Should_NotAffectExistingFailures_Data))]
    public async Task AssertNotNull_should_NotAffectExistingFailures(
        Task<Result<object>> input,
        Result<object> expected
    )
    {
        // Act
        var actual = await input.AssertNotNull();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    // ==========================================================
    // 9) IsFailure<TSucc>(this Task<Result<TSucc>> result)
    // ==========================================================

    // -------------------------------
    // Success Cases => IsFailure = false
    // -------------------------------
    private class IsFailure_Should_ReturnFalse_ForSuccesses_Data : TheoryData<Task<Result<object>>>
    {
        public IsFailure_Should_ReturnFalse_ForSuccesses_Data()
        {
            Add(Task.FromResult((Result<object>)42));
            Add(Task.FromResult((Result<object>)"hello"));
            Add(Task.FromResult((Result<object>)null!));
        }
    }

    [Theory]
    [ClassData(typeof(IsFailure_Should_ReturnFalse_ForSuccesses_Data))]
    public async Task IsFailure_should_ReturnFalse_ForSuccesses(Task<Result<object>> input)
    {
        // Act
        var actual = await input.IsFailure();

        // Assert
        actual.Should().BeFalse("success results should yield IsFailure = false");
    }

    // -------------------------------
    // Failure Cases => IsFailure = true
    // -------------------------------
    private class IsFailure_Should_ReturnTrue_ForFailures_Data : TheoryData<Task<Result<object>>>
    {
        public IsFailure_Should_ReturnTrue_ForFailures_Data()
        {
            Add(
                Task.FromResult(
                    (Result<object>)new InvalidOperationException("Something went wrong")
                )
            );
            Add(Task.FromResult((Result<object>)new NullReferenceException("Null argument")));
        }
    }

    [Theory]
    [ClassData(typeof(IsFailure_Should_ReturnTrue_ForFailures_Data))]
    public async Task IsFailure_should_ReturnTrue_ForFailures(Task<Result<object>> input)
    {
        // Act
        var actual = await input.IsFailure();

        // Assert
        actual.Should().BeTrue("failure results should yield IsFailure = true");
    }

    // ==========================================================
    // 10) IsSuccess<TSucc>(this Task<Result<TSucc>> result)
    // ==========================================================

    // -------------------------------
    // Success Cases => IsSuccess = true
    // -------------------------------
    private class IsSuccess_Should_ReturnTrue_ForSuccesses_Data : TheoryData<Task<Result<object>>>
    {
        public IsSuccess_Should_ReturnTrue_ForSuccesses_Data()
        {
            // Different successes
            Add(Task.FromResult((Result<object>)42));
            Add(Task.FromResult((Result<object>)"hello world"));
            Add(Task.FromResult((Result<object>)null!)); // success containing null
        }
    }

    [Theory]
    [ClassData(typeof(IsSuccess_Should_ReturnTrue_ForSuccesses_Data))]
    public async Task IsSuccess_should_ReturnTrue_ForSuccesses(Task<Result<object>> input)
    {
        // Act
        var actual = await input.IsSuccess();

        // Assert
        actual.Should().BeTrue("a success result should yield IsSuccess = true");
    }

    // -------------------------------
    // Failure Cases => IsSuccess = false
    // -------------------------------
    private class IsSuccess_Should_ReturnFalse_ForFailures_Data : TheoryData<Task<Result<object>>>
    {
        public IsSuccess_Should_ReturnFalse_ForFailures_Data()
        {
            // Various exceptions
            Add(Task.FromResult((Result<object>)new InvalidOperationException("Invalid state")));
            Add(Task.FromResult((Result<object>)new FormatException("Bad format")));
        }
    }

    [Theory]
    [ClassData(typeof(IsSuccess_Should_ReturnFalse_ForFailures_Data))]
    public async Task IsSuccess_should_ReturnFalse_ForFailures(Task<Result<object>> input)
    {
        // Act
        var actual = await input.IsSuccess();

        // Assert
        actual.Should().BeFalse("a failure result should yield IsSuccess = false");
    }

    // ==================================================
    // 11) Get<TSucc>(this Task<Result<TSucc>> result)
    // ==================================================

    // -------------------------------------------------
    // Success Cases: Get should return the unwrapped value
    // -------------------------------------------------
    private class Get_Should_ReturnUnwrappedValue_SuccessData
        : TheoryData<Task<Result<object>>, object>
    {
        public Get_Should_ReturnUnwrappedValue_SuccessData()
        {
            // Success with integer
            Add(Task.FromResult((Result<object>)42), 42);
            // Success with string
            Add(Task.FromResult((Result<object>)"hello"), "hello");
            // Success with null
            Add(Task.FromResult((Result<object>)null!), null!);
            // Success with bool
            Add(Task.FromResult((Result<object>)true), true);
        }
    }

    [Theory]
    [ClassData(typeof(Get_Should_ReturnUnwrappedValue_SuccessData))]
    public async Task Get_should_ReturnUnwrappedValue_WhenSuccess(
        Task<Result<object>> input,
        object expected
    )
    {
        // Act
        var actual = await input.Get();

        // Assert
        actual.Should().Be(expected);
    }

    // -------------------------------------------------
    // Failure Cases: Get should throw the underlying exception
    // -------------------------------------------------
    private class Get_Should_ThrowException_FailureData
        : TheoryData<Task<Result<object>>, Exception>
    {
        public Get_Should_ThrowException_FailureData()
        {
            // InvalidOperationException
            Add(
                Task.FromResult((Result<object>)new InvalidOperationException("Invalid state")),
                new InvalidOperationException("Invalid state")
            );
            // FormatException
            Add(
                Task.FromResult((Result<object>)new FormatException("Bad format")),
                new FormatException("Bad format")
            );
        }
    }

    [Theory]
    [ClassData(typeof(Get_Should_ThrowException_FailureData))]
    public async Task Get_should_ThrowUnderlyingException_WhenFailure(
        Task<Result<object>> input,
        Exception expected
    )
    {
        // Act
        var act = async () => await input.Get();

        // Assert
        await act.Should()
            .ThrowAsync<Exception>()
            .WithMessage(expected.Message)
            .Where(x => x.GetType() == expected.GetType());
    }

    // ============================================================================================
    // 12) SuccessOrDefault<TSucc>(this Task<Result<TSucc>> result, TSucc defaultImpl = default!)
    // ============================================================================================

    // ------------------------------------------------------------------
    // Success Cases: should return the original success value (not default).
    // Format of test data: [subject, defaultValue, expected].
    // ------------------------------------------------------------------
    private class SuccessOrDefault_ShouldReturnSuccessValue_WhenSuccess_Data
        : TheoryData<Task<Result<int>>, int, int>
    {
        public SuccessOrDefault_ShouldReturnSuccessValue_WhenSuccess_Data()
        {
            Add(Task.FromResult((Result<int>)42), -9999, 42);
            Add(Task.FromResult((Result<int>)(int)-7), -9999, -7);
            Add(Task.FromResult((Result<int>)0), 123, 0);
        }
    }

    [Theory]
    [ClassData(typeof(SuccessOrDefault_ShouldReturnSuccessValue_WhenSuccess_Data))]
    public async Task SuccessOrDefault_shouldReturnSuccessValue_WhenSuccess(
        Task<Result<int>> subject,
        int input,
        int expected
    )
    {
        // Arrange is handled by theory

        // Act
        var actual = await subject.SuccessOrDefault(input);

        // Assert
        actual.Should().Be(expected);
    }

    // ------------------------------------------------------------------
    // Failure Cases: should return the provided default.
    // Format of test data: [subject, defaultValue, expected].
    // ------------------------------------------------------------------
    private class SuccessOrDefault_ShouldReturnDefault_WhenFailure_Data
        : TheoryData<Task<Result<int>>, int, int>
    {
        public SuccessOrDefault_ShouldReturnDefault_WhenFailure_Data()
        {
            // Failure returning a custom default
            Add(Task.FromResult((Result<int>)new InvalidOperationException("Fail1")), 123, 123);
            Add(Task.FromResult((Result<int>)new NullReferenceException("Fail2")), 999, 999);
        }
    }

    [Theory]
    [ClassData(typeof(SuccessOrDefault_ShouldReturnDefault_WhenFailure_Data))]
    public async Task SuccessOrDefault_shouldReturnDefault_WhenFailure(
        Task<Result<int>> subject,
        int input,
        int expected
    )
    {
        // Arrange is handled by theory

        // Act
        var actual = await subject.SuccessOrDefault(input);

        // Assert
        actual.Should().Be(expected, "failure result uses the default value");
    }

    // =====================================================================================
    // 13) FailureOrDefault<TSucc>(this Task<Result<TSucc>> result, Exception defaultImpl)
    // =====================================================================================
    //
    //  If the Task<Result<TSucc>> is a failure, we unwrap and return its exception.
    //  Otherwise, if it’s a success, we return the provided default exception.

    // ------------------------------------------------------------------------
    // Failure Cases: Should return the actual failure exception (ignore default)
    // Format: [subject, defaultException, expectedException]
    // ------------------------------------------------------------------------
    private class FailureOrDefault_shouldReturnFailure_WhenFailure_Data
        : TheoryData<Task<Result<int>>, Exception, Exception>
    {
        public FailureOrDefault_shouldReturnFailure_WhenFailure_Data()
        {
            // Example: subject is a failure, default is some random exception,
            // but we expect the subject's original exception to be returned.

            var subjectEx1 = new InvalidOperationException("Subject exception #1");
            var subjectEx2 = new NullReferenceException("Subject exception #2");
            var defaultEx1 = new FormatException("Default exception #A");
            var defaultEx2 = new NotSupportedException("Default exception #B");

            Add(Task.FromResult((Result<int>)subjectEx1), defaultEx1, subjectEx1);
            Add(Task.FromResult((Result<int>)subjectEx2), defaultEx2, subjectEx2);
        }
    }

    [Theory]
    [ClassData(typeof(FailureOrDefault_shouldReturnFailure_WhenFailure_Data))]
    public async Task FailureOrDefault_shouldReturnFailure_WhenFailure(
        Task<Result<int>> subject,
        Exception defaultEx,
        Exception expected
    )
    {
        // Act
        var actual = await subject.FailureOrDefault(defaultEx);

        // Assert
        actual.Should().BeOfType(expected.GetType());
        actual.Message.Should().Be(expected.Message);
    }

    // ------------------------------------------------------------------------
    // Success Cases: Should return the default exception (no failure to unwrap)
    // Format: [subject, defaultException, expectedException]
    // ------------------------------------------------------------------------
    private class FailureOrDefault_shouldReturnDefault_WhenSuccess_Data
        : TheoryData<Task<Result<int>>, Exception, Exception>
    {
        public FailureOrDefault_shouldReturnDefault_WhenSuccess_Data()
        {
            // subject is a success, so there's no failure to unwrap
            // we expect to get the default exception

            var defaultEx1 = new FormatException("Default exception #A");
            var defaultEx2 = new NotSupportedException("Default exception #B");

            Add(Task.FromResult((Result<int>)42), defaultEx1, defaultEx1);
            Add(Task.FromResult((Result<int>)(-999)), defaultEx2, defaultEx2);
        }
    }

    [Theory]
    [ClassData(typeof(FailureOrDefault_shouldReturnDefault_WhenSuccess_Data))]
    public async Task FailureOrDefault_shouldReturnDefault_WhenSuccess(
        Task<Result<int>> subject,
        Exception defaultEx,
        Exception expected
    )
    {
        // Act
        var actual = await subject.FailureOrDefault(defaultEx);

        // Assert
        actual.Should().BeOfType(expected.GetType());
        actual.Message.Should().Be(expected.Message);
    }

    // ======================================
    //   14) Match<TInput, T> (synchronous)
    // ======================================
    //
    // Signature under test:
    //   public static async Task<T> Match<TInput, T>(
    //       this Task<Result<TInput>> res,
    //       Func<TInput, T> Success,
    //       Func<Exception, T> Failure
    //   )
    //
    // This version returns a value of type T. We'll split into two sets of tests:
    //   1) Success scenario → expects the Success delegate to be called
    //   2) Failure scenario → expects the Failure delegate to be called

    // ----------------------------
    //    SUCCESS SCENARIOS
    // ----------------------------

    private class Match_TInput_T_Should_InvokeSuccessDelegate_Data
        : TheoryData<
            Task<Result<int>>, // the input Task<Result<int>>
            Func<int, string>, // success delegate
            Func<Exception, string>, // failure delegate
            string // expected final string
        >
    {
        public Match_TInput_T_Should_InvokeSuccessDelegate_Data()
        {
            // 1) Simple success -> "success #1"
            Add(
                Task.FromResult((Result<int>)42),
                val => $"Got success: {val}",
                ex => $"Failure: {ex.Message}",
                "Got success: 42"
            );

            // 2) Negative success
            Add(
                Task.FromResult((Result<int>)(-10)),
                val => $"Negative success: {val}",
                ex => $"Error occurred => {ex.Message}",
                "Negative success: -10"
            );
        }
    }

    [Theory]
    [ClassData(typeof(Match_TInput_T_Should_InvokeSuccessDelegate_Data))]
    public async Task Match_TInput_T_should_InvokeSuccessDelegate(
        Task<Result<int>> input,
        Func<int, string> successDelegate,
        Func<Exception, string> failureDelegate,
        string expected
    )
    {
        // Act
        var actual = await input.Match(successDelegate, failureDelegate);

        // Assert
        actual.Should().Be(expected);
    }

    // ----------------------------
    //    FAILURE SCENARIOS
    // ----------------------------

    private class Match_TInput_T_Should_InvokeFailureDelegate_Data
        : TheoryData<
            Task<Result<int>>, // the input Task<Result<int>>
            Func<int, string>, // success delegate
            Func<Exception, string>, // failure delegate
            string // expected final string
        >
    {
        public Match_TInput_T_Should_InvokeFailureDelegate_Data()
        {
            // 1) ArgumentNullException
            var ex1 = new NullReferenceException("ArgNull happened");
            Add(
                Task.FromResult((Result<int>)ex1),
                val => $"Success unexpectedly invoked with {val}",
                err => $"Fail invoked => {err.Message}",
                "Fail invoked => ArgNull happened"
            );

            // 2) InvalidOperationException
            var ex2 = new InvalidOperationException("Invalid #2");
            Add(
                Task.FromResult((Result<int>)ex2),
                val => $"Should not see success: {val}",
                err => $"Failure occurs: {err.Message}",
                "Failure occurs: Invalid #2"
            );
        }
    }

    [Theory]
    [ClassData(typeof(Match_TInput_T_Should_InvokeFailureDelegate_Data))]
    public async Task Match_TInput_T_should_InvokeFailureDelegate(
        Task<Result<int>> input,
        Func<int, string> successDelegate,
        Func<Exception, string> failureDelegate,
        string expected
    )
    {
        // Act
        var actual = await input.Match(successDelegate, failureDelegate);

        // Assert
        actual.Should().Be(expected);
    }
}
