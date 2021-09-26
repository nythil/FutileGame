using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using ReactiveUI;
using Splat;
using FutileGame.Models;
using FutileGame.Services;

namespace FutileGame.ViewModels
{
    public class GameSquareViewModel : ReactiveObject
    {
        private readonly Square _model;
        private readonly ISquareValueFormatter _valueFormatter;

        public GameSquareViewModel(Square m, ISquareValueFormatter valueFormatter = null)
        {
            _model = m;
            _valueFormatter = valueFormatter ?? Locator.Current.GetService<ISquareValueFormatter>();

            _text = _model
                .WhenAnyValue(x => x.Value)
                .Select(x => _valueFormatter.FormatValue(x))
                .ToProperty(this, x => x.Text);

            Check = ReactiveCommand.Create(
                canExecute: _model.WhenAny(m => m.Value, x => x.Sender.CanCheck),
                execute: () => _model.Check()
            );

            Uncheck = ReactiveCommand.Create(
                canExecute: _model.WhenAny(m => m.Value, x => x.Sender.CanCheck),
                execute: () => _model.Uncheck()
            );

            Toggle = ReactiveCommand.Create(
                canExecute: _model.WhenAny(m => m.Value, x => x.Sender.CanCheck || x.Sender.CanUncheck),
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

        public ReactiveCommand<Unit, Unit> Check { get; }
        public ReactiveCommand<Unit, Unit> Uncheck { get; }
        public ReactiveCommand<Unit, Unit> Toggle { get; }
    }
}
