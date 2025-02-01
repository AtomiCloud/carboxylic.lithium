using System.Reflection;
using CarboxylicLithium;
using FluentAssertions;
using FluentAssertions.Equivalency;

namespace CarboxylicLithiumTestHelper;

public class ResultEquivalencyStep : IEquivalencyStep
{
    public EquivalencyResult Handle(
        Comparands comparands,
        IEquivalencyValidationContext context,
        IValidateChildNodeEquivalency nestedValidator
    )
    {
        var subjectOriginal = comparands.Subject;
        var expectationOriginal = comparands.Expectation;
        if (subjectOriginal == null)
            return EquivalencyResult.ContinueWithNext;
        if (expectationOriginal == null)
            return EquivalencyResult.ContinueWithNext;

        var subjectType = subjectOriginal.GetType();
        var expectationType = expectationOriginal.GetType();
        if (subjectType != expectationType)
            return EquivalencyResult.ContinueWithNext;

        if (!subjectType.IsGenericType)
            return EquivalencyResult.ContinueWithNext;
        if (!expectationType.IsGenericType)
            return EquivalencyResult.ContinueWithNext;

        if (subjectType.GetGenericTypeDefinition() != typeof(Result<>))
            return EquivalencyResult.ContinueWithNext;
        if (expectationType.GetGenericTypeDefinition() != typeof(Result<>))
            return EquivalencyResult.ContinueWithNext;

        // perform result comparison now
        // use reflection to get _success field

        var sType = subjectOriginal.GetType();
        var sSuccessField = sType.GetField(
            "_isSuccess",
            BindingFlags.Instance | BindingFlags.NonPublic
        );
        if (sSuccessField == null)
            return EquivalencyResult.ContinueWithNext;
        var sSuccess = sSuccessField.GetValue(subjectOriginal);
        if (sSuccess is not bool subject)
            return EquivalencyResult.ContinueWithNext;

        var eType = expectationOriginal.GetType();
        var eSuccessField = eType.GetField(
            "_isSuccess",
            BindingFlags.Instance | BindingFlags.NonPublic
        );
        if (eSuccessField == null)
            return EquivalencyResult.ContinueWithNext;
        var eSuccess = eSuccessField.GetValue(expectationOriginal);
        if (eSuccess is not bool expectation)
            return EquivalencyResult.ContinueWithNext;

        subject.Should().Be(expectation, "both results' success status should be true or false");
        if (subject && expectation)
        {
            // success case
            return EquivalencyResult.ContinueWithNext;
        }

        // failure case
        var sExceptionField = sType.GetField(
            "_exception",
            BindingFlags.Instance | BindingFlags.NonPublic
        );
        if (sExceptionField == null)
            return EquivalencyResult.ContinueWithNext;
        var sException = sExceptionField.GetValue(subjectOriginal);
        if (sException is not Exception sEx)
            return EquivalencyResult.ContinueWithNext;

        var eExceptionField = eType.GetField(
            "_exception",
            BindingFlags.Instance | BindingFlags.NonPublic
        );
        if (eExceptionField == null)
            return EquivalencyResult.ContinueWithNext;
        var eException = eExceptionField.GetValue(expectationOriginal);
        if (eException is not Exception eEx)
            return EquivalencyResult.ContinueWithNext;

        sEx.Should().BeOfType(eEx.GetType());
        sEx.Message.Should().BeEquivalentTo(eEx.Message);

        return EquivalencyResult.EquivalencyProven;
    }
}
