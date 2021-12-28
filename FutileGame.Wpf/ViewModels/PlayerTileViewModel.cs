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
    public class PlayerTileViewModel : ReactiveObject
    {
        private readonly Tile _model;
        private readonly ITileValueFormatter _valueFormatter;

        public PlayerTileViewModel(Tile m, ITileValueFormatter valueFormatter = null)
        {
            _model = m;
            _valueFormatter = valueFormatter ?? Locator.Current.GetService<ITileValueFormatter>();
            if (_model is null)
                throw new ArgumentNullException(nameof(m));
            if (_valueFormatter is null)
                throw new ArgumentNullException(nameof(valueFormatter));

            _text = _model
                .WhenAnyValue(x => x.Value)
                .Select(x => _valueFormatter.FormatValue(x))
                .ToProperty(this, x => x.Text);

            _isChecked = _model
                .WhenAnyValue(x => x.IsChecked)
                .ToProperty(this, x => x.IsChecked);

            Toggle = ReactiveCommand.Create(
                canExecute: _model.WhenAnyValue(
                    m => m.CanCheck, m => m.CanUncheck,
                    (canCheck, canUncheck) => canCheck || canUncheck
                ),
                execute: () =>
                {
                    if (_model.CanCheck)
                    {
                        _model.Check();
                    }
                    else if (_model.CanUncheck)
                    {
                        _model.Uncheck();
                    }
                }
            );
        }

        public int RowIndex => _model.RowIndex;
        public int ColumnIndex => _model.ColumnIndex;

        private readonly ObservableAsPropertyHelper<string> _text;
        public string Text => _text.Value;

        private readonly ObservableAsPropertyHelper<bool> _isChecked;
        public bool IsChecked => _isChecked.Value;

        public ReactiveCommand<Unit, Unit> Toggle { get; }
    }
}
