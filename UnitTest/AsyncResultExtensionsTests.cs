using CarboxylicLithium;
using FluentAssertions;
using Xunit.Sdk;

namespace UnitTest;

public class AsyncResultExtensionsTests
{
    // ======================================
    //          1) MapFailure
    // ======================================

    // Helper exception to illustrate a custom mapping
    private class CustomMappedException(string message) : Exception(message);

    // ----------------------------
    //    SUCCESS SCENARIOS
    // ----------------------------

    private class MapFailure_should_IgnoreMapperForSuccess_Data
        : TheoryData<Task<Result<int>>, Func<Exception, Exception>, Result<int>>
    {
        public MapFailure_should_IgnoreMapperForSuccess_Data()
        {
            // 1) Basic success ignoring the mapper
            Add(Task.FromResult((Result<int>)42), e => e, 42);
            // 2) Using a new exception type in the mapper
            Add(
                Task.FromResult((Result<int>)0),
                e => new InvalidOperationException("Mapped #1: " + e.Message),
                0
            );
            // 3) Another success with a custom mapped type
            Add(
                Task.FromResult((Result<int>)(-100)),
                e => new CustomMappedException("Mapped #2: " + e.Message),
                -100
            );
        }
    }

    [Theory]
    [ClassData(typeof(MapFailure_should_IgnoreMapperForSuccess_Data))]
    public async Task MapFailure_should_IgnoreMapperForSuccess(
        Task<Result<int>> input,
        Func<Exception, Exception> mapper,
        Result<int> expected
    )
    {
        // Act
        var actual = await input.MapFailure(mapper);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    // ----------------------------
    //    FAILURE SCENARIOS
    // ----------------------------

    private class MapFailure_should_ApplyMapperForFailure_Data
        : TheoryData<Task<Result<int>>, Func<Exception, Exception>, Result<int>>
    {
        public MapFailure_should_ApplyMapperForFailure_Data()
        {
            Add(
                Task.FromResult((Result<int>)new ArgumentNullException("paramA", "Original #A")),
                e => e,
                new ArgumentNullException("paramA", "Original #A")
            );

            Add(
                Task.FromResult((Result<int>)new InvalidOperationException("Original #B")),
                e => new CustomMappedException("Mapped #B => " + e.Message),
                new CustomMappedException("Mapped #B => Original #B")
            );

            Add(
                Task.FromResult((Result<int>)new FormatException("Original #C")),
                e => new InvalidOperationException("Mapped #C => " + e.Message),
                new InvalidOperationException("Mapped #C => Original #C")
            );
        }
    }

    [Theory]
    [ClassData(typeof(MapFailure_should_ApplyMapperForFailure_Data))]
    public async Task MapFailure_should_ApplyMapperForFailure(
        Task<Result<int>> input,
        Func<Exception, Exception> mapper,
        Result<int> expected
    )
    {
        // Act
        var actual = await input.MapFailure(mapper);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    // // ======================================
    // //   2) MatchAwait<TInput, T>
    // // ======================================

    // // ----------------------------
    // //    SUCCESS SCENARIOS
    // // ----------------------------

    // private class MatchAwait_TInput_T_should_InvokeSuccessCallback_Data
    //     : TheoryData<
    //         Task<Result<int>>,
    //         Func<int, Task<string>>,
    //         Func<Exception, Task<string>>,
    //         string
    //     >
    // {
    //     public MatchAwait_TInput_T_should_InvokeSuccessCallback_Data()
    //     {
    //         // 1) Simple success returning a string
    //         Add(
    //             Task.FromResult((Result<int>)42),
    //             async num => await Task.FromResult($"success: {num}"),
    //             async ex => await Task.FromResult($"failure: {ex.Message}"),
    //             "success: 42"
    //         );

    //         // 2) Another success returning a different string
    //         Add(
    //             Task.FromResult((Result<int>)(-999)),
    //             async num =>
    //             {
    //                 await Task.Delay(1); // simulate a small async delay
    //                 return $"got negative {num}";
    //             },
    //             async ex =>
    //             {
    //                 await Task.Yield();
    //                 return $"[ERROR] {ex.Message}";
    //             },
    //             "got negative -999"
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(MatchAwait_TInput_T_should_InvokeSuccessCallback_Data))]
    // public async Task MatchAwait_TInput_T_should_InvokeSuccessCallback(
    //     Task<Result<int>> input,
    //     Func<int, Task<string>> successCallback,
    //     Func<Exception, Task<string>> failureCallback,
    //     string expected
    // )
    // {
    //     // Act
    //     var actual = await input.MatchAwait(successCallback, failureCallback);

    //     // Assert
    //     actual.Should().Be(expected);
    // }

    // // ----------------------------
    // //    FAILURE SCENARIOS
    // // ----------------------------

    // private class MatchAwait_TInput_T_should_InvokeFailureCallback_Data
    //     : TheoryData<
    //         Task<Result<int>>,
    //         Func<int, Task<string>>,
    //         Func<Exception, Task<string>>,
    //         string
    //     >
    // {
    //     public MatchAwait_TInput_T_should_InvokeFailureCallback_Data()
    //     {
    //         // 1) ArgumentNullException
    //         Add(
    //             Task.FromResult((Result<int>)new NullReferenceException("some detail #1")),
    //             async num =>
    //             {
    //                 await Task.Yield();
    //                 return $"success: {num}";
    //             },
    //             async ex => await Task.FromResult($"caught: {ex.Message}"),
    //             "caught: some detail #1"
    //         );

    //         // 2) InvalidOperationException
    //         Add(
    //             Task.FromResult((Result<int>)new InvalidOperationException("test message #2")),
    //             num => Task.FromResult($"should not run: {num}"),
    //             ex => Task.FromResult($"handled => {ex.Message}"),
    //             "handled => test message #2"
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(MatchAwait_TInput_T_should_InvokeFailureCallback_Data))]
    // public async Task MatchAwait_TInput_T_should_InvokeFailureCallback(
    //     Task<Result<int>> input,
    //     Func<int, Task<string>> successCallback,
    //     Func<Exception, Task<string>> failureCallback,
    //     string expected
    // )
    // {
    //     // Act
    //     var actual = await input.MatchAwait(successCallback, failureCallback);

    //     // Assert
    //     actual.Should().Be(expected);
    // }

    // // A custom exception we can throw inside our Success or Failure callbacks,
    // // then catch afterward to verify which callback was actually used, and
    // // what message it produced.
    // private class CallbackExecutedException(string msg) : Exception(msg);

    // // ======================================
    // // 3) MatchAwait<TInput> with no return
    // // ======================================
    // //
    // // We'll split tests into two scenarios:
    // //   1) Success scenario: Expect success callback to be invoked.
    // //   2) Failure scenario: Expect failure callback to be invoked.
    // //
    // // We pass the callbacks themselves in the TheoryData rather than building them
    // // in the test method, as requested. Each callback throws a CallbackExecutedException
    // // to indicate which path was taken and to provide a testable message.

    // // ----------------------------
    // //    SUCCESS SCENARIOS
    // // ----------------------------

    // private class MatchAwait_TInput_Should_InvokeSuccessCallback_Data
    //     : TheoryData<
    //         Task<Result<int>>, // The async result input
    //         Func<int, Task>, // The success callback
    //         Func<Exception, Task>, // The failure callback
    //         string // The expected message if success callback is invoked
    //     >
    // {
    //     public MatchAwait_TInput_Should_InvokeSuccessCallback_Data()
    //     {
    //         // 1) Basic success (42)
    //         Add(
    //             Task.FromResult((Result<int>)42),
    //             async (int val) =>
    //             {
    //                 await Task.Yield();
    //                 throw new CallbackExecutedException($"SUCCESS: Value={val}");
    //             },
    //             async ex =>
    //             {
    //                 await Task.Yield();
    //                 throw new CallbackExecutedException($"FAILURE: {ex.Message}");
    //             },
    //             "SUCCESS: Value=42"
    //         );

    //         // 2) Negative success (-10)
    //         Add(
    //             Task.FromResult((Result<int>)(-10)),
    //             async (int val) =>
    //             {
    //                 await Task.Yield();
    //                 throw new CallbackExecutedException($"OK with negative {val}");
    //             },
    //             async ex =>
    //             {
    //                 await Task.Yield();
    //                 throw new CallbackExecutedException($"Should NOT see error => {ex.Message}");
    //             },
    //             "OK with negative -10"
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(MatchAwait_TInput_Should_InvokeSuccessCallback_Data))]
    // public async Task MatchAwait_TInput_should_InvokeSuccessCallback_UsingTheoryData(
    //     Task<Result<int>> input,
    //     Func<int, Task> successCallback,
    //     Func<Exception, Task> failureCallback,
    //     string expectedSuccessMessage
    // )
    // {
    //     // Because the library method returns no value, we'll detect
    //     // which callback was hit by catching our custom exception.

    //     async Task Act()
    //     {
    //         await input.MatchAwait(successCallback, failureCallback);
    //     }

    //     // We EXpect the success callback to run → custom exception with expected message
    //     var ex = await Assert.ThrowsAsync<CallbackExecutedException>(Act);
    //     ex.Message.Should().Be(expectedSuccessMessage);
    // }

    // // ----------------------------
    // //    FAILURE SCENARIOS
    // // ----------------------------

    // private class MatchAwait_TInput_Should_InvokeFailureCallback_Data
    //     : TheoryData<
    //         Task<Result<int>>, // The async result input
    //         Func<int, Task>, // The success callback
    //         Func<Exception, Task>, // The failure callback
    //         string // The expected message if failure callback is invoked
    //     >
    // {
    //     public MatchAwait_TInput_Should_InvokeFailureCallback_Data()
    //     {
    //         // 1) ArgumentNullException
    //         var ex1 = new NullReferenceException("Missing param");
    //         Add(
    //             Task.FromResult((Result<int>)ex1),
    //             async val =>
    //             {
    //                 await Task.Yield();
    //                 throw new CallbackExecutedException($"WRONG success call => {val}");
    //             },
    //             async ex =>
    //             {
    //                 await Task.Yield();
    //                 throw new CallbackExecutedException($"FAIL path => {ex.Message}");
    //             },
    //             "FAIL path => Missing param"
    //         );

    //         // 2) InvalidOperationException
    //         var ex2 = new InvalidOperationException("some invalid op");
    //         Add(
    //             Task.FromResult((Result<int>)ex2),
    //             async val =>
    //             {
    //                 await Task.Yield();
    //                 throw new CallbackExecutedException($"Wrongly invoked success => {val}");
    //             },
    //             async ex =>
    //             {
    //                 await Task.Yield();
    //                 throw new CallbackExecutedException($"Fail: {ex.Message}");
    //             },
    //             "Fail: some invalid op"
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(MatchAwait_TInput_Should_InvokeFailureCallback_Data))]
    // public async Task MatchAwait_TInput_should_InvokeFailureCallback_UsingTheoryData(
    //     Task<Result<int>> input,
    //     Func<int, Task> successCallback,
    //     Func<Exception, Task> failureCallback,
    //     string expectedFailureMessage
    // )
    // {
    //     // Again, we check which callback triggered by capturing the custom exception.

    //     async Task Act()
    //     {
    //         // If 'input' is failure, we expect the failure callback to throw our custom exception
    //         await input.MatchAwait(successCallback, failureCallback);
    //     }

    //     // We Expect the failure callback to run → custom exception with expected message
    //     var ex = await Assert.ThrowsAsync<CallbackExecutedException>(Act);
    //     ex.Message.Should().Be(expectedFailureMessage);
    // }

    // // A custom exception to detect exactly which callback was triggered.
    // private class MatchCallbackException(string? message) : Exception(message);

    // // ======================================
    // //   4) Match<TInput> (void return)
    // // ======================================
    // //
    // // Signature under test:
    // //   public static async Task Match<TInput>(
    // //       this Task<Result<TInput>> res,
    // //       Action<TInput> Success,
    // //       Action<Exception> Failure
    // //   )
    // //
    // // We'll split the tests into success and failure scenarios, passing the
    // // callbacks in the TheoryData. Each callback throws MatchCallbackException
    // // to confirm it's the one being invoked.

    // // ----------------------------
    // //    SUCCESS SCENARIOS
    // // ----------------------------

    // private class Match_TInput_should_InvokeSuccessAction_Data
    //     : TheoryData<
    //         Task<Result<int>>, // input Task<Result<int>>
    //         Action<int>, // success action
    //         Action<Exception>, // failure action
    //         string // expected exception message
    //     >
    // {
    //     public Match_TInput_should_InvokeSuccessAction_Data()
    //     {
    //         // 1) Simple success 100
    //         Add(
    //             Task.FromResult((Result<int>)100),
    //             val => throw new MatchCallbackException($"[SUCCESS] Got value: {val}"),
    //             ex => throw new MatchCallbackException($"[FAILURE] Unexpected: {ex.Message}"),
    //             "[SUCCESS] Got value: 100"
    //         );

    //         // 2) Negative success -20
    //         Add(
    //             Task.FromResult((Result<int>)(-20)),
    //             val => throw new MatchCallbackException($"Success => negative: {val}"),
    //             ex => throw new MatchCallbackException($"Failure => {ex.Message}"),
    //             "Success => negative: -20"
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(Match_TInput_should_InvokeSuccessAction_Data))]
    // public async Task Match_TInput_should_InvokeSuccessAction(
    //     Task<Result<int>> input,
    //     Action<int> successAction,
    //     Action<Exception> failureAction,
    //     string expectedMessage
    // )
    // {
    //     // Act
    //     async Task Act()
    //     {
    //         // We expect the success action to throw MatchCallbackException
    //         await input.Match(successAction, failureAction);
    //     }

    //     // Assert
    //     var ex = await Assert.ThrowsAsync<MatchCallbackException>(Act);
    //     ex.Message.Should().Be(expectedMessage);
    // }

    // // ----------------------------
    // //    FAILURE SCENARIOS
    // // ----------------------------

    // private class Match_TInput_should_InvokeFailureAction_Data
    //     : TheoryData<
    //         Task<Result<int>>, // input Task<Result<int>>
    //         Action<int>, // success action
    //         Action<Exception>, // failure action
    //         string // expected exception message
    //     >
    // {
    //     public Match_TInput_should_InvokeFailureAction_Data()
    //     {
    //         // 1) ArgumentNullException
    //         Add(
    //             Task.FromResult((Result<int>)new NullReferenceException("missing param")),
    //             val => throw new MatchCallbackException($"Should not see success => {val}"),
    //             ex => throw new MatchCallbackException($"FAIL => {ex.Message}"),
    //             "FAIL => missing param"
    //         );

    //         // 2) InvalidOperationException
    //         Add(
    //             Task.FromResult((Result<int>)new InvalidOperationException("Operation not valid")),
    //             val => throw new MatchCallbackException($"Success? {val}"),
    //             ex => throw new MatchCallbackException($"Failure captured => {ex.Message}"),
    //             "Failure captured => Operation not valid"
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(Match_TInput_should_InvokeFailureAction_Data))]
    // public async Task Match_TInput_should_InvokeFailureAction(
    //     Task<Result<int>> input,
    //     Action<int> successAction,
    //     Action<Exception> failureAction,
    //     string expectedMessage
    // )
    // {
    //     // Act
    //     async Task Act()
    //     {
    //         // We expect the failure action to throw MatchCallbackException
    //         await input.Match(successAction, failureAction);
    //     }

    //     // Assert
    //     var ex = await Assert.ThrowsAsync<MatchCallbackException>(Act);
    //     ex.Message.Should().Be(expectedMessage);
    // }

    // // =================================================
    // //  5) DoAwait<TSucc, TResult>(
    // //    DoType type,
    // //    Func<TSucc, Task<Result<TResult>>> function
    // //  )
    // // =================================================

    // // -----------------------------
    // //     SUCCESS SCENARIOS
    // // -----------------------------
    // private class DoAwait_OverloadA_Success_Data
    //     : TheoryData<Task<Result<int>>, DoType, Func<int, Task<Result<string>>>, Result<int>>
    // {
    //     public DoAwait_OverloadA_Success_Data()
    //     {
    //         // 1) Simple success w/MapErrors
    //         Add(
    //             Task.FromResult((Result<int>)42),
    //             DoType.MapErrors,
    //             async val => await Task.FromResult((Result<string>)$"Transformed:{val}"),
    //             42
    //         );

    //         // 2) Another success w/Ignore
    //         Add(
    //             Task.FromResult((Result<int>)100),
    //             DoType.Ignore,
    //             async val => await Task.FromResult((Result<string>)$"Value:{val}"),
    //             100
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(DoAwait_OverloadA_Success_Data))]
    // public async Task DoAwait_OverloadA_should_ReturnEquivalentSuccess(
    //     Task<Result<int>> input,
    //     DoType doType,
    //     Func<int, Task<Result<string>>> transformFunc,
    //     Result<int> expected
    // )
    // {
    //     // Act
    //     var actual = await input.DoAwait(doType, transformFunc);

    //     // Assert
    //     // For success, we expect the entire Result<string> to match
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // -----------------------------
    // //     FAILURE SCENARIOS
    // // -----------------------------
    // private class DoAwait_OverloadA_Failure_Data
    //     : TheoryData<Task<Result<int>>, DoType, Func<int, Task<Result<string>>>, Result<int>>
    // {
    //     public DoAwait_OverloadA_Failure_Data()
    //     {
    //         // 1) Already failing w/MapErrors
    //         var ex1 = new InvalidOperationException("FailCase1");
    //         Add(
    //             Task.FromResult((Result<int>)ex1),
    //             DoType.MapErrors,
    //             val => Task.FromResult((Result<string>)"ShouldNotHappen"),
    //             ex1
    //         );

    //         // 2) Another failing w/Ignore
    //         var ex2 = new ArgumentException("FailCase2");
    //         Add(
    //             Task.FromResult((Result<int>)ex2),
    //             DoType.Ignore,
    //             val => Task.FromResult((Result<string>)"NotCalled"),
    //             ex2
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(DoAwait_OverloadA_Failure_Data))]
    // public async Task DoAwait_OverloadA_should_ReturnEquivalentFailure(
    //     Task<Result<int>> input,
    //     DoType doType,
    //     Func<int, Task<Result<string>>> transformFunc,
    //     Result<int> expected
    // )
    // {
    //     // Act
    //     var actual = await input.DoAwait(doType, transformFunc);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // =============================
    // // 6) DoAwait<TSucc> Tests
    // // =============================

    // // ---------------------------------
    // // Successful Execution with Ignore
    // // ---------------------------------

    // private class DoAwait_TSucc_Should_ReturnSuccess_Ignore_Data
    //     : TheoryData<Task<Result<int>>, DoType, Func<int, Task>, Result<int>>
    // {
    //     public DoAwait_TSucc_Should_ReturnSuccess_Ignore_Data()
    //     {
    //         // Test Case 1: Simple no-operation task
    //         Add(
    //             Task.FromResult((Result<int>)42),
    //             DoType.Ignore,
    //             async _ =>
    //             {
    //                 await Task.Yield(); // Yields control back, effectively a no-op
    //             },
    //             42 // Expected success value
    //         );

    //         // Test Case 2: Task that simply delays execution
    //         Add(
    //             Task.FromResult((Result<int>)99),
    //             DoType.Ignore,
    //             async _ => await Task.Delay(10), // Delays for 10ms, no impact on value
    //             99 // Expected success value
    //         );
    //     }
    // }

    // // Test for successful execution with DoType.Ignore
    // [Theory]
    // [ClassData(typeof(DoAwait_TSucc_Should_ReturnSuccess_Ignore_Data))]
    // public async Task DoAwait_TSucc_Should_ReturnSuccess_Ignore(
    //     Task<Result<int>> subject,
    //     DoType doType,
    //     Func<int, Task> action,
    //     Result<int> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.DoAwait(doType, action, Errors.MapNone);

    //     // Assert
    //     actual
    //         .Should()
    //         .BeEquivalentTo(expected, "the input result should directly map to expected");
    // }

    // // --------------------------------------
    // // Failure Capture with MapErrors
    // // --------------------------------------

    // private class DoAwait_TSucc_Should_CaptureFailure_MapErrors_Data
    //     : TheoryData<Task<Result<int>>, DoType, Func<int, Task>, Result<int>>
    // {
    //     public DoAwait_TSucc_Should_CaptureFailure_MapErrors_Data()
    //     {
    //         // Failure Case 1: Throws and maps InvalidOperationException
    //         Add(
    //             Task.FromResult((Result<int>)42),
    //             DoType.MapErrors,
    //             async _ =>
    //             {
    //                 await Task.Yield();
    //                 throw new InvalidOperationException("Operation exception");
    //             },
    //             new InvalidOperationException("Operation exception")
    //         );

    //         // Failure Case 2: Throws and maps ArgumentException
    //         Add(
    //             Task.FromResult((Result<int>)99),
    //             DoType.MapErrors,
    //             async _ =>
    //             {
    //                 await Task.Yield();
    //                 throw new ArgumentException("Arg exception");
    //             }, // Exception to map
    //             new ArgumentException("Arg exception") // Expected mapped exception
    //         );
    //     }
    // }

    // // Test for capturing exceptions as failure with DoType.MapErrors
    // [Theory]
    // [ClassData(typeof(DoAwait_TSucc_Should_CaptureFailure_MapErrors_Data))]
    // public async Task DoAwait_TSucc_Should_CaptureFailure_MapErrors(
    //     Task<Result<int>> subject,
    //     DoType doType,
    //     Func<int, Task> action,
    //     Result<int> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.DoAwait(doType, action, Errors.MapAll);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // -------------------------------
    // // Exception Propagation
    // // -------------------------------

    // // Test for ensuring an unmapped exception propagates and is not captured
    // private class DoAwait_TSucc_Should_PropagateUnmappedException_Data
    //     : TheoryData<Task<Result<int>>, DoType, Func<int, Task>, Exception>
    // {
    //     public DoAwait_TSucc_Should_PropagateUnmappedException_Data()
    //     {
    //         // Propagation Case 1: FormatException
    //         Add(
    //             Task.FromResult((Result<int>)123),
    //             DoType.MapErrors,
    //             async _ =>
    //             {
    //                 await Task.Yield();
    //                 throw new FormatException("Unhandled FormatException");
    //             },
    //             new FormatException("Unhandled FormatException")
    //         );

    //         // Propagation Case 2: InvalidOperationException
    //         Add(
    //             Task.FromResult((Result<int>)456),
    //             DoType.MapErrors,
    //             async _ =>
    //             {
    //                 await Task.Yield();
    //                 throw new InvalidOperationException("Unmapped InvalidOperationException");
    //             },
    //             new InvalidOperationException("Unmapped InvalidOperationException")
    //         );
    //     }
    // }

    // // Test ensures an unmapped exception propagates
    // [Theory]
    // [ClassData(typeof(DoAwait_TSucc_Should_PropagateUnmappedException_Data))]
    // public async Task DoAwait_TSucc_Should_PropagateUnmappedException(
    //     Task<Result<int>> subject,
    //     DoType doType,
    //     Func<int, Task> action,
    //     Exception expected
    // )
    // {
    //     // Act
    //     var act = async () => await subject.DoAwait(doType, action, Errors.MapNone);

    //     // Assert
    //     // Expect the original exception to propagate
    //     await act.Should()
    //         .ThrowAsync<Exception>()
    //         .WithMessage(expected.Message)
    //         .Where(x => x.GetType() == expected.GetType());
    // }

    // // ==================================
    // // 7) DoAwait<TSucc> Overload Tests
    // // ==================================

    // // ---------------------------------
    // // Successful Execution with DoAwait
    // // ---------------------------------

    // private class DoAwait_TSucc_Overload_Should_ReturnSuccess_Data
    //     : TheoryData<Task<Result<int>>, DoType, Func<int, Task<string>>, Result<int>>
    // {
    //     public DoAwait_TSucc_Overload_Should_ReturnSuccess_Data()
    //     {
    //         // Test Case 1: Return a string asynchronously
    //         Add(
    //             Task.FromResult((Result<int>)50),
    //             DoType.Ignore,
    //             async val => await Task.FromResult($"Value: {val}"),
    //             50
    //         );

    //         // Test Case 2: Returning an async transformed message
    //         Add(
    //             Task.FromResult((Result<int>)(int)-25),
    //             DoType.Ignore,
    //             async val =>
    //                 val < 0
    //                     ? await Task.FromResult("Negative number")
    //                     : await Task.FromResult("Positive number"),
    //             -25
    //         );
    //     }
    // }

    // // Test for successful async execution
    // [Theory]
    // [ClassData(typeof(DoAwait_TSucc_Overload_Should_ReturnSuccess_Data))]
    // public async Task DoAwait_TSucc_Overload_Should_ReturnSuccess(
    //     Task<Result<int>> subject,
    //     DoType doType,
    //     Func<int, Task<string>> asyncFunc,
    //     Result<int> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.DoAwait(doType, asyncFunc, Errors.MapNone);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // ---------------------------------
    // // Handled Failures with DoAwait
    // // ---------------------------------

    // private class DoAwait_TSucc_Overload_Should_HandleFailure_Data
    //     : TheoryData<Task<Result<int>>, DoType, Func<int, Task<string>>, Result<int>>
    // {
    //     public DoAwait_TSucc_Overload_Should_HandleFailure_Data()
    //     {
    //         // Failure Case 1: Handle InvalidOperationException
    //         Add(
    //             Task.FromResult((Result<int>)20),
    //             DoType.MapErrors,
    //             async val =>
    //             {
    //                 await Task.Yield();
    //                 throw new InvalidOperationException("Error during execution");
    //             },
    //             new InvalidOperationException("Error during execution")
    //         );

    //         // Failure Case 2: Another exception type handled
    //         Add(
    //             Task.FromResult((Result<int>)0),
    //             DoType.MapErrors,
    //             async val =>
    //             {
    //                 await Task.Delay(5);
    //                 throw new NullReferenceException("Out of range during execution");
    //             },
    //             new NullReferenceException("Out of range during execution")
    //         );
    //     }
    // }

    // // Test for handled failures with MapErrors
    // [Theory]
    // [ClassData(typeof(DoAwait_TSucc_Overload_Should_HandleFailure_Data))]
    // public async Task DoAwait_TSucc_Overload_Should_HandleFailure(
    //     Task<Result<int>> subject,
    //     DoType doType,
    //     Func<int, Task<string>> asyncFunc,
    //     Result<int> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.DoAwait(doType, asyncFunc, Errors.MapAll);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // -------------------------------
    // // Unmapped Exception Propagation
    // // -------------------------------

    // // Test data for exception propagation
    // private class DoAwait_TSucc_Overload_Should_PropagateUnmappedException_Data
    //     : TheoryData<Task<Result<int>>, DoType, Func<int, Task<string>>, Exception>
    // {
    //     public DoAwait_TSucc_Overload_Should_PropagateUnmappedException_Data()
    //     {
    //         // Propagation of FormatException
    //         Add(
    //             Task.FromResult((Result<int>)100),
    //             DoType.Ignore,
    //             async val =>
    //             {
    //                 await Task.Yield();
    //                 throw new FormatException("Unhandled format exception");
    //             },
    //             new FormatException("Unhandled format exception")
    //         );

    //         // Propagation of IndexOutOfRangeException
    //         Add(
    //             Task.FromResult((Result<int>)200),
    //             DoType.Ignore,
    //             async val =>
    //             {
    //                 await Task.Yield();
    //                 throw new IndexOutOfRangeException("Index out of range");
    //             },
    //             new IndexOutOfRangeException("Index out of range")
    //         );
    //     }
    // }

    // // Test ensuring that exceptions that are not mapped are propagated
    // [Theory]
    // [ClassData(typeof(DoAwait_TSucc_Overload_Should_PropagateUnmappedException_Data))]
    // public async Task DoAwait_TSucc_Overload_Should_PropagateUnmappedException(
    //     Task<Result<int>> subject,
    //     DoType doType,
    //     Func<int, Task<string>> asyncFunc,
    //     Exception expected
    // )
    // {
    //     // Act
    //     var act = async () => await subject.DoAwait(doType, asyncFunc, Errors.MapNone);

    //     // Assert
    //     await act.Should()
    //         .ThrowAsync<Exception>()
    //         .WithMessage(expected.Message)
    //         .Where(x => x.GetType() == expected.GetType());
    // }

    // // ====================================
    // // 8) ThenAwait<TSucc, TResult> Tests
    // // ====================================

    // // ---------------------------------
    // // Successful Execution with ThenAwait
    // // ---------------------------------

    // private class ThenAwait_Should_ReturnSuccess_Data
    //     : TheoryData<Task<Result<int>>, Func<int, Task<Result<string>>>, Result<string>>
    // {
    //     public ThenAwait_Should_ReturnSuccess_Data()
    //     {
    //         // Test Case 1: Async transformation to uppercase string
    //         Add(
    //             Task.FromResult((Result<int>)1),
    //             async val => await Task.FromResult((Result<string>)$"ONE_{val}"),
    //             "ONE_1"
    //         );

    //         // Test Case 2: Async multiplication
    //         Add(
    //             Task.FromResult((Result<int>)2),
    //             async val => await Task.FromResult((Result<string>)$"TWO_{val * 10}"),
    //             "TWO_20"
    //         );
    //     }
    // }

    // // Test for successful async transformations
    // [Theory]
    // [ClassData(typeof(ThenAwait_Should_ReturnSuccess_Data))]
    // public async Task ThenAwait_Should_ReturnSuccess(
    //     Task<Result<int>> subject,
    //     Func<int, Task<Result<string>>> asyncFunc,
    //     Result<string> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.ThenAwait(asyncFunc);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // ---------------------------------
    // // Handled Failures with ThenAwait
    // // ---------------------------------

    // private class ThenAwait_Should_HandleFailure_Data
    //     : TheoryData<Task<Result<int>>, Func<int, Task<Result<string>>>, Result<string>>
    // {
    //     public ThenAwait_Should_HandleFailure_Data()
    //     {
    //         // Failure Case 1: Async function throws ArgumentNullException
    //         Add(
    //             Task.FromResult((Result<int>)3),
    //             async val =>
    //             {
    //                 await Task.Yield();
    //                 return new NullReferenceException("Argument is null");
    //             },
    //             new NullReferenceException("Argument is null")
    //         );

    //         // Failure Case 2: Another exception type handled
    //         Add(
    //             Task.FromResult((Result<int>)5),
    //             async val =>
    //             {
    //                 await Task.Delay(5);
    //                 return new FormatException("Bad format while processing");
    //             },
    //             new FormatException("Bad format while processing")
    //         );
    //     }
    // }

    // // Test for handled failures
    // [Theory]
    // [ClassData(typeof(ThenAwait_Should_HandleFailure_Data))]
    // public async Task ThenAwait_Should_HandleFailure(
    //     Task<Result<int>> subject,
    //     Func<int, Task<Result<string>>> asyncFunc,
    //     Result<string> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.ThenAwait(asyncFunc);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // -------------------------------
    // // Unmapped Exception Propagation
    // // -------------------------------

    // // Test data for exception propagation
    // private class ThenAwait_Should_PropagateUnmappedException_Data
    //     : TheoryData<Task<Result<int>>, Func<int, Task<Result<string>>>, Exception>
    // {
    //     public ThenAwait_Should_PropagateUnmappedException_Data()
    //     {
    //         // Propagation of InvalidOperationException
    //         Add(
    //             Task.FromResult((Result<int>)8),
    //             async val =>
    //             {
    //                 await Task.Yield();
    //                 throw new InvalidOperationException("Unhandled operation");
    //             },
    //             new InvalidOperationException("Unhandled operation")
    //         );

    //         // Propagation of IndexOutOfRangeException
    //         Add(
    //             Task.FromResult((Result<int>)12),
    //             async val =>
    //             {
    //                 await Task.Yield();
    //                 throw new IndexOutOfRangeException("Out of index");
    //             },
    //             new IndexOutOfRangeException("Out of index")
    //         );
    //     }
    // }

    // // Test ensuring exceptions that are not mapped are propagated
    // [Theory]
    // [ClassData(typeof(ThenAwait_Should_PropagateUnmappedException_Data))]
    // public async Task ThenAwait_Should_PropagateUnmappedException(
    //     Task<Result<int>> subject,
    //     Func<int, Task<Result<string>>> asyncFunc,
    //     Exception expected
    // )
    // {
    //     // Act
    //     var act = async () => await subject.ThenAwait(asyncFunc);

    //     // Assert
    //     await act.Should()
    //         .ThrowAsync<Exception>()
    //         .WithMessage(expected.Message)
    //         .Where(x => x.GetType() == expected.GetType());
    // }

    // // ==========================================================
    // // 9) ThenAwait<TSucc, TResult> with ExceptionFilter Tests
    // // ==========================================================

    // // ---------------------------------
    // // Successful Execution with ThenAwait
    // // ---------------------------------

    // private class ThenAwait_WithExceptionFilter_Should_ReturnSuccess_Data
    //     : TheoryData<Task<Result<int>>, Func<int, Task<string>>, Result<string>>
    // {
    //     public ThenAwait_WithExceptionFilter_Should_ReturnSuccess_Data()
    //     {
    //         // Test Case 1: Successful async conversion of int to string
    //         Add(
    //             Task.FromResult((Result<int>)15),
    //             async val => await Task.FromResult($"Value is {val}"),
    //             "Value is 15"
    //         );

    //         // Test Case 2: Another successful async operation
    //         Add(
    //             Task.FromResult((Result<int>)30),
    //             async val => await Task.FromResult($"Converted to {val * 2}"),
    //             "Converted to 60"
    //         );
    //     }
    // }

    // // Test for ensuring success with async function transformation
    // [Theory]
    // [ClassData(typeof(ThenAwait_WithExceptionFilter_Should_ReturnSuccess_Data))]
    // public async Task ThenAwait_WithExceptionFilter_Should_ReturnSuccess(
    //     Task<Result<int>> subject,
    //     Func<int, Task<string>> function,
    //     Result<string> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.ThenAwait(function, Errors.MapNone);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // ---------------------------------
    // // Handled Failures with ThenAwait
    // // ---------------------------------

    // private class ThenAwait_WithExceptionFilter_Should_HandleFailure_Data
    //     : TheoryData<Task<Result<int>>, Func<int, Task<string>>, Result<string>>
    // {
    //     public ThenAwait_WithExceptionFilter_Should_HandleFailure_Data()
    //     {
    //         // Failure Case 1: Handles an InvalidOperationException
    //         Add(
    //             Task.FromResult((Result<int>)404),
    //             async val =>
    //             {
    //                 await Task.Yield();
    //                 throw new InvalidOperationException("Operation failed during processing");
    //             },
    //             new InvalidOperationException("Operation failed during processing")
    //         );

    //         // Failure Case 2: ArgumentOutOfRangeException handled
    //         Add(
    //             Task.FromResult((Result<int>)202),
    //             async val =>
    //             {
    //                 await Task.Delay(10);
    //                 throw new NullReferenceException("Out of range error");
    //             },
    //             new NullReferenceException("Out of range error")
    //         );
    //     }
    // }

    // // Test for handled failures: exceptions should be captured
    // [Theory]
    // [ClassData(typeof(ThenAwait_WithExceptionFilter_Should_HandleFailure_Data))]
    // public async Task ThenAwait_WithExceptionFilter_Should_HandleFailure(
    //     Task<Result<int>> subject,
    //     Func<int, Task<string>> function,
    //     Result<string> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.ThenAwait(function, Errors.MapAll);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // -------------------------------
    // // Unmapped Exception Propagation
    // // -------------------------------

    // // Exception propagation test data
    // private class ThenAwait_WithExceptionFilter_Should_PropagateUnmappedException_Data
    //     : TheoryData<Task<Result<int>>, Func<int, Task<string>>, Exception>
    // {
    //     public ThenAwait_WithExceptionFilter_Should_PropagateUnmappedException_Data()
    //     {
    //         // Propagation Case 1: FormatException
    //         Add(
    //             Task.FromResult((Result<int>)999),
    //             async val =>
    //             {
    //                 await Task.Yield();
    //                 throw new FormatException("Format misalignment detected");
    //             },
    //             new FormatException("Format misalignment detected")
    //         );

    //         // Propagation Case 2: IndexOutOfRangeException
    //         Add(
    //             Task.FromResult((Result<int>)888),
    //             async val =>
    //             {
    //                 await Task.Yield();
    //                 throw new IndexOutOfRangeException("Array access error");
    //             },
    //             new IndexOutOfRangeException("Array access error")
    //         );
    //     }
    // }

    // // Test for checking propagation of unmapped exceptions
    // [Theory]
    // [ClassData(typeof(ThenAwait_WithExceptionFilter_Should_PropagateUnmappedException_Data))]
    // public async Task ThenAwait_WithExceptionFilter_Should_PropagateUnmappedException(
    //     Task<Result<int>> subject,
    //     Func<int, Task<string>> function,
    //     Exception expected
    // )
    // {
    //     // Act
    //     var act = async () => await subject.ThenAwait(function, Errors.MapNone);

    //     // Assert
    //     await act.Should()
    //         .ThrowAsync<Exception>()
    //         .WithMessage(expected.Message)
    //         .Where(x => x.GetType() == expected.GetType());
    // }

    // // ==================================
    // // 10) ThenAwait<TSucc> Method Tests
    // // Returning Result<Unit>
    // // ==================================

    // // ---------------------------------
    // // Successful Execution with ThenAwait
    // // ---------------------------------

    // private class ThenAwait_WithExceptionFilter_Should_SucceedForVoid_Data
    //     : TheoryData<Task<Result<int>>, Func<int, Task>, Result<Unit>>
    // {
    //     public ThenAwait_WithExceptionFilter_Should_SucceedForVoid_Data()
    //     {
    //         // Test Case 1: Executes task without throwing
    //         Add(
    //             Task.FromResult((Result<int>)10),
    //             async val => await Task.CompletedTask,
    //             new Unit() // Expected result
    //         );

    //         // Test Case 2: Performs a simple delay
    //         Add(Task.FromResult((Result<int>)20), async val => await Task.Delay(5), new Unit());
    //     }
    // }

    // // Test for ensuring success with async function when returning Unit
    // [Theory]
    // [ClassData(typeof(ThenAwait_WithExceptionFilter_Should_SucceedForVoid_Data))]
    // public async Task ThenAwait_WithExceptionFilter_Should_SucceedForVoid(
    //     Task<Result<int>> subject,
    //     Func<int, Task> function,
    //     Result<Unit> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.ThenAwait(function, Errors.MapNone);

    //     // Assert
    //     actual.Should().Be(expected);
    // }

    // // ---------------------------------
    // // Handled Failures with ThenAwait
    // // ---------------------------------

    // private class ThenAwait_WithExceptionFilter_Should_CaptureFailureForVoid_Data
    //     : TheoryData<Task<Result<int>>, Func<int, Task>, Result<Unit>>
    // {
    //     public ThenAwait_WithExceptionFilter_Should_CaptureFailureForVoid_Data()
    //     {
    //         // Failure Case 1: Handling InvalidOperationException
    //         Add(
    //             Task.FromResult((Result<int>)5),
    //             async val =>
    //             {
    //                 await Task.Yield();
    //                 throw new InvalidOperationException("Operation failed");
    //             },
    //             new InvalidOperationException("Operation failed")
    //         );

    //         // Failure Case 2: Capturing IndexOutOfRangeException
    //         Add(
    //             Task.FromResult((Result<int>)8),
    //             async val =>
    //             {
    //                 await Task.Delay(1);
    //                 throw new IndexOutOfRangeException("Index out of range");
    //             },
    //             new IndexOutOfRangeException("Index out of range")
    //         );
    //     }
    // }

    // // Test for handled failures: exceptions should be captured
    // [Theory]
    // [ClassData(typeof(ThenAwait_WithExceptionFilter_Should_CaptureFailureForVoid_Data))]
    // public async Task ThenAwait_WithExceptionFilter_Should_CaptureFailureForVoid(
    //     Task<Result<int>> subject,
    //     Func<int, Task> function,
    //     Result<Unit> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.ThenAwait(function, Errors.MapAll);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // -------------------------------
    // // Unmapped Exception Propagation
    // // -------------------------------

    // // Exception propagation test data
    // private class ThenAwait_WithExceptionFilter_Should_PropagateUnmappedExceptionForVoid_Data
    //     : TheoryData<Task<Result<int>>, Func<int, Task>, Exception>
    // {
    //     public ThenAwait_WithExceptionFilter_Should_PropagateUnmappedExceptionForVoid_Data()
    //     {
    //         // Propagation Case 1: FormatException
    //         Add(
    //             Task.FromResult((Result<int>)12),
    //             async val =>
    //             {
    //                 await Task.Yield();
    //                 throw new FormatException("Unmapped format exception");
    //             },
    //             new FormatException("Unmapped format exception")
    //         );

    //         // Propagation Case 2: ArgumentException
    //         Add(
    //             Task.FromResult((Result<int>)18),
    //             async val =>
    //             {
    //                 await Task.Yield();
    //                 throw new ArgumentException("Unlisted argument error");
    //             },
    //             new ArgumentException("Unlisted argument error")
    //         );
    //     }
    // }

    // // Test ensuring exceptions that are not mapped are propagated
    // [Theory]
    // [ClassData(typeof(ThenAwait_WithExceptionFilter_Should_PropagateUnmappedExceptionForVoid_Data))]
    // public async Task ThenAwait_WithExceptionFilter_Should_PropagateUnmappedExceptionForVoid(
    //     Task<Result<int>> subject,
    //     Func<int, Task> function,
    //     Exception expected
    // )
    // {
    //     // Act
    //     var act = async () => await subject.ThenAwait(function, Errors.MapNone);

    //     // Assert
    //     await act.Should()
    //         .ThrowAsync<Exception>()
    //         .WithMessage(expected.Message)
    //         .Where(x => x.GetType() == expected.GetType());
    // }

    // // ======================================
    // // 11) Then<TSucc, TResult> Method Tests
    // // ======================================

    // // ---------------------------------
    // // Successful Transformation with Then
    // // ---------------------------------

    // private class Then_OverloadA_Should_ReturnSuccess_Data
    //     : TheoryData<Task<Result<int>>, Func<int, Result<string>>, Result<string>>
    // {
    //     public Then_OverloadA_Should_ReturnSuccess_Data()
    //     {
    //         // Test Case 1: Simple transformation to string
    //         Add(
    //             Task.FromResult((Result<int>)5),
    //             val => (Result<string>)$"Value is {val}",
    //             "Value is 5"
    //         );

    //         // Test Case 2: Doubling and converting to string
    //         Add(
    //             Task.FromResult((Result<int>)7),
    //             val => (Result<string>)$"Double {val * 2}",
    //             "Double 14"
    //         );
    //     }
    // }

    // // Test for successful transformation
    // [Theory]
    // [ClassData(typeof(Then_OverloadA_Should_ReturnSuccess_Data))]
    // public async Task Then_OverloadA_Should_ReturnSuccess(
    //     Task<Result<int>> subject,
    //     Func<int, Result<string>> function,
    //     Result<string> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.Then(function);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // ---------------------------------
    // // Handled Failures with Then
    // // ---------------------------------

    // private class Then_OverloadA_Should_HandleFailure_Data
    //     : TheoryData<Task<Result<int>>, Func<int, Result<string>>, Result<string>>
    // {
    //     public Then_OverloadA_Should_HandleFailure_Data()
    //     {
    //         // Failure Case 1: Function throws InvalidOperationException
    //         Add(
    //             Task.FromResult((Result<int>)3),
    //             val => new InvalidOperationException("Invalid operation encountered"),
    //             new InvalidOperationException("Invalid operation encountered")
    //         );

    //         // Failure Case 2: Function throws ArgumentOutOfRangeException
    //         Add(
    //             Task.FromResult((Result<int>)4),
    //             val => new NullReferenceException("Range is incorrect"),
    //             new NullReferenceException("Range is incorrect")
    //         );
    //     }
    // }

    // // Test for scenarios where failures are handled appropriately
    // [Theory]
    // [ClassData(typeof(Then_OverloadA_Should_HandleFailure_Data))]
    // public async Task Then_OverloadA_Should_HandleFailure(
    //     Task<Result<int>> subject,
    //     Func<int, Result<string>> function,
    //     Result<string> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.Then(function);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // -------------------------------
    // // Unmapped Exception Propagation
    // // -------------------------------

    // // Test data for exception propagation
    // private class Then_OverloadA_Should_PropagateUnmappedException_Data
    //     : TheoryData<Task<Result<int>>, Func<int, Result<string>>, Exception>
    // {
    //     public Then_OverloadA_Should_PropagateUnmappedException_Data()
    //     {
    //         // Propagation Case 1: Unhandled FormatException
    //         Add(
    //             Task.FromResult((Result<int>)8),
    //             val => throw new FormatException("Invalid format detected"),
    //             new FormatException("Invalid format detected")
    //         );

    //         // Propagation Case 2: Unhandled ArgumentNullException
    //         Add(
    //             Task.FromResult((Result<int>)10),
    //             val => throw new ArgumentNullException("Null argument found"),
    //             new ArgumentNullException("Null argument found")
    //         );
    //     }
    // }

    // // Test ensuring exceptions that are not handled are propagated
    // [Theory]
    // [ClassData(typeof(Then_OverloadA_Should_PropagateUnmappedException_Data))]
    // public async Task Then_OverloadA_Should_PropagateUnmappedException(
    //     Task<Result<int>> subject,
    //     Func<int, Result<string>> function,
    //     Exception expected
    // )
    // {
    //     // Act
    //     var act = async () => await subject.Then(function);

    //     // Assert
    //     await act.Should()
    //         .ThrowAsync<Exception>()
    //         .WithMessage(expected.Message)
    //         .Where(x => x.GetType() == expected.GetType());
    // }

    // // =======================================
    // //  12) Then<TSucc, TResult> Method Tests
    // // =======================================

    // // ---------------------------------
    // // Successful Transformation with Then
    // // ---------------------------------

    // private class Then_OverloadB_Should_ReturnSuccess_Data
    //     : TheoryData<Task<Result<int>>, Func<int, Result<string>>, Result<string>>
    // {
    //     public Then_OverloadB_Should_ReturnSuccess_Data()
    //     {
    //         // Test Case 1: Simple transformation to string
    //         Add(
    //             Task.FromResult((Result<int>)5),
    //             val => (Result<string>)$"Value is {val}",
    //             "Value is 5"
    //         );

    //         // Test Case 2: Doubling and converting to string
    //         Add(
    //             Task.FromResult((Result<int>)7),
    //             val => (Result<string>)$"Double {val * 2}",
    //             "Double 14"
    //         );
    //     }
    // }

    // // Test for successful transformation
    // [Theory]
    // [ClassData(typeof(Then_OverloadB_Should_ReturnSuccess_Data))]
    // public async Task Then_OverloadB_Should_ReturnSuccess(
    //     Task<Result<int>> subject,
    //     Func<int, Result<string>> function,
    //     Result<string> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.Then(function);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // ---------------------------------
    // // Handled Failures with Then
    // // ---------------------------------

    // private class Then_OverloadB_Should_HandleFailure_Data
    //     : TheoryData<Task<Result<int>>, Func<int, Result<string>>, Result<string>>
    // {
    //     public Then_OverloadB_Should_HandleFailure_Data()
    //     {
    //         // Failure Case 1: Function throws InvalidOperationException
    //         Add(
    //             Task.FromResult((Result<int>)3),
    //             val => new InvalidOperationException("Invalid operation encountered"),
    //             new InvalidOperationException("Invalid operation encountered")
    //         );

    //         // Failure Case 2: Function throws ArgumentOutOfRangeException
    //         Add(
    //             Task.FromResult((Result<int>)4),
    //             val => new NullReferenceException("Range is incorrect"),
    //             new NullReferenceException("Range is incorrect")
    //         );
    //     }
    // }

    // // Test for scenarios where failures are handled appropriately
    // [Theory]
    // [ClassData(typeof(Then_OverloadB_Should_HandleFailure_Data))]
    // public async Task Then_OverloadB_Should_HandleFailure(
    //     Task<Result<int>> subject,
    //     Func<int, Result<string>> function,
    //     Result<string> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.Then(function);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // -------------------------------
    // // Unmapped Exception Propagation
    // // -------------------------------

    // // Test data for exception propagation
    // private class Then_OverloadB_Should_PropagateUnmappedException_Data
    //     : TheoryData<Task<Result<int>>, Func<int, Result<string>>, Exception>
    // {
    //     public Then_OverloadB_Should_PropagateUnmappedException_Data()
    //     {
    //         // Propagation Case 1: Unhandled FormatException
    //         Add(
    //             Task.FromResult((Result<int>)8),
    //             val => throw new FormatException("Invalid format detected"),
    //             new FormatException("Invalid format detected")
    //         );

    //         // Propagation Case 2: Unhandled ArgumentNullException
    //         Add(
    //             Task.FromResult((Result<int>)10),
    //             val => throw new ArgumentNullException("Null argument found"),
    //             new ArgumentNullException("Null argument found")
    //         );
    //     }
    // }

    // // Test ensuring exceptions that are not handled are propagated
    // [Theory]
    // [ClassData(typeof(Then_OverloadB_Should_PropagateUnmappedException_Data))]
    // public async Task Then_OverloadB_Should_PropagateUnmappedException(
    //     Task<Result<int>> subject,
    //     Func<int, Result<string>> function,
    //     Exception expected
    // )
    // {
    //     // Act
    //     var act = async () => await subject.Then(function);

    //     // Assert
    //     await act.Should()
    //         .ThrowAsync<Exception>()
    //         .WithMessage(expected.Message)
    //         .Where(x => x.GetType() == expected.GetType());
    // }

    // // =============================
    // // 13) Then<TSucc> Method Tests
    // // Returning Result<Unit>
    // // =============================

    // // ---------------------------------
    // // Successful Execution with Then
    // // ---------------------------------

    // private class Then_OverloadC_WithExceptionFilter_Should_SucceedForVoid_Data
    //     : TheoryData<Task<Result<int>>, Action<int>, Result<Unit>>
    // {
    //     public Then_OverloadC_WithExceptionFilter_Should_SucceedForVoid_Data()
    //     {
    //         // Test Case 1: No-operation action
    //         Add(
    //             Task.FromResult((Result<int>)10),
    //             val =>
    //             {
    //                 /* No-op */
    //             },
    //             new Unit() // Expected result
    //         );

    //         // Test Case 2: Simple logging
    //         Add(
    //             Task.FromResult((Result<int>)20),
    //             val => Console.WriteLine($"Logging {val}"),
    //             new Unit()
    //         );
    //     }
    // }

    // // Test for ensuring success with action execution returning Unit
    // [Theory]
    // [ClassData(typeof(Then_OverloadC_WithExceptionFilter_Should_SucceedForVoid_Data))]
    // public async Task Then_OverloadC_WithExceptionFilter_Should_SucceedForVoid(
    //     Task<Result<int>> subject,
    //     Action<int> function,
    //     Result<Unit> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.Then(function, Errors.MapNone);

    //     // Assert
    //     actual.Should().Be(expected);
    // }

    // // ---------------------------------
    // // Handled Failures with Then
    // // ---------------------------------

    // private class Then_OverloadC_WithExceptionFilter_Should_CaptureFailureForVoid_Data
    //     : TheoryData<Task<Result<int>>, Action<int>, Result<Unit>>
    // {
    //     public Then_OverloadC_WithExceptionFilter_Should_CaptureFailureForVoid_Data()
    //     {
    //         // Failure Case 1: Action throws InvalidOperationException
    //         Add(
    //             Task.FromResult((Result<int>)5),
    //             val => throw new InvalidOperationException("Operation failed"),
    //             new InvalidOperationException("Operation failed")
    //         );

    //         // Failure Case 2: Action throws IndexOutOfRangeException
    //         Add(
    //             Task.FromResult((Result<int>)8),
    //             val => throw new IndexOutOfRangeException("Index out of range"),
    //             new IndexOutOfRangeException("Index out of range")
    //         );
    //     }
    // }

    // // Test for handled failures: exceptions should be captured
    // [Theory]
    // [ClassData(typeof(Then_OverloadC_WithExceptionFilter_Should_CaptureFailureForVoid_Data))]
    // public async Task Then_OverloadC_WithExceptionFilter_Should_CaptureFailureForVoid(
    //     Task<Result<int>> subject,
    //     Action<int> function,
    //     Result<Unit> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.Then(function, Errors.MapAll);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // -------------------------------
    // // Unmapped Exception Propagation
    // // -------------------------------

    // // Exception propagation test data
    // private class Then_OverloadC_WithExceptionFilter_Should_PropagateUnmappedExceptionForVoid_Data
    //     : TheoryData<Task<Result<int>>, Action<int>, Exception>
    // {
    //     public Then_OverloadC_WithExceptionFilter_Should_PropagateUnmappedExceptionForVoid_Data()
    //     {
    //         // Propagation Case 1: FormatException
    //         Add(
    //             Task.FromResult((Result<int>)12),
    //             val => throw new FormatException("Unmapped format exception"),
    //             new FormatException("Unmapped format exception")
    //         );

    //         // Propagation Case 2: ArgumentException
    //         Add(
    //             Task.FromResult((Result<int>)18),
    //             val => throw new ArgumentException("Unlisted argument error"),
    //             new ArgumentException("Unlisted argument error")
    //         );
    //     }
    // }

    // // Test ensuring exceptions that are not mapped are propagated
    // [Theory]
    // [ClassData(
    //     typeof(Then_OverloadC_WithExceptionFilter_Should_PropagateUnmappedExceptionForVoid_Data)
    // )]
    // public async Task Then_OverloadC_WithExceptionFilter_Should_PropagateUnmappedExceptionForVoid(
    //     Task<Result<int>> subject,
    //     Action<int> function,
    //     Exception expected
    // )
    // {
    //     // Act
    //     var act = async () => await subject.Then(function, Errors.MapNone);

    //     // Assert
    //     await act.Should()
    //         .ThrowAsync<Exception>()
    //         .WithMessage(expected.Message)
    //         .Where(x => x.GetType() == expected.GetType());
    // }

    // // =============================
    // // 14) AssertAwait<TSucc> Method Tests
    // // =============================

    // // ---------------------------------
    // // Successful Assertion with AssertAwait
    // // ---------------------------------

    // private class AssertAwait_Should_PassAssertion_Data
    //     : TheoryData<Task<Result<int>>, Func<int, Task<Result<bool>>>, Result<int>>
    // {
    //     public AssertAwait_Should_PassAssertion_Data()
    //     {
    //         // Test Case 1: Simple assertion that always returns true
    //         Add(
    //             Task.FromResult((Result<int>)42),
    //             async val => await Task.FromResult((Result<bool>)true),
    //             42
    //         );

    //         // Test Case 2: Check if value is positive
    //         Add(
    //             Task.FromResult((Result<int>)100),
    //             async val => await Task.FromResult((Result<bool>)(val > 0)),
    //             100
    //         );
    //     }
    // }

    // // Test for assertions that pass successfully
    // [Theory]
    // [ClassData(typeof(AssertAwait_Should_PassAssertion_Data))]
    // public async Task AssertAwait_Should_PassAssertion(
    //     Task<Result<int>> subject,
    //     Func<int, Task<Result<bool>>> assertion,
    //     Result<int> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.AssertAwait(assertion);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // ---------------------------------
    // // Assertion Failure with AssertAwait
    // // ---------------------------------

    // private class AssertAwait_Should_ReturnAssertionFailure_Data
    //     : TheoryData<Task<Result<int>>, Func<int, Task<Result<bool>>>, string>
    // {
    //     public AssertAwait_Should_ReturnAssertionFailure_Data()
    //     {
    //         // Failure Case 1: Assert if value is zero
    //         Add(
    //             Task.FromResult((Result<int>)0),
    //             async val => await Task.FromResult((Result<bool>)(val != 0)),
    //             "Assertion returned false!"
    //         );

    //         // Failure Case 2: Assert that value is greater than 50
    //         Add(
    //             Task.FromResult((Result<int>)10),
    //             async val => await Task.FromResult((Result<bool>)(val > 50)),
    //             "Value should be greater than 50"
    //         );
    //     }
    // }

    // // Test for scenarios where assertions fail
    // [Theory]
    // [ClassData(typeof(AssertAwait_Should_ReturnAssertionFailure_Data))]
    // public async Task AssertAwait_Should_ReturnAssertionFailure(
    //     Task<Result<int>> subject,
    //     Func<int, Task<Result<bool>>> assertion,
    //     string assertionMessage
    // )
    // {
    //     // Act
    //     var actualResult = await subject.AssertAwait(assertion, assertionMessage);

    //     // Assert
    //     var actual = (Exception?)actualResult;
    //     actual.Should().NotBeNull();
    //     actual.Should().BeOfType<AssertionException>();
    //     actual.Message.Should().Be(assertionMessage);
    // }

    // // -------------------------------
    // // Exception in Assertion with AssertAwait
    // // -------------------------------

    // // Test data for exception propagation in assertion
    // private class AssertAwait_Should_PropagateExceptionInAssertion_Data
    //     : TheoryData<Task<Result<int>>, Func<int, Task<Result<bool>>>, Exception>
    // {
    //     public AssertAwait_Should_PropagateExceptionInAssertion_Data()
    //     {
    //         // Propagation Case 1: FormatException in assertion
    //         Add(
    //             Task.FromResult((Result<int>)7),
    //             async val =>
    //             {
    //                 await Task.Yield();
    //                 throw new FormatException("Assertion threw format exception");
    //             },
    //             new FormatException("Assertion threw format exception")
    //         );

    //         // Propagation Case 2: InvalidOperationException in assertion
    //         Add(
    //             Task.FromResult((Result<int>)15),
    //             async val =>
    //             {
    //                 await Task.Yield();
    //                 throw new InvalidOperationException("Invalid operation in assertion");
    //             },
    //             new InvalidOperationException("Invalid operation in assertion")
    //         );
    //     }
    // }

    // // Test that ensures exceptions within the assertion function are propagated
    // [Theory]
    // [ClassData(typeof(AssertAwait_Should_PropagateExceptionInAssertion_Data))]
    // public async Task AssertAwait_Should_PropagateExceptionInAssertion(
    //     Task<Result<int>> subject,
    //     Func<int, Task<Result<bool>>> assertion,
    //     Exception expected
    // )
    // {
    //     // Act
    //     var act = async () => await subject.AssertAwait(assertion);

    //     // Assert
    //     await act.Should()
    //         .ThrowAsync<Exception>()
    //         .WithMessage(expected.Message)
    //         .Where(x => x.GetType() == expected.GetType());
    // }

    // // ==========================================================
    // // 15) AssertAwait<TSucc> with ExceptionFilter Method Tests
    // // ==========================================================

    // // ---------------------------------
    // // Successful Assertion with AssertAwait
    // // ---------------------------------

    // private class AssertAwait_WithExceptionFilter_Should_PassAssertion_Data
    //     : TheoryData<Task<Result<int>>, Func<int, Task<bool>>, Result<int>>
    // {
    //     public AssertAwait_WithExceptionFilter_Should_PassAssertion_Data()
    //     {
    //         // Test Case 1: Trivial assertion always returning true
    //         Add(Task.FromResult((Result<int>)55), async val => await Task.FromResult(true), 55);

    //         // Test Case 2: Assert to check if value is positive
    //         Add(Task.FromResult((Result<int>)75), async val => await Task.FromResult(val > 0), 75);
    //     }
    // }

    // // Test for assertions that succeed, using exception filtering
    // [Theory]
    // [ClassData(typeof(AssertAwait_WithExceptionFilter_Should_PassAssertion_Data))]
    // public async Task AssertAwait_WithExceptionFilter_Should_PassAssertion(
    //     Task<Result<int>> subject,
    //     Func<int, Task<bool>> assertion,
    //     Result<int> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.AssertAwait(assertion, Errors.MapNone);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // ---------------------------------
    // // Assertion Failure with AssertAwait
    // // ---------------------------------

    // private class AssertAwait_WithExceptionFilter_Should_ReturnAssertionFailure_Data
    //     : TheoryData<Task<Result<int>>, Func<int, Task<bool>>, string>
    // {
    //     public AssertAwait_WithExceptionFilter_Should_ReturnAssertionFailure_Data()
    //     {
    //         // Failure Case 1: Assert if value is zero
    //         Add(
    //             Task.FromResult((Result<int>)0),
    //             async val => await Task.FromResult(val != 0),
    //             "Value should not be zero"
    //         );

    //         // Failure Case 2: Assert that value is greater than 100
    //         Add(
    //             Task.FromResult((Result<int>)20),
    //             async val => await Task.FromResult(val > 100),
    //             "Expected value to exceed 100"
    //         );
    //     }
    // }

    // // Test for scenarios where assertions fail and Exceptions are thrown
    // [Theory]
    // [ClassData(typeof(AssertAwait_WithExceptionFilter_Should_ReturnAssertionFailure_Data))]
    // public async Task AssertAwait_WithExceptionFilter_Should_ReturnAssertionFailure(
    //     Task<Result<int>> subject,
    //     Func<int, Task<bool>> assertion,
    //     string assertionMessage
    // )
    // {
    //     // Act
    //     var actualResult = await subject.AssertAwait(assertion, Errors.MapNone, assertionMessage);

    //     // Assert
    //     var actual = (Exception?)actualResult;
    //     actual.Should().NotBeNull();
    //     actual.Should().BeOfType<AssertionException>();
    //     actual.Message.Should().Be(assertionMessage);
    // }

    // // -------------------------------
    // // Exception Handling in Assertion
    // // -------------------------------

    // private class AssertAwait_WithExceptionFilter_Should_HandleExceptionInAssertion_Data
    //     : TheoryData<Task<Result<int>>, Func<int, Task<bool>>, Result<int>>
    // {
    //     public AssertAwait_WithExceptionFilter_Should_HandleExceptionInAssertion_Data()
    //     {
    //         // Handling Case 1: Throws InvalidOperationException
    //         Add(
    //             Task.FromResult((Result<int>)10),
    //             async val =>
    //             {
    //                 await Task.Yield();
    //                 throw new InvalidOperationException("Exception during assertion!");
    //             },
    //             new InvalidOperationException("Exception during assertion!")
    //         );

    //         // Handling Case 2: Throws ArgumentOutOfRangeException
    //         Add(
    //             Task.FromResult((Result<int>)30),
    //             async val =>
    //             {
    //                 await Task.Yield();
    //                 throw new NullReferenceException("Index was out of range!");
    //             },
    //             new NullReferenceException("Index was out of range!")
    //         );
    //     }
    // }

    // // Test for correctly handling exceptions within assertion function
    // [Theory]
    // [ClassData(typeof(AssertAwait_WithExceptionFilter_Should_HandleExceptionInAssertion_Data))]
    // public async Task AssertAwait_WithExceptionFilter_Should_HandleExceptionInAssertion(
    //     Task<Result<int>> subject,
    //     Func<int, Task<bool>> assertion,
    //     Result<int> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.AssertAwait(assertion, Errors.MapAll);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // -------------------------------
    // // Unmapped Exception Propagation
    // // -------------------------------

    // private class AssertAwait_WithExceptionFilter_Should_PropagateUnmappedException_Data
    //     : TheoryData<Task<Result<int>>, Func<int, Task<bool>>, Exception>
    // {
    //     public AssertAwait_WithExceptionFilter_Should_PropagateUnmappedException_Data()
    //     {
    //         // Propagation Case 1: FormatException
    //         Add(
    //             Task.FromResult((Result<int>)150),
    //             async val =>
    //             {
    //                 await Task.Yield();
    //                 throw new FormatException("Assertion threw format exception");
    //             },
    //             new FormatException("Assertion threw format exception")
    //         );

    //         // Propagation Case 2: IndexOutOfRangeException
    //         Add(
    //             Task.FromResult((Result<int>)300),
    //             async val =>
    //             {
    //                 await Task.Yield();
    //                 throw new IndexOutOfRangeException("Index is out of range in assertion");
    //             },
    //             new IndexOutOfRangeException("Index is out of range in assertion")
    //         );
    //     }
    // }

    // // Test ensuring exceptions that aren't handled are propagated
    // [Theory]
    // [ClassData(typeof(AssertAwait_WithExceptionFilter_Should_PropagateUnmappedException_Data))]
    // public async Task AssertAwait_WithExceptionFilter_Should_PropagateUnmappedException(
    //     Task<Result<int>> subject,
    //     Func<int, Task<bool>> assertion,
    //     Exception expected
    // )
    // {
    //     // Act
    //     var act = async () => await subject.AssertAwait(assertion, Errors.MapNone);

    //     // Assert
    //     await act.Should()
    //         .ThrowAsync<Exception>()
    //         .WithMessage(expected.Message)
    //         .Where(x => x.GetType() == expected.GetType());
    // }

    // // ===============================
    // // 16) Assert<TSucc> Method Tests
    // // ===============================

    // // ---------------------------------
    // // Successful Assertion with Assert
    // // ---------------------------------

    // private class Assert_Should_PassAssertion_Data
    //     : TheoryData<Task<Result<int>>, Func<int, Result<bool>>, Result<int>>
    // {
    //     public Assert_Should_PassAssertion_Data()
    //     {
    //         // Test Case 1: Simple assertion always returning true
    //         Add(Task.FromResult((Result<int>)42), val => (Result<bool>)true, 42);

    //         // Test Case 2: Check if value is positive
    //         Add(Task.FromResult((Result<int>)100), val => (Result<bool>)(val > 0), 100);
    //     }
    // }

    // // Test for assertions that pass successfully
    // [Theory]
    // [ClassData(typeof(Assert_Should_PassAssertion_Data))]
    // public async Task Assert_Should_PassAssertion(
    //     Task<Result<int>> subject,
    //     Func<int, Result<bool>> assertion,
    //     Result<int> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.Assert(assertion);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // ---------------------------------
    // // Assertion Failure with Assert
    // // ---------------------------------

    // private class Assert_Should_ReturnAssertionFailure_Data
    //     : TheoryData<Task<Result<int>>, Func<int, Result<bool>>, string>
    // {
    //     public Assert_Should_ReturnAssertionFailure_Data()
    //     {
    //         // Failure Case 1: Assert if value is zero
    //         Add(
    //             Task.FromResult((Result<int>)0),
    //             val => (Result<bool>)(val != 0),
    //             "Assertion failed: Value should not be zero"
    //         );

    //         // Failure Case 2: Assert that value is greater than 50
    //         Add(
    //             Task.FromResult((Result<int>)10),
    //             val => (Result<bool>)(val > 50),
    //             "Assertion failed: Expected value to be greater than 50"
    //         );
    //     }
    // }

    // // Test for scenarios where assertions fail
    // [Theory]
    // [ClassData(typeof(Assert_Should_ReturnAssertionFailure_Data))]
    // public async Task Assert_Should_ReturnAssertionFailure(
    //     Task<Result<int>> subject,
    //     Func<int, Result<bool>> assertion,
    //     string assertionMessage
    // )
    // {
    //     // Act
    //     var actualResult = await subject.Assert(assertion, assertionMessage);

    //     // Assert
    //     var actual = (Exception?)actualResult;
    //     actual.Should().NotBeNull();
    //     actual.Should().BeOfType<AssertionException>();
    //     actual.Message.Should().Be(assertionMessage);
    // }

    // // -------------------------------
    // // Exception in Assertion with Assert
    // // -------------------------------

    // // Test data for exception propagation in assertion
    // private class Assert_Should_PropagateExceptionInAssertion_Data
    //     : TheoryData<Task<Result<int>>, Func<int, Result<bool>>, Exception>
    // {
    //     public Assert_Should_PropagateExceptionInAssertion_Data()
    //     {
    //         // Propagation Case 1: FormatException in assertion
    //         Add(
    //             Task.FromResult((Result<int>)7),
    //             val => throw new FormatException("Assertion threw format exception"),
    //             new FormatException("Assertion threw format exception")
    //         );

    //         // Propagation Case 2: InvalidOperationException in assertion
    //         Add(
    //             Task.FromResult((Result<int>)15),
    //             val => throw new InvalidOperationException("Invalid operation in assertion"),
    //             new InvalidOperationException("Invalid operation in assertion")
    //         );
    //     }
    // }

    // // Test that ensures exceptions within the assertion function are propagated
    // [Theory]
    // [ClassData(typeof(Assert_Should_PropagateExceptionInAssertion_Data))]
    // public async Task Assert_Should_PropagateExceptionInAssertion(
    //     Task<Result<int>> subject,
    //     Func<int, Result<bool>> assertion,
    //     Exception expected
    // )
    // {
    //     // Act
    //     var act = async () => await subject.Assert(assertion);

    //     // Assert
    //     await act.Should()
    //         .ThrowAsync<Exception>()
    //         .WithMessage(expected.Message)
    //         .Where(x => x.GetType() == expected.GetType());
    // }

    // // =====================================================
    // // 17) Assert<TSucc> with ExceptionFilter Method Tests
    // // =====================================================

    // // ---------------------------------
    // // Successful Assertion with Assert
    // // ---------------------------------

    // private class Assert_WithExceptionFilter_Should_PassAssertion_Data
    //     : TheoryData<Task<Result<int>>, Func<int, bool>, Result<int>>
    // {
    //     public Assert_WithExceptionFilter_Should_PassAssertion_Data()
    //     {
    //         // Test Case 1: Trivial assertion always returning true
    //         Add(Task.FromResult((Result<int>)42), val => true, 42);

    //         // Test Case 2: Assert to check if value is positive
    //         Add(Task.FromResult((Result<int>)100), val => val > 0, 100);
    //     }
    // }

    // // Test for assertions that succeed using exception filtering
    // [Theory]
    // [ClassData(typeof(Assert_WithExceptionFilter_Should_PassAssertion_Data))]
    // public async Task Assert_WithExceptionFilter_Should_PassAssertion(
    //     Task<Result<int>> subject,
    //     Func<int, bool> assertion,
    //     Result<int> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.Assert(assertion, Errors.MapNone);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // ---------------------------------
    // // Assertion Failure with Assert
    // // ---------------------------------

    // private class Assert_WithExceptionFilter_Should_ReturnAssertionFailure_Data
    //     : TheoryData<Task<Result<int>>, Func<int, bool>, string>
    // {
    //     public Assert_WithExceptionFilter_Should_ReturnAssertionFailure_Data()
    //     {
    //         // Failure Case 1: Assert if value is zero
    //         Add(Task.FromResult((Result<int>)0), val => val != 0, "Value should not be zero");

    //         // Failure Case 2: Assert that value is greater than 50
    //         Add(
    //             Task.FromResult((Result<int>)10),
    //             val => val > 50,
    //             "Expected value to be greater than 50"
    //         );
    //     }
    // }

    // // Test for scenarios where assertions fail
    // [Theory]
    // [ClassData(typeof(Assert_WithExceptionFilter_Should_ReturnAssertionFailure_Data))]
    // public async Task Assert_WithExceptionFilter_Should_ReturnAssertionFailure(
    //     Task<Result<int>> subject,
    //     Func<int, bool> assertion,
    //     string assertionMessage
    // )
    // {
    //     // Act
    //     var actualResult = await subject.Assert(assertion, Errors.MapNone, assertionMessage);

    //     // Assert
    //     var actual = (Exception?)actualResult;
    //     actual.Should().NotBeNull();
    //     actual.Should().BeOfType<AssertionException>();
    //     actual.Message.Should().Be(assertionMessage);
    // }

    // // -------------------------------
    // // Exception Handling in Assertion
    // // -------------------------------

    // private class Assert_WithExceptionFilter_Should_HandleExceptionInAssertion_Data
    //     : TheoryData<Task<Result<int>>, Func<int, bool>, Result<int>>
    // {
    //     public Assert_WithExceptionFilter_Should_HandleExceptionInAssertion_Data()
    //     {
    //         // Handling Case 1: Throws InvalidOperationException
    //         Add(
    //             Task.FromResult((Result<int>)42),
    //             val => throw new InvalidOperationException("Exception during assertion!"),
    //             new InvalidOperationException("Exception during assertion!")
    //         );

    //         // Handling Case 2: Throws ArgumentOutOfRangeException
    //         Add(
    //             Task.FromResult((Result<int>)30),
    //             val => throw new NullReferenceException("Index was out of range!"),
    //             new NullReferenceException("Index was out of range!")
    //         );
    //     }
    // }

    // // Test for correctly handling exceptions within assertion function
    // [Theory]
    // [ClassData(typeof(Assert_WithExceptionFilter_Should_HandleExceptionInAssertion_Data))]
    // public async Task Assert_WithExceptionFilter_Should_HandleExceptionInAssertion(
    //     Task<Result<int>> subject,
    //     Func<int, bool> assertion,
    //     Result<int> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.Assert(assertion, Errors.MapAll);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // -------------------------------
    // // Unmapped Exception Propagation
    // // -------------------------------

    // private class Assert_WithExceptionFilter_Should_PropagateUnmappedException_Data
    //     : TheoryData<Task<Result<int>>, Func<int, bool>, Exception>
    // {
    //     public Assert_WithExceptionFilter_Should_PropagateUnmappedException_Data()
    //     {
    //         // Propagation Case 1: FormatException
    //         Add(
    //             Task.FromResult((Result<int>)150),
    //             val => throw new FormatException("Assertion threw format exception"),
    //             new FormatException("Assertion threw format exception")
    //         );

    //         // Propagation Case 2: IndexOutOfRangeException
    //         Add(
    //             Task.FromResult((Result<int>)300),
    //             val => throw new IndexOutOfRangeException("Index is out of range in assertion"),
    //             new IndexOutOfRangeException("Index is out of range in assertion")
    //         );
    //     }
    // }

    // // Test ensuring exceptions that aren't handled are propagated
    // [Theory]
    // [ClassData(typeof(Assert_WithExceptionFilter_Should_PropagateUnmappedException_Data))]
    // public async Task Assert_WithExceptionFilter_Should_PropagateUnmappedException(
    //     Task<Result<int>> subject,
    //     Func<int, bool> assertion,
    //     Exception expected
    // )
    // {
    //     // Act
    //     var act = async () => await subject.Assert(assertion, Errors.MapNone);

    //     // Assert
    //     await act.Should()
    //         .ThrowAsync<Exception>()
    //         .WithMessage(expected.Message)
    //         .Where(x => x.GetType() == expected.GetType());
    // }

    // // ==========================================
    // // 18) IfAwait<TSucc, TResult> Method Tests
    // // ==========================================

    // // ---------------------------------
    // // Predicate is True, Then Should Execute
    // // ---------------------------------

    // private class IfAwait_Should_ExecuteThenBranch_WhenPredicateIsTrue_Data
    //     : TheoryData<
    //         Task<Result<int>>,
    //         Func<int, Task<Result<bool>>>,
    //         Func<int, Task<Result<string>>>,
    //         Func<int, Task<Result<string>>>,
    //         Result<string>
    //     >
    // {
    //     public IfAwait_Should_ExecuteThenBranch_WhenPredicateIsTrue_Data()
    //     {
    //         Add(
    //             Task.FromResult((Result<int>)10),
    //             async val => await Task.FromResult((Result<bool>)(val > 5)),
    //             async val => await Task.FromResult((Result<string>)"Then executed"),
    //             async val => await Task.FromResult((Result<string>)"Else executed"),
    //             "Then executed"
    //         );

    //         Add(
    //             Task.FromResult((Result<int>)20),
    //             async val => await Task.FromResult((Result<bool>)(val == 20)),
    //             async val => await Task.FromResult((Result<string>)"Exactly twenty"),
    //             async val => await Task.FromResult((Result<string>)"Not twenty"),
    //             "Exactly twenty"
    //         );

    //         // Adding another scenario for variety
    //         Add(
    //             Task.FromResult((Result<int>)(-10)),
    //             async val => await Task.FromResult((Result<bool>)(val < 0)),
    //             async val => await Task.FromResult((Result<string>)"Negative number processed"),
    //             async val => await Task.FromResult((Result<string>)"Non-negative case missed"),
    //             "Negative number processed"
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(IfAwait_Should_ExecuteThenBranch_WhenPredicateIsTrue_Data))]
    // public async Task IfAwait_Should_ExecuteThenBranch_WhenPredicateIsTrue(
    //     Task<Result<int>> subject,
    //     Func<int, Task<Result<bool>>> predicate,
    //     Func<int, Task<Result<string>>> Then,
    //     Func<int, Task<Result<string>>> Else,
    //     Result<string> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.IfAwait(predicate, Then, Else);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // ---------------------------------
    // // Predicate is False, Else Should Execute
    // // ---------------------------------

    // private class IfAwait_Should_ExecuteElseBranch_WhenPredicateIsFalse_Data
    //     : TheoryData<
    //         Task<Result<int>>,
    //         Func<int, Task<Result<bool>>>,
    //         Func<int, Task<Result<string>>>,
    //         Func<int, Task<Result<string>>>,
    //         Result<string>
    //     >
    // {
    //     public IfAwait_Should_ExecuteElseBranch_WhenPredicateIsFalse_Data()
    //     {
    //         Add(
    //             Task.FromResult((Result<int>)3),
    //             async val => await Task.FromResult((Result<bool>)(val > 10)),
    //             async val => await Task.FromResult((Result<string>)"Then executed"),
    //             async val => await Task.FromResult((Result<string>)"Else executed"),
    //             "Else executed"
    //         );

    //         Add(
    //             Task.FromResult((Result<int>)(int)-5),
    //             async val => await Task.FromResult((Result<bool>)(val >= 0)),
    //             async val => await Task.FromResult((Result<string>)"Non-negative handled"),
    //             async val =>
    //                 await Task.FromResult((Result<string>)"Negative handled appropriately"),
    //             "Negative handled appropriately"
    //         );

    //         // Additional scenario with zero
    //         Add(
    //             Task.FromResult((Result<int>)0),
    //             async val => await Task.FromResult((Result<bool>)(val > 0)),
    //             async val => await Task.FromResult((Result<string>)"Positive flow"),
    //             async val => await Task.FromResult((Result<string>)"Zero or Negative fallback"),
    //             "Zero or Negative fallback"
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(IfAwait_Should_ExecuteElseBranch_WhenPredicateIsFalse_Data))]
    // public async Task IfAwait_Should_ExecuteElseBranch_WhenPredicateIsFalse(
    //     Task<Result<int>> subject,
    //     Func<int, Task<Result<bool>>> predicate,
    //     Func<int, Task<Result<string>>> Then,
    //     Func<int, Task<Result<string>>> Else,
    //     Result<string> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.IfAwait(predicate, Then, Else);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // ---------------------------------
    // // Predicate is Failure, Should Receive Failure
    // // ---------------------------------

    // private class IfAwait_Should_ReceiveFailure_WhenPredicateFails_Data
    //     : TheoryData<
    //         Task<Result<int>>,
    //         Func<int, Task<Result<bool>>>,
    //         Func<int, Task<Result<string>>>,
    //         Func<int, Task<Result<string>>>,
    //         Result<string>
    //     >
    // {
    //     public IfAwait_Should_ReceiveFailure_WhenPredicateFails_Data()
    //     {
    //         Add(
    //             Task.FromResult((Result<int>)5),
    //             async val =>
    //                 await Task.FromResult(
    //                     (Result<bool>)new InvalidOperationException("Predicate failure")
    //                 ),
    //             async val => await Task.FromResult((Result<string>)"Then executed"),
    //             async val => await Task.FromResult((Result<string>)"Else executed"),
    //             new InvalidOperationException("Predicate failure")
    //         );

    //         // Include scenarios with different exceptions
    //         Add(
    //             Task.FromResult((Result<int>)42),
    //             async val =>
    //                 await Task.FromResult(
    //                     (Result<bool>)new ArgumentException("Invalid argument in predicate")
    //                 ),
    //             async val => await Task.FromResult((Result<string>)"Then flow"),
    //             async val => await Task.FromResult((Result<string>)"Else flow"),
    //             new ArgumentException("Invalid argument in predicate")
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(IfAwait_Should_ReceiveFailure_WhenPredicateFails_Data))]
    // public async Task IfAwait_Should_ReceiveFailure_WhenPredicateFails(
    //     Task<Result<int>> subject,
    //     Func<int, Task<Result<bool>>> predicate,
    //     Func<int, Task<Result<string>>> Then,
    //     Func<int, Task<Result<string>>> Else,
    //     Result<string> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.IfAwait(predicate, Then, Else);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // ---------------------------------
    // // Predicate Throws Exception, Exception Should Propagate
    // // ---------------------------------

    // private class IfAwait_Should_PropagateException_WhenPredicateThrows_Data
    //     : TheoryData<
    //         Task<Result<int>>,
    //         Func<int, Task<Result<bool>>>,
    //         Func<int, Task<Result<string>>>,
    //         Func<int, Task<Result<string>>>,
    //         Exception
    //     >
    // {
    //     public IfAwait_Should_PropagateException_WhenPredicateThrows_Data()
    //     {
    //         Add(
    //             Task.FromResult((Result<int>)8),
    //             async val =>
    //             {
    //                 await Task.Yield();
    //                 throw new FormatException("Exception in predicate");
    //             },
    //             async val => await Task.FromResult((Result<string>)"Then executed"),
    //             async val => await Task.FromResult((Result<string>)"Else executed"),
    //             new FormatException("Exception in predicate")
    //         );

    //         // Include more diverse exception scenarios
    //         Add(
    //             Task.FromResult((Result<int>)(-1)),
    //             async val =>
    //             {
    //                 await Task.Yield();
    //                 throw new NotSupportedException("Not supported predicate scenario");
    //             },
    //             async val => await Task.FromResult((Result<string>)"Then branch"),
    //             async val => await Task.FromResult((Result<string>)"Else branch"),
    //             new NotSupportedException("Not supported predicate scenario")
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(IfAwait_Should_PropagateException_WhenPredicateThrows_Data))]
    // public async Task IfAwait_Should_PropagateException_WhenPredicateThrows(
    //     Task<Result<int>> subject,
    //     Func<int, Task<Result<bool>>> predicate,
    //     Func<int, Task<Result<string>>> Then,
    //     Func<int, Task<Result<string>>> Else,
    //     Exception expected
    // )
    // {
    //     // Act
    //     var act = async () => await subject.IfAwait(predicate, Then, Else);

    //     // Assert
    //     await act.Should()
    //         .ThrowAsync<Exception>()
    //         .WithMessage(expected.Message)
    //         .Where(x => x.GetType() == expected.GetType());
    // }

    // // ---------------------------------
    // // Predicate is True, Then Emits Failure, Should Receive Failure
    // // ---------------------------------

    // private class IfAwait_Should_ReceiveFailure_WhenThenEmitsFailure_Data
    //     : TheoryData<
    //         Task<Result<int>>,
    //         Func<int, Task<Result<bool>>>,
    //         Func<int, Task<Result<string>>>,
    //         Func<int, Task<Result<string>>>,
    //         Result<string>
    //     >
    // {
    //     public IfAwait_Should_ReceiveFailure_WhenThenEmitsFailure_Data()
    //     {
    //         Add(
    //             Task.FromResult((Result<int>)15),
    //             async val => await Task.FromResult((Result<bool>)(val > 10)),
    //             async val =>
    //                 await Task.FromResult(
    //                     (Result<string>)new InvalidOperationException("Then emitted failure")
    //                 ),
    //             async val => await Task.FromResult((Result<string>)"Else executed"),
    //             new InvalidOperationException("Then emitted failure")
    //         );

    //         // Adding another failure scenario
    //         Add(
    //             Task.FromResult((Result<int>)25),
    //             async val => await Task.FromResult((Result<bool>)(val > 20)),
    //             async val =>
    //                 await Task.FromResult(
    //                     (Result<string>)new ApplicationException("Application error in Then")
    //                 ),
    //             async val => await Task.FromResult((Result<string>)"Else called"),
    //             new ApplicationException("Application error in Then")
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(IfAwait_Should_ReceiveFailure_WhenThenEmitsFailure_Data))]
    // public async Task IfAwait_Should_ReceiveFailure_WhenThenEmitsFailure(
    //     Task<Result<int>> subject,
    //     Func<int, Task<Result<bool>>> predicate,
    //     Func<int, Task<Result<string>>> Then,
    //     Func<int, Task<Result<string>>> Else,
    //     Result<string> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.IfAwait(predicate, Then, Else);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // ---------------------------------
    // // Predicate is True, Then Throws Exception, Exception Should Propagate
    // // ---------------------------------

    // private class IfAwait_Should_PropagateException_WhenThenThrows_Data
    //     : TheoryData<
    //         Task<Result<int>>,
    //         Func<int, Task<Result<bool>>>,
    //         Func<int, Task<Result<string>>>,
    //         Func<int, Task<Result<string>>>,
    //         Exception
    //     >
    // {
    //     public IfAwait_Should_PropagateException_WhenThenThrows_Data()
    //     {
    //         Add(
    //             Task.FromResult((Result<int>)20),
    //             async val => await Task.FromResult((Result<bool>)(val > 15)),
    //             async val =>
    //             {
    //                 await Task.Yield();
    //                 throw new FormatException("Exception in Then");
    //             },
    //             async val => await Task.FromResult((Result<string>)"Else executed"),
    //             new FormatException("Exception in Then")
    //         );

    //         // Additional case with another exception type
    //         Add(
    //             Task.FromResult((Result<int>)30),
    //             async val => await Task.FromResult((Result<bool>)(val <= 30)),
    //             async val =>
    //             {
    //                 await Task.Yield();
    //                 throw new InvalidCastException("Invalid cast in Then operation");
    //             },
    //             async val => await Task.FromResult((Result<string>)"Else logic"),
    //             new InvalidCastException("Invalid cast in Then operation")
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(IfAwait_Should_PropagateException_WhenThenThrows_Data))]
    // public async Task IfAwait_Should_PropagateException_WhenThenThrows(
    //     Task<Result<int>> subject,
    //     Func<int, Task<Result<bool>>> predicate,
    //     Func<int, Task<Result<string>>> Then,
    //     Func<int, Task<Result<string>>> Else,
    //     Exception expected
    // )
    // {
    //     // Act
    //     var act = async () => await subject.IfAwait(predicate, Then, Else);

    //     // Assert
    //     await act.Should()
    //         .ThrowAsync<Exception>()
    //         .WithMessage(expected.Message)
    //         .Where(x => x.GetType() == expected.GetType());
    // }

    // // ---------------------------------
    // // Predicate is False, Else Emits Failure, Should Receive Failure
    // // ---------------------------------

    // private class IfAwait_Should_ReceiveFailure_WhenElseEmitsFailure_Data
    //     : TheoryData<
    //         Task<Result<int>>,
    //         Func<int, Task<Result<bool>>>,
    //         Func<int, Task<Result<string>>>,
    //         Func<int, Task<Result<string>>>,
    //         Result<string>
    //     >
    // {
    //     public IfAwait_Should_ReceiveFailure_WhenElseEmitsFailure_Data()
    //     {
    //         Add(
    //             Task.FromResult((Result<int>)8),
    //             async val => await Task.FromResult((Result<bool>)(val > 10)),
    //             async val => await Task.FromResult((Result<string>)"Then executed"),
    //             async val =>
    //                 await Task.FromResult(
    //                     (Result<string>)new InvalidOperationException("Else emitted failure")
    //                 ),
    //             new InvalidOperationException("Else emitted failure")
    //         );

    //         // Adding another failure case
    //         Add(
    //             Task.FromResult((Result<int>)12),
    //             async val => await Task.FromResult((Result<bool>)(val < 0)),
    //             async val => await Task.FromResult((Result<string>)"Then flow"),
    //             async val =>
    //                 await Task.FromResult(
    //                     (Result<string>)new TimeoutException("Timeout error in Else")
    //                 ),
    //             new TimeoutException("Timeout error in Else")
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(IfAwait_Should_ReceiveFailure_WhenElseEmitsFailure_Data))]
    // public async Task IfAwait_Should_ReceiveFailure_WhenElseEmitsFailure(
    //     Task<Result<int>> subject,
    //     Func<int, Task<Result<bool>>> predicate,
    //     Func<int, Task<Result<string>>> Then,
    //     Func<int, Task<Result<string>>> Else,
    //     Result<string> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.IfAwait(predicate, Then, Else);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // ---------------------------------
    // // Predicate is False, Else Throws Exception, Exception Should Propagate
    // // ---------------------------------

    // private class IfAwait_Should_PropagateException_WhenElseThrows_Data
    //     : TheoryData<
    //         Task<Result<int>>,
    //         Func<int, Task<Result<bool>>>,
    //         Func<int, Task<Result<string>>>,
    //         Func<int, Task<Result<string>>>,
    //         Exception
    //     >
    // {
    //     public IfAwait_Should_PropagateException_WhenElseThrows_Data()
    //     {
    //         Add(
    //             Task.FromResult((Result<int>)2),
    //             async val => await Task.FromResult((Result<bool>)(val == 0)),
    //             async val => await Task.FromResult((Result<string>)"Then branch"),
    //             async val =>
    //             {
    //                 await Task.Yield();
    //                 throw new ArgumentException("Exception in Else");
    //             },
    //             new ArgumentException("Exception in Else")
    //         );

    //         // Introducing another thrown exception scenario
    //         Add(
    //             Task.FromResult((Result<int>)100),
    //             async val => await Task.FromResult((Result<bool>)(val < 50)),
    //             async val => await Task.FromResult((Result<string>)"Following Then logic"),
    //             async val =>
    //             {
    //                 await Task.Yield();
    //                 throw new ArithmeticException("Division error in Else logic");
    //             },
    //             new ArithmeticException("Division error in Else logic")
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(IfAwait_Should_PropagateException_WhenElseThrows_Data))]
    // public async Task IfAwait_Should_PropagateException_WhenElseThrows(
    //     Task<Result<int>> subject,
    //     Func<int, Task<Result<bool>>> predicate,
    //     Func<int, Task<Result<string>>> Then,
    //     Func<int, Task<Result<string>>> Else,
    //     Exception expected
    // )
    // {
    //     // Act
    //     var act = async () => await subject.IfAwait(predicate, Then, Else);

    //     // Assert
    //     await act.Should()
    //         .ThrowAsync<Exception>()
    //         .WithMessage(expected.Message)
    //         .Where(x => x.GetType() == expected.GetType());
    // }

    // // =====================================
    // // 19) If<TSucc, TResult> Method Tests
    // // =====================================

    // // ---------------------------------
    // // Predicate is True, Then Should Execute
    // // ---------------------------------

    // private class If_Should_ExecuteThenBranch_WhenPredicateIsTrue_Data
    //     : TheoryData<
    //         Task<Result<int>>,
    //         Func<int, Result<bool>>,
    //         Func<int, Result<string>>,
    //         Func<int, Result<string>>,
    //         Result<string>
    //     >
    // {
    //     public If_Should_ExecuteThenBranch_WhenPredicateIsTrue_Data()
    //     {
    //         Add(
    //             Task.FromResult((Result<int>)10),
    //             val => (Result<bool>)(val > 5),
    //             val => (Result<string>)"Then executed",
    //             val => (Result<string>)"Else executed",
    //             "Then executed"
    //         );

    //         Add(
    //             Task.FromResult((Result<int>)20),
    //             val => (Result<bool>)(val == 20),
    //             val => (Result<string>)"Exactly twenty",
    //             val => (Result<string>)"Not twenty",
    //             "Exactly twenty"
    //         );

    //         // Additional scenario
    //         Add(
    //             Task.FromResult((Result<int>)(-10)),
    //             val => (Result<bool>)(val < 0),
    //             val => (Result<string>)"Negative number processed",
    //             val => (Result<string>)"Non-negative case missed",
    //             "Negative number processed"
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(If_Should_ExecuteThenBranch_WhenPredicateIsTrue_Data))]
    // public async Task If_Should_ExecuteThenBranch_WhenPredicateIsTrue(
    //     Task<Result<int>> subject,
    //     Func<int, Result<bool>> predicate,
    //     Func<int, Result<string>> Then,
    //     Func<int, Result<string>> Else,
    //     Result<string> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.If(predicate, Then, Else);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // ---------------------------------
    // // Predicate is False, Else Should Execute
    // // ---------------------------------

    // private class If_Should_ExecuteElseBranch_WhenPredicateIsFalse_Data
    //     : TheoryData<
    //         Task<Result<int>>,
    //         Func<int, Result<bool>>,
    //         Func<int, Result<string>>,
    //         Func<int, Result<string>>,
    //         Result<string>
    //     >
    // {
    //     public If_Should_ExecuteElseBranch_WhenPredicateIsFalse_Data()
    //     {
    //         Add(
    //             Task.FromResult((Result<int>)3),
    //             val => (Result<bool>)(val > 10),
    //             val => (Result<string>)"Then executed",
    //             val => (Result<string>)"Else executed",
    //             "Else executed"
    //         );

    //         Add(
    //             Task.FromResult((Result<int>)(int)-5),
    //             val => (Result<bool>)(val >= 0),
    //             val => (Result<string>)"Non-negative handled",
    //             val => (Result<string>)"Negative handled appropriately",
    //             "Negative handled appropriately"
    //         );

    //         // Additional scenario
    //         Add(
    //             Task.FromResult((Result<int>)0),
    //             val => (Result<bool>)(val > 0),
    //             val => (Result<string>)"Positive flow",
    //             val => (Result<string>)"Zero or Negative fallback",
    //             "Zero or Negative fallback"
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(If_Should_ExecuteElseBranch_WhenPredicateIsFalse_Data))]
    // public async Task If_Should_ExecuteElseBranch_WhenPredicateIsFalse(
    //     Task<Result<int>> subject,
    //     Func<int, Result<bool>> predicate,
    //     Func<int, Result<string>> Then,
    //     Func<int, Result<string>> Else,
    //     Result<string> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.If(predicate, Then, Else);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // ---------------------------------
    // // Predicate is Failure, Should Receive Failure
    // // ---------------------------------

    // private class If_Should_ReceiveFailure_WhenPredicateFails_Data
    //     : TheoryData<
    //         Task<Result<int>>,
    //         Func<int, Result<bool>>,
    //         Func<int, Result<string>>,
    //         Func<int, Result<string>>,
    //         Result<string>
    //     >
    // {
    //     public If_Should_ReceiveFailure_WhenPredicateFails_Data()
    //     {
    //         Add(
    //             Task.FromResult((Result<int>)5),
    //             val => (Result<bool>)new InvalidOperationException("Predicate failure"),
    //             val => (Result<string>)"Then executed",
    //             val => (Result<string>)"Else executed",
    //             new InvalidOperationException("Predicate failure")
    //         );

    //         // Another failure scenario
    //         Add(
    //             Task.FromResult((Result<int>)42),
    //             val => (Result<bool>)new ArgumentException("Invalid argument in predicate"),
    //             val => (Result<string>)"Then flow",
    //             val => (Result<string>)"Else flow",
    //             new ArgumentException("Invalid argument in predicate")
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(If_Should_ReceiveFailure_WhenPredicateFails_Data))]
    // public async Task If_Should_ReceiveFailure_WhenPredicateFails(
    //     Task<Result<int>> subject,
    //     Func<int, Result<bool>> predicate,
    //     Func<int, Result<string>> Then,
    //     Func<int, Result<string>> Else,
    //     Result<string> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.If(predicate, Then, Else);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // ---------------------------------
    // // Predicate Throws Exception, Exception Should Propagate
    // // ---------------------------------

    // private class If_Should_PropagateException_WhenPredicateThrows_Data
    //     : TheoryData<
    //         Task<Result<int>>,
    //         Func<int, Result<bool>>,
    //         Func<int, Result<string>>,
    //         Func<int, Result<string>>,
    //         Exception
    //     >
    // {
    //     public If_Should_PropagateException_WhenPredicateThrows_Data()
    //     {
    //         Add(
    //             Task.FromResult((Result<int>)8),
    //             val => throw new FormatException("Exception in predicate"),
    //             val => (Result<string>)"Then executed",
    //             val => (Result<string>)"Else executed",
    //             new FormatException("Exception in predicate")
    //         );

    //         // Additional exception scenario
    //         Add(
    //             Task.FromResult((Result<int>)(-1)),
    //             val => throw new NotSupportedException("Not supported predicate scenario"),
    //             val => (Result<string>)"Then branch",
    //             val => (Result<string>)"Else branch",
    //             new NotSupportedException("Not supported predicate scenario")
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(If_Should_PropagateException_WhenPredicateThrows_Data))]
    // public async Task If_Should_PropagateException_WhenPredicateThrows(
    //     Task<Result<int>> subject,
    //     Func<int, Result<bool>> predicate,
    //     Func<int, Result<string>> Then,
    //     Func<int, Result<string>> Else,
    //     Exception expected
    // )
    // {
    //     // Act
    //     var act = async () => await subject.If(predicate, Then, Else);

    //     // Assert
    //     await act.Should()
    //         .ThrowAsync<Exception>()
    //         .WithMessage(expected.Message)
    //         .Where(x => x.GetType() == expected.GetType());
    // }

    // // ---------------------------------
    // // Predicate is True, Then Emits Failure, Should Receive Failure
    // // ---------------------------------

    // private class If_Should_ReceiveFailure_WhenThenEmitsFailure_Data
    //     : TheoryData<
    //         Task<Result<int>>,
    //         Func<int, Result<bool>>,
    //         Func<int, Result<string>>,
    //         Func<int, Result<string>>,
    //         Result<string>
    //     >
    // {
    //     public If_Should_ReceiveFailure_WhenThenEmitsFailure_Data()
    //     {
    //         Add(
    //             Task.FromResult((Result<int>)15),
    //             val => (Result<bool>)(val > 10),
    //             val => (Result<string>)new InvalidOperationException("Then emitted failure"),
    //             val => (Result<string>)"Else executed",
    //             new InvalidOperationException("Then emitted failure")
    //         );

    //         // Additional scenario with different exception
    //         Add(
    //             Task.FromResult((Result<int>)25),
    //             val => (Result<bool>)(val > 20),
    //             val => (Result<string>)new ApplicationException("Application error in Then"),
    //             val => (Result<string>)"Else called",
    //             new ApplicationException("Application error in Then")
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(If_Should_ReceiveFailure_WhenThenEmitsFailure_Data))]
    // public async Task If_Should_ReceiveFailure_WhenThenEmitsFailure(
    //     Task<Result<int>> subject,
    //     Func<int, Result<bool>> predicate,
    //     Func<int, Result<string>> Then,
    //     Func<int, Result<string>> Else,
    //     Result<string> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.If(predicate, Then, Else);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // ---------------------------------
    // // Predicate is True, Then Throws Exception, Exception Should Propagate
    // // ---------------------------------

    // private class If_Should_PropagateException_WhenThenThrows_Data
    //     : TheoryData<
    //         Task<Result<int>>,
    //         Func<int, Result<bool>>,
    //         Func<int, Result<string>>,
    //         Func<int, Result<string>>,
    //         Exception
    //     >
    // {
    //     public If_Should_PropagateException_WhenThenThrows_Data()
    //     {
    //         Add(
    //             Task.FromResult((Result<int>)20),
    //             val => (Result<bool>)(val > 15),
    //             val => throw new FormatException("Exception in Then"),
    //             val => (Result<string>)"Else executed",
    //             new FormatException("Exception in Then")
    //         );

    //         // Additional case with other exception
    //         Add(
    //             Task.FromResult((Result<int>)30),
    //             val => (Result<bool>)(val <= 30),
    //             val => throw new InvalidCastException("Invalid cast in Then operation"),
    //             val => (Result<string>)"Else logic",
    //             new InvalidCastException("Invalid cast in Then operation")
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(If_Should_PropagateException_WhenThenThrows_Data))]
    // public async Task If_Should_PropagateException_WhenThenThrows(
    //     Task<Result<int>> subject,
    //     Func<int, Result<bool>> predicate,
    //     Func<int, Result<string>> Then,
    //     Func<int, Result<string>> Else,
    //     Exception expected
    // )
    // {
    //     // Act
    //     var act = async () => await subject.If(predicate, Then, Else);

    //     // Assert
    //     await act.Should()
    //         .ThrowAsync<Exception>()
    //         .WithMessage(expected.Message)
    //         .Where(x => x.GetType() == expected.GetType());
    // }

    // // ---------------------------------
    // // Predicate is False, Else Emits Failure, Should Receive Failure
    // // ---------------------------------

    // private class If_Should_ReceiveFailure_WhenElseEmitsFailure_Data
    //     : TheoryData<
    //         Task<Result<int>>,
    //         Func<int, Result<bool>>,
    //         Func<int, Result<string>>,
    //         Func<int, Result<string>>,
    //         Result<string>
    //     >
    // {
    //     public If_Should_ReceiveFailure_WhenElseEmitsFailure_Data()
    //     {
    //         Add(
    //             Task.FromResult((Result<int>)8),
    //             val => (Result<bool>)(val > 10),
    //             val => (Result<string>)"Then executed",
    //             val => (Result<string>)new InvalidOperationException("Else emitted failure"),
    //             new InvalidOperationException("Else emitted failure")
    //         );

    //         // New case with TimeoutException
    //         Add(
    //             Task.FromResult((Result<int>)12),
    //             val => (Result<bool>)(val < 0),
    //             val => (Result<string>)"Then flow",
    //             val => (Result<string>)new TimeoutException("Timeout error in Else"),
    //             new TimeoutException("Timeout error in Else")
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(If_Should_ReceiveFailure_WhenElseEmitsFailure_Data))]
    // public async Task If_Should_ReceiveFailure_WhenElseEmitsFailure(
    //     Task<Result<int>> subject,
    //     Func<int, Result<bool>> predicate,
    //     Func<int, Result<string>> Then,
    //     Func<int, Result<string>> Else,
    //     Result<string> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.If(predicate, Then, Else);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // ---------------------------------
    // // Predicate is False, Else Throws Exception, Exception Should Propagate
    // // ---------------------------------

    // private class If_Should_PropagateException_WhenElseThrows_Data
    //     : TheoryData<
    //         Task<Result<int>>,
    //         Func<int, Result<bool>>,
    //         Func<int, Result<string>>,
    //         Func<int, Result<string>>,
    //         Exception
    //     >
    // {
    //     public If_Should_PropagateException_WhenElseThrows_Data()
    //     {
    //         Add(
    //             Task.FromResult((Result<int>)2),
    //             val => (Result<bool>)(val == 0),
    //             val => (Result<string>)"Then branch",
    //             val => throw new ArgumentException("Exception in Else"),
    //             new ArgumentException("Exception in Else")
    //         );

    //         // Introducing arithmetic exception
    //         Add(
    //             Task.FromResult((Result<int>)100),
    //             val => (Result<bool>)(val < 50),
    //             val => (Result<string>)"Following Then logic",
    //             val => throw new ArithmeticException("Division error in Else logic"),
    //             new ArithmeticException("Division error in Else logic")
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(If_Should_PropagateException_WhenElseThrows_Data))]
    // public async Task If_Should_PropagateException_WhenElseThrows(
    //     Task<Result<int>> subject,
    //     Func<int, Result<bool>> predicate,
    //     Func<int, Result<string>> Then,
    //     Func<int, Result<string>> Else,
    //     Exception expected
    // )
    // {
    //     // Act
    //     var act = async () => await subject.If(predicate, Then, Else);

    //     // Assert
    //     await act.Should()
    //         .ThrowAsync<Exception>()
    //         .WithMessage(expected.Message)
    //         .Where(x => x.GetType() == expected.GetType());
    // }

    // // =====================================
    // // 20) Do<TSucc, TResult> Method Tests
    // // =====================================

    // // ---------------------------------
    // // Success Scenario
    // // ---------------------------------

    // private class Do_Should_ReturnSuccess_Data
    //     : TheoryData<Task<Result<int>>, DoType, Func<int, Result<string>>, Result<int>>
    // {
    //     public Do_Should_ReturnSuccess_Data()
    //     {
    //         // Test Case 1: Simple success transformation with DoType.Ignore
    //         Add(
    //             Task.FromResult((Result<int>)10),
    //             DoType.Ignore,
    //             val => (Result<string>)($"Value: {val}"),
    //             10
    //         );

    //         // Test Case 2: Transformation with addition
    //         Add(
    //             Task.FromResult((Result<int>)5),
    //             DoType.Ignore,
    //             val => (Result<string>)($"+5 = {val + 5}"),
    //             5
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(Do_Should_ReturnSuccess_Data))]
    // public async Task Do_Should_ReturnSuccess(
    //     Task<Result<int>> subject,
    //     DoType doType,
    //     Func<int, Result<string>> transformFunc,
    //     Result<int> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.Do(doType, transformFunc);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // ---------------------------------
    // // Failure Scenario
    // // ---------------------------------

    // private class Do_Should_HandleFailure_Data
    //     : TheoryData<Task<Result<int>>, DoType, Func<int, Result<string>>, Result<int>>
    // {
    //     public Do_Should_HandleFailure_Data()
    //     {
    //         // Failure Case 1: Fails with a handled exception
    //         Add(
    //             Task.FromResult((Result<int>)42),
    //             DoType.MapErrors,
    //             val => new InvalidOperationException("Invalid operation"),
    //             new InvalidOperationException("Invalid operation")
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(Do_Should_HandleFailure_Data))]
    // public async Task Do_Should_HandleFailure(
    //     Task<Result<int>> subject,
    //     DoType doType,
    //     Func<int, Result<string>> transformFunc,
    //     Result<int> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.Do(doType, transformFunc);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // ---------------------------------
    // // Exception Propagation
    // // ---------------------------------

    // private class Do_Should_PropagateException_Data
    //     : TheoryData<Task<Result<int>>, DoType, Func<int, Result<string>>, Exception>
    // {
    //     public Do_Should_PropagateException_Data()
    //     {
    //         // Propagation Case 1: Unmapped exception
    //         Add(
    //             Task.FromResult((Result<int>)7),
    //             DoType.Ignore,
    //             val => throw new FormatException("Unmapped exception"),
    //             new FormatException("Unmapped exception")
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(Do_Should_PropagateException_Data))]
    // public async Task Do_Should_PropagateException(
    //     Task<Result<int>> subject,
    //     DoType doType,
    //     Func<int, Result<string>> transformFunc,
    //     Exception expected
    // )
    // {
    //     // Act
    //     var act = async () => await subject.Do(doType, transformFunc);

    //     // Assert
    //     await act.Should()
    //         .ThrowAsync<Exception>()
    //         .WithMessage(expected.Message)
    //         .Where(x => x.GetType() == expected.GetType());
    // }

    // // ====================================================
    // // 21) Do<TSucc, TResult> with ExceptionFilter Tests
    // // ====================================================

    // // ---------------------------------
    // // Success Scenario
    // // ---------------------------------

    // private class Do_WithExceptionFilter_Should_ReturnSuccess_Data
    //     : TheoryData<
    //         Task<Result<int>>,
    //         DoType,
    //         Func<int, string>,
    //         Errors.ExceptionFilter,
    //         Result<int>
    //     >
    // {
    //     public Do_WithExceptionFilter_Should_ReturnSuccess_Data()
    //     {
    //         // Test Case 1: Successful transformation
    //         Add(
    //             Task.FromResult((Result<int>)12),
    //             DoType.Ignore,
    //             val => $"Value-{val}",
    //             Errors.MapNone,
    //             12
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(Do_WithExceptionFilter_Should_ReturnSuccess_Data))]
    // public async Task Do_WithExceptionFilter_Should_ReturnSuccess(
    //     Task<Result<int>> subject,
    //     DoType doType,
    //     Func<int, string> function,
    //     Errors.ExceptionFilter mapException,
    //     Result<int> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.Do(doType, function, mapException);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // ---------------------------------
    // // Failure Handling
    // // ---------------------------------

    // private class Do_WithExceptionFilter_Should_HandleFailure_Data
    //     : TheoryData<
    //         Task<Result<int>>,
    //         DoType,
    //         Func<int, string>,
    //         Errors.ExceptionFilter,
    //         Result<int>
    //     >
    // {
    //     public Do_WithExceptionFilter_Should_HandleFailure_Data()
    //     {
    //         // Failure Scenario
    //         Add(
    //             Task.FromResult((Result<int>)5),
    //             DoType.MapErrors,
    //             val => throw new InvalidOperationException("Mapped failure"),
    //             Errors.MapAll,
    //             new InvalidOperationException("Mapped failure")
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(Do_WithExceptionFilter_Should_HandleFailure_Data))]
    // public async Task Do_WithExceptionFilter_Should_HandleFailure(
    //     Task<Result<int>> subject,
    //     DoType doType,
    //     Func<int, string> function,
    //     Errors.ExceptionFilter mapException,
    //     Result<int> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.Do(doType, function, mapException);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // ---------------------------------
    // // Exception Propagation
    // // ---------------------------------

    // private class Do_WithExceptionFilter_Should_PropagateException_Data
    //     : TheoryData<
    //         Task<Result<int>>,
    //         DoType,
    //         Func<int, string>,
    //         Errors.ExceptionFilter,
    //         Exception
    //     >
    // {
    //     public Do_WithExceptionFilter_Should_PropagateException_Data()
    //     {
    //         // Unmapped exception scenario
    //         Add(
    //             Task.FromResult((Result<int>)9),
    //             DoType.Ignore,
    //             val => throw new FormatException("Unmapped exception"),
    //             Errors.MapNone,
    //             new FormatException("Unmapped exception")
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(Do_WithExceptionFilter_Should_PropagateException_Data))]
    // public async Task Do_WithExceptionFilter_Should_PropagateException(
    //     Task<Result<int>> subject,
    //     DoType doType,
    //     Func<int, string> function,
    //     Errors.ExceptionFilter mapException,
    //     Exception expected
    // )
    // {
    //     // Act
    //     var act = async () => await subject.Do(doType, function, mapException);

    //     // Assert
    //     await act.Should()
    //         .ThrowAsync<Exception>()
    //         .WithMessage(expected.Message)
    //         .Where(x => x.GetType() == expected.GetType());
    // }

    // // ==========================================
    // // 22) Do<TSucc> with ExceptionFilter Tests
    // // ==========================================

    // // ---------------------------------
    // // Success Scenario
    // // ---------------------------------

    // private class DoAction_WithExceptionFilter_Should_ReturnSuccess_Data
    //     : TheoryData<Task<Result<int>>, DoType, Action<int>, Errors.ExceptionFilter, Result<int>>
    // {
    //     public DoAction_WithExceptionFilter_Should_ReturnSuccess_Data()
    //     {
    //         // Test Case 1: Console logging action
    //         Add(
    //             Task.FromResult((Result<int>)20),
    //             DoType.Ignore,
    //             val => Console.WriteLine($"Action executed with value: {val}"),
    //             Errors.MapNone,
    //             20
    //         );

    //         // Test Case 2: No-op action
    //         Add(
    //             Task.FromResult((Result<int>)30),
    //             DoType.Ignore,
    //             val => { }, // No operation
    //             Errors.MapNone,
    //             30
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(DoAction_WithExceptionFilter_Should_ReturnSuccess_Data))]
    // public async Task DoAction_WithExceptionFilter_Should_ReturnSuccess(
    //     Task<Result<int>> subject,
    //     DoType doType,
    //     Action<int> action,
    //     Errors.ExceptionFilter mapException,
    //     Result<int> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.Do(doType, action, mapException);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // ---------------------------------
    // // Failure Handling
    // // ---------------------------------

    // private class DoAction_WithExceptionFilter_Should_HandleFailure_Data
    //     : TheoryData<Task<Result<int>>, DoType, Action<int>, Errors.ExceptionFilter, Result<int>>
    // {
    //     public DoAction_WithExceptionFilter_Should_HandleFailure_Data()
    //     {
    //         // Failure Case 1: InvalidOperationException
    //         Add(
    //             Task.FromResult((Result<int>)15),
    //             DoType.MapErrors,
    //             val => throw new InvalidOperationException("Mapped failure in action"),
    //             Errors.MapAll,
    //             new InvalidOperationException("Mapped failure in action")
    //         );

    //         // Failure Case 2: ArgumentOutOfRangeException
    //         Add(
    //             Task.FromResult((Result<int>)5),
    //             DoType.MapErrors,
    //             val => throw new NullReferenceException("Value is out of range"),
    //             Errors.MapAll,
    //             new NullReferenceException("Value is out of range")
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(DoAction_WithExceptionFilter_Should_HandleFailure_Data))]
    // public async Task DoAction_WithExceptionFilter_Should_HandleFailure(
    //     Task<Result<int>> subject,
    //     DoType doType,
    //     Action<int> action,
    //     Errors.ExceptionFilter mapException,
    //     Result<int> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.Do(doType, action, mapException);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // ---------------------------------
    // // Exception Propagation
    // // ---------------------------------

    // private class DoAction_WithExceptionFilter_Should_PropagateException_Data
    //     : TheoryData<Task<Result<int>>, DoType, Action<int>, Errors.ExceptionFilter, Exception>
    // {
    //     public DoAction_WithExceptionFilter_Should_PropagateException_Data()
    //     {
    //         // Propagation Case 1: Unmapped FormatException
    //         Add(
    //             Task.FromResult((Result<int>)25),
    //             DoType.Ignore,
    //             val => throw new FormatException("Unmapped exception in action"),
    //             Errors.MapNone,
    //             new FormatException("Unmapped exception in action")
    //         );

    //         // Propagation Case 2: Unmapped IndexOutOfRangeException
    //         Add(
    //             Task.FromResult((Result<int>)50),
    //             DoType.Ignore,
    //             val => throw new IndexOutOfRangeException("Index out of bounds"),
    //             Errors.MapNone,
    //             new IndexOutOfRangeException("Index out of bounds")
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(DoAction_WithExceptionFilter_Should_PropagateException_Data))]
    // public async Task DoAction_WithExceptionFilter_Should_PropagateException(
    //     Task<Result<int>> subject,
    //     DoType doType,
    //     Action<int> action,
    //     Errors.ExceptionFilter mapException,
    //     Exception expected
    // )
    // {
    //     // Act
    //     var act = async () => await subject.Do(doType, action, mapException);

    //     // Assert
    //     await act.Should()
    //         .ThrowAsync<Exception>()
    //         .WithMessage(expected.Message)
    //         .Where(x => x.GetType() == expected.GetType());
    // }

    // // ======================================================
    // // 23) Then<TSucc, TResult> with ExceptionFilter Tests
    // // ======================================================

    // // ---------------------------------
    // // Success Scenario
    // // ---------------------------------

    // private class Then_WithExceptionFilter_Should_ReturnSuccess_Data
    //     : TheoryData<Task<Result<int>>, Func<int, string>, Errors.ExceptionFilter, Result<string>>
    // {
    //     public Then_WithExceptionFilter_Should_ReturnSuccess_Data()
    //     {
    //         // Test Case 1: Basic transformation
    //         Add(
    //             Task.FromResult((Result<int>)10),
    //             val => $"Transformed-{val}",
    //             Errors.MapNone,
    //             "Transformed-10"
    //         );

    //         // Test Case 2: Multiplication
    //         Add(
    //             Task.FromResult((Result<int>)3),
    //             val => $"Value x2: {val * 2}",
    //             Errors.MapNone,
    //             "Value x2: 6"
    //         );

    //         // Test Case 3: No transformation needed
    //         Add(Task.FromResult((Result<int>)0), val => $"Zero: {val}", Errors.MapNone, "Zero: 0");
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(Then_WithExceptionFilter_Should_ReturnSuccess_Data))]
    // public async Task Then_WithExceptionFilter_Should_ReturnSuccess(
    //     Task<Result<int>> subject,
    //     Func<int, string> function,
    //     Errors.ExceptionFilter mapException,
    //     Result<string> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.Then(function, mapException);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // ---------------------------------
    // // Handled Exception
    // // ---------------------------------

    // private class Then_WithExceptionFilter_Should_HandleFailure_Data
    //     : TheoryData<Task<Result<int>>, Func<int, string>, Errors.ExceptionFilter, Result<string>>
    // {
    //     public Then_WithExceptionFilter_Should_HandleFailure_Data()
    //     {
    //         // Failure Case 1: Mapped InvalidOperationException
    //         Add(
    //             Task.FromResult((Result<int>)5),
    //             val => throw new InvalidOperationException("Handled failure"),
    //             Errors.MapAll,
    //             new InvalidOperationException("Handled failure")
    //         );

    //         // Failure Case 2: Mapped ArgumentException
    //         Add(
    //             Task.FromResult((Result<int>)11),
    //             val => throw new ArgumentException("Argument failure"),
    //             Errors.MapAll,
    //             new ArgumentException("Argument failure")
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(Then_WithExceptionFilter_Should_HandleFailure_Data))]
    // public async Task Then_WithExceptionFilter_Should_HandleFailure(
    //     Task<Result<int>> subject,
    //     Func<int, string> function,
    //     Errors.ExceptionFilter mapException,
    //     Result<string> expected
    // )
    // {
    //     // Act
    //     var actual = await subject.Then(function, mapException);

    //     // Assert
    //     actual.Should().BeEquivalentTo(expected);
    // }

    // // ---------------------------------
    // // Exception Propagation
    // // ---------------------------------

    // private class Then_WithExceptionFilter_Should_PropagateException_Data
    //     : TheoryData<Task<Result<int>>, Func<int, string>, Errors.ExceptionFilter, Exception>
    // {
    //     public Then_WithExceptionFilter_Should_PropagateException_Data()
    //     {
    //         // Propagation Case 1: Unmapped FormatException
    //         Add(
    //             Task.FromResult((Result<int>)8),
    //             val => throw new FormatException("Unmapped exception"),
    //             Errors.MapNone,
    //             new FormatException("Unmapped exception")
    //         );

    //         // Propagation Case 2: Unmapped NullReferenceException
    //         Add(
    //             Task.FromResult((Result<int>)15),
    //             val => throw new NullReferenceException("Null reference exception"),
    //             Errors.MapNone,
    //             new NullReferenceException("Null reference exception")
    //         );
    //     }
    // }

    // [Theory]
    // [ClassData(typeof(Then_WithExceptionFilter_Should_PropagateException_Data))]
    // public async Task Then_WithExceptionFilter_Should_PropagateException(
    //     Task<Result<int>> subject,
    //     Func<int, string> function,
    //     Errors.ExceptionFilter mapException,
    //     Exception expected
    // )
    // {
    //     // Act
    //     var act = async () => await subject.Then(function, mapException);

    //     // Assert
    //     await act.Should()
    //         .ThrowAsync<Exception>()
    //         .WithMessage(expected.Message)
    //         .Where(x => x.GetType() == expected.GetType());
    // }
}
