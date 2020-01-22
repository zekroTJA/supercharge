using System;
using System.Collections.Generic;
using System.Text;

namespace RiotAPIAccessLayer.Random
{
    public class RandomString
    {
        private const string DEF_CHARSET = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public static string GetRandomString(int len, string charset)
        {
            var rng = new System.Random();
            var res = new char[len];
            var clen = charset.Length;

            for (var i = 0; i < len; i++)
                res[i] = charset[rng.Next(0, clen - 1)];

            return new string(res);
        }

        public static string GetRandomString(int len) =>
            GetRandomString(len, DEF_CHARSET);
    }
}
