using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bssom.Serializer
{
    /// <summary>
    /// An exception thrown during serializing an object graph or deserializing a bss object
    /// </summary>
    public class BssomSerializationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BssomSerializationException"/> class.
        /// </summary>
        public BssomSerializationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BssomSerializationException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public BssomSerializationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BssomSerializationException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="inner"> The exception that is the cause of the current exception</param>
        public BssomSerializationException(string message,Exception inner)
            : base(message,inner)
        {
        }
    }
}
