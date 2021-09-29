using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Diagnostics;
using ReactiveUI;
using System.Linq;
using FutileGame.Models;
using FutileGame.Services;

namespace FutileGame.ViewModels
{
    public class ObjectiveBoardViewModel : ReactiveObject
    {
        private readonly Board _model;

        public ObjectiveBoardViewModel(int numRows, int numColumns, ISquareValueFormatter valueFormatter = null)
            : this(new Board(numRows, numColumns), valueFormatter)
        {
        }

        public ObjectiveBoardViewModel(Board model, ISquareValueFormatter valueFormatter = null)
        {
            _model = model;
            _squares = _model.Squares.Select(sq => new ObjectiveSquareViewModel(sq, valueFormatter)).ToList();
        }

        public int RowCount => _model.RowCount;
        public int ColumnCount => _model.ColumnCount;

        private readonly List<ObjectiveSquareViewModel> _squares;
        public IReadOnlyCollection<ObjectiveSquareViewModel> Squares => _squares;
    }
}
