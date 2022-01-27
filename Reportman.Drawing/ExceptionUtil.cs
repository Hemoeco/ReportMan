using System;

namespace Reportman.Drawing
{
    /// <summary>
    /// Provide utitilies about exception handling
    /// </summary>
    public class ExceptionUtil
    {
        /// <summary>
        /// Check if the provided exception is critical
        /// </summary>
        public static bool IsCritical(Exception ex)
        {
            if (ex is OutOfMemoryException)
                return true;
            if (ex is AppDomainUnloadedException)
                return true;
            if (ex is BadImageFormatException)
                return true;
            if (ex is CannotUnloadAppDomainException)
                return true;
            if (ex is InvalidProgramException)
                return true;
            return false;
        }

    }
}
