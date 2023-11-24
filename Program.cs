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

           while (true)
            {
                string mode = Utility.ReadString("Enter mode or 'q' to quit");

                switch (mode)
                {
                    case "o":
                        Oscillator.Generate(pathBase);
                        break;

                    case "t":
                        TickrateManipulator.Generate(pathBase);
                        break;

                    case "q":
                        return;

                    default:
                        Console.WriteLine($"Invalid mode '{mode}'. Mode must be one of 'o', 't'.");
                        break;
                }
            }
            


        }

        
    }
}
