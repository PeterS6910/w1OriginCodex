using System.Diagnostics;

namespace Contal.IwQuick.Sys
{
    public class StopwatchPool : AObjectPool<Stopwatch>
    {
        protected override Stopwatch CreateObject()
        {
            return new Stopwatch();
        }

        private StopwatchPool()
        {
            
        }

        public static readonly StopwatchPool Singleton = new StopwatchPool();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Stopwatch MeasureStart()
        {
            bool newlyAdded;
            var sw = Get(out newlyAdded);

            if (!newlyAdded)
                sw.Reset();

            sw.Start();
            return sw;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stopWatch"></param>
        /// <returns>time elapsed in miliseconds</returns>
        public long MeasureStop(Stopwatch stopWatch)
        {
            if (null == stopWatch)
                return -1;
            
            stopWatch.Stop();
            var elapsedMs = stopWatch.ElapsedMilliseconds;

            base.Return(stopWatch);
            
            return elapsedMs;
        }

        public override void Return(Stopwatch returnedObject)
        {
            returnedObject.Stop();
            base.Return(returnedObject);
        }
    }

    public static class StopwatchExtensions
    {
        public static long MeasureStopAndReturn(this Stopwatch stopwatch)
        {
            return StopwatchPool.Singleton.MeasureStop(stopwatch);

        }
    }
}
