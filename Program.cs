using System;
using Editor_Reader;
using OsuParsers.Decoders;
using System.Collections.Generic;
using dotenv.net;

namespace SVOscillator
{
    class Program
    {
        static void Main(string[] args)
        {
            DotEnv.Load();
            string pathBase = Environment.GetEnvironmentVariable("SONGS_FOLDER");

            if (args.Length == 0)
            {
                Oscillator.Generate(pathBase);
                return;
            }


            string mode = args[0];

            switch (mode)
            {
                case "o":
                    Oscillator.Generate(pathBase);
                    break;

                case "t":
                    TickrateManipulator.Generate(pathBase);
                    break;

                default:
                    Console.WriteLine($"Invalid mode '{mode}'. Mode must be one of 'o', 't'.");
                    break;
            }


        }

        
    }
}
