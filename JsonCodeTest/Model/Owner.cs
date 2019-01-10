using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace JsonCodeTest.Model
{
    public enum Gender
    {
        Male, Female
    }
    public class Owner
    {
        public string Name { get; set; }
        public Gender Gender { get; set; }
        public int Age { get; set; }
        public IList<Pet> Pets { get; set; }

        // commented out to show other error handler delegate for owners
        //[OnError]
        //internal void OnError(StreamingContext context, ErrorContext errorContext)
        //{
        //    Console.Error.WriteLine($"Owner Error - at {errorContext.Path}: {errorContext.Error.Message}");
        //    errorContext.Handled = true;
        //}
    }
}
