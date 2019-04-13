using System;
using System.Threading;

using SmoothDownloader.Download;

namespace SmoothDownloader
{
    /// <summary>
    /// </summary>
    internal class MuxingInteractiveState
    {
        /// <summary>
        /// </summary>
        private bool _hasDisplayedDuration;

        /// <summary>
        /// </summary>
        private ulong _reachedBaseTicks;

        /// <summary>
        /// </summary>
        private ulong _startTicks;

        /// <summary>
        /// </summary>
        private IStoppable _stoppable;

        /// <summary>
        /// </summary>
        private Thread _thread;

        /// <summary>
        /// </summary>
        public MuxingInteractiveState()
        {
            _hasDisplayedDuration = false;
            _startTicks = 0;
        }

        /// <summary>
        /// </summary>
        /// <param name="isLive"></param>
        /// <param name="stoppable"></param>
        public void SetupStop(bool isLive, IStoppable stoppable)
        {
            if (_thread != null)
            {
                throw new Exception();
            }

            if (isLive == false)
            {
                return;
            }

            _stoppable = stoppable;
            Console.WriteLine("Press any key to stop recording!");

            _thread = new Thread(() =>
            {
                Console.ReadKey(true);
                _stoppable.Stop();
            });

            _thread.Start();
        }

        /// <summary>
        /// </summary>
        public void Abort()
        {
            _thread?.Abort();
        }

        /// <summary>
        /// </summary>
        /// <param name="reachedTicks"></param>
        /// <param name="totalTicks"></param>
        public void DisplayDuration(ulong reachedTicks, ulong totalTicks)
        {
            if (_hasDisplayedDuration == false)
            {
                _hasDisplayedDuration = true;
                Console.Error.WriteLine("Recording duration:");
            }

            ulong timeLeft = 0;

            if (reachedTicks > 0)
            {
                var nowTicks = (ulong)DateTime.Now.Ticks;

                if (_startTicks == 0)
                {
                    _startTicks = nowTicks;
                    _reachedBaseTicks = reachedTicks;
                }
                else
                {
                    var etaDouble = (nowTicks - _startTicks + 0.0) * (totalTicks - reachedTicks) / (reachedTicks - _reachedBaseTicks);

                    if (etaDouble > 0.0 && etaDouble < 3.6e12)
                    {
                        timeLeft = (ulong)etaDouble;
                    }
                }
            }

            var message = "\r" + new TimeSpan((long)(reachedTicks - reachedTicks % 10000000));

            if (timeLeft == 0)
            {
                message += "              ";
            }
            else
            {
                message += ", ETA " + new TimeSpan((long)(timeLeft - timeLeft % 10000000));
            }

            Console.Write(message);
        }
    }
}