﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShooterGame2D
{
    public class MainMenu : Form
    {
        private Button playButton;
        private Label titleLabel;

        public MainMenu()
        {
            InitializeComponent();
            InitlializeControls();
        }
        private void InitializeComponent()
        {
            this.Text = "Main Menu";
            this.Size = new Size(800,600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // judul game
            titleLabel = new Label
            {
                Text = "Slime Invasion!",
                Font = new Font("Pixelify Sans", 48, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(30, 100),
            };
            this.Controls.Add(titleLabel);

            // play button
            playButton = new Button
            {
                Size = new Size(200, 200),
                Location = new Point(this.ClientSize.Width/2 - 100, 250),
                BackgroundImage = Resource.Play,
                BackgroundImageLayout = ImageLayout.Stretch,
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0, MouseDownBackColor = Color.Transparent, MouseOverBackColor = Color.Transparent }
            };
            playButton.Click += PlayButton_Click;
            this.Controls.Add(playButton);
        }

        private void InitlializeControls()
        {
            this.Controls.Add(playButton);
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            LevelForm levelForm = new LevelForm();
            levelForm.ShowDialog();
            this.Close();
        }
    }
}
