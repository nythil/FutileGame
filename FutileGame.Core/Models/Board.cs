using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace FutileGame.Models
{
    public sealed class Board : IEquatable<Board>
    {
        private readonly CompositeDisposable _disposables = new();

        public Board(int numRows, int numColumns)
        {
            RowCount = numRows;
            ColumnCount = numColumns;

            _tiles = Enumerable
                .Range(0, RowCount)
                .SelectMany(r => Enumerable
                    .Range(0, ColumnCount)
                    .Select(c => new Tile(r, c)))
                .ToList();
            Debug.Assert(_tiles.Count == RowCount * ColumnCount);

            _tileCheckedChanges = _tiles
                .Select(t => t.IsCheckedChanges.Skip(1).Select(_ => t))
                .Merge()
                .Publish();

            var checkedNeighboursSeq = _tileCheckedChanges
                .Where(t => t.IsChecked)
                .SelectMany(t => GetNeighbours(t));
            _disposables.Add(checkedNeighboursSeq.Subscribe(t => t?.Increment()));

            var uncheckedNeighboursSeq = _tileCheckedChanges
                .Where(t => !t.IsChecked)
                .SelectMany(t => GetNeighbours(t));
            _disposables.Add(uncheckedNeighboursSeq.Subscribe(t => t?.Decrement()));

            _disposables.Add(_tileCheckedChanges.Connect());
        }

        public static Board Empty(int numRows, int numColumns)
        {
            return new Board(numRows, numColumns);
        }

        public static Board EmptyLike(Board board)
        {
            return Empty(board.RowCount, board.ColumnCount);
        }

        public int RowCount { get; }
        public int ColumnCount { get; }

        private readonly List<Tile> _tiles;
        public IReadOnlyCollection<Tile> Tiles => _tiles.AsReadOnly();

        public bool IsAnyChecked => Tiles.Any(x => x.IsChecked);

        private readonly IConnectableObservable<Tile> _tileCheckedChanges;
        public IObservable<Tile> TileCheckedChanges => _tileCheckedChanges;

        public Tile GetTile(int row, int column)
        {
            if ((0 <= row && row < RowCount) && (0 <= column && column < ColumnCount))
            {
                var tile = _tiles[column + row * ColumnCount];
                Debug.Assert(tile.RowIndex == row && tile.ColumnIndex == column);
                return tile;
            }
            return null;
        }

        private IEnumerable<Tile> GetNeighbours(Tile tile)
        {
            var row = tile.RowIndex;
            var column = tile.ColumnIndex;
            yield return GetTile(row, column - 1);
            yield return GetTile(row, column + 1);
            yield return GetTile(row - 1, column);
            yield return GetTile(row + 1, column);
        }

        public void Freeze()
        {
            _tiles.ForEach(t => t.Freeze());
        }

        public override bool Equals(object obj) => Equals(obj as Board);

        public bool Equals(Board other)
        {
            return other != null &&
                   RowCount == other.RowCount &&
                   ColumnCount == other.ColumnCount &&
                   Enumerable.SequenceEqual(Tiles, other.Tiles);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RowCount, ColumnCount, Tiles);
        }
    }
}
