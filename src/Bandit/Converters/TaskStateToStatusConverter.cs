using Bandit.Entities;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Bandit.Converters
{
    /// <summary>
    /// 작업 상태에 따른 상태 문자열을 반환하는 Converter 입니다.
    /// </summary>
    [ValueConversion(typeof(TaskState), typeof(string))]
    public class TaskStateToStatusConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
            {
                throw new InvalidOperationException("The target must be a string!");
            }

            if ((TaskState)value == TaskState.Idle)
            {
                return "여유롭게 휴식을 취하는 중...";
            }
            else if ((TaskState)value == TaskState.Loading)
            {
                return "열심히 데이터를 실어 나르는 중...";
            }
            else if ((TaskState)value == TaskState.Running)
            {
                return "열심히 작업하는 중...";
            }
            else if ((TaskState)value == TaskState.Stopping)
            {
                return "잠시 쉬는 중...";
            }
            else if ((TaskState)value == TaskState.WebProcessing)
            {
                return "웹 서핑을 하는 중...";
            }
            else
            {
                return "작업 상태를 불러오던 중 오류가 발생했습니다.";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
