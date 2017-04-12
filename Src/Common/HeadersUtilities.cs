﻿namespace Microsoft.ApplicationInsights.Common
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Generic functions that can be used to get and set Http headers.
    /// </summary>
    public static class HeadersUtilities
    {
        /// <summary>
        /// Get the key value from the provided HttpHeader value that is set up as a comma-separated list of key value pairs. Each key value pair is formatted like (key)=(value).
        /// </summary>
        /// <param name="headerValues">The header values that may contain key name/value pairs.</param>
        /// <param name="keyName">The name of the key value to find in the provided header values.</param>
        /// <returns>The first key value, if it is found. If it is not found, then null.</returns>
        public static string GetHeaderKeyValue(IEnumerable<string> headerValues, string keyName)
        {
            if (headerValues != null)
            {
                foreach (string keyNameValue in headerValues)
                {
                    string[] keyNameValueParts = keyNameValue.Trim().Split('=');
                    if (keyNameValueParts.Length == 2 && keyNameValueParts[0].Trim() == keyName)
                    {
                        return keyNameValueParts[1].Trim();
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Transform a header value array into a dictionary.
        /// </summary>
        /// <param name="headerValues">A list of header values.</param>
        /// <returns>Returns the dictionary formed by key-value pairs form an array.</returns>
        public static IDictionary<string, string> GetHeaderDictionary(IEnumerable<string> headerValues)
        {
            IDictionary<string, string> result = new Dictionary<string, string>();

            if (headerValues != null)
            {
                foreach (string keyNameValue in headerValues)
                {
                    string[] keyNameValueParts = keyNameValue.Trim().Split('=');
                    if (keyNameValueParts.Length == 2)
                    {
                        string keyName = keyNameValueParts[0].Trim();
                        if (!result.ContainsKey(keyName))
                        {
                            result.Add(keyName, keyNameValueParts[1].Trim());
                        }
                    }
                }
            }

            if (!result.Any())
            {
                result = null;
            }

            return result;
        }

        /// <summary>
        /// Given the provided list of header value strings, return a list of key name/value pairs
        /// with the provided keyName and keyValue. If the initial header value strings contains
        /// the key name, then the original key value should be replaced with the provided key
        /// value. If the initial header value strings don't contain the key name, then the key
        /// name/value pair should be added to the list and returned.
        /// </summary>
        /// <param name="headerValues">The existing header values that the key/value pair should be added to.</param>
        /// <param name="keyName">The name of the key to add.</param>
        /// <param name="keyValue">The value of the key to add.</param>
        /// <returns>The result of setting the provided key name/value pair into the provided headerValues.</returns>
        public static IEnumerable<string> UpdateHeaderWithKeyValue(IEnumerable<string> headerValues, string keyName, string keyValue)
        {
            string[] newHeaderKeyValue = new[] { string.Format(CultureInfo.InvariantCulture, "{0}={1}", keyName.Trim(), keyValue.Trim()) };
            return headerValues == null || !headerValues.Any()
                ? newHeaderKeyValue
                : headerValues
                    .Where(headerValue => !HeaderMatchesKey(headerValue, keyName))
                    .Concat(newHeaderKeyValue);
        }

        /// <summary>
        /// Check if the header contains the key, case-insensitive, ignore leading and trailing whitespaces.
        /// </summary>
        /// <param name="headerValue">A header value that might contains key value pair.</param>
        /// <param name="key">The key to match.</param>
        /// <returns>Return true when the key matches and return false with it doesn't.</returns>
        private static bool HeaderMatchesKey(string headerValue, string key)
        {
            int equalsSignIndex = headerValue.IndexOf('=');
            if (equalsSignIndex < 0)
            {
                return false;
            }

            // Skip leading whitespace
            int start;
            for (start = 0; start < equalsSignIndex; start++)
            {
                if (!char.IsWhiteSpace(headerValue[start]))
                {
                    break;
                }
            }

            if (string.CompareOrdinal(headerValue, start, key, 0, key.Length) != 0)
            {
                return false;
            }

            // Check trailing whitespace
            for (int i = start + key.Length; i < equalsSignIndex; i++)
            {
                if (!char.IsWhiteSpace(headerValue[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
