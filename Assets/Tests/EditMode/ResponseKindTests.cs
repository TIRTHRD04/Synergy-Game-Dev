using System.Collections.Generic;
using AdaptiveOpponent.Core;
using NUnit.Framework;

namespace AdaptiveOpponent.Tests
{
    /// <summary>The Opponent reports how it chose each response, so the UI never has to guess.</summary>
    public class ResponseKindTests
    {
        private static readonly GameDefinition Def = TestData.Example();

        [Test]
        public void WhileTheGateIsClosedTheResponseIsRandomAndThePlannedResponseIsTheBestResponse()
        {
            Opponent opponent = TestConfigs.Make(TestConfigs.V1());
            opponent.StartMatch();
            opponent.Decide();
            Assert.IsFalse(opponent.GateOpen);
            Assert.AreEqual(ResponseKind.Random, opponent.ResponseKind);
            Assert.AreEqual(0.0, opponent.Lambda);
            Assert.AreEqual(BestResponse.Of(Def, Forecast(opponent)), opponent.PlannedResponse);
        }

        [Test]
        public void MeanModeWithTheGateOpenPlaysThePlannedResponse()
        {
            var overrides = new Dictionary<string, string> { { "meanMode", "true" } };
            Opponent opponent = OpenGate(TestConfigs.Make(TestConfigs.V1(overrides)));
            int response = opponent.Decide();
            Assert.AreEqual(ResponseKind.BestResponse, opponent.ResponseKind);
            Assert.AreEqual(opponent.PlannedResponse, response);
            Assert.AreEqual(Def.AuthoredCounter(1), response, "the authored counter of the spammed action");
        }

        [Test]
        public void ThompsonSamplingWithTheGateOpenPlaysASampledBestResponse()
        {
            Opponent opponent = OpenGate(TestConfigs.Make(TestConfigs.V1()));
            opponent.Decide();
            Assert.AreEqual(ResponseKind.SampledBestResponse, opponent.ResponseKind);
        }

        [Test]
        public void SafeExploitationPlaysEitherThePlannedResponseOrARandomMove()
        {
            var overrides = new Dictionary<string, string>
            {
                { "selector", "\"safeExploitation\"" },
                { "safeBudgetStart", "3.0" },
            };
            Opponent opponent = OpenGate(TestConfigs.Make(TestConfigs.V1(overrides)));
            int counters = 0;
            for (int i = 0; i < 100; i++)
            {
                int response = opponent.Decide();
                Assert.AreNotEqual(ResponseKind.SampledBestResponse, opponent.ResponseKind);
                if (opponent.ResponseKind == ResponseKind.BestResponse)
                {
                    counters++;
                    Assert.AreEqual(opponent.PlannedResponse, response);
                }
                opponent.Observe(1);
            }
            Assert.Greater(counters, 0, "the planned response is played at least sometimes against a spammed action");
        }

        private static Opponent OpenGate(Opponent opponent)
        {
            opponent.StartMatch();
            for (int i = 0; i < 200 && !opponent.GateOpen; i++)
            {
                opponent.Decide();
                opponent.Observe(1);
            }
            Assert.IsTrue(opponent.GateOpen, "spamming one action opens the gate");
            return opponent;
        }

        private static double[] Forecast(Opponent opponent)
        {
            var forecast = new double[Def.ActionCount];
            for (int a = 0; a < forecast.Length; a++)
            {
                forecast[a] = opponent.ForecastOf(a);
            }
            return forecast;
        }
    }
}
