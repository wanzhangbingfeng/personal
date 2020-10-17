using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System.Text.Json.Serialization
{
    public abstract class JsonConverter
    {
        internal JsonConverter() { }

        /// <summary>
        /// Determines whether the type can be converted.
        /// </summary>
        /// <param name="typeToConvert">The type is checked as to whether it can be converted.</param>
        /// <returns>True if the type can be converted, false otherwise.</returns>
        public abstract bool CanConvert(Type typeToConvert);

        // This is used internally to quickly determine the type being converted for JsonConverter<T>.
        internal virtual Type? TypeToConvert => null;
    }
}
