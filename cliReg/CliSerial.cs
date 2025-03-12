
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;


public class CliSerial
{
    private TextBox inputTextBox;
    private RichTextBox outputRichTextBox;
    private Button clearButton;
    private ComboBox comPortComboBox;
    private NumericUpDown comPosrBaudNumericUpDown;
    private Button connectButton;

    private SerialPort serialPort1;
    private bool isConnect;
    private bool isUserInputLocked = false;
    private bool isUserOutputLocked = false;


    private const UInt32 stackSize = 10;
    private string currMsgText;
    private string[] msgStack = new string[stackSize];
    private UInt32 stackUsedSize = 0;
    private UInt32 stackPrevWriteIndex;
    private UInt32 stackNextWriteIndex;
    private UInt32 stackCurrentReadIndex;
    private bool stackIsCurrentText;



    public struct CliSettings
    {
        public int baudRate;
        public int printDelayMs;   // delay between print symbols
        public TextBox inputTextBox;
        public RichTextBox outputRichTextBox;
        public Button clearButton;
        public ComboBox comPortComboBox;
        public NumericUpDown comPosrBaudNumericUpDown;
        public Button connectButton;
    };
    private CliSettings settings;

    public delegate void DataReceived(string data);
    public event DataReceived DataReceivedEvent;



    public CliSerial(CliSettings settings)
    {
        this.settings = settings;

        this.inputTextBox = settings.inputTextBox;
        this.outputRichTextBox = settings.outputRichTextBox;
        this.clearButton = settings.clearButton;
        this.comPortComboBox = settings.comPortComboBox;
        this.comPosrBaudNumericUpDown = settings.comPosrBaudNumericUpDown;
        this.connectButton = settings.connectButton;

        this.comPosrBaudNumericUpDown.Minimum = 1;
        this.comPosrBaudNumericUpDown.Maximum = 921600;
        this.comPosrBaudNumericUpDown.Value = settings.baudRate;

        System.ComponentModel.IContainer components = new System.ComponentModel.Container();
        serialPort1 = new System.IO.Ports.SerialPort(components);
        serialPort1.BaudRate = settings.baudRate;
        serialPort1.DtrEnable = true;
        serialPort1.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        isConnect = false;
        inputTextBox.Enabled = false;

        this.inputTextBox.KeyDown += new KeyEventHandler(InputTextBox_KeyDown);
        this.inputTextBox.KeyUp += new KeyEventHandler(InputTextBox_KeyUp);
        this.outputRichTextBox.TextChanged += new EventHandler(OutputRichTextBox_TextChanged);
        this.clearButton.Click += new EventHandler(clearButton_Click);
        this.connectButton.Click += new EventHandler(ConnectButton_Click);
        this.comPortComboBox.DropDown += new EventHandler(ComPortComboBox_DropDown);
    }


    public bool SendMessage(string message)
    {
        if (!serialPort1.IsOpen)
        {
            if (isConnect)
            {
                try
                {
                    serialPort1.Open();
                }
                catch
                {

                }
            }
            if (!serialPort1.IsOpen) {
                MessageBox.Show("COM port closed!");

                connectButton.Text = "Connect";
                inputTextBox.Enabled = false;
                comPosrBaudNumericUpDown.Enabled = true;
                comPortComboBox.Enabled = true;
                serialPort1.Close();
                isConnect = false;

                return false;
            }
        }

        if (settings.printDelayMs == 0) serialPort1.Write(message);
        else
        {
            for (int i = 0; i < message.Length; i++)
            {
                serialPort1.Write(message[i].ToString());
                Thread.Sleep(settings.printDelayMs);
            }
        }

        return true;
    }


    public void LockUserInput()
    {
        isUserInputLocked = true;
        inputTextBox.Enabled = false;
    }

    public void UnlockUserInput()
    {
        if (isConnect) inputTextBox.Enabled = true;
        isUserInputLocked = false;
    }

    public void LockUserOutput()
    {
        isUserOutputLocked = true;
    }

    public void UnlockUserOutput()
    {
        isUserOutputLocked = false;
    }




    delegate void Del(string text);
    private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
    {
        SerialPort sp = (SerialPort)sender;
        string data = sp.ReadExisting();

        data = data.Replace("\r", "");
        DataReceivedEvent(data);
        if (!isUserOutputLocked) outputRichTextBox.Invoke(new Del((s) => outputRichTextBox.Text += s), data);
    }


    private void OutputRichTextBox_TextChanged(object sender, EventArgs e)
    {
        outputRichTextBox.SelectionStart = outputRichTextBox.Text.Length;
        outputRichTextBox.ScrollToCaret();
    }


    private void InputTextBox_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Up)
        {
            inputTextBox.SelectionStart = inputTextBox.Text.Length;
        }
        else if (e.KeyCode == Keys.Down)
        {
            inputTextBox.SelectionStart = inputTextBox.Text.Length;
        }
    }


    private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            if (!SendMessage(inputTextBox.Text + "\r")) return;

            stackIsCurrentText = true;
            inputTextBox.AutoCompleteMode = AutoCompleteMode.Append;

            if (inputTextBox.Text != "")
            {
                if (stackUsedSize > 0)
                {
                    if (inputTextBox.Text == msgStack[stackPrevWriteIndex])
                    {
                        inputTextBox.Clear();
                        return;
                    }
                }
                if (stackUsedSize < (stackSize - 1)) stackUsedSize++;
                msgStack[stackNextWriteIndex] = inputTextBox.Text;
                stackPrevWriteIndex = stackNextWriteIndex;
                stackCurrentReadIndex = stackNextWriteIndex;
                stackNextWriteIndex++;
                if (stackNextWriteIndex >= stackSize) stackNextWriteIndex = 0;
            }
            inputTextBox.Clear();
        }
        else if (e.KeyCode == Keys.Up)
        {
            if (stackUsedSize == 0) return;
            if (stackCurrentReadIndex == stackNextWriteIndex) return;

            if (!stackIsCurrentText)
            {
                if (stackCurrentReadIndex == 0)
                {
                    if (stackUsedSize != stackNextWriteIndex) stackCurrentReadIndex = stackUsedSize;
                    else return;
                }
                else stackCurrentReadIndex--;
            }

            if (stackIsCurrentText)
            {
                inputTextBox.AutoCompleteMode = AutoCompleteMode.None;
                currMsgText = inputTextBox.Text;
            }

            inputTextBox.Clear();
            inputTextBox.Text = msgStack[stackCurrentReadIndex];
            inputTextBox.SelectionStart = inputTextBox.Text.Length;

            stackIsCurrentText = false;
        }
        else if (e.KeyCode == Keys.Down)
        {
            if (stackUsedSize == 0) return;
            if (stackIsCurrentText) return;

            if (stackCurrentReadIndex == stackPrevWriteIndex)
            {
                stackIsCurrentText = true;
                inputTextBox.AutoCompleteMode = AutoCompleteMode.Append;
                inputTextBox.Clear();
                inputTextBox.Text = currMsgText;
                inputTextBox.SelectionStart = inputTextBox.Text.Length;
            }
            else
            {
                stackCurrentReadIndex++;
                if (stackCurrentReadIndex > stackUsedSize) stackCurrentReadIndex = 0;

                inputTextBox.Clear();
                inputTextBox.Text = msgStack[stackCurrentReadIndex];
                inputTextBox.SelectionStart = inputTextBox.Text.Length;
            }
        }
        else if (e.KeyCode == Keys.Escape)
        {
            stackIsCurrentText = true;
            inputTextBox.Clear();
            stackCurrentReadIndex = stackPrevWriteIndex;
            // Reboot AutoComplete state..
            inputTextBox.AutoCompleteMode = AutoCompleteMode.None;
            inputTextBox.AutoCompleteMode = AutoCompleteMode.Append;
        }
    }


    private void ConnectButton_Click(object sender, EventArgs e)
    {
        if (isConnect)
        {
            connectButton.Text = "Connect";
            inputTextBox.Enabled = false;
            comPosrBaudNumericUpDown.Enabled = true;
            comPortComboBox.Enabled = true;
            serialPort1.Close();
            isConnect = false;
        }
        else
        {
            if (comPortComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Incorrect COM port!");
                return;
            }
            serialPort1.PortName = Convert.ToString(comPortComboBox.Items[comPortComboBox.SelectedIndex]);
            try
            {
                serialPort1.BaudRate = (int)comPosrBaudNumericUpDown.Value;
                serialPort1.Open();
            }
            catch
            {
                MessageBox.Show("COM port closed!");
            }

            if (serialPort1.IsOpen)
            {
                connectButton.Text = "Disconnect";
                if (!isUserInputLocked) inputTextBox.Enabled = true;
                comPosrBaudNumericUpDown.Enabled = false;
                comPortComboBox.Enabled = false;
                isConnect = true;
            }
            else
            {
                connectButton.Text = "Connect";
                inputTextBox.Enabled = false;
                comPosrBaudNumericUpDown.Enabled = true;
                comPortComboBox.Enabled = true;
                isConnect = false;
            }
        }
    }


    private void ComPortComboBox_DropDown(object sender, EventArgs e)
    {
        string[] ports = SerialPort.GetPortNames();

        comPortComboBox.Items.Clear();
        foreach (string port in ports)
        {
            comPortComboBox.Items.Add(port);
        }
    }


    private void clearButton_Click(object sender, EventArgs e)
    {
        this.outputRichTextBox.Clear();
    }

}