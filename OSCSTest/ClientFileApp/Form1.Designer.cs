namespace ClientFileApp
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
			this.pathBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.fileDialogButton = new System.Windows.Forms.Button();
			this.sendBtn = new System.Windows.Forms.Button();
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.SuspendLayout();
			// 
			// pathBox
			// 
			this.pathBox.Location = new System.Drawing.Point(80, 180);
			this.pathBox.Name = "pathBox";
			this.pathBox.Size = new System.Drawing.Size(364, 22);
			this.pathBox.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(33, 183);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(41, 17);
			this.label1.TabIndex = 1;
			this.label1.Text = "Path:";
			// 
			// fileDialogButton
			// 
			this.fileDialogButton.Location = new System.Drawing.Point(450, 176);
			this.fileDialogButton.Name = "fileDialogButton";
			this.fileDialogButton.Size = new System.Drawing.Size(70, 30);
			this.fileDialogButton.TabIndex = 2;
			this.fileDialogButton.Text = "...";
			this.fileDialogButton.UseVisualStyleBackColor = true;
			this.fileDialogButton.Click += new System.EventHandler(this.fileDialogButton_Click);
			// 
			// sendBtn
			// 
			this.sendBtn.Location = new System.Drawing.Point(450, 212);
			this.sendBtn.Name = "sendBtn";
			this.sendBtn.Size = new System.Drawing.Size(70, 30);
			this.sendBtn.TabIndex = 3;
			this.sendBtn.Text = "Send";
			this.sendBtn.UseVisualStyleBackColor = true;
			this.sendBtn.Click += new System.EventHandler(this.sendBtn_Click);
			// 
			// progressBar1
			// 
			this.progressBar1.Location = new System.Drawing.Point(36, 418);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(484, 23);
			this.progressBar1.TabIndex = 4;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(582, 453);
			this.Controls.Add(this.progressBar1);
			this.Controls.Add(this.sendBtn);
			this.Controls.Add(this.fileDialogButton);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.pathBox);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox pathBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button fileDialogButton;
		private System.Windows.Forms.Button sendBtn;
		private System.Windows.Forms.ProgressBar progressBar1;
	}
}

