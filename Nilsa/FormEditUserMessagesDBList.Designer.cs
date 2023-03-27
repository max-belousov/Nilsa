namespace Nilsa
{
    partial class FormEditUserMessagesDBList
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.clbDBs = new System.Windows.Forms.ListBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.buttonRename = new System.Windows.Forms.Button();
            this.buttonDeletePrefix = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.clbDBs);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(10);
            this.groupBox1.Size = new System.Drawing.Size(416, 179);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Базы тематик сообщений";
            // 
            // clbDBs
            // 
            this.clbDBs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbDBs.FormattingEnabled = true;
            this.clbDBs.Location = new System.Drawing.Point(10, 23);
            this.clbDBs.Name = "clbDBs";
            this.clbDBs.Size = new System.Drawing.Size(396, 146);
            this.clbDBs.TabIndex = 0;
            this.clbDBs.SelectedIndexChanged += new System.EventHandler(this.clbDBs_SelectedIndexChanged);
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(329, 185);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Закрыть";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(3, 185);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 3;
            this.button3.Text = "Удалить";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // buttonRename
            // 
            this.buttonRename.Location = new System.Drawing.Point(84, 185);
            this.buttonRename.Name = "buttonRename";
            this.buttonRename.Size = new System.Drawing.Size(102, 23);
            this.buttonRename.TabIndex = 4;
            this.buttonRename.Text = "Переименовать";
            this.buttonRename.UseVisualStyleBackColor = true;
            this.buttonRename.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonDeletePrefix
            // 
            this.buttonDeletePrefix.Location = new System.Drawing.Point(192, 185);
            this.buttonDeletePrefix.Name = "buttonDeletePrefix";
            this.buttonDeletePrefix.Size = new System.Drawing.Size(116, 23);
            this.buttonDeletePrefix.TabIndex = 5;
            this.buttonDeletePrefix.Text = "Заменить префикс Стадии общения";
            this.buttonDeletePrefix.UseVisualStyleBackColor = true;
            this.buttonDeletePrefix.Click += new System.EventHandler(this.buttonDeletePrefix_Click);
            // 
            // FormEditUserMessagesDBList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(416, 215);
            this.Controls.Add(this.buttonDeletePrefix);
            this.Controls.Add(this.buttonRename);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormEditUserMessagesDBList";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Набор запрещенных баз тематик сообщений";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ListBox clbDBs;
        private System.Windows.Forms.Button buttonRename;
        private System.Windows.Forms.Button buttonDeletePrefix;
    }
}