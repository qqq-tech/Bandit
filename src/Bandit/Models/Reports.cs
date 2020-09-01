using Bandit.Entities;
using System;
using System.Collections.ObjectModel;

namespace Bandit.Models
{
    /// <summary>
    /// 애플리케이션 내부 작업 보고서의 읽기 및 쓰기를 제공하는 클래스입니다.
    /// </summary>
    public class Reports
    {
        #region ::Singleton Supports::

        private static Reports _instance = null;

        /// <summary>
        /// 보고서 클래스의 싱글톤 인스턴스를 불러오거나 변경합니다.
        /// </summary>
        public static Reports Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Reports();
                    _instance.RecordedReports = new ObservableCollection<Report>();
                }

                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        #endregion

        #region ::Properties::

        /// <summary>
        /// 현재까지 기록된 보고서들의 목록입니다.
        /// </summary>
        public ObservableCollection<Report> RecordedReports { get; set; }

        #endregion

        #region ::Methods::

        /// <summary>
        /// 새로운 보고서를 추가합니다.
        /// </summary>
        /// <param name="type">보고서의 종류를 지정합니다.</param>
        /// <param name="content">보고서의 내용을 지정합니다.</param>
        public void AddReport(ReportType type, string content)
        {
            RecordedReports.Add(new Report(type, content));
        }

        /// <summary>
        /// 새로운 보고서를 추가합니다.
        /// </summary>
        /// <param name="type">보고서의 종류를 지정합니다.</param>
        /// <param name="content">보고서의 내용을 지정합니다.</param>
        public void AddReportWithDispatcher(ReportType type, string content)
        {
            App.Current.Dispatcher.Invoke(delegate // <--- HERE
            {
                RecordedReports.Add(new Report(type, content));
            });
        }

        /// <summary>
        /// 현재까지 기록된 모든 보고서를 삭제합니다.
        /// </summary>
        public void ResetReports()
        {
            RecordedReports.Clear();
        }

        #endregion
    }
}
