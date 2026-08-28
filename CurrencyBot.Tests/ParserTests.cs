using CurrencyBot.Services;

public class ParserTests
{
    private static List<string> Values(List<Token> tokens)
    {
        var result = new List<string>();
        foreach (var t in tokens) result.Add(t.Value);
        return result;
    }

    [Fact]
    public void SingleNumber_ReturnsSameNumber()
    {
        // "5" -> "5"
        Tokenizer.TryTokenize("5", out var tokens, out _);
        var success = Parser.TryParseToRpn(tokens, out var result, out var error);

        Assert.True(success);
        Assert.Equal(["5"], Values(result));
    }

    [Fact]
    public void SimpleAddition_ReturnsOperandsThenOperator()
    {
        // "5 + 3" -> "5 3 +"
        Tokenizer.TryTokenize("5 + 3", out var tokens, out _);
        var success = Parser.TryParseToRpn(tokens, out var result, out var error);

        Assert.True(success);
        Assert.Equal(["5", "3", "+"], Values(result));
    }

    [Fact]
    public void MultiplicationHasHigherPrecedenceThanAddition()
    {
        // "5 + 3 * 2" -> "5 3 2 * +"
        Tokenizer.TryTokenize("5 + 3 * 2", out var tokens, out _);
        var success = Parser.TryParseToRpn(tokens, out var result, out var error);

        Assert.True(success);
        Assert.Equal(["5", "3", "2", "*", "+"], Values(result));
    }

    [Fact]
    public void Parentheses_OverridePrecedence()
    {
        // "(5 + 3) * 2" -> "5 3 + 2 *"
        Tokenizer.TryTokenize("(5 + 3) * 2", out var tokens, out _);
        var success = Parser.TryParseToRpn(tokens, out var result, out var error);

        Assert.True(success);
        Assert.Equal(["5", "3", "+", "2", "*"], Values(result));
    }

    [Fact]
    public void SamePrecedenceOperators_AreLeftAssociative()
    {
        // "10 - 5 - 2" -> "10 5 - 2 -"
        Tokenizer.TryTokenize("10 - 5 - 2", out var tokens, out _);
        var success = Parser.TryParseToRpn(tokens, out var result, out var error);

        Assert.True(success);
        Assert.Equal(["10", "5", "-", "2", "-"], Values(result));
    }

    [Fact]
    public void Parse_TwoOperatorsInARow_ReturnsFalseWithError()
    {
        // "5++3"
        Tokenizer.TryTokenize("5++3", out var tokens, out _);
        var success = Parser.TryParseToRpn(tokens, out var rpn, out var error);

        Assert.False(success);
        Assert.NotNull(error);
    }

    [Fact]
    public void Parse_UnclosedParenthesis_ReturnsFalseWithError()
    {
        // "(5 + 3"
        Tokenizer.TryTokenize("(5 + 3", out var tokens, out _);
        var success = Parser.TryParseToRpn(tokens, out var rpn, out var error);

        Assert.False(success);
        Assert.NotNull(error);
    }

    [Fact]
    public void Parse_ExtraClosingParenthesis_ReturnsFalseWithError()
    {
        // "5 + 3)"
        Tokenizer.TryTokenize("5 + 3)", out var tokens, out _);
        var success = Parser.TryParseToRpn(tokens, out var rpn, out var error);

        Assert.False(success);
        Assert.NotNull(error);
    }

    [Fact]
    public void Parse_EmptyParentheses_ReturnsFalseWithError()
    {
        // "()"
        Tokenizer.TryTokenize("()", out var tokens, out _);
        var success = Parser.TryParseToRpn(tokens, out var rpn, out var error);

        Assert.False(success);
        Assert.NotNull(error);
    }

    [Fact]
    public void Parse_OperatorAtStart_ReturnsFalseWithError()
    {
        // "+5"
        Tokenizer.TryTokenize("+5", out var tokens, out _);
        var success = Parser.TryParseToRpn(tokens, out var rpn, out var error);

        Assert.False(success);
        Assert.NotNull(error);
    }

    [Fact]
    public void Parse_OperatorAtEnd_ReturnsFalseWithError()
    {
        // "5+"
        Tokenizer.TryTokenize("5+", out var tokens, out _);
        var success = Parser.TryParseToRpn(tokens, out var rpn, out var error);

        Assert.False(success);
        Assert.NotNull(error);
    }

    [Fact]
    public void Parse_OperatorBeforeClosingParenthesis_ReturnsFalseWithError()
    {
        // "(3+)"
        Tokenizer.TryTokenize("(3+)", out var tokens, out _);
        var success = Parser.TryParseToRpn(tokens, out var rpn, out var error);

        Assert.False(success);
        Assert.NotNull(error);
    }

    [Fact]
    public void Parse_TwoOperandsInARowWithoutOperator_ReturnsFalseWithError()
    {
        // "5 3"
        Tokenizer.TryTokenize("5 3", out var tokens, out _);
        var success = Parser.TryParseToRpn(tokens, out var rpn, out var error);

        Assert.False(success);
        Assert.NotNull(error);
    }

    [Fact]
    public void Parse_EmptyTokenList_ReturnsFalseWithError()
    {
        var tokens = new List<Token>();
        var success = Parser.TryParseToRpn(tokens, out var rpn, out var error);

        Assert.False(success);
        Assert.NotNull(error);
    }
}