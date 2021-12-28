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

        public PlayerBoardViewModel(int numRows, int numColumns, ITileValueFormatter valueFormatter = null)
            : this(new Board(numRows, numColumns), valueFormatter)
        {
        }

        public PlayerBoardViewModel(Board model, ITileValueFormatter valueFormatter = null)
        {
            _model = model;
            _tiles = _model.Tiles.Select(sq => new PlayerTileViewModel(sq, valueFormatter)).ToList();

            TileToggledObs = _tiles
                .Select(sq => sq.ObservableForProperty(vm => vm.IsChecked, _ => sq))
                .Merge()
                .Publish()
                .RefCount()
            ;
        }

        public int RowCount => _model.RowCount;
        public int ColumnCount => _model.ColumnCount;

        private readonly List<PlayerTileViewModel> _tiles;
        public IReadOnlyCollection<PlayerTileViewModel> Tiles => _tiles;

        public IObservable<PlayerTileViewModel> TileToggledObs { get; }
    }
}
