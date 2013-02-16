using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Winfy.Core.Extensions {
    public static class StringExtensions {
        public static string ToSHA1(this string str) {
            var sb = new StringBuilder();
            SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(str)).ToList().ForEach(b => sb.Append(b.ToString("x2")));
            return sb.ToString();
        }
    }
}
