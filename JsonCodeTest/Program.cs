using JsonCodeTest.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;

namespace JsonCodeTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var errors = new List<string>();
            var directory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var testFile = Path.Combine(directory, "TestData/Pet.json");
            if (!File.Exists(testFile))
            {
                Console.Error.WriteLine($"'{testFile}' does not exist.");
                return;
            }

            // load json into strongly typed collection
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.MissingMemberHandling = MissingMemberHandling.Error;
            settings.Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs a)
            {
                errors.Add(a.ErrorContext.Error.Message);
                a.ErrorContext.Handled = true;
            };

            List<Owner> owners = null;
            try
            {
                // I know this does nothing useful here, its just an example of Async
                owners = Task.Run( () => JsonConvert.DeserializeObject<List<Owner>>(File.ReadAllText(testFile), settings)).Result;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"ERROR: {ex.Message}");
            }

            if (owners != null)
            {
                WriteCatOwners(owners, Gender.Male);
                Console.WriteLine();
                WriteCatOwners(owners, Gender.Female);
            }
            else
                Console.WriteLine($"'{testFile}' failed to load.");

            WriteErrors(errors);
            Console.ReadKey();
        }

        static private void WriteCatOwners(List<Owner> owners, Gender gender)
        {
            Console.WriteLine(gender);
            var cats = owners.Where(o => (o.Gender == gender) && (o.Pets != null))
                                           .SelectMany(o => o.Pets.Where(p => p.Species == Species.Cat));
            foreach (var cat in cats.OrderBy(c => c.Name))
                Console.WriteLine($"- {cat.Name}");
        }

        /// <summary>
        /// Write out any errors that happened and were ignored
        /// </summary>
        /// <param name="errors"></param>
        static private void WriteErrors(IEnumerable<string> errors)
        {
            if (errors.Any()) 
            {
                Console.WriteLine();
                Console.WriteLine($"The Following errors were ignored parsing owner");
                foreach (var error in errors)
                    Console.WriteLine($"- {error}");
            }
        }
    }
}
