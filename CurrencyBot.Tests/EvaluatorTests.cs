using CurrencyBot.Services;

public class EvaluatorTests
{
    private static Token Num(string v) => new Token(TokenType.Number, v);
    private static Token Op(TokenType t, string v) => new Token(t, v);

    [Fact]
    public void SingleNumber_ReturnsItsValue()
    {
        // RPN: "5" -> 5
        var rpn = new List<Token> { Num("5") };

        var success = Evaluator.TryEvaluate(rpn, out var result, out var error);

        Assert.True(success);
        Assert.Equal(5, result);
    }

    [Fact]
    public void Addition_ReturnsSum()
    {
        // RPN: "5 3 +" -> 8
        var rpn = new List<Token> { Num("5"), Num("3"), Op(TokenType.Plus, "+") };

        var success = Evaluator.TryEvaluate(rpn, out var result, out var error);

        Assert.True(success);
        Assert.Equal(8, result);
    }

    [Fact]
    public void Subtraction_RespectsOperandOrder()
    {
        // RPN: "5 3 -" -> 2 (не -2, важно не перепутать порядок операндов)
        var rpn = new List<Token> { Num("5"), Num("3"), Op(TokenType.Minus, "-") };

        var success = Evaluator.TryEvaluate(rpn, out var result, out var error);

        Assert.True(success);
        Assert.Equal(2, result);
    }

    [Fact]
    public void Division_ReturnsCorrectResult()
    {
        // RPN: "10 2 /" -> 5
        var rpn = new List<Token> { Num("10"), Num("2"), Op(TokenType.Divide, "/") };

        var success = Evaluator.TryEvaluate(rpn, out var result, out var error);

        Assert.True(success);
        Assert.Equal(5, result);
    }

    [Fact]
    public void ComplexExpression_CalculatesCorrectly()
    {
        // "(5 + 3) * 2" -> RPN: "5 3 + 2 *" -> 16
        var rpn = new List<Token>
        {
            Num("5"), Num("3"), Op(TokenType.Plus, "+"), Num("2"), Op(TokenType.Multiply, "*")
        };

        var success = Evaluator.TryEvaluate(rpn, out var result, out var error);

        Assert.True(success);
        Assert.Equal(16, result);
    }

    [Fact]
    public void DivisionByZero_ReturnsFalseWithError()
    {
        // RPN: "5 0 /" -> деление на ноль должно возвращать false и текст ошибки, а не бросать исключение
        var rpn = new List<Token> { Num("5"), Num("0"), Op(TokenType.Divide, "/") };

        var success = Evaluator.TryEvaluate(rpn, out var result, out var error);

        Assert.False(success);
        Assert.NotNull(error);
    }

    [Fact]
    public void AllFourOperators_CalculatesCorrectly()
    {
        // "5 + 3 * 2 - 4 / 2" -> RPN: "5 3 2 * + 4 2 / -" -> 5 + 6 - 2 = 9
        var rpn = new List<Token>
        {
            Num("5"), Num("3"), Num("2"), Op(TokenType.Multiply, "*"), Op(TokenType.Plus, "+"),
            Num("4"), Num("2"), Op(TokenType.Divide, "/"), Op(TokenType.Minus, "-")
        };

        var success = Evaluator.TryEvaluate(rpn, out var result, out var error);

        Assert.True(success);
        Assert.Equal(9, result);
    }

    [Fact]
    public void NestedParenthesesExpression_CalculatesCorrectly()
    {
        // "(5 + 3) * (10 - 4) / 2" -> RPN: "5 3 + 10 4 - * 2 /" -> 8 * 6 / 2 = 24
        var rpn = new List<Token>
        {
            Num("5"), Num("3"), Op(TokenType.Plus, "+"),
            Num("10"), Num("4"), Op(TokenType.Minus, "-"),
            Op(TokenType.Multiply, "*"),
            Num("2"), Op(TokenType.Divide, "/")
        };

        var success = Evaluator.TryEvaluate(rpn, out var result, out var error);

        Assert.True(success);
        Assert.Equal(24, result);
    }

    [Fact]
    public void DeeplyNestedExpression_CalculatesCorrectly()
    {
        // "((2 + 3) * (4 - 1)) / (5 - 2) + 7" -> RPN: "2 3 + 4 1 - * 5 2 - / 7 +" -> (5*3)/3 + 7 = 5 + 7 = 12
        var rpn = new List<Token>
        {
            Num("2"), Num("3"), Op(TokenType.Plus, "+"),
            Num("4"), Num("1"), Op(TokenType.Minus, "-"),
            Op(TokenType.Multiply, "*"),
            Num("5"), Num("2"), Op(TokenType.Minus, "-"),
            Op(TokenType.Divide, "/"),
            Num("7"), Op(TokenType.Plus, "+")
        };

        var success = Evaluator.TryEvaluate(rpn, out var result, out var error);

        Assert.True(success);
        Assert.Equal(12, result);
    }

    [Fact]
    public void ManyOperandsChainedWithSameOperator_CalculatesCorrectly()
    {
        // "1 + 2 + 3 + 4 + 5 + 6" -> RPN: "1 2 + 3 + 4 + 5 + 6 +" -> 21
        var rpn = new List<Token>
        {
            Num("1"), Num("2"), Op(TokenType.Plus, "+"),
            Num("3"), Op(TokenType.Plus, "+"),
            Num("4"), Op(TokenType.Plus, "+"),
            Num("5"), Op(TokenType.Plus, "+"),
            Num("6"), Op(TokenType.Plus, "+")
        };

        var success = Evaluator.TryEvaluate(rpn, out var result, out var error);

        Assert.True(success);
        Assert.Equal(21, result);
    }

    [Fact]
    public void DecimalNumbers_CalculateCorrectly()
    {
        // "2.5 + 3.7 * 2" -> RPN: "2.5 3.7 2 * +" -> 2.5 + 7.4 = 9.9
        var rpn = new List<Token>
        {
            Num("2.5"), Num("3.7"), Num("2"), Op(TokenType.Multiply, "*"), Op(TokenType.Plus, "+")
        };

        var success = Evaluator.TryEvaluate(rpn, out var result, out var error);

        Assert.True(success);
        Assert.Equal((decimal)9.9, result, 5);
    }

    [Fact]
    public void ExpressionResultingInNegativeNumber_CalculatesCorrectly()
    {
        // "3 - 10 * 2" -> RPN: "3 10 2 * -" -> 3 - 20 = -17
        var rpn = new List<Token>
        {
            Num("3"), Num("10"), Num("2"), Op(TokenType.Multiply, "*"), Op(TokenType.Minus, "-")
        };

        var success = Evaluator.TryEvaluate(rpn, out var result, out var error);

        Assert.True(success);
        Assert.Equal(-17, result);
    }

    [Fact]
    public void DivisionProducingFraction_CalculatesCorrectly()
    {
        // "1 / 4 + 1 / 2" -> RPN: "1 4 / 1 2 / +" -> 0.25 + 0.5 = 0.75
        var rpn = new List<Token>
        {
            Num("1"), Num("4"), Op(TokenType.Divide, "/"),
            Num("1"), Num("2"), Op(TokenType.Divide, "/"),
            Op(TokenType.Plus, "+")
        };

        var success = Evaluator.TryEvaluate(rpn, out var result, out var error);

        Assert.True(success);
        Assert.Equal((decimal)0.75, result, 5);
    }
}