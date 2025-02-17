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
    /// Interaction logic for ObjectiveBoard.xaml
    /// </summary>
    public partial class ObjectiveBoard
    {
        public ObjectiveBoard()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x => x.ViewModel)
                    .Subscribe(_ => UpdateBoardView())
                    .DisposeWith(disposables);
            });
        }

        private void UpdateBoardView()
        {
            if (ViewModel is null)
                return;

            myGrid.Rows = ViewModel.RowCount;
            myGrid.Columns = ViewModel.ColumnCount;

            myGrid.Children.Clear();
            foreach (var sqVM in ViewModel.Tiles)
            {
                var btn = new Label
                {
                    Content = sqVM.Text,
                };
                Grid.SetRow(btn, sqVM.RowIndex);
                Grid.SetColumn(btn, sqVM.ColumnIndex);
                myGrid.Children.Add(btn);
            }
        }
    }
}
