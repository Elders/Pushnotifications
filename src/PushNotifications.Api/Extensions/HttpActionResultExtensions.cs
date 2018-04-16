using System.Web.Http;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Globalization;

namespace PushNotifications.Api
{
    public static class StringExtensions
    {
        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static bool IsBase64String(this string s)
        {
            s = s.Trim();
            return (s.Length % 4 == 0) && System.Text.RegularExpressions.Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", System.Text.RegularExpressions.RegexOptions.None);
        }

        public static bool CanUrlTokenDecode(this string input)
        {
            if (input == null)
                return false;

            if (input.IsNumeric())
                return false;

            int len = input.Length;
            if (len < 1)
                return true;

            ///////////////////////////////////////////////////////////////////
            // Step 1: Calculate the number of padding chars to append to this string.
            //         The number of padding chars to append is stored in the last char of the string.
            int numPadChars = (int)input[len - 1] - (int)'0';
            if (numPadChars < 0 || numPadChars > 10)
                return false;


            ///////////////////////////////////////////////////////////////////
            // Step 2: Create array to store the chars (not including the last char)
            //          and the padding chars
            char[] base64Chars = new char[len - 1 + numPadChars];


            ////////////////////////////////////////////////////////
            // Step 3: Copy in the chars. Transform the "-" to "+", and "*" to "/"
            for (int iter = 0; iter < len - 1; iter++)
            {
                char c = input[iter];

                switch (c)
                {
                    case '-':
                        base64Chars[iter] = '+';
                        break;

                    case '_':
                        base64Chars[iter] = '/';
                        break;

                    default:
                        base64Chars[iter] = c;
                        break;
                }
            }

            ////////////////////////////////////////////////////////
            // Step 4: Add padding chars
            for (int iter = len - 1; iter < base64Chars.Length; iter++)
            {
                base64Chars[iter] = '=';
            }

            return new string(base64Chars).IsBase64String();
        }

        public static string UrlDecode(this string self)
        {
            var urlDecoded = System.Web.HttpServerUtility.UrlTokenDecode(self);
            var decodedString = System.Text.Encoding.ASCII.GetString(urlDecoded);

            return decodedString;
        }

        public static string UrlEncode(this string self)
        {
            var stringBytes = System.Text.Encoding.ASCII.GetBytes(self);
            var urlEncoded = System.Web.HttpServerUtility.UrlTokenEncode(stringBytes);

            return urlEncoded;
        }

        public static bool IsUrn(this string s)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(s, @"\b(urn):([a-z0-9][a-z0-9-]{0,31}):([a-z0-9()+,\-.=@;$_!:*'%\/?#]*[a-z0-9+=@$\/])", System.Text.RegularExpressions.RegexOptions.None);
        }

        public static bool IsNumeric(this string str)
        {
            double retNum;
            return Double.TryParse(str, NumberStyles.Number ^ NumberStyles.AllowLeadingSign ^ NumberStyles.AllowTrailingSign, NumberFormatInfo.InvariantInfo, out retNum);
        }
    }

    public static class HttpActionResultExtensions
    {
        public static IHttpActionResult SetLastModifiedHeader(this IHttpActionResult actionResult, DateTime lastModified)
        {
            var x = new LastModifiedHeaderActionResult(actionResult);
            x.AddLastModofied(lastModified);

            return x;
        }

        class LastModifiedHeaderActionResult : IHttpActionResult
        {
            readonly IHttpActionResult actionResult;

            DateTime lastModified;

            public LastModifiedHeaderActionResult(IHttpActionResult actionResult)
            {
                this.actionResult = actionResult;
            }

            public void AddLastModofied(DateTime lastModified)
            {
                this.lastModified = lastModified;
            }

            public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                HttpResponseMessage httpResponseMessage = await this.actionResult.ExecuteAsync(cancellationToken);

                if (lastModified != default(DateTime))
                    httpResponseMessage.Content.Headers.LastModified = lastModified;

                return httpResponseMessage;
            }
        }
    }
}
