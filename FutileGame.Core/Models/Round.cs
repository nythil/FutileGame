using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace FutileGame.Models
{
    public sealed class Round
    {
        public Round(Board objectiveBoard)
        {
            objectiveBoard.Freeze();
            ObjectiveBoard = objectiveBoard;
            PlayerBoard = Board.EmptyLike(ObjectiveBoard);
        }

        public Board ObjectiveBoard { get; }
        public Board PlayerBoard { get; }

        public bool IsVictoryAchieved => PlayerBoard.IsAnyChecked && PlayerBoard.Equals(ObjectiveBoard);

        public IObservable<bool> IsVictoryAchievedChanges
        {
            get => PlayerBoard.TileCheckedChanges.StartWith(null as Tile).Select(_ => IsVictoryAchieved).DistinctUntilChanged();
        }
    }
}
