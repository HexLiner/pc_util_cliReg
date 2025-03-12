using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CliSerial;

namespace MI_601_CRUD_emulator
{
    public partial class CliSerialControl : UserControl
    {

        public delegate void cliQuickActionFunc();
        public struct CliQuickAction
        {
            public cliQuickActionFunc function;
            public string actionName;
        }

        public struct CliSerialControlSettings
        {
            public int baudRate;
            public AutoCompleteStringCollection cliAutoCompleteStringCollection;
            public List<CliQuickAction> actions;
        }

        private CliSerialControlSettings settings;
        public CliSerial cli;


        public CliSerialControl()
        {
            InitializeComponent();

            qCmdButton1.Visible = false;
            qCmdButton2.Visible = false;
            qCmdButton3.Visible = false;
            qCmdButton4.Visible = false;
            qCmdButton5.Visible = false;
            qCmdButton6.Visible = false;
            qCmdButton7.Visible = false;
            qCmdButton8.Visible = false;
        }

        public void SetSettings(CliSerialControlSettings settings)
        {
            this.settings = settings;

            CliSettings cliSettings = new CliSettings
            {
                baudRate = settings.baudRate,
                printDelayMs = 1,
                inputTextBox = cliInputTextBox,
                outputRichTextBox = cliOutputRichTextBox,
                clearButton = cliClearButton,
                comPortComboBox = comPortComboBox,
                comPosrBaudNumericUpDown = cliBaudNumericUpDown,
                connectButton = comPortButton
            };
            cli = new CliSerial(cliSettings);
            
            cliInputTextBox.AutoCompleteCustomSource = settings.cliAutoCompleteStringCollection;
            cliInputTextBox.AutoCompleteMode = AutoCompleteMode.Append;
            cliInputTextBox.AutoCompleteSource = AutoCompleteSource.CustomSource;

            if ((settings.actions.Count > 0) && (settings.actions[0].function != null))
            {
                qCmdButton1.Text = settings.actions[0].actionName;
                qCmdButton1.Visible = true;
            }
            if ((settings.actions.Count > 1) && (settings.actions[1].function != null))
            {
                qCmdButton2.Text = settings.actions[1].actionName;
                qCmdButton2.Visible = true;
            }
            if ((settings.actions.Count > 2) && (settings.actions[2].function != null))
            {
                qCmdButton3.Text = settings.actions[2].actionName;
                qCmdButton3.Visible = true;
            }
            if ((settings.actions.Count > 3) && (settings.actions[3].function != null))
            {
                qCmdButton4.Text = settings.actions[3].actionName;
                qCmdButton4.Visible = true;
            }
            if ((settings.actions.Count > 4) && (settings.actions[4].function != null))
            {
                qCmdButton5.Text = settings.actions[4].actionName;
                qCmdButton5.Visible = true;
            }
            if ((settings.actions.Count > 5) && (settings.actions[5].function != null))
            {
                qCmdButton6.Text = settings.actions[5].actionName;
                qCmdButton6.Visible = true;
            }
            if ((settings.actions.Count > 6) && (settings.actions[6].function != null))
            {
                qCmdButton6.Text = settings.actions[6].actionName;
                qCmdButton6.Visible = true;
            }
            if ((settings.actions.Count > 7) && (settings.actions[7].function != null))
            {
                qCmdButton6.Text = settings.actions[7].actionName;
                qCmdButton6.Visible = true;
            }
        }


        public bool SendMessage(string message)
        {
            return cli.SendMessage(message);
        }


        public void LockUserInput()
        {
            cli.LockUserInput();
        }

        public void UnlockUserInput()
        {
            cli.UnlockUserInput();
        }

        public void LockUserOutput()
        {
            cli.LockUserOutput();
        }

        public void UnlockUserOutput()
        {
            cli.UnlockUserOutput();
        }






        private void cliBreakButton_Click(object sender, EventArgs e)
        {
            cli.SendMessage("\x03");
        }

        private void qCmdButton1_Click(object sender, EventArgs e)
        {
            if ((settings.actions.Count > 0) && (settings.actions[0].function != null)) settings.actions[0].function();
        }

        private void qCmdButton2_Click(object sender, EventArgs e)
        {
            if ((settings.actions.Count > 1) && (settings.actions[1].function != null)) settings.actions[1].function();
        }

        private void qCmdButton3_Click(object sender, EventArgs e)
        {
            if ((settings.actions.Count > 2) && (settings.actions[2].function != null)) settings.actions[2].function();
        }

        private void qCmdButton4_Click(object sender, EventArgs e)
        {
            if ((settings.actions.Count > 3) && (settings.actions[3].function != null)) settings.actions[3].function();
        }

        private void qCmdButton5_Click(object sender, EventArgs e)
        {
            if ((settings.actions.Count > 4) && (settings.actions[4].function != null)) settings.actions[4].function();
        }

        private void qCmdButton6_Click(object sender, EventArgs e)
        {
            if ((settings.actions.Count > 5) && (settings.actions[5].function != null)) settings.actions[5].function();
        }

        private void qCmdButton7_Click(object sender, EventArgs e)
        {
            if ((settings.actions.Count > 6) && (settings.actions[6].function != null)) settings.actions[6].function();
        }

        private void qCmdButton8_Click(object sender, EventArgs e)
        {
            if ((settings.actions.Count > 7) && (settings.actions[7].function != null)) settings.actions[7].function();
        }
    }
}
