using System;
using System.Globalization;
using System.Windows.Data;

namespace Bandit.Converters
{
    /// <summary>
    /// DateTime 개체의 Nullable 여부를 변경시켜주는 Converter 입니다.
    /// </summary>
    [ValueConversion(typeof(DateTime), typeof(Nullable<DateTime>))]
    public class NullableDateTimeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Nullable<DateTime>))
            {
                throw new InvalidOperationException("The target must be a Nullable<DateTime>!");
            }

            return (Nullable<DateTime>)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(DateTime))
            {
                throw new InvalidOperationException("The target must be a DateTime!");
            }

            return (DateTime)value;
        }

        #endregion
    }
}
