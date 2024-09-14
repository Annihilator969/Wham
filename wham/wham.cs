using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WhackAMoleGame
{
    public partial class WhackAMoleForm : Form
    {
        private Random random = new Random();
        private PictureBox[] moleButtons = new PictureBox[9];
        private int score = 0;
        private int molesActive = 0;
        private bool gameStarted = false;
        private int timeRemaining = 60; // Adjust the initial time as needed

        public WhackAMoleForm()
        {
            InitializeComponent();

            // Create the mole buttons
            for (int i = 0; i < 9; i++)
            {
                moleButtons[i] = new PictureBox
                {
                    Size = new Size(50, 50),
                    Image = Properties.Resources.mole, // Replace with your mole image
                    SizeMode = PictureBoxSizeMode.StretchImage
                };
                moleButtons[i].Click += MoleButton_Click;
                Controls.Add(moleButtons[i]);
            }

            // Arrange the mole buttons on the form
            int x = 10;
            int y = 10;
            for (int i = 0; i < 9; i++)
            {
                moleButtons[i].Location = new Point(x, y);
                x += 60;
                if (i % 3 == 2)
                {
                    x = 10;
                    y += 60;
                }
            }

            // Initialize the game
            scoreLabel.Text = "Score: 0";
            timeRemainingLabel.Text = "Time: 60";
            startButton.Enabled = true;
            stopButton.Enabled = false;
        }

        private async void StartGame_Click(object sender, EventArgs e)
        {
            gameStarted = true;
            startButton.Enabled = false;
            stopButton.Enabled = true;

            // Start the game timer
            await Task.Run(() =>
            {
                while (gameStarted && timeRemaining > 0)
                {
                    // Update the timer label
                    timeRemainingLabel.Invoke((MethodInvoker)delegate
                    {
                        timeRemainingLabel.Text = $"Time: {timeRemaining}";
                    });

                    // Generate random mole positions
                    RandomizeMolePositions();

                    // Wait for a random interval
                    Task.Delay(random.Next(500, 2000)).Wait();

                    // Hide the moles
                    HideMoles();

                    timeRemaining--;

                    if (timeRemaining == 0)
                    {
                        // Game over
                        MessageBox.Show($"Game Over! Your final score is: {score}");
                        ResetGame();
                    }
                }
            });
        }

        private void StopGame_Click(object sender, EventArgs e)
        {
            gameStarted = false;
            startButton.Enabled = true;
            stopButton.Enabled = false;
            HideMoles();
        }

        private void MoleButton_Click(object sender, EventArgs e)
        {
            if (gameStarted)
            {
                PictureBox moleButton = (PictureBox)sender;
                if (moleButton.Visible)
                {
                    moleButton.Visible = false;
                    molesActive--;
                    score++;
                    scoreLabel.Text = $"Score: {score}";
                }
            }
        }

        private void RandomizeMolePositions()
        {
            // Determine the number of moles to show
            molesActive = random.Next(1, 4);

            // Create a list of indices to randomly select from
            var availableIndices = new List<int>(Enumerable.Range(0, 9));
            for (int i = 0; i < molesActive; i++)
            {
                int index = random.Next(availableIndices.Count);
                moleButtons[availableIndices[index]].Visible = true;
                availableIndices.RemoveAt(index);
            }
        }

        private void HideMoles()
        {
            foreach (var mole in moleButtons)
            {
                mole.Visible = false;
            }
        }

        private void ResetGame()
        {
            score = 0;
            timeRemaining = 60;
            scoreLabel.Text = "Score: 0";
            timeRemainingLabel.Text = "Time: 60";
        }
    }
}
