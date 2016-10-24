using System;
using System.Collections.Generic;
using InvadersWindowsStoreApp.Model;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace InvadersWindowsStoreApp.View
{
    static class InvadersHelper
    {
        private static readonly Random _random = new Random();

        public static IEnumerable<string> CreateImageList(InvaderType shipType)
        {
            string filename;
            switch (shipType)
            {
                case InvaderType.Bug:
                    filename = "bug";
                    break;
                case InvaderType.Spaceship:
                    filename = "spaceship";
                    break;
                case InvaderType.Star:
                    filename = "star";
                    break;
                case InvaderType.Satellite:
                default:
                    filename = "satellite";
                    break;
            }

            List<string> imageList = new List<string>();
            for (int i = 1; i <= 4; i++)
            {
                imageList.Add($"{filename}{i}.png");
            }
            return imageList;
        }

        public static FrameworkElement ScanLineFactory(int y, int width, double scale)
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Width = width * scale;
            rectangle.Height = 2;
            rectangle.Opacity = 0.1;
            rectangle.Fill = new SolidColorBrush(Colors.White);
            SetCanvasLocation(rectangle, 0, y * scale);

            return rectangle;
        }

        public static void SetCanvasLocation(FrameworkElement control, double x, double y)
        {
            if(control == null)
            {
                return;
            }

            Canvas.SetLeft(control, x);
            Canvas.SetTop(control, y);
        }

        public static void MoveElementOnCanvas(FrameworkElement frameworkElement, double toX, double toY)
        {
            if(frameworkElement == null)
            {
                return;
            }

            double fromX = Canvas.GetLeft(frameworkElement);
            double fromY = Canvas.GetTop(frameworkElement);

            Storyboard storyboard = new Storyboard();
            DoubleAnimation animationX = CreateDoubleAnimation(frameworkElement, fromX, toX, "(Canvas.Left)");
            DoubleAnimation animationY = CreateDoubleAnimation(frameworkElement, fromY, toY, "(Canvas.Top)");
            storyboard.Children.Add(animationX);
            storyboard.Children.Add(animationY);
            storyboard.Begin();
        }

        public static DoubleAnimation CreateDoubleAnimation(FrameworkElement frameworkElement, double from, double to, string propertyToAnimate)
        {
            return frameworkElement == null ? null : CreateDoubleAnimation(frameworkElement, from, to, propertyToAnimate, TimeSpan.FromMilliseconds(25));
        }

        public static void ResizeElement(FrameworkElement control, double width, double height)
        {
            if(control == null)
            {
                return;
            }

            if (control.Width != width)
            {
                control.Width = width;
            }

            if (control.Height != height)
            {
                control.Height = height;
            }
        }

        public static BitmapImage CreateImageFromAssets(string imageFilename)
        {
            return String.IsNullOrEmpty(imageFilename)? null : new BitmapImage(new Uri("ms-appx:///Assets/" + imageFilename));
        }

        internal static FrameworkElement InvaderControlFactory(Invader invader, double scale)
        {
            if(invader == null)
            {
                return null;
            }

            IEnumerable<string> imageNames = CreateImageList(invader.InvaderType);
            AnimatedImage invaderControl = new AnimatedImage(imageNames, TimeSpan.FromSeconds(0.75));
            invaderControl.Width = invader.Size.Width * scale;
            invaderControl.Height = invader.Size.Height * scale;
            SetCanvasLocation(invaderControl, invader.Location.X * scale, invader.Location.Y * scale);

            return invaderControl;
        }

        internal static FrameworkElement ShotControlFactory(Shot shot, double scale)
        {
            if (shot == null)
            {
                return null;
            }

            Rectangle rectangle = new Rectangle();
            rectangle.Fill = new SolidColorBrush(Colors.Yellow);
            rectangle.Width = Shot.ShotSize.Width * scale;
            rectangle.Height = Shot.ShotSize.Height * scale;
            SetCanvasLocation(rectangle, shot.Location.X * scale, shot.Location.Y * scale);

            return rectangle;
        }

        internal static FrameworkElement StarControlFactory(Point point, double scale)
        {
            if(point == null)
            {
                return null;
            }

            FrameworkElement star;
            switch (_random.Next(3))
            {
                case 0:
                    star = new Rectangle();
                    ((Rectangle)star).Fill = new SolidColorBrush(RandomStarColor());
                    star.Width = 2;
                    star.Height = 2;
                    break;
                case 1:
                    star = new Ellipse();
                    ((Ellipse)star).Fill = new SolidColorBrush(RandomStarColor());
                    star.Width = 2;
                    star.Height = 2;
                    break;
                default:
                    star = new StarControl();
                    ((StarControl)star).SetFill(new SolidColorBrush(RandomStarColor()));
                    break;
            }

            SetCanvasLocation(star, point.X * scale, point.Y * scale);
            Canvas.SetZIndex(star, -1000);

            return star;
        }

        internal static FrameworkElement PlayerControlFactory(Player player, double scale)
        {
            if(player == null)
            {
                return null;
            }

            AnimatedImage playerImageControl = new AnimatedImage(new List<string>() { "player.png"}, TimeSpan.FromSeconds(1));
            playerImageControl.Width = player.Size.Width * scale;
            playerImageControl.Height = player.Size.Height * scale;
            SetCanvasLocation(playerImageControl, player.Location.X * scale, player.Location.Y * scale);

            return playerImageControl;
        }

        private static Color RandomStarColor()
        {
            switch (_random.Next(10))
            {
                case 0:
                    return Colors.LightGreen;
                case 1:
                    return Colors.Azure;
                case 2:
                    return Colors.MediumPurple;
                case 3:
                    return Colors.Cyan;
                case 4:
                    return Colors.DarkRed;
                case 5:
                    return Colors.Yellow;
                case 6:
                    return Colors.Red;
                case 7:
                    return Colors.White;
                case 8:
                    return Colors.LightBlue;
                default:
                    return Colors.Gold;
            }
        }

        private static DoubleAnimation CreateDoubleAnimation(FrameworkElement frameworkElement, double from, double to, string propertyToAnimate, TimeSpan timeSpan)
        {
            if(timeSpan == null)
            {
                return null;
            }

            DoubleAnimation animation = new DoubleAnimation();
            Storyboard.SetTarget(animation, frameworkElement);
            Storyboard.SetTargetProperty(animation, propertyToAnimate);
            animation.From = from;
            animation.To = to;
            animation.Duration = timeSpan;

            return animation;
        }
    }
}