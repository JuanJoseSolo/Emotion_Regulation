using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EmotionRegulation;

namespace EmotionalRegulationWF
{
    public partial class Personality : Form
    {
        public Personality()
        {
            InitializeComponent();
        }

        private void AddPers_Click(object sender, EventArgs e)
        {
            float Cons = ConsiBar.Value;
            float Extr = ExtrBar.Value;
            var   Emo  = new Strategies();
            var   a    = Emo.Prueba(Cons, Extr);

            SitSeleValueLabel.Text = a.ToString();
        }

        private void ConsiBar_Scroll(object sender, EventArgs e)
        {
            float Cons = ConsiBar.Value;
            ConsiLabel.Text = Cons.ToString();
        }

        private void ExtrBar_Scroll(object sender, EventArgs e)
        {
            float Ext = ExtrBar.Value;
            ExtLabel.Text = Ext.ToString();
        }
    }
}
