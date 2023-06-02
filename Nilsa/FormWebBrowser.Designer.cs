namespace Nilsa
{
    partial class FormWebBrowser
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWebBrowser));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonRefresh = new System.Windows.Forms.ToolStripButton();
            this.urlTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.tsbGo = new System.Windows.Forms.ToolStripButton();
            this.tsbGetMusicLink = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonExit = new System.Windows.Forms.Button();
            this.timer0 = new System.Windows.Forms.Timer(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.timerAutoClose = new System.Windows.Forms.Timer(this.components);
            this.timer3 = new System.Windows.Forms.Timer(this.components);
            this.timer4 = new System.Windows.Forms.Timer(this.components);
            this.timer5 = new System.Windows.Forms.Timer(this.components);
            this.timerAutorize = new System.Windows.Forms.Timer(this.components);
            this.timerDisconnect = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.loadingStatusLabelTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.loadingStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.titleStatusLabelTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.titleStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.addressStatusLabelTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.addressStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.timerCondition = new System.Windows.Forms.Timer(this.components);
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 82);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(10);
            this.groupBox1.Size = new System.Drawing.Size(1074, 384);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Персонаж";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(75, 75);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonRefresh,
            this.urlTextBox,
            this.tsbGo,
            this.tsbGetMusicLink,
            this.toolStripButton1,
            this.toolStripButton2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1074, 82);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonRefresh
            // 
            this.toolStripButtonRefresh.AutoSize = false;
            this.toolStripButtonRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRefresh.Image = global::Nilsa.Properties.Resources._refresh1;
            this.toolStripButtonRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRefresh.Name = "toolStripButtonRefresh";
            this.toolStripButtonRefresh.Size = new System.Drawing.Size(75, 75);
            this.toolStripButtonRefresh.Text = "Обновить";
            this.toolStripButtonRefresh.Click += new System.EventHandler(this.toolStripButtonRefresh_Click);
            // 
            // urlTextBox
            // 
            this.urlTextBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.urlTextBox.Name = "urlTextBox";
            this.urlTextBox.Size = new System.Drawing.Size(500, 82);
            this.urlTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.urlTextBox_KeyUp);
            // 
            // tsbGo
            // 
            this.tsbGo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbGo.Image = global::Nilsa.Properties.Resources.ic_play_arrow_black_48dp;
            this.tsbGo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbGo.Name = "tsbGo";
            this.tsbGo.Size = new System.Drawing.Size(79, 79);
            this.tsbGo.Text = "Перейти по адресу...";
            this.tsbGo.Click += new System.EventHandler(this.tsbGo_Click);
            // 
            // tsbGetMusicLink
            // 
            this.tsbGetMusicLink.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbGetMusicLink.Image = global::Nilsa.Properties.Resources._open_browser;
            this.tsbGetMusicLink.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbGetMusicLink.Name = "tsbGetMusicLink";
            this.tsbGetMusicLink.Size = new System.Drawing.Size(79, 79);
            this.tsbGetMusicLink.Text = "Получить ссылки на аудио-файлы";
            this.tsbGetMusicLink.Click += new System.EventHandler(this.tsbGetMusicLink_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 79);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 79);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.trackBar1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.buttonExit);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 466);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(10);
            this.panel1.Size = new System.Drawing.Size(1074, 46);
            this.panel1.TabIndex = 1;
            // 
            // trackBar1
            // 
            this.trackBar1.Dock = System.Windows.Forms.DockStyle.Left;
            this.trackBar1.Enabled = false;
            this.trackBar1.LargeChange = 1;
            this.trackBar1.Location = new System.Drawing.Point(10, 10);
            this.trackBar1.Maximum = 5;
            this.trackBar1.Minimum = -5;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(346, 26);
            this.trackBar1.TabIndex = 2;
            this.trackBar1.Value = -2;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Right;
            this.label1.Location = new System.Drawing.Point(885, 10);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 7, 20, 0);
            this.label1.Size = new System.Drawing.Size(55, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "label1";
            // 
            // buttonExit
            // 
            this.buttonExit.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonExit.Location = new System.Drawing.Point(940, 10);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(124, 26);
            this.buttonExit.TabIndex = 0;
            this.buttonExit.Text = "Закрыть";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // timer0
            // 
            this.timer0.Interval = 1000;
            this.timer0.Tick += new System.EventHandler(this.timer0_Tick);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Interval = 1000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // timerAutoClose
            // 
            this.timerAutoClose.Interval = 1000;
            this.timerAutoClose.Tick += new System.EventHandler(this.timerAutoClose_Tick);
            // 
            // timer3
            // 
            this.timer3.Interval = 1000;
            this.timer3.Tick += new System.EventHandler(this.timer3_Tick);
            // 
            // timer4
            // 
            this.timer4.Interval = 1000;
            this.timer4.Tick += new System.EventHandler(this.timer4_Tick);
            // 
            // timer5
            // 
            this.timer5.Interval = 1000;
            this.timer5.Tick += new System.EventHandler(this.timer5_Tick);
            // 
            // timerAutorize
            // 
            this.timerAutorize.Interval = 3000;
            this.timerAutorize.Tick += new System.EventHandler(this.timerAutorize_Tick);
            // 
            // timerDisconnect
            // 
            this.timerDisconnect.Interval = 3000;
            this.timerDisconnect.Tick += new System.EventHandler(this.timerDisconnect_Tick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadingStatusLabelTime,
            this.loadingStatusLabel,
            this.titleStatusLabelTime,
            this.titleStatusLabel,
            this.addressStatusLabelTime,
            this.addressStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 512);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1074, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // loadingStatusLabelTime
            // 
            this.loadingStatusLabelTime.AutoSize = false;
            this.loadingStatusLabelTime.AutoToolTip = true;
            this.loadingStatusLabelTime.Name = "loadingStatusLabelTime";
            this.loadingStatusLabelTime.Size = new System.Drawing.Size(80, 17);
            this.loadingStatusLabelTime.Text = "20:00:09.000";
            this.loadingStatusLabelTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // loadingStatusLabel
            // 
            this.loadingStatusLabel.AutoSize = false;
            this.loadingStatusLabel.AutoToolTip = true;
            this.loadingStatusLabel.Name = "loadingStatusLabel";
            this.loadingStatusLabel.Size = new System.Drawing.Size(100, 17);
            this.loadingStatusLabel.Text = "Loading...";
            this.loadingStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // titleStatusLabelTime
            // 
            this.titleStatusLabelTime.AutoSize = false;
            this.titleStatusLabelTime.AutoToolTip = true;
            this.titleStatusLabelTime.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.titleStatusLabelTime.Name = "titleStatusLabelTime";
            this.titleStatusLabelTime.Size = new System.Drawing.Size(80, 17);
            this.titleStatusLabelTime.Text = "20:00:09.000";
            // 
            // titleStatusLabel
            // 
            this.titleStatusLabel.AutoSize = false;
            this.titleStatusLabel.AutoToolTip = true;
            this.titleStatusLabel.Name = "titleStatusLabel";
            this.titleStatusLabel.Size = new System.Drawing.Size(300, 17);
            this.titleStatusLabel.Text = "titleStatusLabel";
            this.titleStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // addressStatusLabelTime
            // 
            this.addressStatusLabelTime.AutoSize = false;
            this.addressStatusLabelTime.AutoToolTip = true;
            this.addressStatusLabelTime.Name = "addressStatusLabelTime";
            this.addressStatusLabelTime.Size = new System.Drawing.Size(80, 17);
            this.addressStatusLabelTime.Text = "20:00:09.000";
            this.addressStatusLabelTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // addressStatusLabel
            // 
            this.addressStatusLabel.AutoToolTip = true;
            this.addressStatusLabel.Name = "addressStatusLabel";
            this.addressStatusLabel.Size = new System.Drawing.Size(107, 17);
            this.addressStatusLabel.Text = "addressStatusLabel";
            this.addressStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // timerCondition
            // 
            this.timerCondition.Interval = 1000;
            this.timerCondition.Tick += new System.EventHandler(this.timerCondition_Tick);
            // 
            // FormWebBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1074, 534);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormWebBrowser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Браузер";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormWebBrowser_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormWebBrowser_FormClosed);
            this.Shown += new System.EventHandler(this.FormWebBrowser_Shown);
            this.ResizeEnd += new System.EventHandler(this.FormWebBrowser_ResizeEnd);
            this.LocationChanged += new System.EventHandler(this.FormWebBrowser_LocationChanged);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonRefresh;
        private System.Windows.Forms.Timer timer0;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Timer timerAutoClose;
        private System.Windows.Forms.Timer timer3;
        private System.Windows.Forms.Timer timer4;
        private System.Windows.Forms.Timer timer5;
        private System.Windows.Forms.Timer timerAutorize;
        private System.Windows.Forms.Timer timerDisconnect;
        private System.Windows.Forms.ToolStripButton tsbGetMusicLink;
        private System.Windows.Forms.ToolStripTextBox urlTextBox;
        private System.Windows.Forms.ToolStripButton tsbGo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel addressStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel titleStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel loadingStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel loadingStatusLabelTime;
        private System.Windows.Forms.ToolStripStatusLabel titleStatusLabelTime;
        private System.Windows.Forms.ToolStripStatusLabel addressStatusLabelTime;
        private System.Windows.Forms.Timer timerCondition;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
    }
}