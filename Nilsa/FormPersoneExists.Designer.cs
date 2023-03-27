namespace Nilsa
{
    partial class FormPersoneExists
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
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.checkBoxToAll = new System.Windows.Forms.CheckBox();
            this.buttonIgnore = new System.Windows.Forms.Button();
            this.buttonChange = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listViewContact = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.labelContact = new System.Windows.Forms.Label();
            this.listViewPersone = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.labelPersone = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(10);
            this.label1.Size = new System.Drawing.Size(531, 40);
            this.label1.TabIndex = 0;
            this.label1.Text = "Импортируемый Контактер уже существует в списке Контактеров";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.checkBoxToAll);
            this.panel1.Controls.Add(this.buttonIgnore);
            this.panel1.Controls.Add(this.buttonChange);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 479);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.panel1.Size = new System.Drawing.Size(1363, 46);
            this.panel1.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.button1.Dock = System.Windows.Forms.DockStyle.Left;
            this.button1.Location = new System.Drawing.Point(10, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(121, 36);
            this.button1.TabIndex = 3;
            this.button1.Text = "Прервать";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // checkBoxToAll
            // 
            this.checkBoxToAll.AutoSize = true;
            this.checkBoxToAll.Dock = System.Windows.Forms.DockStyle.Right;
            this.checkBoxToAll.Location = new System.Drawing.Point(965, 5);
            this.checkBoxToAll.Name = "checkBoxToAll";
            this.checkBoxToAll.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.checkBoxToAll.Size = new System.Drawing.Size(140, 36);
            this.checkBoxToAll.TabIndex = 2;
            this.checkBoxToAll.Text = "Применить для всех";
            this.checkBoxToAll.UseVisualStyleBackColor = true;
            // 
            // buttonIgnore
            // 
            this.buttonIgnore.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonIgnore.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonIgnore.Location = new System.Drawing.Point(1105, 5);
            this.buttonIgnore.Name = "buttonIgnore";
            this.buttonIgnore.Size = new System.Drawing.Size(124, 36);
            this.buttonIgnore.TabIndex = 0;
            this.buttonIgnore.Text = "Игнорировать";
            this.buttonIgnore.UseVisualStyleBackColor = true;
            // 
            // buttonChange
            // 
            this.buttonChange.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonChange.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonChange.Location = new System.Drawing.Point(1229, 5);
            this.buttonChange.Name = "buttonChange";
            this.buttonChange.Size = new System.Drawing.Size(124, 36);
            this.buttonChange.TabIndex = 1;
            this.buttonChange.Text = "Добавить";
            this.buttonChange.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 40);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listViewContact);
            this.splitContainer1.Panel1.Controls.Add(this.labelContact);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listViewPersone);
            this.splitContainer1.Panel2.Controls.Add(this.labelPersone);
            this.splitContainer1.Size = new System.Drawing.Size(1363, 439);
            this.splitContainer1.SplitterDistance = 689;
            this.splitContainer1.TabIndex = 3;
            // 
            // listViewContact
            // 
            this.listViewContact.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listViewContact.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewContact.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listViewContact.GridLines = true;
            this.listViewContact.Location = new System.Drawing.Point(0, 24);
            this.listViewContact.Name = "listViewContact";
            this.listViewContact.Size = new System.Drawing.Size(689, 415);
            this.listViewContact.TabIndex = 1;
            this.listViewContact.UseCompatibleStateImageBehavior = false;
            this.listViewContact.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Характеристика";
            this.columnHeader1.Width = 149;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Значение";
            this.columnHeader2.Width = 507;
            // 
            // labelContact
            // 
            this.labelContact.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelContact.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelContact.Location = new System.Drawing.Point(0, 0);
            this.labelContact.Name = "labelContact";
            this.labelContact.Size = new System.Drawing.Size(689, 24);
            this.labelContact.TabIndex = 0;
            this.labelContact.Text = "label2";
            // 
            // listViewPersone
            // 
            this.listViewPersone.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
            this.listViewPersone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewPersone.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listViewPersone.GridLines = true;
            this.listViewPersone.Location = new System.Drawing.Point(0, 24);
            this.listViewPersone.Name = "listViewPersone";
            this.listViewPersone.Size = new System.Drawing.Size(670, 415);
            this.listViewPersone.TabIndex = 2;
            this.listViewPersone.UseCompatibleStateImageBehavior = false;
            this.listViewPersone.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Характеристика";
            this.columnHeader3.Width = 149;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Значение";
            this.columnHeader4.Width = 507;
            // 
            // labelPersone
            // 
            this.labelPersone.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelPersone.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelPersone.Location = new System.Drawing.Point(0, 0);
            this.labelPersone.Name = "labelPersone";
            this.labelPersone.Size = new System.Drawing.Size(670, 24);
            this.labelPersone.TabIndex = 1;
            this.labelPersone.Text = "label3";
            // 
            // FormPersoneExists
            // 
            this.AcceptButton = this.buttonChange;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonIgnore;
            this.ClientSize = new System.Drawing.Size(1363, 525);
            this.ControlBox = false;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormPersoneExists";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Импорт данных";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonChange;
        private System.Windows.Forms.Button buttonIgnore;
        public System.Windows.Forms.CheckBox checkBoxToAll;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label labelContact;
        private System.Windows.Forms.Label labelPersone;
        private System.Windows.Forms.ListView listViewContact;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ListView listViewPersone;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Button button1;
    }
}