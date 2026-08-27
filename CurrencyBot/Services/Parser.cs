namespace CurrencyBot.Services;

public class Parser
{
    public static List<Token> ToRpn(List<Token> tokens)
    {
        var result = new List<Token>();
        var stack = new Stack<Token>();

        foreach (var token in tokens)
        {
            switch (token.Type)
            {
                case TokenType.Number:
                    result.Add(token);
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
                        result.Add(stack.Pop());
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
                            result.Add(stack.Pop());
                        }
                    }

                    break;
            }
        }

        foreach (var token in stack)
        {
            result.Add(token);
        }

        return result;
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