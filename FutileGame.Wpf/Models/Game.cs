using ReactiveUI;
using FutileGame.Services;

namespace FutileGame.Models
{
    public sealed class Game : ReactiveObject
    {
        public Game(int numRows, int numColumns)
        {
            _objectiveBoard = new(numRows, numColumns);
            _playerBoard = new(numRows, numColumns);
        }

        private Board _objectiveBoard;
        public Board ObjectiveBoard
        {
            get => _objectiveBoard;
            private set => this.RaiseAndSetIfChanged(ref _objectiveBoard, value);
        }

        private Board _playerBoard;
        public Board PlayerBoard
        {
            get => _playerBoard;
            private set => this.RaiseAndSetIfChanged(ref _playerBoard, value);
        }

        public bool IsVictoryAchieved()
        {
            return _playerBoard.IsAnyChecked && _playerBoard.Equals(_objectiveBoard);
        }

        public void StartNewGame(IObjectiveGenerator objectiveGenerator)
        {
            using (this.DelayChangeNotifications())
            {
                var rowCount = ObjectiveBoard.RowCount;
                var columnCount = ObjectiveBoard.ColumnCount;
                ObjectiveBoard = objectiveGenerator.Generate(rowCount, columnCount, 15);
                PlayerBoard = new(rowCount, columnCount);
            }
        }
    }
}
