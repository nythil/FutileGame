using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace FutileGame.Models
{
    public sealed class Tile : IEquatable<Tile>
    {
        public int RowIndex { get; }
        public int ColumnIndex { get; }

        public Tile(int row, int column)
        {
            RowIndex = row;
            ColumnIndex = column;
            _valueChanges = new BehaviorSubject<int>(0);
        }

        public int Value
        {
            get => _valueChanges.Value;
            private set => _valueChanges.OnNext(value);
        }

        private readonly BehaviorSubject<int> _valueChanges;
        public IObservable<int> ValueChanges => _valueChanges.AsObservable();

        public bool IsChecked => Value != 0;
        public IObservable<bool> IsCheckedChanges => _valueChanges.Select(_ => IsChecked).DistinctUntilChanged();

        public bool CanCheck => Value == 0;
        public IObservable<bool> CanCheckChanges => _valueChanges.Select(_ => CanCheck).DistinctUntilChanged();

        public bool CanUncheck => Value == 1;
        public IObservable<bool> CanUncheckChanges => _valueChanges.Select(_ => CanUncheck).DistinctUntilChanged();

        public void Check()
        {
            if (!CanCheck)
                throw new InvalidOperationException("tile already checked");
            Value = 1;
        }

        public void Uncheck()
        {
            if (!CanUncheck)
                throw new InvalidOperationException("tile not checked");
            Value = 0;
        }

        public void Increment()
        {
            if (IsChecked)
            {
                Value++;
            }
        }

        public void Decrement()
        {
            if (IsChecked)
            {
                Debug.Assert(Value > 1);
                Value--;
            }
        }

        public void Freeze()
        {
            _valueChanges.OnCompleted();
        }

        public override bool Equals(object? obj) => Equals(obj as Tile);

        public bool Equals(Tile? other)
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

        public override string ToString() => $"Tile({RowIndex},{ColumnIndex})";
    }
}
