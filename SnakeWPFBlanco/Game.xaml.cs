using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SnakeWPF
{
    /// <summary>
    /// The Classic Snake Game in WPF
    /// </summary>
    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }
    public partial class Game : Window
    {
        // Consts
        private const int minimi = 5;
        private const int maxWidth = 620;
        private const int maxHeight = 380;

        // Variables for snek
        private Point startingPoint = new Point(100, 100);
        private Point currentPoint = new Point();
        private Direction currentDirection = Direction.Right;
        private Direction lastDirection = Direction.Right;
        private List<Point> snekBody = new List<Point>();
        private List<Point> bonusPoints = new List<Point>();
        private const int bonusCount = 20;
        private int easiness = 20;
        private int snekWidth = 10;
        private int snekLength = 100;
        private int score = 0;

        DispatcherTimer timer;
        private Random rnd = new Random();

        public Game()
        {
            InitializeComponent();
            //TODO
            //initialize the game
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, easiness);
            timer.Tick += new EventHandler(timer_Tick);

            // Attach keyboard input to window
            this.KeyDown += new KeyEventHandler(OnButtonKeyDown);

            // Paint snek and points
            IniBonusPoints();
            PaintSnake(startingPoint);
            currentPoint = startingPoint;

            // Start the game
            timer.Start();
        }

        private void IniBonusPoints()
        {
            // Initialize bonus points
            for (int i = 0; i < bonusCount; i++)
            {
                PaintBonus(i);
            }
        }

        private void PaintSnake(Point currentposition)
        {
            // Paint the body of snek
            Ellipse sh = new Ellipse();
            sh.Fill = Brushes.ForestGreen;
            sh.Width = snekWidth;
            sh.Height = snekWidth;
            Canvas.SetTop(sh, currentposition.Y);
            Canvas.SetLeft(sh, currentposition.X);
            paintCanvas.Children.Add(sh);
            int count = paintCanvas.Children.Count;
            snekBody.Add(currentposition);

            // Restrict snek's length
            if (count > snekLength)
            {
                paintCanvas.Children.RemoveAt(count - snekLength + bonusCount - 1);
                snekBody.RemoveAt(count - snekLength);
            }
        }


        private void PaintBonus(int index)
        {
            //create & paint a new bonus
            //TODO
            // Get random point to paint a point
            Point bonusPoint = new Point(rnd.Next(minimi, maxWidth),
                                        rnd.Next(minimi, maxHeight));

            // Painting a point
            Ellipse pb = new Ellipse();
            pb.Fill = Brushes.Yellow;
            pb.Width = snekWidth;
            pb.Height = snekWidth;
            Canvas.SetTop(pb, bonusPoint.Y);
            Canvas.SetLeft(pb, bonusPoint.X);
            paintCanvas.Children.Insert(index, pb);
            bonusPoints.Insert(index, bonusPoint);

        }

        private void OnButtonKeyDown(object sender, KeyEventArgs e)
        {
            //Keyboard inputs. Don't allow 180 degree turns!
            switch (e.Key)
            {
                case Key.P:
                    if (timer.IsEnabled)
                        timer.Stop();
                    else
                        timer.Start();
                    break;
                case Key.Escape:
                    this.Close();
                    break;
                case Key.Up:
                    if (lastDirection != Direction.Down)
                        currentDirection = Direction.Up;
                    break;
                case Key.Down:
                    if (lastDirection != Direction.Up)
                        currentDirection = Direction.Down;
                    break;
                case Key.Left:
                    if (lastDirection != Direction.Right)
                        currentDirection = Direction.Left;
                    break;
                case Key.Right:
                    if (lastDirection != Direction.Left)
                        currentDirection = Direction.Right;
                    break;
            }
            lastDirection = currentDirection;
        }

        //This method is called everytime timer ticks
        private void timer_Tick(object sender, EventArgs e)
        {
            // Set X- or Y coordinates after current direction
            switch (currentDirection)
            {
                case Direction.Up:
                    currentPoint.Y -= 1;
                    break;
                case Direction.Right:
                    currentPoint.X += 1;
                    break;
                case Direction.Down:
                    currentPoint.Y += 1;
                    break;
                case Direction.Left:
                    currentPoint.X -= 1;
                    break;
                default:
                    break;
            }
            // Paint the snek in new position
            PaintSnake(currentPoint);

            // Check if snake hits the edge
            if ((currentPoint.X < minimi) || (currentPoint.X > maxWidth) ||
                (currentPoint.Y < minimi) || (currentPoint.Y > maxHeight))
            {
                GameOver();
            }

            // Check if snake hits itself
            for (int i = 0; i < snekBody.Count - snekWidth * 2; i++)
            {
                Point point = new Point(snekBody[i].X, snekBody[i].Y);
                if ((Math.Abs(point.X - currentPoint.X) < snekWidth) && (Math.Abs(point.Y - currentPoint.Y) < snekWidth))
                {
                    GameOver();
                    break;
                }
            }

            // Hitting bonus point
            int n = 0;
            foreach (Point point in bonusPoints)
            {
                if ((Math.Abs(point.X - currentPoint.X) < snekWidth) && (Math.Abs(point.Y - currentPoint.Y) < snekWidth))
                {
                    // Increase length
                    snekLength += 10;
                    score += 10;

                    // Increase speed
                    if (easiness > 5)
                    {
                        easiness--;
                        timer.Interval = new TimeSpan(0, 0, 0, 0, easiness);
                    }

                    this.Title = "Snek Stepping - Score: " + score;
                    bonusPoints.RemoveAt(n);
                    paintCanvas.Children.RemoveAt(n);
                    PaintBonus(n);
                    break;
                }
                n++;
            }

        }

        private void GameOver()
        {
            timer.Stop();
            //vaihtoehtoiset lopetukset
            GameOverMsg();
            //GameOverShow();
        }
        private void GameOverMsg()
        {
            MessageBox.Show("Your score: " + score);
            this.Close();
        }
        private void GameOverShow()
        {
            //lisätään kanvaasille txt uudestaan
            txtMessage.Text = "Your score: " + score + "\npress Esc to quit";
            paintCanvas.Children.Add(txtMessage);
            //animaatio joka siirtää kanvaasin lopuksi
            var trs = new TranslateTransform();
            var anim3 = new DoubleAnimation(0, 620, TimeSpan.FromSeconds(15));
            trs.BeginAnimation(TranslateTransform.XProperty, anim3);
            trs.BeginAnimation(TranslateTransform.YProperty, anim3);
            paintCanvas.RenderTransform = trs;
            //no nyt on aika lopettaa
            //this.Close();
        }
    }
}
