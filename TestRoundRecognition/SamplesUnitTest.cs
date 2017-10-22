﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReadCsv;

namespace TestRoundRecognition
{
    [TestClass]
    public class SamplesUnitTest
    {
        private static CultureInfo en = new CultureInfo("en-us");

        [TestMethod]
        public void FindRows_ReturnsTwoRows_FromSamples()
        {
            var rounds = Program.FindRoundsInSamples(
                new BenchVueTrace() {Samples = SamplesWithTwoRounds}
            );

            rounds.Count.Should().Be(2);

            rounds[0].Samples.Count.Should().BeInRange(68, 75);

            // pause 82

            rounds[1].Samples.Count.Should().BeInRange(13, 20);
        }


        

        private static Sample Create(string line)
        {
            var values = line.Trim().Split(';');

            return new Sample(
                DateTime.Parse(values[0]),
                double.Parse(values[1], en)
                );
        }

        private static List<Sample> SamplesWithTwoRounds = new List<Sample>()
        {
// round 1
Create("2017-10-21 22:40:08.304;0.220766267"),
Create("2017-10-21 22:40:08.716;0.222978589"),
Create("2017-10-21 22:40:09.303;0.222586287"),
Create("2017-10-21 22:40:09.849;0.222511959"),
Create("2017-10-21 22:40:10.400;0.276244522"),
Create("2017-10-21 22:40:10.942;0.223308489"),
Create("2017-10-21 22:40:11.496;0.21962439 "),
Create("2017-10-21 22:40:12.043;0.258777063"),
Create("2017-10-21 22:40:12.592;0.305102178"),
Create("2017-10-21 22:40:13.139;0.222507826"),
Create("2017-10-21 22:40:13.693;0.217456322"),
Create("2017-10-21 22:40:14.239;0.225958107"),
Create("2017-10-21 22:40:14.788;0.220845053"),
Create("2017-10-21 22:40:15.340;0.222274313"),
Create("2017-10-21 22:40:15.887;0.214856296"),
Create("2017-10-21 22:40:16.430;0.222874856"),
Create("2017-10-21 22:40:16.985;0.222830541"),
Create("2017-10-21 22:40:17.526;0.241382374"),
Create("2017-10-21 22:40:18.078;0.258279998"),
Create("2017-10-21 22:40:18.612;0.355553096"),
Create("2017-10-21 22:40:19.159;0.271638887"),
Create("2017-10-21 22:40:19.703;0.221490394"),
Create("2017-10-21 22:40:20.247;0.218914686"),
Create("2017-10-21 22:40:20.796;0.22097229 "),
Create("2017-10-21 22:40:21.344;0.223368814"),
Create("2017-10-21 22:40:21.892;0.219773439"),
Create("2017-10-21 22:40:22.437;0.221302491"),
Create("2017-10-21 22:40:22.994;0.220496503"),
Create("2017-10-21 22:40:23.533;0.221278968"),
Create("2017-10-21 22:40:24.085;0.238087893"),
Create("2017-10-21 22:40:24.629;0.347707201"),
Create("2017-10-21 22:40:25.172;0.352291847"),
Create("2017-10-21 22:40:25.726;0.230569446"),
Create("2017-10-21 22:40:26.272;0.324784621"),
Create("2017-10-21 22:40:26.819;0.308738899"),
Create("2017-10-21 22:40:27.368;0.302746332"),
Create("2017-10-21 22:40:27.913;0.382121319"),
Create("2017-10-21 22:40:28.458;0.27872326 "),
Create("2017-10-21 22:40:29.007;0.197961022"),
Create("2017-10-21 22:40:29.554;0.274663629"),
Create("2017-10-21 22:40:30.108;0.13121641 "),
Create("2017-10-21 22:40:30.649;0.112420101"),
Create("2017-10-21 22:40:31.202;0.10683453 "), //
Create("2017-10-21 22:40:31.745;0.111097181"),
Create("2017-10-21 22:40:32.293;0.193398352"),
Create("2017-10-21 22:40:32.845;0.109919612"),
Create("2017-10-21 22:40:33.390;0.231117968"),
Create("2017-10-21 22:40:33.941;0.11649622 "),
Create("2017-10-21 22:40:34.481;0.160667111"),
Create("2017-10-21 22:40:35.030;0.148054271"),
Create("2017-10-21 22:40:35.577;0.178320081"),
Create("2017-10-21 22:40:36.121;0.171005591"),
Create("2017-10-21 22:40:36.674;0.173428378"),
Create("2017-10-21 22:40:37.223;0.181437513"),
Create("2017-10-21 22:40:37.768;0.314329277"),
Create("2017-10-21 22:40:38.312;0.238810657"),
Create("2017-10-21 22:40:38.862;0.184045751"),
Create("2017-10-21 22:40:39.413;0.165900311"),
Create("2017-10-21 22:40:39.961;0.278915025"),
Create("2017-10-21 22:40:40.505;0.253990212"),
Create("2017-10-21 22:40:41.055;0.28507706 "),
Create("2017-10-21 22:40:41.602;0.269062782"),
Create("2017-10-21 22:40:42.150;0.198752001"),
Create("2017-10-21 22:40:42.697;0.169004684"),
Create("2017-10-21 22:40:43.246;0.157922026"),
Create("2017-10-21 22:40:43.793;0.156741078"),
Create("2017-10-21 22:40:44.348;0.162496942"),
Create("2017-10-21 22:40:44.892;0.152277702"),
Create("2017-10-21 22:40:45.442;0.177871629"),
Create("2017-10-21 22:40:45.991;0.169363266"),
Create("2017-10-21 22:40:46.535;0.125412463"),
Create("2017-10-21 22:40:47.083;0.193332779"),
Create("2017-10-21 22:40:47.631;0.112534082"),
Create("2017-10-21 22:40:48.184;0.207613663"), // index 74

// pause
Create("2017-10-21 22:40:48.732;0.127586344"),
Create("2017-10-21 22:40:49.275;0.110159097"),
Create("2017-10-21 22:40:49.822;0.103483298"),
Create("2017-10-21 22:40:50.371;0.123766354"),
Create("2017-10-21 22:40:50.918;0.109866717"),
Create("2017-10-21 22:40:51.465;0.118052203"),
Create("2017-10-21 22:40:52.020;0.110194367"),
Create("2017-10-21 22:40:52.556;0.110494804"),
Create("2017-10-21 22:40:53.101;0.113213488"),
Create("2017-10-21 22:40:53.649;0.136609918"),
Create("2017-10-21 22:40:54.197;0.130174147"),
Create("2017-10-21 22:40:54.744;0.110806026"),
Create("2017-10-21 22:40:55.291;0.102519828"),
Create("2017-10-21 22:40:55.842;0.114216494"),
Create("2017-10-21 22:40:56.391;0.110237587"),
Create("2017-10-21 22:40:56.931;0.109094503"),
Create("2017-10-21 22:40:57.477;0.111972896"),
Create("2017-10-21 22:40:58.025;0.110864835"),
Create("2017-10-21 22:40:58.563;0.126907804"),
Create("2017-10-21 22:40:59.114;0.131297685"),
Create("2017-10-21 22:40:59.662;0.124452163"),
Create("2017-10-21 22:41:00.214;0.137588616"),
Create("2017-10-21 22:41:00.763;0.111405619"),
Create("2017-10-21 22:41:01.315;0.109157591"),
Create("2017-10-21 22:41:01.856;0.114672575"),
Create("2017-10-21 22:41:02.405;0.109760931"),
Create("2017-10-21 22:41:02.955;0.194382007"),
Create("2017-10-21 22:41:03.501;0.208028849"),
Create("2017-10-21 22:41:04.048;0.213810128"),
Create("2017-10-21 22:41:04.592;0.11253067 "),
Create("2017-10-21 22:41:05.141;0.129764714"),
Create("2017-10-21 22:41:05.688;0.124150526"),
Create("2017-10-21 22:41:06.236;0.112670965"),
Create("2017-10-21 22:41:06.781;0.111471825"),
Create("2017-10-21 22:41:07.327;0.12389198 "),
Create("2017-10-21 22:41:07.875;0.138945743"),
Create("2017-10-21 22:41:08.419;0.103817327"),
Create("2017-10-21 22:41:08.972;0.110103837"),
Create("2017-10-21 22:41:09.519;0.120863197"),
Create("2017-10-21 22:41:10.069;0.109913869"),
Create("2017-10-21 22:41:10.614;0.109481287"),
Create("2017-10-21 22:41:11.160;0.129606138"),
Create("2017-10-21 22:41:11.708;0.138970416"),
Create("2017-10-21 22:41:12.254;0.112726748"),
Create("2017-10-21 22:41:12.805;0.109794087"),
Create("2017-10-21 22:41:13.346;0.1091433  "),
Create("2017-10-21 22:41:13.896;0.110112018"),
Create("2017-10-21 22:41:14.437;0.110379647"),
Create("2017-10-21 22:41:14.990;0.114858966"),
Create("2017-10-21 22:41:15.533;0.110876578"),
Create("2017-10-21 22:41:16.083;0.110194705"),
Create("2017-10-21 22:41:16.629;0.109992974"),
Create("2017-10-21 22:41:17.177;0.123867524"),
Create("2017-10-21 22:41:17.723;0.123965403"),
Create("2017-10-21 22:41:18.270;0.115617067"),
Create("2017-10-21 22:41:18.822;0.110740205"),
Create("2017-10-21 22:41:19.370;0.163200888"),
Create("2017-10-21 22:41:19.918;0.109684014"),
Create("2017-10-21 22:41:20.472;0.109061396"),
Create("2017-10-21 22:41:21.019;0.124742626"),
Create("2017-10-21 22:41:21.566;0.108930328"),
Create("2017-10-21 22:41:22.117;0.109650681"),
Create("2017-10-21 22:41:22.665;0.12626037 "),
Create("2017-10-21 22:41:23.214;0.126110553"),
Create("2017-10-21 22:41:23.754;0.186889107"),
Create("2017-10-21 22:41:24.299;0.161640774"),
Create("2017-10-21 22:41:24.846;0.110927836"),
Create("2017-10-21 22:41:25.897;0.139667017"),
Create("2017-10-21 22:41:26.436;0.102730703"),
Create("2017-10-21 22:41:26.988;0.108845272"),
Create("2017-10-21 22:41:27.533;0.118862853"),
Create("2017-10-21 22:41:28.076;0.108351349"),
Create("2017-10-21 22:41:28.641;0.108561101"),
Create("2017-10-21 22:41:29.195;0.109990868"),
Create("2017-10-21 22:41:29.739;0.133168667"),
Create("2017-10-21 22:41:30.293;0.111107052"),
Create("2017-10-21 22:41:30.840;0.109938021"),
Create("2017-10-21 22:41:31.383;0.108791463"),
Create("2017-10-21 22:41:31.930;0.110654701"),
Create("2017-10-21 22:41:32.476;0.110585406"),
Create("2017-10-21 22:41:33.022;0.115992533"),
Create("2017-10-21 22:41:33.568;0.110245523"),
Create("2017-10-21 22:41:34.117;0.10800163 "), // 74+83 = 157

// round 2
Create("2017-10-21 22:41:34.665;0.210544194"),
Create("2017-10-21 22:41:35.211;0.282204246"),
Create("2017-10-21 22:41:35.761;0.215520526"),
Create("2017-10-21 22:41:36.306;0.212653814"),
Create("2017-10-21 22:41:36.855;0.21237879 "),
Create("2017-10-21 22:41:37.400;0.218062509"),
Create("2017-10-21 22:41:37.951;0.168984736"),
Create("2017-10-21 22:41:38.493;0.213282278"),
Create("2017-10-21 22:41:39.039;0.211922348"),
Create("2017-10-21 22:41:39.587;0.22080619 "),
Create("2017-10-21 22:41:40.135;0.212102536"),
Create("2017-10-21 22:41:40.682;0.229975123"),
Create("2017-10-21 22:41:41.226;0.215782444"),
Create("2017-10-21 22:41:41.772;0.302342528"),
Create("2017-10-21 22:41:42.329;0.213257063"),
Create("2017-10-21 22:41:42.878;0.234045607"),
Create("2017-10-21 22:41:43.431;0.213305498"),
Create("2017-10-21 22:41:43.977;0.21357442 "),
Create("2017-10-21 22:41:44.525;0.213349506"),
Create("2017-10-21 22:41:45.070;0.207401217"),
        };
    }
}