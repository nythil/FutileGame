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
                    viewModel => viewModel.Round.PlayerBoard,
                    view => view.playerBoard.ViewModel
                ).DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                    viewModel => viewModel.IsRoundStarted,
                    view => view.playerBoard.IsEnabled
                ).DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                    viewModel => viewModel.Round.ObjectiveBoard,
                    view => view.objectiveBoard.ViewModel
                ).DisposeWith(disposable);

                this.BindCommand(ViewModel,
                    viewModel => viewModel.NewGame,
                    view => view.btnNewGame
                ).DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                    viewModel => viewModel.IsGameStarted,
                    view => view.paneTimeLeft.Visibility,
                    isStarted => isStarted ? Visibility.Visible : Visibility.Collapsed
                ).DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                    viewModel => viewModel.Round.TimeRemaining,
                    view => view.txtTimeLeft.Text,
                    time => time.ToString("000.00")
                ).DisposeWith(disposable);

                this.BindInteraction(ViewModel,
                    viewModel => viewModel.GameEnded,
                    context =>
                    {
                        return Dispatcher.InvokeAsync(() =>
                        {
                            var message = context.Input ? "You win!" : "You lose!";
                            var icon = context.Input ? MessageBoxImage.Information : MessageBoxImage.Exclamation;
                            var result = MessageBox.Show(
                                this,
                                "Play again?",
                                message,
                                MessageBoxButton.YesNo,
                                icon
                            );
                            context.SetOutput(result == MessageBoxResult.Yes);
                        }).Task;
                    }
                ).DisposeWith(disposable);
            });
        }
    }
}
