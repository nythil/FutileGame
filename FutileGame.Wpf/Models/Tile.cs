using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Collections.Generic;
using ReactiveUI;

namespace FutileGame.Models
{
    public sealed class Tile : ReactiveObject, IEquatable<Tile>
    {
        public int RowIndex { get; }
        public int ColumnIndex { get; }

        public Tile(int row, int column)
        {
            RowIndex = row;
            ColumnIndex = column;

            _isChecked = this
                .WhenAnyValue(x => x.Value, v => v != 0)
                .ToProperty(this, x => x.IsChecked, scheduler: ImmediateScheduler.Instance);

            _canCheck = this
                .WhenAnyValue(x => x.Value, v => v == 0)
                .ToProperty(this, x => x.CanCheck, scheduler: ImmediateScheduler.Instance);

            _canUncheck = this
                .WhenAnyValue(x => x.Value, v => v == 1)
                .ToProperty(this, x => x.CanUncheck, scheduler: ImmediateScheduler.Instance);
        }

        private readonly List<Tile> _neighbours = new(4);
        public void SetNeighbours(IEnumerable<Tile> neighbours)
        {
            if (IsInitialized)
                throw new InvalidOperationException("object already initialized");
            _neighbours.AddRange(neighbours);
        }

        private readonly ObservableAsPropertyHelper<bool> _isChecked;
        public bool IsChecked => _isChecked.Value;

        private readonly ObservableAsPropertyHelper<bool> _canCheck;
        public bool CanCheck => _canCheck.Value;

        private readonly ObservableAsPropertyHelper<bool> _canUncheck;
        public bool CanUncheck => _canUncheck.Value;

        public void Check()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("object not initialized");
            if (!CanCheck)
                throw new InvalidOperationException("tile already checked");

            foreach (var sq in _neighbours)
            {
                sq.OnNeighbourChecked(this);
            }
            Value = 1;
        }

        public void Uncheck()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("object not initialized");
            if (!CanUncheck)
                throw new InvalidOperationException("cannot uncheck");

            foreach (var sq in _neighbours)
            {
                sq.OnNeighbourUnchecked(this);
            }
            Value = 0;
        }

        private void OnNeighbourChecked(Tile which)
        {
            if (!_neighbours.Contains(which))
                throw new InvalidOperationException("invalid neighbour");

            if (IsChecked)
            {
                Value++;
            }
        }

        private void OnNeighbourUnchecked(Tile which)
        {
            if (!_neighbours.Contains(which))
                throw new InvalidOperationException("invalid neighbour");

            if (IsChecked)
            {
                Value--;
            }
        }

        private int _value;
        public int Value
        {
            get => _value;
            private set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        private bool IsInitialized => _neighbours.Count > 0;

        public override bool Equals(object obj) => Equals(obj as Tile);

        public bool Equals(Tile other)
        {
            return other != null &&
                   RowIndex == other.RowIndex &&
                   ColumnIndex == other.ColumnIndex &&
                   Value == other.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RowIndex, ColumnIndex, Value);
        }
    }
}
