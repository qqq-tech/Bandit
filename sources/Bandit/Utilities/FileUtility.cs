using System.IO;
using System.Text;

namespace Bandit.Utilities
{
    /// <summary>
    /// 파일 입출력과 관련된 기능을 제공하는 클래스입니다.
    /// </summary>
    internal static class FileUtility
    {
        /// <summary>
        /// 지정된 경로에 텍스트 파일을 작성합니다.
        /// </summary>
        /// <param name="filePath">텍스트 파일이 작성될 경로입니다.</param>
        /// <param name="text">작성될 텍스트입니다.</param>
        /// <param name="encoding">텍스트 파일을 작성하는데에 사용될 인코딩입니다.</param>
        internal static void WriteTextFile(string filePath, string text, Encoding encoding)
        {
            using (Stream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                StreamWriter objSaveFile = new StreamWriter(stream, encoding);
                objSaveFile.Write(text);
                objSaveFile.Close();
                objSaveFile.Dispose();
            }
        }

        /// <summary>
        /// 지정된 텍스트 파일을 읽어옵니다.
        /// </summary>
        /// <param name="filePath">읽어올 텍스트 파일입니다.</param>
        /// <param name="encoding">텍스트 파일의 인코딩입니다.</param>
        /// <returns>텍스트 파일에서 읽어 온 문자열.</returns>
        internal static string ReadTextFile(string filePath, Encoding encoding)
        {
            string temp = string.Empty;
            using (StreamReader objReadFile = new StreamReader(filePath, encoding))
            {
                temp = objReadFile.ReadToEnd();
                objReadFile.Close();
                objReadFile.Dispose();
            }
            return temp;
        }
    }
}
