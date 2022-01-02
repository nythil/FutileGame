using System;
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
        }

        public PlayerBoardViewModel PlayerBoard { get; }
        public ObjectiveBoardViewModel ObjectiveBoard { get; }

        public IObservable<bool> IsVictoryAchievedSeq => _round.IsVictoryAchievedSeq;
    }
}
