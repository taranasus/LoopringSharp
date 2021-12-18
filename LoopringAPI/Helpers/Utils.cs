namespace LoopringAPI
{
    public static class Utils
    {
        public static string IntervalsEnumToString(Intervals interval)
        {
            var intervals = "";
            switch (interval)
            {
                case Intervals.min1:
                case Intervals.min15:
                case Intervals.min30:
                case Intervals.min5:
                    intervals = interval.ToString().Replace("min", "") + "min";
                    break;
                case Intervals.hr4:
                case Intervals.hr2:
                case Intervals.hr1:
                case Intervals.hr12:
                    intervals = interval.ToString().Replace("hr", "") + "hr";
                    break;
                case Intervals.w1:
                    intervals = "1w";
                    break;
                case Intervals.d1:
                    intervals = "1d";
                    break;
            }
            return intervals;
        }
    }
}
