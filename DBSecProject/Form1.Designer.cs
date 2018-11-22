namespace DBSecProject
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
            this.lblLoggedInAs = new System.Windows.Forms.Label();
            this.btnChangeUser = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.tab = new System.Windows.Forms.TabControl();
            this.tbData = new System.Windows.Forms.TabPage();
            this.tbResults = new System.Windows.Forms.TabPage();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.tab.SuspendLayout();
            this.tbData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblLoggedInAs
            // 
            this.lblLoggedInAs.AutoSize = true;
            this.lblLoggedInAs.Location = new System.Drawing.Point(12, 21);
            this.lblLoggedInAs.Name = "lblLoggedInAs";
            this.lblLoggedInAs.Size = new System.Drawing.Size(98, 17);
            this.lblLoggedInAs.TabIndex = 0;
            this.lblLoggedInAs.Text = "Logged in as: ";
            this.lblLoggedInAs.Click += new System.EventHandler(this.label1_Click);
            // 
            // btnChangeUser
            // 
            this.btnChangeUser.Location = new System.Drawing.Point(191, 16);
            this.btnChangeUser.Name = "btnChangeUser";
            this.btnChangeUser.Size = new System.Drawing.Size(139, 30);
            this.btnChangeUser.TabIndex = 3;
            this.btnChangeUser.Text = "Change User";
            this.btnChangeUser.UseVisualStyleBackColor = true;
            this.btnChangeUser.Click += new System.EventHandler(this.btnChangeUser_Click);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(12, 58);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(708, 63);
            this.textBox1.TabIndex = 2;
            // 
            // btnExecute
            // 
            this.btnExecute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExecute.Location = new System.Drawing.Point(726, 58);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(75, 63);
            this.btnExecute.TabIndex = 1;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            // 
            // tab
            // 
            this.tab.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tab.Controls.Add(this.tbData);
            this.tab.Controls.Add(this.tbResults);
            this.tab.Location = new System.Drawing.Point(15, 142);
            this.tab.Name = "tab";
            this.tab.SelectedIndex = 0;
            this.tab.Size = new System.Drawing.Size(786, 471);
            this.tab.TabIndex = 5;
            // 
            // tbData
            // 
            this.tbData.Controls.Add(this.dataGridView1);
            this.tbData.Location = new System.Drawing.Point(4, 25);
            this.tbData.Name = "tbData";
            this.tbData.Padding = new System.Windows.Forms.Padding(3);
            this.tbData.Size = new System.Drawing.Size(778, 442);
            this.tbData.TabIndex = 0;
            this.tbData.Text = "Data";
            this.tbData.UseVisualStyleBackColor = true;
            // 
            // tbResults
            // 
            this.tbResults.Location = new System.Drawing.Point(4, 25);
            this.tbResults.Name = "tbResults";
            this.tbResults.Padding = new System.Windows.Forms.Padding(3);
            this.tbResults.Size = new System.Drawing.Size(778, 442);
            this.tbResults.TabIndex = 1;
            this.tbResults.Text = "Results";
            this.tbResults.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(6, 6);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(766, 430);
            this.dataGridView1.TabIndex = 5;
            // 
            // Form1
            // 
            this.AcceptButton = this.btnExecute;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(813, 625);
            this.Controls.Add(this.tab);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnChangeUser);
            this.Controls.Add(this.lblLoggedInAs);
            this.Name = "Form1";
            this.Text = "Secure Database";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.tab.ResumeLayout(false);
            this.tbData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblLoggedInAs;
        private System.Windows.Forms.Button btnChangeUser;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.TabControl tab;
        private System.Windows.Forms.TabPage tbData;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TabPage tbResults;
    }
}

