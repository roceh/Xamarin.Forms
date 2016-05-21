using Xamarin.Forms;
using Xamarin.Forms.Platform.Skia;

namespace SkiaWin32Demo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Forms.Init();

            var win32 = new Win32Window(800, 600);

            win32.Run(new App());
        }

        public class App : Application
        {
            public App()
            {
                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

                var grid2 = new Grid();
                grid2.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                grid2.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                grid2.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                grid2.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

                grid2.Children.Add(new Label() { BackgroundColor = Color.Aqua, Text = "1", VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center }, 0, 0);
                grid2.Children.Add(new Label() { BackgroundColor = Color.Aqua, Text = "2", VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center }, 1, 0);
                grid2.Children.Add(new Label() { BackgroundColor = Color.Aqua, Text = "3", VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center }, 0, 1);
                grid2.Children.Add(new Label() { BackgroundColor = Color.Aqua, Text = "4", VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center }, 1, 1);


                grid.Children.Add(grid2, 0, 0);
                grid.Children.Add(new Label() { BackgroundColor = Color.Red, Text = "Hello", VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center }, 1, 0);
                grid.Children.Add(new Label() { BackgroundColor = Color.Green, Text = "Xamarin", VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center }, 0, 1);
                grid.Children.Add(new Label() { BackgroundColor = Color.Yellow, Text = "Forms", VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center }, 1, 1);

                MainPage = new ContentPage
                {
                    Content = grid
                };
            }
        }
    }
}