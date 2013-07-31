using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Chip8Emu.Core;

namespace Chip8Emu
{
    public class Chip8Control : Control, IGraphicsDevice, IInputDevice
    {
        private class KeyData : ICloneable 
        {
            public KeyData(Keys key)
            {
                Key = key;
            }

            public Keys Key;
            public bool IsPressed;
            public bool JustPressed;

            public object Clone()
            {
                return MemberwiseClone();
            }
        }

        public event EventHandler ScaleChanged;

        private System.Windows.Forms.Timer _refreshTimer;
        private int _scale;
        private Point _smallest = new Point(100, 100), _largest = new Point(-1, -1);//, _lastsmallest = new Point(100, 100), _lastlargest = new Point(-1, -1);
        private bool _canRefresh = true;
        public bool[,] _pixels;

        private KeyData[] _keyMapping;

        public Chip8Control()
        {
            Scale = 4;
            _refreshTimer = new System.Windows.Forms.Timer();
            _refreshTimer.Interval = 10;
            _refreshTimer.Tick += _refreshTimer_Tick;
             _refreshTimer.Start();
            _pixels = new bool[64 + 1, 32 + 1];
            _keyMapping = CreateKeyMapping();
        }

        public void ClearScreen()
        {
            _pixels = new bool[64 + 1, 32 + 1];
            Invalidate();
        }

        public int Scale 
        {
            get { return _scale; }
            set 
            {
                _scale = value;
                OnScaleChanged(EventArgs.Empty);
                Invalidate(); 
            }
        }

        public Size Resolution { get { return new Size((int)(64 * Scale), (int)(32 * Scale)); } }


        public void DrawSprite(byte x, byte y, byte[] rows)
        {
            _canRefresh = false;
            for (int row = 0; row < rows.Length; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    int xx = (x + (7 - col));
                    int yy = (y + row);

                    if (xx < 0 || xx >= _pixels.GetLength(0) || yy < 0 || yy >= _pixels.GetLength(1))
                        continue;

                    if ((rows[row] & (1 << col)) == 1 << col)
                    {
                        if (xx < _smallest.X)
                            _smallest.X = xx;
                        if (xx > _largest.X)
                            _largest.X = xx;
                        if (yy < _smallest.Y)
                            _smallest.Y = yy;
                        if (yy > _largest.Y)
                            _largest.Y = yy;

                        _pixels[xx, yy] = !_pixels[xx, yy];
                    }
                }
            }

            Invalidate(new Rectangle(x * Scale, y * Scale, 8 * Scale, rows.Length * Scale));
            _canRefresh = true;
        }

        public bool IsKeyDown(int key)
        {
            return _keyMapping[key].IsPressed;
        }

        public int AwaitKey()
        {
            while (true)
            {
                for (int i = 0; i < _keyMapping.Length; i++)
                {
                    if (_keyMapping[i].JustPressed)
                        return i;
                }
                Thread.Sleep(1);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            for (int x = 0; x < _pixels.GetLength(0); x++)
            {
                for (int y = 0; y < _pixels.GetLength(1); y++)
                {
                    if (_pixels[x, y])
                    {
                        e.Graphics.FillRectangle(Brushes.White, x * Scale, y * Scale, Scale, Scale);
                    }
                }
            }
            
            base.OnPaint(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            var item = _keyMapping.FirstOrDefault(x => x.Key == e.KeyCode);
            if (item != null)
                item.IsPressed = true;

            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            var item = _keyMapping.FirstOrDefault(x => x.Key == e.KeyCode);
            if (item != null)
            {
                item.IsPressed = false;
                item.JustPressed = true;
            }
            base.OnKeyUp(e);
        }
       
        protected virtual void OnScaleChanged(EventArgs e)
        {
            if (ScaleChanged != null)
                ScaleChanged(this, e);
        }

        private void _refreshTimer_Tick(object sender, EventArgs e)
        {
            foreach (var keyData in _keyMapping)
                keyData.JustPressed = false;
            //  Invalidate();

            //if (_largest.X != -1 && _canRefresh)
            //{
            //    Parent.Text = _smallest.ToString() + " - " + _largest.ToString();
            //    Invalidate(new Rectangle(_smallest.X * Scale, _smallest.Y * Scale, (_largest.X - _smallest.X) * Scale, (_largest.Y - _smallest.Y) * Scale));
            //    _smallest = new Point(100, 100);
            //    _largest = new Point(-1, -1);
            //}
            
        }

        private KeyData[] CreateKeyMapping()
        { 
            return new KeyData[]
            {
                new KeyData(Keys.NumPad0),   // 0
                new KeyData(Keys.NumPad1),   // 1
                new KeyData(Keys.NumPad2),   // 2
                new KeyData(Keys.NumPad3),   // 3
                new KeyData(Keys.NumPad4),   // 4
                new KeyData(Keys.NumPad5),   // 5
                new KeyData(Keys.NumPad6),   // 6
                new KeyData(Keys.NumPad7),   // 7
                new KeyData(Keys.NumPad8),   // 8
                new KeyData(Keys.NumPad9),   // 9
                new KeyData(Keys.Home),    // A
                new KeyData(Keys.End),  // B
                new KeyData(Keys.PageUp),  // C
                new KeyData(Keys.PageDown),       // D
                new KeyData(Keys.Subtract),     // E
                new KeyData(Keys.Add),    // F

            };
        }

        private T[] CloneArray<T>(T[] array)
        {
            if (array[0] is ICloneable)
            {
                T[] newArray = new T[array.Length];
                for (int i = 0; i < array.Length; i++)
                    newArray[i] = (T)((ICloneable)array[i]).Clone();
                return newArray;
            }
            else
                return array.Clone() as T[];
        }
    }
}
