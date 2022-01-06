using System;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using FutileGame.Services;
using FutileGame.Models;

namespace FutileGame.ViewModels
{
    public class RoundViewModel : ReactiveObject
    {
        private readonly Round _round;

        public RoundViewModel(Round round, ITileValueFormatter valueFormatter = null)
        {
            _round = round;
            if (_round is null)
                throw new ArgumentNullException(nameof(round));

            PlayerBoard = new PlayerBoardViewModel(_round.PlayerBoard, valueFormatter);
            ObjectiveBoard = new ObjectiveBoardViewModel(_round.ObjectiveBoard, valueFormatter);

            _isStarted = _round
                .IsVictoryAchievedSeq.IsEmpty()
                .ToProperty(this, x => x.IsStarted, () => true);
        }

        private readonly ObservableAsPropertyHelper<bool> _isStarted;
        public bool IsStarted => _isStarted.Value;

        public PlayerBoardViewModel PlayerBoard { get; }
        public ObjectiveBoardViewModel ObjectiveBoard { get; }

        public IObservable<bool> IsVictoryAchievedSeq => _round.IsVictoryAchievedSeq;
    }
}
