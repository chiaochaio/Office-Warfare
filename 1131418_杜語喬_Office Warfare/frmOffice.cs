using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _1131418_杜語喬_Office_Warfare
{
    public partial class frmOffice : Form
    {
        public frmOffice()
        {
            InitializeComponent();
        }

        private void btnExplain_Click(object sender, EventArgs e)
        {
            string rules = "【公司職級之戰 - 遊戲規則】\n\n" +
                   "1. 翻牌決定陣營 (紅/藍)。\n" +
                   "2. 每回合可翻牌或移動一格。\n" +
                   "3. 職級 7 > 6 > 5 > 4 > 3 > 2 > 1。\n" +
                   "4. 特殊：等級 1 (實習生) 可吃 等級 7 (董事長)。\n" +
                   "5. 吃光對方所有棋子即獲勝！";
            MessageBox.Show(rules, "入職指南");
        }
    }
}
