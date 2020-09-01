using Bandit.Entities;
using MaterialDesignColors;
using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Bandit.Converters
{
    [ValueConversion(typeof(TaskState), typeof(Brush))]
    public class TaskStateToSolidBrushConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(Brush))
                throw new InvalidOperationException("The target must be a Brush!");

            if ((TaskState)value == TaskState.Idle)
            {
                Color color = SwatchHelper.Lookup[MaterialDesignColor.Grey300];
                return new SolidColorBrush(color);
            }
            else if ((TaskState)value == TaskState.Loading)
            {
                Color color = SwatchHelper.Lookup[MaterialDesignColor.Blue400];
                return new SolidColorBrush(color);
            }
            else if ((TaskState)value == TaskState.Running)
            {
                Color color = SwatchHelper.Lookup[MaterialDesignColor.Blue500];
                return new SolidColorBrush(color);
            }
            else if ((TaskState)value == TaskState.Stopping)
            {
                Color color = SwatchHelper.Lookup[MaterialDesignColor.DeepOrange500];
                return new SolidColorBrush(color);
            }
            else if ((TaskState)value == TaskState.WebProcessing)
            {
                Color color = SwatchHelper.Lookup[MaterialDesignColor.BlueA700];
                return new SolidColorBrush(color);
            }
            else
            {
                return "작업 상태를 불러오던 중 오류가 발생했습니다.";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
