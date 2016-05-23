using System;
using System.Collections.Generic;

namespace BDO.Analysis
{
    static class WeightedAverageHelper
    {
        public static double GetWeightAverage(IEnumerable<Tuple<double, int>> valueToObservations)
        {
            int totalObservations = 0;
            double valueSum = 0;
            foreach (var pair in valueToObservations)
            {
                totalObservations += pair.Item2;
                valueSum = pair.Item1*pair.Item2;
            }
            return valueSum/totalObservations;
        }
    }
}