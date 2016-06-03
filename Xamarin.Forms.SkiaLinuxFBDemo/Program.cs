using Xamarin.Forms;
using Xamarin.Forms.Platform.Skia;
using System.Threading.Tasks;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace Xamarin.Forms.SkiaLinuxFBDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Forms.Init();

            var linuxFB = new LinuxFrameBuffer();
            linuxFB.TouchScreenHandler = new DefaultTouchScreenHandler("/dev/input/event0", new Rectangle(0, 0, 800, 480), new Rectangle(0, 0, 800, 480));
            //linuxFB.MouseHandler = new DefaultMouseHandler("/dev/input/event0");
            //linuxFB.KeyboardHandler = new DefaultKeyboardHandler("/dev/input/event2");

            linuxFB.Run(new App());
        }

        public class HeaderTextCell : ViewCell
        {
            public HeaderTextCell()
            {
                var label = new Label() { VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center };

                label.SetBinding(Label.TextProperty, "Text");

                View = label;
            }
        }

        public class ListTextCell : ViewCell
        {
            public ListTextCell()
            {
                var label = new Label() { VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center };

                label.SetBinding(Label.TextProperty, "Name");

                View = label;
            }
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

                var image = new Image() { Aspect = Aspect.AspectFill };

                var description = new Label();

                var listview = new ListView() { RowHeight = 50 };
                listview.ItemTemplate = new DataTemplate(typeof(ListTextCell));
                listview.ItemSelected += (sender, e) => 
                    { 
                        image.Source = (e.SelectedItem as Frog).Image;
                        description.Text = (e.SelectedItem as Frog).Description; 
                    };


                grid.Children.Add(listview, 0, 1, 0, 2);
                grid.Children.Add(image, 1, 0);
                grid.Children.Add(description, 1, 1);

                XmlSerializer serializer = new XmlSerializer(typeof(Frogs));
                StreamReader reader = new StreamReader("frogs.xml");
                var frogs = (Frogs)serializer.Deserialize(reader);
                reader.Close();

                listview.ItemsSource = frogs;

                MainPage = new ContentPage
                {
                    Content = grid
                };
            }
        }
    }
}