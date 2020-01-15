using System.Linq;

namespace Lambda.SetMyPublicIp.Helpers
{
    public static class GeneralHelpers
    {
        /// <summary>
        /// Splits a string of multiple ip's and returns only the first one
        /// </summary>
        /// <param name="multipleIps">the string representing multiple ip addresses</param>
        /// <returns>first ip</returns>
        public static string GetFirstIp(string multipleIps)
        {
            return multipleIps?.Split(',').FirstOrDefault();
        }
    }
}
