using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;

namespace StackExchange.Precompilation
{
    static class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;

            try
            {
                if (!CompilationProxy.RunCs(args))
                {
                    Environment.ExitCode = 1;
                }
            }
            catch (Exception ex)
            {
                var agg = ex as AggregateException;
                Console.WriteLine("ERROR: An unhandled exception occured");
                if (agg != null)
                {
                    agg = agg.Flatten();
                    foreach (var inner in agg.InnerExceptions)
                    {
                        Console.Error.WriteLine("error: " + inner);
                    }
                }
                else
                {
                    Console.Error.WriteLine("error: " + ex);
                }
                Environment.ExitCode = 2;
            }
        }

        private static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = new AssemblyName(args.Name);

            // if we failed to resolve a simple name, nothing we can do
            if (name.Name.Equals(name.FullName)) return null;

            // drops the version information, effectively a binding redirect to whatever version is adjacent to the migrator dll.
            return Assembly.Load(name.Name);
        }
    }
}
