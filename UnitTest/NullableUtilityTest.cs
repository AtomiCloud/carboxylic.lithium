// using CarboxylicLithium;
// using FluentAssertions;

// namespace UnitTest;

// public class NullableUtilityTest
// {
//     // ======================================
//     // 1) ToNullableResultOr<T> where struct
//     // ======================================

//     // Data for handling null input for integer
//     [Fact]
//     public void ToNullableResultOr_Should_HandleNullInputForInteger()
//     {
//         // Arrange
//         int? input = null;

//         // Act
//         var actual = input.ToNullableResultOr<int, int>(x => x * 2);
//         var expected = new Result<int?>((int?)null);
//         // Assert
//         actual.Should().BeEquivalentTo(expected);
//     }

//     // Data for handling nullable integer inputs
//     private class ToNullableResultOr_WithIntegerInputs_Data
//         : TheoryData<int?, Func<int, Result<int>>, Result<int?>>
//     {
//         public ToNullableResultOr_WithIntegerInputs_Data()
//         {
//             Add(10, x => -x * 2, -20);
//             Add(5, x => x, 5);
//             Add(-3, x => x + 10, 7);
//         }
//     }

//     [Theory]
//     [ClassData(typeof(ToNullableResultOr_WithIntegerInputs_Data))]
//     public void ToNullableResultOr_Should_HandleIntegerInputs(
//         int? input,
//         Func<int, Result<int>> act,
//         Result<int?> expected
//     )
//     {
//         // Act
//         var actual = input.ToNullableResultOr(act);

//         // Assert
//         actual.Should().BeEquivalentTo(expected);
//     }

//     // ======================================
//     // 2) ToNullableResultOr<T> where class
//     // ======================================

//     [Fact]
//     public void ToNullableResultOr_Should_HandleNullInputForClass()
//     {
//         // Arrange
//         Person? input = null;

//         // Act
//         var actual = input.ToNullableResultOr<Person, Employee>(x => new Employee
//         {
//             Name = x.Name,
//             Salary = x.Age * 2,
//         });
//         var expected = new Result<Employee?>((Employee?)null);
//         // Assert
//         actual.Should().BeEquivalentTo(expected);
//     }

//     // Data for handling nullable objects inputs
//     private class ToNullableResultOr_WithObjectInputs_Data
//         : TheoryData<Person?, Func<Person, Result<Employee>>, Result<Employee?>>
//     {
//         public ToNullableResultOr_WithObjectInputs_Data()
//         {
//             Add(
//                 new Person { Name = "Ernest", Age = 10 },
//                 x => new Employee { Name = x.Name, Salary = x.Age * 2 },
//                 new Employee { Name = "Ernest", Salary = 20 }
//             );

//             Add(
//                 new Person { Name = "John", Age = 20 },
//                 x => new Employee { Name = $"{x.Name}-{x.Age}", Salary = x.Age + x.Name.Length },
//                 new Employee { Name = "John-20", Salary = 24 }
//             );
//             Add(
//                 new Person { Name = "Jane", Age = 60 },
//                 x => new Employee { Name = x.Name + x.Name, Salary = x.Age - 5 },
//                 new Employee { Name = "JaneJane", Salary = 55 }
//             );
//         }
//     }

//     [Theory]
//     [ClassData(typeof(ToNullableResultOr_WithObjectInputs_Data))]
//     public void ToNullableResultOr_Should_HandleObjectInputs(
//         Person? input,
//         Func<Person, Result<Employee>> act,
//         Result<Employee?> expected
//     )
//     {
//         // Act
//         var actual = NullUtil.ToNullableResultOr(input, act);

//         // Assert
//         actual.Should().BeEquivalentTo(expected);
//     }

//     // ======================================
//     // 3) ToNullableTaskResultOr<T> where struct
//     // ======================================

//     [Fact]
//     public async Task ToNullableTaskResultOr_Should_HandleNullInputForInteger()
//     {
//         // Arrange
//         int? input = null;

//         // Act
//         var actual = await input.ToNullableTaskResultOr<int, int>(async x =>
//             await Task.FromResult((Result<int>)x * 2)
//         );
//         var expected = new Result<int?>((int?)null);

//         // Assert
//         actual.Should().BeEquivalentTo(expected);
//     }

//     private class ToNullableTaskResultOr_WithIntegerInputs_Data
//         : TheoryData<int?, Func<int, Task<Result<int>>>, Result<int?>>
//     {
//         public ToNullableTaskResultOr_WithIntegerInputs_Data()
//         {
//             Add(10, x => Task.FromResult((Result<int>)(x * -2)), -20);
//             Add(5, x => Task.FromResult((Result<int>)(x)), 5);
//             Add(-3, x => Task.FromResult((Result<int>)(x + 10)), 7);
//         }
//     }

//     [Theory]
//     [ClassData(typeof(ToNullableTaskResultOr_WithIntegerInputs_Data))]
//     public async Task ToNullableTaskResultOr_Should_HandleIntegerInputs(
//         int? input,
//         Func<int, Task<Result<int>>> act,
//         Result<int?> expected
//     )
//     {
//         // Act
//         var actual = await input.ToNullableTaskResultOr(act);

//         // Assert
//         actual.Should().BeEquivalentTo(expected);
//     }

//     // ======================================
//     // 4) ToNullableTaskResultOr<T> where class
//     // ======================================

//     [Fact]
//     public async Task ToNullableTaskResultOr_Should_HandleNullInputForClass()
//     {
//         // Arrange
//         Person? input = null;

//         // Act
//         var actual = await input.ToNullableTaskResultOr<Person, Employee>(async x =>
//             await Task.FromResult(
//                 (Result<Employee>)new Employee { Name = x.Name, Salary = x.Age * 2 }
//             )
//         );
//         var expected = new Result<Employee?>((Employee?)null);

//         // Assert
//         actual.Should().BeEquivalentTo(expected);
//     }

//     // Data for handling nullable objects inputs
//     private class ToNullableTaskResultOr_WithObjectInputs_Data
//         : TheoryData<Person?, Func<Person, Task<Result<Employee>>>, Result<Employee?>>
//     {
//         public ToNullableTaskResultOr_WithObjectInputs_Data()
//         {
//             Add(
//                 new Person { Name = "Ernest", Age = 10 },
//                 x =>
//                     Task.FromResult(
//                         (Result<Employee>)new Employee { Name = x.Name, Salary = x.Age * 2 }
//                     ),
//                 new Employee { Name = "Ernest", Salary = 20 }
//             );

//             Add(
//                 new Person { Name = "John", Age = 20 },
//                 x =>
//                     Task.FromResult(
//                         (Result<Employee>)
//                             new Employee
//                             {
//                                 Name = $"{x.Name}-{x.Age}",
//                                 Salary = x.Age + x.Name.Length,
//                             }
//                     ),
//                 new Employee { Name = "John-20", Salary = 24 }
//             );

//             Add(
//                 new Person { Name = "Jane", Age = 60 },
//                 x =>
//                     Task.FromResult(
//                         (Result<Employee>)
//                             new Employee { Name = x.Name + x.Name, Salary = x.Age - 5 }
//                     ),
//                 new Employee { Name = "JaneJane", Salary = 55 }
//             );
//         }
//     }

//     [Theory]
//     [ClassData(typeof(ToNullableTaskResultOr_WithObjectInputs_Data))]
//     public async Task ToNullableTaskResultOr_Should_HandleObjectInputs(
//         Person? input,
//         Func<Person, Task<Result<Employee>>> act,
//         Result<Employee?> expected
//     )
//     {
//         // Act
//         var actual = await input.ToNullableTaskResultOr(act);

//         // Assert
//         actual.Should().BeEquivalentTo(expected);
//     }

//     // =======================
//     // 5) ToNullableResult<T>
//     // =======================

//     // Data for nullable primitive value types
//     private class ToNullableResult_PrimitiveValueTypes_Data : TheoryData<int?, Result<int?>>
//     {
//         public ToNullableResult_PrimitiveValueTypes_Data()
//         {
//             Add(null, new Result<int?>((int?)null));
//             Add(42, new Result<int?>(42));
//             Add(0, new Result<int?>(0));
//         }
//     }

//     [Theory]
//     [ClassData(typeof(ToNullableResult_PrimitiveValueTypes_Data))]
//     public void ToNullableResult_Should_HandlePrimitiveValueTypes(int? input, Result<int?> expected)
//     {
//         // Act
//         var actual = input.ToNullableResult();

//         // Assert
//         actual.Should().BeEquivalentTo(expected);
//     }

//     // Data for nullable reference types
//     private class ToNullableResult_ReferenceTypes_Data : TheoryData<string?, Result<string?>>
//     {
//         public ToNullableResult_ReferenceTypes_Data()
//         {
//             Add(null, new Result<string?>((string?)null));
//             Add("Hello", new Result<string?>("Hello"));
//             Add("", new Result<string?>(string.Empty));
//         }
//     }

//     [Theory]
//     [ClassData(typeof(ToNullableResult_ReferenceTypes_Data))]
//     public void ToNullableResult_Should_HandleReferenceTypes(
//         string? input,
//         Result<string?> expected
//     )
//     {
//         // Act
//         var actual = input.ToNullableResult();

//         // Assert
//         actual.Should().BeEquivalentTo(expected);
//     }

//     // ============================
//     // 6) ToNullableTaskResult<T>
//     // ============================

//     // Data for nullable primitive value types in async context
//     private class ToNullableTaskResult_PrimitiveValueTypes_Data
//         : TheoryData<int?, Task<Result<int?>>>
//     {
//         public ToNullableTaskResult_PrimitiveValueTypes_Data()
//         {
//             Add(null, Task.FromResult((Result<int?>)(int?)null));
//             Add(42, Task.FromResult((Result<int?>)42));
//             Add(0, Task.FromResult((Result<int?>)0));
//         }
//     }

//     [Theory]
//     [ClassData(typeof(ToNullableTaskResult_PrimitiveValueTypes_Data))]
//     public async Task ToNullableTaskResult_Should_HandlePrimitiveValueTypes(
//         int? input,
//         Task<Result<int?>> expectedTask
//     )
//     {
//         // Arrange
//         var expected = await expectedTask;

//         // Act
//         var actual = await input.ToNullableTaskResult();

//         // Assert
//         actual.Should().BeEquivalentTo(expected);
//     }

//     // Data for nullable reference types in async context
//     private class ToNullableTaskResult_ReferenceTypes_Data
//         : TheoryData<string?, Task<Result<string?>>>
//     {
//         public ToNullableTaskResult_ReferenceTypes_Data()
//         {
//             Add(null, Task.FromResult((Result<string?>)(string?)null));
//             Add("World", Task.FromResult((Result<string?>)"World"));
//             Add("", Task.FromResult((Result<string?>)string.Empty));
//         }
//     }

//     [Theory]
//     [ClassData(typeof(ToNullableTaskResult_ReferenceTypes_Data))]
//     public async Task ToNullableTaskResult_Should_HandleReferenceTypes(
//         string? input,
//         Task<Result<string?>> expectedTask
//     )
//     {
//         // Arrange
//         var expected = await expectedTask;

//         // Act
//         var actual = await input.ToNullableTaskResult();

//         // Assert
//         actual.Should().BeEquivalentTo(expected);
//     }
// }
