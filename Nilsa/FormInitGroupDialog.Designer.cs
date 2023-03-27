namespace Nilsa
{
    partial class FormInitGroupDialog
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInitGroupDialog));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.listBoxOutMsg = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.panelPersonen = new System.Windows.Forms.Panel();
            this.listBoxPersonen = new System.Windows.Forms.ListBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.buttonPersonenDn = new System.Windows.Forms.Button();
            this.buttonPersonenUp = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonPersonenDelete = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonPersonenAdd = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxAlgSetAction = new System.Windows.Forms.ComboBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBoxAlgSetList = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.button9 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.contextMenuStripVectorKoefOut = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripEQOutMsgValues = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolTipMessage = new System.Windows.Forms.ToolTip(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.button7 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.timerClose = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panelPersonen.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.contextMenuStripVectorKoefOut.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tabControl1);
            this.groupBox1.Controls.Add(this.panelPersonen);
            this.groupBox1.Controls.Add(this.panel3);
            this.groupBox1.Controls.Add(this.listBox1);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.groupBox1.Size = new System.Drawing.Size(1681, 650);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Группы";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(10, 367);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1383, 280);
            this.tabControl1.TabIndex = 7;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.textBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1375, 254);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Текст";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(3, 3);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(1369, 248);
            this.textBox1.TabIndex = 2;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.listBoxOutMsg);
            this.tabPage2.Controls.Add(this.tableLayoutPanel7);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1375, 254);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Подбор";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // listBoxOutMsg
            // 
            this.listBoxOutMsg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxOutMsg.FormattingEnabled = true;
            this.listBoxOutMsg.Location = new System.Drawing.Point(3, 8);
            this.listBoxOutMsg.Name = "listBoxOutMsg";
            this.listBoxOutMsg.Size = new System.Drawing.Size(1369, 243);
            this.listBoxOutMsg.TabIndex = 8;
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.AutoSize = true;
            this.tableLayoutPanel7.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel7.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel7.ColumnCount = 1;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel7.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel7.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 4;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(1369, 5);
            this.tableLayoutPanel7.TabIndex = 7;
            // 
            // panelPersonen
            // 
            this.panelPersonen.Controls.Add(this.listBoxPersonen);
            this.panelPersonen.Controls.Add(this.panel4);
            this.panelPersonen.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelPersonen.Location = new System.Drawing.Point(1393, 367);
            this.panelPersonen.Name = "panelPersonen";
            this.panelPersonen.Padding = new System.Windows.Forms.Padding(0, 0, 0, 7);
            this.panelPersonen.Size = new System.Drawing.Size(278, 280);
            this.panelPersonen.TabIndex = 23;
            // 
            // listBoxPersonen
            // 
            this.listBoxPersonen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxPersonen.FormattingEnabled = true;
            this.listBoxPersonen.Location = new System.Drawing.Point(0, 26);
            this.listBoxPersonen.Name = "listBoxPersonen";
            this.listBoxPersonen.Size = new System.Drawing.Size(278, 247);
            this.listBoxPersonen.TabIndex = 1;
            this.listBoxPersonen.SelectedIndexChanged += new System.EventHandler(this.listBoxPersonen_SelectedIndexChanged);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.buttonPersonenDn);
            this.panel4.Controls.Add(this.buttonPersonenUp);
            this.panel4.Controls.Add(this.label6);
            this.panel4.Controls.Add(this.buttonPersonenDelete);
            this.panel4.Controls.Add(this.label7);
            this.panel4.Controls.Add(this.buttonPersonenAdd);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(278, 26);
            this.panel4.TabIndex = 0;
            // 
            // buttonPersonenDn
            // 
            this.buttonPersonenDn.Dock = System.Windows.Forms.DockStyle.Left;
            this.buttonPersonenDn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPersonenDn.Image = global::Nilsa.Properties.Resources.ic_expand_more_black_24dp;
            this.buttonPersonenDn.Location = new System.Drawing.Point(186, 0);
            this.buttonPersonenDn.Name = "buttonPersonenDn";
            this.buttonPersonenDn.Size = new System.Drawing.Size(38, 26);
            this.buttonPersonenDn.TabIndex = 4;
            this.buttonPersonenDn.UseVisualStyleBackColor = true;
            this.buttonPersonenDn.Click += new System.EventHandler(this.buttonPersonenDn_Click);
            // 
            // buttonPersonenUp
            // 
            this.buttonPersonenUp.Dock = System.Windows.Forms.DockStyle.Left;
            this.buttonPersonenUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPersonenUp.Image = global::Nilsa.Properties.Resources.ic_expand_less_black_24dp;
            this.buttonPersonenUp.Location = new System.Drawing.Point(148, 0);
            this.buttonPersonenUp.Name = "buttonPersonenUp";
            this.buttonPersonenUp.Size = new System.Drawing.Size(38, 26);
            this.buttonPersonenUp.TabIndex = 3;
            this.buttonPersonenUp.UseVisualStyleBackColor = true;
            this.buttonPersonenUp.Click += new System.EventHandler(this.buttonPersonenUp_Click);
            // 
            // label6
            // 
            this.label6.Dock = System.Windows.Forms.DockStyle.Left;
            this.label6.Location = new System.Drawing.Point(131, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 26);
            this.label6.TabIndex = 2;
            // 
            // buttonPersonenDelete
            // 
            this.buttonPersonenDelete.Dock = System.Windows.Forms.DockStyle.Left;
            this.buttonPersonenDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPersonenDelete.Image = global::Nilsa.Properties.Resources.ic_cancel_black_18dp;
            this.buttonPersonenDelete.Location = new System.Drawing.Point(93, 0);
            this.buttonPersonenDelete.Margin = new System.Windows.Forms.Padding(0);
            this.buttonPersonenDelete.Name = "buttonPersonenDelete";
            this.buttonPersonenDelete.Size = new System.Drawing.Size(38, 26);
            this.buttonPersonenDelete.TabIndex = 1;
            this.buttonPersonenDelete.UseVisualStyleBackColor = true;
            this.buttonPersonenDelete.Click += new System.EventHandler(this.buttonPersonenDelete_Click);
            // 
            // label7
            // 
            this.label7.Dock = System.Windows.Forms.DockStyle.Left;
            this.label7.Location = new System.Drawing.Point(76, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 26);
            this.label7.TabIndex = 5;
            // 
            // buttonPersonenAdd
            // 
            this.buttonPersonenAdd.Dock = System.Windows.Forms.DockStyle.Left;
            this.buttonPersonenAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPersonenAdd.Image = global::Nilsa.Properties.Resources.ic_group_add_black_18dp;
            this.buttonPersonenAdd.Location = new System.Drawing.Point(0, 0);
            this.buttonPersonenAdd.Margin = new System.Windows.Forms.Padding(0);
            this.buttonPersonenAdd.Name = "buttonPersonenAdd";
            this.buttonPersonenAdd.Size = new System.Drawing.Size(76, 26);
            this.buttonPersonenAdd.TabIndex = 0;
            this.buttonPersonenAdd.UseVisualStyleBackColor = true;
            this.buttonPersonenAdd.Click += new System.EventHandler(this.buttonPersonenAdd_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.comboBoxAlgSetAction);
            this.panel3.Controls.Add(this.comboBox1);
            this.panel3.Controls.Add(this.comboBoxAlgSetList);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(10, 310);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1661, 57);
            this.panel3.TabIndex = 22;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Сообщение";
            // 
            // comboBoxAlgSetAction
            // 
            this.comboBoxAlgSetAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAlgSetAction.FormattingEnabled = true;
            this.comboBoxAlgSetAction.Items.AddRange(new object[] {
            "Не менять",
            "Для всех групп"});
            this.comboBoxAlgSetAction.Location = new System.Drawing.Point(520, 26);
            this.comboBoxAlgSetAction.Name = "comboBoxAlgSetAction";
            this.comboBoxAlgSetAction.Size = new System.Drawing.Size(164, 21);
            this.comboBoxAlgSetAction.TabIndex = 20;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Публикация на стене"});
            this.comboBox1.Location = new System.Drawing.Point(0, 26);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(164, 21);
            this.comboBox1.TabIndex = 4;
            // 
            // comboBoxAlgSetList
            // 
            this.comboBoxAlgSetList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAlgSetList.FormattingEnabled = true;
            this.comboBoxAlgSetList.Items.AddRange(new object[] {
            "Личное сообщение",
            "Публикация на стене"});
            this.comboBoxAlgSetList.Location = new System.Drawing.Point(187, 26);
            this.comboBoxAlgSetList.Name = "comboBoxAlgSetList";
            this.comboBoxAlgSetList.Size = new System.Drawing.Size(327, 21);
            this.comboBoxAlgSetList.TabIndex = 19;
            this.comboBoxAlgSetList.SelectedIndexChanged += new System.EventHandler(this.comboBoxAlgSetList_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(184, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Алгоритм";
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(10, 59);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(1661, 251);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textBox2);
            this.panel1.Controls.Add(this.button9);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.button4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(10, 16);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(0, 10, 0, 10);
            this.panel1.Size = new System.Drawing.Size(1661, 43);
            this.panel1.TabIndex = 21;
            // 
            // textBox2
            // 
            this.textBox2.Dock = System.Windows.Forms.DockStyle.Right;
            this.textBox2.Location = new System.Drawing.Point(1209, 10);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(271, 20);
            this.textBox2.TabIndex = 19;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            this.textBox2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox2_KeyDown);
            // 
            // button9
            // 
            this.button9.Dock = System.Windows.Forms.DockStyle.Right;
            this.button9.Enabled = false;
            this.button9.Location = new System.Drawing.Point(1480, 10);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(22, 23);
            this.button9.TabIndex = 18;
            this.button9.Text = ">";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Right;
            this.label3.Location = new System.Drawing.Point(1502, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 23);
            this.label3.TabIndex = 10;
            // 
            // button4
            // 
            this.button4.Dock = System.Windows.Forms.DockStyle.Right;
            this.button4.Location = new System.Drawing.Point(1537, 10);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(124, 23);
            this.button4.TabIndex = 9;
            this.button4.Text = "Фильтрация списка";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Dock = System.Windows.Forms.DockStyle.Right;
            this.button1.Location = new System.Drawing.Point(1596, 10);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Закрыть";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Dock = System.Windows.Forms.DockStyle.Left;
            this.button2.Location = new System.Drawing.Point(175, 10);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(141, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Послать по списку";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Dock = System.Windows.Forms.DockStyle.Left;
            this.button3.Location = new System.Drawing.Point(10, 10);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(133, 23);
            this.button3.TabIndex = 3;
            this.button3.Text = "Послать выбранному";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // contextMenuStripVectorKoefOut
            // 
            this.contextMenuStripVectorKoefOut.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem5,
            this.toolStripMenuItem6,
            this.toolStripMenuItem7});
            this.contextMenuStripVectorKoefOut.Name = "contextMenuStripVectorKoef";
            this.contextMenuStripVectorKoefOut.Size = new System.Drawing.Size(148, 70);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(147, 22);
            this.toolStripMenuItem5.Text = "Не важно";
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(147, 22);
            this.toolStripMenuItem6.Text = "Важно";
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(147, 22);
            this.toolStripMenuItem7.Text = "Очень важно";
            // 
            // contextMenuStripEQOutMsgValues
            // 
            this.contextMenuStripEQOutMsgValues.Name = "contextMenuStripEQInMsgValues";
            this.contextMenuStripEQOutMsgValues.Size = new System.Drawing.Size(61, 4);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.button7);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.numericUpDown1);
            this.panel2.Controls.Add(this.button2);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.button3);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 650);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(10);
            this.panel2.Size = new System.Drawing.Size(1681, 43);
            this.panel2.TabIndex = 4;
            // 
            // button7
            // 
            this.button7.Dock = System.Windows.Forms.DockStyle.Left;
            this.button7.Location = new System.Drawing.Point(405, 10);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(212, 23);
            this.button7.TabIndex = 6;
            this.button7.Text = "Инициировать от всех Персонажей";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // label5
            // 
            this.label5.Dock = System.Windows.Forms.DockStyle.Left;
            this.label5.Location = new System.Drawing.Point(373, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 23);
            this.label5.TabIndex = 7;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Dock = System.Windows.Forms.DockStyle.Left;
            this.numericUpDown1.Location = new System.Drawing.Point(316, 10);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(57, 20);
            this.numericUpDown1.TabIndex = 4;
            this.numericUpDown1.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // label4
            // 
            this.label4.Dock = System.Windows.Forms.DockStyle.Left;
            this.label4.Location = new System.Drawing.Point(143, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 23);
            this.label4.TabIndex = 5;
            // 
            // timerClose
            // 
            this.timerClose.Interval = 1000;
            this.timerClose.Tick += new System.EventHandler(this.timerClose_Tick);
            // 
            // FormInitGroupDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1681, 693);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormInitGroupDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Инициировать диалог в Группах";
            this.groupBox1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.panelPersonen.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.contextMenuStripVectorKoefOut.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.ListBox listBoxOutMsg;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripVectorKoefOut;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripEQOutMsgValues;
        private System.Windows.Forms.ToolTip toolTipMessage;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox comboBoxAlgSetAction;
        private System.Windows.Forms.ComboBox comboBoxAlgSetList;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Panel panelPersonen;
        private System.Windows.Forms.ListBox listBoxPersonen;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button buttonPersonenUp;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button buttonPersonenDelete;
        private System.Windows.Forms.Button buttonPersonenAdd;
        private System.Windows.Forms.Button buttonPersonenDn;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Timer timerClose;
    }
}