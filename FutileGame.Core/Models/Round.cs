using ReactiveUI;

namespace FutileGame.Models
{
    public sealed class Round : ReactiveObject
    {
        public Round(Board objectiveBoard)
        {
            _objectiveBoard = objectiveBoard;
            _playerBoard = Board.EmptyLike(_objectiveBoard);
        }

        private readonly Board _objectiveBoard;
        public Board ObjectiveBoard => _objectiveBoard;

        private readonly Board _playerBoard;
        public Board PlayerBoard => _playerBoard;

        public bool IsVictoryAchieved()
        {
            return _playerBoard.IsAnyChecked && _playerBoard.Equals(_objectiveBoard);
        }
    }
}
