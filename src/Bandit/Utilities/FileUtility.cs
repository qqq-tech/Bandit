using System.IO;
using System.Text;

namespace Bandit.Utilities
{
    internal static class FileUtility
    {
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
