namespace ConnectedComponents
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
            this.Drawing = new System.Windows.Forms.PictureBox();
            this.FindComponents = new System.Windows.Forms.Button();
            this.PercentageSlider = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.FieldSizeSlider = new System.Windows.Forms.TrackBar();
            this.Status = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Drawing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PercentageSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FieldSizeSlider)).BeginInit();
            this.SuspendLayout();
            // 
            // Drawing
            // 
            this.Drawing.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Drawing.Location = new System.Drawing.Point(12, 12);
            this.Drawing.Name = "Drawing";
            this.Drawing.Size = new System.Drawing.Size(485, 405);
            this.Drawing.TabIndex = 0;
            this.Drawing.TabStop = false;
            this.Drawing.Paint += new System.Windows.Forms.PaintEventHandler(this.Drawing_Paint);
            // 
            // FindComponents
            // 
            this.FindComponents.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.FindComponents.Location = new System.Drawing.Point(358, 538);
            this.FindComponents.Name = "FindComponents";
            this.FindComponents.Size = new System.Drawing.Size(139, 37);
            this.FindComponents.TabIndex = 2;
            this.FindComponents.Text = "Find components";
            this.FindComponents.UseVisualStyleBackColor = true;
            this.FindComponents.Click += new System.EventHandler(this.FindComponents_Click);
            // 
            // PercentageSlider
            // 
            this.PercentageSlider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.PercentageSlider.LargeChange = 2;
            this.PercentageSlider.Location = new System.Drawing.Point(176, 423);
            this.PercentageSlider.Maximum = 40;
            this.PercentageSlider.Name = "PercentageSlider";
            this.PercentageSlider.Size = new System.Drawing.Size(321, 53);
            this.PercentageSlider.TabIndex = 3;
            this.PercentageSlider.Value = 20;
            this.PercentageSlider.Scroll += new System.EventHandler(this.PercentageSlider_Scroll);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 434);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "White percentage:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 470);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 17);
            this.label2.TabIndex = 6;
            this.label2.Text = "Field size:";
            // 
            // FieldSizeSlider
            // 
            this.FieldSizeSlider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.FieldSizeSlider.LargeChange = 2;
            this.FieldSizeSlider.Location = new System.Drawing.Point(176, 470);
            this.FieldSizeSlider.Maximum = 100;
            this.FieldSizeSlider.Minimum = 1;
            this.FieldSizeSlider.Name = "FieldSizeSlider";
            this.FieldSizeSlider.Size = new System.Drawing.Size(321, 53);
            this.FieldSizeSlider.TabIndex = 5;
            this.FieldSizeSlider.Value = 5;
            this.FieldSizeSlider.Scroll += new System.EventHandler(this.FieldSizeSlider_Scroll);
            // 
            // Status
            // 
            this.Status.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Status.AutoSize = true;
            this.Status.Location = new System.Drawing.Point(15, 538);
            this.Status.Name = "Status";
            this.Status.Size = new System.Drawing.Size(0, 17);
            this.Status.TabIndex = 7;
            // 
            // Form1
            // 
            this.AcceptButton = this.FindComponents;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(509, 587);
            this.Controls.Add(this.Status);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.FieldSizeSlider);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PercentageSlider);
            this.Controls.Add(this.FindComponents);
            this.Controls.Add(this.Drawing);
            this.DoubleBuffered = true;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Find connected components";
            this.Resize += new System.EventHandler(this.Form1_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.Drawing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PercentageSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FieldSizeSlider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox Drawing;
        private System.Windows.Forms.Button FindComponents;
        private System.Windows.Forms.TrackBar PercentageSlider;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar FieldSizeSlider;
        private System.Windows.Forms.Label Status;
    }
}

