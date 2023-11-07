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

            float lo = ReadFloat("Enter Minimum SV (decimal)");
            float hi = ReadFloat("Enter Maximum SV (decimal)");
            int measuresPerRun = ReadInt("Enter number of measures half of one oscillation period should take (integer)");

            var reader = new EditorReader();
            reader.FetchAll();
            WaitForUserInput("Scroll in Editor to starting point, then hit 'Enter'");
            float startTime = reader.EditorTime();

            WaitForUserInput("Scroll in Editor to ending point, then hit 'Enter'");
            float endTime = reader.EditorTime();


            string path = $"{Environment.GetEnvironmentVariable("SONGS_FOLDER")}\\{reader.ContainingFolder}\\{reader.Filename}";

            var beatmap = BeatmapDecoder.Decode(@path);

            var timingPoints = beatmap.TimingPoints;
            var initial = timingPoints[0];
            int beatsPerMeasure = BeatsPerMeasure(initial.TimeSignature);
            double measureLength = initial.BeatLength * beatsPerMeasure;
            int numMeasures = (int)((endTime - startTime) / measureLength) + 1;

            float timeStep = (float)(initial.BeatLength / 2);

            float time = startTime;
            float svStep = (hi - lo) / (measuresPerRun * beatsPerMeasure * 2);
            float sv = lo;
            
            for (int i = 0; i < numMeasures; i++)
            {
                

                for (int j = 0; j < beatsPerMeasure * 2; j++)
                {
                    if (TryFindExistingByOffset(timingPoints, time, out int idx))
                    {
                        timingPoints[idx].BeatLength = CalcBeatlength(sv);
                    }
                    else
                    {
                        timingPoints.Add(MakeGreenLine(time, sv, initial.TimeSignature));
                    }
                    sv += svStep;
                    time += timeStep;
                }

                svStep = ((i + 1) % measuresPerRun) == 0 ? -svStep : svStep;
            }

            beatmap.Save(path);
            Console.WriteLine("Finished!");


        }

        private static float ReadFloat(string msg)
        {
            Console.WriteLine(msg);
            string input = Console.ReadLine();
            float retVal;
            if (!float.TryParse(input, out retVal))
            {
                throw new InvalidOperationException("Could not read float");
            }
            return retVal;
        }

        private static int ReadInt(string msg)
        {
            return (int)ReadFloat(msg);
        }

        private static void WaitForUserInput(string msg)
        {
            Console.WriteLine(msg);
            Console.ReadLine();
        }

        private static int BeatsPerMeasure(OsuParsers.Enums.Beatmaps.TimeSignature timeSignature)
        {
            int retVal = timeSignature == OsuParsers.Enums.Beatmaps.TimeSignature.SimpleQuadruple ? 4 : 3;
            return retVal;
        }

        private static OsuParsers.Beatmaps.Objects.TimingPoint MakeGreenLine(float offset, float sv, OsuParsers.Enums.Beatmaps.TimeSignature timeSignature = OsuParsers.Enums.Beatmaps.TimeSignature.SimpleQuadruple)
        {
            var retVal = new OsuParsers.Beatmaps.Objects.TimingPoint();

            retVal.Inherited = true;
            retVal.CustomSampleSet = 1;
            retVal.SampleSet = OsuParsers.Enums.Beatmaps.SampleSet.Soft;
            retVal.Offset = (int)offset;
            retVal.Effects = OsuParsers.Enums.Beatmaps.Effects.None;
            retVal.TimeSignature = timeSignature;
            retVal.Volume = 50;
            retVal.BeatLength = CalcBeatlength(sv);

            return retVal;
        }

        private static bool TryFindExistingByOffset(List<OsuParsers.Beatmaps.Objects.TimingPoint> timingPoints, float offset, out int result)
        {
            for (int i = 0; i < timingPoints.Count; i++)
            {
                if (timingPoints[i].Offset == (int)offset)
                {
                    result = i;
                    return true;
                }
            }

            result = -1;
            return false;

        }

        private static float CalcBeatlength(float sv)
        {
            return 1 / ((float)-0.01 * sv);
        }
    }
}
