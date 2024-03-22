using System.Globalization;
using System;

public static class StringExtensions
{
    const string IsBase64StringPattern = @"^[a-zA-Z0-9\+/]*={0,3}$";

    public static string UberDecode(this string input)
    {
        if (input.CanBase64UrlTokenDecode())
            input = input.Base64UrlTokenDecode();
        else if (input.IsBase64String())
            input = input.Base64Decode();

        return input;
    }

    public static string Base64Encode(this string input)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(input);
        return System.Convert.ToBase64String(plainTextBytes);
    }

    public static string Base64Decode(this string input)
    {
        var base64EncodedBytes = System.Convert.FromBase64String(input);
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }

    public static bool IsBase64String(this string input)
    {
        input = input.Trim();
        return (input.Length % 4 == 0) && System.Text.RegularExpressions.Regex.IsMatch(input, IsBase64StringPattern, System.Text.RegularExpressions.RegexOptions.None);
    }

    public static bool CanBase64UrlTokenDecode(this string input)
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

    public static string Base64UrlTokenDecode(this string self)
    {
        byte[] urlDecoded = Base64UrlTokenDecodeToByteArray(self);
        var decodedString = System.Text.Encoding.UTF8.GetString(urlDecoded);

        return decodedString;
    }

    public static byte[] Base64UrlTokenDecodeToByteArray(string input)
    {
        if (input == null) throw new ArgumentNullException("input");

        int len = input.Length;
        if (len < 1)
            return new byte[0];

        ///////////////////////////////////////////////////////////////////
        // Step 1: Calculate the number of padding chars to append to this string.
        //         The number of padding chars to append is stored in the last char of the string.
        int numPadChars = (int)input[len - 1] - (int)'0';
        if (numPadChars < 0 || numPadChars > 10)
            return null;


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

        // Do the actual conversion
        return Convert.FromBase64CharArray(base64Chars, 0, base64Chars.Length);
    }


    public static bool IsNumeric(this string str)
    {
        double retNum;
        return Double.TryParse(str, NumberStyles.Number ^ NumberStyles.AllowLeadingSign ^ NumberStyles.AllowTrailingSign, NumberFormatInfo.InvariantInfo, out retNum);
    }

    public static string UrlEncode(this string str) => Uri.EscapeDataString(str);

    public static string UrlDecode(this string str) => Uri.UnescapeDataString(str);
}

