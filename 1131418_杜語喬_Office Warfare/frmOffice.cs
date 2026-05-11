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
        // ========== 全域變數定義 ==========
        // 棋盤資料陣列：正數(1-7)代表紅方，負數(-1~-7)代表藍方
        private int[] boardData = new int[16];
        
        // 翻牌狀態追蹤：記錄每張牌是否已被翻開
        private bool[] isFlipped = new bool[16];
        
        // PictureBox 控制項陣列：對應介面上的 16 個棋格
        private PictureBox[] pbTiles = new PictureBox[16];
        
        // 隨機數產生器
        private Random random = new Random();

        public frmOffice()
        {
            InitializeComponent();
        }

        private void frmOffice_Load(object sender, EventArgs e)
        {
            // 將介面上的 PictureBox 控制項加入陣列
            pbTiles[0] = pictureBox1;
            pbTiles[1] = pictureBox2;
            pbTiles[2] = pictureBox3;
            pbTiles[3] = pictureBox4;
            pbTiles[4] = pictureBox5;
            pbTiles[5] = pictureBox6;
            pbTiles[6] = pictureBox7;
            pbTiles[7] = pictureBox8;
            pbTiles[8] = pictureBox9;
            pbTiles[9] = pictureBox10;
            pbTiles[10] = pictureBox11;
            pbTiles[11] = pictureBox12;
            pbTiles[12] = pictureBox13;
            pbTiles[13] = pictureBox14;
            pbTiles[14] = pictureBox15;
            pbTiles[15] = pictureBox16;

            // 執行洗牌初始化
            ShuffleBoard();
        }

        /// <summary>
        /// 洗牌方法：初始化棋盤資料並隨機打亂排列
        /// </summary>
        private void ShuffleBoard()
        {
            // 初始化棋盤資料：紅方 1~7（各1枚），藍方 -1~-7（各1枚），額外加上 1 和 -1（各1枚）
            List<int> tiles = new List<int>();
            
            // 加入紅方棋子（1~7 各一枚）
            for (int i = 1; i <= 7; i++)
                tiles.Add(i);
            
            // 加入藍方棋子（-1~-7 各一枚）
            for (int i = 1; i <= 7; i++)
                tiles.Add(-i);

            // 使用 Fisher-Yates 演算法隨機打亂棋盤資料
            for (int i = tiles.Count - 1; i > 0; i--)
            {
                int randomIndex = random.Next(i + 1);
                
                // 交換元素
                int temp = tiles[i];
                tiles[i] = tiles[randomIndex];
                tiles[randomIndex] = temp;
            }

            // 將打亂後的棋子放入 boardData
            for (int i = 0; i < 16; i++)
                boardData[i] = tiles[i];

            // 重置翻牌狀態為全部未翻
            for (int i = 0; i < 16; i++)
                isFlipped[i] = false;

            // 配置 PictureBox：設定背面圖、Tag 值與點擊事件
            for (int i = 0; i < 16; i++)
            {
                // 設定背面圖片
                pbTiles[i].Image = Properties.Resources.Tile_Back;
                
                // 設定 Tag 為索引值，方便點擊時識別
                pbTiles[i].Tag = i;
                
                // 移除重複的事件註冊，然後添加點擊事件
                pbTiles[i].Click -= PbTile_Click;
                pbTiles[i].Click += PbTile_Click;
            }
        }

        /// <summary>
        /// PictureBox 點擊事件處理（後續實作）
        /// </summary>
        private void PbTile_Click(object sender, EventArgs e)
        {
            // 此處後續實作翻牌邏輯
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
