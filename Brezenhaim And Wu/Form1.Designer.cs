namespace Brezenhaim_And_Wu
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.DrawButton1 = new System.Windows.Forms.Button();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.DrawButton2 = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(283, 77);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(753, 433);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // DrawButton1
            // 
            this.DrawButton1.Location = new System.Drawing.Point(354, 22);
            this.DrawButton1.Name = "DrawButton1";
            this.DrawButton1.Size = new System.Drawing.Size(203, 34);
            this.DrawButton1.TabIndex = 1;
            this.DrawButton1.Text = "Брезенхам";
            this.DrawButton1.UseVisualStyleBackColor = true;
            this.DrawButton1.Click += new System.EventHandler(this.DrawButton1_Click);
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(527, 525);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(303, 45);
            this.trackBar1.TabIndex = 2;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // DrawButton2
            // 
            this.DrawButton2.Location = new System.Drawing.Point(758, 22);
            this.DrawButton2.Name = "DrawButton2";
            this.DrawButton2.Size = new System.Drawing.Size(203, 34);
            this.DrawButton2.TabIndex = 3;
            this.DrawButton2.Text = "Ву";
            this.DrawButton2.UseVisualStyleBackColor = true;
            this.DrawButton2.Click += new System.EventHandler(this.DrawButton2_Click);
            // 
            // clearButton
            // 
            this.clearButton.Location = new System.Drawing.Point(625, 22);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(75, 23);
            this.clearButton.TabIndex = 4;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(1270, 595);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.DrawButton2);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.DrawButton1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button DrawButton1;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Button DrawButton2;
        private System.Windows.Forms.Button clearButton;
    }
}

