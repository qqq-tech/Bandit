using Bandit.Entities;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Bandit.Converters
{
    /// <summary>
    /// 작업 상태에 따른 설명 문자열을 반환하는 Converter 입니다.
    /// </summary>
    [ValueConversion(typeof(TaskState), typeof(string))]
    public class TaskStateToDescriptionConverter : IValueConverter
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
                return "애플리케이션이 유휴 상태입니다. 작업을 시작하세요!";
            }
            else if ((TaskState)value == TaskState.Loading)
            {
                return "애플리케이션이 필요한 정보를 불러오는 중입니다!";
            }
            else if ((TaskState)value == TaskState.Running)
            {
                return "현재 애플리케이션이 작업을 처리하는 중입니다!";
            }
            else if ((TaskState)value == TaskState.Stopping)
            {
                return "작업이 중지되었습니다!";
            }
            else if ((TaskState)value == TaskState.WebProcessing)
            {
                return "웹 작업을 처리하는 중입니다!";
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
