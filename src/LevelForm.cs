using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;

namespace ShooterGame2D
{
    public class LevelForm : Form
    {
        private Button restartButton;
        private Button exitButton;
        private Bitmap backgroundBuffer;
        private SoundPlayer BGM = new SoundPlayer(Resource.bgm1);
        private SoundPlayer Shoot = new SoundPlayer(Resource.tembak);
        private SoundPlayer Lose = new SoundPlayer(Resource.lose);

        private int ScoreCounter = 0;
        private Label Score;
        private ProgressBar HealthGraph;
        private Label TimeLabel;
        private System.Windows.Forms.Timer TimerText = new System.Windows.Forms.Timer();
        private int elapsedSeconds = 0;

        public Player player;
        public List<Bullet> Bullets = new List<Bullet>();

        public List<Item> items = new List<Item>();
        System.Windows.Forms.Timer HealthTimer = new System.Windows.Forms.Timer();

        public List<Slime> slimes = new List<Slime>();

        private System.Windows.Forms.Timer Timer = new System.Windows.Forms.Timer();
        HashSet<Keys> keysPressed = new HashSet<Keys>();

        public LevelForm()
        {
            InitializeComponent();
            EnemySpawner();
            ItemSpawner();
            InitPlugins(true);
        }

        private void InitializeComponent()
        {
            this.Text = "Level Form";
            this.Size = new Size(1920, 1080);
            this.BackColor = Color.DarkGray;
            this.StartPosition = FormStartPosition.CenterScreen;
            backgroundBuffer = new Bitmap(Resource.background__2_, this.ClientSize.Width, this.ClientSize.Height);
            BGM.PlayLooping();

            player = new Player(new PointF(this.ClientSize.Width / 2, this.ClientSize.Height / 2));

            // score label
            Score = new Label
            {
                Text = "Score: 0",
                Location = new Point(10, 70),
                AutoSize = true,
                Font = new Font("Pixelify Sans", 24),
                ForeColor = Color.Black,
                BackColor = Color.Transparent
            };
            this.Controls.Add(Score);

            // health bar
            HealthGraph = new ProgressBar
            {
                Location = new Point(10, 20),
                Size = new Size(400, 40),
                Maximum = 100,
                Value = player.Health,
                ForeColor = Color.Red,
                BackColor = Color.Red,
            };
            //this.Controls.Add(HealthGraph);

            // time label
            TimeLabel = new Label
            {
                Text = "00:00:00",
                Location = new Point(this.ClientSize.Width / 2 - 75, 0),
                AutoSize = true,
                Font = new Font("Pixelify Sans", 24),
                ForeColor = Color.Black,
                BackColor = Color.White
            };
            this.Controls.Add(TimeLabel);
            TimerText.Interval = 1000;
            TimerText.Tick += TimeText;
            TimerText.Start();

            // restart button
            restartButton = new Button
            {
                Size = new Size(80, 80),
                Location = new Point(this.ClientSize.Width - 180, 10),
                BackgroundImage = Resource.Restart,
                BackgroundImageLayout = ImageLayout.Stretch,
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0, MouseDownBackColor = Color.Transparent, MouseOverBackColor = Color.Transparent }
            };
            restartButton.Click += RestartButton_Click;
            this.Controls.Add(restartButton);

            // exit button
            exitButton = new Button
            {
                Size = new Size(80, 80),
                Location = new Point(ClientSize.Width - 90, 10),
                BackgroundImage = Resource.Exit,
                BackgroundImageLayout = ImageLayout.Stretch,
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0, MouseDownBackColor = Color.Transparent, MouseOverBackColor = Color.Transparent }
            };
            exitButton.Click += ExitButton_Click;
            this.Controls.Add(exitButton);

            // move controls
            this.KeyPreview = true;
            this.KeyDown += OnKeyDown;
            this.KeyUp += OnKeyUp;
            this.Paint += DrawGame;

            // timer
            this.DoubleBuffered = true;
            Timer.Interval = 20;
            Timer.Tick += GameLoop;
            Timer.Start();

            // on click mouse
            this.MouseDown += OnMouseClick;

        }

        private void TimeText(object sender, EventArgs e)
        {
            elapsedSeconds++;
            TimeLabel.Text = TimeSpan.FromSeconds(elapsedSeconds).ToString(@"hh\:mm\:ss");
        }

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            keysPressed.Add(e.KeyCode);
        }

        public void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                PointF playerCenter = new PointF(player.Position.X + 15, player.Position.Y + 15);
                PointF mousePos = e.Location;
                Bullets.Add(new Bullet(playerCenter, mousePos));
                //Shoot.Play();
            }
        }

        public void OnKeyUp(object sender, KeyEventArgs e)
        {
            keysPressed.Remove(e.KeyCode);
            player.StopWalking();
        }

        public void GameLoop(object sender, EventArgs e)
        {
            player.Walk(keysPressed, this.ClientSize);
            UpdateGame();
            Invalidate();
        }

        private void UpdateGame()
        {
            // update bullets
            for (int i = Bullets.Count - 1; i >= 0; i--)
            {
                Bullets[i].Update();
                if (Bullets[i].IsOffScreen(this.ClientSize))
                {
                    Bullets.RemoveAt(i);
                }
            }

            // update player
            for (int i = items.Count - 1; i >= 0; i--)
            {
                if (player.collider.Intersects(items[i].collider))
                {
                    items[i].GiveEffect(player);
                    UpdateHealthBar();
                    items.RemoveAt(i);
                }
            }

            // update slime
            for (int i = slimes.Count - 1; i >= 0; i--)
            {
                var slime = slimes[i];
                slime.TargettingPlayer(player); //polymorphism

                // klo kena player
                if (player.collider.Intersects(slime.collider))
                {
                    player.TakeDamage(slime.Damage);
                    UpdateHealthBar();
                }

                // klo kena bullet
                for (int j = Bullets.Count - 1; j >= 0; j--)
                {
                    if (slime.collider.Intersects(Bullets[j].collider))
                    {
                        slime.TakeDamage(1);
                        Bullets.RemoveAt(j);
                        if (slime.Health <= 0)
                        {
                            slimes.RemoveAt(i);
                            UpdateScore(slime.Score);
                            break;
                        }
                    }
                }

            }
        }

        private void UpdateScore(int slimeScore)
        {
            ScoreCounter += slimeScore;
            Score.Text = $"Score: {ScoreCounter}"; // polymorphism
        }

        private void UpdateHealthBar()
        {
            HealthGraph.Value = Math.Max(0, Math.Min(player.Health, HealthGraph.Maximum));

            if (player.Health <= 0)
            {
                Timer.Stop();
                BGM.Stop();
                Lose.Play();

                // ntar diganti form baru (score, waktu, restart, exit)
                MessageBox.Show("Game Over! Your score: " + ScoreCounter);
                Application.Exit();

            }
        }

        private void InitPlugins(bool isActive)
        {
            if (isActive)
            {
                SlimeFactory.Register(pos => new Red(pos));
                SlimeFactory.Register(pos => new Blue(pos));
                SlimeFactory.Register(pos => new Black(pos));
            }
        }

        private void EnemySpawner()
        {
            SlimeFactory.Register(pos => new Green(pos));
            var rand = new Random();
            System.Windows.Forms.Timer slimeTimer = new System.Windows.Forms.Timer { Interval = 1500 };
            slimeTimer.Tick += (s, e) =>
            {
                var spawnPos = new PointF(
                    rand.Next(0, this.ClientSize.Width - 30),
                    rand.Next(0, this.ClientSize.Height - 30)
                );
                var slime = SlimeFactory.CreateRandom(spawnPos);
                if (slime != null) slimes.Add(slime);
            };
            slimeTimer.Start();
        }


        public void ItemSpawner()
        {
            HealthTimer.Interval = 10000;
            HealthTimer.Tick += (s, e) =>
            {
                var rand = new Random();
                var spawnPos = new PointF(
                    rand.Next(0, this.ClientSize.Width - 30),
                    rand.Next(0, this.ClientSize.Height - 30)
                );
                items.Add(new HealthPotion(spawnPos));
            };
            HealthTimer.Start();
        }

        private void DrawGame(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(backgroundBuffer, 0, 0);

            List<IDrawable> drawables = new List<IDrawable>();
            drawables.Add(player);
            drawables.AddRange(slimes);
            drawables.AddRange(Bullets);
            drawables.AddRange(items);

            foreach (var drawable in drawables)
            {
                drawable.Draw(e.Graphics);
            }

            // health bar
            int barWidth = 400;
            int barHeight = 40;
            int x = 10;
            int y = 20;
            float healthPercent = Math.Max(0, (float)player.Health / 100f);

            if (healthPercent > 0.5) e.Graphics.FillRectangle(Brushes.LimeGreen, x, y, (int)(barWidth * healthPercent), barHeight); // bar
            else if (healthPercent > 0.25) e.Graphics.FillRectangle(Brushes.Yellow, x, y, (int)(barWidth * healthPercent), barHeight); // bar
            else e.Graphics.FillRectangle(Brushes.Red, x, y, (int)(barWidth * healthPercent), barHeight); // bar
            e.Graphics.DrawRectangle(Pens.Black, x, y, barWidth, barHeight); // border

        }

        private void RestartButton_Click(object sender, EventArgs e)
        {
            player = new Player(new PointF(this.ClientSize.Width / 2, this.ClientSize.Height / 2));

            slimes.Clear();
            Bullets.Clear();
            items.Clear();

            ScoreCounter = 0;
            Score.Text = "Score: 0";

            player.Health = 100;
            HealthGraph.Value = player.Health;

            TimerText.Stop();
            elapsedSeconds = 0;
            TimeLabel.Text = "00:00:00";
            TimerText.Start();

            BGM.Stop();
            BGM.Play();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
