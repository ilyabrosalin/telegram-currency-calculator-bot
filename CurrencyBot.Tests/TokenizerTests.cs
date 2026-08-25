using CurrencyBot.Services;
using Xunit;

namespace CurrencyBot.Tests;

public class TokenizerTests
{
    [Fact]
    public void Tokenize_SimpleExpressionWithoutSpaces_ReturnsCorrectTokens()
    {
        var tokenizer = new Tokenizer();
        var tokens = Tokenizer.Tokenize("5+3");

        Assert.Equal(4, tokens.Count);
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal("5", tokens[0].Value);
        Assert.Equal(TokenType.Plus, tokens[1].Type);
        Assert.Equal(TokenType.Number, tokens[2].Type);
        Assert.Equal("3", tokens[2].Value);
    }

    [Fact]
    public void Tokenize_SameExpressionWithSpaces_ReturnsIdenticalTokens()
    {
        var tokenizer = new Tokenizer();

        var withoutSpaces = Tokenizer.Tokenize("5+3");
        var withSpaces = Tokenizer.Tokenize("5 + 3");

        Assert.Equal(withoutSpaces.Count, withSpaces.Count);
        for (int i = 0; i < withoutSpaces.Count; i++)
        {
            Assert.Equal(withoutSpaces[i].Type, withSpaces[i].Type);
            Assert.Equal(withoutSpaces[i].Value, withSpaces[i].Value);
        }
    }

    [Fact]
    public void Tokenize_MultiDigitNumber_ParsesAsSingleToken()
    {
        var tokenizer = new Tokenizer();
        var tokens = Tokenizer.Tokenize("123+45");

        Assert.Equal("123", tokens[0].Value);
        Assert.Equal("45", tokens[2].Value);
    }

    [Fact]
    public void Tokenize_DecimalNumber_ParsesAsSingleToken()
    {
        var tokenizer = new Tokenizer();
        var tokens = Tokenizer.Tokenize("3.14+1");

        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal("3.14", tokens[0].Value);
    }

    [Fact]
    public void Tokenize_InvalidCharacter_ThrowsFormatException()
    {
        var tokenizer = new Tokenizer();

        Assert.Throws<FormatException>(() => Tokenizer.Tokenize("5 & 3"));
    }

    [Fact]
    public void Tokenize_EmptyString_ReturnsEmptyTokenList()
    {
        var tokenizer = new Tokenizer();
        var tokens = Tokenizer.Tokenize("");

        Assert.Empty(tokens);
    }
}