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
                (List<Task<Result<int>>>)
                    [
                        Task.FromResult((Result<int>)1),
                        Task.FromResult((Result<int>)2),
                        Task.FromResult((Result<int>)3),
                        Task.FromResult((Result<int>)0),
                        Task.FromResult((Result<int>)(-1)),
                    ]
            );

            // Case 2: Empty collection (which means no failures too)
            Add((List<Task<Result<int>>>)[]);
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
                (List<Task<Result<int>>>)
                    [
                        Task.FromResult((Result<int>)1),
                        Task.FromResult((Result<int>)new InvalidOperationException()),
                        Task.FromResult((Result<int>)3),
                        Task.FromResult((Result<int>)(-5)),
                    ]
            );

            // Case 2: Mixed, two failures
            Add(
                (List<Task<Result<int>>>)
                    [
                        Task.FromResult(
                            (Result<int>)new InvalidOperationException("First failure")
                        ),
                        Task.FromResult((Result<int>)1),
                        Task.FromResult((Result<int>)new Exception("Second failure")),
                        Task.FromResult((Result<int>)2),
                        Task.FromResult((Result<int>)(-3)),
                    ]
            );

            // Case 3: All failures
            Add(
                (List<Task<Result<int>>>)
                    [
                        Task.FromResult((Result<int>)new Exception("Failure")),
                        Task.FromResult(
                            (Result<int>)new InvalidOperationException("Second Failure")
                        ),
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
                (List<Task<Result<int>>>)
                    [
                        Task.FromResult((Result<int>)new Exception("Error")),
                        Task.FromResult((Result<int>)2),
                        Task.FromResult((Result<int>)new InvalidOperationException()),
                    ]
            );

            // Case 3: Success last
            Add(
                (List<Task<Result<int>>>)
                    [
                        Task.FromResult((Result<int>)new Exception()),
                        Task.FromResult((Result<int>)new InvalidOperationException()),
                        Task.FromResult((Result<int>)3),
                    ]
            );

            // Case 4: Multiple successes
            Add(
                (List<Task<Result<int>>>)
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
                (List<Task<Result<int>>>)
                    [
                        Task.FromResult((Result<int>)new Exception()),
                        Task.FromResult((Result<int>)new InvalidOperationException()),
                        Task.FromResult((Result<int>)new Exception("Another error")),
                    ]
            );

            // Case 2: Empty collection, false because there's no success
            Add((List<Task<Result<int>>>)[]);
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
                (List<Task<Result<int>>>)
                    [
                        Task.FromResult((Result<int>)new Exception()),
                        Task.FromResult((Result<int>)new InvalidOperationException()),
                        Task.FromResult((Result<int>)new Exception("Another error")),
                    ]
            );

            // Case 2: Empty collection, should return true as there are no successes
            Add((List<Task<Result<int>>>)[]);
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
                (List<Task<Result<int>>>)
                    [
                        Task.FromResult((Result<int>)1),
                        Task.FromResult((Result<int>)new InvalidOperationException()),
                        Task.FromResult((Result<int>)new Exception()),
                    ]
            );

            // Case 2: Success in the middle
            Add(
                (List<Task<Result<int>>>)
                    [
                        Task.FromResult((Result<int>)new Exception("Error")),
                        Task.FromResult((Result<int>)2),
                        Task.FromResult((Result<int>)new InvalidOperationException()),
                    ]
            );

            // Case 3: Success last
            Add(
                (List<Task<Result<int>>>)
                    [
                        Task.FromResult((Result<int>)new Exception()),
                        Task.FromResult((Result<int>)new InvalidOperationException()),
                        Task.FromResult((Result<int>)3),
                    ]
            );

            // Case 4: Mixed with multiple successes
            Add(
                (List<Task<Result<int>>>)
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
                (List<Task<Result<int>>>)
                    [
                        Task.FromResult((Result<int>)new InvalidOperationException()),
                        Task.FromResult((Result<int>)1),
                        Task.FromResult((Result<int>)2),
                    ]
            );

            // Case 2: Failure in the middle
            Add(
                (List<Task<Result<int>>>)
                    [
                        Task.FromResult((Result<int>)1),
                        Task.FromResult((Result<int>)new Exception("Error")),
                        Task.FromResult((Result<int>)2),
                    ]
            );

            // Case 3: Failure last
            Add(
                (List<Task<Result<int>>>)
                    [
                        Task.FromResult((Result<int>)1),
                        Task.FromResult((Result<int>)2),
                        Task.FromResult((Result<int>)new Exception()),
                    ]
            );

            // Case 4: Multiple failures
            Add(
                (List<Task<Result<int>>>)
                    [
                        Task.FromResult((Result<int>)new Exception("First failure")),
                        Task.FromResult(
                            (Result<int>)new InvalidOperationException("Second failure")
                        ),
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
                (List<Task<Result<int>>>)
                    [
                        Task.FromResult((Result<int>)1),
                        Task.FromResult((Result<int>)2),
                        Task.FromResult((Result<int>)0),
                    ]
            );

            // Case 2: Empty collection, naturally returning false
            Add((List<Task<Result<int>>>)[]);
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
                (List<Task<Result<int>>>)
                    [
                        Task.FromResult((Result<int>)1),
                        Task.FromResult((Result<int>)2),
                        Task.FromResult((Result<int>)3),
                    ],
                (List<int>)[1, 2, 3]
            );

            // Case 2: Including boundary values
            Add(
                (List<Task<Result<int>>>)
                    [
                        Task.FromResult((Result<int>)int.MaxValue),
                        Task.FromResult((Result<int>)int.MinValue),
                        Task.FromResult((Result<int>)0),
                    ],
                (List<int>)[int.MaxValue, int.MinValue, 0]
            );

            // Case 3: Empty collection leading to empty result
            Add((List<Task<Result<int>>>)[], (List<int>)[]);
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
                (List<Task<Result<int>>>)
                    [
                        Task.FromResult(
                            (Result<int>)new InvalidOperationException("First failure")
                        ),
                        Task.FromResult((Result<int>)2),
                        Task.FromResult((Result<int>)3),
                    ]
            );

            // Case 2: Failure in the middle
            Add(
                (List<Task<Result<int>>>)
                    [
                        Task.FromResult((Result<int>)1),
                        Task.FromResult((Result<int>)new ArgumentException("Middle failure")),
                        Task.FromResult((Result<int>)3),
                    ]
            );

            // Case 3: Failure at the end
            Add(
                (List<Task<Result<int>>>)
                    [
                        Task.FromResult((Result<int>)1),
                        Task.FromResult((Result<int>)2),
                        Task.FromResult((Result<int>)new Exception("Last failure")),
                    ]
            );

            // Case 4: Multiple failures
            Add(
                (List<Task<Result<int>>>)
                    [
                        Task.FromResult((Result<int>)new Exception("Multi-failure 1")),
                        Task.FromResult(
                            (Result<int>)new InvalidOperationException("Multi-failure 2")
                        ),
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
        var act = async () =>
        {
            _ = await collection.Get();
        };

        // Assert
        await act.Should().ThrowAsync<AggregateException>("there were failures in the collection.");
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
                (List<Task<Result<int>>>)
                    [Task.FromResult((Result<int>)ex1), Task.FromResult((Result<int>)ex2)],
                (List<Exception>)[ex1, ex2]
            );

            // Case 2: Mixed entries
            var ex3 = new Exception("General Error");
            Add(
                (List<Task<Result<int>>>)
                    [
                        Task.FromResult((Result<int>)1),
                        Task.FromResult((Result<int>)ex3),
                        Task.FromResult((Result<int>)3),
                    ],
                (List<Exception>)[ex3]
            );

            // Case 3: Empty collection, expecting empty result
            Add((List<Task<Result<int>>>)[], (List<Exception>)[]);
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
            Add(
                (List<Task<Result<int>>>)
                    [Task.FromResult((Result<int>)1), Task.FromResult((Result<int>)2)]
            );
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
                (List<Task<Result<int>>>)
                    [
                        Task.FromResult((Result<int>)1),
                        Task.FromResult((Result<int>)2),
                        Task.FromResult((Result<int>)3),
                    ],
                (List<int>)[1, 2, 3]
            );

            // Case 2: Mixed, with more successes than failures
            Add(
                (List<Task<Result<int>>>)
                    [
                        Task.FromResult((Result<int>)1),
                        Task.FromResult((Result<int>)new Exception("Failure")),
                        Task.FromResult((Result<int>)2),
                        Task.FromResult((Result<int>)3),
                    ],
                (List<int>)[1, 2, 3]
            );

            // Case 3: Empty list gives empty success list
            Add((List<Task<Result<int>>>)[], (List<int>)[]);
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
                (List<Task<Result<int>>>)
                    [
                        Task.FromResult<Result<int>>(new Exception("Failure")),
                        Task.FromResult<Result<int>>(
                            new InvalidOperationException("Another failure")
                        ),
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
                (List<Task<Result<int>>>)
                    [
                        Task.FromResult((Result<int>)1),
                        Task.FromResult((Result<int>)2),
                        Task.FromResult((Result<int>)3),
                    ],
                (List<Result<int>>)[1, 2, 3]
            );

            // Case 2: Mixed results
            Add(
                (List<Task<Result<int>>>)
                    [
                        Task.FromResult((Result<int>)1),
                        Task.FromResult((Result<int>)new InvalidOperationException("Fail")),
                        Task.FromResult((Result<int>)3),
                    ],
                (List<Result<int>>)[1, new InvalidOperationException("Fail"), 3]
            );

            // Case 3: Empty collection
            Add((List<Task<Result<int>>>)[], (List<Result<int>>)[]);
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
                (List<Task<Result<int>>>)
                    [
                        Task.FromResult((Result<int>)1),
                        Task.FromResult((Result<int>)new InvalidOperationException("Fail")),
                        Task.FromResult((Result<int>)2),
                    ]
            );

            // Case 2: All successes
            Add(
                (List<Task<Result<int>>>)
                    [
                        Task.FromResult((Result<int>)10),
                        Task.FromResult((Result<int>)20),
                        Task.FromResult((Result<int>)30),
                    ]
            );

            // Case 3: All failures
            Add(
                (List<Task<Result<int>>>)
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
                (List<Task<Result<int>>>)
                    [
                        Task.FromResult((Result<int>)1),
                        Task.FromResult((Result<int>)new InvalidOperationException("Failure")),
                        Task.FromResult((Result<int>)2),
                    ],
                (List<Result<int>>)[1, new InvalidOperationException("Failure"), 2]
            );

            // Case 2: All successes
            Add(
                (List<Task<Result<int>>>)
                    [
                        Task.FromResult((Result<int>)10),
                        Task.FromResult((Result<int>)20),
                        Task.FromResult((Result<int>)30),
                    ],
                (List<Result<int>>)[10, 20, 30]
            );

            // Case 3: All failures
            Add(
                (List<Task<Result<int>>>)
                    [
                        Task.FromResult((Result<int>)new Exception("Error 1")),
                        Task.FromResult((Result<int>)new InvalidOperationException("Error 2")),
                    ],
                (List<Result<int>>)
                    [new Exception("Error 1"), new InvalidOperationException("Error 2")]
            );

            // Case 4: Empty collection
            Add((List<Task<Result<int>>>)[], (List<Result<int>>)[]);
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
        act.Should()
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
        act.Should()
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

    // ================================================================================
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
    // ================================================================================

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
                new InvalidOperationException($"Func physically threw for i=10")
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
        act.Should()
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
}
