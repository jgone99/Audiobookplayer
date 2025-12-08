using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Audiobookplayer.Converters
{
    public class MillisecondsToTimeSpanConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double milliseconds)
            {
                TimeSpan timeSpan = TimeSpan.FromMilliseconds(milliseconds);
                return timeSpan.ToString(@"hh\:mm\:ss");
            }
            return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
