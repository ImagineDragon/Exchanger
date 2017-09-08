using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Exchanger.Helpers
{
    public abstract class MD5Helper
    {
        public static string GetHashString(string s)
        {
            var bytes = Encoding.Unicode.GetBytes(s);

            var CSP = new MD5CryptoServiceProvider();

            var byteHash = CSP.ComputeHash(bytes);

            var hash = byteHash.Aggregate(string.Empty, (current, b) => current + $"{b:x2}");

            return hash;
        }
    }
}