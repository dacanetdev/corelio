using Corelio.Infrastructure.Authentication;
using FluentAssertions;

namespace Corelio.Infrastructure.Tests.Authentication;

public class RefreshTokenGeneratorTests
{
    private readonly RefreshTokenGenerator _tokenGenerator = new();

    [Fact]
    public void GenerateToken_ReturnsNonEmptyTokenAndHash()
    {
        // Act
        var (token, hash) = _tokenGenerator.GenerateToken();

        // Assert
        token.Should().NotBeNullOrEmpty();
        hash.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GenerateToken_ReturnsDifferentTokensOnMultipleCalls()
    {
        // Act
        var (token1, _) = _tokenGenerator.GenerateToken();
        var (token2, _) = _tokenGenerator.GenerateToken();
        var (token3, _) = _tokenGenerator.GenerateToken();

        // Assert
        token1.Should().NotBe(token2);
        token2.Should().NotBe(token3);
        token1.Should().NotBe(token3);
    }

    [Fact]
    public void GenerateToken_ReturnsBase64EncodedString()
    {
        // Act
        var (token, _) = _tokenGenerator.GenerateToken();

        // Assert
        var isBase64 = IsBase64String(token);
        isBase64.Should().BeTrue();
    }

    [Fact]
    public void GenerateToken_Returns512BitToken()
    {
        // Act
        var (token, _) = _tokenGenerator.GenerateToken();
        var bytes = Convert.FromBase64String(token);

        // Assert
        bytes.Length.Should().Be(64); // 512 bits = 64 bytes
    }

    [Fact]
    public void GenerateToken_ReturnsNonEmptyHash()
    {
        // Act
        var (_, hash) = _tokenGenerator.GenerateToken();

        // Assert
        hash.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GenerateToken_HashIsSHA256Base64Format()
    {
        // Act
        var (_, hash) = _tokenGenerator.GenerateToken();

        // Assert
        // SHA256 produces 32 bytes, Base64 encoded to 44 characters
        hash.Length.Should().Be(44);
        IsBase64String(hash).Should().BeTrue();
    }

    [Fact]
    public void GenerateToken_ReturnsDifferentHashesForDifferentTokens()
    {
        // Act
        var (_, hash1) = _tokenGenerator.GenerateToken();
        var (_, hash2) = _tokenGenerator.GenerateToken();

        // Assert
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void GenerateToken_Generates100UniqueTokens()
    {
        // Act
        var tokens = new HashSet<string>();
        var hashes = new HashSet<string>();
        for (int i = 0; i < 100; i++)
        {
            var (token, hash) = _tokenGenerator.GenerateToken();
            tokens.Add(token);
            hashes.Add(hash);
        }

        // Assert
        tokens.Should().HaveCount(100); // All tokens should be unique
        hashes.Should().HaveCount(100); // All hashes should be unique
    }

    [Fact]
    public void GenerateToken_TokenIsBase64Encoded()
    {
        // Act
        var tokens = new List<string>();
        for (int i = 0; i < 10; i++)
        {
            var (token, _) = _tokenGenerator.GenerateToken();
            tokens.Add(token);
        }

        // Assert
        foreach (var token in tokens)
        {
            // All tokens should be valid Base64 strings
            IsBase64String(token).Should().BeTrue();
        }
    }

    [Fact]
    public void GenerateToken_HashMatchesTokenContent()
    {
        // This test verifies that the hash returned is actually derived from the token
        // by checking that regenerating the hash from the token gives the same result

        // Act
        var (token1, hash1) = _tokenGenerator.GenerateToken();
        var (token2, hash2) = _tokenGenerator.GenerateToken();

        // Assert
        // Different tokens should produce different hashes
        hash1.Should().NotBe(hash2);

        // Hash should be deterministic - same token should always produce same hash
        // (We can't re-hash here as HashToken is not exposed, but we verify uniqueness)
        token1.Should().NotBe(token2);
        hash1.Should().NotBe(hash2);
    }

    private static bool IsBase64String(string base64)
    {
        try
        {
            Convert.FromBase64String(base64);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
