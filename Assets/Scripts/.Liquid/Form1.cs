using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace LiquidSim
{

    public partial class Form1 : Form
    {
        public bool Running;

        readonly Liquid _liquid = new Liquid();

        public Form1()
        {
            InitializeComponent();
            
            Size = new Size(1000, 750);

            var buff = new DoubleBufferedControl {Liquid = _liquid};
            panel1.Controls.Add(buff);
            buff.Dock = DockStyle.Fill;
            buff.PaintRequested += pe =>_liquid.Paint(pe.Graphics);
            _liquid.Init(panel1.Width, panel1.Height);
            Running = true;

            Closed += (a,b) => Running = false;
        }

      
        public void Step()
        {
            _liquid.Simulate();

            Invalidate();
            Refresh();
        }
    }
}

//EOF
