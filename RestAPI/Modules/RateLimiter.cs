using System;
using System.Runtime.Serialization;

namespace RestAPI.Modules
{
    /// <summary>
    /// Contains information about the limiter
    /// status like total burst count, remaining
    /// tickets and time of next ticket reset.
    /// </summary>
    [Serializable]
    public class Reservation
    {
        public int Burst;
        public int Remaining;
        public DateTime Reset;

        [IgnoreDataMember]
        public bool Success;
    }

    /// <summary>
    /// A rate limiter based on the Token bucket algorithm
    /// (https://en.wikipedia.org/wiki/Token_bucket).
    /// 
    /// Limit defines the time span after which new tokens
    /// are re-created.
    /// Burst defines the maximum ammount of tokens a bucket
    /// can keep and which can be taken at once.
    /// 
    /// Token regenerations occurs virtually. That means,
    /// the ammount of tokens to be added to the bucket is
    /// actually only calculated and added on request, which
    /// imlies, that the actual ammount of tokens in the
    /// limiters bucket only represents the last state of
    /// token ammount and not the actual current ammount
    /// of available tokens.
    /// </summary>
    public class RateLimiter
    {
        public TimeSpan Limit { get; private set; }
        public int Burst { get; private set; }
        public int Tokens { get; private set; }
        public DateTime Last { get; private set; }

        /// <summary>
        /// Creates a new instance of RateLimiter.
        /// </summary>
        /// <param name="limit">Limit defines time span in which new tokens will be generated</param>
        /// <param name="burst">Burst is the maximum ammount of tokens a bucket can contain</param>
        public RateLimiter(TimeSpan limit, int burst)
        {
            Limit = limit;
            Burst = burst;
            Tokens = burst;
        }

        /// <summary>
        /// Calculates and collects the burst ammount,
        /// the availavke ammount of tokens and the time
        /// until a new token is virtually generated.
        /// </summary>
        /// <param name="success">The success state of the reservation</param>
        /// <returns>Reservation instance</returns>
        private Reservation CalcReservation(bool success) =>
            new Reservation()
            {
                Burst = Burst,
                Remaining = Tokens,
                Reset = Last.Add(Limit),
                Success = success,
            };

        /// <summary>
        /// Takes the ammount n of requested tokens from the bucket and
        /// returns a Reservation state object. If the bucket does not 
        /// contain the requested ammount of tokens, the Success state
        /// of the reservation is false.
        /// </summary>
        /// <param name="n">The ammount of requested tokens (defaultly 1)</param>
        /// <returns>Reservation state</returns>
        public Reservation Reserve(int n = 1)
        {
            if (n <= 0)
            {
                return CalcReservation(true);
            }

            if (Last != default(DateTime))
            {
                var tokensSinceLast = (int)Math.Floor(DateTime.Now.Subtract(Last) / Limit);
                Tokens += tokensSinceLast;
            }

            if (Tokens > Burst)
            {
                Tokens = Burst;
            }

            if (Tokens >= n)
            {
                Tokens -= n;
                Last = DateTime.Now;

                return CalcReservation(true);
            }

            return CalcReservation(false);
        }

        /// <summary>
        /// This is an alias for Reserve(n) with only
        /// returning the reservation success state.
        /// </summary>
        /// <param name="n">The ammount of requested tokens (defaultly 1)</param>
        /// <returns>The reservation success state</returns>
        public bool Allow(int n = 1) =>
            Reserve(n).Success;
    }
}
