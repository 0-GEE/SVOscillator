using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Editor_Reader;
using OsuParsers.Decoders;

namespace SVOscillator
{
    class Oscillator
    {
        public static void Generate(string pathBase)
        {
            float lo = Utility.ReadFloat("Enter Minimum SV (decimal)");
            float hi = Utility.ReadFloat("Enter Maximum SV (decimal)");
            int measuresPerRun = Utility.ReadInt("Enter number of measures half of one oscillation period should take (integer)");

            var reader = new EditorReader();
            reader.FetchAll();
            Utility.WaitForUserInput("Scroll in Editor to starting point, then hit 'Enter'");
            float startTime = reader.EditorTime();

            Utility.WaitForUserInput("Scroll in Editor to ending point, then hit 'Enter'");
            float endTime = reader.EditorTime();


            string path = $"{pathBase}\\{reader.ContainingFolder}\\{reader.Filename}";

            var beatmap = BeatmapDecoder.Decode(@path);

            var timingPoints = beatmap.TimingPoints;
            var initial = timingPoints[0];
            int beatsPerMeasure = Utility.BeatsPerMeasure(initial.TimeSignature);
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
                    if (Utility.TryFindExistingByOffset(timingPoints, time, out int idx))
                    {
                        timingPoints[idx].BeatLength = Utility.CalcBeatlength(sv);
                    }
                    else
                    {
                        timingPoints.Add(Utility.MakeGreenLine(time, sv, initial.TimeSignature));
                    }
                    sv += svStep;
                    time += timeStep;
                }

                svStep = ((i + 1) % measuresPerRun) == 0 ? -svStep : svStep;
            }

            beatmap.Save(path);
            Console.WriteLine("Finished!");
        }
    }
}
