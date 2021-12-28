using System;
using ReactiveUI;
using FutileGame.Services;

namespace FutileGame.Models
{
    public sealed class Game : ReactiveObject
    {
        private readonly IObjectiveGenerator _objectiveGenerator;

        public Game(int numRows, int numColumns, IObjectiveGenerator objectiveGenerator)
        {
            _objectiveGenerator = objectiveGenerator;
            if (_objectiveGenerator is null)
                throw new ArgumentNullException(nameof(objectiveGenerator));
            RowCount = numRows;
            ColumnCount = numColumns;
        }

        public int RowCount { get; }
        public int ColumnCount { get; }

        private Round _round;
        public Round Round
        {
            get => _round;
            private set => this.RaiseAndSetIfChanged(ref _round, value);
        }

        public bool IsVictoryAchieved()
        {
            return _round.IsVictoryAchieved();
        }

        public void StartNewRound()
        {
            var numSteps = RowCount * ColumnCount / 2 + 1;
            var objective = _objectiveGenerator.Generate(RowCount, ColumnCount, numSteps);
            this.Round = new(objective);
        }
    }
}
