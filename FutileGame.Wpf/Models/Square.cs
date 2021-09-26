using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
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
            Debug.Assert(_neighbours.Count == 0);
            _neighbours.AddRange(neighbours);
        }

        public IObservable<bool> CanCheck => this.WhenAnyValue(x => x.Value).Select(v => v == 0);

        public void Check()
        {
            Debug.Assert(Value == 0);
            Debug.Assert(_neighbours.Count > 0);
            Value++;
            foreach (var sq in _neighbours)
            {
                sq.Increment();
            }
        }

        private void Increment()
        {
            if (Value > 0)
            {
                Value++;
            }
        }

        private int _value;
        public int Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }
    }
}
