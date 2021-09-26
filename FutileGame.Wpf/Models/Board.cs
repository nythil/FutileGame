using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ReactiveUI;

namespace FutileGame.Models
{
    public sealed class Board : ReactiveObject
    {
        public Board(int numRows, int numColumns)
        {
            RowCount = numRows;
            ColumnCount = numColumns;

            _squares = new List<Square>(numRows * numColumns);
            for (int r = 0; r < RowCount; r++)
            {
                for (int c = 0; c < ColumnCount; c++)
                {
                    _squares.Add(new(r, c));
                }
            }

            foreach (var sq in _squares)
            {
                sq.SetNeighbours(GetNeighbours(sq.RowIndex, sq.ColumnIndex));
            }
        }

        public int RowCount { get; }
        public int ColumnCount { get; }

        private readonly List<Square> _squares;
        public IReadOnlyCollection<Square> Squares => _squares;

        public Square GetSquare(int row, int column)
        {
            if ((0 <= row && row < RowCount) && (0 <= column && column < ColumnCount))
            {
                return GetSquareCore(row, column);
            }
            return null;
        }

        private Square GetSquareCore(int row, int column)
        {
            Debug.Assert(0 <= row && row < RowCount);
            Debug.Assert(0 <= column && column < ColumnCount);
            return _squares[column + row * ColumnCount];
        }

        public IEnumerable<Square> GetNeighbours(int row, int column)
        {
            var neighbours = new List<Square>(4);
            neighbours.Add(GetSquare(row, column - 1));
            neighbours.Add(GetSquare(row, column + 1));
            neighbours.Add(GetSquare(row - 1, column));
            neighbours.Add(GetSquare(row + 1, column));
            return neighbours.Where(sq => sq is not null);
        }
    }
}
