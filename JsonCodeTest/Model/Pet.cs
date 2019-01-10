using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace JsonCodeTest.Model
{
    public enum Species
    {
        Cat, Dog, Fish
    }

    public class Pet
    {
        public string Name { get; set; }

        [JsonProperty(PropertyName = "type")]
        public Species Species { get; set; }

        [OnError]
        internal void OnError(StreamingContext context, ErrorContext errorContext)
        {
            Console.Error.WriteLine($"Pet Error ignored - at {errorContext.Path}: {errorContext.Error.Message}");
            errorContext.Handled = true;
        }
    }
}
