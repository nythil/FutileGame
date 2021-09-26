using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using ReactiveUI;
using FutileGame.Models;

namespace FutileGame.ViewModels
{
    public class ObjectiveSquareViewModel : ReactiveObject
    {
        private readonly Square _model;

        public ObjectiveSquareViewModel(Square m)
        {
            _model = m;
        }

        private static string GetValueText(int x) => x switch
        {
            0 => "",
            5 => "1",
            _ => x.ToString()
        };

        public int RowIndex => _model.RowIndex;
        public int ColumnIndex => _model.ColumnIndex;
        public string Text => GetValueText(_model.Value);
    }
}
