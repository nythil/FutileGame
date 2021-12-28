using System;
using System.Collections.Generic;
using System.Linq;
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
using Splat;
using FutileGame.ViewModels;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using FutileGame.Services;

namespace FutileGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainWindowViewModel(5, 5, new DefaultTileValueFormatter());

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel,
                    viewModel => viewModel.PlayerBoard,
                    view => view.playerBoard.ViewModel
                ).DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                    viewModel => viewModel.ObjectiveBoard,
                    view => view.objectiveBoard.ViewModel
                ).DisposeWith(disposable);

                this.BindCommand(ViewModel,
                    viewModel => viewModel.StartGame,
                    view => view.btnStartGame
                ).DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                    viewModel => viewModel.IsGameStarted,
                    view => view.playerBoard.IsEnabled
                ).DisposeWith(disposable);

                this.BindInteraction(ViewModel,
                    viewModel => viewModel.GameEnded,
                    context =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            var result = MessageBox.Show(
                                this,
                                "Play again?",
                                "Victory!",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Question
                            );
                            context.SetOutput(result == MessageBoxResult.Yes);
                        });
                        return Observable.Return(Unit.Default);
                    }
                ).DisposeWith(disposable);
            });
        }
    }
}
