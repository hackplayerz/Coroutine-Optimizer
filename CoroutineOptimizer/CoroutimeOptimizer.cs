namespace CoroutineOptimizer
{
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// Optimized coroutine.
    /// </summary>
    public static class CoroutineOptimizer
    {
        //----------------------------------------------------
        //	IEqualityComparer
        //	[ 참고: https://libsora.so/posts/csharp-dictionary-enum-key-without-gc/ ]
        class FloatComparer : IEqualityComparer<float>
        {
            bool IEqualityComparer<float>.Equals(float x, float y) { return x == y; }
            int IEqualityComparer<float>.GetHashCode(float obj) { return obj.GetHashCode(); }

        }//	class FloatComparer : IEqualityComparer<float>
         //----------------------------------------------------

        public static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
        public static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();
        private static readonly Dictionary<float, WaitForSeconds> _timeInterval = new Dictionary<float,WaitForSeconds>(new FloatComparer());

        /// <summary>
        /// Optimized wait for second function.
        /// </summary>
        /// <param name="seconds">For waiting time</param>
        /// <returns>Routine.</returns>
        public static WaitForSeconds WaitForSeconds(float seconds)
        {
            WaitForSeconds routine;
            if(!_timeInterval.TryGetValue(seconds,out routine))
            {
                _timeInterval.Add(seconds,routine = new WaitForSeconds(seconds));
            }
            return routine;
        }
    }
}