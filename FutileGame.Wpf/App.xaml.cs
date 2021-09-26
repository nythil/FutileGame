using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Splat;
using FutileGame.Services;

namespace FutileGame
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Locator.CurrentMutable.Register<ISquareValueFormatter>(() => new DefaultSquareValueFormatter());

            base.OnStartup(e);
        }
    }
}
