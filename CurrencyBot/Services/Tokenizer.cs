namespace CurrencyBot.Services;

public static class Tokenizer
{
    private static readonly Dictionary<char, TokenType> TokenTypeMappings = new()
    {
        { '+', TokenType.Plus },
        { '-', TokenType.Minus },
        { '*', TokenType.Multiply },
        { '/', TokenType.Divide },
        { '(', TokenType.LeftParen },
        { ')', TokenType.RightParen },
    };

    public static List<Token> Tokenize(string text)
    {
        var tokens = new List<Token>();

        var index = 0;

        while (index < text.Length)
        {
            if (char.IsDigit(text[index]))
            {
                var number = ReadNumber(text, ref index);
                tokens.Add(new Token(TokenType.Number, number));
                continue;
            }

            if (TokenTypeMappings.TryGetValue(text[index], out var tokenType))
            {
                tokens.Add(new Token(tokenType, text[index].ToString()));
            }
            else
            {
                switch (text[index])
                {
                    case ' ':
                        break;
                    case '^':
                        throw new Exception(TokenizerErrors.UnsupportedPowerOperator);
                    default:
                        throw new Exception(TokenizerErrors.InvalidCharacter(text[index]));
                }
            }

            index++;
        }

        if (tokens.Count == 0)
        {
            throw new Exception(TokenizerErrors.EmptyInput);
        }

        return tokens;
    }

    private static string ReadNumber(string text, ref int index)
    {
        var start = index;
        var currentState = NumberParsingState.IntegerPart;

        while (index < text.Length)
        {
            if (char.IsDigit(text[index]))
            {
                index++;
                continue;
            }

            if (text[index] != '.' && text[index] != ',')
            {
                break;
            }

            if (currentState != NumberParsingState.IntegerPart || index + 1 >= text.Length ||
                !char.IsDigit(text[index + 1]))
            {
                throw new Exception(TokenizerErrors.InvalidNumberFormat);
            }

            currentState = NumberParsingState.FractionalPart;
            index++;
        }

        return text.Substring(start, index - start).Replace(',', '.');
    }

    private enum NumberParsingState
    {
        IntegerPart,
        FractionalPart
    }
}