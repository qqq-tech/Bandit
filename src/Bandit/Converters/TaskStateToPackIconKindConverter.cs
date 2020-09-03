using Bandit.Entities;
using MaterialDesignThemes.Wpf;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Bandit.Converters
{
    [ValueConversion(typeof(TaskState), typeof(PackIconKind))]
    public class TaskStateToPackIconKindConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(PackIconKind))
            {
                throw new InvalidOperationException("The target must be a PackIconKind!");
            }

            if (((TaskState)value) == TaskState.Idle)
            {
                return PackIconKind.Play;
            }
            else
            {
                return PackIconKind.Pause;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
