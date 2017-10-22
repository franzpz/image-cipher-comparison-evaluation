using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReadCsv;

namespace TestRoundRecognition
{
    [TestClass]
    public class IntegrationTest
    {
        [TestMethod]
        public void ReadFile_Returns5ExtRounds()
        {
            var file =
                @"D:\GoogleDrive\FH Joanneum\Master\graphs\bench-vue-logs\Cipher2-5x10-Trace 2017-10-22 11-03-17 0.csv";
            var trace = Program.ConvertFileToTrace(file);

            trace.Rounds.Count.Should().Be(5);
        }
    }
}
