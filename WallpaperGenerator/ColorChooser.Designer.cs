namespace GuiLabs.Wallpaper
{
	partial class ColorChooser
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.ButtonChooseColor = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// ButtonChooseColor
			// 
			this.ButtonChooseColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
			this.ButtonChooseColor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ButtonChooseColor.Location = new System.Drawing.Point(0, 0);
			this.ButtonChooseColor.Name = "ButtonChooseColor";
			this.ButtonChooseColor.Size = new System.Drawing.Size(66, 61);
			this.ButtonChooseColor.TabIndex = 0;
			this.ButtonChooseColor.UseVisualStyleBackColor = false;
			this.ButtonChooseColor.Click += new System.EventHandler(this.ButtonChooseColor_Click);
			// 
			// ColorChooser
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.ButtonChooseColor);
			this.Name = "ColorChooser";
			this.Size = new System.Drawing.Size(66, 61);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button ButtonChooseColor;
	}
}
