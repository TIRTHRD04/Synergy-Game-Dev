using System;

namespace AdaptiveOpponent.SimCli
{
    /// <summary>Command-line entry point for the headless simulator.</summary>
    public static class Program
    {
        /// <summary>Runs one simulator command and returns the process exit code.</summary>
        public static int Main(string[] args)
        {
            Console.Error.WriteLine("usage: sim <suite|compare|gate-check> ... (see docs/COMMANDS.md)");
            return 2;
        }
    }
}
