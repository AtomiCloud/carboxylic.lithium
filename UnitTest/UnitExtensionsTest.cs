using CarboxylicLithium;
using FluentAssertions;

namespace UnitTest;

public class UnitExtensionsTest
{
    // ========================================
    // Unit<T1> Extension Method
    // ========================================

    private class Unit_T1_Should_ConvertActionToFuncReturningUnit_Data
        : TheoryData<Action<string>, string>
    {
        public Unit_T1_Should_ConvertActionToFuncReturningUnit_Data()
        {
            // Case 1: Action that does nothing with regular string
            Add(s => { }, "test");

            // Case 2: Action with different string
            Add(s => { }, "hello world");

            // Case 3: Action with empty string
            Add(s => { }, string.Empty);
        }
    }

    [Theory]
    [ClassData(typeof(Unit_T1_Should_ConvertActionToFuncReturningUnit_Data))]
    public void Unit_T1_Should_ConvertActionToFuncReturningUnit(Action<string> action, string input)
    {
        // Act
        var actualFunc = action.Unit();
        var actual = actualFunc(input);
        var expected = new Unit();
        // Assert
        actual.Should().Be(expected);
    }

    // [Fact]
    // public void Unit_T1_Should_ExecuteOriginalAction()
    // {
    //     // Arrange
    //     var wasCalled = false;

    //     // Act
    //     var unitFunc = ((Action<int>)Action).Unit();
    //     unitFunc(42);

    //     // Assert
    //     wasCalled.Should().BeTrue();
    //     return;

    //     void Action(int x)
    //     {
    //         wasCalled = true;
    //     }
    // }

    // ========================================
    // Unit<T1,T2> Extension Method
    // ========================================

    private class Unit_T1_T2_Should_ConvertActionToFuncReturningUnit_Data
        : TheoryData<Action<int, string>, int, string>
    {
        public Unit_T1_T2_Should_ConvertActionToFuncReturningUnit_Data()
        {
            // Case 1: Simple values
            Add((i, s) => { }, 1, "test");

            // Case 2: Zero and empty string
            Add((i, s) => { }, 0, string.Empty);

            // Case 3: Negative number and longer string
            Add((i, s) => { }, -10, "longer test string");
        }
    }

    [Theory]
    [ClassData(typeof(Unit_T1_T2_Should_ConvertActionToFuncReturningUnit_Data))]
    public void Unit_T1_T2_Should_ConvertActionToFuncReturningUnit(
        Action<int, string> action,
        int input1,
        string input2
    )
    {
        // Act
        var actualFunc = action.Unit();
        var actual = actualFunc(input1, input2);
        var expected = new Unit();
        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Unit_T1_T2_Should_ExecuteOriginalAction()
    {
        // Arrange
        var sum = 0;

        void Action(int x, int y)
        {
            sum = x + y;
        }

        // Act
        var unitFunc = ((Action<int, int>)Action).Unit();
        unitFunc(20, 22);

        // Assert
        sum.Should().Be(42);
    }

    // ========================================
    // Unit<T1,T2,T3> Extension Method
    // ========================================

    private class Unit_T1_T2_T3_Should_ConvertActionToFuncReturningUnit_Data
        : TheoryData<Action<int, string, bool>, int, string, bool>
    {
        public Unit_T1_T2_T3_Should_ConvertActionToFuncReturningUnit_Data()
        {
            // Case 1: Basic values
            Add((i, s, b) => { }, 1, "test", true);

            // Case 2: Different values
            Add((i, s, b) => { }, 42, "hello", false);

            // Case 3: Edge cases
            Add((i, s, b) => { }, 0, string.Empty, true);
        }
    }

    [Theory]
    [ClassData(typeof(Unit_T1_T2_T3_Should_ConvertActionToFuncReturningUnit_Data))]
    public void Unit_T1_T2_T3_Should_ConvertActionToFuncReturningUnit(
        Action<int, string, bool> action,
        int input1,
        string input2,
        bool input3
    )
    {
        // Act
        var actualFunc = action.Unit();
        var actual = actualFunc(input1, input2, input3);
        var expected = new Unit();
        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Unit_T1_T2_T3_Should_ExecuteOriginalAction()
    {
        // Arrange
        var result = string.Empty;

        // Act
        var unitFunc = ((Action<int, string, bool>)Action).Unit();
        unitFunc(42, "test", true);

        // Assert
        result.Should().Be("42 - test");
        return;

        void Action(int x, string s, bool b)
        {
            result = b ? $"{x} - {s}" : s;
        }
    }

    // ========================================
    // Unit<T1,T2,T3,T4> Extension Method
    // ========================================

    private class Unit_T1_T2_T3_T4_Should_ConvertActionToFuncReturningUnit_Data
        : TheoryData<Action<int, string, bool, char>, int, string, bool, char>
    {
        public Unit_T1_T2_T3_T4_Should_ConvertActionToFuncReturningUnit_Data()
        {
            // Case 1: Basic values
            Add((i, s, b, c) => { }, 1, "test", true, 'A');

            // Case 2: Different values
            Add((i, s, b, c) => { }, 42, "hello", false, '!');

            // Case 3: Edge cases
            Add((i, s, b, c) => { }, 0, string.Empty, true, ' ');
        }
    }

    [Theory]
    [ClassData(typeof(Unit_T1_T2_T3_T4_Should_ConvertActionToFuncReturningUnit_Data))]
    public void Unit_T1_T2_T3_T4_Should_ConvertActionToFuncReturningUnit(
        Action<int, string, bool, char> action,
        int input1,
        string input2,
        bool input3,
        char input4
    )
    {
        // Act
        var actualFunc = action.Unit();
        var actual = actualFunc(input1, input2, input3, input4);
        var expected = new Unit();
        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Unit_T1_T2_T3_T4_Should_ExecuteOriginalAction()
    {
        // Arrange
        var result = string.Empty;
        Action<int, string, bool, char> action = (x, s, b, c) =>
            result = $"{s}{c}{x}{(b ? "True" : "False")}";

        // Act
        var unitFunc = action.Unit();
        unitFunc(123, "test", true, '-');

        // Assert
        result.Should().Be("test-123True");
    }

    // ========================================
    // Unit<T1,T2,T3,T4,T5> Extension Method
    // ========================================

    private class Unit_T1_T2_T3_T4_T5_Should_ConvertActionToFuncReturningUnit_Data
        : TheoryData<Action<int, string, bool, char, double>, int, string, bool, char, double>
    {
        public Unit_T1_T2_T3_T4_T5_Should_ConvertActionToFuncReturningUnit_Data()
        {
            // Case 1: Basic values
            Add((i, s, b, c, d) => { }, 1, "test", true, 'A', 1.5);

            // Case 2: Different values
            Add((i, s, b, c, d) => { }, 42, "hello", false, '!', 0.0);

            // Case 3: Edge cases
            Add((i, s, b, c, d) => { }, 0, string.Empty, true, ' ', -2.5);
        }
    }

    [Theory]
    [ClassData(typeof(Unit_T1_T2_T3_T4_T5_Should_ConvertActionToFuncReturningUnit_Data))]
    public void Unit_T1_T2_T3_T4_T5_Should_ConvertActionToFuncReturningUnit(
        Action<int, string, bool, char, double> action,
        int input1,
        string input2,
        bool input3,
        char input4,
        double input5
    )
    {
        // Act
        var actualFunc = action.Unit();
        var actual = actualFunc(input1, input2, input3, input4, input5);
        var expected = new Unit();
        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Unit_T1_T2_T3_T4_T5_Should_ExecuteOriginalAction()
    {
        // Arrange
        var sum = 0.0;
        void action(int x, string s, bool b, char c, double d)
        {
            sum = x + d;
        }

        // Act
        var unitFunc = ((Action<int, string, bool, char, double>)action).Unit();
        unitFunc(40, "ignored", true, 'X', 2.0);

        // Assert
        sum.Should().Be(42.0);
    }

    // ========================================
    // Unit<T1...T6> Extension Method
    // ========================================

    private class Unit_T1_To_T6_Should_ConvertActionToFuncReturningUnit_Data
        : TheoryData<Action<int, int, int, int, int, int>, int, int, int, int, int, int>
    {
        public Unit_T1_To_T6_Should_ConvertActionToFuncReturningUnit_Data()
        {
            // Case 1: Basic values
            Add((a, b, c, d, e, f) => { }, 1, 2, 3, 4, 5, 6);

            // Case 2: Different values
            Add((a, b, c, d, e, f) => { }, 10, 20, 30, 40, 50, 60);

            // Case 3: Edge cases
            Add((a, b, c, d, e, f) => { }, 0, 0, 0, 0, 0, 0);
        }
    }

    [Theory]
    [ClassData(typeof(Unit_T1_To_T6_Should_ConvertActionToFuncReturningUnit_Data))]
    public void Unit_T1_To_T6_Should_ConvertActionToFuncReturningUnit(
        Action<int, int, int, int, int, int> action,
        int a,
        int b,
        int c,
        int d,
        int e,
        int f
    )
    {
        // Act
        var actualFunc = action.Unit();
        var actual = actualFunc(a, b, c, d, e, f);
        var expected = new Unit();
        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Unit_T1_To_T6_Should_ExecuteOriginalAction()
    {
        // Arrange
        var result = 0;
        Action<int, int, int, int, int, int> action = (a, b, c, d, e, f) =>
            result = a + b + c + d + e + f;

        // Act
        var unitFunc = action.Unit();
        unitFunc(1, 2, 3, 4, 5, 6);

        // Assert
        result.Should().Be(21);
    }

    // ========================================
    // Unit<T1...T7> Extension Method
    // ========================================

    private class Unit_T1_To_T7_Should_ConvertActionToFuncReturningUnit_Data
        : TheoryData<
            Action<char, char, char, char, char, char, char>,
            char,
            char,
            char,
            char,
            char,
            char,
            char
        >
    {
        public Unit_T1_To_T7_Should_ConvertActionToFuncReturningUnit_Data()
        {
            // Case 1: Basic values
            Add((a, b, c, d, e, f, g) => { }, 'T', 'E', 'S', 'T', 'I', 'N', 'G');

            // Case 2: Different values
            Add((a, b, c, d, e, f, g) => { }, 'A', 'B', 'C', 'D', 'E', 'F', 'G');

            // Case 3: Edge cases
            Add((a, b, c, d, e, f, g) => { }, ' ', ' ', ' ', ' ', ' ', ' ', ' ');
        }
    }

    [Theory]
    [ClassData(typeof(Unit_T1_To_T7_Should_ConvertActionToFuncReturningUnit_Data))]
    public void Unit_T1_To_T7_Should_ConvertActionToFuncReturningUnit(
        Action<char, char, char, char, char, char, char> action,
        char a,
        char b,
        char c,
        char d,
        char e,
        char f,
        char g
    )
    {
        // Act
        var actualFunc = action.Unit();
        var actual = actualFunc(a, b, c, d, e, f, g);
        var expected = new Unit();
        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Unit_T1_To_T7_Should_ExecuteOriginalAction()
    {
        // Arrange
        var concatenated = string.Empty;
        Action<char, char, char, char, char, char, char> action = (a, b, c, d, e, f, g) =>
            concatenated = new string(new[] { a, b, c, d, e, f, g });

        // Act
        var unitFunc = action.Unit();
        unitFunc('T', 'E', 'S', 'T', 'I', 'N', 'G');

        // Assert
        concatenated.Should().Be("TESTING");
    }

    // ========================================
    // Unit<T1...T8> Extension Method
    // ========================================

    private class Unit_T1_To_T8_Should_ConvertActionToFuncReturningUnit_Data
        : TheoryData<
            Action<int, int, int, int, int, int, int, int>,
            int,
            int,
            int,
            int,
            int,
            int,
            int,
            int
        >
    {
        public Unit_T1_To_T8_Should_ConvertActionToFuncReturningUnit_Data()
        {
            // Case 1: Basic values
            Add((a, b, c, d, e, f, g, h) => { }, 1, 2, 3, 4, 5, 6, 7, 8);

            // Case 2: Different values
            Add((a, b, c, d, e, f, g, h) => { }, 10, 20, 30, 40, 50, 60, 70, 80);

            // Case 3: Edge cases
            Add((a, b, c, d, e, f, g, h) => { }, 0, 0, 0, 0, 0, 0, 0, 0);
        }
    }

    [Theory]
    [ClassData(typeof(Unit_T1_To_T8_Should_ConvertActionToFuncReturningUnit_Data))]
    public void Unit_T1_To_T8_Should_ConvertActionToFuncReturningUnit(
        Action<int, int, int, int, int, int, int, int> action,
        int a,
        int b,
        int c,
        int d,
        int e,
        int f,
        int g,
        int h
    )
    {
        // Act
        var actualFunc = action.Unit();
        var actual = actualFunc(a, b, c, d, e, f, g, h);
        var expected = new Unit();
        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Unit_T1_To_T8_Should_ExecuteOriginalAction()
    {
        // Arrange
        var product = 1;
        Action<int, int, int, int, int, int, int, int> action = (a, b, c, d, e, f, g, h) =>
            product = a * b * c * d * e * f * g * h;

        // Act
        var unitFunc = action.Unit();
        unitFunc(1, 2, 3, 4, 5, 6, 7, 8);

        // Assert
        product.Should().Be(40320); // 8!
    }

    // For brevity, I'll implement the remaining methods (9-15) with less detailed variations
    // but following the same pattern

    // ========================================
    // Unit<T1...T9> Extension Method
    // ========================================

    [Fact]
    public void Unit_T1_To_T9_Should_ConvertActionToFuncReturningUnit()
    {
        // Arrange
        Action<int, int, int, int, int, int, int, int, int> action = (
            a,
            b,
            c,
            d,
            e,
            f,
            g,
            h,
            i
        ) => { };

        // Act
        var actualFunc = action.Unit();
        var actual = actualFunc(1, 2, 3, 4, 5, 6, 7, 8, 9);
        var expected = new Unit();

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Unit_T1_To_T9_Should_ExecuteOriginalAction()
    {
        // Arrange
        var sum = 0;
        Action<int, int, int, int, int, int, int, int, int> action = (a, b, c, d, e, f, g, h, i) =>
            sum = a + b + c + d + e + f + g + h + i;

        // Act
        var unitFunc = action.Unit();
        unitFunc(1, 2, 3, 4, 5, 6, 7, 8, 9);

        // Assert
        sum.Should().Be(45); // Sum of 1-9
    }

    // ========================================
    // Unit<T1...T10> Extension Method
    // ========================================

    [Fact]
    public void Unit_T1_To_T10_Should_ConvertActionToFuncReturningUnit()
    {
        // Arrange
        Action<int, int, int, int, int, int, int, int, int, int> action = (
            a,
            b,
            c,
            d,
            e,
            f,
            g,
            h,
            i,
            j
        ) => { };

        // Act
        var actualFunc = action.Unit();
        var actual = actualFunc(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
        var expected = new Unit();

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Unit_T1_To_T10_Should_ExecuteOriginalAction()
    {
        // Arrange
        var wasCalled = false;
        var parameters = new int[10];
        Action<int, int, int, int, int, int, int, int, int, int> action = (
            a,
            b,
            c,
            d,
            e,
            f,
            g,
            h,
            i,
            j
        ) =>
        {
            wasCalled = true;
            parameters[0] = a;
            parameters[1] = b;
            parameters[2] = c;
            parameters[3] = d;
            parameters[4] = e;
            parameters[5] = f;
            parameters[6] = g;
            parameters[7] = h;
            parameters[8] = i;
            parameters[9] = j;
        };

        // Act
        var unitFunc = action.Unit();
        unitFunc(0, 1, 2, 3, 4, 5, 6, 7, 8, 9);

        // Assert
        wasCalled.Should().BeTrue();
        parameters.Should().BeEquivalentTo(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
    }

    // ========================================
    // Unit<T1...T11> Extension Method
    // ========================================

    [Fact]
    public void Unit_T1_To_T11_Should_ConvertActionToFuncReturningUnit()
    {
        // Arrange
        Action<int, int, int, int, int, int, int, int, int, int, int> action = (
            a,
            b,
            c,
            d,
            e,
            f,
            g,
            h,
            i,
            j,
            k
        ) => { };

        // Act
        var actualFunc = action.Unit();
        var actual = actualFunc(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);
        var expected = new Unit();

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Unit_T1_To_T11_Should_ExecuteOriginalAction()
    {
        // Arrange
        var wasCalled = false;
        var lastParam = 0;
        Action<int, int, int, int, int, int, int, int, int, int, int> action = (
            a,
            b,
            c,
            d,
            e,
            f,
            g,
            h,
            i,
            j,
            k
        ) =>
        {
            wasCalled = true;
            lastParam = k;
        };

        // Act
        var unitFunc = action.Unit();
        unitFunc(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);

        // Assert
        wasCalled.Should().BeTrue();
        lastParam.Should().Be(11);
    }

    // ========================================
    // Unit<T1...T12> Extension Method
    // ========================================

    [Fact]
    public void Unit_T1_To_T12_Should_ConvertActionToFuncReturningUnit()
    {
        // Arrange
        Action<int, int, int, int, int, int, int, int, int, int, int, int> action = (
            a,
            b,
            c,
            d,
            e,
            f,
            g,
            h,
            i,
            j,
            k,
            l
        ) => { };

        // Act
        var actualFunc = action.Unit();
        var actual = actualFunc(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);
        var expected = new Unit();

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Unit_T1_To_T12_Should_ExecuteOriginalAction()
    {
        // Arrange
        var wasCalled = false;
        var lastParam = 0;
        Action<int, int, int, int, int, int, int, int, int, int, int, int> action = (
            a,
            b,
            c,
            d,
            e,
            f,
            g,
            h,
            i,
            j,
            k,
            l
        ) =>
        {
            wasCalled = true;
            lastParam = l;
        };

        // Act
        var unitFunc = action.Unit();
        unitFunc(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);

        // Assert
        wasCalled.Should().BeTrue();
        lastParam.Should().Be(12);
    }

    // ========================================
    // Unit<T1...T13> Extension Method
    // ========================================

    [Fact]
    public void Unit_T1_To_T13_Should_ConvertActionToFuncReturningUnit()
    {
        // Arrange
        Action<int, int, int, int, int, int, int, int, int, int, int, int, int> action = (
            a,
            b,
            c,
            d,
            e,
            f,
            g,
            h,
            i,
            j,
            k,
            l,
            m
        ) => { };

        // Act
        var actualFunc = action.Unit();
        var actual = actualFunc(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13);
        var expected = new Unit();

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Unit_T1_To_T13_Should_ExecuteOriginalAction()
    {
        // Arrange
        var wasCalled = false;
        var lastParam = 0;
        Action<int, int, int, int, int, int, int, int, int, int, int, int, int> action = (
            a,
            b,
            c,
            d,
            e,
            f,
            g,
            h,
            i,
            j,
            k,
            l,
            m
        ) =>
        {
            wasCalled = true;
            lastParam = m;
        };

        // Act
        var unitFunc = action.Unit();
        unitFunc(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13);

        // Assert
        wasCalled.Should().BeTrue();
        lastParam.Should().Be(13);
    }

    // ========================================
    // Unit<T1...T14> Extension Method
    // ========================================

    [Fact]
    public void Unit_T1_To_T14_Should_ConvertActionToFuncReturningUnit()
    {
        // Arrange
        Action<int, int, int, int, int, int, int, int, int, int, int, int, int, int> action = (
            a,
            b,
            c,
            d,
            e,
            f,
            g,
            h,
            i,
            j,
            k,
            l,
            m,
            n
        ) => { };

        // Act
        var actualFunc = action.Unit();
        var actual = actualFunc(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14);
        var expected = new Unit();

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Unit_T1_To_T14_Should_ExecuteOriginalAction()
    {
        // Arrange
        var wasCalled = false;
        var lastParam = 0;
        Action<int, int, int, int, int, int, int, int, int, int, int, int, int, int> action = (
            a,
            b,
            c,
            d,
            e,
            f,
            g,
            h,
            i,
            j,
            k,
            l,
            m,
            n
        ) =>
        {
            wasCalled = true;
            lastParam = n;
        };

        // Act
        var unitFunc = action.Unit();
        unitFunc(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14);

        // Assert
        wasCalled.Should().BeTrue();
        lastParam.Should().Be(14);
    }

    // ========================================
    // Unit<T1...T15> Extension Method
    // ========================================

    [Fact]
    public void Unit_T1_To_T15_Should_ConvertActionToFuncReturningUnit()
    {
        // Arrange
        Action<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int> action = (
            a,
            b,
            c,
            d,
            e,
            f,
            g,
            h,
            i,
            j,
            k,
            l,
            m,
            n,
            o
        ) => { };

        // Act
        var actualFunc = action.Unit();
        var actual = actualFunc(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15);
        var expected = new Unit();

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Unit_T1_To_T15_Should_ExecuteOriginalAction()
    {
        // Arrange
        var wasCalled = false;
        var lastParam = 0;
        Action<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int> action = (
            a,
            b,
            c,
            d,
            e,
            f,
            g,
            h,
            i,
            j,
            k,
            l,
            m,
            n,
            o
        ) =>
        {
            wasCalled = true;
            lastParam = o;
        };

        // Act
        var unitFunc = action.Unit();
        unitFunc(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15);

        // Assert
        wasCalled.Should().BeTrue();
        lastParam.Should().Be(15);
    }
}
