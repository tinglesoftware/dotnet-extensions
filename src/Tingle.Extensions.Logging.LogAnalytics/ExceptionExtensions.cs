using System.Globalization;
using System.Threading;

namespace System
{
    internal static class ExceptionExtensions
    {
        /// <summary>
        /// Returns a culture-independent string representation of the given <paramref name="exception"/> object, 
        /// appropriate for diagnostics tracing.
        /// </summary>
        public static string ToInvariantString(this Exception exception)
        {
            if (exception == null)
            {
                return string.Empty;
            }

            CultureInfo originalUICulture = Thread.CurrentThread.CurrentUICulture;
            try
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
                return exception.ToString();
            }
            finally
            {
                Thread.CurrentThread.CurrentUICulture = originalUICulture;
            }
        }
    }
}
