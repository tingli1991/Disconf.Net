using System.Security.Cryptography;
using System.Text;

namespace Disconf.Net.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HashAlgorithmHelper<T> where T : HashAlgorithm, new()
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalText"></param>
        /// <param name="upper"></param>
        /// <returns></returns>
        public static string ComputeHash(string originalText, bool upper = true)
        {
            return ComputeHash(Encoding.UTF8.GetBytes(originalText), upper);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="upper"></param>
        /// <returns></returns>
        public static string ComputeHash(byte[] data, bool upper = true)
        {
            using (T algorithm = new T())
            {
                var ret = algorithm.ComputeHash(data);
                StringBuilder tmp = new StringBuilder();
                for (int i = 0; i < ret.Length; i++)
                {
                    tmp.Append(ret[i].ToString(upper ? "X2" : "x2"));
                }
                return tmp.ToString();
            }
        }
    }
}