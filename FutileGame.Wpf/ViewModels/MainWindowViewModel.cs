using ReactiveUI;
using Splat;
using FutileGame.Services;

namespace FutileGame.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        public MainWindowViewModel(int numRows, int numColumns, ISquareValueFormatter valueFormatter = null)
        {
            _gameBoard = new GameBoardViewModel(numRows, numColumns, valueFormatter);
            _objectiveBoard = new ObjectiveBoardViewModel(numRows, numColumns, valueFormatter);
        }

        private GameBoardViewModel _gameBoard;
        public GameBoardViewModel GameBoard
        {
            get => _gameBoard;
            private set => this.RaiseAndSetIfChanged(ref _gameBoard, value);
        }

        private ObjectiveBoardViewModel _objectiveBoard;
        public ObjectiveBoardViewModel ObjectiveBoard
        {
            get => _objectiveBoard;
            private set => this.RaiseAndSetIfChanged(ref _objectiveBoard, value);
        }
    }
}
