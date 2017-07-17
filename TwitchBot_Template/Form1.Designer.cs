namespace TwitchBot_Template
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
            this.chatBox = new System.Windows.Forms.RichTextBox();
            this.twitchPicBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.twitchPicBox)).BeginInit();
            this.SuspendLayout();
            // 
            // chatBox
            // 
            this.chatBox.Location = new System.Drawing.Point(12, 196);
            this.chatBox.Name = "chatBox";
            this.chatBox.Size = new System.Drawing.Size(365, 413);
            this.chatBox.TabIndex = 0;
            this.chatBox.Text = "";
            // 
            // twitchPicBox
            // 
            this.twitchPicBox.Image = global::TwitchBot_Template.Properties.Resources.Untitled_design__4_;
            this.twitchPicBox.Location = new System.Drawing.Point(12, 12);
            this.twitchPicBox.Name = "twitchPicBox";
            this.twitchPicBox.Size = new System.Drawing.Size(365, 178);
            this.twitchPicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.twitchPicBox.TabIndex = 1;
            this.twitchPicBox.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(388, 620);
            this.Controls.Add(this.twitchPicBox);
            this.Controls.Add(this.chatBox);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.twitchPicBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox chatBox;
        private System.Windows.Forms.PictureBox twitchPicBox;
    }
}

