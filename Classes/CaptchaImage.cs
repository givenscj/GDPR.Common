// Copyright (C) 2007 XtractPro (Data Extraction Magazine)
// http://www.xtractpro.com, support@xtractpro.com
//
// This code is provided AS IS, with NO WARRANTY expressed or implied.
// Any use of this free open source code is at your own risk.
//
// You are hereby granted the right to use it and change it,
// provided that you acknowledge XtractPro somewhere in your
// source files, documentation, web site and application.

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing.Design;

namespace GDPR.Common.Classes
{
    /// <summary>
    /// CAPTCHA Image generation, on each Image property call,
    /// based on all other customizable property values.
    /// </summary>
    [DefaultProperty("Text")]
    [Serializable]
    public class CaptchaImage
    {
        private Random _random = new Random();

        public CaptchaImage() { Text = ""; }

        #region Bitmap

        [Browsable(false)]
        [Category("Bitmap")]
        [Description("Generated image")]
        public Bitmap Image
        {
            get
            {
                Bitmap bitmap = new Bitmap(_width, _height,
                    PixelFormat.Format32bppPArgb);

                Graphics g = null;
                Font font = null;
                HatchBrush textBrush = null;
                HatchBrush backBrush = null;
                try
                {
                    // Fill Background
                    g = Graphics.FromImage(bitmap);
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    Rectangle rect = new Rectangle(0, 0, _width, _height);
                    textBrush = new HatchBrush(_textStyle,
                        _textForeColor, _textBackColor);
                    backBrush = new HatchBrush(_backStyle,
                        _backForeColor, _backBackColor);
                    g.FillRectangle(backBrush, rect);

                    // Write Text
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;
                    stringFormat.LineAlignment = StringAlignment.Center;
                    font = new Font(_fontName, _fontSize, _fontStyle);
                    GraphicsPath path = new GraphicsPath();
                    path.AddString(_text, font.FontFamily,
                        (int)font.Style, font.Size, rect, stringFormat);

                    // Warp Text
                    if (_warp > 0)
                    {
                        Matrix matrix = new Matrix();
                        float warp = 10 - _warp * 6 / 100;
                        PointF[] points = {
                            new PointF(_random.Next(rect.Width) / warp,
                                _random.Next(rect.Height) / warp),
                            new PointF(rect.Width - _random.Next(rect.Width) / warp,
                                _random.Next(rect.Height) / warp),
                            new PointF(_random.Next(rect.Width) / warp,
                                rect.Height - _random.Next(rect.Height) / warp),
                            new PointF(rect.Width - _random.Next(rect.Width) / warp,
                                rect.Height - _random.Next(rect.Height) / warp) };
                        path.Warp(points, rect, matrix, WarpMode.Perspective, 0F);
                    }
                    g.FillPath(textBrush, path);

                    // Random Noise over Text
                    if (_noise > 0)
                    {
                        int maxDim = Math.Max(rect.Width, rect.Height);
                        int radius = (int)(maxDim * _noise / 3000);
                        int maxGran = (int)(rect.Width * rect.Height
                            / (100 - (_noise >= 90 ? 90 : _noise)));
                        for (int i = 0; i < maxGran; i++)
                            g.FillEllipse(textBrush,
                                _random.Next(rect.Width), _random.Next(rect.Height),
                                _random.Next(radius), _random.Next(radius));
                    }

                    // Draw Lines over Text
                    if (_lines > 0)
                    {
                        int lines = ((int)_lines / 30) + 1;
                        using (Pen pen = new Pen(textBrush, 1))
                            for (int i = 0; i < lines; i++)
                            {
                                PointF[] points = new PointF[lines > 2 ? lines - 1 : 2];
                                for (int j = 0; j < points.Length; j++)
                                    points[j] = new PointF(_random.Next(rect.Width),
                                        _random.Next(rect.Height));
                                g.DrawCurve(pen, points, 1.75F);
                            }
                    }
                }
                finally
                {
                    // Dispose GDI+ objects
                    if (g != null)
                        g.Dispose();
                    if (font != null)
                        font.Dispose();
                    if (textBrush != null)
                        textBrush.Dispose();
                    if (backBrush != null)
                        backBrush.Dispose();
                }

                return bitmap;
            }
        }

        private int _width = 275;
        [Category("Bitmap")]
        [Description("Image Width")]
        public int Width
        {
            get { return _width; }
            set
            {
                if (value < 10 || value > 1000)
                    throw new Exception("Image Width must be between 10 and 1000");
                _width = value;
            }
        }

        private int _height = 100;
        [Category("Bitmap")]
        [Description("Image Height")]
        public int Height
        {
            get { return _height; }
            set
            {
                if (value < 10 || value > 1000)
                    throw new Exception("Image Height must be between 10 and 1000");
                _height = value;
            }
        }
        #endregion

        #region Effects

        private int _noise = 100;
        [Category("Effects")]
        [Description("Noise Factor")]
        public int NoiseFactor
        {
            get { return _noise; }
            set
            {
                if (value < 0 || value > 100)
                    throw new Exception("Noise Factor must be between 0 and 100");
                _noise = value;
            }
        }

        private int _warp = 100;
        [Category("Effects")]
        [Description("Warp Factor")]
        public int WarpFactor
        {
            get { return _warp; }
            set
            {
                if (value < 0 || value > 100)
                    throw new Exception("Warp Factor must be between 0 and 100");
                _warp = value;
            }
        }

        private int _lines = 100;
        [Category("Effects")]
        [Description("Lines Factor")]
        public int LinesFactor
        {
            get { return _lines; }
            set
            {
                if (value < 0 || value > 100)
                    throw new Exception("Lines Factor must be between 0 and 100");
                _lines = value;
            }
        }
        #endregion

        #region Brushes

        public HatchStyle _textStyle = HatchStyle.NarrowHorizontal;
        [Category("Hatch Brush Text")]
        [Description("Hatch Brush Text Style")]
        public HatchStyle TextStyle
        {
            get { return _textStyle; }
            set { _textStyle = value; }
        }

        private Color _textForeColor = Color.Black;
        [Category("Hatch Brush Text")]
        [Description("Hatch Brush Text Fore Color")]
        public Color TextForeColor
        {
            get { return _textForeColor; }
            set { _textForeColor = value; }
        }

        private Color _textBackColor = Color.DarkGray;
        [Category("Hatch Brush Text")]
        [Description("Hatch Brush Text Background Color")]
        public Color TextBackColor
        {
            get { return _textBackColor; }
            set { _textBackColor = value; }
        }

        public HatchStyle _backStyle = HatchStyle.DottedGrid;
        [Category("Hatch Brush Background")]
        [Description("Hatch Brush Background Style")]
        public HatchStyle BackgroundStyle
        {
            get { return _backStyle; }
            set { _backStyle = value; }
        }

        private Color _backForeColor = Color.LightGray;
        [Category("Hatch Brush Background")]
        [Description("Hatch Brush Background Fore Color")]
        public Color BackgroundForeColor
        {
            get { return _backForeColor; }
            set { _backForeColor = value; }
        }

        private Color _backBackColor = Color.White;
        [Category("Hatch Brush Background")]
        [Description("Hatch Brush Background Background Color")]
        public Color BackgroundBackColor
        {
            get { return _backBackColor; }
            set { _backBackColor = value; }
        }
        #endregion

        #region Text

        private string _text = "";
        [Category("Text"),
        Description("Text string. Empty to auto-generate random string.")]
        public string Text
        {
            get { return _text; }
            set
            {
                if (value == null
                    || (value.Length != 0 && value.Length < 2)
                    || value.Length > 10)
                    throw new Exception("Text Length must be between 2 and 10");

                _text = value;
                if (_text.Length == 0)
                    for (int i = 0; i < _length; i++)
                    {
                        char c;
                        do
                            c = _charSet[_random.Next(0, _charSet.Length)];
                        while (_unique && _length <= _charSet.Length && _text.IndexOf(c) >= 0);
                        _text += c;
                    }
                _length = _text.Length;
            }
        }

        private string _charSet = "23456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz";
        [Category("Text")]
        [Description("Character set for auto-text generation")]
        public string CharacterSet
        {
            get { return _charSet; }
            set
            {
                if (value == null || value.Length < 2)
                    throw new Exception("Minimum character set size is 2");
                _charSet = value;
                Text = "";
            }
        }

        private int _length = 8;
        [Category("Text")]
        [Description("Text Length")]
        public int Length
        {
            get { return _length; }
            set
            {
                if (value < 2 || value > 10)
                    throw new Exception("Text Length must be between 2 and 10");
                _length = value;
                Text = "";
            }
        }

        private bool _unique = true;
        [Category("Text")]
        [Description("Unique characters or not")]
        public bool Unique
        {
            get { return _unique; }
            set
            {
                _unique = value;
                Text = "";
            }
        }
        #endregion

        #region Font

        private string _fontName = "Tahoma";
        [Category("Font")]
        [Description("Font Name")]
        [TypeConverter(typeof(FontConverter.FontNameConverter))]
        [Editor("System.Drawing.Design.FontNameEditor", typeof(UITypeEditor))]
        public string FontName
        {
            get { return _fontName; }
            set { _fontName = value; }
        }

        private int _fontSize = 40;
        [Category("Font")]
        [Description("Font Size")]
        public int FontSize
        {
            get { return _fontSize; }
            set { _fontSize = value; }
        }

        private FontStyle _fontStyle = FontStyle.Bold;
        [Category("Font")]
        [Description("Font Style")]
        public FontStyle FontStyle
        {
            get { return _fontStyle; }
            set { _fontStyle = value; }
        }
        #endregion
    }
}
