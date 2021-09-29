using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReactiveUI;
using FutileGame.ViewModels;
using FutileGame.Services;

namespace FutileGame.Views
{
    /// <summary>
    /// Interaction logic for GameBoard.xaml
    /// </summary>
    public partial class GameBoard
    {
        public GameBoard()
        {
            InitializeComponent();
            ViewModel = new(5, 5, new DefaultSquareValueFormatter());

            myGrid.RowDefinitions.Clear();
            for (int y = 0; y < ViewModel.RowCount; y++)
            {
                myGrid.RowDefinitions.Add(new());
            }

            myGrid.ColumnDefinitions.Clear();
            for (int x = 0; x < ViewModel.ColumnCount; x++)
            {
                myGrid.ColumnDefinitions.Add(new());
            }

            myGrid.Children.Clear();
            foreach (var sqVM in ViewModel.Squares)
            {
                var btn = new Button
                {
                    Tag = sqVM
                };
                Grid.SetRow(btn, sqVM.RowIndex);
                Grid.SetColumn(btn, sqVM.ColumnIndex);
                myGrid.Children.Add(btn);
            }

            this.WhenActivated(disposables =>
            {
                foreach (var btn in myGrid.Children.OfType<Button>())
                {
                    var sqVM = (GameSquareViewModel)btn.Tag;
                    sqVM.WhenAnyValue(v => v.Text).BindTo(btn, b => b.Content).DisposeWith(disposables);
                    btn.Command = sqVM.Toggle;
                }
            });
        }
    }
}
