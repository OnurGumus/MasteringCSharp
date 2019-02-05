using NodaTime;
using System;
using System.Threading;

namespace Primitives
{
    class Program
    {
        static short squantity = 0;
        static int iquantity = 0;
        static long lquantity = 0;
        static void Main(string[] args)
        {
            var newQuantity  = Interlocked.Read(ref lquantity);

            var f = 0f;
            var d = 0d;
            var money = 0M;
            Console.WriteLine(0.3 * 3.0 + 0.1 == 1.0);
            Console.WriteLine(0.3M * 3.0M + 0.1M == 1.0M);


            var now = DateTime.Now;
            var nowWithOffset = DateTimeOffset.Now;
            var trueNow = SystemClock.Instance.GetCurrentInstant();
            ZonedDateTime nowInIsoUtc = trueNow.InUtc();
            var london = DateTimeZoneProviders.Tzdb["Europe/London"];
            var londonTime = trueNow.InZone(london);
            var localDate = new LocalDateTime(2019, 3, 31, 1, 45, 00);
            london.AtStrictly(localDate);
            var ticks = DateTime.Now.Ticks;
        }

        class Foo
        {
            public override int GetHashCode()
            {
                //chosen by a fair dice roll!
                return 4;
            }
        }
    }
}
