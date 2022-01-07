using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace FutileGame.Models
{
    public sealed class Round
    {
        private const int kTimePerTile = 3;

        private readonly CountdownTimer _timer;

        public Round(Board objectiveBoard)
        {
            objectiveBoard.Freeze();
            ObjectiveBoard = objectiveBoard;
            PlayerBoard = Board.EmptyLike(ObjectiveBoard);

            _timer = new(ObjectiveBoard.CheckedCount * kTimePerTile);

            IsVictoryAchievedSeq = PlayerBoard
                .TileCheckedChanges
                .StartWith(null as Tile)
                .Any(_ => IsVictoryAchieved)
                .Do(_ => _timer.Stop())
                .Amb(_timer.TimeoutSeq.Select(_ => false));
        }

        public Board ObjectiveBoard { get; }
        public Board PlayerBoard { get; }

        public bool IsVictoryAchieved => PlayerBoard.IsAnyChecked && PlayerBoard.Equals(ObjectiveBoard);
        public IObservable<bool> IsVictoryAchievedSeq { get; }

        public double GetRemainingTime() => _timer.GetRemainingTime();

        public void Start() => _timer.Start();
        public void Pause() => _timer.Pause();

        public bool IsStarted => _timer.IsStarted;
        public IObservable<bool> IsStartedSeq => _timer.IsStartedSeq;
    }
}
