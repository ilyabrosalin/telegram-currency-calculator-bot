using CurrencyBot.Services;

namespace CurrencyBot.Tests;

public class TokenizerTests
{
    [Fact]
    // "5+3" -> [Number "5", Plus, Number "3"]
    public void Tokenize_SimpleExpressionWithoutSpaces_ReturnsCorrectTokens()
    {
        var text = "5+3";
        var success = Tokenizer.TryTokenize(text, out var result, out var error);

        Assert.True(success);
        Assert.Null(error);
        Assert.Equal(3, result.Count);
        Assert.Equal(TokenType.Number, result[0].Type);
        Assert.Equal("5", result[0].Value);
        Assert.Equal(TokenType.Plus, result[1].Type);
        Assert.Equal(TokenType.Number, result[2].Type);
        Assert.Equal("3", result[2].Value);
    }

    [Fact]
    // "5+3" и "5 + 3" -> одинаковый список токенов (пробелы игнорируются)
    public void Tokenize_SameExpressionWithSpaces_ReturnsIdenticalTokens()
    {
        var text1 = "5+3";
        var text2 = "5 + 3";
        var success1 = Tokenizer.TryTokenize(text1, out var resultWithoutSpaces, out var error1);
        var success2 = Tokenizer.TryTokenize(text2, out var resultWithSpaces, out var error2);

        Assert.True(success1);
        Assert.True(success2);
        Assert.Equal(resultWithoutSpaces.Count, resultWithSpaces.Count);
        for (var i = 0; i < resultWithoutSpaces.Count; i++)
        {
            Assert.Equal(resultWithoutSpaces[i].Type, resultWithSpaces[i].Type);
            Assert.Equal(resultWithoutSpaces[i].Value, resultWithSpaces[i].Value);
        }
    }

    [Fact]
    // "123+45" -> [Number "123", Plus, Number "45"] (не по одной цифре)
    public void Tokenize_MultiDigitNumber_ParsesAsSingleToken()
    {
        var text = "123+45";
        var success = Tokenizer.TryTokenize(text, out var result, out var error);

        Assert.True(success);
        Assert.Equal("123", result[0].Value);
        Assert.Equal("45", result[2].Value);
    }

    [Fact]
    // "3.14+1" -> [Number "3.14", Plus, Number "1"] (точка не разбивает число)
    public void Tokenize_DecimalNumber_ParsesAsSingleToken()
    {
        var text = "3.14+1";
        var success = Tokenizer.TryTokenize(text, out var result, out var error);

        Assert.True(success);
        Assert.Equal(TokenType.Number, result[0].Type);
        Assert.Equal("3.14", result[0].Value);
    }

    [Fact]
    // "5 & 3" -> false, error: "Недопустимый символ '&'"
    public void Tokenize_InvalidCharacter_ReturnsFalseWithError()
    {
        var text = "5 & 3";
        var success = Tokenizer.TryTokenize(text, out var result, out var error);

        Assert.False(success);
        Assert.Equal("Недопустимый символ '&'", error);
    }

    [Fact]
    // "2 + 3 & 2 - 3" -> false, error: "Недопустимый символ '&'"
    // (останавливается на первом недопустимом символе, не токенизирует "2 - 3" дальше)
    public void Tokenize_InvalidCharacter_StopsAtFirstOccurrence()
    {
        var text = "2 + 3 & 2 - 3";
        var success = Tokenizer.TryTokenize(text, out var result, out var error);

        Assert.False(success);
        Assert.Equal("Недопустимый символ '&'", error);
    }

    [Fact]
    // "2 ^ 3" -> false, error: "Оператор '^' не поддерживается"
    // ('^' похож на оператор, но не реализован — сообщение должно отличаться
    // от "недопустимого символа")
    public void Tokenize_UnsupportedButRecognizedOperator_ReturnsSpecificError()
    {
        var text = "2 ^ 3";
        var success = Tokenizer.TryTokenize(text, out var result, out var error);

        Assert.False(success);
        Assert.Equal("Оператор '^' не поддерживается", error);
    }

    [Fact]
    // "3.5.6 + 1" -> false, error (некорректный формат числа, две точки подряд)
    public void Tokenize_NumberWithTwoDots_ReturnsFalseWithError()
    {
        var text = "3.5.6 + 1";
        var success = Tokenizer.TryTokenize(text, out var result, out var error);

        Assert.False(success);
        Assert.Equal("Некорректный формат числа", error);
    }

    [Fact]
    // "" -> false, error: "Пустой ввод"
    public void Tokenize_EmptyString_ReturnsFalseWithError()
    {
        var text = "";
        var success = Tokenizer.TryTokenize(text, out var result, out var error);

        Assert.False(success);
        Assert.Equal("Пустой ввод", error);
    }

    [Fact]
    // "   " -> false, error: "Пустой ввод" (строка из пробелов трактуется как пустой ввод)
    public void Tokenize_WhitespaceOnlyString_ReturnsFalseWithError()
    {
        var text = "   ";
        var success = Tokenizer.TryTokenize(text, out var result, out var error);

        Assert.False(success);
        Assert.Equal("Пустой ввод", error);
    }
}