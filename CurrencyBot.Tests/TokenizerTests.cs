using CurrencyBot.Services;

namespace CurrencyBot.Tests;

public class TokenizerTests
{
    [Fact]
    // "5+3" -> [Number "5", Plus, Number "3"]
    public void Tokenize_SimpleExpressionWithoutSpaces_ReturnsCorrectTokens()
    {
        var tokens = Tokenizer.Tokenize("5+3");

        Assert.Equal(3, tokens.Count);
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal("5", tokens[0].Value);
        Assert.Equal(TokenType.Plus, tokens[1].Type);
        Assert.Equal(TokenType.Number, tokens[2].Type);
        Assert.Equal("3", tokens[2].Value);
    }

    [Fact]
    // "5+3" и "5 + 3" -> одинаковый список токенов (пробелы игнорируются)
    public void Tokenize_SameExpressionWithSpaces_ReturnsIdenticalTokens()
    {
        var tokens1 = Tokenizer.Tokenize("5+3");
        var tokens2 = Tokenizer.Tokenize("5 + 3");

        Assert.Equal(tokens1.Count, tokens2.Count);
        for (var i = 0; i < tokens1.Count; i++)
        {
            Assert.Equal(tokens1[i].Type, tokens2[i].Type);
            Assert.Equal(tokens1[i].Value, tokens2[i].Value);
        }
    }

    [Fact]
    // "123+45" -> [Number "123", Plus, Number "45"] (не по одной цифре)
    public void Tokenize_MultiDigitNumber_ParsesAsSingleToken()
    {
        var tokens = Tokenizer.Tokenize("123+45");

        Assert.Equal("123", tokens[0].Value);
        Assert.Equal("45", tokens[2].Value);
    }

    [Fact]
    // "3.14+1" -> [Number "3.14", Plus, Number "1"] (точка не разбивает число)
    public void Tokenize_DecimalNumber_ParsesAsSingleToken()
    {
        var tokens = Tokenizer.Tokenize("3.14+1");

        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal("3.14", tokens[0].Value);
    }

    [Fact]
    // "5 & 3" -> исключение: "Недопустимый символ '&'"
    public void Tokenize_InvalidCharacter_ReturnsFalseWithError()
    {
        var text = "5 & 3";

        var ex = Assert.Throws<Exception>(() => Tokenizer.Tokenize(text));

        Assert.Equal("Недопустимый символ '&'", ex.Message);
    }

    [Fact]
    // "2 + 3 & 2 - 3" -> исключение: "Недопустимый символ '&'"
    // (останавливается на первом недопустимом символе, не токенизирует "2 - 3" дальше)
    public void Tokenize_InvalidCharacter_StopsAtFirstOccurrence()
    {
        var text = "2 + 3 & 2 - 3";

        var ex = Assert.Throws<Exception>(() => Tokenizer.Tokenize(text));

        Assert.Equal("Недопустимый символ '&'", ex.Message);
    }

    [Fact]
    // "2 ^ 3" -> исключение: "Оператор '^' не поддерживается"
    // ('^' похож на оператор, но не реализован — сообщение должно отличаться
    // от "недопустимого символа")
    public void Tokenize_UnsupportedButRecognizedOperator_ReturnsSpecificError()
    {
        var text = "2 ^ 3";

        var ex = Assert.Throws<Exception>(() => Tokenizer.Tokenize(text));

        Assert.Equal("Оператор '^' не поддерживается", ex.Message);
    }

    [Fact]
    // "3.5.6 + 1" -> исключение: "Некорректный формат числа" (две точки подряд)
    public void Tokenize_NumberWithTwoDots_ReturnsFalseWithError()
    {
        var text = "3.5.6 + 1";

        var ex = Assert.Throws<Exception>(() => Tokenizer.Tokenize(text));

        Assert.Equal("Некорректный формат числа", ex.Message);
    }

    [Fact]
    // "" -> исключение: "Пустой ввод"
    public void Tokenize_EmptyString_ReturnsFalseWithError()
    {
        var text = "";

        var ex = Assert.Throws<Exception>(() => Tokenizer.Tokenize(text));

        Assert.Equal("Пустой ввод", ex.Message);
    }

    [Fact]
    // "   " -> исключение: "Пустой ввод" (строка из пробелов трактуется как пустой ввод)
    public void Tokenize_WhitespaceOnlyString_ReturnsFalseWithError()
    {
        var text = "   ";

        var ex = Assert.Throws<Exception>(() => Tokenizer.Tokenize(text));

        Assert.Equal("Пустой ввод", ex.Message);
    }
}