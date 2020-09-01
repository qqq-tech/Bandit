using System;
using System.Windows.Data;

namespace Bandit.Converters
{
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class DateTimeToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a string!");

            int militaryHour = ((DateTime)value).Hour;

            string dailyHour = (militaryHour > 12) ? $"오후 {militaryHour - 12, 2:D2}" : $"오전 {militaryHour, 2:D2}";

            return $"{militaryHour, 2:D2}:{((DateTime)value).Minute,2:D2} ({dailyHour}:{((DateTime)value).Minute,2:D2})";
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
