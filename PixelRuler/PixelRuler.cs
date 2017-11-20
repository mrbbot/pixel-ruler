using System;
using System.Drawing;
using System.Windows.Forms;

namespace PixelRuler
{
    public sealed partial class PixelRuler : Form
    {
        private const int Grip = 8;
        private const int FontSize = 14;

        private readonly Font _drawFont;
        private readonly StringFormat _verticalDrawFormat;

        public PixelRuler()
        {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.None;
            DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw, true);
            Resize += Form1_Resize;
            Move += Form1_Move;
            MinimumSize = new Size(Grip * 2, Grip * 2);
            Opacity = 0.8f;
            TopMost = true;
            BackColor = ColorTranslator.FromHtml("#222222");

            _drawFont = new Font("Arial", FontSize);
            _verticalDrawFormat = new StringFormat {FormatFlags = StringFormatFlags.DirectionVertical};

            Form1_Resize(null, null);
            Form1_Move(null, null);
        }

        private int _width, _height;
        private float _xStart, _yStart, _posXStart, _posYStart;
        private Rectangle _topLeft, _top, _topRight, _middleLeft, _middle, _middleRight, _bottomLeft, _bottom, _bottomRight;
        private SizeF _widthSize, _heightSize;

        private void Form1_Resize(object sender, EventArgs e)
        {
            _width = ClientSize.Width;
            _height = ClientSize.Height;

            _topLeft = new Rectangle(0, 0, Grip, Grip);
            _top = new Rectangle(Grip, 0, _width - (Grip * 2), Grip);
            _topRight = new Rectangle(_width - Grip, 0, Grip, Grip);

            _middleLeft = new Rectangle(0, Grip, Grip, _height - (Grip * 2));
            _middle = new Rectangle(Grip, Grip, _width - (Grip * 2), _height - (Grip * 2));
            _middleRight = new Rectangle(_width - Grip, Grip, Grip, _height - (Grip * 2));

            _bottomLeft = new Rectangle(0, _height - Grip, Grip, Grip);
            _bottom = new Rectangle(Grip, _height - Grip, _width - (Grip * 2), Grip);
            _bottomRight = new Rectangle(_width - Grip, _height - Grip, Grip, Grip);

            var graphics = CreateGraphics();
            _widthSize = graphics.MeasureString(_width.ToString(), _drawFont);
            _heightSize = graphics.MeasureString(_height.ToString(), _drawFont, new PointF(0, 0), _verticalDrawFormat);
            _xStart = (_width / 2f) - (_widthSize.Width / 2f);
            _yStart = (_height / 2f) - (_heightSize.Height / 2f);

            var postionSize = graphics.MeasureString($"({ Location.X }, { Location.Y })", _drawFont);
            _posXStart = (_width / 2f) - (postionSize.Width / 2f);
            _posYStart = (_height / 2f) - (postionSize.Height / 2f);
            Refresh();
        }

        private void Form1_Move(object sender, EventArgs e)
        {
            var postionSize = CreateGraphics().MeasureString($"({ Location.X }, { Location.Y })", _drawFont);
            _posXStart = (_width / 2f) - (postionSize.Width / 2f);
            _posYStart = (_height / 2f) - (postionSize.Height / 2f);
            Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawString(_width.ToString(), _drawFont, Brushes.White, _xStart, 0);
            e.Graphics.DrawString(_height.ToString(), _drawFont, Brushes.White, 0, _yStart, _verticalDrawFormat);

            if (_width <= 100 || _height <= 100) return;
            e.Graphics.DrawString(_height.ToString(), _drawFont, Brushes.White, _width - _heightSize.Width, _yStart, _verticalDrawFormat);
            e.Graphics.DrawString(_width.ToString(), _drawFont, Brushes.White, _xStart, _height - _widthSize.Height);

            if(_width > 150 && _height > 150)
                e.Graphics.DrawString($"({ Location.X }, { Location.Y })", _drawFont, Brushes.White, _posXStart, _posYStart);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x84)
            {
                var pos = PointToClient(new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16));

                if(_topLeft.Contains(pos))
                {
                    m.Result = (IntPtr)13;
                    return;
                }
                if (_top.Contains(pos))
                {
                    m.Result = (IntPtr)12;
                    return;
                }
                if (_topRight.Contains(pos))
                {
                    m.Result = (IntPtr)14;
                    return;
                }
                if (_middleLeft.Contains(pos))
                {
                    m.Result = (IntPtr)10;
                    return;
                }
                if (_middle.Contains(pos))
                {
                    m.Result = (IntPtr)2;
                    return;
                }
                if (_middleRight.Contains(pos))
                {
                    m.Result = (IntPtr)11;
                    return;
                }
                if (_bottomLeft.Contains(pos))
                {
                    m.Result = (IntPtr)16;
                    return;
                }
                if (_bottom.Contains(pos))
                {
                    m.Result = (IntPtr)15;
                    return;
                }
                if (_bottomRight.Contains(pos))
                {
                    m.Result = (IntPtr)17;
                    return;
                }
            }
            base.WndProc(ref m);
        }
    }
}
