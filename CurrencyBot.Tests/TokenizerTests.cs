using CurrencyBot.Services;

namespace CurrencyBot.Tests;

public class TokenizerTests
{
    [Fact]
    public void Tokenize_SimpleExpressionWithoutSpaces_ReturnsCorrectTokens()
    {
        var text = "5+3";
        var success = Tokenizer.TryTokenize(text, out var result, out var error);

        Assert.True(success);
        Assert.Equal(3, result.Count);
        Assert.Equal(TokenType.Number, result[0].Type);
        Assert.Equal("5", result[0].Value);
        Assert.Equal(TokenType.Plus, result[1].Type);
        Assert.Equal(TokenType.Number, result[2].Type);
        Assert.Equal("3", result[2].Value);
    }

    [Fact]
    public void Tokenize_SameExpressionWithSpaces_ReturnsIdenticalTokens()
    {
        var text1 = "5+3";
        var text2 = "5 + 3";
        var success1 = Tokenizer.TryTokenize(text1, out var resultWithoutSpaces, out var error1);
        var success2 = Tokenizer.TryTokenize(text2, out var resultWithSpaces, out var error2);
        
        Assert.True(success2);
        Assert.Equal(resultWithoutSpaces.Count, resultWithSpaces.Count);
        for (var i = 0; i < resultWithoutSpaces.Count; i++)
        {
            Assert.Equal(resultWithoutSpaces[i].Type, resultWithSpaces[i].Type);
            Assert.Equal(resultWithoutSpaces[i].Value, resultWithSpaces[i].Value);
        }
    }

    [Fact]
    public void Tokenize_MultiDigitNumber_ParsesAsSingleToken()
    {
        var text = "123+45";
        var success = Tokenizer.TryTokenize(text, out var result, out var error);

        Assert.Equal("123", result[0].Value);
        Assert.Equal("45", result[2].Value);
    }

    [Fact]
    public void Tokenize_DecimalNumber_ParsesAsSingleToken()
    {
        var text = "3.14+1";
        var success = Tokenizer.TryTokenize(text, out var result, out var error);

        Assert.Equal(TokenType.Number, result[0].Type);
        Assert.Equal("3.14", result[0].Value);
    }

    [Fact]
    public void Tokenize_InvalidCharacter_ThrowsFormatException()
    {
        var text = "5 & 3";
        var success = Tokenizer.TryTokenize(text, out var result, out var error);
        Assert.False(success);
        Assert.Equal("Недопустимый символ &", error);
    }

    [Fact]
    public void Tokenize_EmptyString_ReturnsEmptyTokenList()
    {
        var text = "";
        var success = Tokenizer.TryTokenize(text, out var result, out var error);
        Assert.Empty(result);
    }
}