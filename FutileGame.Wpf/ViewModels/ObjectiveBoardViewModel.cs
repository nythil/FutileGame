﻿using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Diagnostics;
using ReactiveUI;
using System.Linq;
using FutileGame.Models;
using FutileGame.Services;

namespace FutileGame.ViewModels
{
    public class ObjectiveBoardViewModel : ReactiveObject
    {
        private readonly Board _model;

        public ObjectiveBoardViewModel(Board model, ITileValueFormatter? valueFormatter = null)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _tiles = _model.Tiles.Select(sq => new ObjectiveTileViewModel(sq, valueFormatter)).ToList();
        }

        public int RowCount => _model.RowCount;
        public int ColumnCount => _model.ColumnCount;

        private readonly List<ObjectiveTileViewModel> _tiles;
        public IReadOnlyCollection<ObjectiveTileViewModel> Tiles => _tiles;
    }
}
