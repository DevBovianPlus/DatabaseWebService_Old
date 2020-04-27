using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Web;

namespace DatabaseWebService.Common
{
    public static class DataTypesHelper
    {
        public static bool IsNumeric(this string text)
        {
            double test;
            return double.TryParse(text, out test);
        }

        public static string Parse(string sTrim)
        {
            return String.IsNullOrEmpty(sTrim) ? "" : sTrim.Trim();
        }

        public static int ParseInt(object param)
        {
            int num = 0;

            if (param != null)
            {
                int.TryParse(param.ToString(), out num);

                if (num < 0)
                    num = 0;
            }

            return num;
        }

        public static Nullable<int> ParseNullableInt(object param)
        {
            int num = 0;

            if (param != null)
            {
                int.TryParse(param.ToString(), out num);

                if (num < 0)
                    return null;

                return num;
            }
            else
                return null;
        }

        public static decimal ParseDecimal(object param)
        {
            decimal num = 0;
            if (param != null)
            {
                decimal.TryParse(param.ToString(), out num);

                if (num < 0)
                    num = 0;
            }

            return num;
        }

        public static double ParseDouble(object param)
        {
            double num = 0;
            if (param != null)
            {
                double.TryParse(param.ToString(), out num);

                if (num < 0)
                    num = 0;
            }

            return num;
        }

        public static int IsIntNull(int? num)
        {
            return num.HasValue ? num.Value : 0;
        }

        public static DateTime IsDateTimeNull(DateTime? date)
        {
            return date == null ? DateTime.MinValue : date.Value;
        }

        public static void LogThis(string message)
        {
            var directory = AppDomain.CurrentDomain.BaseDirectory;
            File.AppendAllText(directory + "log.txt", DateTime.Now + " " + message + Environment.NewLine);
        }

        public static void getError(Exception e, ref string errors)
        {
            if (e.GetType() != typeof(HttpException)) errors += " -------- " + e.ToString();
            if (e.InnerException != null) getError(e.InnerException, ref errors);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethodName()
        {
            var st = new StackTrace();
            var sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }

        public static string ConcatenateErrorIN_DB(string resource, string error, string methodName)
        {
            return resource + " in method : " + methodName + " Error : " + error;
        }

        public static String GetDateTimeMonthByNumber(int month)
        {
            switch (month)
            {
                case 1: return "Januar";
                case 2: return "Februar";
                case 3: return "Marec";
                case 4: return "April";
                case 5: return "Maj";
                case 6: return "Junij";
                case 7: return "Julij";
                case 8: return "Avgust";
                case 9: return "September";
                case 10: return "Oktober";
                case 11: return "November";
                case 12: return "December";
            }

            return "";
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
}