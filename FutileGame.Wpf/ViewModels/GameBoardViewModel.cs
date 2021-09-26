using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Diagnostics;
using ReactiveUI;
using System.Linq;
using FutileGame.Models;

namespace FutileGame.ViewModels
{
    public class GameBoardViewModel : ReactiveObject
    {
        private readonly Board _model;

        public GameBoardViewModel(int numRows, int numColumns)
            : this(new Board(numRows, numColumns))
        {
        }

        public GameBoardViewModel(Board model)
        {
            _model = model;
            _squares = _model.Squares.Select(sq => new GameSquareViewModel(sq)).ToList();
        }

        public int RowCount => _model.RowCount;
        public int ColumnCount => _model.ColumnCount;

        private readonly List<GameSquareViewModel> _squares;
        public IReadOnlyCollection<GameSquareViewModel> Squares => _squares;
    }
}
