using System;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using FutileGame.Services;
using FutileGame.Models;

namespace FutileGame.ViewModels
{
    public class RoundViewModel : ReactiveObject
    {
        private readonly Round _round;

        public RoundViewModel(Round round, ITileValueFormatter? valueFormatter = null)
        {
            _round = round ?? throw new ArgumentNullException(nameof(round));

            PlayerBoard = new PlayerBoardViewModel(_round.PlayerBoard, valueFormatter);
            ObjectiveBoard = new ObjectiveBoardViewModel(_round.ObjectiveBoard, valueFormatter);

            _isStarted = _round
                .IsStartedSeq
                .ObserveOnDispatcher()
                .ToProperty(this, x => x.IsStarted);

            _timeRemaining = Observable
                .Interval(TimeSpan.FromMilliseconds(73))
                .Select(_ => _round.GetRemainingTime())
                .TakeUntil(t => t == 0 || _round.IsVictoryAchieved)
                .ObserveOnDispatcher()
                .ToProperty(this, x => x.TimeRemaining);

            StartRound = ReactiveCommand.Create(
                canExecute: _round.IsStartedSeq.Select(isStarted => !isStarted).Append(false).ObserveOnDispatcher(),
                execute: () => { _round.Start(); }
            );

            PauseRound = ReactiveCommand.Create(
                canExecute: _round.IsStartedSeq.ObserveOnDispatcher(),
                execute: () => { _round.Pause(); }
            );
        }

        public ReactiveCommand<Unit, Unit> StartRound { get; }
        public ReactiveCommand<Unit, Unit> PauseRound { get; }

        private readonly ObservableAsPropertyHelper<bool> _isStarted;
        public bool IsStarted => _isStarted.Value;

        private readonly ObservableAsPropertyHelper<double> _timeRemaining;
        public double TimeRemaining => _timeRemaining.Value;

        public PlayerBoardViewModel PlayerBoard { get; }
        public ObjectiveBoardViewModel ObjectiveBoard { get; }

        public IObservable<bool> IsVictoryAchievedSeq => _round.IsVictoryAchievedSeq;
    }
}
