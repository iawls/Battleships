namespace Battleships
{
    partial class GameScreen
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
            this.components = new System.ComponentModel.Container();
            this.paint_timer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // paint_timer
            // 
            this.paint_timer.Enabled = true;
            this.paint_timer.Interval = 16;
            this.paint_timer.Tick += new System.EventHandler(this.paint_timer_Tick);
            // 
            // GameScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.ClientSize = new System.Drawing.Size(763, 528);
            this.Name = "GameScreen";
            this.Text = "GameScreen";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer paint_timer;
    }
}