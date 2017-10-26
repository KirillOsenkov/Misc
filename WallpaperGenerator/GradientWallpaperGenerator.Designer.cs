namespace GuiLabs.Wallpaper
{
	partial class GradientWallpaperGenerator
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
            this.drawWindow1 = new GuiLabs.Canvas.DrawWindow();
            this.ButtonSetAsWallpaper = new System.Windows.Forms.Button();
            this.ButtonFillRandom = new System.Windows.Forms.Button();
            this.ButtonSave = new System.Windows.Forms.Button();
            this.ColorBottomRight = new GuiLabs.Wallpaper.ColorChooser();
            this.ColorBottomLeft = new GuiLabs.Wallpaper.ColorChooser();
            this.ColorTopRight = new GuiLabs.Wallpaper.ColorChooser();
            this.ColorTopLeft = new GuiLabs.Wallpaper.ColorChooser();
            this.SuspendLayout();
            // 
            // drawWindow1
            // 
            this.drawWindow1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.drawWindow1.Location = new System.Drawing.Point(48, 48);
            this.drawWindow1.Name = "drawWindow1";
            this.drawWindow1.ShouldRedrawOnWindowPaint = true;
            this.drawWindow1.Size = new System.Drawing.Size(648, 472);
            this.drawWindow1.TabIndex = 7;
            this.drawWindow1.TabStop = false;
            this.drawWindow1.Repaint += new GuiLabs.Canvas.Events.RepaintHandler(this.drawWindow1_Repaint);
            this.drawWindow1.Resize += new System.EventHandler(this.drawWindow1_Resize);
            // 
            // ButtonSetAsWallpaper
            // 
            this.ButtonSetAsWallpaper.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButtonSetAsWallpaper.Location = new System.Drawing.Point(204, 10);
            this.ButtonSetAsWallpaper.Name = "ButtonSetAsWallpaper";
            this.ButtonSetAsWallpaper.Size = new System.Drawing.Size(152, 32);
            this.ButtonSetAsWallpaper.TabIndex = 1;
            this.ButtonSetAsWallpaper.Text = "Set as wallpaper";
            this.ButtonSetAsWallpaper.UseVisualStyleBackColor = true;
            this.ButtonSetAsWallpaper.Click += new System.EventHandler(this.ButtonSetAsWallpaper_Click);
            // 
            // ButtonFillRandom
            // 
            this.ButtonFillRandom.Location = new System.Drawing.Point(54, 10);
            this.ButtonFillRandom.Name = "ButtonFillRandom";
            this.ButtonFillRandom.Size = new System.Drawing.Size(144, 32);
            this.ButtonFillRandom.TabIndex = 0;
            this.ButtonFillRandom.Text = "Randomize";
            this.ButtonFillRandom.UseVisualStyleBackColor = true;
            this.ButtonFillRandom.Click += new System.EventHandler(this.ButtonFillRandom_Click);
            // 
            // ButtonSave
            // 
            this.ButtonSave.Location = new System.Drawing.Point(362, 10);
            this.ButtonSave.Name = "ButtonSave";
            this.ButtonSave.Size = new System.Drawing.Size(176, 32);
            this.ButtonSave.TabIndex = 2;
            this.ButtonSave.Text = "Save colors to text file";
            this.ButtonSave.UseVisualStyleBackColor = true;
            this.ButtonSave.Click += new System.EventHandler(this.ButtonSave_Click);
            // 
            // ColorBottomRight
            // 
            this.ColorBottomRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ColorBottomRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(42)))), ((int)(((byte)(85)))));
            this.ColorBottomRight.Location = new System.Drawing.Point(696, 520);
            this.ColorBottomRight.Name = "ColorBottomRight";
            this.ColorBottomRight.Size = new System.Drawing.Size(48, 48);
            this.ColorBottomRight.TabIndex = 5;
            this.ColorBottomRight.BackColorChanged += new System.EventHandler(this.ColorBottomRight_BackColorChanged);
            // 
            // ColorBottomLeft
            // 
            this.ColorBottomLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ColorBottomLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(82)))), ((int)(((byte)(253)))));
            this.ColorBottomLeft.Location = new System.Drawing.Point(0, 520);
            this.ColorBottomLeft.Name = "ColorBottomLeft";
            this.ColorBottomLeft.Size = new System.Drawing.Size(48, 48);
            this.ColorBottomLeft.TabIndex = 6;
            this.ColorBottomLeft.BackColorChanged += new System.EventHandler(this.ColorBottomLeft_BackColorChanged);
            // 
            // ColorTopRight
            // 
            this.ColorTopRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ColorTopRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(111)))), ((int)(((byte)(55)))), ((int)(((byte)(170)))));
            this.ColorTopRight.Location = new System.Drawing.Point(696, 0);
            this.ColorTopRight.Name = "ColorTopRight";
            this.ColorTopRight.Size = new System.Drawing.Size(48, 48);
            this.ColorTopRight.TabIndex = 4;
            this.ColorTopRight.BackColorChanged += new System.EventHandler(this.colorTopRight_BackColorChanged);
            // 
            // ColorTopLeft
            // 
            this.ColorTopLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(195)))), ((int)(((byte)(168)))));
            this.ColorTopLeft.Location = new System.Drawing.Point(0, 0);
            this.ColorTopLeft.Name = "ColorTopLeft";
            this.ColorTopLeft.Size = new System.Drawing.Size(48, 48);
            this.ColorTopLeft.TabIndex = 3;
            this.ColorTopLeft.BackColorChanged += new System.EventHandler(this.ColorTopLeft_BackColorChanged);
            // 
            // GradientWallpaperGenerator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(747, 573);
            this.Controls.Add(this.ButtonSave);
            this.Controls.Add(this.ButtonFillRandom);
            this.Controls.Add(this.ButtonSetAsWallpaper);
            this.Controls.Add(this.ColorBottomRight);
            this.Controls.Add(this.ColorBottomLeft);
            this.Controls.Add(this.ColorTopRight);
            this.Controls.Add(this.ColorTopLeft);
            this.Controls.Add(this.drawWindow1);
            this.Name = "GradientWallpaperGenerator";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Random gradient wallpaper generator";
            this.ResumeLayout(false);

		}

		#endregion

		private GuiLabs.Canvas.DrawWindow drawWindow1;
		private ColorChooser ColorTopLeft;
		private ColorChooser ColorTopRight;
		private ColorChooser ColorBottomLeft;
		private ColorChooser ColorBottomRight;
		private System.Windows.Forms.Button ButtonSetAsWallpaper;
		private System.Windows.Forms.Button ButtonFillRandom;
		private System.Windows.Forms.Button ButtonSave;
	}
}