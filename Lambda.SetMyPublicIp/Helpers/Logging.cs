using System;

namespace Lambda.SetMyPublicIp.Helpers
{
    public static class Logging
    {
        /// <summary>
        /// Adds a prefix to the message and logs it
        /// </summary>
        /// <param name="message">the Message to be logged</param>
        public static void Log(string message)
        {
            Console.WriteLine($" - SetMyPublicIp: {message}");
        }
    }
}
