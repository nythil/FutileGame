using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace FutileGame.Models
{
    public sealed class CountdownTimer
    {
        private readonly SerialDisposable _timeoutDisposable = new();
        private readonly BehaviorSubject<DateTimeOffset?> _dueTimeSeq = new(null);
        private double _remainingTime;

        public CountdownTimer(double initialTime)
        {
            _remainingTime = initialTime;

            IObservable<Unit> MakeTimerSeq(DateTimeOffset? dueTime)
            {
                if (dueTime.HasValue) // timer active
                    return Observable.Timer(dueTime.Value).Select(_ => Unit.Default);
                if (_remainingTime > 0) // timer paused or stopped
                    return Observable.Never<Unit>();
                return Observable.Empty<Unit>(); // time has run out
            }

            var timeoutSeq = _dueTimeSeq
                .Select(MakeTimerSeq)
                .Switch()
                .Do(_ => StopInternal(0))
                .DefaultIfEmpty()
                .PublishLast();
            TimeoutSeq = timeoutSeq;
            _timeoutDisposable.Disposable = timeoutSeq.Connect();
        }

        public double GetRemainingTime()
        {
            var dueTime = _dueTimeSeq.Value;
            if (dueTime.HasValue)
                return Math.Max(0, (dueTime.Value - DateTimeOffset.Now).TotalSeconds);
            return _remainingTime;
        }

        public IObservable<double> RemainingTimeLeapSeq => _dueTimeSeq.Select(_ => GetRemainingTime());

        public bool IsStarted => _dueTimeSeq.Value is not null;
        public IObservable<bool> IsStartedSeq => _dueTimeSeq.Select(_ => IsStarted).DistinctUntilChanged();

        public bool IsStopped => _dueTimeSeq.Select(_ => false).Append(true).Take(1).Wait();

        /// <summary>
        /// Completes with a single item when time runs out.
        /// </summary>
        public IObservable<Unit> TimeoutSeq { get; }

        public void Start()
        {
            if (IsStarted)
                return;
            if (_remainingTime <= 0)
                throw new InvalidOperationException("no time left");
            RestartInternal();
        }

        private void RestartInternal()
        {
            var dueTime = DateTimeOffset.Now + TimeSpan.FromSeconds(_remainingTime);
            _dueTimeSeq.OnNext(dueTime);
        }

        public void Pause()
        {
            if (!IsStarted)
                return;
            _remainingTime = GetRemainingTime();
            _dueTimeSeq.OnNext(null);
        }

        public void Stop()
        {
            if (!IsStarted)
                return;
            StopInternal(GetRemainingTime());
        }

        private void StopInternal(double remainingTime)
        {
            _remainingTime = remainingTime;
            _dueTimeSeq.OnNext(null);
            _dueTimeSeq.OnCompleted();
        }

        public void Decrement(double amount)
        {
            var newRemainingTime = GetRemainingTime() - amount;
            if (newRemainingTime <= 0)
            {
                StopInternal(0);
                return;
            }
            _remainingTime = newRemainingTime;

            if (IsStarted)
            {
                RestartInternal();
            }
        }
    }
}
