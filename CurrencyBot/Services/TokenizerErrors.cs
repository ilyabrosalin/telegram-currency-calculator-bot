namespace CurrencyBot.Services;

public static class TokenizerErrors
{
    public const string InvalidNumberFormat = "Некорректный формат числа";
    public const string EmptyInput = "Пустой ввод";
    public const string UnsupportedPowerOperator = "Оператор '^' не поддерживается";

    public static string InvalidCharacter(char character) => $"Недопустимый символ '{character}'";
}