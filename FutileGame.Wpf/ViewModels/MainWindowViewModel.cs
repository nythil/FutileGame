using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using ReactiveUI;
using Splat;
using FutileGame.Services;
using FutileGame.Models;

namespace FutileGame.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private readonly Game _game;
        private readonly IObjectiveGenerator _objectiveGenerator;
        private readonly SerialDisposable _victorySub = new();

        public MainWindowViewModel(int numRows, int numColumns, ISquareValueFormatter valueFormatter = null, IObjectiveGenerator objectiveGenerator = null)
        {
            _game = new Game(numRows, numColumns);
            _objectiveGenerator = objectiveGenerator ?? Locator.Current.GetService<IObjectiveGenerator>();

            _playerBoard = _game
                .WhenAnyValue(g => g.PlayerBoard)
                .Select(g => new PlayerBoardViewModel(g, valueFormatter))
                .ToProperty(this, x => x.PlayerBoard);

            _objectiveBoard = _game
                .WhenAnyValue(g => g.ObjectiveBoard)
                .Select(g => new ObjectiveBoardViewModel(g, valueFormatter))
                .ToProperty(this, x => x.ObjectiveBoard);

            StartGame = ReactiveCommand.Create(() =>
            {
                IsGameStarted = false;
                _game.StartNewGame(_objectiveGenerator);
                IsGameStarted = true;
            });

            _victorySub.Disposable = this
                .WhenAnyObservable(vm => vm.PlayerBoard.SquareToggledObs)
                .Where(_ => _game.IsVictoryAchieved())
                .Subscribe(async _ =>
                {
                    IsGameStarted = false;
                    var startNewGame = await GameEnded.Handle(true);
                    if (startNewGame)
                        Observable.Return(Unit.Default).InvokeCommand(StartGame);
                });
        }

        private bool _isGameStarted = false;
        public bool IsGameStarted
        {
            get => _isGameStarted;
            private set => this.RaiseAndSetIfChanged(ref _isGameStarted, value);
        }

        private readonly ObservableAsPropertyHelper<PlayerBoardViewModel> _playerBoard;
        public PlayerBoardViewModel PlayerBoard => _playerBoard.Value;

        private readonly ObservableAsPropertyHelper<ObjectiveBoardViewModel> _objectiveBoard;
        public ObjectiveBoardViewModel ObjectiveBoard => _objectiveBoard.Value;

        public ReactiveCommand<Unit, Unit> StartGame { get; }

        public Interaction<bool, bool> GameEnded { get; } = new();
    }
}
