using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chip8Emu
{
    public partial class OutputWindow : Form
    {
        public OutputWindow()
        {
            InitializeComponent();
            chip8Control1.Scale = 4;
        }

        public Chip8Control Chip8Control { get { return chip8Control1; } }

        private void chip8Control1_ScaleChanged(object sender, EventArgs e)
        {
            ClientSize = chip8Control1.Resolution;
            Height += menuStrip1.Height;
        }

        private void scaleToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void xToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chip8Control1.Scale = 1;
        }

        private void xToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            chip8Control1.Scale = 2;
        }

        private void xToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            chip8Control1.Scale = 4;
        }

        private void xToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            chip8Control1.Scale = 8;
        }

        private void xToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            chip8Control1.Scale = 16;
        }
    }
}
