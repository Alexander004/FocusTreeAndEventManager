﻿using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using FocusTreeManager.Model;
using FocusTreeManager.Model.TabModels;
using GalaSoft.MvvmLight.Messaging;

namespace FocusTreeManager.Views.UserControls
{
    /// <summary>
    /// Interaction logic for Focus.xaml
    /// </summary>
    public partial class Focus : UserControl
    {
        private Point OldPoint;

        private readonly DispatcherTimer dispatcherTimer;

        public Focus()
        {
            InitializeComponent();
            loadLocales();
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
            //Timer
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            dispatcherTimer.Start();
        }

        private void StartImageDispatcher()
        {
            FocusModel model = DataContext as FocusModel;
            if (model == null) return;
            Dispatcher.Invoke(() =>
            {
                Binding binding = new Binding
                {
                    Path = new PropertyPath("Icon"),
                    Source = model
                };
                BindingOperations.SetBinding(FocusIcon, Image.SourceProperty, binding);
            });
        }

        private void NotificationMessageReceived(NotificationMessage msg)
        {
            if (msg.Notification == "ChangeLanguage")
            {
                loadLocales();
            }
        }

        private void loadLocales()
        {
            ResourceDictionary resourceLocalization = new ResourceDictionary
            {
                Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative)
            };
            Resources.MergedDictionaries.Add(resourceLocalization);
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            DetectPositionPoints();
        }

        public void DetectPositionPoints()
        {
            DependencyObject parent = VisualTreeHelper.GetParent(this);
            while (!(parent is Grid))
            {
                if (parent == null)
                {
                    dispatcherTimer.Stop();
                    return;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
            Point position = TranslatePoint(new Point(1, 1), (FrameworkElement)parent);
            //If the focus has not changed position
            if (OldPoint == position)
            {
                return;
            }
            OldPoint = position;
            FocusModel model = DataContext as FocusModel;
            if (model == null) return;
            model.setPoints(new Point(position.X + (RenderSize.Width / 2), position.Y + 40),
                new Point(position.X + (RenderSize.Width / 2), position.Y + (RenderSize.Height)),
                new Point(position.X, position.Y + (RenderSize.Height / 2)),
                new Point(position.X + (RenderSize.Width), position.Y + (RenderSize.Height / 2)));
            ((FocusGridModel)((FrameworkElement)parent).DataContext).UpdateFocus(model);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //If key is not return
            if (e.Key != Key.Return) return;
            TextBoxName.Visibility = Visibility.Hidden;
            ReleaseMouseCapture();
            e.Handled = true;
        }

        private void MenuRenameFocus_Click(object sender, RoutedEventArgs e)
        {
            TextBoxName.Visibility = Visibility.Visible;
            CaptureMouse();
        }

        private void VisualFocus_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //If the mouse is not captured
            if (!IsMouseCaptured) return;
            Window window = Window.GetWindow(this);
            if (window != null)
            {
                Point pos = e.GetPosition(window);
                Rect rect = TransformToVisual(window)
                    .TransformBounds(LayoutInformation.GetLayoutSlot(this));
                if (!rect.Contains(pos))
                {
                    TextBoxName.Visibility = Visibility.Hidden;
                    ReleaseMouseCapture();
                }
            }
        }

        private void VisualFocus_Loaded(object sender, RoutedEventArgs e)
        {            
            //Image loading async
            StartImageDispatcher();
        }
    }
}
