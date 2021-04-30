﻿namespace BiArcTutorial
{
    partial class Form
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
            this.NextShapeButton = new System.Windows.Forms.Button();
            this.CurveCount = new System.Windows.Forms.Label();
            this.NextArcButton = new System.Windows.Forms.Button();
            this.ErrorLevel = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.ErrorLevel)).BeginInit();
            this.SuspendLayout();
            // 
            // NextShapeButton
            // 
            this.NextShapeButton.Location = new System.Drawing.Point(13, 13);
            this.NextShapeButton.Name = "NextShapeButton";
            this.NextShapeButton.Size = new System.Drawing.Size(75, 23);
            this.NextShapeButton.TabIndex = 0;
            this.NextShapeButton.Text = "Next Shape";
            this.NextShapeButton.UseVisualStyleBackColor = true;
            this.NextShapeButton.Click += new System.EventHandler(this.NextShapeButton_Click);
            // 
            // CurveCount
            // 
            this.CurveCount.AutoSize = true;
            this.CurveCount.Location = new System.Drawing.Point(94, 18);
            this.CurveCount.Name = "CurveCount";
            this.CurveCount.Size = new System.Drawing.Size(44, 13);
            this.CurveCount.TabIndex = 1;
            this.CurveCount.Text = "Counter";
            // 
            // NextArcButton
            // 
            this.NextArcButton.Location = new System.Drawing.Point(13, 43);
            this.NextArcButton.Name = "NextArcButton";
            this.NextArcButton.Size = new System.Drawing.Size(75, 23);
            this.NextArcButton.TabIndex = 2;
            this.NextArcButton.Text = "Next Arc";
            this.NextArcButton.UseVisualStyleBackColor = true;
            this.NextArcButton.Click += new System.EventHandler(this.NextArcButton_Click);
            // 
            // ErrorLevel
            // 
            this.ErrorLevel.DecimalPlaces = 1;
            this.ErrorLevel.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.ErrorLevel.Location = new System.Drawing.Point(13, 73);
            this.ErrorLevel.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.ErrorLevel.Name = "ErrorLevel";
            this.ErrorLevel.Size = new System.Drawing.Size(75, 20);
            this.ErrorLevel.TabIndex = 3;
            this.ErrorLevel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ErrorLevel.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ErrorLevel.ValueChanged += new System.EventHandler(this.ErrorLevel_ValueChanged);
            // 
            // Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1065, 730);
            this.Controls.Add(this.ErrorLevel);
            this.Controls.Add(this.NextArcButton);
            this.Controls.Add(this.CurveCount);
            this.Controls.Add(this.NextShapeButton);
            this.Name = "Form";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.ErrorLevel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button NextShapeButton;
        private System.Windows.Forms.Label CurveCount;
        private System.Windows.Forms.Button NextArcButton;
        private System.Windows.Forms.NumericUpDown ErrorLevel;
    }
}

