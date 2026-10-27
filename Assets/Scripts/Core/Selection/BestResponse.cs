namespace AdaptiveOpponent.Core
{
    /// <summary>
    /// The best response to a forecast: the response with the highest expected payoff, sum_a p(a) U(r, a).
    /// Ties go to the authored counter of the most likely action, then to the lower index (ADR 0002).
    /// </summary>
    public static class BestResponse
    {
        /// <summary>Expected payoffs within this distance count as tied.</summary>
        public const double TieTolerance = 1e-12;

        /// <summary>The best response to <paramref name="forecast"/> (one probability per action).</summary>
        public static int Of(GameDefinition definition, double[] forecast)
        {
            int top = TopAction(forecast, definition.ActionCount);
            int authored = definition.AuthoredCounter(top);
            int best = -1;
            double bestValue = double.NegativeInfinity;
            for (int r = 0; r < definition.ResponseCount; r++)
            {
                double value = ExpectedPayoff(definition, forecast, r);
                if (value > bestValue + TieTolerance)
                {
                    best = r;
                    bestValue = value;
                }
                else if (value >= bestValue - TieTolerance && r == authored)
                {
                    best = r;
                }
            }
            return best;
        }

        /// <summary>The expected payoff of <paramref name="response"/> against <paramref name="forecast"/>.</summary>
        public static double ExpectedPayoff(GameDefinition definition, double[] forecast, int response)
        {
            double value = 0;
            for (int a = 0; a < definition.ActionCount; a++)
            {
                value += forecast[a] * definition.Payoff(response, a);
            }
            return value;
        }

        /// <summary>The most likely action in <paramref name="forecast"/>; ties go to the lower index.</summary>
        public static int TopAction(double[] forecast, int count)
        {
            int top = 0;
            for (int a = 1; a < count; a++)
            {
                if (forecast[a] > forecast[top] + TieTolerance)
                {
                    top = a;
                }
            }
            return top;
        }
    }
}
