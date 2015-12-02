﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MaterialDesignThemes.Wpf
{
    public class ComboBoxPopup : Popup
    {
        public static readonly DependencyProperty UpContentTemplateProperty
            = DependencyProperty.Register(nameof(UpContentTemplateProperty),
                typeof(ControlTemplate),
                typeof(ComboBoxPopup),
                new UIPropertyMetadata(null));

        public ControlTemplate UpContentTemplate
        {
            get { return (ControlTemplate)GetValue(UpContentTemplateProperty); }
            set { SetValue(UpContentTemplateProperty, value); }
        }

        public static readonly DependencyProperty DownContentTemplateProperty
            = DependencyProperty.Register(nameof(DownContentTemplateProperty),
                typeof(ControlTemplate),
                typeof(ComboBoxPopup),
                new UIPropertyMetadata(null));

        public ControlTemplate DownContentTemplate
        {
            get { return (ControlTemplate)GetValue(DownContentTemplateProperty); }
            set { SetValue(DownContentTemplateProperty, value); }
        }

        public static readonly DependencyProperty DefaultContentTemplateProperty
            = DependencyProperty.Register(nameof(DefaultContentTemplateProperty),
                typeof(ControlTemplate),
                typeof(ComboBoxPopup),
                new UIPropertyMetadata(null));

        public ControlTemplate DefaultContentTemplate
        {
            get { return (ControlTemplate)GetValue(DefaultContentTemplateProperty); }
            set { SetValue(DefaultContentTemplateProperty, value); }
        }

        public static readonly DependencyProperty UpVerticalOffsetProperty
            = DependencyProperty.Register(nameof(UpVerticalOffsetProperty),
                typeof(double),
                typeof(ComboBoxPopup),
                new PropertyMetadata(0.0));

        public double UpVerticalOffset
        {
            get { return (double)GetValue(UpVerticalOffsetProperty); }
            set { SetValue(UpVerticalOffsetProperty, value); }
        }

        public static readonly DependencyProperty DownVerticalOffsetProperty
            = DependencyProperty.Register(nameof(DownVerticalOffsetProperty),
                typeof(double),
                typeof(ComboBoxPopup),
                new PropertyMetadata(0.0));

        public double DownVerticalOffset
        {
            get { return (double)GetValue(DownVerticalOffsetProperty); }
            set { SetValue(DownVerticalOffsetProperty, value); }
        }

        public static readonly DependencyProperty DefaultVerticalOffsetProperty
            = DependencyProperty.Register(nameof(DefaultVerticalOffsetProperty),
                typeof(double),
                typeof(ComboBoxPopup),
                new PropertyMetadata(0.0));

        public double DefaultVerticalOffset
        {
            get { return (double)GetValue(DefaultVerticalOffsetProperty); }
            set { SetValue(DefaultVerticalOffsetProperty, value); }
        }

        public ComboBoxPopup()
        {
            this.CustomPopupPlacementCallback = ComboBoxCustomPopupPlacementCallback;
        }

        private void SetChildTemplateIfNeed(ControlTemplate template)
        {
            var contentControl = Child as ContentControl;
            if (contentControl == null) throw new InvalidOperationException("Child must be ContentControl");

            if (!ReferenceEquals(contentControl.Template, template))
            {
                contentControl.Template = template;
            }
        }

        private CustomPopupPlacement[] ComboBoxCustomPopupPlacementCallback(Size popupSize, Size targetSize,
            Point offset)
        {
            var locationFromScreen = this.PlacementTarget.PointToScreen(new Point(0, 0));

            var mainVisual = TreeHelper.FindMainTreeVisual(this.PlacementTarget);

            int screenWidth = (int) DpiHelper.TransformToDeviceX(mainVisual, SystemParameters.PrimaryScreenWidth);
            int screenHeight = (int) DpiHelper.TransformToDeviceY(mainVisual, SystemParameters.PrimaryScreenHeight);

            int locationX = (int)locationFromScreen.X % screenWidth;
            int locationY = (int)locationFromScreen.Y % screenHeight;

            double realOffsetX = (popupSize.Width - targetSize.Width) / 2.0;
            double offsetX = DpiHelper.TransformToDeviceX(mainVisual, offset.X);
            double defaultVerticalOffsetIndepent = DpiHelper.TransformToDeviceY(mainVisual, DefaultVerticalOffset);
            double upVerticalOffsetIndepent = DpiHelper.TransformToDeviceY(mainVisual, UpVerticalOffset);
            double downVerticalOffsetIndepent = DpiHelper.TransformToDeviceY(mainVisual, DownVerticalOffset);

            if (locationX + popupSize.Width - realOffsetX > screenWidth
                || locationX + realOffsetX < 0)
            {
                SetChildTemplateIfNeed(DefaultContentTemplate);

                double newY = locationY + popupSize.Height > screenHeight
                    ? -(defaultVerticalOffsetIndepent + popupSize.Height)
                    : defaultVerticalOffsetIndepent + targetSize.Height;

                return new[] { new CustomPopupPlacement(new Point(offsetX, newY), PopupPrimaryAxis.Horizontal) };
            }
            else if (locationY + popupSize.Height > screenHeight)
            {
                SetChildTemplateIfNeed(UpContentTemplate);

                double newY = upVerticalOffsetIndepent - popupSize.Height + targetSize.Height;

                return new[] { new CustomPopupPlacement(new Point(offsetX, newY), PopupPrimaryAxis.None) };
            }
            else
            {
                SetChildTemplateIfNeed(DownContentTemplate);

                double newY = downVerticalOffsetIndepent;

                return new[] { new CustomPopupPlacement(new Point(offsetX, newY), PopupPrimaryAxis.None) };
            }
        }
    }
}
