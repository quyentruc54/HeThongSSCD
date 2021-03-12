using Newtonsoft.Json;
using System;

namespace NovaAlert.Common.Utils
{
    public static class JsonHelper
    {
        public static string ExtractJSON(string str)
        {
            int firstOpen = str.IndexOf('{');
            int firstClose;
            string candidate;

            do
            {
                firstClose = str.LastIndexOf('}');
                if (firstClose <= firstOpen)
                {
                    return null;
                }

                do
                {
                    candidate = str.Substring(firstOpen, firstClose + 1);
                    if (IsValidJON(candidate))
                    {
                        return candidate;
                    }
                    else
                    {
                        firstClose = str.Substring(0, firstClose).LastIndexOf('}');
                    }
                }
                while (firstClose > firstOpen);
            } 
            while (firstOpen != -1);

            return null;
        }

        static bool IsValidJON(string str)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Perform a deep Copy of the object, using Json as a serialisation method.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T CloneJson<T>(this T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source));
        }
    }
}
