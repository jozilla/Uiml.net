using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Shape
{
    public enum Orientation
    {
        HORIZONTAL,
        VERTICAL
    };

    public partial class Line : UserControl
    {
        Orientation m_orientation = Orientation.HORIZONTAL;
        int m_length = 0;
        Color m_color = Color.Green;
        string m_label = "";

        public Line()
        {
            InitializeComponent();
            Size = new Size(0, 0);
            LineColor = Color.Black;
        }

        public Line(int length, Orientation orientation) : this()
        {
            Length = length;
            Orientation = orientation;
        }

        public Line(int length, Orientation orientation, Color color) : this(length,orientation)
        {
            LineColor = color;
        }

        private void initSize()
        {
            if (Orientation == Orientation.HORIZONTAL)
            {
                Size = new Size(Length, 2);
            } 
            else if (Orientation == Orientation.VERTICAL)
            {
                Size = new Size(2, Length);
            }
        }

        public int Length
        {
            get
            {
                return m_length;
            }
            set
            {                
                m_length = value;
                initSize();
            }
        }

        public Orientation Orientation
        {
            get
            {
                return m_orientation;
            }
            set
            {
                m_orientation = value;
                initSize();
            }
        }

        public Color LineColor
        {
            get
            {
                return BackColor;                
            }
            set
            {
                BackColor = value;
            }
        }

        public string Label
        {
            get
            {
                return m_label;
            }
            set
            {
                m_label = value;
            }
        }

    }
}
