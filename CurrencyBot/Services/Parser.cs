namespace CurrencyBot.Services;

public class Parser
{
    public static bool TryParseToRpn(List<Token> tokens, out List<Token> result, out string? error)
    {
        var tokenList = new List<Token>();
        var stack = new Stack<Token>();

        foreach (var token in tokens)
        {
            switch (token.Type)
            {
                case TokenType.Number:
                    tokenList.Add(token);
                    break;
                case TokenType.Plus:
                case TokenType.Minus:
                case TokenType.Multiply:
                case TokenType.Divide:
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
                    break;
                case TokenType.LeftParen:
                    stack.Push(token);
                    break;
                case TokenType.RightParen:
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

                    break;
            }
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