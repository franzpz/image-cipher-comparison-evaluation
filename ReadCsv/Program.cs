using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using static System.Console;

namespace ReadCsv
{
    public class Program
    {
        public static CultureInfo CultureInfo = new CultureInfo("en-us");
        private const int CipherPauseThreshold = 20;
        private const int RoundThreshold = 15;
        private const double IdleRangeStart = 0.1;
        private const double IdleRangeEnd = 0.15;

        static void Main(string[] args)
        {
            var logs = @"D:\GoogleDrive\FH Joanneum\Master\graphs\bench-vue-logs";

            WriteLine($"Reading files from {logs}");

            foreach (var file in Directory.GetFiles(logs, "*.csv"))
            {
                if(Path.GetFileName(file).StartsWith(CalcPrefix))
                    continue;

                var isIdle = Path.GetFileNameWithoutExtension(file)
                    .ToLower()
                    .StartsWith("idle");
             
                WriteLine("--------------------------------------");
                WriteLine($"Working on file {file}");
                WriteLine($"File is idle: {isIdle}");

                var trace = ConvertFileToTrace(file, isIdle);

                WriteLine("Trace generated:");
                WriteLine(trace);
                WriteLine("--------------------------------------");

                WriteToExcel(trace);
            }

            Read();
        }

        private static void WriteToExcel(BenchVueTrace trace)
        {
            var newFile = Path.Combine(Path.GetDirectoryName(trace.File), CalcPrefix + Path.GetFileName(trace.File));

            using (var writer = new StreamWriter(newFile))
            using (var csv = new CsvWriter(writer, new Configuration() {Delimiter = ";"}))
            {
                csv.WriteField("Source File");
                csv.WriteField(trace.File);
                csv.NextRecord();

                csv.WriteField("Start");
                csv.WriteField(trace.Start);

                csv.WriteField("End");
                csv.WriteField(trace.End);
                csv.NextRecord();

                csv.WriteField("Min");
                csv.WriteField(trace.Min);

                csv.WriteField("Max");
                csv.WriteField(trace.Max);
                csv.NextRecord();

                csv.WriteField("Count");
                csv.WriteField(trace.Count);
                csv.NextRecord();

                csv.WriteField("Average");
                csv.WriteField(trace.Mittelwert);
                csv.NextRecord();

                csv.WriteField("Standardabweichung");
                csv.WriteField(trace.Standardabweichung);
                csv.NextRecord();

                for (int i = 0; i < trace.Rounds.Count; i++)
                {
                    var round = trace.Rounds[i];
                    csv.NextRecord();
                    
                    csv.WriteField("Round");
                    csv.WriteField(i);
                    csv.NextRecord();

                    csv.WriteField("Min");
                    csv.WriteField(round.Min);

                    csv.WriteField("Max");
                    csv.WriteField(round.Max);
                    csv.NextRecord();

                    csv.WriteField("Count");
                    csv.WriteField(round.Count);
                    csv.NextRecord();

                    csv.WriteField("Average");
                    csv.WriteField(round.Mittelwert);
                    csv.NextRecord();

                    csv.WriteField("Standardabweichung");
                    csv.WriteField(round.Standardabweichung);
                    csv.NextRecord();

                    csv.NextRecord();
                }

                csv.Flush();
            }
        }

        private static string CalcPrefix
        {
            get { return "Calc-Values-"; }
        }

        public static BenchVueTrace ConvertFileToTrace(string file, bool isIdle = false)
        {
            CultureInfo enCulture;
            var trace = new BenchVueTrace();
            trace.File = file;
            using (var reader = new StreamReader(file))
            using (var csv = new CsvReader(reader, new Configuration() {Delimiter = ";"}))
            {
                for (int i = 0; i < 4; i++)
                    csv.Read();

                csv.Read();
                trace.Start = csv.GetField<DateTime>(1);
                csv.Read();
                trace.End = csv.GetField<DateTime>(1);

                for (int i = 0; i < 7; i++)
                    csv.Read();

                while (csv.Read())
                {
                    trace.Samples.Add(new Sample()
                    {
                        Timestamp = csv.GetField<DateTime>(0),
                        Value = double.Parse(csv.GetField<string>(1), CultureInfo)
                    });
                }
            }

            trace.Samples = trace.Samples.OrderBy(x => x.Timestamp).ToList();

            if (isIdle)
            {
                var samples = trace.Samples.Select(x => x.Value).ToList();
                trace.Count = samples.Count;
                trace.Mittelwert = samples.Average();
                trace.Min = samples.Min();
                trace.Max = samples.Max();
                trace.Standardabweichung = samples
                    .Select(s =>
                    {
                        var minusMittel = s - trace.Mittelwert;
                        return (double)Math.Sqrt((double)(minusMittel * minusMittel));
                    })
                    .Average();
            }
            else
            {
                trace.Rounds = FindRoundsInSamples(trace);

                var values = trace.Rounds;
                trace.Count = values.Count;
                trace.Mittelwert = values.Select(s => s.Mittelwert).Average();
                trace.Min = values.Select(s => s.Mittelwert).Min();
                trace.Max = values.Select(s => s.Mittelwert).Max();
                trace.Standardabweichung = values.Select(s => s.Mittelwert)
                    .Select(s =>
                    {
                        var minusMittel = s - trace.Mittelwert;
                        return (double)Math.Sqrt((double)(minusMittel * minusMittel));
                    })
                    .Average();
            }

            return trace;
        }

        public static List<Round> FindRoundsInSamples(BenchVueTrace trace)
        {
            var rounds = new List<Round>();
            Round currRound = null;

            for (int i = 0; i < trace.Samples.Count; i++)
            {
                var currSample = trace.Samples[i];

                /*
                 * isciphersample && 
                 * nextsamplesarecipher || lastsampleswerecipher
                 * 
                 * 
                 * isIdle && (
                 *  nextsamplesarecipher && lastsampleswerecipher
                 * 
                 * 
                 */

                var nextSamplesAreCipher = NextSamplesAreCipher(trace.Samples, i);
                var lastSamplesWereCipher = LastSamplesWereCipher(trace.Samples, i);
                var isCipher = IsCipherSample(currSample);
                var isIdle = IsIdleSample(currSample);

                if (isCipher && 
                    (
                        (currRound == null && nextSamplesAreCipher && lastSamplesWereCipher) ||
                        (currRound != null && (nextSamplesAreCipher || lastSamplesWereCipher))
                    )
                )
                {
                    if (currRound == null)
                    {
                        currRound = new Round();
                        rounds.Add(currRound);
                    }

                    currRound.Samples.Add(currSample);
                }
                else if (isIdle && (
                             nextSamplesAreCipher && lastSamplesWereCipher)
                )
                {
                    currRound?.Samples.Add(currSample);
                }
                else
                {
                    currRound = null;
                }
            }

            rounds = rounds
                .Where(x => x.Samples.Count > 10)
                .ToList();

            rounds
                .ForEach(x =>
                {
                    for (int i = 0; i < x.Samples.Count; i++)
                    {
                        if (i >= x.Samples.Count)
                            break;

                        if (IsIdleSample(x.Samples[i]))
                            x.Samples.Remove(x.Samples[i]);
                        else
                            break;
                    }

                    for (int i = x.Samples.Count - 1; i >= 0; i--)
                    {
                        if (i >= x.Samples.Count)
                            break;

                        if (IsIdleSample(x.Samples[i]))
                            x.Samples.Remove(x.Samples[i]);
                        else
                            break;
                    }

                    x.Count = x.Samples.Count;
                    x.Mittelwert = x.Samples.Select(s => s.Value).Average();
                    x.Min = x.Samples.Select(s => s.Value).Min();
                    x.Max = x.Samples.Select(s => s.Value).Max();
                    x.Standardabweichung = x.Samples.Select(s => s.Value)
                        .Select(s =>
                        {
                            var minusMittel = s - x.Mittelwert;
                            return (double)Math.Sqrt((double)(minusMittel * minusMittel));
                        })
                        .Average();
                });

            return rounds;
        }

        private static bool IsCipherSample(Sample currSample)
        {
            return currSample.Value > IdleRangeEnd;
        }

        private static bool IsIdleSample(Sample currSample)
        {
            return currSample.Value > IdleRangeStart && currSample.Value <= IdleRangeEnd;
        }

        private static bool NextSamplesAreCipher(List<Sample> traceSamples, int i)
        {
            if (i + 1 >= traceSamples.Count)
                return true;

            var numOfSamplesLookedAt = 0;
            var percentageNeedsToBeCipher = 0.5;

            var maxNumberOfSamplesToLookAt = 14;
            var minNumberOfSamplesToLookAt = Math.Min(5, traceSamples.Count - i);

            var actNumOfCipherSamples = 0;

            for (int n = i + 1; n < i + maxNumberOfSamplesToLookAt; n++)
            {
                if (n >= traceSamples.Count)
                    break;

                numOfSamplesLookedAt++;
                
                if (IsCipherSample(traceSamples[n]))
                    actNumOfCipherSamples++;

                if(numOfSamplesLookedAt < minNumberOfSamplesToLookAt)
                    continue;

                if ((double)actNumOfCipherSamples / (double)numOfSamplesLookedAt
                    >= percentageNeedsToBeCipher)
                    return true;
            }

            return false;
        }

        private static bool LastSamplesWereCipher(List<Sample> traceSamples, int i)
        {
            if (i == 0)
                return true;

            var numOfSamplesLookedAt = 0;
            var percentageNeedsToBeCipher = 0.5;

            var maxNumberOfSamplesToLookAt = 14;
            var minNumberOfSamplesToLookAt = Math.Min(5, i);

            var actNumOfCipherSamples = 0;

            for (int n = i - maxNumberOfSamplesToLookAt; n < i; n++)
            {
                if (n < 0)
                    continue;

                numOfSamplesLookedAt++;

                if (IsCipherSample(traceSamples[n]))
                    actNumOfCipherSamples++;

                if (numOfSamplesLookedAt < minNumberOfSamplesToLookAt)
                    continue;

                if ((double)actNumOfCipherSamples / (double)numOfSamplesLookedAt
                    >= percentageNeedsToBeCipher)
                    return true;
            }

            return false;
        }

        private static bool OneOfLastSamplesWasCipher(List<Sample> traceSamples, int i)
        {
            if (i == 0)
                return false;

            bool oneOfLastSamplesWasCipher = false;

            for (int n = i - 6; n < i; n++)
            {
                if (n >= 0 && traceSamples.Count > n)
                    oneOfLastSamplesWasCipher = oneOfLastSamplesWasCipher || IsCipherSample(traceSamples[n]);
            }

            return oneOfLastSamplesWasCipher;
        }
    }

    public class Round
    {
        public int Index { get; set; }
        public List<Sample> Samples { get; set; } = new List<Sample>();

        public long Count;
        public double Mittelwert;
        public double Min;
        public double Max;
        public double Standardabweichung;

        public override string ToString()
        {
            return
                $@"----------------------
Count: {Count}
Mittelwert: {Mittelwert}
Min: {Min}
Max: {Max}
Standardabweichung: {Standardabweichung}
------------------------";
        }
    }

    public class BenchVueTrace
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string File { get; set; }

        public List<Sample> Samples = new List<Sample>();

        public List<Round> Rounds { get; set; } = new List<Round>();

        public long Count;
        public double Mittelwert;
        public double Min;
        public double Max;
        public double Standardabweichung;

        public override string ToString()
        {
            var traceString = 
                $@"----------------------
Start: {Start}
End: {End}
File: {File}
Count: {Count}
Mittelwert: {Mittelwert}
Min: {Min}
Max: {Max}
Standardabweichung: {Standardabweichung}
------------------------";

            Rounds.ForEach(r => traceString += r.ToString());

            return traceString;
        }
    }

    public class Sample
    {
        private static long currIndex = 0;

        public Sample()
        {
            Index = currIndex++;
        }

        public Sample(DateTime timestamp, double value) : this()
        {
            Timestamp = timestamp;
            Value = value;
        }

        public long Index { get; set; }

        public DateTime Timestamp { get; set; }
        public double Value { get; set; }
    }
}
