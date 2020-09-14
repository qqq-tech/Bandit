using Bandit.Entities;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Bandit.Converters
{
    /// <summary>
    /// 작업 상태에 따른 활성화 여부를 반환하는 Converter 입니다.
    /// </summary>
    [ValueConversion(typeof(TaskState), typeof(bool))]
    public class TaskStateToIsEnabledConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool))
            {
                throw new InvalidOperationException("The target must be a bool!");
            }

            if (((TaskState)value) == TaskState.Idle)
            {
                return true;
            }
            else if (((TaskState)value) == TaskState.Loading)
            {
                return false;
            }
            else if (((TaskState)value) == TaskState.Running)
            {
                return true;
            }
            else if (((TaskState)value) == TaskState.Stopping)
            {
                return true;
            }
            else if (((TaskState)value) == TaskState.WebProcessing)
            {
                return true;
            }
            else
            {
                return true;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}