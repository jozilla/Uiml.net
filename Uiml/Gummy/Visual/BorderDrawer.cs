using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;


namespace Uiml.Gummy.Visual
{
    public class BorderDrawer
    {
        private Color borderColor = Color.Black;

        private static int WM_NCPAINT = 0x0085;
        private static int WM_ERASEBKGND = 0x0014;
        private static int WM_PAINT = 0x000F;

        public void DrawBorder(ref Message message, int width, int height)
        {
            if (message.Msg == WM_NCPAINT || message.Msg == WM_ERASEBKGND || message.Msg == WM_PAINT)
            {
                IntPtr hdc = GetDCEx(message.HWnd, (IntPtr)1, 1 | 0x0020);

                if (hdc != IntPtr.Zero)
                {
                    Graphics graphics = Graphics.FromHdc(hdc);
                    Rectangle rectangle = new Rectangle(0, 0, width, height);
                    ControlPaint.DrawBorder(graphics, rectangle, borderColor, ButtonBorderStyle.Solid);

                    message.Result = (IntPtr)1;
                    ReleaseDC(message.HWnd, hdc);
                }
            }
        }

        public Color BorderColor
        {
            get { return borderColor; }
            set { borderColor = value; }
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetDCEx(IntPtr hwnd, IntPtr hrgnclip, uint fdwOptions);

        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hwnd, IntPtr hDC);
    }
}
