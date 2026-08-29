namespace CurrencyBot.Services;

public class Parser
{
    public static bool TryParseToRpn(List<Token> tokens, out List<Token> result, out string? error)
    {
        var tokenList = new List<Token>();
        var stack = new Stack<Token>();

        var expectOperand = true;
        var expectOperatorOrEnd = false;
        var parenDepth = 0;
        Token? previousToken = null;

        foreach (var token in tokens)
        {
            switch (token.Type)
            {
                case TokenType.Number:
                    if (!expectOperand)
                    {
                        result = default;
                        error = "Пропущен оператор между операндами";
                        return false;
                    }

                    tokenList.Add(token);
                    expectOperand = false;
                    expectOperatorOrEnd = true;

                    break;
                case TokenType.Plus:
                case TokenType.Minus:
                case TokenType.Multiply:
                case TokenType.Divide:
                    if (!expectOperatorOrEnd)
                    {
                        result = default;
                        error = "Ожидался операнд, получен оператор";
                        return false;
                    }

                    if (stack.Count == 0)
                    {
                        stack.Push(token);
                    }
                    else if (GetPriority(stack.Peek().Type) >= GetPriority(token.Type))
                    {
                        tokenList.Add(stack.Pop());
                        stack.Push(token);
                    }
                    else if (GetPriority(stack.Peek().Type) <= GetPriority(token.Type))
                    {
                        stack.Push(token);
                    }

                    previousToken = token;
                    expectOperand = true;
                    expectOperatorOrEnd = false;

                    break;
                case TokenType.LeftParen:
                    if (!expectOperand)
                    {
                        result = default;
                        error = "Пропущен оператор";
                        return false;
                    }

                    previousToken = token;
                    parenDepth++;
                    stack.Push(token);
                    break;
                case TokenType.RightParen:
                    if (!expectOperatorOrEnd)
                    {
                        result = default;
                        error = previousToken != null && previousToken.Type == TokenType.LeftParen
                            ? "Пустые скобки"
                            : "Ожидался операнд после оператора, выражение в скобках не завершено";
                        return false;
                    }


                    while (stack.Count > 0)
                    {
                        if (stack.Peek().Type == TokenType.LeftParen)
                        {
                            stack.Pop();
                        }
                        else
                        {
                            tokenList.Add(stack.Pop());
                        }
                    }

                    
                    parenDepth--;
                    break;
            }
        }

        if (expectOperand)
        {
            result = default;
            error = "Выражение не завершено, отсутствует операнд";
            return false;
        }

        if (parenDepth != 0)
        {
            result = default;
            error = "Непарные скобки";
            return false;
        }

        foreach (var token in stack)
        {
            tokenList.Add(token);
        }

        result = tokenList;
        error = null;
        return true;
    }

    private static int GetPriority(TokenType type)
    {
        return type switch
        {
            TokenType.Multiply or TokenType.Divide => 2,
            TokenType.Plus or TokenType.Minus => 1,
            _ => 0
        };
    }
}