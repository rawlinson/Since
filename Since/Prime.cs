using System.Diagnostics.Contracts;

namespace Since
{
    /// <summary>
    /// 
    /// </summary>
    public static class Prime
    {
        /// <summary>
        /// 
        /// </summary>
        public static uint[] Primes { get; } = {
            3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
            1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
            17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
            187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
            1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369 };
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsPrime(ulong value)
        {
            if (value <= 3 && value > 1)
                return true;

            if (value % 2 == 0 || value % 3 == 0)
                return false;

            foreach (var prime in Prime.Primes)
            {
                if (prime == value)
                    return true;
            }

            return Prime.IsLargePrime(value);
        }

        private static bool TryGetLeastFactor(uint value, out uint result)
        {
            if (value == 2 || value == 3)
            {
                result = value;
                return true;
            }

            result = default(uint);
            if (value % 2 == 0 || value % 3 == 0)
                return false;

            foreach (var prime in Prime.Primes)
            {
                if (prime != value)
                    continue;
                result = prime;
                return true;
            }

            return Prime.IsLargePrime(value);
        }

        private static bool IsLargePrime(ulong value)
        {
            if (value % 2 == 0 || value % 3 == 0)
                return false;

            for (ulong i = 5; i * i <= value; i += 6)
            {
                if (value % i == 0 || value % (i + 2) == 0)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the prime that is greater than or equal to the supplied number.
        /// </summary>
        /// <param name="min"></param>
        /// <returns></returns>
        public static int GetNextPrime(int min)
        {
            Contract.Requires(min >= 0);

            foreach (var prime in Prime.Primes)
            {
                if (prime >= min)
                    return (int)prime;
            }
 
            for (var i = (min | 1); i < int.MaxValue; i += 2)
            {
                if (Prime.IsLargePrime((ulong)i) && ((i - 1) % 101 != 0))
                    return i;
            }

            return min;
        }
        
    }
}
