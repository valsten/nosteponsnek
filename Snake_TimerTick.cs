    //This method is called everytime timer ticks
    private void timer_Tick(object sender, EventArgs e)
    {
      switch (currentDirection)
      {
        case Direction.Down: //down
          currentPosition.Y += 1;
          break;
        case Direction.Up: //up
          currentPosition.Y -= 1;
          break;
        case Direction.Left: //left
          currentPosition.X -= 1;
          break;
        case Direction.Right: //right
          currentPosition.X += 1;
          break;
      }
      PaintSnake(currentPosition);
      //Restrict the game area
      if ((currentPosition.X < minimi) || (currentPosition.X > maxWidth)
            || (currentPosition.Y < minimi) || (currentPosition.Y > maxHeight))
        GameOver();

      // Hitting bonus point
      int n = 0;
      foreach (Point point in bonusPoints)
      {
        if ((Math.Abs(point.X - currentPosition.X) < snakeWidth) &&
            (Math.Abs(point.Y - currentPosition.Y) < snakeWidth))
        {
          snakeLength += 10;
          score += 10;
          //nopeutetaan peliä
          if (easiness > 5)
          { 
            easiness--;
            timer.Interval = new TimeSpan(0, 0, 0, 0, easiness);
          }
          this.Title = "SnakeWPF - your score: " + score;
          bonusPoints.RemoveAt(n);
          paintCanvas.Children.RemoveAt(n);
          PaintBonus(n);
          break;
        }
        n++;
      }

      //Check if snake hits itself
      for (int i = 0; i < (snakeParts.Count - snakeWidth * 2); i++)
      {
        Point point = new Point(snakeParts[i].X, snakeParts[i].Y);
        if ((Math.Abs(point.X - currentPosition.X) < (snakeWidth)) && (Math.Abs(point.Y - currentPosition.Y) < (snakeWidth)))
        {
          GameOver();
          break;
        }
      }
    }
