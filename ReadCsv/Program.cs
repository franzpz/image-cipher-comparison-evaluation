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
        private const decimal IdleRangeStart = 0.1m;
        private const decimal IdleRangeEnd = 0.15m;
        private const decimal voltage = 3.9m;

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
                csv.WriteField(trace.Min.ToString(CultureInfo));

                csv.WriteField("Max");
                csv.WriteField(trace.Max.ToString(CultureInfo));
                csv.NextRecord();

                csv.WriteField("Count");
                csv.WriteField(trace.Count);
                csv.NextRecord();

                csv.WriteField("Average");
                csv.WriteField(trace.Mittelwert.ToString(CultureInfo));
                csv.NextRecord();

                csv.WriteField("Standardabweichung");
                csv.WriteField(trace.Standardabweichung.ToString(CultureInfo));
                csv.NextRecord();

                csv.WriteField("Average Watt");
                csv.WriteField(trace.MittelwertWatt.ToString(CultureInfo));
                csv.NextRecord();

                csv.WriteField("Standardabweichung Watt");
                csv.WriteField(trace.StandardabweichungWattbefore.ToString(CultureInfo));
                csv.NextRecord();

                csv.WriteField("Hours");
                csv.WriteField(trace.Hours.ToString(CultureInfo));
                csv.WriteField(TimeSpan.FromHours((double)trace.Hours).Hours);
                csv.WriteField(TimeSpan.FromHours((double)trace.Hours).Minutes);
                csv.WriteField(TimeSpan.FromHours((double)trace.Hours).Seconds);
                csv.NextRecord();

                csv.WriteField("Watt Hours");
                csv.WriteField(trace.Watthours.ToString(CultureInfo));
                csv.NextRecord();

                csv.WriteField("Within Sigma 1 in %");
                csv.WriteField(trace.PercentageOfValuesWithin1Sigma.ToString(CultureInfo));
                csv.NextRecord();

                csv.WriteField("Within Sigma 2 in %");
                csv.WriteField(trace.PercentageOfValuesWithin2Sigma.ToString(CultureInfo));
                csv.NextRecord();

                csv.WriteField("Within Sigma 3 in %");
                csv.WriteField(trace.PercentageOfValuesWithin3Sigma.ToString(CultureInfo));
                csv.NextRecord();

                if (trace.Rounds.Any())
                {
                    csv.WriteField("Standardabweichung Watt min");
                    csv.WriteField(trace.Rounds.Select(x => x.StandardabweichungWattbefore).Min()
                        .ToString(CultureInfo));
                    csv.NextRecord();

                    csv.WriteField("Standardabweichung Watt max");
                    csv.WriteField(trace.Rounds.Select(x => x.StandardabweichungWattbefore).Max()
                        .ToString(CultureInfo));
                    csv.NextRecord();

                    csv.WriteField("Watt hours per round min");
                    csv.WriteField(trace.Rounds.Select(x => x.Watthours).Min().ToString(CultureInfo));
                    csv.NextRecord();

                    csv.WriteField("Watt hours per round max");
                    csv.WriteField(trace.Rounds.Select(x => x.Watthours).Max().ToString(CultureInfo));
                    csv.NextRecord();

                    csv.WriteField("Average time per round (based on 100)");
                    csv.WriteField(trace.Rounds.Average(x => x.Hours / 100 * 60 * 60).ToString(CultureInfo));
                    csv.NextRecord();
                }

                for (int i = 0; i < trace.Rounds.Count; i++)
                {
                    var round = trace.Rounds[i];
                    csv.NextRecord();
                    
                    csv.WriteField("Round");
                    csv.WriteField(i);
                    csv.NextRecord();

                    csv.WriteField("Start");
                    csv.WriteField(round.Start);

                    csv.WriteField("End");
                    csv.WriteField(round.End);
                    csv.NextRecord();

                    csv.WriteField("Min");
                    csv.WriteField(round.Min.ToString(CultureInfo));

                    csv.WriteField("Max");
                    csv.WriteField(round.Max.ToString(CultureInfo));
                    csv.NextRecord();

                    csv.WriteField("Count");
                    csv.WriteField(round.Count);
                    csv.NextRecord();

                    csv.WriteField("Average");
                    csv.WriteField(round.Mittelwert.ToString(CultureInfo));
                    csv.NextRecord();

                    csv.WriteField("Standardabweichung");
                    csv.WriteField(round.Standardabweichung.ToString(CultureInfo));
                    csv.NextRecord();

                    csv.WriteField("Average Watt");
                    csv.WriteField(round.MittelwertWatt.ToString(CultureInfo));
                    csv.NextRecord();

                    csv.WriteField("Standardabweichung Watt");
                    csv.WriteField(round.StandardabweichungWattbefore.ToString(CultureInfo));
                    csv.NextRecord();

                    csv.WriteField("Hours");
                    csv.WriteField(round.Hours.ToString(CultureInfo));
                    csv.WriteField(TimeSpan.FromHours((double)round.Hours).Hours);
                    csv.WriteField(TimeSpan.FromHours((double)round.Hours).Minutes);
                    csv.WriteField(TimeSpan.FromHours((double)round.Hours).Seconds);
                    csv.NextRecord();

                    csv.WriteField("Watt Hours");
                    csv.WriteField(round.Watthours.ToString(CultureInfo));
                    csv.NextRecord();

                    csv.WriteField("Within Sigma 1 in %");
                    csv.WriteField(round.PercentageOfValuesWithin1Sigma.ToString(CultureInfo));
                    csv.NextRecord();

                    csv.WriteField("Within Sigma 2 in %");
                    csv.WriteField(round.PercentageOfValuesWithin2Sigma.ToString(CultureInfo));
                    csv.NextRecord();

                    csv.WriteField("Within Sigma 3 in %");
                    csv.WriteField(round.PercentageOfValuesWithin3Sigma.ToString(CultureInfo));
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
                try
                {
                    csv.Read();
                    trace.End = csv.GetField<DateTime>(1);
                }
                catch (Exception)
                {
                    trace.End = DateTime.MaxValue;
                }

                while (!csv.GetField<string>(0).StartsWith("Time"))
                    csv.Read();

                while (csv.Read())
                {
                    var value = csv.GetField<string>(1);
                    decimal dValue;
                    if (value.Contains(","))
                        dValue = (decimal)double.Parse(value);
                    else
                        dValue = (decimal)double.Parse(value, CultureInfo);

                    trace.Samples.Add(new Sample()
                    {
                        Timestamp = csv.GetField<DateTime>(0),
                        Value = dValue
                    });
                }
            }

            trace.Samples = trace.Samples.OrderBy(x => x.Timestamp).ToList();

            if (isIdle)
            {
                var samples = trace.Samples.Select(x => x.Value).ToList();

                trace = (BenchVueTrace) CalculateResults(samples, trace);
            }
            else
            {
                trace.Rounds = FindRoundsInSamples(trace);

                var samples = trace.Rounds.Select(x => x.Mittelwert).ToList();
                trace = (BenchVueTrace) CalculateResults(samples, trace);
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

                    var byTimestamp = x.Samples.OrderBy(s => s.Timestamp).ToArray();
                    x.Start = byTimestamp.First().Timestamp;
                    x.End = byTimestamp.Last().Timestamp;

                    x = CalculateResults(x.Samples.Select(s => s.Value).ToList(), x);
                });

            return rounds;
        }

        public static Round CalculateResults(List<decimal> samples, Round trace = null)
        {
            var wattSamples = samples.Select(x => x * voltage).ToList();

            trace = trace ?? new BenchVueTrace();
            trace.Count = samples.Count;
            trace.Mittelwert = samples.Average();
            trace.MittelwertWatt = wattSamples.Average();
            trace.Min = samples.Min();
            trace.Max = samples.Max();

            trace.Standardabweichung = (decimal)Math.Sqrt((double)samples
                .Select(s =>
                {
                    var minusMittel = s - trace.Mittelwert;
                    return (decimal)(minusMittel * minusMittel);
                })
                .Average()
            );

            trace.StandardabweichungWattbefore = (decimal)Math.Sqrt((double)wattSamples
                .Select(s =>
                {
                    var minusMittel = s - trace.MittelwertWatt;
                    return (decimal)(minusMittel * minusMittel);
                })
                .Average()
            );

            trace.StandardabweichungWattafter = trace.Standardabweichung * voltage;

            if (trace.End != DateTime.MaxValue)
            {
                var hours = (decimal)(trace.End - trace.Start).TotalHours;
                trace.Hours = hours;
                trace.Watthours = hours * trace.MittelwertWatt;
            }

            trace.PercentageOfValuesWithin1Sigma = CalcWithin(wattSamples, 1, trace);
            trace.PercentageOfValuesWithin2Sigma = CalcWithin(wattSamples, 2, trace);
            trace.PercentageOfValuesWithin3Sigma = CalcWithin(wattSamples, 3, trace);
            /*
             * count = samples
             */

            return trace;
        }

        public static decimal CalcWithin(List<decimal> wattSamples, int sigma, Round r)
        {
            var min = (r.MittelwertWatt - ((decimal)sigma) * r.StandardabweichungWattbefore);
            var max = (r.MittelwertWatt + ((decimal)sigma) * r.StandardabweichungWattbefore);
            var countValuesWithin =
                wattSamples.Count(x => x >= min && x <= max);
            return ((decimal)countValuesWithin) / ((decimal)r.Count) * 100.0m;
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
            var percentageNeedsToBeCipher = 0.5m;

            var maxNumberOfSamplesToLookAt = 14;
            var minNumberOfSamplesToLookAt = Math.Min(5, traceSamples.Count - i);

            var actNumOfCipherSamples = 0.0m;

            for (int n = i + 1; n < i + maxNumberOfSamplesToLookAt; n++)
            {
                if (n >= traceSamples.Count)
                    break;

                numOfSamplesLookedAt++;
                
                if (IsCipherSample(traceSamples[n]))
                    actNumOfCipherSamples++;

                if(numOfSamplesLookedAt < minNumberOfSamplesToLookAt)
                    continue;

                if ((decimal)actNumOfCipherSamples / (decimal)numOfSamplesLookedAt
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
            var percentageNeedsToBeCipher = 0.5m;

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

                if ((decimal)actNumOfCipherSamples / (decimal)numOfSamplesLookedAt
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

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public long Count;
        public decimal Mittelwert;
        public decimal Min;
        public decimal Max;
        public decimal Standardabweichung;

        public decimal MittelwertWatt { get; set; }
        public decimal StandardabweichungWattbefore { get; set; }
        public decimal StandardabweichungWattafter { get; set; }
        public decimal Hours { get; set; }
        public decimal Watthours { get; set; }

        public decimal PercentageOfValuesWithin1Sigma = 0;
        public decimal PercentageOfValuesWithin2Sigma = 0;
        public decimal PercentageOfValuesWithin3Sigma = 0;

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

    public class BenchVueTrace : Round
    {
        public string File { get; set; }

        public List<Sample> Samples = new List<Sample>();

        public List<Round> Rounds { get; set; } = new List<Round>();

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

        public Sample(DateTime timestamp, decimal value) : this()
        {
            Timestamp = timestamp;
            Value = value;
        }

        public long Index { get; set; }

        public DateTime Timestamp { get; set; }
        public decimal Value { get; set; }
    }
}
