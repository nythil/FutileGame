using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Diagnostics;
using ReactiveUI;
using System.Linq;
using FutileGame.Models;
using FutileGame.Services;

namespace FutileGame.ViewModels
{
    public class GameBoardViewModel : ReactiveObject
    {
        private readonly Board _model;

        public GameBoardViewModel(int numRows, int numColumns, ISquareValueFormatter valueFormatter = null)
            : this(new Board(numRows, numColumns), valueFormatter)
        {
        }

        public GameBoardViewModel(Board model, ISquareValueFormatter valueFormatter = null)
        {
            _model = model;
            _squares = _model.Squares.Select(sq => new GameSquareViewModel(sq, valueFormatter)).ToList();
        }

        public int RowCount => _model.RowCount;
        public int ColumnCount => _model.ColumnCount;

        private readonly List<GameSquareViewModel> _squares;
        public IReadOnlyCollection<GameSquareViewModel> Squares => _squares;
    }
}
