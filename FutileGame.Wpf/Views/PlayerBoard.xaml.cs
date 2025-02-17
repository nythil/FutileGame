﻿using System;
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
    /// Interaction logic for PlayerBoard.xaml
    /// </summary>
    public partial class PlayerBoard
    {
        public PlayerBoard()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x => x.ViewModel)
                    .Subscribe(_ => UpdateBoardView(disposables))
                    .DisposeWith(disposables);
            });
        }

        private void UpdateBoardView(CompositeDisposable disposables)
        {
            if (ViewModel is null)
                return;

            myGrid.Rows = ViewModel.RowCount;
            myGrid.Columns = ViewModel.ColumnCount;

            myGrid.Children.Clear();
            foreach (var sqVM in ViewModel.Tiles)
            {
                var btn = new Button
                {
                    Command = sqVM.Toggle,
                };
                Grid.SetRow(btn, sqVM.RowIndex);
                Grid.SetColumn(btn, sqVM.ColumnIndex);
                sqVM.WhenAnyValue(v => v.Text)
                    .BindTo(btn, b => b.Content)
                    .DisposeWith(disposables);
                myGrid.Children.Add(btn);
            }
        }
    }
}
