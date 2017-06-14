namespace ClientApp
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
			this.MessageBox = new System.Windows.Forms.TextBox();
			this.SubmitBtn = new System.Windows.Forms.Button();
			this.outputBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// MessageBox
			// 
			this.MessageBox.Location = new System.Drawing.Point(12, 419);
			this.MessageBox.Name = "MessageBox";
			this.MessageBox.Size = new System.Drawing.Size(479, 22);
			this.MessageBox.TabIndex = 0;
			// 
			// SubmitBtn
			// 
			this.SubmitBtn.Location = new System.Drawing.Point(497, 418);
			this.SubmitBtn.Name = "SubmitBtn";
			this.SubmitBtn.Size = new System.Drawing.Size(76, 25);
			this.SubmitBtn.TabIndex = 1;
			this.SubmitBtn.Text = "Submit";
			this.SubmitBtn.UseVisualStyleBackColor = true;
			this.SubmitBtn.Click += new System.EventHandler(this.SubmitBtn_Click);
			// 
			// outputBox
			// 
			this.outputBox.Location = new System.Drawing.Point(12, 12);
			this.outputBox.Multiline = true;
			this.outputBox.Name = "outputBox";
			this.outputBox.Size = new System.Drawing.Size(558, 400);
			this.outputBox.TabIndex = 2;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(582, 453);
			this.Controls.Add(this.outputBox);
			this.Controls.Add(this.SubmitBtn);
			this.Controls.Add(this.MessageBox);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox MessageBox;
		private System.Windows.Forms.Button SubmitBtn;
		private System.Windows.Forms.TextBox outputBox;
	}
}

