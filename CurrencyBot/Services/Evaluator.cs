using System.Globalization;

namespace CurrencyBot.Services;

public static class Evaluator
{
    public static bool TryEvaluate(IReadOnlyList<Token> rpn, out decimal result, out string? error)
    {
        var stack = new Stack<decimal>();

        foreach (var token in rpn)
        {
            switch (token.Type)
            {
                case TokenType.Number:
                    stack.Push(decimal.Parse(token.Value, CultureInfo.InvariantCulture));
                    break;
                case TokenType.Plus:
                case TokenType.Minus:
                case TokenType.Divide:
                case TokenType.Multiply:
                {
                    var numberRight = stack.Pop();
                    var numberLeft = stack.Pop();

                    if (token.Type == TokenType.Divide && numberRight == 0)
                    {
                        result = default;
                        error = "Деление на ноль";
                        return false;
                    }

                    var operationResult = token.Type switch
                    {
                        TokenType.Plus => numberLeft + numberRight,
                        TokenType.Minus => numberLeft - numberRight,
                        TokenType.Multiply => numberLeft * numberRight,
                        TokenType.Divide => numberLeft / numberRight,
                        _ => throw new InvalidOperationException("Неподдерживаемый оператор")
                    };

                    stack.Push(operationResult);
                    break;
                }
            }
        }

        result = stack.Pop();
        error = null;
        return true;
    }
}