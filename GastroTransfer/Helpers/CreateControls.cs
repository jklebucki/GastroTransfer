using GastroTransfer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GastroTransfer.Helpers
{
    public static class CreateControls
    {
        public static Button CreateProductButton(Window window, RoutedEventHandler routedEventHandler, ProducedItem item)
        {
            Viewbox box = new Viewbox
            {
                Stretch = Stretch.Uniform,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Width = 160,
            };

            TextBlock text = new TextBlock
            {
                Text = $"{item.Name}",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                Width = 160,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5, 5, 5, 5)
            };
            box.Child = text;

            Button button = new Button()
            {
                Name = $"N_{item.ProducedItemId}",
                Content = box,
                Tag = item.ExternalGroupId,
                Height = 90,
                Width = 180,
                Margin = new Thickness(5, 5, 5, 5),
                FontSize = 24,
                Style = window.FindResource("RoundCorner") as Style
            };
            button.Click += new RoutedEventHandler(routedEventHandler);// Production_Button_Click);
            return button;
        }

        public static Button CreateFilterButton(Window window, RoutedEventHandler routedEventHandler, ProductGroup item)
        {
            Viewbox box = new Viewbox
            {
                Stretch = Stretch.Uniform,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Width = 160,
            };

            TextBlock text = new TextBlock
            {
                Text = $"{item.GroupName}",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                Width = 160,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5, 5, 5, 5)
            };
            box.Child = text;

            Button button = new Button()
            {
                Name = $"N_{item.ExternalGroupId}",
                Content = box,
                Tag = item.ExternalGroupId,
                Height = 35,
                Width = 180,
                Margin = new Thickness(5, 5, 5, 5),
                FontSize = 24,
                Style = window.FindResource("RoundCorner") as Style
            };
            button.Click += new RoutedEventHandler(routedEventHandler);
            return button;
        }
    }
}
