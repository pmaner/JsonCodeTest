using JsonCodeTest.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;

namespace JsonCodeTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Catch everything
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            var errors = new List<string>();

            // Make sure our file exists and error nicely
            var directory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var testFile = Path.Combine(directory, "TestData/Pet.json");
            if (!File.Exists(testFile))
            {
                Console.Error.WriteLine($"'{testFile}' does not exist.");
                return;
            }

            // load json into strongly typed collection
            // Show that settigns can be set
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.MissingMemberHandling = MissingMemberHandling.Error;
            // catch and collect errors in the parsing
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

            // write out our cats if we loaded anything
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

        /// <summary>
        /// As the code for both the print out is amost the same, so just make it
        /// a fuction
        /// </summary>
        /// <param name="owners"></param>
        /// <param name="gender"></param>
        static private void WriteCatOwners(List<Owner> owners, Gender gender)
        {
            Console.WriteLine(gender);
            var cats = owners.Where(o => (o.Gender == gender) && (o.Pets != null))
                                           .SelectMany(o => o.Pets.Where(p => p.Species == Species.Cat));

            // linq is not executed until enumerated so adding the order later just composes what will run
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

        /// <summary>
        /// Catch exceptions anywhere in the current .net domain and handle them in some
        /// consistent way. Would usually log to some file or database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Debug.WriteLine($"ERROR - UnHandled Domain Exception {(e.ExceptionObject as Exception).Message}");
        }
    }
}
