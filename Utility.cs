using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVOscillator
{
    class Utility
    {
        public static float ReadFloat(string msg)
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

        public static int ReadInt(string msg)
        {
            return (int)ReadFloat(msg);
        }

        public static string ReadString(string msg)
        {
            Console.WriteLine(msg);
            return Console.ReadLine();
        }

        public static void WaitForUserInput(string msg)
        {
            Console.WriteLine(msg);
            Console.ReadLine();
        }


        public static int BeatsPerMeasure(OsuParsers.Enums.Beatmaps.TimeSignature timeSignature)
        {
            int retVal = timeSignature == OsuParsers.Enums.Beatmaps.TimeSignature.SimpleQuadruple ? 4 : 3;
            return retVal;
        }

        public static OsuParsers.Beatmaps.Objects.TimingPoint MakeGreenLine(float offset, float sv, OsuParsers.Enums.Beatmaps.TimeSignature timeSignature = OsuParsers.Enums.Beatmaps.TimeSignature.SimpleQuadruple)
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

        public static OsuParsers.Beatmaps.Objects.TimingPoint MakeRedLine(float offset, int bpm, OsuParsers.Enums.Beatmaps.TimeSignature timeSignature = OsuParsers.Enums.Beatmaps.TimeSignature.SimpleQuadruple)
        {
            var retVal = new OsuParsers.Beatmaps.Objects.TimingPoint();

            retVal.Inherited = false;
            retVal.CustomSampleSet = 1;
            retVal.SampleSet = OsuParsers.Enums.Beatmaps.SampleSet.Soft;
            retVal.Offset = (int)offset;
            retVal.Effects = OsuParsers.Enums.Beatmaps.Effects.None;
            retVal.TimeSignature = timeSignature;
            retVal.Volume = 50;
            retVal.BeatLength = BpmToBeatlength(bpm);

            return retVal;
        }

        public static bool TryFindExistingByOffset(List<OsuParsers.Beatmaps.Objects.TimingPoint> timingPoints, float offset, out int result)
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

        public static bool TryFindExistingByOffset(List<OsuParsers.Beatmaps.Objects.TimingPoint> timingPoints, float offset, out List<int> result)
        {
            result = new List<int>();
            bool retVal = false;

            for (int i = 0; i < timingPoints.Count; i++)
            {
                if (timingPoints[i].Offset == (int)offset)
                {
                    result.Add(i);
                    retVal = true;
                }
            }

            return retVal;
        }

        public static bool TryFindMostRecentTimingPoint(List<OsuParsers.Beatmaps.Objects.TimingPoint> timingPoints, float offset, out int result, bool inherited = true)
        {
            for (float time = (int)offset; time >= 0; time--)
            {
                if (TryFindExistingByOffset(timingPoints, time, out List<int> candidates))
                {
                    foreach (int idx in candidates)
                    {
                        if (timingPoints[idx].Inherited == inherited)
                        {
                            result = idx;
                            return true;
                        }
                    }

                    continue;
                }
            }

            result = -1;
            return false;
        }


        public static float CalcBeatlength(float sv)
        {
            return 1 / ((float)-0.01 * sv);
        }

        public static float CalcSv(double beatLength)
        {
            return -1f / (0.01f * (float)beatLength);
        }

        public static double BpmToBeatlength(int bpm)
        {
            return 60000f / bpm;
        }

        public static int BeatlengthToBpm(double beatLength)
        {
            return (int)Math.Round(60000f / beatLength);
        }
    }
}
