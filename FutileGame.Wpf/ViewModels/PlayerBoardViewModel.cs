using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Collections.Generic;
using ReactiveUI;
using FutileGame.Models;
using FutileGame.Services;

namespace FutileGame.ViewModels
{
    public class PlayerBoardViewModel : ReactiveObject
    {
        private readonly Board _model;

        public PlayerBoardViewModel(int numRows, int numColumns, ISquareValueFormatter valueFormatter = null)
            : this(new Board(numRows, numColumns), valueFormatter)
        {
        }

        public PlayerBoardViewModel(Board model, ISquareValueFormatter valueFormatter = null)
        {
            _model = model;
            _squares = _model.Squares.Select(sq => new PlayerSquareViewModel(sq, valueFormatter)).ToList();

            SquareToggledObs = _squares
                .Select(sq => sq.WhenAny(vm => vm.IsChecked, change => change.Sender))
                .Merge()
                .Publish()
                .RefCount()
            ;
        }

        public int RowCount => _model.RowCount;
        public int ColumnCount => _model.ColumnCount;

        private readonly List<PlayerSquareViewModel> _squares;
        public IReadOnlyCollection<PlayerSquareViewModel> Squares => _squares;

        public IObservable<PlayerSquareViewModel> SquareToggledObs { get; }
    }
}
