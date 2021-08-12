using MetroFramework.Components;
using MetroFramework.Drawing;
using MetroFramework.Interfaces;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MetroFramework.Controls
{
    public class MetroContextMenu : ContextMenuStrip, IMetroControl
    {
        #region Interface

        [Category("Metro Appearance")]
        public event EventHandler<MetroPaintEventArgs> CustomPaintBackground;
        protected virtual void OnCustomPaintBackground(MetroPaintEventArgs e)
        {
            if (GetStyle(ControlStyles.UserPaint) && CustomPaintBackground != null)
            {
                CustomPaintBackground(this, e);
            }
        }

        [Category("Metro Appearance")]
        public event EventHandler<MetroPaintEventArgs> CustomPaint;
        protected virtual void OnCustomPaint(MetroPaintEventArgs e)
        {
            if (GetStyle(ControlStyles.UserPaint) && CustomPaint != null)
            {
                CustomPaint(this, e);
            }
        }

        [Category("Metro Appearance")]
        public event EventHandler<MetroPaintEventArgs> CustomPaintForeground;
        protected virtual void OnCustomPaintForeground(MetroPaintEventArgs e)
        {
            if (GetStyle(ControlStyles.UserPaint) && CustomPaintForeground != null)
            {
                CustomPaintForeground(this, e);
            }
        }

        private MetroColorStyle metroStyle = MetroColorStyle.Default;
        [Category("Metro Appearance")]
        [DefaultValue(MetroColorStyle.Default)]
        public MetroColorStyle Style
        {
            get
            {
                return DesignMode || metroStyle != MetroColorStyle.Default
                    ? metroStyle
                    : StyleManager != null && metroStyle == MetroColorStyle.Default
                    ? StyleManager.Style
                    : StyleManager == null && metroStyle == MetroColorStyle.Default ? MetroColorStyle.Blue : metroStyle;
            }
            set { metroStyle = value; }
        }

        private MetroThemeStyle metroTheme = MetroThemeStyle.Default;
        [Category("Metro Appearance")]
        [DefaultValue(MetroThemeStyle.Default)]
        public MetroThemeStyle Theme
        {
            get
            {
                return DesignMode || metroTheme != MetroThemeStyle.Default
                    ? metroTheme
                    : StyleManager != null && metroTheme == MetroThemeStyle.Default
                    ? StyleManager.Theme
                    : StyleManager == null && metroTheme == MetroThemeStyle.Default ? MetroThemeStyle.Light : metroTheme;
            }
            set { metroTheme = value; }
        }

        private MetroStyleManager metroStyleManager = null;
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MetroStyleManager StyleManager
        {
            get { return metroStyleManager; }
            set
            {
                metroStyleManager = value;
                SetTheme();
            }
        }

        [DefaultValue(false)]
        [Category("Metro Appearance")]
        public bool UseCustomBackColor { get; set; } = false;

        [DefaultValue(false)]
        [Category("Metro Appearance")]
        public bool UseCustomForeColor { get; set; } = false;

        [DefaultValue(false)]
        [Category("Metro Appearance")]
        public bool UseStyleColors { get; set; } = false;

        [Browsable(false)]
        [Category("Metro Behaviour")]
        [DefaultValue(false)]
        public bool UseSelectable
        {
            get { return GetStyle(ControlStyles.Selectable); }
            set { SetStyle(ControlStyles.Selectable, value); }
        }

        #endregion


        public MetroContextMenu(IContainer Container)
        {
            if (Container != null)
            {
                Container.Add(this);
            }
        }

        private void SetTheme()
        {
            BackColor = MetroPaint.BackColor.Form(Theme);
            ForeColor = MetroPaint.ForeColor.Button.Normal(Theme);
            Renderer = new MetroCTXRenderer(Theme, Style);
        }

        private class MetroCTXRenderer : ToolStripProfessionalRenderer
        {
            readonly MetroThemeStyle _theme;
            public MetroCTXRenderer(MetroThemeStyle Theme, MetroColorStyle Style) : base(new ContextColors(Theme, Style))
            {
                _theme = Theme;
            }

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                e.TextColor = MetroPaint.ForeColor.Button.Normal(_theme);
                base.OnRenderItemText(e);
            }
        }

        private class ContextColors : ProfessionalColorTable
        {
            readonly MetroThemeStyle _theme = MetroThemeStyle.Light;
            readonly MetroColorStyle _style = MetroColorStyle.Blue;

            public ContextColors(MetroThemeStyle Theme, MetroColorStyle Style)
            {
                _theme = Theme;
                _style = Style;
            }

            public override Color MenuItemSelected
            {
                get { return MetroPaint.GetStyleColor(_style); }
            }

            public override Color MenuBorder
            {
                get { return MetroPaint.BackColor.Form(_theme); }
            }

            public override Color ToolStripBorder
            {
                get { return MetroPaint.GetStyleColor(_style); }
            }

            public override Color MenuItemBorder
            {
                get { return MetroPaint.GetStyleColor(_style); }
            }

            public override Color ToolStripDropDownBackground
            {
                get { return MetroPaint.BackColor.Form(_theme); }
            }

            public override Color ImageMarginGradientBegin
            {
                get { return MetroPaint.BackColor.Form(_theme); }
            }

            public override Color ImageMarginGradientMiddle
            {
                get { return MetroPaint.BackColor.Form(_theme); }
            }

            public override Color ImageMarginGradientEnd
            {
                get { return MetroPaint.BackColor.Form(_theme); }
            }
        }
    }
}
