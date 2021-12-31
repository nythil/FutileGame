using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FutileGame.Services;

namespace FutileGame.Models
{
    public sealed class Game
    {
        private readonly IObjectiveGenerator _objectiveGenerator;

        public Game(int numRows, int numColumns, IObjectiveGenerator objectiveGenerator)
        {
            _objectiveGenerator = objectiveGenerator;
            if (_objectiveGenerator is null)
                throw new ArgumentNullException(nameof(objectiveGenerator));
            RowCount = numRows;
            ColumnCount = numColumns;
            _roundChanges = new(null);
        }

        public int RowCount { get; }
        public int ColumnCount { get; }

        public Round Round
        {
            get => _roundChanges.Value;
            private set => _roundChanges.OnNext(value);
        }

        private readonly BehaviorSubject<Round> _roundChanges;
        public IObservable<Round> RoundChanges => _roundChanges.AsObservable();

        public bool IsVictoryAchieved => this.Round.IsVictoryAchieved;

        public IObservable<bool> IsVictoryAchievedChanges => this.Round.IsVictoryAchievedChanges;

        public void StartNewRound()
        {
            var numSteps = RowCount * ColumnCount / 2 + 1;
            var objective = _objectiveGenerator.Generate(RowCount, ColumnCount, numSteps);
            this.Round = new(objective);
        }
    }
}
