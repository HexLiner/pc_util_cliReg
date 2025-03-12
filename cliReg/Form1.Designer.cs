namespace cliReg
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.pollingControlButton = new System.Windows.Forms.Button();
            this.pollingPreiodNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.tableUpdateProgressBar = new System.Windows.Forms.ProgressBar();
            this.regWriteButton = new System.Windows.Forms.Button();
            this.regReadButton = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.AddressColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TypeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MinColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MaxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UpdColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.pollingIterationsNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.logRichTextBox = new System.Windows.Forms.RichTextBox();
            this.logEnCheckBox = new System.Windows.Forms.CheckBox();
            this.graphEnCheckBox = new System.Windows.Forms.CheckBox();
            this.logClrButton = new System.Windows.Forms.Button();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.cliSerialControl1 = new MI_601_CRUD_emulator.CliSerialControl();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pollingPreiodNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pollingIterationsNumericUpDown)).BeginInit();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(2, 1);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(779, 457);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.cliSerialControl1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(771, 431);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "CLI";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.graphEnCheckBox);
            this.tabPage2.Controls.Add(this.logEnCheckBox);
            this.tabPage2.Controls.Add(this.pollingIterationsNumericUpDown);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.pollingControlButton);
            this.tabPage2.Controls.Add(this.pollingPreiodNumericUpDown);
            this.tabPage2.Controls.Add(this.tableUpdateProgressBar);
            this.tabPage2.Controls.Add(this.regWriteButton);
            this.tabPage2.Controls.Add(this.regReadButton);
            this.tabPage2.Controls.Add(this.dataGridView1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(771, 431);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Table";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 409);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Polling period, ms:";
            // 
            // pollingControlButton
            // 
            this.pollingControlButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pollingControlButton.Location = new System.Drawing.Point(388, 405);
            this.pollingControlButton.Name = "pollingControlButton";
            this.pollingControlButton.Size = new System.Drawing.Size(58, 23);
            this.pollingControlButton.TabIndex = 5;
            this.pollingControlButton.Text = "Start";
            this.pollingControlButton.UseVisualStyleBackColor = true;
            this.pollingControlButton.Click += new System.EventHandler(this.pollingControlButton_Click);
            // 
            // pollingPreiodNumericUpDown
            // 
            this.pollingPreiodNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pollingPreiodNumericUpDown.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.pollingPreiodNumericUpDown.Location = new System.Drawing.Point(99, 406);
            this.pollingPreiodNumericUpDown.Maximum = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            this.pollingPreiodNumericUpDown.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.pollingPreiodNumericUpDown.Name = "pollingPreiodNumericUpDown";
            this.pollingPreiodNumericUpDown.Size = new System.Drawing.Size(59, 20);
            this.pollingPreiodNumericUpDown.TabIndex = 4;
            this.pollingPreiodNumericUpDown.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // tableUpdateProgressBar
            // 
            this.tableUpdateProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableUpdateProgressBar.Location = new System.Drawing.Point(4, 392);
            this.tableUpdateProgressBar.Name = "tableUpdateProgressBar";
            this.tableUpdateProgressBar.Size = new System.Drawing.Size(763, 10);
            this.tableUpdateProgressBar.TabIndex = 3;
            // 
            // regWriteButton
            // 
            this.regWriteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.regWriteButton.Location = new System.Drawing.Point(642, 405);
            this.regWriteButton.Name = "regWriteButton";
            this.regWriteButton.Size = new System.Drawing.Size(58, 23);
            this.regWriteButton.TabIndex = 2;
            this.regWriteButton.Text = "Write";
            this.regWriteButton.UseVisualStyleBackColor = true;
            this.regWriteButton.Click += new System.EventHandler(this.regWriteButton_Click);
            // 
            // regReadButton
            // 
            this.regReadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.regReadButton.Location = new System.Drawing.Point(706, 405);
            this.regReadButton.Name = "regReadButton";
            this.regReadButton.Size = new System.Drawing.Size(58, 23);
            this.regReadButton.TabIndex = 1;
            this.regReadButton.Text = "Read";
            this.regReadButton.UseVisualStyleBackColor = true;
            this.regReadButton.Click += new System.EventHandler(this.regReadButton_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.AddressColumn,
            this.NameColumn,
            this.ValueColumn,
            this.TypeColumn,
            this.MinColumn,
            this.MaxColumn,
            this.UpdColumn});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.EnableHeadersVisualStyles = false;
            this.dataGridView1.GridColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.dataGridView1.Location = new System.Drawing.Point(6, 4);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(763, 383);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridView1_CellBeginEdit);
            this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
            // 
            // AddressColumn
            // 
            this.AddressColumn.HeaderText = "Address";
            this.AddressColumn.Name = "AddressColumn";
            this.AddressColumn.ReadOnly = true;
            this.AddressColumn.Width = 118;
            // 
            // NameColumn
            // 
            this.NameColumn.HeaderText = "Name";
            this.NameColumn.Name = "NameColumn";
            this.NameColumn.ReadOnly = true;
            this.NameColumn.Width = 118;
            // 
            // ValueColumn
            // 
            this.ValueColumn.HeaderText = "Value";
            this.ValueColumn.Name = "ValueColumn";
            this.ValueColumn.Width = 118;
            // 
            // TypeColumn
            // 
            this.TypeColumn.HeaderText = "Type";
            this.TypeColumn.Name = "TypeColumn";
            this.TypeColumn.ReadOnly = true;
            this.TypeColumn.Width = 119;
            // 
            // MinColumn
            // 
            this.MinColumn.HeaderText = "Min";
            this.MinColumn.Name = "MinColumn";
            this.MinColumn.ReadOnly = true;
            this.MinColumn.Width = 118;
            // 
            // MaxColumn
            // 
            this.MaxColumn.HeaderText = "Max";
            this.MaxColumn.Name = "MaxColumn";
            this.MaxColumn.ReadOnly = true;
            this.MaxColumn.Width = 118;
            // 
            // UpdColumn
            // 
            this.UpdColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.UpdColumn.HeaderText = "Upd";
            this.UpdColumn.Name = "UpdColumn";
            this.UpdColumn.Width = 32;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.chart1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(771, 431);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Graph";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(164, 409);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Iterations:";
            // 
            // pollingIterationsNumericUpDown
            // 
            this.pollingIterationsNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pollingIterationsNumericUpDown.Location = new System.Drawing.Point(220, 406);
            this.pollingIterationsNumericUpDown.Maximum = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            this.pollingIterationsNumericUpDown.Name = "pollingIterationsNumericUpDown";
            this.pollingIterationsNumericUpDown.Size = new System.Drawing.Size(59, 20);
            this.pollingIterationsNumericUpDown.TabIndex = 8;
            this.pollingIterationsNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.logClrButton);
            this.tabPage4.Controls.Add(this.logRichTextBox);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(771, 431);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Log";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // logRichTextBox
            // 
            this.logRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logRichTextBox.Location = new System.Drawing.Point(3, 3);
            this.logRichTextBox.Name = "logRichTextBox";
            this.logRichTextBox.ReadOnly = true;
            this.logRichTextBox.Size = new System.Drawing.Size(765, 425);
            this.logRichTextBox.TabIndex = 0;
            this.logRichTextBox.Text = "";
            // 
            // logEnCheckBox
            // 
            this.logEnCheckBox.AutoSize = true;
            this.logEnCheckBox.Location = new System.Drawing.Point(342, 408);
            this.logEnCheckBox.Name = "logEnCheckBox";
            this.logEnCheckBox.Size = new System.Drawing.Size(44, 17);
            this.logEnCheckBox.TabIndex = 9;
            this.logEnCheckBox.Text = "Log";
            this.logEnCheckBox.UseVisualStyleBackColor = true;
            // 
            // graphEnCheckBox
            // 
            this.graphEnCheckBox.AutoSize = true;
            this.graphEnCheckBox.Location = new System.Drawing.Point(286, 408);
            this.graphEnCheckBox.Name = "graphEnCheckBox";
            this.graphEnCheckBox.Size = new System.Drawing.Size(55, 17);
            this.graphEnCheckBox.TabIndex = 10;
            this.graphEnCheckBox.Text = "Graph";
            this.graphEnCheckBox.UseVisualStyleBackColor = true;
            // 
            // logClrButton
            // 
            this.logClrButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.logClrButton.Location = new System.Drawing.Point(742, 3);
            this.logClrButton.Name = "logClrButton";
            this.logClrButton.Size = new System.Drawing.Size(26, 23);
            this.logClrButton.TabIndex = 1;
            this.logClrButton.Text = "X";
            this.logClrButton.UseVisualStyleBackColor = true;
            this.logClrButton.Click += new System.EventHandler(this.logClrButton_Click);
            // 
            // chart1
            // 
            this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(3, 3);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(765, 425);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // cliSerialControl1
            // 
            this.cliSerialControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cliSerialControl1.Location = new System.Drawing.Point(0, 3);
            this.cliSerialControl1.Name = "cliSerialControl1";
            this.cliSerialControl1.Size = new System.Drawing.Size(768, 423);
            this.cliSerialControl1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 461);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pollingPreiodNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pollingIterationsNumericUpDown)).EndInit();
            this.tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private MI_601_CRUD_emulator.CliSerialControl cliSerialControl1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button regReadButton;
        private System.Windows.Forms.Button regWriteButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn AddressColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ValueColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn TypeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn MinColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn MaxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn UpdColumn;
        private System.Windows.Forms.ProgressBar tableUpdateProgressBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button pollingControlButton;
        private System.Windows.Forms.NumericUpDown pollingPreiodNumericUpDown;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.NumericUpDown pollingIterationsNumericUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.RichTextBox logRichTextBox;
        private System.Windows.Forms.CheckBox graphEnCheckBox;
        private System.Windows.Forms.CheckBox logEnCheckBox;
        private System.Windows.Forms.Button logClrButton;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
    }
}

