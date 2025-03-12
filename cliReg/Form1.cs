using MI_601_CRUD_emulator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;
using static CliSerial;

namespace cliReg
{
    public partial class Form1 : Form
    {
        const UInt16 columnIndexVarAddr = 0;
        const UInt16 columnIndexVarName = 1;
        const UInt16 columnIndexVarValue = 2;
        const UInt16 columnIndexVarType = 3;
        const UInt16 columnIndexVarMin = 4;
        const UInt16 columnIndexVarMax = 5;
        const UInt16 columnIndexVarUpdFlag = 6;

        UInt32 recvTimeoutCnt;
        string dataReceivedBuffer = "";

        System.Windows.Forms.Timer processTimer;
        bool isRWProcess = false;
        Thread prcessThread;

        UInt32 pollingTimestampCnt;
        UInt32 pollingTimerCnt;
        bool isPollingEnabled = false;
        Int32 pollingIterationsQty;
        Int32 pollingIterationsCnt;

        private bool isIntTableUpdate = false;

        const UInt16 delayBetweenCmdMs = 20;
        const UInt16 recvTimeoutMs = 1000;


        public enum VarRegion
        {
            RAM,
            EEPROM
        };
        public enum VarType
        {
            H32, U32, S32, H16, U16, S16, H8, U8, S8, STRING
        };
        public enum VarEndian
        {
            BE, LE
        };
        public struct VarConfig
        {
            public string name;
            public VarRegion region;
            public UInt16 addr;
            public VarType type;
            public VarEndian endian;
            public UInt16 size;
            public Int64 minValue;
            public Int64 maxValue;
        };
        public enum VarCommStatus
        {
            init,
            ok,
            readCommError,
            readCommTimeout,
            readAccessError,
            writeCommError,
            writeCommTimeout,
            writeAccessError,
            verifError,
            incorrectValue
        };
        public struct DeviceVar
        {
            public VarConfig config;
            public VarCommStatus commStatus;
            public Int64 readDigValue;
            public Int64 newDigValue;
            public string readTextValue;
            public string newTextValue;
        }

        VarConfig[] deviceVarConfigs;
        DeviceVar[] deviceVars;
        VarConfig[] deviceVarConfigsExample =
        {
            new VarConfig() { name = "var1", region = VarRegion.RAM, addr = 0x0001, type = VarType.S32, endian = VarEndian.LE, size = 0, minValue = -134, maxValue = 1110},
            new VarConfig() { name = "var2", region = VarRegion.RAM, addr = 0x0005, type = VarType.STRING, endian = VarEndian.BE, size = 7, minValue = 0, maxValue = 0},
            new VarConfig() { name = "var3", region = VarRegion.RAM, addr = 0x000C, type = VarType.H8, endian = VarEndian.BE, size = 0, minValue = 20, maxValue = 100},
            new VarConfig() { name = "var4", region = VarRegion.EEPROM, addr = 0x0001, type = VarType.U16, endian = VarEndian.BE, size = 0, minValue = 0, maxValue = 550},
            new VarConfig() { name = "var5", region = VarRegion.EEPROM, addr = 0x0003, type = VarType.H8, endian = VarEndian.BE, size = 0, minValue = 20, maxValue = 100},
        };


        public Form1()
        {
            InitializeComponent();

            // CLI init
            CliSerialControl.CliSerialControlSettings cliSerialControlSettings = new CliSerialControl.CliSerialControlSettings();
            cliSerialControlSettings.baudRate = 9600;
            cliSerialControlSettings.cliAutoCompleteStringCollection = new AutoCompleteStringCollection()
            {
                "rr",
                "rd",
                "rq",
                "wr",
                "er",
                "ed",
                "eq",
                "er",
            };
            cliSerialControlSettings.actions = new List<CliSerialControl.CliQuickAction>
            {
                /*
                new CliSerialControl.CliQuickAction
                {
                    actionName = "Get state",
                    function = rudCliQCmdGetState
                },
                */
            };
            cliSerialControl1.SetSettings(cliSerialControlSettings);
            cliSerialControl1.cli.DataReceivedEvent += new DataReceived(DataReceivedHandler);

            tableInit();
            allTableUpdate();

            processTimer = new System.Windows.Forms.Timer();
            processTimer.Tick += ProcessTimer_Tick;
            processTimer.Interval = 10;
            processTimer.Start();

            tableUpdateProgressBar.Minimum = 0;
            tableUpdateProgressBar.Maximum = deviceVars.Length;
            tableUpdateProgressBar.Value = 0;

            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart1.ChartAreas[0].CursorX.AutoScroll = true;
            chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;

            chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            chart1.ChartAreas[0].CursorY.AutoScroll = true;
            chart1.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
        }


        public void tableInit()
        {
            // Get settings
            try
            {
                if (File.Exists("settings.xml"))
                {
                    Stream settingsStream = new FileStream("settings.xml", FileMode.Open);
                    XmlSerializer serializer = new XmlSerializer(typeof(VarConfig[]));
                    deviceVarConfigs = serializer.Deserialize(settingsStream) as VarConfig[];
                    settingsStream.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("settings.xml: " + ex.Message);
                Environment.Exit(0);
            }


            if (!File.Exists("settings.xml"))
            {
                deviceVarConfigs = deviceVarConfigsExample;

                try
                {
                    Stream writer = new FileStream("settings.xml", FileMode.Create);
                    XmlSerializer serializer = new XmlSerializer(typeof(VarConfig[]));
                    serializer.Serialize(writer, deviceVarConfigs);
                    writer.Close();
                }
                catch
                {
                    MessageBox.Show("App cofiguration file error!");
                }
            }

            for (int i = 0; i < deviceVarConfigs.Length; i++)
            {
                Array.Resize(ref deviceVars, (i + 1));
                deviceVars[i].config = deviceVarConfigs[i];
            }




            for (int varIndex = 0; varIndex < deviceVars.Length; varIndex++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[varIndex].Cells[columnIndexVarAddr].Value = String.Format("0x{0:X4}", deviceVars[varIndex].config.addr);
                dataGridView1.Rows[varIndex].Cells[columnIndexVarName].Value = deviceVars[varIndex].config.name;
                switch (deviceVars[varIndex].config.type)
                {
                    case VarType.H32:
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarType].Value = "H32";
                        if (deviceVars[varIndex].config.minValue > deviceVars[varIndex].config.maxValue)
                        {
                            MessageBox.Show("settings.xml:\n" + "\"" + deviceVars[varIndex].config.name + "\": incorrect Min value");
                            Environment.Exit(0);
                        }
                        if (deviceVars[varIndex].config.minValue == deviceVars[varIndex].config.maxValue)
                        {
                            deviceVars[varIndex].config.minValue = 0;
                            deviceVars[varIndex].config.maxValue = (UInt32)0xFFFFFFFF;
                        }
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarMin].Value = String.Format("0x{0:X8}", deviceVars[varIndex].config.minValue);
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarMax].Value = String.Format("0x{0:X8}", deviceVars[varIndex].config.maxValue);
                        deviceVars[varIndex].config.size = 4;
                        deviceVars[varIndex].commStatus = VarCommStatus.init;
                        break;
                    case VarType.U32:
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarType].Value = "U32";
                        if (deviceVars[varIndex].config.minValue > deviceVars[varIndex].config.maxValue)
                        {
                            MessageBox.Show("settings.xml:\n" + "\"" + deviceVars[varIndex].config.name + "\": incorrect Min value");
                            Environment.Exit(0);
                        }
                        if (deviceVars[varIndex].config.minValue == deviceVars[varIndex].config.maxValue)
                        {
                            deviceVars[varIndex].config.minValue = 0;
                            deviceVars[varIndex].config.maxValue = (UInt32)0xFFFFFFFF;
                        }
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarMin].Value = ((UInt32)deviceVars[varIndex].config.minValue);
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarMax].Value = ((UInt32)deviceVars[varIndex].config.maxValue);
                        deviceVars[varIndex].config.size = 4;
                        deviceVars[varIndex].commStatus = VarCommStatus.init;
                        break;
                    case VarType.S32:
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarType].Value = "S32";
                        if (deviceVars[varIndex].config.minValue > deviceVars[varIndex].config.maxValue)
                        {
                            MessageBox.Show("settings.xml:\n" + "\"" + deviceVars[varIndex].config.name + "\": incorrect Min value");
                            Environment.Exit(0);
                        }
                        if (deviceVars[varIndex].config.minValue == deviceVars[varIndex].config.maxValue)
                        {
                            deviceVars[varIndex].config.minValue = -(Int32)(0x7FFFFFFF);
                            deviceVars[varIndex].config.maxValue = (Int32)0x7FFFFFFF;
                        }
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarMin].Value = ((Int32)deviceVars[varIndex].config.minValue);
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarMax].Value = ((Int32)deviceVars[varIndex].config.maxValue);
                        deviceVars[varIndex].config.size = 4;
                        deviceVars[varIndex].commStatus = VarCommStatus.init;
                        break;
                    case VarType.H16:
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarType].Value = "H16";
                        if (deviceVars[varIndex].config.minValue > deviceVars[varIndex].config.maxValue)
                        {
                            MessageBox.Show("settings.xml:\n" + "\"" + deviceVars[varIndex].config.name + "\": incorrect Min value");
                            Environment.Exit(0);
                        }
                        if (deviceVars[varIndex].config.minValue == deviceVars[varIndex].config.maxValue)
                        {
                            deviceVars[varIndex].config.minValue = 0;
                            deviceVars[varIndex].config.maxValue = (UInt16)0xFFFF;
                        }
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarMin].Value = String.Format("0x{0:X4}", deviceVars[varIndex].config.minValue);
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarMax].Value = String.Format("0x{0:X4}", deviceVars[varIndex].config.maxValue);
                        deviceVars[varIndex].config.size = 2;
                        deviceVars[varIndex].commStatus = VarCommStatus.init;
                        break;
                    case VarType.U16:
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarType].Value = "U16";
                        if (deviceVars[varIndex].config.minValue > deviceVars[varIndex].config.maxValue)
                        {
                            MessageBox.Show("settings.xml:\n" + "\"" + deviceVars[varIndex].config.name + "\": incorrect Min value");
                            Environment.Exit(0);
                        }
                        if (deviceVars[varIndex].config.minValue == deviceVars[varIndex].config.maxValue)
                        {
                            deviceVars[varIndex].config.minValue = 0;
                            deviceVars[varIndex].config.maxValue = (UInt16)0xFFFF;
                        }
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarMin].Value = ((UInt16)deviceVars[varIndex].config.minValue);
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarMax].Value = ((UInt16)deviceVars[varIndex].config.maxValue);
                        deviceVars[varIndex].config.size = 2;
                        deviceVars[varIndex].commStatus = VarCommStatus.init;
                        break;
                    case VarType.S16:
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarType].Value = "S16";
                        if (deviceVars[varIndex].config.minValue > deviceVars[varIndex].config.maxValue)
                        {
                            MessageBox.Show("settings.xml:\n" + "\"" + deviceVars[varIndex].config.name + "\": incorrect Min value");
                            Environment.Exit(0);
                        }
                        if (deviceVars[varIndex].config.minValue == deviceVars[varIndex].config.maxValue)
                        {
                            deviceVars[varIndex].config.minValue = -(Int16)(0x7FFF);
                            deviceVars[varIndex].config.maxValue = (Int16)0x7FFF;
                        }
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarMin].Value = ((Int16)deviceVars[varIndex].config.minValue);
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarMax].Value = ((Int16)deviceVars[varIndex].config.maxValue);
                        deviceVars[varIndex].config.size = 2;
                        deviceVars[varIndex].commStatus = VarCommStatus.init;
                        break;
                    case VarType.H8:
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarType].Value = "H8";
                        if (deviceVars[varIndex].config.minValue > deviceVars[varIndex].config.maxValue)
                        {
                            MessageBox.Show("settings.xml:\n" + "\"" + deviceVars[varIndex].config.name + "\": incorrect Min value");
                            Environment.Exit(0);
                        }
                        if (deviceVars[varIndex].config.minValue == deviceVars[varIndex].config.maxValue)
                        {
                            deviceVars[varIndex].config.minValue = 0;
                            deviceVars[varIndex].config.maxValue = (UInt16)0xFF;
                        }
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarMin].Value = String.Format("0x{0:X2}", deviceVars[varIndex].config.minValue);
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarMax].Value = String.Format("0x{0:X2}", deviceVars[varIndex].config.maxValue);
                        deviceVars[varIndex].config.size = 1;
                        deviceVars[varIndex].commStatus = VarCommStatus.init;
                        break;
                    case VarType.U8:
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarType].Value = "U8";
                        if (deviceVars[varIndex].config.minValue > deviceVars[varIndex].config.maxValue)
                        {
                            MessageBox.Show("settings.xml:\n" + "\"" + deviceVars[varIndex].config.name + "\": incorrect Min value");
                            Environment.Exit(0);
                        }
                        if (deviceVars[varIndex].config.minValue == deviceVars[varIndex].config.maxValue)
                        {
                            deviceVars[varIndex].config.minValue = 0;
                            deviceVars[varIndex].config.maxValue = (UInt16)0xFF;
                        }
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarMin].Value = ((UInt16)deviceVars[varIndex].config.minValue);
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarMax].Value = ((UInt16)deviceVars[varIndex].config.maxValue);
                        deviceVars[varIndex].config.size = 1;
                        deviceVars[varIndex].commStatus = VarCommStatus.init;
                        break;
                    case VarType.S8:
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarType].Value = "S8";
                        if (deviceVars[varIndex].config.minValue > deviceVars[varIndex].config.maxValue)
                        {
                            MessageBox.Show("settings.xml:\n" + "\"" + deviceVars[varIndex].config.name + "\": incorrect Min value");
                            Environment.Exit(0);
                        }
                        if (deviceVars[varIndex].config.minValue == deviceVars[varIndex].config.maxValue)
                        {
                            deviceVars[varIndex].config.minValue = -(Int16)(0x7F);
                            deviceVars[varIndex].config.maxValue = (Int16)0x7F;
                        }
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarMin].Value = deviceVars[varIndex].config.minValue;
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarMax].Value = deviceVars[varIndex].config.maxValue;
                        deviceVars[varIndex].config.size = 1;
                        deviceVars[varIndex].commStatus = VarCommStatus.init;
                        break;
                    case VarType.STRING:
                        if (deviceVars[varIndex].config.size == 0)
                        {
                            MessageBox.Show("settings.xml:\n" + "\"" + deviceVars[varIndex].config.name + "\": incorrect Size");
                            Environment.Exit(0);
                        }
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarType].Value = "STRING";
                        deviceVars[varIndex].commStatus = VarCommStatus.init;
                        break;
                }
                dataGridView1.Rows[varIndex].Cells[columnIndexVarUpdFlag].Value = false;
            }
        }

        public void allTableUpdate()
        {
            for (int varIndex = 0; varIndex < deviceVars.Length; varIndex++)
            {
                tableUpdate(varIndex);
            }
        }

        public void tableUpdate(int varIndex)
        {
            isIntTableUpdate = true;

            if (deviceVars[varIndex].commStatus != VarCommStatus.ok)
            {
                dataGridView1.Rows[varIndex].Cells[columnIndexVarValue].Value = deviceVars[varIndex].commStatus;
                this.dataGridView1.Rows[varIndex].Cells[columnIndexVarValue].Style.BackColor = Color.RosyBrown;
            }
            else
            {

                switch (deviceVars[varIndex].config.type)
                {
                    case VarType.H32:
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarValue].Value = String.Format("0x{0:X8}", deviceVars[varIndex].readDigValue);
                        break;
                    case VarType.U32:
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarValue].Value = ((UInt32)deviceVars[varIndex].readDigValue);
                        break;
                    case VarType.S32:
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarValue].Value = ((Int32)deviceVars[varIndex].readDigValue);
                        break;
                    case VarType.H16:
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarValue].Value = String.Format("0x{0:X4}", deviceVars[varIndex].readDigValue);
                        break;
                    case VarType.U16:
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarValue].Value = ((UInt16)deviceVars[varIndex].readDigValue);
                        break;
                    case VarType.S16:
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarValue].Value = ((Int16)deviceVars[varIndex].readDigValue);
                        break;
                    case VarType.H8:
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarValue].Value = String.Format("0x{0:X2}", deviceVars[varIndex].readDigValue);
                        break;
                    case VarType.U8:
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarValue].Value = ((UInt16)deviceVars[varIndex].readDigValue);
                        break;
                    case VarType.S8:
                        if ((deviceVars[varIndex].readDigValue & 0x80) != 0)
                        {
                            dataGridView1.Rows[varIndex].Cells[columnIndexVarValue].Value = (-(Int16)deviceVars[varIndex].readDigValue);
                        }
                        else
                        {
                            dataGridView1.Rows[varIndex].Cells[columnIndexVarValue].Value = ((UInt16)deviceVars[varIndex].readDigValue);
                        }
                        break;
                    case VarType.STRING:
                        dataGridView1.Rows[varIndex].Cells[columnIndexVarValue].Value = deviceVars[varIndex].readTextValue;
                        break;
                }
                this.dataGridView1.Rows[varIndex].Cells[columnIndexVarValue].Style.BackColor = Color.White;
            }
            isIntTableUpdate = false;
        }



        private void regReadButton_Click(object sender, EventArgs e)
        {
            if (isRWProcess) return;

            dataGridView1.Enabled = false;
            regReadButton.Enabled = false;
            regWriteButton.Enabled = false;
            pollingControlButton.Enabled = false;
            pollingPreiodNumericUpDown.Enabled = false;

            prcessThread = new Thread(readAllRegs);
            prcessThread.Start();
            isRWProcess = true;
        }


        private void regWriteButton_Click(object sender, EventArgs e)
        {
            if (isRWProcess) return;

            dataGridView1.Enabled = false;
            regReadButton.Enabled = false;
            regWriteButton.Enabled = false;
            pollingControlButton.Enabled = false;
            pollingPreiodNumericUpDown.Enabled = false;

            prcessThread = new Thread(writeAllRegs);
            prcessThread.Start();
            isRWProcess = true;
        }


        delegate void DelPB(int progress);
        public void readAllRegs()
        {
            tableUpdateProgressBar.Invoke(new DelPB((s) => tableUpdateProgressBar.Maximum = s), deviceVars.Length);
            for (int varIndex = 0; varIndex < deviceVars.Length; varIndex++)
            {
                readVar(varIndex);
                deviceVars[varIndex].newTextValue = deviceVars[varIndex].readTextValue;
                deviceVars[varIndex].newDigValue = deviceVars[varIndex].readDigValue;
                tableUpdateProgressBar.Invoke(new DelPB((s) => tableUpdateProgressBar.Value = s), (varIndex + 1));
            }
            tableUpdateProgressBar.Invoke(new DelPB((s) => tableUpdateProgressBar.Value = s), 0);
        }

        public void writeAllRegs()
        {
            tableUpdateProgressBar.Invoke(new DelPB((s) => tableUpdateProgressBar.Maximum = s), deviceVars.Length);
            for (int varIndex = 0; varIndex < deviceVars.Length; varIndex++)
            {
                if ((deviceVars[varIndex].newTextValue != deviceVars[varIndex].readTextValue) ||
                    (deviceVars[varIndex].newDigValue != deviceVars[varIndex].readDigValue))
                {
                    writeVar(varIndex);
                    deviceVars[varIndex].newTextValue = deviceVars[varIndex].readTextValue;
                    deviceVars[varIndex].newDigValue = deviceVars[varIndex].readDigValue;
                }
                tableUpdateProgressBar.Invoke(new DelPB((s) => tableUpdateProgressBar.Value = s), (varIndex + 1));
            }
            tableUpdateProgressBar.Invoke(new DelPB((s) => tableUpdateProgressBar.Value = s), 0);
        }

        public void readSelectedRegs()
        {
            for (int varIndex = 0; varIndex < deviceVars.Length; varIndex++)
            {
                if ((bool)dataGridView1.Rows[varIndex].Cells[columnIndexVarUpdFlag].Value == true)
                {
                    readVar(varIndex);
                    deviceVars[varIndex].newTextValue = deviceVars[varIndex].readTextValue;
                    deviceVars[varIndex].newDigValue = deviceVars[varIndex].readDigValue;
                }
            }
        }



        delegate void DelSL();
        delegate bool DelSM(string data);
        private bool readVar(int varIndex)
        {
            UInt16 readValue;
            List<UInt16> readValues = new List<UInt16>();
            string cmd = "";
            string cmd_args;
            int i;


            if (varIndex >= deviceVars.Length) return false;

            deviceVars[varIndex].commStatus = VarCommStatus.ok;

            if (deviceVars[varIndex].config.region == VarRegion.RAM) cmd = "rr";
            else if (deviceVars[varIndex].config.region == VarRegion.EEPROM) cmd = "er";

            cliSerialControl1.Invoke(new DelSL(() => cliSerialControl1.LockUserInput()));
            cliSerialControl1.Invoke(new DelSL(() => cliSerialControl1.LockUserOutput()));
            dataReceivedBuffer = "";
            for (i = 0; i < deviceVars[varIndex].config.size; i++)
            {
                dataReceivedBuffer = "";
                cmd_args = String.Format("{0:X4}", (deviceVars[varIndex].config.addr + i));
                if (!Convert.ToBoolean(cliSerialControl1.Invoke(new DelSM((s) => cliSerialControl1.SendMessage(s)), (cmd + cmd_args + "\r"))))
                {
                    cliSerialControl1.Invoke(new DelSL(() => cliSerialControl1.UnlockUserInput()));
                    cliSerialControl1.Invoke(new DelSL(() => cliSerialControl1.UnlockUserOutput()));
                    deviceVars[varIndex].commStatus = VarCommStatus.readCommError;
                    return false;
                }
                recvTimeoutCnt = recvTimeoutMs;
                while (dataReceivedBuffer.Length < (cmd.Length + cmd_args.Length + 1 + 2))
                {
                    if (recvTimeoutCnt > 0) recvTimeoutCnt--;
                    else
                    {
                        cliSerialControl1.Invoke(new DelSL(() => cliSerialControl1.UnlockUserInput()));
                        cliSerialControl1.Invoke(new DelSL(() => cliSerialControl1.UnlockUserOutput()));
                        deviceVars[varIndex].commStatus = VarCommStatus.readCommTimeout;
                        return false;
                    }
                    Thread.Sleep(1);
                }
                Thread.Sleep(delayBetweenCmdMs);

                // Parsing
                string[] readStrings = dataReceivedBuffer.Split(new char[] { '\n' });
                readStrings[1] = readStrings[1].Trim('\r');
                try
                {
                    readValue = Convert.ToUInt16(readStrings[1], 16);
                }
                catch
                {
                    //// tmp
                    try
                    {
                        readStrings[2] = readStrings[2].Trim('\r');
                        readValue = Convert.ToUInt16(readStrings[2], 16);
                    }
                    catch
                    {
                        cliSerialControl1.Invoke(new DelSL(() => cliSerialControl1.UnlockUserInput()));
                        cliSerialControl1.Invoke(new DelSL(() => cliSerialControl1.UnlockUserOutput()));
                        deviceVars[varIndex].commStatus = VarCommStatus.readCommError;
                        return false;
                    }
                }
                cliSerialControl1.Invoke(new DelSL(() => cliSerialControl1.UnlockUserInput()));
                cliSerialControl1.Invoke(new DelSL(() => cliSerialControl1.UnlockUserOutput()));
                readValues.Add(readValue);
            }


            switch (deviceVars[varIndex].config.type)
            {
                case VarType.H32:
                case VarType.U32:
                case VarType.H16:
                case VarType.U16:
                case VarType.H8:
                case VarType.U8:
                    deviceVars[varIndex].readDigValue = 0;
                    if (deviceVars[varIndex].config.endian == VarEndian.LE)
                    {
                        for (i = 0; i < deviceVars[varIndex].config.size; i++)
                        {
                            deviceVars[varIndex].readDigValue |= (Int64)((UInt64)readValues[i] << (8 * i));
                        }
                    }
                    else
                    {
                        for (i = 0; i < deviceVars[varIndex].config.size; i++)
                        {
                            deviceVars[varIndex].readDigValue |= (Int64)((UInt64)readValues[i] << (8 * (deviceVars[varIndex].config.size - i)));
                        }
                    }

                    if (deviceVars[varIndex].readDigValue < deviceVars[varIndex].config.minValue) deviceVars[varIndex].commStatus = VarCommStatus.incorrectValue;
                    if (deviceVars[varIndex].readDigValue > deviceVars[varIndex].config.maxValue) deviceVars[varIndex].commStatus = VarCommStatus.incorrectValue;
                    break;

                case VarType.S32:
                case VarType.S16:
                case VarType.S8:
                    deviceVars[varIndex].readDigValue = 0;
                    if (deviceVars[varIndex].config.endian == VarEndian.LE)
                    {
                        for (i = 0; i < deviceVars[varIndex].config.size; i++)
                        {
                            deviceVars[varIndex].readDigValue |= (Int64)((UInt64)readValues[i] << (8 * i));
                        }
                        if ((readValues[i - 1] & 0x80) != 0)
                        {
                            for (; i < 8; i++)
                            {
                                deviceVars[varIndex].readDigValue |= (Int64)((UInt64)0xFF << (8 * i));
                            }
                        }
                    }
                    else
                    {
                        for (i = 0; i < deviceVars[varIndex].config.size; i++)
                        {
                            deviceVars[varIndex].readDigValue |= (Int64)((UInt64)readValues[i] << (8 * (deviceVars[varIndex].config.size - i)));
                        }
                        if ((readValues[0] & 0x80) != 0)
                        {
                            for (; i < 8; i++)
                            {
                                deviceVars[varIndex].readDigValue |= (Int64)((UInt64)0xFF << (8 * i));
                            }
                        }
                    }

                    if (deviceVars[varIndex].readDigValue < deviceVars[varIndex].config.minValue) deviceVars[varIndex].commStatus = VarCommStatus.incorrectValue;
                    if (deviceVars[varIndex].readDigValue > deviceVars[varIndex].config.maxValue) deviceVars[varIndex].commStatus = VarCommStatus.incorrectValue;
                    break;

                case VarType.STRING:
                    deviceVars[varIndex].readTextValue = "";
                    if (deviceVars[varIndex].config.endian == VarEndian.LE)
                    {
                        for (i = deviceVars[varIndex].config.size; i >= 0; i--)
                        {
                            if (((readValues[i] < 32) || (readValues[i] > 126)) && (readValues[i] != '\0')) deviceVars[varIndex].commStatus = VarCommStatus.incorrectValue;
                            deviceVars[varIndex].readTextValue += (char)readValues[i];
                        }
                    }
                    else
                    {
                        for (i = 0; i < deviceVars[varIndex].config.size; i++)
                        {
                            if (((readValues[i] < 32) || (readValues[i] > 126)) && (readValues[i] != '\0')) deviceVars[varIndex].commStatus = VarCommStatus.incorrectValue;
                            deviceVars[varIndex].readTextValue += (char)readValues[i];
                        }
                    }
                    break;
            }

            return true;
        }

        private bool writeVar(int varIndex)
        {
            string cmd = "";
            string cmd_args;
            List<UInt16> writeValues = new List<UInt16>();


            if (varIndex >= deviceVars.Length) return false;
            if (deviceVars[varIndex].commStatus != VarCommStatus.ok) return false;

            if (deviceVars[varIndex].config.region == VarRegion.RAM) cmd = "rw";
            else if (deviceVars[varIndex].config.region == VarRegion.EEPROM) cmd = "ew";

            switch (deviceVars[varIndex].config.type)
            {
                case VarType.H32:
                case VarType.U32:
                case VarType.S32:
                    if (deviceVars[varIndex].config.endian == VarEndian.LE)
                    {
                        writeValues.Add((UInt16)((deviceVars[varIndex].newDigValue >> 0) & 0xFF));
                        writeValues.Add((UInt16)((deviceVars[varIndex].newDigValue >> 8) & 0xFF));
                        writeValues.Add((UInt16)((deviceVars[varIndex].newDigValue >> 16) & 0xFF));
                        writeValues.Add((UInt16)((deviceVars[varIndex].newDigValue >> 24) & 0xFF));
                    }
                    else
                    {
                        writeValues.Add((UInt16)((deviceVars[varIndex].newDigValue >> 24) & 0xFF));
                        writeValues.Add((UInt16)((deviceVars[varIndex].newDigValue >> 16) & 0xFF));
                        writeValues.Add((UInt16)((deviceVars[varIndex].newDigValue >> 8) & 0xFF));
                        writeValues.Add((UInt16)((deviceVars[varIndex].newDigValue >> 0) & 0xFF));
                    }
                    break;
                case VarType.H16:
                case VarType.U16:
                case VarType.S16:
                    if (deviceVars[varIndex].config.endian == VarEndian.LE)
                    {
                        writeValues.Add((UInt16)((deviceVars[varIndex].newDigValue >> 0) & 0xFF));
                        writeValues.Add((UInt16)((deviceVars[varIndex].newDigValue >> 8) & 0xFF));
                    }
                    else
                    {
                        writeValues.Add((UInt16)((deviceVars[varIndex].newDigValue >> 8) & 0xFF));
                        writeValues.Add((UInt16)((deviceVars[varIndex].newDigValue >> 0) & 0xFF));
                    }
                    break;
                case VarType.H8:
                case VarType.U8:
                case VarType.S8:
                    writeValues.Add((UInt16)((deviceVars[varIndex].newDigValue >> 0) & 0xFF));
                    break;
                case VarType.STRING:
                    int i;
                    for (i = 0; i < (deviceVars[varIndex].config.size - deviceVars[varIndex].newTextValue.Length); i++)
                    {
                        deviceVars[varIndex].newTextValue += '\0';
                    }

                    if (deviceVars[varIndex].config.endian == VarEndian.LE)
                    {
                        for (i = 0; i < deviceVars[varIndex].newTextValue.Length; i++)
                        {
                            writeValues.Add((UInt16)(deviceVars[varIndex].newTextValue[deviceVars[varIndex].newTextValue.Length - i]));
                        }
                    }
                    else
                    {
                        for (i = 0; i < deviceVars[varIndex].newTextValue.Length; i++)
                        {
                            writeValues.Add((UInt16)(deviceVars[varIndex].newTextValue[i]));
                        }
                    }
                    break;
            }

            cliSerialControl1.Invoke(new DelSL(() => cliSerialControl1.LockUserInput()));
            cliSerialControl1.Invoke(new DelSL(() => cliSerialControl1.LockUserOutput()));
            for (int i = 0; i < deviceVars[varIndex].config.size; i++)
            {
                cmd_args = String.Format("{0:X4}", (deviceVars[varIndex].config.addr + i));
                cmd_args += " ";
                cmd_args += String.Format("{0:X2}", writeValues[i]);
                if (!Convert.ToBoolean(cliSerialControl1.Invoke(new DelSM((s) => cliSerialControl1.SendMessage(s)), (cmd + cmd_args + "\r"))))
                {
                    cliSerialControl1.Invoke(new DelSL(() => cliSerialControl1.UnlockUserInput()));
                    cliSerialControl1.Invoke(new DelSL(() => cliSerialControl1.UnlockUserOutput()));
                    deviceVars[varIndex].commStatus = VarCommStatus.writeCommError;
                    return false;
                }
                recvTimeoutCnt = recvTimeoutMs;
                while (dataReceivedBuffer.Length < (cmd.Length + cmd_args.Length + 1))
                {
                    if (recvTimeoutCnt > 0) recvTimeoutCnt--;
                    else
                    {
                        cliSerialControl1.Invoke(new DelSL(() => cliSerialControl1.UnlockUserInput()));
                        cliSerialControl1.Invoke(new DelSL(() => cliSerialControl1.UnlockUserOutput()));
                        deviceVars[varIndex].commStatus = VarCommStatus.writeCommTimeout;
                        return false;
                    }
                    Thread.Sleep(1);
                }
                Thread.Sleep(delayBetweenCmdMs);

            }
            cliSerialControl1.Invoke(new DelSL(() => cliSerialControl1.UnlockUserInput()));
            cliSerialControl1.Invoke(new DelSL(() => cliSerialControl1.UnlockUserOutput()));

            readVar(varIndex);
            if (deviceVars[varIndex].commStatus != VarCommStatus.ok) return false;
            if (deviceVars[varIndex].config.type == VarType.STRING)
            {
                if (deviceVars[varIndex].newTextValue != deviceVars[varIndex].readTextValue)
                {
                    deviceVars[varIndex].commStatus = VarCommStatus.verifError;
                    return false;
                }
            }
            else if (deviceVars[varIndex].newDigValue != deviceVars[varIndex].readDigValue)
            {
                deviceVars[varIndex].commStatus = VarCommStatus.verifError;
                return false;
            }

            return true;
        }


        private void addSelectVarsToLog(UInt64 timestampMs)
        {
            logRichTextBox.Text += timestampMs.ToString() + "; ";
            for (int varIndex = 0; varIndex < deviceVars.Length; varIndex++)
            {
                if ((bool)dataGridView1.Rows[varIndex].Cells[columnIndexVarUpdFlag].Value == true)
                {
                    if (deviceVars[varIndex].commStatus != VarCommStatus.ok) logRichTextBox.Text += "N/D";
                    else if (deviceVars[varIndex].config.type == VarType.STRING) logRichTextBox.Text += deviceVars[varIndex].readTextValue;
                    else logRichTextBox.Text += deviceVars[varIndex].readDigValue.ToString();

                    logRichTextBox.Text += "; ";
                }
            }
            logRichTextBox.Text += "\n";
        }

        private void addSelectVarsToGraph(UInt32 timestampMs)
        {
            for (int i = 0; i < deviceVars.Length; i++)
            {
                if (((bool)dataGridView1.Rows[i].Cells[columnIndexVarUpdFlag].Value == true) && (deviceVars[i].config.type != VarType.STRING))
                {
                    chart1.Series[deviceVars[i].config.name].Points.AddXY(timestampMs, deviceVars[i].readDigValue);
                }
            }
        }

        private void ProcessTimer_Tick(object sender, EventArgs e)
        {
            if (pollingTimerCnt > 0) pollingTimerCnt--;
            pollingTimestampCnt += 10;

            if (isRWProcess)
            {
                if (prcessThread.ThreadState == ThreadState.Stopped)
                {
                    if (isPollingEnabled)
                    {
                        if (pollingTimerCnt == 0)
                        {
                            pollingIterationsCnt++;
                            if ((pollingIterationsQty == -1) || (pollingIterationsCnt < pollingIterationsQty))
                            {
                                pollingTimerCnt = (UInt32)pollingPreiodNumericUpDown.Value / 10;

                                allTableUpdate();
                                if (logEnCheckBox.Checked) addSelectVarsToLog(pollingTimestampCnt);
                                if (graphEnCheckBox.Checked) addSelectVarsToGraph(pollingTimestampCnt);

                                //prcessThread = new Thread(() => readVar(pollingVarIndex));
                                prcessThread = new Thread(readSelectedRegs);
                                prcessThread.Start();

                                if (pollingIterationsQty != -1) tableUpdateProgressBar.Value = pollingIterationsCnt;
                            }
                            else
                            {
                                dataGridView1.Enabled = true;
                                regReadButton.Enabled = true;
                                regWriteButton.Enabled = true;
                                pollingPreiodNumericUpDown.Enabled = true;
                                pollingControlButton.Text = "Start";
                                isPollingEnabled = false;
                                tableUpdateProgressBar.Value = 0;
                            }
                        }
                    }
                    else
                    {
                        allTableUpdate();
                        dataGridView1.Enabled = true;
                        regReadButton.Enabled = true;
                        regWriteButton.Enabled = true;
                        pollingControlButton.Enabled = true;
                        pollingPreiodNumericUpDown.Enabled = true;
                        isRWProcess = false;
                    }
                }
            }
        }


        private void DataReceivedHandler(string data)
        {
            dataReceivedBuffer += data;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (prcessThread != null) prcessThread.Abort();
        }



        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            string new_reg_value_string;
            Int64 new_reg_value;
            bool isHexFormat;


            if (e.ColumnIndex == columnIndexVarValue)
            {
                if ((e.RowIndex == -1) || isIntTableUpdate) return;

                new_reg_value_string = Convert.ToString(this.dataGridView1.Rows[e.RowIndex].Cells[columnIndexVarValue].Value);

                if (deviceVars[e.RowIndex].config.type != VarType.STRING)
                {
                    isHexFormat = (new_reg_value_string.IndexOf("0x") != -1);
                    try
                    {
                        if (isHexFormat) new_reg_value = Convert.ToInt64(new_reg_value_string, 16);
                        else new_reg_value = Convert.ToInt64(new_reg_value_string, 10);
                    }
                    catch
                    {
                        deviceVars[e.RowIndex].commStatus = VarCommStatus.incorrectValue;
                        this.dataGridView1.Rows[e.RowIndex].Cells[columnIndexVarValue].Value = deviceVars[e.RowIndex].commStatus;
                        this.dataGridView1.Rows[e.RowIndex].Cells[columnIndexVarValue].Style.BackColor = Color.RosyBrown;
                        return;
                    }

                    if ((new_reg_value > deviceVars[e.RowIndex].config.maxValue) || (new_reg_value < (deviceVars[e.RowIndex].config.minValue)))
                    {
                        deviceVars[e.RowIndex].commStatus = VarCommStatus.incorrectValue;
                        this.dataGridView1.Rows[e.RowIndex].Cells[columnIndexVarValue].Value = deviceVars[e.RowIndex].commStatus;
                        this.dataGridView1.Rows[e.RowIndex].Cells[columnIndexVarValue].Style.BackColor = Color.RosyBrown;
                        return;
                    }
                    deviceVars[e.RowIndex].newDigValue = new_reg_value;
                }
                else
                {
                    if (new_reg_value_string.Length > deviceVars[e.RowIndex].config.size)
                    {
                        deviceVars[e.RowIndex].commStatus = VarCommStatus.incorrectValue;
                        this.dataGridView1.Rows[e.RowIndex].Cells[columnIndexVarValue].Value = deviceVars[e.RowIndex].commStatus;
                        this.dataGridView1.Rows[e.RowIndex].Cells[columnIndexVarValue].Style.BackColor = Color.RosyBrown;
                        return;
                    }
                    deviceVars[e.RowIndex].newTextValue = new_reg_value_string;
                }
                this.dataGridView1.Rows[e.RowIndex].Cells[columnIndexVarValue].Style.BackColor = Color.Yellow;
            }
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex == columnIndexVarValue)
            {
                if ((e.RowIndex == -1) || isIntTableUpdate) return;

                isIntTableUpdate = true;
                if (deviceVars[e.RowIndex].commStatus != VarCommStatus.ok)
                {
                    this.dataGridView1.Rows[e.RowIndex].Cells[columnIndexVarValue].Value = "";
                    this.dataGridView1.Rows[e.RowIndex].Cells[columnIndexVarValue].Style.BackColor = Color.Yellow;
                    deviceVars[e.RowIndex].commStatus = VarCommStatus.ok;
                }
                isIntTableUpdate = false;
            }
        }

        private void pollingControlButton_Click(object sender, EventArgs e)
        {
            if (isPollingEnabled)
            {
                dataGridView1.Enabled = true;
                regReadButton.Enabled = true;
                regWriteButton.Enabled = true;
                pollingPreiodNumericUpDown.Enabled = true;
                pollingControlButton.Text = "Start";
                isPollingEnabled = false;
                return;
            }

            if (isRWProcess) return;


            int pollingVarsQty = 0;
            for (int i = 0; i < deviceVars.Length; i++)
            {
                if ((bool)dataGridView1.Rows[i].Cells[columnIndexVarUpdFlag].Value == true) pollingVarsQty++;
            }
            if (pollingVarsQty == 0)
            {
                MessageBox.Show("Polling variables isn't selected!");
                return;
            }


            if (logEnCheckBox.Checked)
            {
                logRichTextBox.Text += "Time; ";
                for (int i = 0; i < deviceVars.Length; i++)
                {
                    if ((bool)dataGridView1.Rows[i].Cells[columnIndexVarUpdFlag].Value == true) logRichTextBox.Text += deviceVars[i].config.name + "; ";
                }
                logRichTextBox.Text += "\n";
            }

            if (graphEnCheckBox.Checked)
            {
                chart1.Series.Clear();
                chart1.Legends.Clear();
                for (int i = 0; i < deviceVars.Length; i++)
                {
                    if (((bool)dataGridView1.Rows[i].Cells[columnIndexVarUpdFlag].Value == true) && (deviceVars[i].config.type != VarType.STRING))
                    {
                        chart1.Series.Add(deviceVars[i].config.name);
                        chart1.Series[deviceVars[i].config.name].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                        chart1.Series[deviceVars[i].config.name].BorderWidth = 2;
                        chart1.Legends.Add(deviceVars[i].config.name);
                    }
                }
            }


            dataGridView1.Enabled = false;
            regReadButton.Enabled = false;
            regWriteButton.Enabled = false;
            pollingPreiodNumericUpDown.Enabled = false;
            pollingControlButton.Text = "Stop";
            pollingIterationsQty = (Int32)pollingIterationsNumericUpDown.Value;
            pollingIterationsCnt = 0;
            pollingTimestampCnt = 0;
            if (pollingIterationsNumericUpDown.Value == 0) pollingIterationsQty = -1;   // infinity

            if (pollingIterationsQty != -1) tableUpdateProgressBar.Maximum = pollingIterationsQty;

            prcessThread = new Thread(readSelectedRegs);
            prcessThread.Start();
            pollingTimerCnt = (UInt32)pollingPreiodNumericUpDown.Value / 10;
            isPollingEnabled = true;
            isRWProcess = true;
        }

        private void logClrButton_Click(object sender, EventArgs e)
        {
            logRichTextBox.Clear();
        }
    }
}
