﻿using System.Windows.Controls;
using System.Windows.Data;
using AntRunner.Converters;
using AntRunner.Models;

namespace AntRunner.Controls.Tiles.Views
{
    public partial class BombKitControl
    {
        public BombKitControl(MapTile tile)
        {
            InitializeComponent();
            DataContext = tile;
            SetBinding(Canvas.TopProperty, new Binding("Y") { Converter = new PositionToCanvasScaleConverter() });
            SetBinding(Canvas.LeftProperty, new Binding("X") { Converter = new PositionToCanvasScaleConverter() });
        }
    }
}
