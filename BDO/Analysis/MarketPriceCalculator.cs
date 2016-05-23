using System;
using System.Collections.Generic;
using System.Linq;
using BDO.Domain;
using BDO.Domain.Observation;

namespace BDO.Analysis
{
    /// <summary>
    /// Evaluates market history and determines a 'market' price based on current configuration.
    /// </summary>
    public static class MarketPriceCalculator
    {
        public static double MarketTax { get; set; } = .3;

        public static double CityTax { get; set; } = .05;

        /// <summary>
        /// The property to use for price evaluation.
        /// </summary>
        public static MarketPriceProperty MarketPriceProperty { get; set; } = MarketPriceProperty.Price;

        /// <summary>
        /// Gets or sets a value that only the most recent observation should be used for evaluation. If false,
        /// an average is calculated.
        /// </summary>
        public static bool UseMostRecent { get; set; } = true;

        /// <summary>
        /// The number of days an observation is considered valid. Observations older than this are not used
        /// in calculations. Use a value of 0 to consider all observations.
        /// </summary>
        public static int MaximumObservationAgeInDays { get; set; } = 0;

        /// <summary>
        /// Uses a weighted average to calcuate the market price, which prefers more recent data over older data.
        /// </summary>
        public static bool UseWeightedAverage { get; set; } = false;

        /// <summary>
        /// The number of days to apply full weight to.
        /// </summary>
        public static int WeightAverageFullContributionAgeInDays { get; set; }

        public static GapInterpolationMethod InterpolationMethod { get; set; }


        /// <summary>
        /// Gets the net income from selling this item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static int GetNetSaleRevenue(Item item)
        {
            return (int) (GetMarketPrice(item)*(1 - MarketTax - CityTax));
        }

        /// <summary>
        /// Gets the calculated market price for an item, based on the current configuration.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static int GetMarketPrice(Item item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Cannot determine market price for null item.");

            if (!item.AllMarketData.Any())
                return 0;

            if (UseMostRecent)
                return GetPriceFunction().Invoke(item.CurrentMarketData);

            Dictionary<DateTime, double> history;
            if (MaximumObservationAgeInDays == 0)
                history = MarketHistoryBuilder.BuildHistory(item.AllMarketData, InterpolationMethod, GetPriceFunction());
            else
                history = MarketHistoryBuilder.BuildHistory(item.AllMarketData, InterpolationMethod, GetPriceFunction(), MaximumObservationAgeInDays);

            //todo: weighted average
            if (UseWeightedAverage)
                return (int) GetWeightedAverage(history);

            return (int) GetStraightAverage(history);
        }

        /// <summary>
        /// Gets the items market history, by day, using the selected <see cref="InterpolationMethod"/>.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, double> GetFullMarketHistory(Item item)
        {
            return MarketHistoryBuilder.BuildHistory(item.AllMarketData, InterpolationMethod, GetPriceFunction());
        }


        static double GetStraightAverage(Dictionary<DateTime, double> history)
        {
            return history.Average(p => p.Value);
        }

        static double GetWeightedAverage(Dictionary<DateTime, double> history)
        {
            throw new NotImplementedException("Weighted averages not yet available.");
        }

        static Func<MarketObservation, int> GetPriceFunction()
        {
            switch (MarketPriceProperty)
            {
                case MarketPriceProperty.Price:
                    return (p) => p.Price;
                case MarketPriceProperty.LastSalePrice:
                    return p => p.LastSalePrice;
                case MarketPriceProperty.Low:
                    return p => p.Low;
                case MarketPriceProperty.High:
                    return p => p.High;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}