using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ivi.Visa.Interop;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var rm = new ResourceManagerClass();
           

            string addr = "USB0::0x2A8D::0xB318::MY57330031::0::INSTR";
            var myDmm2 = new FormattedIO488Class();
            myDmm2.IO = (IMessage)rm.Open(addr, AccessMode.NO_LOCK, 2000, "");
            


            while (true)
            {
                myDmm2.IO.WriteString("READ?");
                var result = myDmm2.ReadString();
                Console.WriteLine(result);
            }

            

            while (true)
            {
                Write("Press ENTER to start read/write");
                var key = Console.ReadKey();
                if (key.Key != ConsoleKey.Enter)
                    return;

                Write("Reading from device");

                var myDmm = new FormattedIO488Class();
                try
                {
                    myDmm.IO = (IMessage) rm.Open(addr, AccessMode.NO_LOCK, 2000, "");

                    myDmm.IO.WriteString("DATA:DATA? NVMEM");
                    var result = myDmm.ReadString();

                    var rawNumbers = result.Split(',');

                    Write($"found {rawNumbers.Length} measurements");

                    var formatProvider = new CultureInfo("en-us");

                    var chart = new Chart();

                    chart.AmpereValues = rawNumbers
                        .Select(x => double.Parse(x, formatProvider))
                        .ToList();

                    chart.Count = chart.AmpereValues.Count;
                    chart.Mittelwert = chart.AmpereValues.Average();
                    chart.Min = chart.AmpereValues.Min();
                    chart.Max = chart.AmpereValues.Max();
                    chart.Standardabweichung = chart.AmpereValues
                        .Select(x =>
                        {
                            var minusMittel = x - chart.Mittelwert;
                            return (double) Math.Sqrt((double) (minusMittel * minusMittel));
                        })
                        .Average();

                    Console.WriteLine(chart);

                    Write("Input Title (Default: " + chart.Title + "):");

                    var title = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(title))
                        chart.Title = title + "-" + chart.Title;

                    Write("Creating file: " + chart.Title);

                    var json = JsonConvert.SerializeObject(chart);
                    File.WriteAllText(@"D:\GoogleDrive\FH Joanneum\Master\graphs\logs\" + chart.Title + ".json", json,
                        Encoding.UTF8);

                    Write("Done creating file");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);

                }
                finally
                {
                    myDmm.IO.Close();
                }
            }

            Console.Read();
        }

        public static void Write(string t)
        {
            Console.WriteLine(t);
        }
    
    }

    class Chart
    {
        public List<double> AmpereValues = new List<double>();
        public long Count;
        public double Mittelwert;
        public double Min;
        public double Max;
        public double Standardabweichung;

        public string Title = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");

        public override string ToString()
        {
            return 
         $@"count {Count}
            mittelwert {Mittelwert}
            min {Min}
            max {Max}
            standardabweichung {Standardabweichung}";
        }
    }
}
