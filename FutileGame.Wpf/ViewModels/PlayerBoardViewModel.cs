﻿using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Collections.Generic;
using ReactiveUI;
using FutileGame.Models;
using FutileGame.Services;

namespace FutileGame.ViewModels
{
    public class PlayerBoardViewModel : ReactiveObject
    {
        private readonly Board _model;

        public PlayerBoardViewModel(Board model, ITileValueFormatter? valueFormatter = null)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _tiles = _model.Tiles.Select(sq => new PlayerTileViewModel(sq, valueFormatter)).ToList();
        }

        public int RowCount => _model.RowCount;
        public int ColumnCount => _model.ColumnCount;

        private readonly List<PlayerTileViewModel> _tiles;
        public IReadOnlyCollection<PlayerTileViewModel> Tiles => _tiles.AsReadOnly();
    }
}
