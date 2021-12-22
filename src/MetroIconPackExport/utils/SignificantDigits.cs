using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroIconPackExport.utils
{
    //from https://stackoverflow.com/questions/158172/formatting-numbers-with-significant-figures-in-c-sharp/1987721#1987721
    public static class Precision
    {
        // 2^-24
        public const float FLOAT_EPSILON = 0.0000000596046448f;

        // 2^-53
        public const double DOUBLE_EPSILON = 0.00000000000000011102230246251565d;

        public static bool AlmostEquals(this double a, double b, double epsilon = DOUBLE_EPSILON)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (a == b)
            {
                return true;
            }
            // ReSharper restore CompareOfFloatsByEqualityOperator

            return (System.Math.Abs(a - b) < epsilon);
        }

        public static bool AlmostEquals(this float a, float b, float epsilon = FLOAT_EPSILON)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (a == b)
            {
                return true;
            }
            // ReSharper restore CompareOfFloatsByEqualityOperator

            return (System.Math.Abs(a - b) < epsilon);
        }
    }

    public static class SignificantDigits
    {
        public static double Round(this double value, int significantDigits)
        {
            int unneededRoundingPosition;
            return RoundSignificantDigits(value, significantDigits, out unneededRoundingPosition);
        }

        public static string ToString(this double value, int significantDigits)
        {
            // this method will round and then append zeros if needed.
            // i.e. if you round .002 to two significant figures, the resulting number should be .0020.



            int roundingPosition;
            var roundedValue = RoundSignificantDigits(value, significantDigits, out roundingPosition);

            // when rounding causes a cascading round affecting digits of greater significance, 
            // need to re-round to get a correct rounding position afterwards
            // this fixes a bug where rounding 9.96 to 2 figures yeilds 10.0 instead of 10
            RoundSignificantDigits(roundedValue, significantDigits, out roundingPosition);

            if (Math.Abs(roundingPosition) > 9)
            {
                // use exponential notation format
                // ReSharper disable FormatStringProblem
                return string.Format("{0:E" + (significantDigits - 1) + "}", roundedValue);
                // ReSharper restore FormatStringProblem
            }

            // string.format is only needed with decimal numbers (whole numbers won't need to be padded with zeros to the right.)
            // ReSharper disable FormatStringProblem
            return roundingPosition > 0 ? string.Format("{0:F" + roundingPosition + "}", roundedValue) : roundedValue.ToString();
            // ReSharper restore FormatStringProblem
        }
        public static string ToString2(this double value, int significantDigits)
        {
            // this method will round and then not append zeros

            int roundingPosition;
            var roundedValue = RoundSignificantDigits(value, significantDigits, out roundingPosition);

            // when rounding causes a cascading round affecting digits of greater significance, 
            // need to re-round to get a correct rounding position afterwards
            // this fixes a bug where rounding 9.96 to 2 figures yeilds 10.0 instead of 10
            RoundSignificantDigits(roundedValue, significantDigits, out roundingPosition);

            if (Math.Abs(roundingPosition) > 9)
            {
                // use exponential notation format
                // ReSharper disable FormatStringProblem
                return string.Format("{0:E" + (significantDigits - 1) + "}", roundedValue);
                // ReSharper restore FormatStringProblem
            }

            // string.format is only needed with decimal numbers (whole numbers won't need to be padded with zeros to the right.)
            // ReSharper disable FormatStringProblem
            return roundedValue.ToString();
            // ReSharper restore FormatStringProblem
        }

        private static double RoundSignificantDigits(double value, int significantDigits, out int roundingPosition)
        {
            // this method will return a rounded double value at a number of signifigant figures.
            // the sigFigures parameter must be between 0 and 15, exclusive.

            roundingPosition = 0;

            if (value.AlmostEquals(0d))
            {
                roundingPosition = significantDigits - 1;
                return 0d;
            }

            if (double.IsNaN(value))
            {
                return double.NaN;
            }

            if (double.IsPositiveInfinity(value))
            {
                return double.PositiveInfinity;
            }

            if (double.IsNegativeInfinity(value))
            {
                return double.NegativeInfinity;
            }

            if (significantDigits < 1 || significantDigits > 15)
            {
                throw new ArgumentOutOfRangeException("significantDigits", value, "The significantDigits argument must be between 1 and 15.");
            }

            // The resulting rounding position will be negative for rounding at whole numbers, and positive for decimal places.
            roundingPosition = significantDigits - 1 - (int)(Math.Floor(Math.Log10(Math.Abs(value))));

            // try to use a rounding position directly, if no scale is needed.
            // this is because the scale mutliplication after the rounding can introduce error, although 
            // this only happens when you're dealing with really tiny numbers, i.e 9.9e-14.
            if (roundingPosition > 0 && roundingPosition < 16)
            {
                return Math.Round(value, roundingPosition, MidpointRounding.AwayFromZero);
            }

            // Shouldn't get here unless we need to scale it.
            // Set the scaling value, for rounding whole numbers or decimals past 15 places
            var scale = Math.Pow(10, Math.Ceiling(Math.Log10(Math.Abs(value))));

            return Math.Round(value / scale, significantDigits, MidpointRounding.AwayFromZero) * scale;
        }
    }
}