using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Editor_Reader;
using OsuParsers.Decoders;

namespace SVOscillator
{
    class TickrateManipulator
    {
        public static void Generate(string pathBase)
        {
            var reader = new EditorReader();
            reader.FetchAll();

            string path = $"{pathBase}\\{reader.ContainingFolder}\\{reader.Filename}";

            int mult = Utility.ReadInt("Enter tick rate multiplier");

            Utility.WaitForUserInput("Scroll in editor to start time, then hit 'Enter'");
            float startTime = reader.EditorTime();

            Utility.WaitForUserInput("Scroll in editor to end time, then hit 'Enter'");
            float endTime = reader.EditorTime();

            var beatmap = BeatmapDecoder.Decode(path);
            var timingPoints = beatmap.TimingPoints;

            var initial = timingPoints[0];

            var baseBeatLength = initial.BeatLength;
            var baseBpm = Utility.BeatlengthToBpm(baseBeatLength);

            int newBpm = baseBpm * mult;

            var startRedLine = Utility.MakeRedLine(startTime, newBpm);
            var endRedLine = Utility.MakeRedLine(endTime, baseBpm);

            float svToReturnTo = 1f;

            if (Utility.TryFindExistingByOffset(timingPoints, startTime, out int idx))
            {
                float oldSv = Utility.CalcSv(timingPoints[idx].BeatLength);
                svToReturnTo = oldSv;
                float newSv = oldSv / mult;

                timingPoints[idx].BeatLength = Utility.CalcBeatlength(newSv);
            }
            else if (Utility.TryFindMostRecentTimingPoint(timingPoints, startTime, out idx))
            {
                float oldSv = Utility.CalcSv(timingPoints[idx].BeatLength);
                svToReturnTo = oldSv;
                float newSv = oldSv / mult;

                timingPoints.Add(Utility.MakeGreenLine(startTime, newSv));

            }
            else
            {
                timingPoints.Add(Utility.MakeGreenLine(startTime, Utility.CalcSv(initial.BeatLength)));
            }


            if (!Utility.TryFindExistingByOffset(timingPoints, endTime, out idx))
            {
                timingPoints.Add(Utility.MakeGreenLine(endTime, svToReturnTo));
            }

            timingPoints.Add(startRedLine);
            timingPoints.Add(endRedLine);

            beatmap.Save(path);

            Console.WriteLine("Finished!");

        }
    }
}
