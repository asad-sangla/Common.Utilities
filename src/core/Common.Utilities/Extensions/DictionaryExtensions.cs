using System.Text;

namespace Common.Utilities.Extensions;

public static class DictionaryExtensions
{
    public static string ToCommaSeparatedString(this IDictionary<string, string>? dictionary)
    {
        if (dictionary == null || !dictionary.Any())
        {
            return string.Empty;
        }

        var stringBuilder = new StringBuilder();
        foreach (var kvp in dictionary)
        {
            stringBuilder.Append($"{kvp.Key}: {kvp.Value}, ");
        }

        // Remove the trailing comma and space
        if (stringBuilder.Length > 2)
        {
            stringBuilder.Length -= 2;
        }

        return stringBuilder.ToString();
    }
}