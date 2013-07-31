using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Chip8Emu.Core;

namespace Chip8Emu
{
    public partial class MainForm : Form
    {
        private Chip8Cpu _cpu;
        private Chip8Interpreter _interpreter;
        private OutputWindow _outputWindow;

        public MainForm()
        {
            InitializeComponent();
        }

        private void ResetAll()
        {
            lvCode.Items.Clear();
            lvRegisters.Items.Clear();
            ResetColours();
            for (int i = 0; i < 16; i++)
            {
                lvRegisters.Items.Add(new ListViewItem(new string[] { "V" + i.ToString("X"), "0000" }));
            }
            lvRegisters.Items.Add(new ListViewItem(new string[] { "I", "0000" }));
            lvRegisters.Items.Add(new ListViewItem(new string[] { "Delay", "0000" }));
            lvRegisters.Items.Add(new ListViewItem(new string[] { "Sound", "0000" }));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_cpu != null)
            {
                toolStripStatusLabel1.Text = _cpu.ProgramCounter.ToString("X4");
                
                for (int i = 0; i < _cpu.V.Length; i++)
                {
                    UpdateItem(lvRegisters.Items[i], _cpu.V[i].ToString("X4"));
                }

                UpdateItem(lvRegisters.Items[lvRegisters.Items.Count - 3], _cpu.I.ToString("X4"));
                UpdateItem(lvRegisters.Items[lvRegisters.Items.Count - 2], _cpu.DelayTimer.ToString("X4"));
                UpdateItem(lvRegisters.Items[lvRegisters.Items.Count - 1], _cpu.SoundTimer.ToString("X4"));
            }
        }

        private void UpdateItem(ListViewItem item, string newValue)
        {
            if (item.SubItems[1].Text != newValue)
            {
                item.ForeColor = Color.Red;
                item.SubItems[1].Text = newValue;
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ResetAll();
                byte[] code = File.ReadAllBytes(ofd.FileName);
                _outputWindow = new OutputWindow();
                _outputWindow.Show();

                _cpu = new Chip8Cpu(_outputWindow.Chip8Control, _outputWindow.Chip8Control);
                _interpreter = new Chip8Interpreter(_cpu, code);

                Chip8Disassembler disassembler = new Chip8Disassembler(_cpu.Memory);
                foreach (var instruction in disassembler.Disassemble())
                {
                    lvCode.Items.Add(new ListViewItem(
                        new string[] 
                        { 
                            instruction.Offset.ToString("X8"), 
                            instruction.Value.ToString("X4"), 
                            instruction.OpCode.Name.ToString() 
                        }) 
                        { Tag = instruction });
                }

                _interpreter.Resumed += _interpreter_Resumed;
                _interpreter.Suspended += _interpreter_Suspended;
                _interpreter.Stepped += _interpreter_Stepped;

                HighlightCurrentInstruction();
                new Thread(_interpreter.InterpetCode).Start();
            }
        }

        private void _interpreter_Stepped(object sender, EventArgs e)
        {
            Invoke(new Action(() =>
            {
                ResetColours();
                HighlightCurrentInstruction();
            }));
        }

        private void _interpreter_Suspended(object sender, EventArgs e)
        {
            HighlightCurrentInstruction();
        }

        private void _interpreter_Resumed(object sender, EventArgs e)
        {
            ResetColours();
        }

        private void HighlightCurrentInstruction()
        {
            foreach (ListViewItem item in lvCode.Items)
            {
                if ((item.Tag as Chip8Instruction).Offset == _cpu.ProgramCounter)
                {
                    item.BackColor = Color.Black;
                    item.ForeColor = lvCode.BackColor;
                    item.EnsureVisible();
                }
                else if (item.BackColor != lvCode.BackColor)
                {
                    item.BackColor = lvCode.BackColor;
                    item.ForeColor = Color.Black;
                }
            }
        }

        private void ResetColours()
        {
            foreach (ListViewItem item in lvRegisters.Items)
            {
                if (item.ForeColor == Color.Red)
                    item.ForeColor = Color.Black;
            }
        }

        private void breakToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _interpreter.Suspend();
        }

        private void resumeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _interpreter.Resume();
        }

        private void stepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Thread(_interpreter.Step).Start();
        }

        private void dumpMemoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < _cpu.Memory.Length; i++)
            {
                if (i % 0x10 == 0)
                    builder.Append("\r\n" + i.ToString("X8") + ":    ");

                builder.Append(_cpu.Memory[i].ToString("X2") + " ");
            }

            richTextBox1.Text = builder.ToString();
        }
    }
}
