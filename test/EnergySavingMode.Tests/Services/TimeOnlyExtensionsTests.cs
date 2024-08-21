using EnergySavingMode.Services;
using FluentAssertions;

namespace EnergySavingMode.Tests.Services;

public class TimeOnlyExtensionsTests
{
    public class IsBefore
    {
        [Fact]
        public void Should_return_true_when_before()
        {
            // Arrange
            var t1 = new TimeOnly(12, 0);
            var t2 = new TimeOnly(13, 0);

            // Act
            var actual = TimeOnlyExtensions.IsBefore(t1, t2);

            // Assert
            actual.Should().BeTrue($"{t1} is before {t2}");
        }

        [Fact]
        public void Should_return_false_when_after()
        {
            // Arrange
            var t1 = new TimeOnly(12, 0);
            var t2 = new TimeOnly(0, 0);

            // Act
            var actual = TimeOnlyExtensions.IsBefore(t1, t2);

            // Assert
            actual.Should().BeFalse($"{t1} is not before {t2}");
        }

        [Fact]
        public void Should_return_false_when_equal()
        {
            // Arrange
            var t1 = new TimeOnly(12, 0);
            var t2 = new TimeOnly(12, 0);

            // Act
            var actual = TimeOnlyExtensions.IsBefore(t1, t2);

            // Assert
            actual.Should().BeFalse($"{t1} is equal to (thus not before) {t2}");
        }

        [Fact]
        public void Should_return_false_when_inside_grace_period()
        {
            // Arrange
            var t1 = new TimeOnly(11, 59, 59, 999);
            var t2 = new TimeOnly(12, 0, 0, 0);

            // Act
            var actual = TimeOnlyExtensions.IsBefore(t1, t2);

            // Assert
            actual.Should().BeFalse($"{t1} is before {t2} but within grace period");
        }
    }
}
