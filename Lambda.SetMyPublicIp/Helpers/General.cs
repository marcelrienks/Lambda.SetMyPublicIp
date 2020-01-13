using System;
using System.Collections.Generic;

namespace Lambda.SetMyPublicIp.Helpers
{
    public static class General
    {
        /// <summary>
        /// Serializes message and stack trace information of he exception into json
        /// </summary>
        /// <param name="exception">the Exception to be handled</param>
        /// <returns>json representation of the exception</returns>
        public static string SerializeException(Exception exception)
        {
            if (exception == null)
                return string.Empty;

            var body = new Dictionary<string, string>();
            body.Add("Exception", exception.Message);
            body.Add("Stacktrace", exception.StackTrace);
            if (exception.InnerException != null)
                body.Add("InnerException", exception.InnerException.Message);

            return System.Text.Json.JsonSerializer.Serialize(body); 
        }
    }
}
