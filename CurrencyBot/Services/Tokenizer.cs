namespace CurrencyBot.Services;

public class Tokenizer
{
    public static bool TryTokenize(string text, out List<Token> result, out string? error)
    {
        var tokens = new List<Token>();

        var index = 0;

        while (index < text.Length)
        {
            if (char.IsDigit(text[index]))
            {
                var start = index;
                var hasSeparator = false;

                while (index < text.Length)
                {
                    if (char.IsDigit(text[index]))
                    {
                        index++;
                    }
                    else if ((text[index] == '.' || text[index] == ',') && !hasSeparator &&
                             (index + 1 < text.Length && char.IsDigit(text[index + 1])))
                    {
                        hasSeparator = true;
                        index++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (index < text.Length && (text[index] == '.' || text[index] == ','))
                {
                    result = default;
                    error = "Некорректный формат числа";
                    return false;
                }

                var number = text.Substring(start, index - start).Replace(',', '.');

                tokens.Add(new Token { Type = TokenType.Number, Value = number });
                continue;
            }

            switch (text[index])
            {
                case '+':
                    tokens.Add(new Token { Type = TokenType.Plus, Value = text[index].ToString() });
                    break;
                case '-':
                    tokens.Add(new Token { Type = TokenType.Minus, Value = text[index].ToString() });
                    break;
                case '*':
                    tokens.Add(new Token { Type = TokenType.Multiply, Value = text[index].ToString() });
                    break;
                case '/':
                    tokens.Add(new Token { Type = TokenType.Divide, Value = text[index].ToString() });
                    break;
                case '(':
                    tokens.Add(new Token { Type = TokenType.LeftParen, Value = text[index].ToString() });
                    break;
                case ')':
                    tokens.Add(new Token { Type = TokenType.RightParen, Value = text[index].ToString() });
                    break;
                case ' ':
                    break;
                case '^':
                    result = default;
                    error = "Оператор '^' не поддерживается";
                    return false;
                default:
                    result = default;
                    error = $"Недопустимый символ '{text[index]}'";
                    return false;
            }

            index++;
        }

        if (tokens.Count == 0)
        {
            result = default;
            error = "Пустой ввод";
            return false;
        }

        result = tokens;
        error = null;
        return true;
    }
}

public enum TokenType
{
    Number,
    Plus,
    Minus,
    Multiply,
    Divide,
    LeftParen,
    RightParen
}

public class Token
{
    public TokenType Type { get; set; }
    public string Value { get; set; }
}