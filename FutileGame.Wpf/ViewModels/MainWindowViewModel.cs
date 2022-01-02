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
        private readonly CompositeDisposable _disposables = new();

        public MainWindowViewModel(int numRows, int numColumns, ITileValueFormatter valueFormatter = null, IObjectiveGenerator objectiveGenerator = null)
        {
            objectiveGenerator ??= Locator.Current.GetService<IObjectiveGenerator>();
            valueFormatter ??= Locator.Current.GetService<ITileValueFormatter>();
            _game = new Game(numRows, numColumns, objectiveGenerator);

            var roundSeq = _game.RoundChanges
                .Select(r => r is null ? null : new RoundViewModel(r, valueFormatter))
                .Publish();

            _playerBoard = roundSeq
                .Select(r => r?.PlayerBoard)
                .ToProperty(this, x => x.PlayerBoard);

            _objectiveBoard = roundSeq
                .Select(r => r?.ObjectiveBoard)
                .ToProperty(this, x => x.ObjectiveBoard);

            StartGame = ReactiveCommand.Create(() =>
            {
                _game.StartNewRound();
            });

            _isGameStarted = roundSeq
                .WhereNotNull()
                .Select(r => r.IsVictoryAchievedSeq.IsEmpty().StartWith(true))
                .Switch()
                .ToProperty(this, x => x.IsGameStarted, () => false);

            _disposables.Add(roundSeq
                .WhereNotNull()
                .Select(r => r.IsVictoryAchievedSeq)
                .Switch()
                .Subscribe(async isVictory =>
                {
                    var startNewGame = await GameEnded.Handle(isVictory);
                    if (startNewGame)
                        Observable.Return(Unit.Default).InvokeCommand(StartGame);
                }));

            _disposables.Add(roundSeq.Connect());
        }

        private readonly ObservableAsPropertyHelper<bool> _isGameStarted;
        public bool IsGameStarted => _isGameStarted.Value;

        private readonly ObservableAsPropertyHelper<PlayerBoardViewModel> _playerBoard;
        public PlayerBoardViewModel PlayerBoard => _playerBoard.Value;

        private readonly ObservableAsPropertyHelper<ObjectiveBoardViewModel> _objectiveBoard;
        public ObjectiveBoardViewModel ObjectiveBoard => _objectiveBoard.Value;

        public ReactiveCommand<Unit, Unit> StartGame { get; }

        public Interaction<bool, bool> GameEnded { get; } = new();
    }
}
