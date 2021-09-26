using System;
using System.Collections.Generic;
using ReactiveUI;

namespace FutileGame.Models
{
    public sealed class Square : ReactiveObject
    {
        public int RowIndex { get; }
        public int ColumnIndex { get; }

        public Square(int row, int column)
        {
            RowIndex = row;
            ColumnIndex = column;
        }

        private readonly List<Square> _neighbours = new(4);
        public void SetNeighbours(IEnumerable<Square> neighbours)
        {
            if (IsInitialized())
                throw new InvalidOperationException("object already initialized");
            _neighbours.AddRange(neighbours);
        }

        public bool CanCheck => Value == 0;

        public void Check()
        {
            if (!IsInitialized())
                throw new InvalidOperationException("object not initialized");
            if (!CanCheck)
                throw new InvalidOperationException("square already checked");

            Value++;
            foreach (var sq in _neighbours)
            {
                sq.OnNeighbourChecked(this);
            }
        }

        public bool CanUncheck => Value == 1;

        public void Uncheck()
        {
            if (!IsInitialized())
                throw new InvalidOperationException("object not initialized");
            if (!CanUncheck)
                throw new InvalidOperationException("cannot uncheck");

            foreach (var sq in _neighbours)
            {
                sq.OnNeighbourUnchecked(this);
            }
            Value--;
        }

        private void OnNeighbourChecked(Square which)
        {
            if (!_neighbours.Contains(which))
                throw new InvalidOperationException("invalid neighbour");

            if (Value > 0)
            {
                Value++;
            }
        }

        private void OnNeighbourUnchecked(Square which)
        {
            if (!_neighbours.Contains(which))
                throw new InvalidOperationException("invalid neighbour");

            if (Value > 0)
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

        private bool IsInitialized() => _neighbours.Count > 0;
    }
}
