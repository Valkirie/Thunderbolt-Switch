using System;
using System.Drawing;
using System.Windows.Forms;

namespace DockerForm
{
    public static class CustomMessageBox
    {
        public static DialogResult Show(string Text, string Title, eDialogButtons Buttons, Image Image)
        {
            MessageForm message = new MessageForm();
            message.Text = Title;

            if (Image != null)
                message.picImage.BackgroundImage = Image;
            message.picImage.BackgroundImageLayout = ImageLayout.Stretch;

            message.lblText.Text = Text;

            switch (Buttons)
            {
                case eDialogButtons.OK:
                    message.btnYes.Visible = false;
                    message.btnNo.Visible = false;
                    message.btnCancel.Visible = false;
                    message.btnOK.Location = message.btnCancel.Location;
                    break;
                case eDialogButtons.OKCancel:
                    message.btnYes.Visible = false;
                    message.btnNo.Visible = false;
                    break;
                case eDialogButtons.YesNo:
                    message.btnOK.Visible = false;
                    message.btnCancel.Visible = false;
                    message.btnYes.Location = message.btnOK.Location;
                    message.btnNo.Location = message.btnCancel.Location;
                    break;
                case eDialogButtons.YesNoCancel:
                    message.btnOK.Visible = false;
                    break;
            }

            if (message.lblText.Height > 64)
                message.Height = (message.lblText.Top + message.lblText.Height) + 78;

            return (message.ShowDialog());
        }

        partial class MessageForm
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
                this.picImage = new System.Windows.Forms.PictureBox();
                this.lblText = new System.Windows.Forms.Label();
                this.btnYes = new System.Windows.Forms.Button();
                this.btnNo = new System.Windows.Forms.Button();
                this.btnCancel = new System.Windows.Forms.Button();
                this.btnOK = new System.Windows.Forms.Button();
                ((System.ComponentModel.ISupportInitialize)(this.picImage)).BeginInit();
                this.SuspendLayout();
                //
                // picImage
                //
                this.picImage.ErrorImage = null;
                this.picImage.InitialImage = null;
                this.picImage.Location = new System.Drawing.Point(15, 15);
                this.picImage.Name = "picImage";
                this.picImage.Size = new System.Drawing.Size(64, 64);
                this.picImage.TabIndex = 0;
                this.picImage.TabStop = false;
                //
                // lblText
                //
                this.lblText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                            | System.Windows.Forms.AnchorStyles.Left)
                            | System.Windows.Forms.AnchorStyles.Right)));
                this.lblText.AutoSize = true;
                this.lblText.Location = new System.Drawing.Point(85, 15);
                this.lblText.MaximumSize = new System.Drawing.Size(294, 0);
                this.lblText.Name = "lblText";
                this.lblText.Size = new System.Drawing.Size(28, 13);
                this.lblText.TabIndex = 0;
                this.lblText.Text = "Text";
                //
                // btnYes
                //
                this.btnYes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
                this.btnYes.Location = new System.Drawing.Point(139, 88);
                this.btnYes.Name = "btnYes";
                this.btnYes.Size = new System.Drawing.Size(75, 23);
                this.btnYes.TabIndex = 2;
                this.btnYes.Text = "Yes";
                this.btnYes.UseVisualStyleBackColor = true;
                this.btnYes.Click += new System.EventHandler(this.btnYes_Click);
                //
                // btnNo
                //
                this.btnNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
                this.btnNo.Location = new System.Drawing.Point(220, 88);
                this.btnNo.Name = "btnNo";
                this.btnNo.Size = new System.Drawing.Size(75, 23);
                this.btnNo.TabIndex = 3;
                this.btnNo.Text = "No";
                this.btnNo.UseVisualStyleBackColor = true;
                this.btnNo.Click += new System.EventHandler(this.btnNo_Click);
                //
                // btnCancel
                //
                this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
                this.btnCancel.Location = new System.Drawing.Point(301, 88);
                this.btnCancel.Name = "btnCancel";
                this.btnCancel.Size = new System.Drawing.Size(75, 23);
                this.btnCancel.TabIndex = 1;
                this.btnCancel.Text = "Cancel";
                this.btnCancel.UseVisualStyleBackColor = true;
                this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
                //
                // btnOK
                //
                this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
                this.btnOK.Location = new System.Drawing.Point(220, 88);
                this.btnOK.Name = "btnOK";
                this.btnOK.Size = new System.Drawing.Size(75, 23);
                this.btnOK.TabIndex = 4;
                this.btnOK.Text = "OK";
                this.btnOK.UseVisualStyleBackColor = true;
                this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
                //
                // MessageForm
                //
                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.ClientSize = new System.Drawing.Size(394, 129);
                this.Controls.Add(this.btnYes);
                this.Controls.Add(this.btnNo);
                this.Controls.Add(this.btnCancel);
                this.Controls.Add(this.picImage);
                this.Controls.Add(this.lblText);
                this.Controls.Add(this.btnOK);
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.Name = "MessageForm";
                this.Padding = new System.Windows.Forms.Padding(15);
                this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                this.Text = "Title";
                ((System.ComponentModel.ISupportInitialize)(this.picImage)).EndInit();
                this.ResumeLayout(false);
                this.PerformLayout();

            }

            #endregion

            internal System.Windows.Forms.Button btnCancel;
            internal System.Windows.Forms.Button btnNo;
            internal System.Windows.Forms.Button btnYes;
            internal System.Windows.Forms.Button btnOK;
            internal System.Windows.Forms.PictureBox picImage;
            internal System.Windows.Forms.Label lblText;
        }

        internal partial class MessageForm : Form
        {
            internal MessageForm() => InitializeComponent();

            private void btnYes_Click(object sender, EventArgs e) =>
                DialogResult = DialogResult.Yes;

            private void btnNo_Click(object sender, EventArgs e) =>
                DialogResult = DialogResult.No;

            private void btnCancel_Click(object sender, EventArgs e) =>
                DialogResult = DialogResult.Cancel;

            private void btnOK_Click(object sender, EventArgs e) =>
                DialogResult = DialogResult.OK;
        }

        public enum eDialogButtons { OK, OKCancel, YesNo, YesNoCancel }
    }
}
