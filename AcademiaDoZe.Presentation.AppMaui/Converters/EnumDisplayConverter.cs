
// Nicolas Vaz

using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace AcademiaDoZe.Presentation.AppMaui.Converters;

public sealed class EnumDisplayConverter : IValueConverter
{
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not Enum enumValue)
            return string.Empty;

        var member = enumValue.GetType()
            .GetMember(enumValue.ToString())
            .FirstOrDefault();

        var display = member?
            .GetCustomAttribute<DisplayAttribute>();

        return display?.GetName() ?? enumValue.ToString();
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
