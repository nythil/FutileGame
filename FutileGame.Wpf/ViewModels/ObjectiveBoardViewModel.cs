using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Diagnostics;
using ReactiveUI;
using System.Linq;
using FutileGame.Models;

namespace FutileGame.ViewModels
{
    public class ObjectiveBoardViewModel : ReactiveObject
    {
        private readonly Board _model;

        public ObjectiveBoardViewModel(int numRows, int numColumns)
            : this(new Board(numRows, numColumns))
        {
        }

        public ObjectiveBoardViewModel(Board model)
        {
            _model = model;
            _squares = _model.Squares.Select(sq => new ObjectiveSquareViewModel(sq)).ToList();
        }

        public int RowCount => _model.RowCount;
        public int ColumnCount => _model.ColumnCount;

        private readonly List<ObjectiveSquareViewModel> _squares;
        public IReadOnlyCollection<ObjectiveSquareViewModel> Squares => _squares;
    }
}
