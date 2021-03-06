﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace BattleshipSolver
{
    public partial class Form1 : Form
    {
        private Game _game;
        private GameDrawer _gameDrawer;
        public Form1()
        {
            InitializeComponent();

            this.Text = "Battleships";
            this.Size = new Size(1000, 1000);
            this.BackColor = Color.White;

            var solveButton = new Button
            {
                Text = "Solve",
            };

            solveButton.Click += SolveButton_Click;
            this.Controls.Add(solveButton);

            var restartButton = new Button
            {
                Text = "Restart",
                Location = new Point(200,0),
            };

            restartButton.Click += RestartButton_Click;
            this.Controls.Add(restartButton);

            var gameLoader = new LoadGameFromFile();
            _game = gameLoader.Load("Games/Game3.txt").GetAwaiter().GetResult();
            _gameDrawer = new GameDrawer(_game);
            this.Paint += Form1_Paint;
        }

        private async void RestartButton_Click(object sender, EventArgs e)
        {
            var gameLoader = new LoadGameFromFile();
            _game = await gameLoader.Load("Games/Game3.txt");
            _gameDrawer = new GameDrawer(_game);

            using var g = this.CreateGraphics();
            g.Clear(this.BackColor);
            _gameDrawer.Draw(g);
        }

        private void SolveButton_Click(object sender, EventArgs e)
        {
            var result = _game.Solve();

            if (result is null)
            {
                MessageBox.Show("No more solutions found");
            }
            else
            {
                using var g = this.CreateGraphics();
                g.Clear(this.BackColor);
                _gameDrawer.Draw(g);
                _gameDrawer.DrawMove(g, result);
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            _gameDrawer.Draw(e.Graphics);
        }
    }
}
