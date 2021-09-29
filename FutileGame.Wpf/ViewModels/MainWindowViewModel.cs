using System;
using System.Reactive;
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

            GenerateObjective = ReactiveCommand.Create(() =>
            {
                var generator = Locator.Current.GetService<IObjectiveGenerator>();
                var board = generator.Generate(numRows, numColumns, numRows * numColumns / 2);
                ObjectiveBoard = new(board, valueFormatter);
                GameBoard = new(numRows, numColumns, valueFormatter);
            });
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

        public ReactiveCommand<Unit, Unit> GenerateObjective { get; }
    }
}
