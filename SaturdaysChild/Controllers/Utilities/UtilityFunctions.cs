using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace SaturdaysChild.Controllers.Utilities
{
    public class UtilityFunctions
    {
        /// <summary>
        ///  
        /// </summary>
        /// <returns></returns>
        public string assignInternalId()
        {
            var chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var charSet = new HashSet<char>(chars).ToArray();
            using (var rng = new RNGCryptoServiceProvider())
            {
                var internalId = new StringBuilder();
                var buffer = new byte[128];
                while (internalId.Length <= 10)
                {
                    rng.GetBytes(buffer);
                    for (var a = 0; a < buffer.Length && internalId.Length <= 10; a++)
                    {
                        if (chars.Contains(buffer[a].ToString())) internalId.Append(buffer[a]);
                    }
                }
                return internalId.ToString();
            }
        }

        public int getHoursFromViewModel(int hours, int minutes)
        {
            return (hours * 60) + minutes;
        }
    }
}