namespace controller
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
            this.localIP = new System.Windows.Forms.TextBox();
            this.start = new System.Windows.Forms.Button();
            this.receiverIP = new System.Windows.Forms.TextBox();
            this.cancel = new System.Windows.Forms.Button();
            this.localIPTextBox = new System.Windows.Forms.TextBox();
            this.receiverIPTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // localIP
            // 
            this.localIP.Location = new System.Drawing.Point(110, 13);
            this.localIP.Name = "localIP";
            this.localIP.ReadOnly = true;
            this.localIP.Size = new System.Drawing.Size(214, 20);
            this.localIP.TabIndex = 0;
            // 
            // start
            // 
            this.start.Location = new System.Drawing.Point(136, 65);
            this.start.Name = "start";
            this.start.Size = new System.Drawing.Size(78, 23);
            this.start.TabIndex = 1;
            this.start.Text = "Start";
            this.start.UseVisualStyleBackColor = true;
            this.start.Click += new System.EventHandler(this.Start_Click);
            // 
            // receiverIP
            // 
            this.receiverIP.Location = new System.Drawing.Point(110, 39);
            this.receiverIP.Name = "receiverIP";
            this.receiverIP.Size = new System.Drawing.Size(214, 20);
            this.receiverIP.TabIndex = 0;
            // 
            // cancel
            // 
            this.cancel.Location = new System.Drawing.Point(217, 65);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(78, 23);
            this.cancel.TabIndex = 1;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // localIPTextBox
            // 
            this.localIPTextBox.Location = new System.Drawing.Point(4, 13);
            this.localIPTextBox.Name = "Local IP TextBox";
            this.localIPTextBox.ReadOnly = true;
            this.localIPTextBox.Size = new System.Drawing.Size(100, 20);
            this.localIPTextBox.TabIndex = 2;
            this.localIPTextBox.Text = "Local IP:";
            // 
            // receiverIPTextBox
            // 
            this.receiverIPTextBox.Location = new System.Drawing.Point(4, 40);
            this.receiverIPTextBox.Name = "Receiver IP TextBox";
            this.receiverIPTextBox.ReadOnly = true;
            this.receiverIPTextBox.Size = new System.Drawing.Size(100, 20);
            this.receiverIPTextBox.TabIndex = 3;
            this.receiverIPTextBox.Text = "Receiver IP:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(431, 100);
            this.Controls.Add(this.receiverIPTextBox);
            this.Controls.Add(this.localIPTextBox);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.start);
            this.Controls.Add(this.receiverIP);
            this.Controls.Add(this.localIP);
            this.Name = "Form1";
            this.Text = "Controller";
            this.Load += new System.EventHandler(this.Controller_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox localIP;
        private System.Windows.Forms.Button start;
        private System.Windows.Forms.TextBox receiverIP;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.TextBox localIPTextBox;
        private System.Windows.Forms.TextBox receiverIPTextBox;
    }
}

