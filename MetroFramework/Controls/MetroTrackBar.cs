﻿/**
 * MetroFramework - Modern UI for WinForms
 * 
 * The MIT License (MIT)
 * Copyright (c) 2011 Sven Walter, http://github.com/viperneo
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in the 
 * Software without restriction, including without limitation the rights to use, copy, 
 * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
 * and to permit persons to whom the Software is furnished to do so, subject to the 
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
 * PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
 * CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
 * OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
using MetroFramework.Components;
using MetroFramework.Drawing;
using MetroFramework.Interfaces;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MetroFramework.Controls
{
    [ToolboxBitmap(typeof(TrackBar))]
    [DefaultEvent("Scroll")]
    public class MetroTrackBar : Control, IMetroControl
    {
        #region Interface

        [Category(MetroDefaults.PropertyCategory.Appearance)]
        public event EventHandler<MetroPaintEventArgs> CustomPaintBackground;
        protected virtual void OnCustomPaintBackground(MetroPaintEventArgs e)
        {
            if (GetStyle(ControlStyles.UserPaint) && CustomPaintBackground != null)
            {
                CustomPaintBackground(this, e);
            }
        }

        [Category(MetroDefaults.PropertyCategory.Appearance)]
        public event EventHandler<MetroPaintEventArgs> CustomPaint;
        protected virtual void OnCustomPaint(MetroPaintEventArgs e)
        {
            if (GetStyle(ControlStyles.UserPaint) && CustomPaint != null)
            {
                CustomPaint(this, e);
            }
        }

        [Category(MetroDefaults.PropertyCategory.Appearance)]
        public event EventHandler<MetroPaintEventArgs> CustomPaintForeground;
        protected virtual void OnCustomPaintForeground(MetroPaintEventArgs e)
        {
            if (GetStyle(ControlStyles.UserPaint) && CustomPaintForeground != null)
            {
                CustomPaintForeground(this, e);
            }
        }

        private MetroColorStyle metroStyle = MetroColorStyle.Default;
        [Category(MetroDefaults.PropertyCategory.Appearance)]
        [DefaultValue(MetroColorStyle.Default)]
        public MetroColorStyle Style
        {
            get
            {
                return DesignMode || metroStyle != MetroColorStyle.Default
                    ? metroStyle
                    : StyleManager != null && metroStyle == MetroColorStyle.Default
                    ? StyleManager.Style
                    : StyleManager == null && metroStyle == MetroColorStyle.Default ? MetroDefaults.Style : metroStyle;
            }
            set { metroStyle = value; }
        }

        private MetroThemeStyle metroTheme = MetroThemeStyle.Default;
        [Category(MetroDefaults.PropertyCategory.Appearance)]
        [DefaultValue(MetroThemeStyle.Default)]
        public MetroThemeStyle Theme
        {
            get
            {
                return DesignMode || metroTheme != MetroThemeStyle.Default
                    ? metroTheme
                    : StyleManager != null && metroTheme == MetroThemeStyle.Default
                    ? StyleManager.Theme
                    : StyleManager == null && metroTheme == MetroThemeStyle.Default ? MetroDefaults.Theme : metroTheme;
            }
            set { metroTheme = value; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MetroStyleManager StyleManager { get; set; } = null;

        [DefaultValue(false)]
        [Category(MetroDefaults.PropertyCategory.Appearance)]
        public bool UseCustomBackColor { get; set; } = false;

        [Browsable(false)]
        [DefaultValue(false)]
        [Category(MetroDefaults.PropertyCategory.Appearance)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool UseCustomForeColor { get; set; } = false;

        [Browsable(false)]
        [DefaultValue(false)]
        [Category(MetroDefaults.PropertyCategory.Appearance)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool UseStyleColors { get; set; } = false;

        [Browsable(false)]
        [Category(MetroDefaults.PropertyCategory.Behaviour)]
        [DefaultValue(true)]
        public bool UseSelectable
        {
            get { return GetStyle(ControlStyles.Selectable); }
            set { SetStyle(ControlStyles.Selectable, value); }
        }

        #endregion

        #region Events

        public event EventHandler ValueChanged;
        private void OnValueChanged()
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        public event ScrollEventHandler Scroll;
        private void OnScroll(ScrollEventType scrollType, int newValue)
        {
            Scroll?.Invoke(this, new ScrollEventArgs(scrollType, newValue));
        }


        #endregion

        #region Fields

        [DefaultValue(false)]
        [Category(MetroDefaults.PropertyCategory.Appearance)]
        public bool DisplayFocus { get; set; } = false;

        private int trackerValue = 50;
        [DefaultValue(50)]
        public int Value
        {
            get { return trackerValue; }
            set
            {
                if (value >= barMinimum & value <= barMaximum)
                {
                    trackerValue = value;
                    OnValueChanged();
                    Invalidate();
                }
                else throw new ArgumentOutOfRangeException(nameof(Value), "Value is outside appropriate range (min, max)");
            }
        }

        private int barMinimum = 0;
        [DefaultValue(0)]
        public int Minimum
        {
            get { return barMinimum; }
            set
            {
                if (value < barMaximum)
                {
                    barMinimum = value;
                    if (trackerValue < barMinimum)
                    {
                        trackerValue = barMinimum;
                        ValueChanged?.Invoke(this, new EventArgs());
                    }
                    Invalidate();
                }
                else throw new ArgumentOutOfRangeException(nameof(Minimum), "Minimal value is greather than maximal one");
            }
        }


        private int barMaximum = 100;
        [DefaultValue(100)]
        public int Maximum
        {
            get { return barMaximum; }
            set
            {
                if (value > barMinimum)
                {
                    barMaximum = value;
                    if (trackerValue > barMaximum)
                    {
                        trackerValue = barMaximum;
                        ValueChanged?.Invoke(this, new EventArgs());
                    }
                    Invalidate();
                }
                else throw new ArgumentOutOfRangeException(nameof(Maximum), "Maximal value is lower than minimal one");
            }
        }

        [DefaultValue(1)]
        public int SmallChange { get; set; } = 1;

        [DefaultValue(5)]
        public int LargeChange { get; set; } = 5;

        private int mouseWheelBarPartitions = 10;
        [DefaultValue(10)]
        public int MouseWheelBarPartitions
        {
            get { return mouseWheelBarPartitions; }
            set
            {
                mouseWheelBarPartitions = value > 0 ? value : throw new ArgumentOutOfRangeException(nameof(MouseWheelBarPartitions), "MouseWheelBarPartitions has to be greather than zero");
            }
        }

        private bool isHovered = false;
        private bool isPressed = false;
        private bool isFocused = false;

        #endregion

        #region Constructor

        public MetroTrackBar(int min, int max, int value)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.Selectable |
                     ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.UserMouse |
                     ControlStyles.UserPaint, true);

            BackColor = Color.Transparent;

            Minimum = min;
            Maximum = max;
            Value = value;
        }

        public MetroTrackBar() : this(0, 100, 50) { }

        #endregion

        #region Paint Methods

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            try
            {
                Color backColor = BackColor;

                if (!UseCustomBackColor)
                {
                    backColor = MetroPaint.BackColor.Form(Theme);
                }

                if (backColor.A == 255)
                {
                    e.Graphics.Clear(backColor);
                    return;
                }

                base.OnPaintBackground(e);

                OnCustomPaintBackground(new MetroPaintEventArgs(backColor, Color.Empty, e.Graphics));
            }
            catch
            {
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                if (GetStyle(ControlStyles.AllPaintingInWmPaint))
                {
                    OnPaintBackground(e);
                }

                OnCustomPaint(new MetroPaintEventArgs(Color.Empty, Color.Empty, e.Graphics));
                OnPaintForeground(e);
            }
            catch
            {
                Invalidate();
            }
        }

        protected virtual void OnPaintForeground(PaintEventArgs e)
        {
            Color thumbColor, barColor;

            if (isHovered && !isPressed && Enabled)
            {
                thumbColor = MetroPaint.BackColor.TrackBar.Thumb.Hover(Theme);
                barColor = MetroPaint.BackColor.TrackBar.Bar.Hover(Theme);
            }
            else if (isHovered && isPressed && Enabled)
            {
                thumbColor = MetroPaint.BackColor.TrackBar.Thumb.Press(Theme);
                barColor = MetroPaint.BackColor.TrackBar.Bar.Press(Theme);
            }
            else if (!Enabled)
            {
                thumbColor = MetroPaint.BackColor.TrackBar.Thumb.Disabled(Theme);
                barColor = MetroPaint.BackColor.TrackBar.Bar.Disabled(Theme);
            }
            else
            {
                thumbColor = MetroPaint.BackColor.TrackBar.Thumb.Normal(Theme);
                barColor = MetroPaint.BackColor.TrackBar.Bar.Normal(Theme);
            }

            DrawTrackBar(e.Graphics, thumbColor, barColor);

            if (DisplayFocus && isFocused)
                ControlPaint.DrawFocusRectangle(e.Graphics, ClientRectangle);
        }

        private void DrawTrackBar(Graphics g, Color thumbColor, Color barColor)
        {
            int TrackX = (trackerValue - barMinimum) * (Width - 6) / (barMaximum - barMinimum);

            using (SolidBrush b = new(thumbColor))
            {
                Rectangle barRect = new(0, (Height / 2) - 2, TrackX, 4);
                g.FillRectangle(b, barRect);

                Rectangle thumbRect = new(TrackX, (Height / 2) - 8, 6, 16);
                g.FillRectangle(b, thumbRect);
            }

            using (SolidBrush b = new(barColor))
            {
                Rectangle barRect = new(TrackX + 7, (Height / 2) - 2, Width - TrackX + 7, 4);
                g.FillRectangle(b, barRect);
            }
        }

        #endregion

        #region Focus Methods

        protected override void OnGotFocus(EventArgs e)
        {
            isFocused = true;
            Invalidate();

            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            isFocused = false;
            isHovered = false;
            isPressed = false;
            Invalidate();

            base.OnLostFocus(e);
        }

        protected override void OnEnter(EventArgs e)
        {
            isFocused = true;
            Invalidate();

            base.OnEnter(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            isFocused = false;
            isHovered = false;
            isPressed = false;
            Invalidate();

            base.OnLeave(e);
        }

        #endregion

        #region Keyboard Methods

        protected override void OnKeyDown(KeyEventArgs e)
        {
            isHovered = true;
            isPressed = true;
            Invalidate();

            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            isHovered = false;
            isPressed = false;
            Invalidate();

            base.OnKeyUp(e);

            switch (e.KeyCode)
            {
                case Keys.Down:
                case Keys.Left:
                    SetProperValue(Value - (int)SmallChange);
                    OnScroll(ScrollEventType.SmallDecrement, Value);
                    break;
                case Keys.Up:
                case Keys.Right:
                    SetProperValue(Value + (int)SmallChange);
                    OnScroll(ScrollEventType.SmallIncrement, Value);
                    break;
                case Keys.Home:
                    Value = barMinimum;
                    break;
                case Keys.End:
                    Value = barMaximum;
                    break;
                case Keys.PageDown:
                    SetProperValue(Value - (int)LargeChange);
                    OnScroll(ScrollEventType.LargeDecrement, Value);
                    break;
                case Keys.PageUp:
                    SetProperValue(Value + (int)LargeChange);
                    OnScroll(ScrollEventType.LargeIncrement, Value);
                    break;
            }

            if (Value == barMinimum)
                OnScroll(ScrollEventType.First, Value);

            if (Value == barMaximum)
                OnScroll(ScrollEventType.Last, Value);

            Point pt = PointToClient(Cursor.Position);
            OnMouseMove(new MouseEventArgs(MouseButtons.None, 0, pt.X, pt.Y, 0));
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Tab | ModifierKeys == Keys.Shift)
                return base.ProcessDialogKey(keyData);
            else
            {
                OnKeyDown(new KeyEventArgs(keyData));
                return true;
            }
        }

        #endregion

        #region Mouse Methods

        protected override void OnMouseEnter(EventArgs e)
        {
            isHovered = true;
            Invalidate();

            base.OnMouseEnter(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isPressed = true;
                Invalidate();
            }

            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                Capture = true;
                OnScroll(ScrollEventType.ThumbTrack, trackerValue);
                OnValueChanged();
                OnMouseMove(e);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (Capture & e.Button == MouseButtons.Left)
            {
                ScrollEventType set = ScrollEventType.ThumbPosition;
                Point pt = e.Location;
                int p = pt.X;

                float coef = (float)(barMaximum - barMinimum) / (float)(ClientSize.Width - 3);
                trackerValue = (int)((p * coef) + barMinimum);

                if (trackerValue <= barMinimum)
                {
                    trackerValue = barMinimum;
                    set = ScrollEventType.First;
                }
                else if (trackerValue >= barMaximum)
                {
                    trackerValue = barMaximum;
                    set = ScrollEventType.Last;
                }

                OnScroll(set, trackerValue);
                OnValueChanged();

                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            isPressed = false;
            Invalidate();

            base.OnMouseUp(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            isHovered = false;
            Invalidate();

            base.OnMouseLeave(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            int v = e.Delta / 120 * (barMaximum - barMinimum) / mouseWheelBarPartitions;
            SetProperValue(Value + v);
        }

        #endregion

        #region Overridden Methods

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Invalidate();
        }

        #endregion

        #region Helper Methods

        private void SetProperValue(int val)
        {
            Value = val < barMinimum ? barMinimum : val > barMaximum ? barMaximum : val;
        }

        #endregion
    }
}
