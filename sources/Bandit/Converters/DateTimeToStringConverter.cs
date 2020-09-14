using System;
using System.Globalization;
using System.Windows.Data;

namespace Bandit.Converters
{
    /// <summary>
    /// DateTime 객체를 지정된 형식의 문자열로 만들어주는 Converter 입니다.
    /// </summary>
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class DateTimeToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
            {
                throw new InvalidOperationException("The target must be a string!");
            }

            int hour24 = ((DateTime)value).Hour;

            string hour12 = (hour24 > 12) ? $"PM {hour24 - 12, 2:D2}" : $"AM {hour24, 2:D2}";

            return $"{hour24, 2:D2}:{((DateTime)value).Minute,2:D2} ({hour12}:{((DateTime)value).Minute,2:D2})";
        }

        public object ConvertBack(object value, Type targetType, object parameter,CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
