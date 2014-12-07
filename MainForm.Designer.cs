namespace gribov.pro
{
    partial class MainForm
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
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbPeriod = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbCount = new System.Windows.Forms.ComboBox();
            this.cbEmergency = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 132);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Load data";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(252, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(137, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Время для загрузки (сек)";
            // 
            // cbPeriod
            // 
            this.cbPeriod.FormattingEnabled = true;
            this.cbPeriod.Items.AddRange(new object[] {
            "0,5",
            "1",
            "1,5",
            "2"});
            this.cbPeriod.Location = new System.Drawing.Point(15, 25);
            this.cbPeriod.Name = "cbPeriod";
            this.cbPeriod.Size = new System.Drawing.Size(121, 21);
            this.cbPeriod.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(197, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Минимальное количество элементов";
            // 
            // cbCount
            // 
            this.cbCount.FormattingEnabled = true;
            this.cbCount.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4"});
            this.cbCount.Location = new System.Drawing.Point(15, 65);
            this.cbCount.Name = "cbCount";
            this.cbCount.Size = new System.Drawing.Size(121, 21);
            this.cbCount.TabIndex = 5;
            // 
            // cbEmergency
            // 
            this.cbEmergency.FormattingEnabled = true;
            this.cbEmergency.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3"});
            this.cbEmergency.Location = new System.Drawing.Point(15, 105);
            this.cbEmergency.Name = "cbEmergency";
            this.cbEmergency.Size = new System.Drawing.Size(121, 21);
            this.cbEmergency.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 89);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(208, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Дополнительное время ожидания (сек)";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(517, 169);
            this.Controls.Add(this.cbEmergency);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbCount);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbPeriod);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Name = "MainForm";
            this.Text = "Асинхронная загрузка";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbPeriod;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbCount;
        private System.Windows.Forms.ComboBox cbEmergency;
        private System.Windows.Forms.Label label4;
    }
}

