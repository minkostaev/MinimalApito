namespace Apito.Models.TheMachine;

using System.Globalization;

public class Culture
{
    public Culture()
    {
        try
        {
            CultureName = CultureInfo.CurrentCulture.Name;
            EnglishName = CultureInfo.CurrentCulture.EnglishName;
            DecimalSeparator = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
        }
        catch (Exception) { }
    }

    public string? CultureName { get; private set; }
    public string? EnglishName { get; private set; }
    public string? DecimalSeparator { get; private set; }
}