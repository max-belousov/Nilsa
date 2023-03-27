namespace Nilsa
{
    partial class FormEditEQMessagesDB
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEditEQMessagesDB));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.grid1 = new SourceGrid.Grid();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button9 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.button6 = new System.Windows.Forms.ToolStripButton();
            this.button5 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.button7 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.button2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.button10 = new System.Windows.Forms.ToolStripButton();
            this.button12 = new System.Windows.Forms.ToolStripButton();
            this.tbInvertSelection = new System.Windows.Forms.ToolStripButton();
            this.button13 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.button11 = new System.Windows.Forms.ToolStripButton();
            this.buttonImportCSVFromServer = new System.Windows.Forms.ToolStripButton();
            this.button8 = new System.Windows.Forms.ToolStripButton();
            this.buttonExportCSVToServer = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.button4 = new System.Windows.Forms.ToolStripButton();
            this.button3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripMenuItemColumnsOrder = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.changeDelimitersToTilda = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.groupBox1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.grid1);
            this.groupBox1.Controls.Add(this.panel2);
            this.groupBox1.Controls.Add(this.toolStrip1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(10);
            this.groupBox1.Size = new System.Drawing.Size(1477, 443);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Сообщения";
            // 
            // grid1
            // 
            this.grid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid1.EnableSort = true;
            this.grid1.Location = new System.Drawing.Point(10, 115);
            this.grid1.Name = "grid1";
            this.grid1.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.grid1.SelectionMode = SourceGrid.GridSelectionMode.Row;
            this.grid1.Size = new System.Drawing.Size(1457, 318);
            this.grid1.TabIndex = 20;
            this.grid1.TabStop = true;
            this.grid1.ToolTipText = "";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.button9);
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(10, 80);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.panel2.Size = new System.Drawing.Size(1457, 35);
            this.panel2.TabIndex = 18;
            // 
            // button9
            // 
            this.button9.Dock = System.Windows.Forms.DockStyle.Left;
            this.button9.Enabled = false;
            this.button9.Location = new System.Drawing.Point(271, 5);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(22, 25);
            this.button9.TabIndex = 17;
            this.button9.Text = ">";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.textBox1.Location = new System.Drawing.Point(0, 5);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(271, 20);
            this.textBox1.TabIndex = 16;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(50, 50);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.button6,
            this.button5,
            this.toolStripSeparator1,
            this.button7,
            this.toolStripSeparator2,
            this.button2,
            this.toolStripSeparator3,
            this.button10,
            this.button12,
            this.tbInvertSelection,
            this.button13,
            this.toolStripSeparator4,
            this.button11,
            this.buttonImportCSVFromServer,
            this.button8,
            this.buttonExportCSVToServer,
            this.toolStripSeparator5,
            this.button4,
            this.button3,
            this.toolStripDropDownButton1});
            this.toolStrip1.Location = new System.Drawing.Point(10, 23);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1457, 57);
            this.toolStrip1.TabIndex = 19;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // button6
            // 
            this.button6.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.button6.Image = ((System.Drawing.Image)(resources.GetObject("button6.Image")));
            this.button6.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(54, 54);
            this.button6.Text = "Добавить";
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button5
            // 
            this.button5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.button5.Image = ((System.Drawing.Image)(resources.GetObject("button5.Image")));
            this.button5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(54, 54);
            this.button5.Text = "Удалить";
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 57);
            // 
            // button7
            // 
            this.button7.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.button7.Image = ((System.Drawing.Image)(resources.GetObject("button7.Image")));
            this.button7.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(54, 54);
            this.button7.Text = "Копировать";
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 57);
            // 
            // button2
            // 
            this.button2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.button2.Image = ((System.Drawing.Image)(resources.GetObject("button2.Image")));
            this.button2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(54, 54);
            this.button2.Text = "Редактировать";
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 57);
            // 
            // button10
            // 
            this.button10.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.button10.Image = ((System.Drawing.Image)(resources.GetObject("button10.Image")));
            this.button10.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(54, 54);
            this.button10.Text = "Отметить все";
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // button12
            // 
            this.button12.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.button12.Image = ((System.Drawing.Image)(resources.GetObject("button12.Image")));
            this.button12.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(54, 54);
            this.button12.Text = "Снять все отметки";
            this.button12.Click += new System.EventHandler(this.button12_Click);
            // 
            // tbInvertSelection
            // 
            this.tbInvertSelection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbInvertSelection.Image = global::Nilsa.Properties.Resources._select_invert;
            this.tbInvertSelection.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbInvertSelection.Name = "tbInvertSelection";
            this.tbInvertSelection.Size = new System.Drawing.Size(54, 54);
            this.tbInvertSelection.Text = "Инвертировать выделение";
            this.tbInvertSelection.Click += new System.EventHandler(this.tbInvertSelection_Click);
            // 
            // button13
            // 
            this.button13.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.button13.Image = ((System.Drawing.Image)(resources.GetObject("button13.Image")));
            this.button13.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(54, 54);
            this.button13.Text = "Отметить по фильтру";
            this.button13.Click += new System.EventHandler(this.button13_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 57);
            // 
            // button11
            // 
            this.button11.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.button11.Image = ((System.Drawing.Image)(resources.GetObject("button11.Image")));
            this.button11.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(54, 54);
            this.button11.Text = "Импорт из CSV";
            this.button11.Click += new System.EventHandler(this.button11_Click);
            // 
            // buttonImportCSVFromServer
            // 
            this.buttonImportCSVFromServer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonImportCSVFromServer.Image = global::Nilsa.Properties.Resources._import_file;
            this.buttonImportCSVFromServer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonImportCSVFromServer.Name = "buttonImportCSVFromServer";
            this.buttonImportCSVFromServer.Size = new System.Drawing.Size(54, 54);
            this.buttonImportCSVFromServer.Text = "Импорт с Сервера";
            this.buttonImportCSVFromServer.Click += new System.EventHandler(this.buttonImportCSVFromServer_Click);
            // 
            // button8
            // 
            this.button8.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.button8.Image = ((System.Drawing.Image)(resources.GetObject("button8.Image")));
            this.button8.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(54, 54);
            this.button8.Text = "Экспорт в CSV";
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // buttonExportCSVToServer
            // 
            this.buttonExportCSVToServer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonExportCSVToServer.Image = global::Nilsa.Properties.Resources._export_csv;
            this.buttonExportCSVToServer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonExportCSVToServer.Name = "buttonExportCSVToServer";
            this.buttonExportCSVToServer.Size = new System.Drawing.Size(54, 54);
            this.buttonExportCSVToServer.Text = "Экспорт отм. на Сервер";
            this.buttonExportCSVToServer.Click += new System.EventHandler(this.buttonExportCSVToServer_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 57);
            // 
            // button4
            // 
            this.button4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.button4.Image = global::Nilsa.Properties.Resources.filter_list_on;
            this.button4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(54, 54);
            this.button4.Text = "Фильтрация";
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.button3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.button3.Image = global::Nilsa.Properties.Resources._select_message;
            this.button3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(54, 54);
            this.button3.Text = "Выбрать Сообщение";
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemColumnsOrder,
            this.toolStripMenuItem2,
            this.changeDelimitersToTilda});
            this.toolStripDropDownButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.toolStripDropDownButton1.Image = global::Nilsa.Properties.Resources.ic_arrow_drop_down_grey600_48dp;
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(63, 54);
            this.toolStripDropDownButton1.Text = "Сервис";
            // 
            // toolStripMenuItemColumnsOrder
            // 
            this.toolStripMenuItemColumnsOrder.Margin = new System.Windows.Forms.Padding(10);
            this.toolStripMenuItemColumnsOrder.Name = "toolStripMenuItemColumnsOrder";
            this.toolStripMenuItemColumnsOrder.Size = new System.Drawing.Size(413, 24);
            this.toolStripMenuItemColumnsOrder.Text = "Порядок столбцов";
            this.toolStripMenuItemColumnsOrder.Click += new System.EventHandler(this.toolStripMenuItemColumnsOrder_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(410, 6);
            // 
            // changeDelimitersToTilda
            // 
            this.changeDelimitersToTilda.Margin = new System.Windows.Forms.Padding(10);
            this.changeDelimitersToTilda.Name = "changeDelimitersToTilda";
            this.changeDelimitersToTilda.Size = new System.Drawing.Size(413, 24);
            this.changeDelimitersToTilda.Text = "Заменить разделители запятые на тильды";
            this.changeDelimitersToTilda.Click += new System.EventHandler(this.changeDelimitersToTilda_Click);
            // 
            // FormEditEQMessagesDB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1477, 443);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormEditEQMessagesDB";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Редактирование базы Сообщений Контактера";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormEditEQMessagesDB_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton button6;
        private System.Windows.Forms.ToolStripButton button5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton button7;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton button2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton button10;
        private System.Windows.Forms.ToolStripButton button12;
        private System.Windows.Forms.ToolStripButton button13;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton button11;
        private System.Windows.Forms.ToolStripButton button8;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton button4;
        private System.Windows.Forms.ToolStripButton tbInvertSelection;
        public System.Windows.Forms.ToolStripButton button3;
        private System.Windows.Forms.ToolStripButton buttonImportCSVFromServer;
        private System.Windows.Forms.ToolStripButton buttonExportCSVToServer;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem changeDelimitersToTilda;
        private SourceGrid.Grid grid1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemColumnsOrder;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
    }
}