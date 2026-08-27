using CurrencyBot.Services;

public class ParserTests
{
    private static Token Num(string v) => new Token { Type = TokenType.Number, Value = v };
    private static Token Op(TokenType t, string v) => new Token { Type = t, Value = v };
    
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
        var tokens = new List<Token> { Num("5") };

        var rpn = Parser.ToRpn(tokens);

        Assert.Equal(new List<string> { "5" }, Values(rpn));
    }

    [Fact]
    public void SimpleAddition_ReturnsOperandsThenOperator()
    {
        // "5 + 3" -> "5 3 +"
        var tokens = new List<Token>
        {
            Num("5"), Op(TokenType.Plus, "+"), Num("3")
        };

        var rpn = Parser.ToRpn(tokens);

        Assert.Equal(new List<string> { "5", "3", "+" }, Values(rpn));
    }

    [Fact]
    public void MultiplicationHasHigherPrecedenceThanAddition()
    {
        // "5 + 3 * 2" -> "5 3 2 * +"
        var tokens = new List<Token>
        {
            Num("5"), Op(TokenType.Plus, "+"), Num("3"), Op(TokenType.Multiply, "*"), Num("2")
        };

        var rpn = Parser.ToRpn(tokens);

        Assert.Equal(new List<string> { "5", "3", "2", "*", "+" }, Values(rpn));
    }

    [Fact]
    public void Parentheses_OverridePrecedence()
    {
        // "(5 + 3) * 2" -> "5 3 + 2 *"
        var tokens = new List<Token>
        {
            Op(TokenType.LeftParen, "("), Num("5"), Op(TokenType.Plus, "+"), Num("3"), Op(TokenType.RightParen, ")"),
            Op(TokenType.Multiply, "*"), Num("2")
        };

        var rpn = Parser.ToRpn(tokens);

        Assert.Equal(new List<string> { "5", "3", "+", "2", "*" }, Values(rpn));
    }

    [Fact]
    public void SamePrecedenceOperators_AreLeftAssociative()
    {
        // "10 - 5 - 2" -> "10 5 - 2 -"
        var tokens = new List<Token>
        {
            Num("10"), Op(TokenType.Minus, "-"), Num("5"), Op(TokenType.Minus, "-"), Num("2")
        };

        var rpn = Parser.ToRpn(tokens);

        Assert.Equal(new List<string> { "10", "5", "-", "2", "-" }, Values(rpn));
    }
}