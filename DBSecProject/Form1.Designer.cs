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
            this.txQuery = new System.Windows.Forms.TextBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.tab = new System.Windows.Forms.TabControl();
            this.tbData = new System.Windows.Forms.TabPage();
            this.grid = new System.Windows.Forms.DataGridView();
            this.tbResults = new System.Windows.Forms.TabPage();
            this.txResults = new System.Windows.Forms.TextBox();
            this.tab.SuspendLayout();
            this.tbData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.tbResults.SuspendLayout();
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
            this.btnChangeUser.Location = new System.Drawing.Point(349, 14);
            this.btnChangeUser.Name = "btnChangeUser";
            this.btnChangeUser.Size = new System.Drawing.Size(139, 30);
            this.btnChangeUser.TabIndex = 3;
            this.btnChangeUser.Text = "Change User";
            this.btnChangeUser.UseVisualStyleBackColor = true;
            this.btnChangeUser.Click += new System.EventHandler(this.btnChangeUser_Click);
            // 
            // txQuery
            // 
            this.txQuery.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txQuery.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txQuery.Location = new System.Drawing.Point(12, 58);
            this.txQuery.Multiline = true;
            this.txQuery.Name = "txQuery";
            this.txQuery.Size = new System.Drawing.Size(708, 63);
            this.txQuery.TabIndex = 2;
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
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // tab
            // 
            this.tab.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tab.Controls.Add(this.tbData);
            this.tab.Controls.Add(this.tbResults);
            this.tab.Location = new System.Drawing.Point(15, 127);
            this.tab.Name = "tab";
            this.tab.SelectedIndex = 0;
            this.tab.Size = new System.Drawing.Size(786, 486);
            this.tab.TabIndex = 5;
            // 
            // tbData
            // 
            this.tbData.Controls.Add(this.grid);
            this.tbData.Location = new System.Drawing.Point(4, 25);
            this.tbData.Name = "tbData";
            this.tbData.Padding = new System.Windows.Forms.Padding(3);
            this.tbData.Size = new System.Drawing.Size(778, 457);
            this.tbData.TabIndex = 0;
            this.tbData.Text = "Data";
            this.tbData.UseVisualStyleBackColor = true;
            // 
            // grid
            // 
            this.grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grid.Location = new System.Drawing.Point(6, 6);
            this.grid.Name = "grid";
            this.grid.RowTemplate.Height = 24;
            this.grid.Size = new System.Drawing.Size(766, 445);
            this.grid.TabIndex = 5;
            // 
            // tbResults
            // 
            this.tbResults.Controls.Add(this.txResults);
            this.tbResults.Location = new System.Drawing.Point(4, 25);
            this.tbResults.Name = "tbResults";
            this.tbResults.Padding = new System.Windows.Forms.Padding(3);
            this.tbResults.Size = new System.Drawing.Size(778, 457);
            this.tbResults.TabIndex = 1;
            this.tbResults.Text = "Results";
            this.tbResults.UseVisualStyleBackColor = true;
            // 
            // txResults
            // 
            this.txResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txResults.Location = new System.Drawing.Point(6, 6);
            this.txResults.Multiline = true;
            this.txResults.Name = "txResults";
            this.txResults.Size = new System.Drawing.Size(766, 445);
            this.txResults.TabIndex = 0;
            // 
            // Form1
            // 
            this.AcceptButton = this.btnExecute;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(813, 625);
            this.Controls.Add(this.tab);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.txQuery);
            this.Controls.Add(this.btnChangeUser);
            this.Controls.Add(this.lblLoggedInAs);
            this.Name = "Form1";
            this.Text = "Secure Database";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.tab.ResumeLayout(false);
            this.tbData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.tbResults.ResumeLayout(false);
            this.tbResults.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblLoggedInAs;
        private System.Windows.Forms.Button btnChangeUser;
        private System.Windows.Forms.TextBox txQuery;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.TabControl tab;
        private System.Windows.Forms.TabPage tbData;
        private System.Windows.Forms.DataGridView grid;
        private System.Windows.Forms.TabPage tbResults;
        private System.Windows.Forms.TextBox txResults;
    }
}

