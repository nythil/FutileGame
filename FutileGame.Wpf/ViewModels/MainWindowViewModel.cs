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

            _round = _game.RoundChanges
                .Select(r => r is null ? null : new RoundViewModel(r, valueFormatter))
                .ToProperty(this, x => x.Round);

            _isRoundStarted = this
                .WhenAnyValue(x => x.Round.IsStarted)
                .StartWith(Round?.IsStarted ?? false)
                .ToProperty(this, x => x.IsRoundStarted);

            StartGame = ReactiveCommand.Create(() =>
            {
                _game.StartNewRound();
            });

            _disposables.Add(this
                .WhenAnyObservable(x => x.Round.IsVictoryAchievedSeq)
                .Subscribe(async isVictory =>
                {
                    var startNewGame = await GameEnded.Handle(isVictory);
                    if (startNewGame)
                        Observable.Return(Unit.Default).InvokeCommand(StartGame);
                }));
        }

        private readonly ObservableAsPropertyHelper<RoundViewModel> _round;
        public RoundViewModel Round => _round.Value;

        private readonly ObservableAsPropertyHelper<bool> _isRoundStarted;
        public bool IsRoundStarted => _isRoundStarted.Value;

        public ReactiveCommand<Unit, Unit> StartGame { get; }

        public Interaction<bool, bool> GameEnded { get; } = new();
    }
}
