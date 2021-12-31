using System;
using System.Diagnostics;
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
        private readonly SerialDisposable _victorySub = new();

        public MainWindowViewModel(int numRows, int numColumns, ITileValueFormatter valueFormatter = null, IObjectiveGenerator objectiveGenerator = null)
        {
            objectiveGenerator ??= Locator.Current.GetService<IObjectiveGenerator>();
            _game = new Game(numRows, numColumns, objectiveGenerator);

            _playerBoard = _game.RoundChanges
                .WhereNotNull()
                .Select(r => new PlayerBoardViewModel(r.PlayerBoard, valueFormatter))
                .ToProperty(this, x => x.PlayerBoard, null as PlayerBoardViewModel);

            _objectiveBoard = _game.RoundChanges
                .WhereNotNull()
                .Select(r => new ObjectiveBoardViewModel(r.ObjectiveBoard, valueFormatter))
                .ToProperty(this, x => x.ObjectiveBoard, null as ObjectiveBoardViewModel);

            StartGame = ReactiveCommand.Create(() =>
            {
                IsGameStarted = false;
                _game.StartNewRound();
                IsGameStarted = true;
            });

            _victorySub.Disposable = _game.RoundChanges
                .WhereNotNull()
                .Select(r => r.IsVictoryAchievedChanges.SkipWhile(isVictory => !isVictory))
                .Switch()
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
