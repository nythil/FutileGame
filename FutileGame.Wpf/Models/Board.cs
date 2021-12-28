using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ReactiveUI;

namespace FutileGame.Models
{
    public sealed class Board : ReactiveObject, IEquatable<Board>
    {
        public Board(int numRows, int numColumns)
        {
            RowCount = numRows;
            ColumnCount = numColumns;

            _tiles = new List<Tile>(numRows * numColumns);
            for (int r = 0; r < RowCount; r++)
            {
                for (int c = 0; c < ColumnCount; c++)
                {
                    _tiles.Add(new(r, c));
                }
            }

            foreach (var sq in _tiles)
            {
                sq.SetNeighbours(GetNeighbours(sq.RowIndex, sq.ColumnIndex));
            }
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
        public IReadOnlyCollection<Tile> Tiles => _tiles;

        public Tile GetTile(int row, int column)
        {
            if ((0 <= row && row < RowCount) && (0 <= column && column < ColumnCount))
            {
                return GetTileCore(row, column);
            }
            return null;
        }

        private Tile GetTileCore(int row, int column)
        {
            Debug.Assert(0 <= row && row < RowCount);
            Debug.Assert(0 <= column && column < ColumnCount);
            return _tiles[column + row * ColumnCount];
        }

        public IEnumerable<Tile> GetNeighbours(int row, int column)
        {
            var neighbours = new List<Tile>(4);
            neighbours.Add(GetTile(row, column - 1));
            neighbours.Add(GetTile(row, column + 1));
            neighbours.Add(GetTile(row - 1, column));
            neighbours.Add(GetTile(row + 1, column));
            return neighbours.Where(sq => sq is not null);
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

        public bool IsAnyChecked => Tiles.Any(x => x.IsChecked);
    }
}
