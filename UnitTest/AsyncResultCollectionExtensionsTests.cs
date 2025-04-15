using CarboxylicLithium;
using FluentAssertions;

namespace UnitTest;

public class AsyncResultCollectionExtensionsTests
{
    // ==============
    // 1) AllSucceed
    // ==============

    private class AllSucceed_Should_ReturnTrue_Data : TheoryData<IEnumerable<Task<Result<int>>>>
    {
        public AllSucceed_Should_ReturnTrue_Data()
        {
            // Case 1: All successes
            Add(
                [
                    Task.FromResult((Result<int>)1),
                    Task.FromResult((Result<int>)2),
                    Task.FromResult((Result<int>)3),
                    Task.FromResult((Result<int>)0),
                    Task.FromResult((Result<int>)(-1)),
                ]
            );

            // Case 2: Empty collection (which means no failures too)
            Add([]);
        }
    }

    // AllSucceed should return true if all results in the collection are successes
    [Theory]
    [ClassData(typeof(AllSucceed_Should_ReturnTrue_Data))]
    public async Task AllSucceed_Should_ReturnTrue_WhenAllSuccesses(
        IEnumerable<Task<Result<int>>> collection
    )
    {
        // Act
        var actual = await collection.AllSucceed();

        // Assert
        actual.Should().BeTrue("all elements were successful or the collection is empty.");
    }

    private class AllSucceed_Should_ReturnFalse_Data : TheoryData<IEnumerable<Task<Result<int>>>>
    {
        public AllSucceed_Should_ReturnFalse_Data()
        {
            // Case 1: Contains one failure
            Add(
                [
                    Task.FromResult((Result<int>)1),
                    Task.FromResult((Result<int>)new InvalidOperationException()),
                    Task.FromResult((Result<int>)3),
                    Task.FromResult((Result<int>)(-5)),
                ]
            );

            // Case 2: Mixed, two failures
            Add(
                [
                    Task.FromResult((Result<int>)new InvalidOperationException("First failure")),
                    Task.FromResult((Result<int>)1),
                    Task.FromResult((Result<int>)new Exception("Second failure")),
                    Task.FromResult((Result<int>)2),
                    Task.FromResult((Result<int>)(-3)),
                ]
            );

            // Case 3: All failures
            Add(
                [
                    Task.FromResult((Result<int>)new Exception("Failure")),
                    Task.FromResult((Result<int>)new InvalidOperationException("Second Failure")),
                ]
            );
        }
    }

    // AllSucceed should return false if at least one result in the collection is a failure
    [Theory]
    [ClassData(typeof(AllSucceed_Should_ReturnFalse_Data))]
    public async Task AllSucceed_Should_ReturnFalse_WhenAnyFailure(
        IEnumerable<Task<Result<int>>> collection
    )
    {
        // Act
        var actual = await collection.AllSucceed();

        // Assert
        actual.Should().BeFalse("there was at least one failure in the collection.");
    }

    // 1 END

    // ==============
    // 2) AnySucceed
    // ==============

    private class AnySucceed_Should_ReturnTrue_IfAnySuccess_Data
        : TheoryData<IEnumerable<Task<Result<int>>>>
    {
        public AnySucceed_Should_ReturnTrue_IfAnySuccess_Data()
        {
            // Case 1: Success first
            Add(
                [
                    Task.FromResult((Result<int>)1),
                    Task.FromResult((Result<int>)new InvalidOperationException()),
                    Task.FromResult((Result<int>)new Exception()),
                ]
            );

            // Case 2: Success in the middle
            Add(
                [
                    Task.FromResult((Result<int>)new Exception("Error")),
                    Task.FromResult((Result<int>)2),
                    Task.FromResult((Result<int>)new InvalidOperationException()),
                ]
            );

            // Case 3: Success last
            Add(
                [
                    Task.FromResult((Result<int>)new Exception()),
                    Task.FromResult((Result<int>)new InvalidOperationException()),
                    Task.FromResult((Result<int>)3),
                ]
            );

            // Case 4: Multiple successes
            Add(
                [
                    Task.FromResult((Result<int>)1),
                    Task.FromResult((Result<int>)2),
                    Task.FromResult((Result<int>)3),
                ]
            );
        }
    }

    // AnySucceed should return true if there is at least one success in the collection
    [Theory]
    [ClassData(typeof(AnySucceed_Should_ReturnTrue_IfAnySuccess_Data))]
    public async Task AnySucceed_Should_ReturnTrue_IfAnySuccess(
        IEnumerable<Task<Result<int>>> collection
    )
    {
        // Act
        var actual = await collection.AnySucceed();

        // Assert
        actual.Should().BeTrue("at least one element in the collection was successful.");
    }

    private class AnySucceed_Should_ReturnFalse_IfAllFailures_Data
        : TheoryData<IEnumerable<Task<Result<int>>>>
    {
        public AnySucceed_Should_ReturnFalse_IfAllFailures_Data()
        {
            // Case 1: All failures
            Add(
                [
                    Task.FromResult((Result<int>)new Exception()),
                    Task.FromResult((Result<int>)new InvalidOperationException()),
                    Task.FromResult((Result<int>)new Exception("Another error")),
                ]
            );

            // Case 2: Empty collection, false because there's no success
            Add([]);
        }
    }

    // AnySucceed should return false if all results in the collection are failures
    [Theory]
    [ClassData(typeof(AnySucceed_Should_ReturnFalse_IfAllFailures_Data))]
    public async Task AnySucceed_Should_ReturnFalse_IfAllFailures(
        IEnumerable<Task<Result<int>>> collection
    )
    {
        // Act
        var actual = await collection.AnySucceed();

        // Assert
        actual.Should().BeFalse("all elements in the collection were failures.");
    }

    // 2 END

    // ==============
    // 3) AllFail
    // ==============

    private class AllFail_Should_ReturnTrue_IfAllFailures_Data
        : TheoryData<IEnumerable<Task<Result<int>>>>
    {
        public AllFail_Should_ReturnTrue_IfAllFailures_Data()
        {
            // Case 1: All failures
            Add(
                [
                    Task.FromResult((Result<int>)new Exception()),
                    Task.FromResult((Result<int>)new InvalidOperationException()),
                    Task.FromResult((Result<int>)new Exception("Another error")),
                ]
            );

            // Case 2: Empty collection, should return true as there are no successes
            Add([]);
        }
    }

    // AllFail should return true if all results in the collection are failures
    [Theory]
    [ClassData(typeof(AllFail_Should_ReturnTrue_IfAllFailures_Data))]
    public async Task AllFail_Should_ReturnTrue_IfAllFailures(
        IEnumerable<Task<Result<int>>> collection
    )
    {
        // Act
        var actual = await collection.AllFail();

        // Assert
        actual
            .Should()
            .BeTrue("all elements in the collection were failures or the collection is empty.");
    }

    private class AllFail_Should_ReturnFalse_IfAnySuccess_Data
        : TheoryData<IEnumerable<Task<Result<int>>>>
    {
        public AllFail_Should_ReturnFalse_IfAnySuccess_Data()
        {
            // Case 1: Success first
            Add(
                [
                    Task.FromResult((Result<int>)1),
                    Task.FromResult((Result<int>)new InvalidOperationException()),
                    Task.FromResult((Result<int>)new Exception()),
                ]
            );

            // Case 2: Success in the middle
            Add(
                [
                    Task.FromResult((Result<int>)new Exception("Error")),
                    Task.FromResult((Result<int>)2),
                    Task.FromResult((Result<int>)new InvalidOperationException()),
                ]
            );

            // Case 3: Success last
            Add(
                [
                    Task.FromResult((Result<int>)new Exception()),
                    Task.FromResult((Result<int>)new InvalidOperationException()),
                    Task.FromResult((Result<int>)3),
                ]
            );

            // Case 4: Mixed with multiple successes
            Add(
                [
                    Task.FromResult((Result<int>)1),
                    Task.FromResult((Result<int>)new Exception("Failure")),
                    Task.FromResult((Result<int>)2),
                ]
            );
        }
    }

    // AllFail should return false if there is at least one success in the collection
    [Theory]
    [ClassData(typeof(AllFail_Should_ReturnFalse_IfAnySuccess_Data))]
    public async Task AllFail_Should_ReturnFalse_IfAnySuccess(
        IEnumerable<Task<Result<int>>> collection
    )
    {
        // Act
        var actual = await collection.AllFail();

        // Assert
        actual.Should().BeFalse("there was at least one success in the collection.");
    }

    // 3 END

    // ==============
    // 4) AnyFail
    // ==============

    private class AnyFail_Should_ReturnTrue_IfAnyFailure_Data
        : TheoryData<IEnumerable<Task<Result<int>>>>
    {
        public AnyFail_Should_ReturnTrue_IfAnyFailure_Data()
        {
            // Case 1: Failure first
            Add(
                [
                    Task.FromResult((Result<int>)new InvalidOperationException()),
                    Task.FromResult((Result<int>)1),
                    Task.FromResult((Result<int>)2),
                ]
            );

            // Case 2: Failure in the middle
            Add(
                [
                    Task.FromResult((Result<int>)1),
                    Task.FromResult((Result<int>)new Exception("Error")),
                    Task.FromResult((Result<int>)2),
                ]
            );

            // Case 3: Failure last
            Add(
                [
                    Task.FromResult((Result<int>)1),
                    Task.FromResult((Result<int>)2),
                    Task.FromResult((Result<int>)new Exception()),
                ]
            );

            // Case 4: Multiple failures
            Add(
                [
                    Task.FromResult((Result<int>)new Exception("First failure")),
                    Task.FromResult((Result<int>)new InvalidOperationException("Second failure")),
                    Task.FromResult((Result<int>)1),
                ]
            );
        }
    }

    // AnyFail should return true if there is at least one failure in the collection
    [Theory]
    [ClassData(typeof(AnyFail_Should_ReturnTrue_IfAnyFailure_Data))]
    public async Task AnyFail_Should_ReturnTrue_IfAnyFailure(
        IEnumerable<Task<Result<int>>> collection
    )
    {
        // Act
        var actual = await collection.AnyFail();

        // Assert
        actual.Should().BeTrue("there is at least one failure in the collection.");
    }

    private class AnyFail_Should_ReturnFalse_IfAllSuccesses_Data
        : TheoryData<IEnumerable<Task<Result<int>>>>
    {
        public AnyFail_Should_ReturnFalse_IfAllSuccesses_Data()
        {
            // Case 1: All successes
            Add(
                [
                    Task.FromResult((Result<int>)1),
                    Task.FromResult((Result<int>)2),
                    Task.FromResult((Result<int>)0),
                ]
            );

            // Case 2: Empty collection, naturally returning false
            Add([]);
        }
    }

    // AnyFail should return false if all results in the collection are successful
    [Theory]
    [ClassData(typeof(AnyFail_Should_ReturnFalse_IfAllSuccesses_Data))]
    public async Task AnyFail_Should_ReturnFalse_IfAllSuccesses(
        IEnumerable<Task<Result<int>>> collection
    )
    {
        // Act
        var actual = await collection.AnyFail();

        // Assert
        actual
            .Should()
            .BeFalse("all elements in the collection were successful or the collection is empty.");
    }

    // 4 END

    // ==============
    // 5)   Get
    // ==============

    private class Get_Should_ReturnAllSuccesses_Data
        : TheoryData<IEnumerable<Task<Result<int>>>, IEnumerable<int>>
    {
        public Get_Should_ReturnAllSuccesses_Data()
        {
            // Case 1: Simple all successes
            Add(
                [
                    Task.FromResult((Result<int>)1),
                    Task.FromResult((Result<int>)2),
                    Task.FromResult((Result<int>)3),
                ],
                (List<int>)[1, 2, 3]
            );

            // Case 2: Including boundary values
            Add(
                [
                    Task.FromResult((Result<int>)int.MaxValue),
                    Task.FromResult((Result<int>)int.MinValue),
                    Task.FromResult((Result<int>)0),
                ],
                (List<int>)[int.MaxValue, int.MinValue, 0]
            );

            // Case 3: Empty collection leading to empty result
            Add([], (List<int>)[]);
        }
    }

    // Get should return all successes if the entire collection succeeds
    [Theory]
    [ClassData(typeof(Get_Should_ReturnAllSuccesses_Data))]
    public async Task Get_Should_ReturnAllSuccesses(
        IEnumerable<Task<Result<int>>> collection,
        IEnumerable<int> expected
    )
    {
        // Act
        var actual = await collection.Get();

        // Assert
        actual
            .Should()
            .BeEquivalentTo(expected, "all elements in the collection were successful.");
    }

    private class Get_Should_ThrowAggregateException_IfAnyFailure_Data
        : TheoryData<IEnumerable<Task<Result<int>>>>
    {
        public Get_Should_ThrowAggregateException_IfAnyFailure_Data()
        {
            // Case 1: Failure at start
            Add(
                [
                    Task.FromResult((Result<int>)new InvalidOperationException("First failure")),
                    Task.FromResult((Result<int>)2),
                    Task.FromResult((Result<int>)3),
                ]
            );

            // Case 2: Failure in the middle
            Add(
                [
                    Task.FromResult((Result<int>)1),
                    Task.FromResult((Result<int>)new ArgumentException("Middle failure")),
                    Task.FromResult((Result<int>)3),
                ]
            );

            // Case 3: Failure at the end
            Add(
                [
                    Task.FromResult((Result<int>)1),
                    Task.FromResult((Result<int>)2),
                    Task.FromResult((Result<int>)new Exception("Last failure")),
                ]
            );

            // Case 4: Multiple failures
            Add(
                [
                    Task.FromResult((Result<int>)new Exception("Multi-failure 1")),
                    Task.FromResult((Result<int>)new InvalidOperationException("Multi-failure 2")),
                ]
            );
        }
    }

    // Get should throw AggregateException when there are failures in the collection
    [Theory]
    [ClassData(typeof(Get_Should_ThrowAggregateException_IfAnyFailure_Data))]
    public async Task Get_Should_ThrowAggregateException_IfAnyFailure(
        IEnumerable<Task<Result<int>>> collection
    )
    {
        // Act
        var act = async () => await collection.Get();

        // Assert
        await act.Should().ThrowAsync<AggregateException>("there were failures in the collection.");
    }

    private class Get_Should_ReturnEmptyCollection_WhenInputIsEmpty_Data
        : TheoryData<IEnumerable<Task<Result<int>>>>
    {
        public Get_Should_ReturnEmptyCollection_WhenInputIsEmpty_Data()
        {
            // Empty collection case
            Add(Array.Empty<Task<Result<int>>>());
        }
    }

    [Theory]
    [ClassData(typeof(Get_Should_ReturnEmptyCollection_WhenInputIsEmpty_Data))]
    public async Task Get_Should_ReturnEmptyCollection_WhenInputIsEmpty(
        IEnumerable<Task<Result<int>>> collection
    )
    {
        // Act
        var result = await collection.Get();

        // Assert
        result
            .Should()
            .BeEmpty("an empty collection of results should return an empty collection");
    }

    // 5 END

    // ==============
    // 6) GetFailures
    // ==============

    private class GetFailures_Should_ReturnAllFailures_Data
        : TheoryData<IEnumerable<Task<Result<int>>>, IEnumerable<Exception>>
    {
        public GetFailures_Should_ReturnAllFailures_Data()
        {
            // Case 1: Only failures
            var ex1 = new InvalidOperationException("Error 1");
            var ex2 = new ArgumentException("Error 2");
            Add(
                [Task.FromResult((Result<int>)ex1), Task.FromResult((Result<int>)ex2)],
                (List<Exception>)[ex1, ex2]
            );

            // Case 2: Mixed entries
            var ex3 = new Exception("General Error");
            Add(
                [
                    Task.FromResult((Result<int>)1),
                    Task.FromResult((Result<int>)ex3),
                    Task.FromResult((Result<int>)3),
                ],
                (List<Exception>)[ex3]
            );

            // Case 3: Empty collection, expecting empty result
            Add([], (List<Exception>)[]);
        }
    }

    // GetFailures should return all exceptions found in the collection
    [Theory]
    [ClassData(typeof(GetFailures_Should_ReturnAllFailures_Data))]
    public async Task GetFailures_Should_ReturnAllFailures(
        IEnumerable<Task<Result<int>>> collection,
        IEnumerable<Exception> expected
    )
    {
        // Act
        var actual = await collection.GetFailures().ToListAsync();

        // Assert
        actual
            .Should()
            .BeEquivalentTo(expected, "these are the expected failures from the collection.");
    }

    private class GetFailures_Should_ReturnEmpty_IfNoFailures_Data
        : TheoryData<IEnumerable<Task<Result<int>>>>
    {
        public GetFailures_Should_ReturnEmpty_IfNoFailures_Data()
        {
            // Case 1: All successes
            Add([Task.FromResult((Result<int>)1), Task.FromResult((Result<int>)2)]);
        }
    }

    // GetFailures should return an empty collection if there are no failures
    [Theory]
    [ClassData(typeof(GetFailures_Should_ReturnEmpty_IfNoFailures_Data))]
    public async Task GetFailures_Should_ReturnEmpty_IfNoFailures(
        IEnumerable<Task<Result<int>>> collection
    )
    {
        // Act
        var actual = await collection.GetFailures().ToListAsync();

        // Assert
        actual.Should().BeEmpty("there are no failures in the collection.");
    }

    // 6 END


    // ================
    // 7) GetSuccesses
    // ================

    private class GetSuccesses_Should_ReturnAllSuccesses_Data
        : TheoryData<IEnumerable<Task<Result<int>>>, IEnumerable<int>>
    {
        public GetSuccesses_Should_ReturnAllSuccesses_Data()
        {
            // Case 1: Only successes
            Add(
                [
                    Task.FromResult((Result<int>)1),
                    Task.FromResult((Result<int>)2),
                    Task.FromResult((Result<int>)3),
                ],
                (List<int>)[1, 2, 3]
            );

            // Case 2: Mixed, with more successes than failures
            Add(
                [
                    Task.FromResult((Result<int>)1),
                    Task.FromResult((Result<int>)new Exception("Failure")),
                    Task.FromResult((Result<int>)2),
                    Task.FromResult((Result<int>)3),
                ],
                (List<int>)[1, 2, 3]
            );

            // Case 3: Empty list gives empty success list
            Add([], (List<int>)[]);
        }
    }

    // GetSuccesses should return all successful values from the collection
    [Theory]
    [ClassData(typeof(GetSuccesses_Should_ReturnAllSuccesses_Data))]
    public async Task GetSuccesses_Should_ReturnAllSuccesses(
        IEnumerable<Task<Result<int>>> collection,
        IEnumerable<int> expected
    )
    {
        // Act
        var actual = await collection.GetSuccesses().ToListAsync();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    private class GetSuccesses_Should_ReturnEmpty_IfNoSuccesses_Data
        : TheoryData<IEnumerable<Task<Result<int>>>>
    {
        public GetSuccesses_Should_ReturnEmpty_IfNoSuccesses_Data()
        {
            // Case 1: All failures
            Add(
                [
                    Task.FromResult<Result<int>>(new Exception("Failure")),
                    Task.FromResult<Result<int>>(new InvalidOperationException("Another failure")),
                ]
            );
        }
    }

    // GetSuccesses should return an empty collection if there are no successes
    [Theory]
    [ClassData(typeof(GetSuccesses_Should_ReturnEmpty_IfNoSuccesses_Data))]
    public async Task GetSuccesses_Should_ReturnEmpty_IfNoSuccesses(
        IEnumerable<Task<Result<int>>> collection
    )
    {
        // Act
        var actualSuccesses = await collection.GetSuccesses().ToListAsync();

        // Assert
        actualSuccesses.Should().BeEmpty("there are no successes in the collection.");
    }

    private class GetSuccesses_Should_ReturnEmpty_WhenInputIsEmpty_Data
        : TheoryData<IEnumerable<Task<Result<int>>>>
    {
        public GetSuccesses_Should_ReturnEmpty_WhenInputIsEmpty_Data()
        {
            // Empty collection case
            Add(Array.Empty<Task<Result<int>>>());
        }
    }

    [Theory]
    [ClassData(typeof(GetSuccesses_Should_ReturnEmpty_WhenInputIsEmpty_Data))]
    public async Task GetSuccesses_Should_ReturnEmpty_WhenInputIsEmpty(
        IEnumerable<Task<Result<int>>> collection
    )
    {
        // Act
        var result = await collection.GetSuccesses().ToListAsync();

        // Assert
        result.Should().BeEmpty("an empty input collection should return an empty collection");
    }

    // 7 END


    // ==================
    // 8)    AwaitAll
    // ==================

    private class AwaitAll_Should_ReturnAllResults_Data
        : TheoryData<IEnumerable<Task<Result<int>>>, IEnumerable<Result<int>>>
    {
        public AwaitAll_Should_ReturnAllResults_Data()
        {
            // Case 1: All successes
            Add(
                [
                    Task.FromResult((Result<int>)1),
                    Task.FromResult((Result<int>)2),
                    Task.FromResult((Result<int>)3),
                ],
                (List<Result<int>>)[1, 2, 3]
            );

            // Case 2: Mixed results
            Add(
                [
                    Task.FromResult((Result<int>)1),
                    Task.FromResult((Result<int>)new InvalidOperationException("Fail")),
                    Task.FromResult((Result<int>)3),
                ],
                (List<Result<int>>)[1, new InvalidOperationException("Fail"), 3]
            );

            // Case 3: Empty collection
            Add([], (List<Result<int>>)[]);
        }
    }

    // AwaitAll should wait for all tasks and return a collection of the results
    [Theory]
    [ClassData(typeof(AwaitAll_Should_ReturnAllResults_Data))]
    public async Task AwaitAll_Should_ReturnAllResults(
        IEnumerable<Task<Result<int>>> collection,
        IEnumerable<Result<int>> expected
    )
    {
        // Act
        var actual = (await collection.AwaitAll()).ToArray();
        var ex = expected.ToArray();

        // Assert
        actual.Should().BeEquivalentTo(ex);
    }

    // 8 END


    // ==================
    // 9)    AwaitAny
    // ==================

    private class AwaitAny_Should_ReturnAnyResult_Data : TheoryData<IEnumerable<Task<Result<int>>>>
    {
        public AwaitAny_Should_ReturnAnyResult_Data()
        {
            // Case 1: Mixed successes and failures
            Add(
                [
                    Task.FromResult((Result<int>)1),
                    Task.FromResult((Result<int>)new InvalidOperationException("Fail")),
                    Task.FromResult((Result<int>)2),
                ]
            );

            // Case 2: All successes
            Add(
                [
                    Task.FromResult((Result<int>)10),
                    Task.FromResult((Result<int>)20),
                    Task.FromResult((Result<int>)30),
                ]
            );

            // Case 3: All failures
            Add(
                [
                    Task.FromResult((Result<int>)new Exception("Error 1")),
                    Task.FromResult((Result<int>)new InvalidOperationException("Error 2")),
                ]
            );
        }
    }

    // AwaitAny should wait for any task to complete and return that result
    [Theory]
    [ClassData(typeof(AwaitAny_Should_ReturnAnyResult_Data))]
    public async Task AwaitAny_Should_ReturnAnyResult(IEnumerable<Task<Result<int>>> collection)
    {
        // Act
        var actual = await collection.AwaitAny();

        // Assert
        actual
            .Should()
            .BeOfType<Result<int>>("AwaitAny should return a valid result from any of the tasks.");
    }

    // 9 END


    // ==========================
    // 10) ToIAsyncEnumerable
    // ==========================

    private class ToIAsyncEnumerable_Should_ConvertToAsyncEnumerable_Data
        : TheoryData<IEnumerable<Task<Result<int>>>, IEnumerable<Result<int>>>
    {
        public ToIAsyncEnumerable_Should_ConvertToAsyncEnumerable_Data()
        {
            // Case 1: Mixed collection
            Add(
                [
                    Task.FromResult((Result<int>)1),
                    Task.FromResult((Result<int>)new InvalidOperationException("Failure")),
                    Task.FromResult((Result<int>)2),
                ],
                (List<Result<int>>)[1, new InvalidOperationException("Failure"), 2]
            );

            // Case 2: All successes
            Add(
                [
                    Task.FromResult((Result<int>)10),
                    Task.FromResult((Result<int>)20),
                    Task.FromResult((Result<int>)30),
                ],
                (List<Result<int>>)[10, 20, 30]
            );

            // Case 3: All failures
            Add(
                [
                    Task.FromResult((Result<int>)new Exception("Error 1")),
                    Task.FromResult((Result<int>)new InvalidOperationException("Error 2")),
                ],
                (List<Result<int>>)
                    [new Exception("Error 1"), new InvalidOperationException("Error 2")]
            );

            // Case 4: Empty collection
            Add([], (List<Result<int>>)[]);
        }
    }

    // ToIAsyncEnumerable should convert the collection to an async enumerable with the same results
    [Theory]
    [ClassData(typeof(ToIAsyncEnumerable_Should_ConvertToAsyncEnumerable_Data))]
    public async Task ToIAsyncEnumerable_Should_ConvertToAsyncEnumerable(
        IEnumerable<Task<Result<int>>> input,
        IEnumerable<Result<int>> expected
    )
    {
        // Act

        var actual = await input.ToIAsyncEnumerable().ToListAsync();
        var ex = expected.ToList();

        // Assert
        actual.Should().BeEquivalentTo(ex);
    }

    // 10 END

    // =========================================================================
    // 11) DoAwaitEach<TSucc, TResult>
    //    (this IEnumerable<Task<Result<TSucc>>> results,
    //     DoType type,
    //     Func<TSucc, Task<Result<TResult>>> function)
    //
    // We will cover:
    //  - Scenario A: All are successes (DoType.MapErrors and DoType.Ignore variants)
    //  - Scenario B: Some failures among them
    //  - Scenario C: No failures but function throws an exception
    // =========================================================================

    private class DoAwaitEach_TSucc_TResult_Should_ProcessResults_SuccessData
        : TheoryData<
            List<Task<Result<int>>>,
            DoType,
            Func<int, Task<Result<string>>>,
            List<Result<int>>
        >
    {
        public DoAwaitEach_TSucc_TResult_Should_ProcessResults_SuccessData()
        {
            // Scenario A1: All successes, DoType.MapErrors
            Add(
                [Task.FromResult((Result<int>)10), Task.FromResult((Result<int>)20)],
                DoType.MapErrors,
                async i => await Task.FromResult((Result<string>)($"Value:{i}")),
                // Original list is unaffected by the function, so the expected are the same success results
                [10, 20]
            );

            // Scenario A2: All successes, DoType.Ignore
            Add(
                [
                    Task.FromResult((Result<int>)1),
                    Task.FromResult((Result<int>)2),
                    Task.FromResult((Result<int>)3),
                ],
                DoType.Ignore,
                async i =>
                {
                    // Just returning success with some computed string
                    await Task.Delay(1);
                    return (Result<string>)($"Number:{i}");
                },
                [1, 2, 3]
            );
        }
    }

    [Theory]
    [ClassData(typeof(DoAwaitEach_TSucc_TResult_Should_ProcessResults_SuccessData))]
    public async Task DoAwaitEach_TSucc_TResult_should_ProcessResults_Success(
        List<Task<Result<int>>> input,
        DoType doType,
        Func<int, Task<Result<string>>> transform,
        List<Result<int>> expected
    )
    {
        // Arrange

        // Act
        var actual = await Task.WhenAll(input.DoAwaitEach(doType, transform));

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    private class DoAwaitEach_TSucc_TResult_Should_ProcessResults_FailureData
        : TheoryData<
            List<Task<Result<int>>>,
            DoType,
            Func<int, Task<Result<string>>>,
            List<Result<int>>
        >
    {
        public DoAwaitEach_TSucc_TResult_Should_ProcessResults_FailureData()
        {
            // Scenario B1: Some failures, DoType.MapErrors
            Add(
                [
                    Task.FromResult((Result<int>)new InvalidOperationException("Failure #1")),
                    Task.FromResult((Result<int>)42),
                ],
                DoType.MapErrors,
                async i =>
                {
                    await Task.Delay(1);
                    return (Result<string>)($"Got:{i}");
                },
                // We expect the first to remain failure, second to remain success
                [new InvalidOperationException("Failure #1"), 42]
            );

            // Scenario B2: multiple failures, DoType.Ignore
            Add(
                [
                    Task.FromResult((Result<int>)new NullReferenceException("Null #2")),
                    Task.FromResult((Result<int>)new InvalidOperationException("Op #3")),
                    Task.FromResult((Result<int>)100),
                ],
                DoType.Ignore,
                async i =>
                {
                    await Task.Delay(1);
                    return (Result<string>)"dummy";
                },
                [new NullReferenceException("Null #2"), new InvalidOperationException("Op #3"), 100]
            );
        }
    }

    [Theory]
    [ClassData(typeof(DoAwaitEach_TSucc_TResult_Should_ProcessResults_FailureData))]
    public async Task DoAwaitEach_TSucc_TResult_should_ProcessResults_Failure(
        List<Task<Result<int>>> input,
        DoType doType,
        Func<int, Task<Result<string>>> transform,
        List<Result<int>> expected
    )
    {
        // Arrange

        // Act
        var actual = await Task.WhenAll(input.DoAwaitEach(doType, transform));

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    private class DoAwaitEach_TSucc_TResult_Should_Throw_UnmappedException_Data
        : TheoryData<List<Task<Result<int>>>, DoType, Func<int, Task<Result<string>>>, Exception>
    {
        public DoAwaitEach_TSucc_TResult_Should_Throw_UnmappedException_Data()
        {
            // Scenario C1: function throws, unhandled => FormatException
            Add(
                [Task.FromResult((Result<int>)10), Task.FromResult((Result<int>)20)],
                DoType.MapErrors,
                async i =>
                {
                    await Task.Yield();
                    // Throw an unmapped exception
                    throw new FormatException("Unhandled format err.");
                },
                new FormatException("Unhandled format err.")
            );
        }
    }

    [Theory]
    [ClassData(typeof(DoAwaitEach_TSucc_TResult_Should_Throw_UnmappedException_Data))]
    public async Task DoAwaitEach_TSucc_TResult_should_Throw_UnmappedException(
        List<Task<Result<int>>> input,
        DoType doType,
        Func<int, Task<Result<string>>> transform,
        Exception expected
    )
    {
        // Arrange

        // Act
        var act = async () =>
        {
            var tasks = input.DoAwaitEach(doType, transform);
            // Force iteration
            await Task.WhenAll(tasks);
        };

        // Assert
        await act.Should()
            .ThrowAsync<Exception>()
            .WithMessage(expected.Message)
            .Where(x => x.GetType() == expected.GetType());
    }

    // 11 END


    // ===================================================================================
    // 12) DoAwaitEach<TSucc, TResult>
    //    (this IEnumerable<Task<Result<TSucc>>> results,
    //     DoType type,
    //     Func<TSucc, Task<TResult>> function,
    //     ExceptionFilter mapException)
    //
    // We will test:
    //  - Scenario A: All success, no exceptions thrown
    //  - Scenario B: Some failures in the input (with DoType.MapErrors / Ignore)
    //  - Scenario C: The function throws a mapped exception (should become a failure)
    //  - Scenario D: The function throws an unmapped exception (should be rethrown)
    // ===================================================================================

    private class DoAwaitEach_TSucc_TResult_Overload_Should_Handle_Success_Data
        : TheoryData<
            List<Task<Result<int>>>,
            DoType,
            Func<int, Task<int>>,
            bool /*mapAll*/
            ,
            List<Result<int>>
        >
    {
        public DoAwaitEach_TSucc_TResult_Overload_Should_Handle_Success_Data()
        {
            // Scenario A1: All success, DoType.MapErrors
            Add(
                [
                    Task.FromResult((Result<int>)1),
                    Task.FromResult((Result<int>)2),
                    Task.FromResult((Result<int>)5),
                ],
                DoType.MapErrors,
                async x =>
                {
                    await Task.Delay(1);
                    // Just a pass-through
                    return x + 100;
                },
                true, // mapAll => every exception is mapped, though we won't throw
                // We expect original successes to remain intact
                [1, 2, 5]
            );

            // Scenario A2: All success, DoType.Ignore
            Add(
                [Task.FromResult((Result<int>)10), Task.FromResult((Result<int>)20)],
                DoType.Ignore,
                async x =>
                {
                    await Task.Yield();
                    // pass-through
                    return x + 1000;
                },
                true,
                [10, 20]
            );
        }
    }

    [Theory]
    [ClassData(typeof(DoAwaitEach_TSucc_TResult_Overload_Should_Handle_Success_Data))]
    public async Task DoAwaitEach_TSucc_TResult_Overload_Should_Handle_Success(
        List<Task<Result<int>>> subject,
        DoType doType,
        Func<int, Task<int>> function,
        bool mapAll,
        List<Result<int>> expected
    )
    {
        // Arrange
        Errors.ExceptionFilter mapException = mapAll ? Errors.MapAll : Errors.MapNone;

        // Act
        var tasks = subject.DoAwaitEach(doType, function, mapException);
        var actual = await Task.WhenAll(tasks);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    private class DoAwaitEach_TSucc_TResult_Overload_Should_Handle_Failures_Data
        : TheoryData<
            List<Task<Result<int>>>,
            DoType,
            Func<int, Task<int>>,
            bool /*mapAll*/
            ,
            List<Result<int>>
        >
    {
        public DoAwaitEach_TSucc_TResult_Overload_Should_Handle_Failures_Data()
        {
            // Scenario B1: Some already failures, DoType.MapErrors
            Add(
                [
                    Task.FromResult((Result<int>)new InvalidOperationException("Fail #1")),
                    Task.FromResult((Result<int>)42),
                ],
                DoType.MapErrors,
                async x =>
                {
                    await Task.Delay(1);
                    // pass-through
                    return x * 10;
                },
                false, // no mapping for thrown except
                [new InvalidOperationException("Fail #1"), 42]
            );

            // Scenario B2: multiple failures, DoType.Ignore
            Add(
                [
                    Task.FromResult((Result<int>)new NullReferenceException("NullEx #2")),
                    Task.FromResult((Result<int>)new InvalidOperationException("IOEx #3")),
                    Task.FromResult((Result<int>)99),
                ],
                DoType.Ignore,
                async x =>
                {
                    await Task.Yield();
                    // pass-through
                    return x;
                },
                true, // mapAll => though none thrown
                [
                    new NullReferenceException("NullEx #2"),
                    new InvalidOperationException("IOEx #3"),
                    99,
                ]
            );
        }
    }

    [Theory]
    [ClassData(typeof(DoAwaitEach_TSucc_TResult_Overload_Should_Handle_Failures_Data))]
    public async Task DoAwaitEach_TSucc_TResult_Overload_Should_Handle_Failures(
        List<Task<Result<int>>> subject,
        DoType doType,
        Func<int, Task<int>> function,
        bool mapAll,
        List<Result<int>> expected
    )
    {
        // Arrange
        Errors.ExceptionFilter mapException = mapAll ? Errors.MapAll : Errors.MapNone;

        // Act
        var tasks = subject.DoAwaitEach(doType, function, mapException);
        var awaited = await Task.WhenAll(tasks);

        // Assert
        awaited.Should().BeEquivalentTo(expected);
    }

    private class DoAwaitEach_TSucc_TResult_Overload_Should_MapException_Data
        : TheoryData<
            List<Task<Result<int>>>,
            DoType,
            Func<int, Task<int>>,
            Errors.ExceptionFilter,
            List<Result<int>>
        >
    {
        public DoAwaitEach_TSucc_TResult_Overload_Should_MapException_Data()
        {
            // Scenario C: function throws a mapped exception => transforms to failure
            Add(
                [Task.FromResult((Result<int>)10), Task.FromResult((Result<int>)20)],
                DoType.MapErrors,
                async x =>
                {
                    await Task.Delay(1);
                    throw new InvalidOperationException("Mapped IOEx");
                },
                // We'll map all => the thrown exception becomes a failure
                Errors.MapAll,
                [
                    new InvalidOperationException("Mapped IOEx"),
                    new InvalidOperationException("Mapped IOEx"),
                ]
            );
        }
    }

    [Theory]
    [ClassData(typeof(DoAwaitEach_TSucc_TResult_Overload_Should_MapException_Data))]
    public async Task DoAwaitEach_TSucc_TResult_Overload_Should_MapException(
        List<Task<Result<int>>> subject,
        DoType doType,
        Func<int, Task<int>> function,
        Errors.ExceptionFilter mapException,
        List<Result<int>> expected
    )
    {
        // Arrange

        // Act
        var tasks = subject.DoAwaitEach(doType, function, mapException);
        var actual = await Task.WhenAll(tasks);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    private class DoAwaitEach_TSucc_TResult_Overload_Should_Propagate_UnmappedException_Data
        : TheoryData<
            List<Task<Result<int>>>,
            DoType,
            Func<int, Task<int>>,
            Errors.ExceptionFilter,
            Exception
        >
    {
        public DoAwaitEach_TSucc_TResult_Overload_Should_Propagate_UnmappedException_Data()
        {
            // Scenario D: function throws a not mapped exception => rethrow
            Add(
                [Task.FromResult((Result<int>)100), Task.FromResult((Result<int>)51)],
                DoType.Ignore,
                async x =>
                {
                    await Task.Yield();
                    throw new NullReferenceException("We do not map this");
                },
                Errors.MapNone,
                new NullReferenceException("We do not map this")
            );
        }
    }

    [Theory]
    [ClassData(typeof(DoAwaitEach_TSucc_TResult_Overload_Should_Propagate_UnmappedException_Data))]
    public async Task DoAwaitEach_TSucc_TResult_Overload_Should_Propagate_UnmappedException(
        List<Task<Result<int>>> subject,
        DoType doType,
        Func<int, Task<int>> function,
        Errors.ExceptionFilter mapException,
        Exception expected
    )
    {
        // Arrange

        // Act
        var act = async () =>
        {
            var tasks = subject.DoAwaitEach(doType, function, mapException);
            var awaited = await Task.WhenAll(tasks);
        };

        // Assert
        await act.Should()
            .ThrowAsync<Exception>()
            .WithMessage(expected.Message)
            .Where(x => x.GetType() == expected.GetType());
    }

    // 12 END

    // =========================================================================
    // 13) DoAwaitEach<TSucc>(
    //      this IEnumerable<Task<Result<TSucc>>> results,
    //      DoType type,
    //      Func<TSucc, Task> function,
    //      ExceptionFilter mapException
    //    )
    //
    // Summary:
    //   This extension method applies `DoAwait(...)` to each element. It accepts
    //   an exception filter to potentially map thrown exceptions to failures.
    //   We'll test various scenarios to ensure robust failure handling.
    //
    // Scenarios:
    //   A) All input successes, function runs without throwing => remain successes.
    //   B) Some input failures => remain failures; successes are unaffected.
    //   C) Function throws an exception that is mapped => success → new failure.
    //   D) Function throws an exception that is not mapped => propagated.
    // ============================================================================

    // -------------------------------------------------------------
    // Scenario A: All input successes, function does not throw
    // -------------------------------------------------------------

    private class DoAwaitEach_TSucc_WithoutThrow_ScenarioA_Data
        : TheoryData<
            List<Task<Result<int>>>,
            DoType,
            Func<int, Task>,
            Errors.ExceptionFilter,
            List<Result<int>>
        >
    {
        public DoAwaitEach_TSucc_WithoutThrow_ScenarioA_Data()
        {
            // A1: DoType.MapErrors, no exception
            Add(
                [Task.FromResult((Result<int>)10), Task.FromResult((Result<int>)20)],
                DoType.MapErrors,
                async x => await Task.Delay(1),
                Errors.MapAll,
                [10, 20]
            );

            // A2: DoType.Ignore, no exception
            Add(
                [Task.FromResult((Result<int>)30), Task.FromResult((Result<int>)40)],
                DoType.Ignore,
                async x => await Task.Yield(),
                Errors.MapNone, // shouldn't matter as no throws
                [30, 40]
            );
        }
    }

    [Theory]
    [ClassData(typeof(DoAwaitEach_TSucc_WithoutThrow_ScenarioA_Data))]
    public async Task DoAwaitEach_TSucc_WithoutThrow_Should_Retain_Successes(
        List<Task<Result<int>>> input,
        DoType doType,
        Func<int, Task> function,
        Errors.ExceptionFilter mapException,
        List<Result<int>> expected
    )
    {
        // Act
        var tasks = input.DoAwaitEach(doType, function, mapException);
        var actual = await Task.WhenAll(tasks);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    // -------------------------------------------------------------
    // Scenario B: Some input failures => remain failures
    // -------------------------------------------------------------

    private class DoAwaitEach_TSucc_WithFailures_ScenarioB_Data
        : TheoryData<
            List<Task<Result<int>>>,
            DoType,
            Func<int, Task>,
            Errors.ExceptionFilter,
            List<Result<int>>
        >
    {
        public DoAwaitEach_TSucc_WithFailures_ScenarioB_Data()
        {
            // B1: With pre-existing failures
            Add(
                [
                    Task.FromResult((Result<int>)new InvalidOperationException("Fail #1")),
                    Task.FromResult((Result<int>)(50)),
                ],
                DoType.MapErrors,
                async x => await Task.CompletedTask, // No-op success function
                Errors.MapAll,
                [new InvalidOperationException("Fail #1"), 50]
            );
        }
    }

    [Theory]
    [ClassData(typeof(DoAwaitEach_TSucc_WithFailures_ScenarioB_Data))]
    public async Task DoAwaitEach_TSucc_WithFailures_Should_Retain_Failures(
        List<Task<Result<int>>> input,
        DoType doType,
        Func<int, Task> function,
        Errors.ExceptionFilter mapException,
        List<Result<int>> expected
    )
    {
        // Act
        var tasks = input.DoAwaitEach(doType, function, mapException);
        var actual = await Task.WhenAll(tasks);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    // -------------------------------------------------------------
    // Scenario C: Function throws exception, mapped to failure
    // -------------------------------------------------------------

    private class DoAwaitEach_TSucc_WithMappedThrow_ScenarioC_Data
        : TheoryData<
            List<Task<Result<int>>>,
            DoType,
            Func<int, Task>,
            Errors.ExceptionFilter,
            List<Result<int>>
        >
    {
        public DoAwaitEach_TSucc_WithMappedThrow_ScenarioC_Data()
        {
            // C1: Function throws InvalidOperationException => is mapped
            Add(
                [Task.FromResult((Result<int>)25), Task.FromResult((Result<int>)30)],
                DoType.MapErrors,
                async _ =>
                {
                    await Task.Yield();
                    throw new InvalidOperationException("Intentional throw");
                },
                Errors.MapIfExceptionIs<InvalidOperationException>(),
                [
                    new InvalidOperationException("Intentional throw"),
                    new InvalidOperationException("Intentional throw"),
                ]
            );
        }
    }

    [Theory]
    [ClassData(typeof(DoAwaitEach_TSucc_WithMappedThrow_ScenarioC_Data))]
    public async Task DoAwaitEach_TSucc_WithMappedThrow_Should_Become_Failures(
        List<Task<Result<int>>> input,
        DoType doType,
        Func<int, Task> function,
        Errors.ExceptionFilter mapException,
        List<Result<int>> expected
    )
    {
        // Act
        var tasks = input.DoAwaitEach(doType, function, mapException);
        var actual = await Task.WhenAll(tasks);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    // -------------------------------------------------------------
    // Scenario D: Function throws exception, not mapped => rethrow
    // -------------------------------------------------------------

    private class DoAwaitEach_TSucc_UnmappedThrow_ScenarioD_Data
        : TheoryData<
            List<Task<Result<int>>>,
            DoType,
            Func<int, Task>,
            Errors.ExceptionFilter,
            Exception
        >
    {
        public DoAwaitEach_TSucc_UnmappedThrow_ScenarioD_Data()
        {
            // D1: Function throws NullReferenceException, filter doesn't map it
            Add(
                [Task.FromResult((Result<int>)10), Task.FromResult((Result<int>)0)],
                DoType.Ignore,
                async _ =>
                {
                    await Task.Delay(1);
                    throw new NullReferenceException("Not mapped");
                },
                Errors.MapIfExceptionIs<InvalidOperationException>(),
                new NullReferenceException("Not mapped")
            );
        }
    }

    [Theory]
    [ClassData(typeof(DoAwaitEach_TSucc_UnmappedThrow_ScenarioD_Data))]
    public void DoAwaitEach_TSucc_UnmappedThrow_Should_Propagate_Exception(
        List<Task<Result<int>>> input,
        DoType doType,
        Func<int, Task> function,
        Errors.ExceptionFilter mapException,
        Exception expected
    )
    {
        // Act
        var act = async () =>
        {
            var tasks = input.DoAwaitEach(doType, function, mapException);
            await Task.WhenAll(tasks);
        };

        // Assert
        act.Should()
            .ThrowAsync<Exception>()
            .WithMessage(expected.Message)
            .Where(x => x.GetType() == expected.GetType());
    }

    // 13 END

    // ========================================================================================
    // 14) DoEach<TSucc, TResult>(
    //      this IEnumerable<Task<Result<TSucc>>> results,
    //      DoType type,
    //      Func<TSucc, Result<TResult>> function
    //    )
    //
    // Summary:
    //   Applies Do(...) to each element of "IEnumerable<Task<Result<TSucc>>>".
    //   The core Do(...) can transform success → failure or ignore them,
    //   but any physically thrown exceptions are unhandled (bubbling up).
    //
    // Scenarios:
    //   A) All input successes, function returns success => remain successes.
    //   B) Some input failures => remain as failures; successes remain successes.
    //   C) The function physically throws => unhandled exception is rethrown.
    //   D) The function returns failures:
    //      - DoType.MapErrors => original success becomes new failure
    //      - DoType.Ignore => original success remains success
    //
    // We'll split these scenarios into multiple TheoryData classes for clarity.
    // ========================================================================================

    // ====================================================
    // Scenario A: All input successes, function => success
    // ====================================================
    private class DoEach_TSucc_TResult_ScenarioA_Data
        : TheoryData<List<Task<Result<int>>>, DoType, Func<int, Result<string>>, List<Result<int>>>
    {
        public DoEach_TSucc_TResult_ScenarioA_Data()
        {
            // A1) DoType.MapErrors, function always returns success
            Add(
                [
                    Task.FromResult((Result<int>)10),
                    Task.FromResult((Result<int>)20),
                    Task.FromResult((Result<int>)30),
                ],
                DoType.MapErrors,
                _ => (Result<string>)"AlwaysSuccess",
                [10, 20, 30]
            );

            // A2) DoType.Ignore, function always returns success
            Add(
                [Task.FromResult((Result<int>)1), Task.FromResult((Result<int>)2)],
                DoType.Ignore,
                x => (Result<string>)$"Val:{x}",
                [1, 2]
            );
        }
    }

    [Theory]
    [ClassData(typeof(DoEach_TSucc_TResult_ScenarioA_Data))]
    public async Task DoEach_TSucc_TResult_scenarioA_Should_RetainSuccess(
        List<Task<Result<int>>> input,
        DoType doType,
        Func<int, Result<string>> function,
        List<Result<int>> expected
    )
    {
        // Act
        var actualTasks = input.DoEach(doType, function);
        var actual = await Task.WhenAll(actualTasks);

        // Assert
        actual
            .Should()
            .BeEquivalentTo(expected, because: "function returns success => no change to success");
    }

    // ==============================================
    // Scenario B: Some input failures => remain so
    // ==============================================
    private class DoEach_TSucc_TResult_ScenarioB_Data
        : TheoryData<List<Task<Result<int>>>, DoType, Func<int, Result<string>>, List<Result<int>>>
    {
        public DoEach_TSucc_TResult_ScenarioB_Data()
        {
            // B1) DoType.MapErrors, function returns success
            Add(
                [
                    Task.FromResult((Result<int>)new InvalidOperationException("Fail #1")),
                    Task.FromResult((Result<int>)42),
                    Task.FromResult((Result<int>)new ArgumentException("Fail #2")),
                ],
                DoType.MapErrors,
                i => (Result<string>)$"OK:{i}", // always success
                // Already-fail items remain fail, success remains success
                [new InvalidOperationException("Fail #1"), 42, new ArgumentException("Fail #2")]
            );

            // B2) DoType.Ignore, function returns success
            Add(
                [
                    Task.FromResult((Result<int>)new FormatException("FmtFail #A")),
                    Task.FromResult((Result<int>)(-99)),
                ],
                DoType.Ignore,
                i => (Result<string>)$"PosVal:{i}", // always success
                [new FormatException("FmtFail #A"), -99]
            );
        }
    }

    [Theory]
    [ClassData(typeof(DoEach_TSucc_TResult_ScenarioB_Data))]
    public async Task DoEach_TSucc_TResult_scenarioB_Should_RetainExistingFailures(
        List<Task<Result<int>>> input,
        DoType doType,
        Func<int, Result<string>> function,
        List<Result<int>> expected
    )
    {
        // Act
        var actualTasks = input.DoEach(doType, function);
        var actual = await Task.WhenAll(actualTasks);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    // ======================================================
    // Scenario C: The function physically throws == unhandled
    // ======================================================
    private class DoEach_TSucc_TResult_ScenarioC_Data
        : TheoryData<List<Task<Result<int>>>, DoType, Func<int, Result<string>>, Exception>
    {
        public DoEach_TSucc_TResult_ScenarioC_Data()
        {
            // C1) function physically throws for certain values
            Add(
                [Task.FromResult((Result<int>)10), Task.FromResult((Result<int>)5)],
                DoType.MapErrors,
                i =>
                {
                    if (i >= 10)
                    {
                        throw new InvalidOperationException($"Func physically threw for i={i}");
                    }

                    return (Result<string>)"SafeValue";
                },
                new InvalidOperationException("Func physically threw for i=10")
            );
        }
    }

    [Theory]
    [ClassData(typeof(DoEach_TSucc_TResult_ScenarioC_Data))]
    public async Task DoEach_TSucc_TResult_scenarioC_Should_PropagateUncaughtExceptions(
        List<Task<Result<int>>> input,
        DoType doType,
        Func<int, Result<string>> function,
        Exception expected
    )
    {
        // Act
        var act = async () =>
        {
            var tasks = input.DoEach(doType, function);
            await Task.WhenAll(tasks);
        };

        // Assert
        await act.Should()
            .ThrowAsync<Exception>()
            .WithMessage(expected.Message)
            .Where(x => x.GetType() == expected.GetType());
    }

    // =====================================================================
    // Scenario D: The function returns failure
    //  - DoType.MapErrors => success transforms → failure
    //  - DoType.Ignore   => success remains success
    //  * If input is already failure, it remains so.
    // =====================================================================
    private class DoEach_TSucc_TResult_ScenarioD_Data
        : TheoryData<List<Task<Result<int>>>, DoType, Func<int, Result<string>>, List<Result<int>>>
    {
        public DoEach_TSucc_TResult_ScenarioD_Data()
        {
            // D1) function always returns failure => DoType.MapErrors => any success -> new failure
            Add(
                [Task.FromResult((Result<int>)1), Task.FromResult((Result<int>)100)],
                DoType.MapErrors,
                i => (Result<string>)new NullReferenceException("FunReturnedFailure"),
                [
                    new NullReferenceException("FunReturnedFailure"),
                    new NullReferenceException("FunReturnedFailure"),
                ]
            );

            // D2) function always returns failure => DoType.Ignore => original success remains success
            Add(
                [Task.FromResult((Result<int>)(-5)), Task.FromResult((Result<int>)500)],
                DoType.Ignore,
                i => (Result<string>)new ArgumentException($"Fail for {i}"),
                [-5, 500]
            );

            // D3) Mixed input & function conditionally returns failure
            //   even -> success, odd -> new FormatException
            //   doType => MapErrors => success -> failure if function fails
            Add(
                [
                    Task.FromResult((Result<int>)new InvalidOperationException("AlreadyFail")),
                    Task.FromResult((Result<int>)3), // odd => new fail
                    Task.FromResult((Result<int>)8), // even => success
                ],
                DoType.MapErrors,
                i =>
                    (i % 2 == 0)
                        ? (Result<string>)"EvenOk"
                        : (Result<string>)new FormatException("Odd => fail"),
                [
                    new InvalidOperationException("AlreadyFail"),
                    new FormatException("Odd => fail"),
                    8,
                ]
            );
        }
    }

    [Theory]
    [ClassData(typeof(DoEach_TSucc_TResult_ScenarioD_Data))]
    public async Task DoEach_TSucc_TResult_scenarioD_Should_ApplyFailuresFromFunction(
        List<Task<Result<int>>> input,
        DoType doType,
        Func<int, Result<string>> function,
        List<Result<int>> expected
    )
    {
        // Act
        var tasks = input.DoEach(doType, function);
        var actual = await Task.WhenAll(tasks);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    // 14 END

    // ========================
    // 15) DoEach (Overload B)
    // ========================

    // Success scenarios for DoEach (Overload B)
    private class DoEach_OverloadB_Success_Data
        : TheoryData<
            IEnumerable<Task<Result<int>>>,
            Func<int, int>,
            DoType,
            IEnumerable<Task<Result<int>>>
        >
    {
        public DoEach_OverloadB_Success_Data()
        {
            // Identity transformation with Ignore DoType
            Add(
                [Task.FromResult((Result<int>)1), Task.FromResult((Result<int>)2)],
                x => x,
                DoType.Ignore,
                [Task.FromResult((Result<int>)1), Task.FromResult((Result<int>)2)]
            );

            // Increment transformation with Ignore DoType
            Add(
                [Task.FromResult((Result<int>)2), Task.FromResult((Result<int>)11)],
                x => x + 1,
                DoType.Ignore,
                [Task.FromResult((Result<int>)2), Task.FromResult((Result<int>)11)]
            );

            // Transformation to negative with Ignore DoType
            Add(
                [Task.FromResult((Result<int>)(-1))],
                x => -x,
                DoType.Ignore,
                [Task.FromResult((Result<int>)(int)-1)]
            );
        }
    }

    // AsyncResultCollectionExtensions.DoEach should return equivalent results for successful transformations
    [Theory]
    [ClassData(typeof(DoEach_OverloadB_Success_Data))]
    public async Task DoEach_Should_ReturnSuccess(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, int> transformation,
        DoType doType,
        IEnumerable<Task<Result<int>>> expected
    )
    {
        // Act
        var actual = await Task.WhenAll(inputs.DoEach(doType, transformation, Errors.MapNone));
        var ex = await Task.WhenAll(expected);
        // Assert
        actual.Should().BeEquivalentTo(ex);
    }

    // Failure scenarios for DoEach (Overload B)
    private class DoEach_OverloadB_Failure_Data
        : TheoryData<
            IEnumerable<Task<Result<int>>>,
            Func<int, int>,
            DoType,
            IEnumerable<Task<Result<int>>>
        >
    {
        public DoEach_OverloadB_Failure_Data()
        {
            // Fails with mapped InvalidOperationException when using MapErrors DoType
            Add(
                [Task.FromResult((Result<int>)1)],
                _ => throw new InvalidOperationException("Invalid operation"),
                DoType.MapErrors,
                [Task.FromResult((Result<int>)new InvalidOperationException("Invalid operation"))]
            );

            // Failure due to negative to positive constraint when using MapErrors DoType
            Add(
                [Task.FromResult((Result<int>)(-1))],
                x => throw new NullReferenceException($"Value must be non-negative but got {x}"),
                DoType.MapErrors,
                [
                    Task.FromResult(
                        (Result<int>)
                            new NullReferenceException("Value must be non-negative but got -1")
                    ),
                ]
            );
        }
    }

    // AsyncResultCollectionExtensions.DoEach should handle exceptions and return failures when they occur
    [Theory]
    [ClassData(typeof(DoEach_OverloadB_Failure_Data))]
    public async Task DoEach_Should_ReturnFailure(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, int> transformation,
        DoType doType,
        IEnumerable<Task<Result<int>>> expected
    )
    {
        // Act
        var actual = await Task.WhenAll(inputs.DoEach(doType, transformation, Errors.MapAll));
        var ex = await Task.WhenAll(expected);
        // Assert
        actual.Should().BeEquivalentTo(ex);
    }

    // Exception Propagation scenarios for DoEach (Overload B)
    private class DoEach_OverloadB_PropagateException_Data
        : TheoryData<IEnumerable<Task<Result<int>>>, Func<int, int>, DoType, Exception>
    {
        public DoEach_OverloadB_PropagateException_Data()
        {
            // Propagates FormatException that is not mapped
            Add(
                [Task.FromResult((Result<int>)5)],
                _ => throw new FormatException("Invalid format exception"),
                DoType.Ignore,
                new FormatException("Invalid format exception")
            );

            // Propagates NullReferenceException that is not mapped
            Add(
                [Task.FromResult((Result<int>)0)],
                _ => throw new NullReferenceException("Null reference error"),
                DoType.Ignore,
                new NullReferenceException("Null reference error")
            );
        }
    }

    // AsyncResultCollectionExtensions.DoEach should propagate exceptions that aren't mapped
    [Theory]
    [ClassData(typeof(DoEach_OverloadB_PropagateException_Data))]
    public async Task DoEach_Should_PropagateUnmappedException(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, int> transformation,
        DoType doType,
        Exception expected
    )
    {
        // Act
        var act = async () =>
            await Task.WhenAll(inputs.DoEach(doType, transformation, Errors.MapNone));

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage(expected.Message);
    }

    // 15 END

    // ==============
    // DoEach (Overload C)
    // ==============

    // Success scenarios for DoEach (Overload C)
    private class DoEach_OverloadC_Success_Data
        : TheoryData<
            IEnumerable<Task<Result<int>>>,
            Action<int>,
            DoType,
            IEnumerable<Task<Result<int>>>
        >
    {
        public DoEach_OverloadC_Success_Data()
        {
            // Identity no-op action with Ignore DoType
            Add(
                [Task.FromResult((Result<int>)1)],
                x => { },
                DoType.Ignore,
                [Task.FromResult((Result<int>)1)]
            );

            // Logging action with Ignore DoType
            Add(
                [Task.FromResult((Result<int>)2)],
                x => Console.WriteLine($"Processed value: {x}"),
                DoType.Ignore,
                [Task.FromResult((Result<int>)2)]
            );
        }
    }

    // AsyncResultCollectionExtensions.DoEach (Overload C) should complete successfully for no-op actions
    [Theory]
    [ClassData(typeof(DoEach_OverloadC_Success_Data))]
    public async Task DoEachC_Should_ReturnSuccess(
        IEnumerable<Task<Result<int>>> inputs,
        Action<int> action,
        DoType doType,
        IEnumerable<Task<Result<int>>> expected
    )
    {
        // Act
        var results = inputs.DoEach(doType, action, Errors.MapNone);

        // Assert
        (await Task.WhenAll(results))
            .Should()
            .BeEquivalentTo(await Task.WhenAll(expected));
    }

    // Failure scenarios for DoEach (Overload C)
    private class DoEach_OverloadC_Failure_Data
        : TheoryData<
            IEnumerable<Task<Result<int>>>,
            Action<int>,
            DoType,
            IEnumerable<Task<Result<int>>>
        >
    {
        public DoEach_OverloadC_Failure_Data()
        {
            // Method fails by throwing InvalidOperationException with MapErrors DoType
            Add(
                [Task.FromResult((Result<int>)1)],
                _ => throw new InvalidOperationException("Invalid action execution"),
                DoType.MapErrors,
                [
                    Task.FromResult(
                        (Result<int>)new InvalidOperationException("Invalid action execution")
                    ),
                ]
            );

            // Method fails with ArgumentOutOfRangeException using MapErrors DoType
            Add(
                [Task.FromResult((Result<int>)3)],
                _ => throw new NullReferenceException("Index out of range during action execution"),
                DoType.MapErrors,
                [
                    Task.FromResult(
                        (Result<int>)
                            new NullReferenceException("Index out of range during action execution")
                    ),
                ]
            );
        }
    }

    // AsyncResultCollectionExtensions.DoEach (Overload C) should handle failures and map exceptions properly
    [Theory]
    [ClassData(typeof(DoEach_OverloadC_Failure_Data))]
    public async Task DoEachC_Should_ReturnFailure(
        IEnumerable<Task<Result<int>>> inputs,
        Action<int> action,
        DoType doType,
        IEnumerable<Task<Result<int>>> expected
    )
    {
        // Act
        var results = inputs.DoEach(doType, action, Errors.MapAll);

        // Assert
        (await Task.WhenAll(results))
            .Should()
            .BeEquivalentTo(await Task.WhenAll(expected));
    }

    // Exception Propagation scenarios for DoEach (Overload C)
    private class DoEach_OverloadC_PropagateException_Data
        : TheoryData<IEnumerable<Task<Result<int>>>, Action<int>, DoType, Exception>
    {
        public DoEach_OverloadC_PropagateException_Data()
        {
            // FormatException propagates when unmapped
            Add(
                [Task.FromResult((Result<int>)5)],
                _ => throw new FormatException("FormatException in action"),
                DoType.Ignore,
                new FormatException("FormatException in action")
            );

            // NullReferenceException propagates when unmapped
            Add(
                [Task.FromResult((Result<int>)0)],
                _ => throw new NullReferenceException("NullReferenceException in action"),
                DoType.Ignore,
                new NullReferenceException("NullReferenceException in action")
            );
        }
    }

    // AsyncResultCollectionExtensions.DoEach (Overload C) should propagate unmapped exceptions properly
    [Theory]
    [ClassData(typeof(DoEach_OverloadC_PropagateException_Data))]
    public async Task DoEachC_Should_PropagateUnmappedException(
        IEnumerable<Task<Result<int>>> inputs,
        Action<int> action,
        DoType doType,
        Exception expected
    )
    {
        // Act
        var act = async () => await Task.WhenAll(inputs.DoEach(doType, action, Errors.MapNone));

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage(expected.Message);
    }

    // 16 END


    // ===============================
    // 17) ThenAwaitEach (Overload A)
    // ===============================

    // Success scenarios for ThenAwaitEach (Overload A)
    private class ThenAwaitEach_OverloadA_Success_Data
        : TheoryData<
            IEnumerable<Task<Result<int>>>,
            Func<int, Task<Result<string>>>,
            IEnumerable<Task<Result<string>>>
        >
    {
        public ThenAwaitEach_OverloadA_Success_Data()
        {
            // Async transformation to a prefixed string
            Add(
                [Task.FromResult((Result<int>)1)],
                async val => await Task.FromResult((Result<string>)$"Prefixed-{val}"),
                [Task.FromResult((Result<string>)"Prefixed-1")]
            );

            // Async multiplication and conversion to string
            Add(
                [Task.FromResult((Result<int>)2)],
                async val => await Task.FromResult((Result<string>)($"Doubled-{val * 2}")),
                [Task.FromResult((Result<string>)"Doubled-4")]
            );
        }
    }

    // AsyncResultCollectionExtensions.ThenAwaitEach should successfully apply async transformations
    [Theory]
    [ClassData(typeof(ThenAwaitEach_OverloadA_Success_Data))]
    public async Task ThenAwaitEachA_Should_ReturnSuccess(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, Task<Result<string>>> transform,
        IEnumerable<Task<Result<string>>> expected
    )
    {
        // Act
        var actual = await Task.WhenAll(inputs.ThenAwaitEach(transform));

        // Assert
        actual.Should().BeEquivalentTo(await Task.WhenAll(expected));
    }

    // Failure scenarios for ThenAwaitEach (Overload A)
    private class ThenAwaitEach_OverloadA_Failure_Data
        : TheoryData<
            IEnumerable<Task<Result<int>>>,
            Func<int, Task<Result<string>>>,
            IEnumerable<Task<Result<string>>>
        >
    {
        public ThenAwaitEach_OverloadA_Failure_Data()
        {
            // Transformation fails with InvalidOperationException
            Add(
                [Task.FromResult((Result<int>)1)],
                async val =>
                    await Task.FromResult(
                        (Result<string>)new InvalidOperationException("Transformation failure")
                    ),
                [
                    Task.FromResult(
                        (Result<string>)new InvalidOperationException("Transformation failure")
                    ),
                ]
            );

            // Transformation fails due to null value constraint
            Add(
                [Task.FromResult((Result<int>)(-1))],
                async val =>
                    await Task.FromResult(
                        (Result<string>)new NullReferenceException("Value cannot be null")
                    ),
                [
                    Task.FromResult(
                        (Result<string>)new NullReferenceException("Value cannot be null")
                    ),
                ]
            );
        }
    }

    // AsyncResultCollectionExtensions.ThenAwaitEach should correctly capture failures in transformations
    [Theory]
    [ClassData(typeof(ThenAwaitEach_OverloadA_Failure_Data))]
    public async Task ThenAwaitEachA_Should_CaptureFailure(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, Task<Result<string>>> transform,
        IEnumerable<Task<Result<string>>> expected
    )
    {
        // Act
        var actual = await Task.WhenAll(inputs.ThenAwaitEach(transform));

        // Assert
        actual.Should().BeEquivalentTo(await Task.WhenAll(expected));
    }

    // Exception Propagation scenarios for ThenAwaitEach (Overload A)
    private class ThenAwaitEach_OverloadA_PropagateException_Data
        : TheoryData<IEnumerable<Task<Result<int>>>, Func<int, Task<Result<string>>>, Exception>
    {
        public ThenAwaitEach_OverloadA_PropagateException_Data()
        {
            // FormatException during async operation
            Add(
                [Task.FromResult((Result<int>)5)],
                async val =>
                {
                    await Task.Yield();
                    throw new FormatException("Async formatting exception");
                },
                new FormatException("Async formatting exception")
            );

            // NullReferenceException in async transformation
            Add(
                [Task.FromResult((Result<int>)0)],
                async val =>
                {
                    await Task.Yield();
                    throw new NullReferenceException("Null reference in async operation");
                },
                new NullReferenceException("Null reference in async operation")
            );
        }
    }

    // AsyncResultCollectionExtensions.ThenAwaitEach should propagate unmapped exceptions in async transformations
    [Theory]
    [ClassData(typeof(ThenAwaitEach_OverloadA_PropagateException_Data))]
    public async Task ThenAwaitEachA_Should_PropagateUnmappedException(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, Task<Result<string>>> transform,
        Exception expected
    )
    {
        // Act
        Func<Task> act = () => Task.WhenAll(inputs.ThenAwaitEach(transform));

        // Assert
        await act.Should()
            .ThrowAsync<Exception>()
            .WithMessage(expected.Message)
            .Where(x => x.GetType() == expected.GetType());
    }

    // 17 END

    // ===============================
    // 18) ThenAwaitEach (Overload B)
    // ===============================

    // Success scenarios for ThenAwaitEach (Overload B)
    private class ThenAwaitEach_OverloadB_Success_Data
        : TheoryData<
            IEnumerable<Task<Result<int>>>,
            Func<int, Task<string>>,
            IEnumerable<Task<Result<string>>>
        >
    {
        public ThenAwaitEach_OverloadB_Success_Data()
        {
            // Async transformation to append suffix
            Add(
                [Task.FromResult((Result<int>)1)],
                async val => await Task.FromResult($"Appended-{val}"),
                [Task.FromResult((Result<string>)"Appended-1")]
            );

            // Async multiply and convert to string
            Add(
                [Task.FromResult((Result<int>)3)],
                async val => await Task.FromResult($"Tripled-{val * 3}"),
                [Task.FromResult((Result<string>)"Tripled-9")]
            );
        }
    }

    // AsyncResultCollectionExtensions.ThenAwaitEach (Overload B) should successfully apply async string transformations
    [Theory]
    [ClassData(typeof(ThenAwaitEach_OverloadB_Success_Data))]
    public async Task ThenAwaitEachB_Should_ReturnSuccess(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, Task<string>> transform,
        IEnumerable<Task<Result<string>>> expected
    )
    {
        // Act
        var actual = await Task.WhenAll(inputs.ThenAwaitEach(transform, Errors.MapNone));

        // Assert
        actual.Should().BeEquivalentTo(await Task.WhenAll(expected));
    }

    // Failure scenarios for ThenAwaitEach (Overload B)
    private class ThenAwaitEach_OverloadB_Failure_Data
        : TheoryData<
            IEnumerable<Task<Result<int>>>,
            Func<int, Task<string>>,
            IEnumerable<Task<Result<string>>>
        >
    {
        public ThenAwaitEach_OverloadB_Failure_Data()
        {
            // Fails with InvalidOperationException
            Add(
                [Task.FromResult((Result<int>)2)],
                async val =>
                {
                    await Task.Yield();
                    throw new InvalidOperationException("Operation failed");
                },
                [Task.FromResult((Result<string>)new InvalidOperationException("Operation failed"))]
            );

            // Fails due to null constraint throwing exception
            Add(
                [Task.FromResult((Result<int>)4)],
                async val =>
                {
                    await Task.Yield();
                    throw new NullReferenceException("Null value encountered");
                },
                [
                    Task.FromResult(
                        (Result<string>)new NullReferenceException("Null value encountered")
                    ),
                ]
            );
        }
    }

    // AsyncResultCollectionExtensions.ThenAwaitEach (Overload B) should handle exceptional transforms as failures
    [Theory]
    [ClassData(typeof(ThenAwaitEach_OverloadB_Failure_Data))]
    public async Task ThenAwaitEachB_Should_CaptureFailure(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, Task<string>> transform,
        IEnumerable<Task<Result<string>>> expected
    )
    {
        // Act
        var actual = await Task.WhenAll(inputs.ThenAwaitEach(transform, Errors.MapAll));

        // Assert
        actual.Should().BeEquivalentTo(await Task.WhenAll(expected));
    }

    // Exception Propagation scenarios for ThenAwaitEach (Overload B)
    private class ThenAwaitEach_OverloadB_PropagateException_Data
        : TheoryData<IEnumerable<Task<Result<int>>>, Func<int, Task<string>>, Exception>
    {
        public ThenAwaitEach_OverloadB_PropagateException_Data()
        {
            // Propagate FormatException
            Add(
                [Task.FromResult((Result<int>)8)],
                async val =>
                {
                    await Task.Yield();
                    throw new FormatException("Unexpected format");
                },
                new FormatException("Unexpected format")
            );

            // Propagate NullReferenceException
            Add(
                [Task.FromResult((Result<int>)10)],
                async val =>
                {
                    await Task.Yield();
                    throw new NullReferenceException("Null reference issue");
                },
                new NullReferenceException("Null reference issue")
            );
        }
    }

    // AsyncResultCollectionExtensions.ThenAwaitEach (Overload B) should propagate unmapped exceptions
    [Theory]
    [ClassData(typeof(ThenAwaitEach_OverloadB_PropagateException_Data))]
    public async Task ThenAwaitEachB_Should_PropagateUnmappedException(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, Task<string>> transform,
        Exception expected
    )
    {
        // Act
        Func<Task> act = () => Task.WhenAll(inputs.ThenAwaitEach(transform, Errors.MapNone));

        // Assert
        await act.Should()
            .ThrowAsync<Exception>()
            .WithMessage(expected.Message)
            .Where(x => x.GetType() == expected.GetType());
    }

    // 18 END

    // ===============================
    // 19) ThenAwaitEach (Overload C)
    // ===============================

    // Success scenarios for ThenAwaitEach (Overload C)
    private class ThenAwaitEach_OverloadC_Success_Data
        : TheoryData<
            IEnumerable<Task<Result<int>>>,
            Func<int, Task>,
            IEnumerable<Task<Result<Unit>>>
        >
    {
        public ThenAwaitEach_OverloadC_Success_Data()
        {
            // Async no-op action
            Add(
                [Task.FromResult((Result<int>)1)],
                async val => await Task.CompletedTask,
                [Task.FromResult((Result<Unit>)new Unit())]
            );

            // Async delay action
            Add(
                [Task.FromResult((Result<int>)2)],
                async val => await Task.Delay(5),
                [Task.FromResult((Result<Unit>)new Unit())]
            );
        }
    }

    // AsyncResultCollectionExtensions.ThenAwaitEach (Overload C) should successfully complete async actions
    [Theory]
    [ClassData(typeof(ThenAwaitEach_OverloadC_Success_Data))]
    public async Task ThenAwaitEachC_Should_ReturnSuccess(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, Task> action,
        IEnumerable<Task<Result<Unit>>> expected
    )
    {
        // Act
        var actual = await Task.WhenAll(inputs.ThenAwaitEach(action, Errors.MapNone));

        // Assert
        actual.Should().BeEquivalentTo(await Task.WhenAll(expected));
    }

    // Failure scenarios for ThenAwaitEach (Overload C)
    private class ThenAwaitEach_OverloadC_Failure_Data
        : TheoryData<
            IEnumerable<Task<Result<int>>>,
            Func<int, Task>,
            IEnumerable<Task<Result<Unit>>>
        >
    {
        public ThenAwaitEach_OverloadC_Failure_Data()
        {
            // Fail due to InvalidOperationException during action
            Add(
                [Task.FromResult((Result<int>)3)],
                async val =>
                {
                    await Task.Yield();
                    throw new InvalidOperationException("Operation failed");
                },
                [Task.FromResult((Result<Unit>)new InvalidOperationException("Operation failed"))]
            );

            // Fail due to NullReferenceException during execution
            Add(
                [Task.FromResult((Result<int>)4)],
                async val =>
                {
                    await Task.Yield();
                    throw new NullReferenceException("Null value encountered");
                },
                [
                    Task.FromResult(
                        (Result<Unit>)new NullReferenceException("Null value encountered")
                    ),
                ]
            );
        }
    }

    // AsyncResultCollectionExtensions.ThenAwaitEach (Overload C) should capture exceptions as failures during actions
    [Theory]
    [ClassData(typeof(ThenAwaitEach_OverloadC_Failure_Data))]
    public async Task ThenAwaitEachC_Should_CaptureFailure(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, Task> action,
        IEnumerable<Task<Result<Unit>>> expected
    )
    {
        // Act
        var actual = await Task.WhenAll(inputs.ThenAwaitEach(action, Errors.MapAll));

        // Assert
        actual.Should().BeEquivalentTo(await Task.WhenAll(expected));
    }

    // Exception Propagation scenarios for ThenAwaitEach (Overload C)
    private class ThenAwaitEach_OverloadC_PropagateException_Data
        : TheoryData<IEnumerable<Task<Result<int>>>, Func<int, Task>, Exception>
    {
        public ThenAwaitEach_OverloadC_PropagateException_Data()
        {
            // Propagate FormatException during async execution
            Add(
                [Task.FromResult((Result<int>)6)],
                async val =>
                {
                    await Task.Yield();
                    throw new FormatException("Unexpected format");
                },
                new FormatException("Unexpected format")
            );

            // Propagate NullReferenceException during async transformation
            Add(
                [Task.FromResult((Result<int>)8)],
                async val =>
                {
                    await Task.Yield();
                    throw new NullReferenceException("Null value error");
                },
                new NullReferenceException("Null value error")
            );
        }
    }

    // AsyncResultCollectionExtensions.ThenAwaitEach (Overload C) should propagate unmapped exceptions
    [Theory]
    [ClassData(typeof(ThenAwaitEach_OverloadC_PropagateException_Data))]
    public async Task ThenAwaitEachC_Should_PropagateUnmappedException(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, Task> action,
        Exception expected
    )
    {
        // Act
        Func<Task> act = () => Task.WhenAll(inputs.ThenAwaitEach(action, Errors.MapNone));

        // Assert
        await act.Should()
            .ThrowAsync<Exception>()
            .WithMessage(expected.Message)
            .Where(x => x.GetType() == expected.GetType());
    }

    // 19 END

    // ===============================
    // 20) ThenEach (Overload A)
    // ===============================

    // Success scenarios for ThenEach (Overload A)
    private class ThenEach_OverloadA_Success_Data
        : TheoryData<
            IEnumerable<Task<Result<int>>>,
            Func<int, Result<string>>,
            IEnumerable<Task<Result<string>>>
        >
    {
        public ThenEach_OverloadA_Success_Data()
        {
            // Simple string transformation
            Add(
                [Task.FromResult((Result<int>)1)],
                val => (Result<string>)($"Converted to string {val}"),
                [Task.FromResult((Result<string>)"Converted to string 1")]
            );

            // Multiply by 2 and convert to string
            Add(
                [Task.FromResult((Result<int>)3)],
                val => (Result<string>)($"Twice the value is {val * 2}"),
                [Task.FromResult((Result<string>)"Twice the value is 6")]
            );
        }
    }

    // AsyncResultCollectionExtensions.ThenEach (Overload A) should apply transformations successfully
    [Theory]
    [ClassData(typeof(ThenEach_OverloadA_Success_Data))]
    public async Task ThenEachA_Should_ReturnSuccess(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, Result<string>> transform,
        IEnumerable<Task<Result<string>>> expected
    )
    {
        // Act
        var actual = await Task.WhenAll(inputs.ThenEach(transform));

        // Assert
        actual.Should().BeEquivalentTo(await Task.WhenAll(expected));
    }

    // Failure scenarios for ThenEach (Overload A)
    private class ThenEach_OverloadA_Failure_Data
        : TheoryData<
            IEnumerable<Task<Result<int>>>,
            Func<int, Result<string>>,
            IEnumerable<Task<Result<string>>>
        >
    {
        public ThenEach_OverloadA_Failure_Data()
        {
            // Fail transformation by throwing InvalidOperationException
            Add(
                [Task.FromResult((Result<int>)2)],
                val => new InvalidOperationException("Operation failed"),
                [Task.FromResult((Result<string>)new InvalidOperationException("Operation failed"))]
            );

            // Fail due to NullReferenceException in transformation
            Add(
                [Task.FromResult((Result<int>)5)],
                val => new NullReferenceException("Null reference encountered"),
                [
                    Task.FromResult(
                        (Result<string>)new NullReferenceException("Null reference encountered")
                    ),
                ]
            );
        }
    }

    // AsyncResultCollectionExtensions.ThenEach (Overload A) should recover from failures as exceptions
    [Theory]
    [ClassData(typeof(ThenEach_OverloadA_Failure_Data))]
    public async Task ThenEachA_Should_CaptureFailure(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, Result<string>> transform,
        IEnumerable<Task<Result<string>>> expected
    )
    {
        // Act
        var actual = await Task.WhenAll(inputs.ThenEach(transform));

        // Assert
        actual.Should().BeEquivalentTo(await Task.WhenAll(expected));
    }

    // Exception Propagation scenarios for ThenEach (Overload A)
    private class ThenEach_OverloadA_PropagateException_Data
        : TheoryData<IEnumerable<Task<Result<int>>>, Func<int, Result<string>>, Exception>
    {
        public ThenEach_OverloadA_PropagateException_Data()
        {
            // Propagate FormatException
            Add(
                [Task.FromResult((Result<int>)4)],
                val => throw new FormatException("Improper format"),
                new FormatException("Improper format")
            );

            // Propagate NullReferenceException
            Add(
                [Task.FromResult((Result<int>)7)],
                val => throw new NullReferenceException("Null issue"),
                new NullReferenceException("Null issue")
            );
        }
    }

    // AsyncResultCollectionExtensions.ThenEach (Overload A) should propagate unmapped exceptions
    [Theory]
    [ClassData(typeof(ThenEach_OverloadA_PropagateException_Data))]
    public async Task ThenEachA_Should_PropagateUnmappedException(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, Result<string>> transform,
        Exception expected
    )
    {
        // Act
        Func<Task> act = () => Task.WhenAll(inputs.ThenEach(transform));

        // Assert
        await act.Should()
            .ThrowAsync<Exception>()
            .WithMessage(expected.Message)
            .Where(x => x.GetType() == expected.GetType());
    }

    // 20 END

    // ===============================
    // 21) ThenEach (Overload B)
    // ===============================

    // Success scenarios for ThenEach (Overload B)
    private class ThenEach_OverloadB_Success_Data
        : TheoryData<
            IEnumerable<Task<Result<int>>>,
            Func<int, string>,
            IEnumerable<Task<Result<string>>>
        >
    {
        public ThenEach_OverloadB_Success_Data()
        {
            // Simple transformation to string
            Add(
                [Task.FromResult((Result<int>)1)],
                val => $"String-{val}",
                [Task.FromResult((Result<string>)"String-1")]
            );

            // Doubling the value and converting to string
            Add(
                [Task.FromResult((Result<int>)2)],
                val => $"Double-{val * 2}",
                [Task.FromResult((Result<string>)"Double-4")]
            );
        }
    }

    // AsyncResultCollectionExtensions.ThenEach (Overload B) should successfully apply synchronous transformations
    [Theory]
    [ClassData(typeof(ThenEach_OverloadB_Success_Data))]
    public async Task ThenEachB_Should_ReturnSuccess(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, string> transform,
        IEnumerable<Task<Result<string>>> expected
    )
    {
        // Act
        var actual = await Task.WhenAll(inputs.ThenEach(transform, Errors.MapNone));

        // Assert
        actual.Should().BeEquivalentTo(await Task.WhenAll(expected));
    }

    // Failure scenarios for ThenEach (Overload B)
    private class ThenEach_OverloadB_Failure_Data
        : TheoryData<
            IEnumerable<Task<Result<int>>>,
            Func<int, string>,
            IEnumerable<Task<Result<string>>>
        >
    {
        public ThenEach_OverloadB_Failure_Data()
        {
            // Throw InvalidOperationException
            Add(
                [Task.FromResult((Result<int>)1)],
                val => throw new InvalidOperationException("Failed transformation"),
                [
                    Task.FromResult(
                        (Result<string>)new InvalidOperationException("Failed transformation")
                    ),
                ]
            );

            // Throw NullReferenceException
            Add(
                [Task.FromResult((Result<int>)3)],
                val => throw new NullReferenceException("Null reference during transformation"),
                [
                    Task.FromResult(
                        (Result<string>)
                            new NullReferenceException("Null reference during transformation")
                    ),
                ]
            );
        }
    }

    // AsyncResultCollectionExtensions.ThenEach (Overload B) should handle exceptions and map them to Result<string>
    [Theory]
    [ClassData(typeof(ThenEach_OverloadB_Failure_Data))]
    public async Task ThenEachB_Should_CaptureFailure(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, string> transform,
        IEnumerable<Task<Result<string>>> expected
    )
    {
        // Act
        var actual = await Task.WhenAll(inputs.ThenEach(transform, Errors.MapAll));

        // Assert
        actual.Should().BeEquivalentTo(await Task.WhenAll(expected));
    }

    // Exception Propagation scenarios for ThenEach (Overload B)
    private class ThenEach_OverloadB_PropagateException_Data
        : TheoryData<IEnumerable<Task<Result<int>>>, Func<int, string>, Exception>
    {
        public ThenEach_OverloadB_PropagateException_Data()
        {
            // Propagate FormatException
            Add(
                [Task.FromResult((Result<int>)5)],
                val => throw new FormatException("Invalid format detected"),
                new FormatException("Invalid format detected")
            );

            // Propagate NullReferenceException
            Add(
                [Task.FromResult((Result<int>)8)],
                val => throw new NullReferenceException("Issue with null reference"),
                new NullReferenceException("Issue with null reference")
            );
        }
    }

    // AsyncResultCollectionExtensions.ThenEach (Overload B) should propagate unmapped exceptions
    [Theory]
    [ClassData(typeof(ThenEach_OverloadB_PropagateException_Data))]
    public async Task ThenEachB_Should_PropagateUnmappedException(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, string> transform,
        Exception expected
    )
    {
        // Act
        Func<Task> act = () => Task.WhenAll(inputs.ThenEach(transform, Errors.MapNone));

        // Assert
        await act.Should()
            .ThrowAsync<Exception>()
            .WithMessage(expected.Message)
            .Where(x => x.GetType() == expected.GetType());
    }

    // 21 END

    // ===============================
    // 22) ThenEach (Overload C)
    // ===============================

    // Success scenarios for ThenEach (Overload C)
    private class ThenEach_OverloadC_Success_Data
        : TheoryData<IEnumerable<Task<Result<int>>>, Action<int>, IEnumerable<Task<Result<Unit>>>>
    {
        public ThenEach_OverloadC_Success_Data()
        {
            // No-op action
            Add(
                [Task.FromResult((Result<int>)1)],
                val => { },
                [Task.FromResult((Result<Unit>)new Unit())]
            );

            // Logging action
            Add(
                [Task.FromResult((Result<int>)2)],
                val => Console.WriteLine($"Value processed: {val}"),
                [Task.FromResult((Result<Unit>)new Unit())]
            );
        }
    }

    // AsyncResultCollectionExtensions.ThenEach (Overload C) should execute actions successfully
    [Theory]
    [ClassData(typeof(ThenEach_OverloadC_Success_Data))]
    public async Task ThenEachC_Should_ReturnSuccess(
        IEnumerable<Task<Result<int>>> inputs,
        Action<int> action,
        IEnumerable<Task<Result<Unit>>> expected
    )
    {
        // Act
        var actual = await Task.WhenAll(inputs.ThenEach(action, Errors.MapNone));

        // Assert
        actual.Should().BeEquivalentTo(await Task.WhenAll(expected));
    }

    // Failure scenarios for ThenEach (Overload C)
    private class ThenEach_OverloadC_Failure_Data
        : TheoryData<IEnumerable<Task<Result<int>>>, Action<int>, IEnumerable<Task<Result<Unit>>>>
    {
        public ThenEach_OverloadC_Failure_Data()
        {
            // Action fails with InvalidOperationException
            Add(
                [Task.FromResult((Result<int>)3)],
                val => throw new InvalidOperationException("Invalid operation"),
                [Task.FromResult((Result<Unit>)new InvalidOperationException("Invalid operation"))]
            );

            // Action fails with NullReferenceException
            Add(
                [Task.FromResult((Result<int>)4)],
                val => throw new NullReferenceException("Null reference error"),
                [Task.FromResult((Result<Unit>)new NullReferenceException("Null reference error"))]
            );
        }
    }

    // AsyncResultCollectionExtensions.ThenEach (Overload C) should map exceptions to Result<Unit> for failures
    [Theory]
    [ClassData(typeof(ThenEach_OverloadC_Failure_Data))]
    public async Task ThenEachC_Should_CaptureFailure(
        IEnumerable<Task<Result<int>>> inputs,
        Action<int> action,
        IEnumerable<Task<Result<Unit>>> expected
    )
    {
        // Act
        var actual = await Task.WhenAll(inputs.ThenEach(action, Errors.MapAll));

        // Assert
        actual.Should().BeEquivalentTo(await Task.WhenAll(expected));
    }

    // Exception Propagation scenarios for ThenEach (Overload C)
    private class ThenEach_OverloadC_PropagateException_Data
        : TheoryData<IEnumerable<Task<Result<int>>>, Action<int>, Exception>
    {
        public ThenEach_OverloadC_PropagateException_Data()
        {
            // Propagate FormatException
            Add(
                [Task.FromResult((Result<int>)5)],
                val => throw new FormatException("Format exception encountered"),
                new FormatException("Format exception encountered")
            );

            // Propagate NullReferenceException
            Add(
                [Task.FromResult((Result<int>)6)],
                val => throw new NullReferenceException("Null encountered"),
                new NullReferenceException("Null encountered")
            );
        }
    }

    // AsyncResultCollectionExtensions.ThenEach (Overload C) should propagate exceptions that aren't mapped
    [Theory]
    [ClassData(typeof(ThenEach_OverloadC_PropagateException_Data))]
    public async Task ThenEachC_Should_PropagateUnmappedException(
        IEnumerable<Task<Result<int>>> inputs,
        Action<int> action,
        Exception expected
    )
    {
        // Act
        Func<Task> act = () => Task.WhenAll(inputs.ThenEach(action, Errors.MapNone));

        // Assert
        await act.Should()
            .ThrowAsync<Exception>()
            .WithMessage(expected.Message)
            .Where(x => x.GetType() == expected.GetType());
    }

    // 22 END

    // ===============================
    // 23) AssertAwaitEach (Overload A)
    // ===============================

    // Success scenarios for AssertAwaitEach (Overload A)
    private class AssertAwaitEach_OverloadA_Success_Data
        : TheoryData<
            IEnumerable<Task<Result<int>>>,
            Func<int, Task<Result<bool>>>,
            IEnumerable<Task<Result<int>>>
        >
    {
        public AssertAwaitEach_OverloadA_Success_Data()
        {
            // Assert true for all items
            Add(
                [Task.FromResult((Result<int>)1)],
                async val => await Task.FromResult((Result<bool>)true),
                [Task.FromResult((Result<int>)1)]
            );

            // Assert each is positive
            Add(
                [Task.FromResult((Result<int>)2)],
                async val => await Task.FromResult((Result<bool>)(val > 0)),
                [Task.FromResult((Result<int>)2)]
            );
        }
    }

    // AsyncResultCollectionExtensions.AssertAwaitEach (Overload A) should pass assertions
    [Theory]
    [ClassData(typeof(AssertAwaitEach_OverloadA_Success_Data))]
    public async Task AssertAwaitEachA_Should_PassAssertion(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, Task<Result<bool>>> assertion,
        IEnumerable<Task<Result<int>>> expected
    )
    {
        // Act
        var actual = await Task.WhenAll(inputs.AssertAwaitEach(assertion));

        // Assert
        actual.Should().BeEquivalentTo(await Task.WhenAll(expected));
    }

    // Assertion failure scenarios for AssertAwaitEach (Overload A)
    private class AssertAwaitEach_OverloadA_Failure_Data
        : TheoryData<
            IEnumerable<Task<Result<int>>>,
            Func<int, Task<Result<bool>>>,
            string,
            Result<int>[]
        >
    {
        public AssertAwaitEach_OverloadA_Failure_Data()
        {
            // Fail on non-zero requirement
            Add(
                [Task.FromResult((Result<int>)0)],
                async val => await Task.FromResult((Result<bool>)(val != 0)),
                "Value should be non-zero",
                [new AssertionException("Value should be non-zero")]
            );

            // Fail if not greater than 1
            Add(
                [Task.FromResult((Result<int>)1)],
                async val => await Task.FromResult((Result<bool>)(val > 1)),
                "Value should be greater than 1",
                [new AssertionException("Value should be greater than 1")]
            );
        }
    }

    // AsyncResultCollectionExtensions.AssertAwaitEach (Overload A) should handle assertion failures
    [Theory]
    [ClassData(typeof(AssertAwaitEach_OverloadA_Failure_Data))]
    public async Task AssertAwaitEachA_Should_ReturnAssertionFailure(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, Task<Result<bool>>> assertion,
        string assertionMessage,
        Result<int>[] expected
    )
    {
        // Act
        var actual = await Task.WhenAll(inputs.AssertAwaitEach(assertion, assertionMessage));

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    // Exception Propagation scenarios for AssertAwaitEach (Overload A)
    private class AssertAwaitEach_OverloadA_PropagateException_Data
        : TheoryData<IEnumerable<Task<Result<int>>>, Func<int, Task<Result<bool>>>, Exception>
    {
        public AssertAwaitEach_OverloadA_PropagateException_Data()
        {
            // Propagate FormatException in assertion
            Add(
                [Task.FromResult((Result<int>)5)],
                async val =>
                {
                    await Task.Yield();
                    throw new FormatException("Assertion threw format exception");
                },
                new FormatException("Assertion threw format exception")
            );

            // Propagate NullReferenceException in assertion
            Add(
                [Task.FromResult((Result<int>)6)],
                async val =>
                {
                    await Task.Yield();
                    throw new NullReferenceException("Null assertion");
                },
                new NullReferenceException("Null assertion")
            );
        }
    }

    // AsyncResultCollectionExtensions.AssertAwaitEach (Overload A) should propagate exceptions during assertions
    [Theory]
    [ClassData(typeof(AssertAwaitEach_OverloadA_PropagateException_Data))]
    public async Task AssertAwaitEachA_Should_PropagateExceptionInAssertion(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, Task<Result<bool>>> assertion,
        Exception expected
    )
    {
        // Act
        Func<Task> act = () => Task.WhenAll(inputs.AssertAwaitEach(assertion));

        // Assert
        await act.Should()
            .ThrowAsync<Exception>()
            .WithMessage(expected.Message)
            .Where(x => x.GetType() == expected.GetType());
    }

    // 23 END


    // ===============================
    // 24) AssertAwaitEach (Overload B)
    // ===============================

    // Success scenarios for AssertAwaitEach (Overload B)
    private class AssertAwaitEach_OverloadB_Success_Data
        : TheoryData<
            IEnumerable<Task<Result<int>>>,
            Func<int, Task<bool>>,
            IEnumerable<Task<Result<int>>>
        >
    {
        public AssertAwaitEach_OverloadB_Success_Data()
        {
            // Assert true for all items
            Add(
                [Task.FromResult((Result<int>)1)],
                async val => await Task.FromResult(true),
                [Task.FromResult((Result<int>)1)]
            );

            // Assert each is positive
            Add(
                [Task.FromResult((Result<int>)3)],
                async val => await Task.FromResult(val > 0),
                [Task.FromResult((Result<int>)3)]
            );
        }
    }

    // AsyncResultCollectionExtensions.AssertAwaitEach (Overload B) should pass assertions
    [Theory]
    [ClassData(typeof(AssertAwaitEach_OverloadB_Success_Data))]
    public async Task AssertAwaitEachB_Should_PassAssertion(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, Task<bool>> assertion,
        IEnumerable<Task<Result<int>>> expected
    )
    {
        // Act
        var actual = await Task.WhenAll(inputs.AssertAwaitEach(assertion, Errors.MapNone));

        // Assert
        actual.Should().BeEquivalentTo(await Task.WhenAll(expected));
    }

    // Assertion failure scenarios for AssertAwaitEach (Overload B)
    private class AssertAwaitEach_OverloadB_Failure_Data
        : TheoryData<IEnumerable<Task<Result<int>>>, Func<int, Task<bool>>, string, Result<int>[]>
    {
        public AssertAwaitEach_OverloadB_Failure_Data()
        {
            // Fail on non-zero requirement
            Add(
                [Task.FromResult((Result<int>)0)],
                async val => await Task.FromResult(val != 0),
                "Value must not be zero",
                [new AssertionException("Value must not be zero")]
            );

            // Fail if not greater than 5
            Add(
                [Task.FromResult((Result<int>)2)],
                async val => await Task.FromResult(val > 5),
                "Value should exceed 5",
                [new AssertionException("Value should exceed 5")]
            );
        }
    }

    // AsyncResultCollectionExtensions.AssertAwaitEach (Overload B) should handle assertion failures
    [Theory]
    [ClassData(typeof(AssertAwaitEach_OverloadB_Failure_Data))]
    public async Task AssertAwaitEachB_Should_ReturnAssertionFailure(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, Task<bool>> assertion,
        string assertionMessage,
        Result<int>[] expected
    )
    {
        // Act
        var actual = await Task.WhenAll(
            inputs.AssertAwaitEach(assertion, Errors.MapNone, assertionMessage)
        );

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    // Exception Propagation scenarios for AssertAwaitEach (Overload B)
    private class AssertAwaitEach_OverloadB_PropagateException_Data
        : TheoryData<IEnumerable<Task<Result<int>>>, Func<int, Task<bool>>, Exception>
    {
        public AssertAwaitEach_OverloadB_PropagateException_Data()
        {
            // Propagate FormatException in assertion
            Add(
                [Task.FromResult((Result<int>)5)],
                async val =>
                {
                    await Task.Yield();
                    throw new FormatException("Unexpected format exception in assertion");
                },
                new FormatException("Unexpected format exception in assertion")
            );

            // Propagate NullReferenceException in assertion
            Add(
                [Task.FromResult((Result<int>)7)],
                async val =>
                {
                    await Task.Yield();
                    throw new NullReferenceException("Null reference encountered in assertion");
                },
                new NullReferenceException("Null reference encountered in assertion")
            );
        }
    }

    // AsyncResultCollectionExtensions.AssertAwaitEach (Overload B) should propagate exceptions during assertions
    [Theory]
    [ClassData(typeof(AssertAwaitEach_OverloadB_PropagateException_Data))]
    public async Task AssertAwaitEachB_Should_PropagateExceptionInAssertion(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, Task<bool>> assertion,
        Exception expected
    )
    {
        // Act
        Func<Task> act = () => Task.WhenAll(inputs.AssertAwaitEach(assertion, Errors.MapNone));

        // Assert
        await act.Should()
            .ThrowAsync<Exception>()
            .WithMessage(expected.Message)
            .Where(x => x.GetType() == expected.GetType());
    }

    // ===============================
    // 25) AssertEach (Overload A)
    // ===============================

    // Success scenarios for AssertEach (Overload A)
    private class AssertEach_OverloadA_Success_Data
        : TheoryData<
            IEnumerable<Task<Result<int>>>,
            Func<int, Result<bool>>,
            IEnumerable<Task<Result<int>>>
        >
    {
        public AssertEach_OverloadA_Success_Data()
        {
            // Assert success for non-zero values
            Add(
                [Task.FromResult((Result<int>)1)],
                val => (Result<bool>)true,
                [Task.FromResult((Result<int>)1)]
            );

            // Assert positive values
            Add(
                [Task.FromResult((Result<int>)5)],
                val => (Result<bool>)(val > 0),
                [Task.FromResult((Result<int>)5)]
            );
        }
    }

    // AsyncResultCollectionExtensions.AssertEach (Overload A) should pass assertions
    [Theory]
    [ClassData(typeof(AssertEach_OverloadA_Success_Data))]
    public async Task AssertEachA_Should_PassAssertion(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, Result<bool>> assertion,
        IEnumerable<Task<Result<int>>> expected
    )
    {
        // Act
        var actual = await Task.WhenAll(inputs.AssertEach(assertion));

        // Assert
        actual.Should().BeEquivalentTo(await Task.WhenAll(expected));
    }

    // Assertion failure scenarios for AssertEach (Overload A)
    private class AssertEach_OverloadA_Failure_Data
        : TheoryData<IEnumerable<Task<Result<int>>>, Func<int, Result<bool>>, string, Result<int>[]>
    {
        public AssertEach_OverloadA_Failure_Data()
        {
            // Fail on zero
            Add(
                [Task.FromResult((Result<int>)0), Task.FromResult((Result<int>)0)],
                val => (Result<bool>)(val != 0),
                "Expected value to be non-zero",
                [
                    new AssertionException("Expected value to be non-zero"),
                    new AssertionException("Expected value to be non-zero"),
                ]
            );

            // Fail if not greater than 10
            Add(
                [Task.FromResult((Result<int>)3)],
                val => (Result<bool>)(val > 10),
                "Expected value to be greater than 10",
                [new AssertionException("Expected value to be greater than 10")]
            );
        }
    }

    // AsyncResultCollectionExtensions.AssertEach (Overload A) should handle assertion failures
    [Theory]
    [ClassData(typeof(AssertEach_OverloadA_Failure_Data))]
    public async Task AssertEachA_Should_ReturnAssertionFailure(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, Result<bool>> assertion,
        string assertionMessage,
        Result<int>[] expected
    )
    {
        // Act
        var actual = await Task.WhenAll(inputs.AssertEach(assertion, assertionMessage));

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    // Exception Propagation scenarios for AssertEach (Overload A)
    private class AssertEach_OverloadA_PropagateException_Data
        : TheoryData<IEnumerable<Task<Result<int>>>, Func<int, Result<bool>>, Exception>
    {
        public AssertEach_OverloadA_PropagateException_Data()
        {
            // Propagate FormatException during assertion
            Add(
                [Task.FromResult((Result<int>)4)],
                val => throw new FormatException("Formatting exception in assertion"),
                new FormatException("Formatting exception in assertion")
            );

            // Propagate NullReferenceException during assertion
            Add(
                [Task.FromResult((Result<int>)6)],
                val => throw new NullReferenceException("Null reference error in assertion"),
                new NullReferenceException("Null reference error in assertion")
            );
        }
    }

    // AsyncResultCollectionExtensions.AssertEach (Overload A) should propagate exceptions occurring during assertions
    [Theory]
    [ClassData(typeof(AssertEach_OverloadA_PropagateException_Data))]
    public async Task AssertEachA_Should_PropagateExceptionInAssertion(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, Result<bool>> assertion,
        Exception expected
    )
    {
        // Act
        Func<Task> act = () => Task.WhenAll(inputs.AssertEach(assertion));

        // Assert
        await act.Should()
            .ThrowAsync<Exception>()
            .WithMessage(expected.Message)
            .Where(x => x.GetType() == expected.GetType());
    }

    // ===============================
    // 26) AssertEach (Overload B)
    // ===============================

    // Success scenarios for AssertEach (Overload B)
    private class AssertEach_OverloadB_Success_Data
        : TheoryData<
            IEnumerable<Task<Result<int>>>,
            Func<int, bool>,
            IEnumerable<Task<Result<int>>>
        >
    {
        public AssertEach_OverloadB_Success_Data()
        {
            // Assert all values are non-zero
            Add(
                [Task.FromResult((Result<int>)1)],
                val => val != 0,
                [Task.FromResult((Result<int>)1)]
            );

            // Assert numbers are positive
            Add(
                [Task.FromResult((Result<int>)4)],
                val => val > 0,
                [Task.FromResult((Result<int>)4)]
            );
        }
    }

    // AsyncResultCollectionExtensions.AssertEach (Overload B) should pass assertions
    [Theory]
    [ClassData(typeof(AssertEach_OverloadB_Success_Data))]
    public async Task AssertEachB_Should_PassAssertion(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, bool> assertion,
        IEnumerable<Task<Result<int>>> expected
    )
    {
        // Act
        var actual = await Task.WhenAll(inputs.AssertEach(assertion, Errors.MapNone));

        // Assert
        actual.Should().BeEquivalentTo(await Task.WhenAll(expected));
    }

    // Assertion failure scenarios for AssertEach (Overload B)
    private class AssertEach_OverloadB_Failure_Data
        : TheoryData<IEnumerable<Task<Result<int>>>, Func<int, bool>, string, Result<int>[]>
    {
        public AssertEach_OverloadB_Failure_Data()
        {
            // Fail on zero
            Add(
                [Task.FromResult((Result<int>)0)],
                val => val != 0,
                "Value should not be zero",
                [new AssertionException("Value should not be zero")]
            );

            // Fail if not greater than 3
            Add(
                [Task.FromResult((Result<int>)2), Task.FromResult((Result<int>)1)],
                val => val > 3,
                "Value must be greater than 3",
                [
                    new AssertionException("Value must be greater than 3"),
                    new AssertionException("Value must be greater than 3"),
                ]
            );
        }
    }

    // AsyncResultCollectionExtensions.AssertEach (Overload B) should handle assertion failures
    [Theory]
    [ClassData(typeof(AssertEach_OverloadB_Failure_Data))]
    public async Task AssertEachB_Should_ReturnAssertionFailure(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, bool> assertion,
        string assertionMessage,
        Result<int>[] expected
    )
    {
        // Act
        var actual = await Task.WhenAll(
            inputs.AssertEach(assertion, Errors.MapNone, assertionMessage)
        );

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    // Exception Propagation scenarios for AssertEach (Overload B)
    private class AssertEach_OverloadB_PropagateException_Data
        : TheoryData<IEnumerable<Task<Result<int>>>, Func<int, bool>, Exception>
    {
        public AssertEach_OverloadB_PropagateException_Data()
        {
            // Propagate FormatException during assertion
            Add(
                [Task.FromResult((Result<int>)5)],
                val => throw new FormatException("Exception in format during assertion"),
                new FormatException("Exception in format during assertion")
            );

            // Propagate NullReferenceException during assertion
            Add(
                [Task.FromResult((Result<int>)7)],
                val => throw new NullReferenceException("Null reference during assertion"),
                new NullReferenceException("Null reference during assertion")
            );
        }
    }

    // AsyncResultCollectionExtensions.AssertEach (Overload B) should propagate exceptions that occur during assertions
    [Theory]
    [ClassData(typeof(AssertEach_OverloadB_PropagateException_Data))]
    public async Task AssertEachB_Should_PropagateExceptionInAssertion(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, bool> assertion,
        Exception expected
    )
    {
        // Act
        Func<Task> act = () => Task.WhenAll(inputs.AssertEach(assertion, Errors.MapNone));

        // Assert
        await act.Should()
            .ThrowAsync<Exception>()
            .WithMessage(expected.Message)
            .Where(x => x.GetType() == expected.GetType());
    }

    private class IfEach_Should_ExecuteThen_WhenPredicateTrue_Data
        : TheoryData<
            IEnumerable<Task<Result<int>>>,
            Func<int, Result<bool>>,
            Func<int, Result<string>>,
            Func<int, Result<string>>,
            IEnumerable<Task<Result<string>>>
        >
    {
        public IfEach_Should_ExecuteThen_WhenPredicateTrue_Data()
        {
            // All predicates return true
            Add(
                [Task.FromResult((Result<int>)5), Task.FromResult((Result<int>)10)],
                val => true,
                val => $"Then_{val}",
                val => $"Else_{val}",
                [
                    Task.FromResult((Result<string>)"Then_5"),
                    Task.FromResult((Result<string>)"Then_10"),
                ]
            );

            // Mixed data with only successful items
            Add(
                [Task.FromResult((Result<int>)3), Task.FromResult((Result<int>)7)],
                val => val > 2,
                val => $"Then_{val}",
                val => $"Else_{val}",
                [
                    Task.FromResult((Result<string>)"Then_3"),
                    Task.FromResult((Result<string>)"Then_7"),
                ]
            );

            // Empty collection
            Add([], val => true, val => $"Then_{val}", val => $"Else_{val}", []);
        }
    }

    [Theory]
    [ClassData(typeof(IfEach_Should_ExecuteThen_WhenPredicateTrue_Data))]
    public async Task IfEach_Should_ExecuteThen_WhenPredicateTrue(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, Result<bool>> predicate,
        Func<int, Result<string>> thenFunc,
        Func<int, Result<string>> elseFunc,
        IEnumerable<Task<Result<string>>> expected
    )
    {
        // Act
        var result = inputs.IfEach(predicate, thenFunc, elseFunc);

        // Assert
        var actual = await Task.WhenAll(result);
        var expectedArray = await Task.WhenAll(expected);

        actual.Should().BeEquivalentTo(expectedArray);
    }

    private class IfEach_Should_ExecuteElse_WhenPredicateFalse_Data
        : TheoryData<
            IEnumerable<Task<Result<int>>>,
            Func<int, Result<bool>>,
            Func<int, Result<string>>,
            Func<int, Result<string>>,
            IEnumerable<Task<Result<string>>>
        >
    {
        public IfEach_Should_ExecuteElse_WhenPredicateFalse_Data()
        {
            // All predicates return false
            Add(
                [Task.FromResult((Result<int>)5), Task.FromResult((Result<int>)10)],
                val => false,
                val => $"Then_{val}",
                val => $"Else_{val}",
                [
                    Task.FromResult((Result<string>)"Else_5"),
                    Task.FromResult((Result<string>)"Else_10"),
                ]
            );

            // Mixed predicate results
            Add(
                [Task.FromResult((Result<int>)1), Task.FromResult((Result<int>)4)],
                val => val > 3,
                val => $"Then_{val}",
                val => $"Else_{val}",
                [
                    Task.FromResult((Result<string>)"Else_1"),
                    Task.FromResult((Result<string>)"Then_4"),
                ]
            );
        }
    }

    [Theory]
    [ClassData(typeof(IfEach_Should_ExecuteElse_WhenPredicateFalse_Data))]
    public async Task IfEach_Should_ExecuteElse_WhenPredicateFalse(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, Result<bool>> predicate,
        Func<int, Result<string>> thenFunc,
        Func<int, Result<string>> elseFunc,
        IEnumerable<Task<Result<string>>> expected
    )
    {
        // Act
        var result = inputs.IfEach(predicate, thenFunc, elseFunc);

        // Assert
        var actual = await Task.WhenAll(result);
        var expectedArray = await Task.WhenAll(expected);

        actual.Should().BeEquivalentTo(expectedArray);
    }

    private class IfEach_Should_RetainFailures_Data
        : TheoryData<
            IEnumerable<Task<Result<int>>>,
            Func<int, Result<bool>>,
            Func<int, Result<string>>,
            Func<int, Result<string>>,
            IEnumerable<Task<Result<string>>>
        >
    {
        public IfEach_Should_RetainFailures_Data()
        {
            // Collection with failures
            Add(
                [
                    Task.FromResult((Result<int>)5),
                    Task.FromResult((Result<int>)new InvalidOperationException("Test failure")),
                    Task.FromResult((Result<int>)10),
                ],
                val => true,
                val => $"Then_{val}",
                val => $"Else_{val}",
                [
                    Task.FromResult((Result<string>)"Then_5"),
                    Task.FromResult((Result<string>)new InvalidOperationException("Test failure")),
                    Task.FromResult((Result<string>)"Then_10"),
                ]
            );

            // Collection with only failures
            Add(
                [
                    Task.FromResult((Result<int>)new ArgumentException("Arg error 1")),
                    Task.FromResult((Result<int>)new NullReferenceException("Null error")),
                ],
                val => true,
                val => $"Then_{val}",
                val => $"Else_{val}",
                [
                    Task.FromResult((Result<string>)new ArgumentException("Arg error 1")),
                    Task.FromResult((Result<string>)new NullReferenceException("Null error")),
                ]
            );
        }
    }

    [Theory]
    [ClassData(typeof(IfEach_Should_RetainFailures_Data))]
    public async Task IfEach_Should_RetainFailures(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, Result<bool>> predicate,
        Func<int, Result<string>> thenFunc,
        Func<int, Result<string>> elseFunc,
        IEnumerable<Task<Result<string>>> expected
    )
    {
        // Act
        var result = inputs.IfEach(predicate, thenFunc, elseFunc);

        // Assert
        var actual = await Task.WhenAll(result);
        var expectedArray = await Task.WhenAll(expected);

        actual.Should().BeEquivalentTo(expectedArray);
    }

    private class IfEach_Should_PropagateException_Data
        : TheoryData<
            IEnumerable<Task<Result<int>>>,
            Func<int, Result<bool>>,
            Func<int, Result<string>>,
            Func<int, Result<string>>,
            Exception
        >
    {
        public IfEach_Should_PropagateException_Data()
        {
            // Exception in predicate
            Add(
                [Task.FromResult((Result<int>)5)],
                val => throw new FormatException("Exception in predicate"),
                val => $"Then_{val}",
                val => $"Else_{val}",
                new FormatException("Exception in predicate")
            );

            // Exception in then function
            Add(
                [Task.FromResult((Result<int>)7)],
                val => true,
                val => throw new ArgumentNullException("value", "Exception in then function"),
                val => $"Else_{val}",
                new ArgumentNullException("value", "Exception in then function")
            );

            // Exception in else function
            Add(
                [Task.FromResult((Result<int>)3)],
                val => false,
                val => $"Then_{val}",
                val => throw new InvalidOperationException("Exception in else function"),
                new InvalidOperationException("Exception in else function")
            );
        }
    }

    [Theory]
    [ClassData(typeof(IfEach_Should_PropagateException_Data))]
    public async Task IfEach_Should_PropagateException(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, Result<bool>> predicate,
        Func<int, Result<string>> thenFunc,
        Func<int, Result<string>> elseFunc,
        Exception expected
    )
    {
        // Act
        Func<Task> act = () => Task.WhenAll(inputs.IfEach(predicate, thenFunc, elseFunc));

        // Assert
        await act.Should()
            .ThrowAsync<Exception>()
            .WithMessage(expected.Message)
            .Where(x => x.GetType() == expected.GetType());
    }

    private class IfAwaitEach_Should_ExecuteThen_WhenPredicateTrue_Data
        : TheoryData<
            IEnumerable<Task<Result<int>>>,
            Func<int, Task<Result<bool>>>,
            Func<int, Task<Result<string>>>,
            Func<int, Task<Result<string>>>,
            IEnumerable<Task<Result<string>>>
        >
    {
        public IfAwaitEach_Should_ExecuteThen_WhenPredicateTrue_Data()
        {
            // All predicates return true
            Add(
                [Task.FromResult((Result<int>)5), Task.FromResult((Result<int>)10)],
                val => Task.FromResult((Result<bool>)true),
                val => Task.FromResult((Result<string>)$"Then_{val}"),
                val => Task.FromResult((Result<string>)$"Else_{val}"),
                [
                    Task.FromResult((Result<string>)"Then_5"),
                    Task.FromResult((Result<string>)"Then_10"),
                ]
            );

            // Mixed predicates with only successful items
            Add(
                [Task.FromResult((Result<int>)3), Task.FromResult((Result<int>)7)],
                val => Task.FromResult((Result<bool>)(val > 2)),
                val => Task.FromResult((Result<string>)$"Then_{val}"),
                val => Task.FromResult((Result<string>)$"Else_{val}"),
                [
                    Task.FromResult((Result<string>)"Then_3"),
                    Task.FromResult((Result<string>)"Then_7"),
                ]
            );
        }
    }

    [Theory]
    [ClassData(typeof(IfAwaitEach_Should_ExecuteThen_WhenPredicateTrue_Data))]
    public async Task IfAwaitEach_Should_ExecuteThen_WhenPredicateTrue(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, Task<Result<bool>>> predicate,
        Func<int, Task<Result<string>>> thenFunc,
        Func<int, Task<Result<string>>> elseFunc,
        IEnumerable<Task<Result<string>>> expected
    )
    {
        // Act
        var result = inputs.IfAwaitEach(predicate, thenFunc, elseFunc);

        // Assert
        var actual = await Task.WhenAll(result);
        var expectedArray = await Task.WhenAll(expected);

        actual.Should().BeEquivalentTo(expectedArray);
    }

    private class IfAwaitEach_Should_ExecuteElse_WhenPredicateFalse_Data
        : TheoryData<
            IEnumerable<Task<Result<int>>>,
            Func<int, Task<Result<bool>>>,
            Func<int, Task<Result<string>>>,
            Func<int, Task<Result<string>>>,
            IEnumerable<Task<Result<string>>>
        >
    {
        public IfAwaitEach_Should_ExecuteElse_WhenPredicateFalse_Data()
        {
            // All predicates return false
            Add(
                [Task.FromResult((Result<int>)5), Task.FromResult((Result<int>)10)],
                val => Task.FromResult((Result<bool>)false),
                val => Task.FromResult((Result<string>)$"Then_{val}"),
                val => Task.FromResult((Result<string>)$"Else_{val}"),
                [
                    Task.FromResult((Result<string>)"Else_5"),
                    Task.FromResult((Result<string>)"Else_10"),
                ]
            );

            // Mixed predicate results
            Add(
                [Task.FromResult((Result<int>)1), Task.FromResult((Result<int>)4)],
                val => Task.FromResult((Result<bool>)(val > 3)),
                val => Task.FromResult((Result<string>)$"Then_{val}"),
                val => Task.FromResult((Result<string>)$"Else_{val}"),
                [
                    Task.FromResult((Result<string>)"Else_1"),
                    Task.FromResult((Result<string>)"Then_4"),
                ]
            );
        }
    }

    [Theory]
    [ClassData(typeof(IfAwaitEach_Should_ExecuteElse_WhenPredicateFalse_Data))]
    public async Task IfAwaitEach_Should_ExecuteElse_WhenPredicateFalse(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, Task<Result<bool>>> predicate,
        Func<int, Task<Result<string>>> thenFunc,
        Func<int, Task<Result<string>>> elseFunc,
        IEnumerable<Task<Result<string>>> expected
    )
    {
        // Act
        var result = inputs.IfAwaitEach(predicate, thenFunc, elseFunc);

        // Assert
        var actual = await Task.WhenAll(result);
        var expectedArray = await Task.WhenAll(expected);

        actual.Should().BeEquivalentTo(expectedArray);
    }

    private class IfAwaitEach_Should_RetainFailures_Data
        : TheoryData<
            IEnumerable<Task<Result<int>>>,
            Func<int, Task<Result<bool>>>,
            Func<int, Task<Result<string>>>,
            Func<int, Task<Result<string>>>,
            IEnumerable<Task<Result<string>>>
        >
    {
        public IfAwaitEach_Should_RetainFailures_Data()
        {
            // Collection with failures
            Add(
                [
                    Task.FromResult((Result<int>)5),
                    Task.FromResult((Result<int>)new InvalidOperationException("Test failure")),
                    Task.FromResult((Result<int>)10),
                ],
                val => Task.FromResult((Result<bool>)true),
                val => Task.FromResult((Result<string>)$"Then_{val}"),
                val => Task.FromResult((Result<string>)$"Else_{val}"),
                [
                    Task.FromResult((Result<string>)"Then_5"),
                    Task.FromResult((Result<string>)new InvalidOperationException("Test failure")),
                    Task.FromResult((Result<string>)"Then_10"),
                ]
            );
        }
    }

    [Theory]
    [ClassData(typeof(IfAwaitEach_Should_RetainFailures_Data))]
    public async Task IfAwaitEach_Should_RetainFailures(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, Task<Result<bool>>> predicate,
        Func<int, Task<Result<string>>> thenFunc,
        Func<int, Task<Result<string>>> elseFunc,
        IEnumerable<Task<Result<string>>> expected
    )
    {
        // Act
        var result = inputs.IfAwaitEach(predicate, thenFunc, elseFunc);

        // Assert
        var actual = await Task.WhenAll(result);
        var expectedArray = await Task.WhenAll(expected);

        actual.Should().BeEquivalentTo(expectedArray);
    }

    private class IfAwaitEach_Should_PropagateException_Data
        : TheoryData<
            IEnumerable<Task<Result<int>>>,
            Func<int, Task<Result<bool>>>,
            Func<int, Task<Result<string>>>,
            Func<int, Task<Result<string>>>,
            Exception
        >
    {
        public IfAwaitEach_Should_PropagateException_Data()
        {
            // Exception in predicate
            Add(
                [Task.FromResult((Result<int>)5)],
                val => throw new FormatException("Exception in predicate"),
                val => Task.FromResult((Result<string>)$"Then_{val}"),
                val => Task.FromResult((Result<string>)$"Else_{val}"),
                new FormatException("Exception in predicate")
            );

            // Exception in then function
            Add(
                [Task.FromResult((Result<int>)7)],
                val => Task.FromResult((Result<bool>)true),
                val => throw new ArgumentNullException("value", "Exception in then function"),
                val => Task.FromResult((Result<string>)$"Else_{val}"),
                new ArgumentNullException("value", "Exception in then function")
            );

            // Exception in else function
            Add(
                [Task.FromResult((Result<int>)3)],
                val => Task.FromResult((Result<bool>)false),
                val => Task.FromResult((Result<string>)$"Then_{val}"),
                val => throw new InvalidOperationException("Exception in else function"),
                new InvalidOperationException("Exception in else function")
            );
        }
    }

    [Theory]
    [ClassData(typeof(IfAwaitEach_Should_PropagateException_Data))]
    public async Task IfAwaitEach_Should_PropagateException(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, Task<Result<bool>>> predicate,
        Func<int, Task<Result<string>>> thenFunc,
        Func<int, Task<Result<string>>> elseFunc,
        Exception expected
    )
    {
        // Act
        Func<Task> act = () => Task.WhenAll(inputs.IfAwaitEach(predicate, thenFunc, elseFunc));

        // Assert
        await act.Should()
            .ThrowAsync<Exception>()
            .WithMessage(expected.Message)
            .Where(x => x.GetType() == expected.GetType());
    }

    // Tests for MatchEach (Overload A with Func returning TResult)
    private class MatchEach_OverloadA_Success_Data
        : TheoryData<
            IEnumerable<Task<Result<int>>>,
            Func<int, string>,
            Func<Exception, string>,
            IEnumerable<Task<string>>
        >
    {
        public MatchEach_OverloadA_Success_Data()
        {
            // Single success result
            Add(
                [Task.FromResult((Result<int>)42)],
                val => $"Success: {val}",
                err => $"Error: {err.Message}",
                [Task.FromResult("Success: 42")]
            );

            // Multiple success results
            Add(
                [
                    Task.FromResult((Result<int>)10),
                    Task.FromResult((Result<int>)20),
                    Task.FromResult((Result<int>)30),
                ],
                val => $"Success: {val}",
                err => $"Error: {err.Message}",
                [
                    Task.FromResult("Success: 10"),
                    Task.FromResult("Success: 20"),
                    Task.FromResult("Success: 30"),
                ]
            );

            // Empty collection
            Add([], val => $"Success: {val}", err => $"Error: {err.Message}", []);
        }
    }

    [Theory]
    [ClassData(typeof(MatchEach_OverloadA_Success_Data))]
    public async Task MatchEachA_Should_HandleSuccesses(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, string> success,
        Func<Exception, string> failure,
        IEnumerable<Task<string>> expected
    )
    {
        // Act
        var actual = inputs.MatchEach(success, failure);

        // Assert
        var actualResults = await Task.WhenAll(actual);
        var expectedResults = await Task.WhenAll(expected);
        actualResults.Should().BeEquivalentTo(expectedResults);
    }

    private class MatchEach_OverloadA_Failure_Data
        : TheoryData<
            IEnumerable<Task<Result<int>>>,
            Func<int, string>,
            Func<Exception, string>,
            IEnumerable<Task<string>>
        >
    {
        public MatchEach_OverloadA_Failure_Data()
        {
            // Single failure result
            Add(
                [Task.FromResult((Result<int>)new InvalidOperationException("Operation failed"))],
                val => $"Success: {val}",
                err => $"Error: {err.Message}",
                [Task.FromResult("Error: Operation failed")]
            );

            // Mixed success and failure results
            Add(
                [
                    Task.FromResult((Result<int>)10),
                    Task.FromResult((Result<int>)new ArgumentException("Invalid argument")),
                    Task.FromResult((Result<int>)30),
                ],
                val => $"Success: {val}",
                err => $"Error: {err.Message}",
                [
                    Task.FromResult("Success: 10"),
                    Task.FromResult("Error: Invalid argument"),
                    Task.FromResult("Success: 30"),
                ]
            );

            // Multiple failure results
            Add(
                [
                    Task.FromResult((Result<int>)new InvalidOperationException("First error")),
                    Task.FromResult((Result<int>)new ArgumentException("Second error")),
                ],
                val => $"Success: {val}",
                err => $"Error: {err.Message}",
                [Task.FromResult("Error: First error"), Task.FromResult("Error: Second error")]
            );
        }
    }

    [Theory]
    [ClassData(typeof(MatchEach_OverloadA_Failure_Data))]
    public async Task MatchEachA_Should_HandleFailures(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, string> success,
        Func<Exception, string> failure,
        IEnumerable<Task<string>> expected
    )
    {
        // Act
        var actual = inputs.MatchEach(success, failure);

        // Assert
        var actualResults = await Task.WhenAll(actual);
        var expectedResults = await Task.WhenAll(expected);
        actualResults.Should().BeEquivalentTo(expectedResults);
    }

    private class MatchEach_OverloadA_PropagateException_Data
        : TheoryData<
            IEnumerable<Task<Result<int>>>,
            Func<int, string>,
            Func<Exception, string>,
            Exception
        >
    {
        public MatchEach_OverloadA_PropagateException_Data()
        {
            // Exception in success handler
            Add(
                [Task.FromResult((Result<int>)42)],
                val => throw new FormatException("Exception in success handler"),
                err => $"Error: {err.Message}",
                new FormatException("Exception in success handler")
            );

            // Exception in failure handler
            Add(
                [Task.FromResult((Result<int>)new InvalidOperationException("Original error"))],
                val => $"Success: {val}",
                err => throw new NullReferenceException("Exception in failure handler"),
                new NullReferenceException("Exception in failure handler")
            );
        }
    }

    [Theory]
    [ClassData(typeof(MatchEach_OverloadA_PropagateException_Data))]
    public async Task MatchEachA_Should_PropagateException(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, string> success,
        Func<Exception, string> failure,
        Exception expected
    )
    {
        // Act
        var matchedResults = inputs.MatchEach(success, failure);
        Func<Task> act = () => Task.WhenAll(matchedResults);

        // Assert
        await act.Should()
            .ThrowAsync<Exception>()
            .WithMessage(expected.Message)
            .Where(x => x.GetType() == expected.GetType());
    }

    // Tests for MatchEach (Overload B with Action)
    private class MatchEach_OverloadB_Data
        : TheoryData<IEnumerable<Task<Result<int>>>, List<string>>
    {
        public MatchEach_OverloadB_Data()
        {
            // Success cases
            var successLog1 = new List<string>();
            Add([Task.FromResult((Result<int>)42)], successLog1);

            // Multiple success results
            var successLog2 = new List<string>();
            Add([Task.FromResult((Result<int>)10), Task.FromResult((Result<int>)20)], successLog2);

            // Failure cases
            var failureLog1 = new List<string>();
            Add(
                [Task.FromResult((Result<int>)new InvalidOperationException("Operation failed"))],
                failureLog1
            );

            // Mixed success and failure results
            var mixedLog = new List<string>();
            Add(
                [
                    Task.FromResult((Result<int>)5),
                    Task.FromResult((Result<int>)new ArgumentException("Invalid argument")),
                ],
                mixedLog
            );
        }
    }

    [Theory]
    [ClassData(typeof(MatchEach_OverloadB_Data))]
    public async Task MatchEachB_Should_ExecuteActions(
        IEnumerable<Task<Result<int>>> inputs,
        List<string> log
    )
    {
        // Act
        var tasks = inputs.MatchEach(
            val => log.Add($"Success: {val}"),
            err => log.Add($"Error: {err.Message}")
        );
        await Task.WhenAll(tasks);

        // Assert
        var expected = new List<string>();
        foreach (var input in await Task.WhenAll(inputs))
        {
            input.Match(
                val => expected.Add($"Success: {val}"),
                err => expected.Add($"Error: {err.Message}")
            );
        }
        log.Should().BeEquivalentTo(expected);
    }

    private class MatchEach_OverloadB_PropagateException_Data
        : TheoryData<IEnumerable<Task<Result<int>>>, Action<int>, Action<Exception>, Exception>
    {
        public MatchEach_OverloadB_PropagateException_Data()
        {
            // Exception in success action
            Add(
                [Task.FromResult((Result<int>)42)],
                val => throw new InvalidCastException("Exception in success action"),
                err => { },
                new InvalidCastException("Exception in success action")
            );

            // Exception in failure action
            Add(
                [Task.FromResult((Result<int>)new ArgumentException("Original error"))],
                val => { },
                err => throw new DivideByZeroException("Exception in failure action"),
                new DivideByZeroException("Exception in failure action")
            );
        }
    }

    [Theory]
    [ClassData(typeof(MatchEach_OverloadB_PropagateException_Data))]
    public async Task MatchEachB_Should_PropagateException(
        IEnumerable<Task<Result<int>>> inputs,
        Action<int> success,
        Action<Exception> failure,
        Exception expected
    )
    {
        // Act
        var matchedResults = inputs.MatchEach(success, failure);
        Func<Task> act = () => Task.WhenAll(matchedResults);

        // Assert
        await act.Should()
            .ThrowAsync<Exception>()
            .WithMessage(expected.Message)
            .Where(x => x.GetType() == expected.GetType());
    }

    // Tests for MatchAwaitEach (Overload A with Func returning Task<TResult>)
    private class MatchAwaitEach_OverloadA_Success_Data
        : TheoryData<
            IEnumerable<Task<Result<int>>>,
            Func<int, Task<string>>,
            Func<Exception, Task<string>>,
            IEnumerable<Task<string>>
        >
    {
        public MatchAwaitEach_OverloadA_Success_Data()
        {
            // Single success result
            Add(
                [Task.FromResult((Result<int>)42)],
                val => Task.FromResult($"Success: {val}"),
                err => Task.FromResult($"Error: {err.Message}"),
                [Task.FromResult("Success: 42")]
            );

            // Multiple success results
            Add(
                [
                    Task.FromResult((Result<int>)10),
                    Task.FromResult((Result<int>)20),
                    Task.FromResult((Result<int>)30),
                ],
                val => Task.FromResult($"Success: {val}"),
                err => Task.FromResult($"Error: {err.Message}"),
                [
                    Task.FromResult("Success: 10"),
                    Task.FromResult("Success: 20"),
                    Task.FromResult("Success: 30"),
                ]
            );

            // Empty collection
            Add(
                [],
                val => Task.FromResult($"Success: {val}"),
                err => Task.FromResult($"Error: {err.Message}"),
                []
            );
        }
    }

    [Theory]
    [ClassData(typeof(MatchAwaitEach_OverloadA_Success_Data))]
    public async Task MatchAwaitEachA_Should_HandleSuccesses(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, Task<string>> success,
        Func<Exception, Task<string>> failure,
        IEnumerable<Task<string>> expected
    )
    {
        // Act
        var actual = inputs.MatchAwaitEach(success, failure);

        // Assert
        var actualResults = await Task.WhenAll(actual);
        var expectedResults = await Task.WhenAll(expected);
        actualResults.Should().BeEquivalentTo(expectedResults);
    }

    private class MatchAwaitEach_OverloadA_Failure_Data
        : TheoryData<
            IEnumerable<Task<Result<int>>>,
            Func<int, Task<string>>,
            Func<Exception, Task<string>>,
            IEnumerable<Task<string>>
        >
    {
        public MatchAwaitEach_OverloadA_Failure_Data()
        {
            // Single failure result
            Add(
                [Task.FromResult((Result<int>)new InvalidOperationException("Operation failed"))],
                val => Task.FromResult($"Success: {val}"),
                err => Task.FromResult($"Error: {err.Message}"),
                [Task.FromResult("Error: Operation failed")]
            );

            // Mixed success and failure results
            Add(
                [
                    Task.FromResult((Result<int>)10),
                    Task.FromResult((Result<int>)new ArgumentException("Invalid argument")),
                    Task.FromResult((Result<int>)30),
                ],
                val => Task.FromResult($"Success: {val}"),
                err => Task.FromResult($"Error: {err.Message}"),
                [
                    Task.FromResult("Success: 10"),
                    Task.FromResult("Error: Invalid argument"),
                    Task.FromResult("Success: 30"),
                ]
            );

            // Multiple failure results
            Add(
                [
                    Task.FromResult((Result<int>)new InvalidOperationException("First error")),
                    Task.FromResult((Result<int>)new ArgumentException("Second error")),
                ],
                val => Task.FromResult($"Success: {val}"),
                err => Task.FromResult($"Error: {err.Message}"),
                [Task.FromResult("Error: First error"), Task.FromResult("Error: Second error")]
            );
        }
    }

    [Theory]
    [ClassData(typeof(MatchAwaitEach_OverloadA_Failure_Data))]
    public async Task MatchAwaitEachA_Should_HandleFailures(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, Task<string>> success,
        Func<Exception, Task<string>> failure,
        IEnumerable<Task<string>> expected
    )
    {
        // Act
        var actual = inputs.MatchAwaitEach(success, failure);

        // Assert
        var actualResults = await Task.WhenAll(actual);
        var expectedResults = await Task.WhenAll(expected);
        actualResults.Should().BeEquivalentTo(expectedResults);
    }

    private class MatchAwaitEach_OverloadA_PropagateException_Data
        : TheoryData<
            IEnumerable<Task<Result<int>>>,
            Func<int, Task<string>>,
            Func<Exception, Task<string>>,
            Exception
        >
    {
        public MatchAwaitEach_OverloadA_PropagateException_Data()
        {
            // Exception in success handler
            Add(
                [Task.FromResult((Result<int>)42)],
                val => throw new FormatException("Exception in success handler"),
                err => Task.FromResult($"Error: {err.Message}"),
                new FormatException("Exception in success handler")
            );

            // Exception in failure handler
            Add(
                [Task.FromResult((Result<int>)new InvalidOperationException("Original error"))],
                val => Task.FromResult($"Success: {val}"),
                err => throw new NullReferenceException("Exception in failure handler"),
                new NullReferenceException("Exception in failure handler")
            );

            // Exception in async success handler
            Add(
                [Task.FromResult((Result<int>)42)],
                val =>
                    Task.FromException<string>(
                        new InvalidCastException("Async exception in success")
                    ),
                err => Task.FromResult($"Error: {err.Message}"),
                new InvalidCastException("Async exception in success")
            );

            // Exception in async failure handler
            Add(
                [Task.FromResult((Result<int>)new ArgumentException("Original error"))],
                val => Task.FromResult($"Success: {val}"),
                err =>
                    Task.FromException<string>(
                        new DivideByZeroException("Async exception in failure")
                    ),
                new DivideByZeroException("Async exception in failure")
            );
        }
    }

    [Theory]
    [ClassData(typeof(MatchAwaitEach_OverloadA_PropagateException_Data))]
    public async Task MatchAwaitEachA_Should_PropagateException(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, Task<string>> success,
        Func<Exception, Task<string>> failure,
        Exception expected
    )
    {
        // Act
        var matchedResults = inputs.MatchAwaitEach(success, failure);
        Func<Task> act = () => Task.WhenAll(matchedResults);

        // Assert
        await act.Should()
            .ThrowAsync<Exception>()
            .WithMessage(expected.Message)
            .Where(x => x.GetType() == expected.GetType());
    }

    // Tests for MatchAwaitEach (Overload B with Func returning Task without value)
    private class MatchAwaitEach_OverloadB_Data
        : TheoryData<IEnumerable<Task<Result<int>>>, List<string>>
    {
        public MatchAwaitEach_OverloadB_Data()
        {
            // Success cases
            var successLog1 = new List<string>();
            Add([Task.FromResult((Result<int>)42)], successLog1);

            // Multiple success results
            var successLog2 = new List<string>();
            Add([Task.FromResult((Result<int>)10), Task.FromResult((Result<int>)20)], successLog2);

            // Failure cases
            var failureLog1 = new List<string>();
            Add(
                [Task.FromResult((Result<int>)new InvalidOperationException("Operation failed"))],
                failureLog1
            );

            // Mixed success and failure results
            var mixedLog = new List<string>();
            Add(
                [
                    Task.FromResult((Result<int>)5),
                    Task.FromResult((Result<int>)new ArgumentException("Invalid argument")),
                ],
                mixedLog
            );
        }
    }

    [Theory]
    [ClassData(typeof(MatchAwaitEach_OverloadB_Data))]
    public async Task MatchAwaitEachB_Should_ExecuteActions(
        IEnumerable<Task<Result<int>>> inputs,
        List<string> log
    )
    {
        // Act
        var tasks = inputs.MatchAwaitEach(
            val =>
            {
                log.Add($"Success: {val}");
                return Task.CompletedTask;
            },
            err =>
            {
                log.Add($"Error: {err.Message}");
                return Task.CompletedTask;
            }
        );
        await Task.WhenAll(tasks);

        // Assert
        var expected = new List<string>();
        foreach (var input in await Task.WhenAll(inputs))
        {
            input.Match(
                val => expected.Add($"Success: {val}"),
                err => expected.Add($"Error: {err.Message}")
            );
        }
        log.Should().BeEquivalentTo(expected);
    }

    private class MatchAwaitEach_OverloadB_PropagateException_Data
        : TheoryData<
            IEnumerable<Task<Result<int>>>,
            Func<int, Task>,
            Func<Exception, Task>,
            Exception
        >
    {
        public MatchAwaitEach_OverloadB_PropagateException_Data()
        {
            // Exception in success action
            Add(
                [Task.FromResult((Result<int>)42)],
                val => throw new InvalidCastException("Exception in success action"),
                err => Task.CompletedTask,
                new InvalidCastException("Exception in success action")
            );

            // Exception in failure action
            Add(
                [Task.FromResult((Result<int>)new ArgumentException("Original error"))],
                val => Task.CompletedTask,
                err => throw new DivideByZeroException("Exception in failure action"),
                new DivideByZeroException("Exception in failure action")
            );

            // Exception in async success task
            Add(
                [Task.FromResult((Result<int>)42)],
                val =>
                    Task.FromException(new IndexOutOfRangeException("Async exception in success")),
                err => Task.CompletedTask,
                new IndexOutOfRangeException("Async exception in success")
            );

            // Exception in async failure task
            Add(
                [Task.FromResult((Result<int>)new ArgumentException("Original error"))],
                val => Task.CompletedTask,
                err => Task.FromException(new KeyNotFoundException("Async exception in failure")),
                new KeyNotFoundException("Async exception in failure")
            );
        }
    }

    [Theory]
    [ClassData(typeof(MatchAwaitEach_OverloadB_PropagateException_Data))]
    public async Task MatchAwaitEachB_Should_PropagateException(
        IEnumerable<Task<Result<int>>> inputs,
        Func<int, Task> success,
        Func<Exception, Task> failure,
        Exception expected
    )
    {
        // Act
        var matchedResults = inputs.MatchAwaitEach(success, failure);
        Func<Task> act = () => Task.WhenAll(matchedResults);

        // Assert
        await act.Should()
            .ThrowAsync<Exception>()
            .WithMessage(expected.Message)
            .Where(x => x.GetType() == expected.GetType());
    }
}
