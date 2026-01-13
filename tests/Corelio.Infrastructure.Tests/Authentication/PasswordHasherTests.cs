using Corelio.Infrastructure.Authentication;
using FluentAssertions;

namespace Corelio.Infrastructure.Tests.Authentication;

public class PasswordHasherTests
{
    private readonly PasswordHasher _passwordHasher = new();

    [Fact]
    public void HashPassword_ReturnsValidBCryptHash()
    {
        // Arrange
        var password = "SecurePassword123!";

        // Act
        var hash = _passwordHasher.HashPassword(password);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        hash.Should().StartWith("$2a$"); // BCrypt format
        hash.Length.Should().Be(60); // BCrypt hash length
    }

    [Fact]
    public void HashPassword_WithSamePassword_ReturnsDifferentHashes()
    {
        // Arrange
        var password = "SecurePassword123!";

        // Act
        var hash1 = _passwordHasher.HashPassword(password);
        var hash2 = _passwordHasher.HashPassword(password);

        // Assert
        hash1.Should().NotBe(hash2); // Due to different salts
    }

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ReturnsTrue()
    {
        // Arrange
        var password = "SecurePassword123!";
        var hash = _passwordHasher.HashPassword(password);

        // Act
        var result = _passwordHasher.VerifyPassword(password, hash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_WithIncorrectPassword_ReturnsFalse()
    {
        // Arrange
        var password = "SecurePassword123!";
        var wrongPassword = "WrongPassword456!";
        var hash = _passwordHasher.HashPassword(password);

        // Act
        var result = _passwordHasher.VerifyPassword(wrongPassword, hash);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("password")]
    [InlineData("12345678")]
    [InlineData("Password123!")]
    [InlineData("VeryLongPasswordWith!@#$%^&*()SpecialCharacters123456789")]
    [InlineData("")]
    public void VerifyPassword_WithVariousPasswords_WorksCorrectly(string password)
    {
        // Arrange
        var hash = _passwordHasher.HashPassword(password);

        // Act
        var correctResult = _passwordHasher.VerifyPassword(password, hash);
        var incorrectResult = _passwordHasher.VerifyPassword(password + "wrong", hash);

        // Assert
        correctResult.Should().BeTrue();
        incorrectResult.Should().BeFalse();
    }

    [Fact]
    public void HashPassword_UsesCorrectWorkFactor()
    {
        // Arrange
        var password = "SecurePassword123!";

        // Act
        var hash = _passwordHasher.HashPassword(password);

        // Assert
        hash.Should().StartWith("$2a$12$"); // BCrypt with work factor 12
    }

    [Fact]
    public void HashPassword_WithUnicodeCharacters_WorksCorrectly()
    {
        // Arrange
        var password = "Contraseña123!@#$%^&*()ñáéíóú";

        // Act
        var hash = _passwordHasher.HashPassword(password);
        var result = _passwordHasher.VerifyPassword(password, hash);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_WithEmptyHash_ThrowsException()
    {
        // Arrange
        var password = "SecurePassword123!";
        var emptyHash = string.Empty;

        // Act & Assert
        var act = () => _passwordHasher.VerifyPassword(password, emptyHash);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void VerifyPassword_WithInvalidHash_ThrowsException()
    {
        // Arrange
        var password = "SecurePassword123!";
        var invalidHash = "not-a-valid-bcrypt-hash";

        // Act & Assert
        var act = () => _passwordHasher.VerifyPassword(password, invalidHash);
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void HashPassword_IsConsistentAcrossMultipleVerifications()
    {
        // Arrange
        var password = "SecurePassword123!";
        var hash = _passwordHasher.HashPassword(password);

        // Act & Assert
        for (int i = 0; i < 10; i++)
        {
            var result = _passwordHasher.VerifyPassword(password, hash);
            result.Should().BeTrue();
        }
    }

    [Fact]
    public void HashPassword_WithMaxLengthPassword_WorksCorrectly()
    {
        // Arrange
        var password = new string('a', 1000); // Very long password

        // Act
        var hash = _passwordHasher.HashPassword(password);
        var result = _passwordHasher.VerifyPassword(password, hash);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_IsCaseSensitive()
    {
        // Arrange
        var password = "SecurePassword123!";
        var hash = _passwordHasher.HashPassword(password);

        // Act
        var correctCaseResult = _passwordHasher.VerifyPassword("SecurePassword123!", hash);
        var incorrectCaseResult = _passwordHasher.VerifyPassword("securepassword123!", hash);

        // Assert
        correctCaseResult.Should().BeTrue();
        incorrectCaseResult.Should().BeFalse();
    }
}
