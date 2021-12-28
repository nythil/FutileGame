using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using ReactiveUI;
using Splat;
using FutileGame.Models;
using FutileGame.Services;

namespace FutileGame.ViewModels
{
    public class ObjectiveTileViewModel : ReactiveObject
    {
        private readonly Tile _model;
        private readonly ITileValueFormatter _valueFormatter;

        public ObjectiveTileViewModel(Tile m, ITileValueFormatter valueFormatter = null)
        {
            _model = m;
            _valueFormatter = valueFormatter ?? Locator.Current.GetService<ITileValueFormatter>();
            if (_model is null)
                throw new ArgumentNullException(nameof(m));
            if (_valueFormatter is null)
                throw new ArgumentNullException(nameof(valueFormatter));
        }

        public int RowIndex => _model.RowIndex;
        public int ColumnIndex => _model.ColumnIndex;
        public string Text => _valueFormatter.FormatValue(_model.Value);
    }
}
