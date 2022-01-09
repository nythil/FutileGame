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

        public MainWindowViewModel(int numRows, int numColumns, ITileValueFormatter? valueFormatter = null, IObjectiveGenerator? objectiveGenerator = null)
        {
            objectiveGenerator ??= Locator.Current.GetService<IObjectiveGenerator>()
                ?? throw new ArgumentNullException(nameof(objectiveGenerator));
            _game = new Game(numRows, numColumns, objectiveGenerator);

            _round = _game.RoundChanges
                .Select(r => r is null ? null : new RoundViewModel(r, valueFormatter))
                .ToProperty(this, x => x.Round);

            _isRoundStarted = this
                .WhenAnyValue(x => x.Round!.IsStarted)
                .StartWith(Round?.IsStarted ?? false)
                .ToProperty(this, x => x.IsRoundStarted);

            _isGameStarted = this
                .WhenAnyValue(x => x.Round, (RoundViewModel? r) => r is not null)
                .StartWith(false)
                .ToProperty(this, x => x.IsGameStarted);

            _score = _game
                .ScoreSeq
                .ToProperty(this, x => x.Score);

            NewGame = ReactiveCommand.Create(() =>
            {
                _game.StartNewRound();
            });

            _disposables.Add(this
                .WhenAnyObservable(x => x.Round!.IsVictoryAchievedSeq)
                .ObserveOnDispatcher()
                .Subscribe(async isVictory =>
                {
                    var startNewGame = await GameEnded.Handle(isVictory);
                    if (startNewGame)
                        Observable.Return(Unit.Default).InvokeCommand(NewGame);
                }));
        }

        private readonly ObservableAsPropertyHelper<bool> _isGameStarted;
        public bool IsGameStarted => _isGameStarted.Value;

        private readonly ObservableAsPropertyHelper<RoundViewModel?> _round;
        public RoundViewModel? Round => _round.Value;

        private readonly ObservableAsPropertyHelper<bool> _isRoundStarted;
        public bool IsRoundStarted => _isRoundStarted.Value;

        private readonly ObservableAsPropertyHelper<int> _score;
        public int Score => _score.Value;

        public ReactiveCommand<Unit, Unit> NewGame { get; }

        public Interaction<bool, bool> GameEnded { get; } = new();
    }
}
