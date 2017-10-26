namespace GuiLabs.Wallpaper
{
	partial class Benchmark
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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.drawWindow1 = new GuiLabs.Canvas.DrawWindow();
			this.drawWindow2 = new GuiLabs.Canvas.DrawWindow();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.drawWindow1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.drawWindow2);
			this.splitContainer1.Size = new System.Drawing.Size(628, 483);
			this.splitContainer1.SplitterDistance = 314;
			this.splitContainer1.TabIndex = 0;
			// 
			// drawWindow1
			// 
			this.drawWindow1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.drawWindow1.Location = new System.Drawing.Point(0, 0);
			this.drawWindow1.Name = "drawWindow1";
			this.drawWindow1.ShouldRedrawOnWindowPaint = true;
			this.drawWindow1.Size = new System.Drawing.Size(314, 483);
			this.drawWindow1.TabIndex = 2;
			this.drawWindow1.Click += new System.EventHandler(this.drawWindow1_Click);
			this.drawWindow1.Repaint += new GuiLabs.Canvas.Events.RepaintHandler(this.drawWindow1_Repaint);
			// 
			// drawWindow2
			// 
			this.drawWindow2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.drawWindow2.Location = new System.Drawing.Point(0, 0);
			this.drawWindow2.Name = "drawWindow2";
			this.drawWindow2.ShouldRedrawOnWindowPaint = false;
			this.drawWindow2.Size = new System.Drawing.Size(310, 483);
			this.drawWindow2.TabIndex = 4;
			this.drawWindow2.Click += new System.EventHandler(this.drawWindow2_Click);
			this.drawWindow2.Paint += new System.Windows.Forms.PaintEventHandler(this.drawWindow2_Paint);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(628, 483);
			this.Controls.Add(this.splitContainer1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.Resize += new System.EventHandler(this.Form1_Resize);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private GuiLabs.Canvas.DrawWindow drawWindow1;
		private GuiLabs.Canvas.DrawWindow drawWindow2;

	}
}

