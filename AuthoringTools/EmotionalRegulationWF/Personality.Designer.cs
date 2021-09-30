
namespace EmotionalRegulationWF
{
    partial class Personality
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
            this.Consientioness = new System.Windows.Forms.Label();
            this.ExtrBar = new System.Windows.Forms.TrackBar();
            this.Extraversion = new System.Windows.Forms.Label();
            this.AddPers = new System.Windows.Forms.Button();
            this.SitSeleLabel = new System.Windows.Forms.Label();
            this.SitSeleValueLabel = new System.Windows.Forms.Label();
            this.ExtLabel = new System.Windows.Forms.Label();
            this.ConsiLabel = new System.Windows.Forms.Label();
            this.ConsiBar = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.ExtrBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ConsiBar)).BeginInit();
            this.SuspendLayout();
            // 
            // Consientioness
            // 
            this.Consientioness.AutoSize = true;
            this.Consientioness.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Consientioness.Location = new System.Drawing.Point(284, 57);
            this.Consientioness.Name = "Consientioness";
            this.Consientioness.Size = new System.Drawing.Size(160, 25);
            this.Consientioness.TabIndex = 1;
            this.Consientioness.Text = "Consientioness";
            this.Consientioness.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ExtrBar
            // 
            this.ExtrBar.AccessibleName = "ExtrBar";
            this.ExtrBar.Location = new System.Drawing.Point(214, 257);
            this.ExtrBar.Maximum = 100;
            this.ExtrBar.Name = "ExtrBar";
            this.ExtrBar.Size = new System.Drawing.Size(309, 45);
            this.ExtrBar.TabIndex = 12;
            this.ExtrBar.Scroll += new System.EventHandler(this.ExtrBar_Scroll);
            // 
            // Extraversion
            // 
            this.Extraversion.AutoSize = true;
            this.Extraversion.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Extraversion.Location = new System.Drawing.Point(312, 210);
            this.Extraversion.Name = "Extraversion";
            this.Extraversion.Size = new System.Drawing.Size(132, 25);
            this.Extraversion.TabIndex = 14;
            this.Extraversion.Text = "Extraversion";
            this.Extraversion.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // AddPers
            // 
            this.AddPers.AccessibleName = "Add01Button";
            this.AddPers.Location = new System.Drawing.Point(346, 385);
            this.AddPers.Name = "AddPers";
            this.AddPers.Size = new System.Drawing.Size(75, 23);
            this.AddPers.TabIndex = 17;
            this.AddPers.Text = "Add";
            this.AddPers.UseVisualStyleBackColor = true;
            this.AddPers.Click += new System.EventHandler(this.AddPers_Click);
            // 
            // SitSeleLabel
            // 
            this.SitSeleLabel.AccessibleName = "";
            this.SitSeleLabel.AutoSize = true;
            this.SitSeleLabel.Font = new System.Drawing.Font("Akhbar MT", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.SitSeleLabel.Location = new System.Drawing.Point(12, 354);
            this.SitSeleLabel.Name = "SitSeleLabel";
            this.SitSeleLabel.Size = new System.Drawing.Size(210, 33);
            this.SitSeleLabel.TabIndex = 16;
            this.SitSeleLabel.Text = "Situation Selection";
            // 
            // SitSeleValueLabel
            // 
            this.SitSeleValueLabel.AccessibleName = "ValLabel";
            this.SitSeleValueLabel.AutoSize = true;
            this.SitSeleValueLabel.Font = new System.Drawing.Font("MS UI Gothic", 15.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SitSeleValueLabel.Location = new System.Drawing.Point(86, 387);
            this.SitSeleValueLabel.Name = "SitSeleValueLabel";
            this.SitSeleValueLabel.Size = new System.Drawing.Size(59, 21);
            this.SitSeleValueLabel.TabIndex = 18;
            this.SitSeleValueLabel.Text = "Value";
            // 
            // ExtLabel
            // 
            this.ExtLabel.AccessibleName = "ExtraLabel";
            this.ExtLabel.AutoSize = true;
            this.ExtLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExtLabel.Location = new System.Drawing.Point(539, 257);
            this.ExtLabel.Name = "ExtLabel";
            this.ExtLabel.Size = new System.Drawing.Size(19, 20);
            this.ExtLabel.TabIndex = 20;
            this.ExtLabel.Text = "0";
            // 
            // ConsiLabel
            // 
            this.ConsiLabel.AccessibleName = "ConsLabel";
            this.ConsiLabel.AutoSize = true;
            this.ConsiLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ConsiLabel.Location = new System.Drawing.Point(539, 106);
            this.ConsiLabel.Name = "ConsiLabel";
            this.ConsiLabel.Size = new System.Drawing.Size(19, 20);
            this.ConsiLabel.TabIndex = 19;
            this.ConsiLabel.Text = "0";
            // 
            // ConsiBar
            // 
            this.ConsiBar.AccessibleName = "ConsiBar";
            this.ConsiBar.Location = new System.Drawing.Point(214, 112);
            this.ConsiBar.Maximum = 100;
            this.ConsiBar.Name = "ConsiBar";
            this.ConsiBar.Size = new System.Drawing.Size(309, 45);
            this.ConsiBar.TabIndex = 13;
            this.ConsiBar.Scroll += new System.EventHandler(this.ConsiBar_Scroll);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ExtLabel);
            this.Controls.Add(this.ConsiLabel);
            this.Controls.Add(this.SitSeleValueLabel);
            this.Controls.Add(this.AddPers);
            this.Controls.Add(this.SitSeleLabel);
            this.Controls.Add(this.Extraversion);
            this.Controls.Add(this.ConsiBar);
            this.Controls.Add(this.ExtrBar);
            this.Controls.Add(this.Consientioness);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.ExtrBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ConsiBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Consientioness;
        private System.Windows.Forms.TrackBar ExtrBar;
        private System.Windows.Forms.Label Extraversion;
        private System.Windows.Forms.Button AddPers;
        private System.Windows.Forms.Label SitSeleLabel;
        private System.Windows.Forms.Label SitSeleValueLabel;
        private System.Windows.Forms.Label ExtLabel;
        private System.Windows.Forms.Label ConsiLabel;
        private System.Windows.Forms.TrackBar ConsiBar;
    }
}

