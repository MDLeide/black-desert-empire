using System;
using System.Collections.Generic;
using System.Linq;
using BDO.Domain.Observation;

namespace BDO.Analysis
{
    static class MarketHistoryBuilder
    {
        public static Dictionary<DateTime, double> BuildHistory(IEnumerable<MarketObservation> observations, GapInterpolationMethod interpolationMethod, Func<MarketObservation, int> getPriceFunction)
        {
            switch (interpolationMethod)
            {
                case GapInterpolationMethod.Linear:
                    return LinearInterpolation(observations.ToArray(), getPriceFunction);
                default:
                    throw new ArgumentOutOfRangeException(nameof(interpolationMethod), interpolationMethod, null);
            }
        }

        public static Dictionary<DateTime, double> BuildHistory(IEnumerable<MarketObservation> observations, GapInterpolationMethod interpolationMethod, Func<MarketObservation, int> getPriceFunction, int maxDays)
        {
            return BuildHistory(observations, interpolationMethod, getPriceFunction).Take(maxDays).ToDictionary(p => p.Key, p => p.Value);
        }

        static Dictionary<DateTime, double> LinearInterpolation(IEnumerable<MarketObservation> observations, Func<MarketObservation, int> getPriceFunction)
        {
            var dates = GetAllDates(observations);
            bool open = false;
            int openIndex = 0;
            int openCount = 0;
            double last = 0;
            var results = new Dictionary<DateTime, double>();

            for (int i = 0; i < dates.Length; i++)
            {
                var o = observations.Where(p => p.EntryTime.Date == dates[i]).ToArray();
                if (!o.Any())
                {
                    openCount++;
                    if (open)
                        continue;
                    open = true;
                    openIndex = i;
                }
                else
                {
                    var avg = o.Average(getPriceFunction);
                    if (open)
                    {
                        var dif = (avg - last);
                        double increment;
                        if (dif == 0)
                            increment = 0;
                        else
                            increment = dif/(openCount + 1);

                        for (int j = openIndex; j < i; j++)
                        {
                            last = last + increment;
                            results.Add(dates[j], last);
                        }

                        open = false;
                    }
                    results.Add(dates[i], avg);
                    last = avg;
                }
            }

            return results;
        }

        static DateTime[] GetAllDates(IEnumerable<MarketObservation> observations)
        {
            if (!observations.Any())
                return new DateTime[] {};

            var start = observations.Select(p => p.EntryTime).Min().Date;
            var end = observations.Select(p => p.EntryTime).Max().Date;

            var dayCount = (end - start).Days + 1;
            var dates = new DateTime[dayCount];

            for (int i = 0; i < dayCount; i++)
                dates[i] = start.AddDays(i);

            return dates;
        }
    }
}