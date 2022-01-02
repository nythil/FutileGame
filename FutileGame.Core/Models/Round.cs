using System;
using System.Reactive.Linq;

namespace FutileGame.Models
{
    public sealed class Round
    {
        public Round(Board objectiveBoard)
        {
            objectiveBoard.Freeze();
            ObjectiveBoard = objectiveBoard;
            PlayerBoard = Board.EmptyLike(ObjectiveBoard);

            IsVictoryAchievedSeq = PlayerBoard.TileCheckedChanges
                .StartWith(null as Tile)
                .Any(_ => IsVictoryAchieved);
        }

        public Board ObjectiveBoard { get; }
        public Board PlayerBoard { get; }

        public bool IsVictoryAchieved => PlayerBoard.IsAnyChecked && PlayerBoard.Equals(ObjectiveBoard);
        public IObservable<bool> IsVictoryAchievedSeq { get; }
    }
}
