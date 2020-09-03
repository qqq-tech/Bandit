using System;

namespace Bandit.Entities
{
    /// <summary>
    /// 보고할 정보를 저장하는 클래스입니다.
    /// </summary>
    public class Report
    {
        /// <summary>
        /// 보고서의 종류를 지정합니다.
        /// </summary>
        public ReportType Type { get; set; }

        /// <summary>
        /// 보고서가 작성된 시간을 반환합니다.
        /// </summary>
        public DateTime DateTime { get; }

        /// <summary>
        /// 보고서의 내용을 지정합니다.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 새로운 보고서 인스턴스를 생성합니다.
        /// </summary>
        public Report() { }

        /// <summary>
        /// 새로운 보고서 인스턴스를 생성한 후 초기화합니다.
        /// </summary>
        /// <param name="type">보고서의 종류를 지정합니다.</param>
        /// <param name="content">보고서의 내용을 지정합니다.</param>
        public Report(ReportType type, string content)
        {
            Type = type;
            DateTime = DateTime.Now;
            Content = content;
        }
    }
}
