using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace FutileGame.Models
{
    public sealed class Round
    {
        private const int kTimePerTile = 3;

        public Round(Board objectiveBoard)
        {
            objectiveBoard.Freeze();
            ObjectiveBoard = objectiveBoard;
            PlayerBoard = Board.EmptyLike(ObjectiveBoard);

            var roundTime = ObjectiveBoard.CheckedCount * kTimePerTile;
            DueTime = DateTime.Now + TimeSpan.FromSeconds(roundTime);

            IsVictoryAchievedSeq = PlayerBoard.TileCheckedChanges
                .StartWith(null as Tile)
                .Any(_ => IsVictoryAchieved)
                .Timeout(DueTime, Observable.Return(false));
        }

        public DateTime DueTime { get; }

        public Board ObjectiveBoard { get; }
        public Board PlayerBoard { get; }

        public bool IsVictoryAchieved => PlayerBoard.IsAnyChecked && PlayerBoard.Equals(ObjectiveBoard);
        public IObservable<bool> IsVictoryAchievedSeq { get; }
    }
}
