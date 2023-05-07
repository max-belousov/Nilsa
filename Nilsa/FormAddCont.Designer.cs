namespace Nilsa
{
    partial class FormAddCont
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
            this.firstNameTextBox = new System.Windows.Forms.TextBox();
            this.lastNameTextBox = new System.Windows.Forms.TextBox();
            this.cidTextBox = new System.Windows.Forms.TextBox();
            this.firstNameLabel = new System.Windows.Forms.Label();
            this.lastNameLabel = new System.Windows.Forms.Label();
            this.cidLabel = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.ownerLabel = new System.Windows.Forms.Label();
            this.ownerTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // firstNameTextBox
            // 
            this.firstNameTextBox.Location = new System.Drawing.Point(50, 60);
            this.firstNameTextBox.Name = "firstNameTextBox";
            this.firstNameTextBox.Size = new System.Drawing.Size(300, 20);
            this.firstNameTextBox.TabIndex = 0;
            // 
            // lastNameTextBox
            // 
            this.lastNameTextBox.Location = new System.Drawing.Point(50, 160);
            this.lastNameTextBox.Name = "lastNameTextBox";
            this.lastNameTextBox.Size = new System.Drawing.Size(300, 20);
            this.lastNameTextBox.TabIndex = 1;
            // 
            // cidTextBox
            // 
            this.cidTextBox.Location = new System.Drawing.Point(50, 260);
            this.cidTextBox.Name = "cidTextBox";
            this.cidTextBox.Size = new System.Drawing.Size(300, 20);
            this.cidTextBox.TabIndex = 2;
            // 
            // firstNameLabel
            // 
            this.firstNameLabel.Location = new System.Drawing.Point(125, 20);
            this.firstNameLabel.Name = "firstNameLabel";
            this.firstNameLabel.Size = new System.Drawing.Size(150, 15);
            this.firstNameLabel.TabIndex = 3;
            this.firstNameLabel.Text = "Имя нового контактера";
            this.firstNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lastNameLabel
            // 
            this.lastNameLabel.Location = new System.Drawing.Point(100, 120);
            this.lastNameLabel.Name = "lastNameLabel";
            this.lastNameLabel.Size = new System.Drawing.Size(200, 15);
            this.lastNameLabel.TabIndex = 4;
            this.lastNameLabel.Text = "Фамилия нового контактера";
            this.lastNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cidLabel
            // 
            this.cidLabel.Location = new System.Drawing.Point(100, 220);
            this.cidLabel.Name = "cidLabel";
            this.cidLabel.Size = new System.Drawing.Size(200, 15);
            this.cidLabel.TabIndex = 5;
            this.cidLabel.Text = "Cid контактера";
            this.cidLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(222, 399);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(150, 50);
            this.okButton.TabIndex = 12;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(12, 399);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(150, 50);
            this.cancelButton.TabIndex = 13;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // ownerLabel
            // 
            this.ownerLabel.Location = new System.Drawing.Point(100, 320);
            this.ownerLabel.Name = "ownerLabel";
            this.ownerLabel.Size = new System.Drawing.Size(200, 15);
            this.ownerLabel.TabIndex = 14;
            this.ownerLabel.Text = "Соцсеть контактера";
            this.ownerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ownerTextBox
            // 
            this.ownerTextBox.Location = new System.Drawing.Point(50, 360);
            this.ownerTextBox.Name = "ownerTextBox";
            this.ownerTextBox.Size = new System.Drawing.Size(300, 20);
            this.ownerTextBox.TabIndex = 15;
            // 
            // FormAddCont
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 461);
            this.Controls.Add(this.ownerTextBox);
            this.Controls.Add(this.ownerLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cidLabel);
            this.Controls.Add(this.lastNameLabel);
            this.Controls.Add(this.firstNameLabel);
            this.Controls.Add(this.cidTextBox);
            this.Controls.Add(this.lastNameTextBox);
            this.Controls.Add(this.firstNameTextBox);
            this.MaximumSize = new System.Drawing.Size(400, 500);
            this.MinimumSize = new System.Drawing.Size(400, 500);
            this.Name = "FormAddCont";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Добавление контактера";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox firstNameTextBox;
        private System.Windows.Forms.TextBox lastNameTextBox;
        private System.Windows.Forms.TextBox cidTextBox;
        private System.Windows.Forms.Label firstNameLabel;
        private System.Windows.Forms.Label lastNameLabel;
        private System.Windows.Forms.Label cidLabel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label ownerLabel;
        private System.Windows.Forms.TextBox ownerTextBox;
    }
}