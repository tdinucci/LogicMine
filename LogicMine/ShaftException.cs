using System;

namespace LogicMine
{
    /// <summary>
    /// The exception type which is thrown when an error occurs within a shaft 
    /// </summary>
    public class ShaftException : Exception
    {
        /// <summary>
        /// Construct a new ShaftException
        /// </summary>
        /// <param name="message">The exception message</param>
        public ShaftException(string message) : base(message)
        {
        }

        /// <summary>
        /// Construct a new ShaftException
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The exception which lead to the current one</param>
        public ShaftException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}