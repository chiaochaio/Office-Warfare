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

        // 當前玩家：1 代表紅方，2 代表藍方，預設由紅方開始翻牌
        private int currentPlayer = 1;

        // 玩家 1 所屬陣營：0 = 未定, 1 = 紅, -1 = 藍
        private int p1Side = 0;

        public frmOffice()
        {
            InitializeComponent();
        }

        private void frmOffice_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("========== Form_Load 開始 ==========");
            
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

            // ========== 綁定所有 PictureBox 的點擊事件 ==========
            for (int i = 0; i < 16; i++)
            {
                pbTiles[i].Cursor = Cursors.Hand;
                pbTiles[i].Tag = i;
                
                // 綁定事件處理程式（使用 += 而非委派）
                pbTiles[i].MouseDown += PbTile_MouseDown;
                
                System.Diagnostics.Debug.WriteLine($"已綁定 pbTiles[{i}]");
            }
            
            // 將按鈕設置到最前面，確保不會被 PictureBox 蓋住
            btnExplain.BringToFront();
            
            // 初始化玩家狀態顯示
            UpdatePlayerStatus();
            System.Diagnostics.Debug.WriteLine("========== Form_Load 完成 ==========\n");
        }

        /// <summary>
        /// 洗牌方法：初始化棋盤資料並隨機打亂排列
        /// </summary>
        private void ShuffleBoard()
        {
            // 初始化棋盤資料：紅方 1~7（各1枚，1額外多1枚），藍方 -1~-7（各1枚，-1額外多1枚）
            List<int> tiles = new List<int>();
            
            for (int i = 1; i <= 7; i++)
                tiles.Add(i);
            
            tiles.Add(1); // 額外的紅方實習生
            
            for (int i = 1; i <= 7; i++)
                tiles.Add(-i);
            
            tiles.Add(-1); // 額外的藍方實習生

            // Fisher-Yates 打亂
            for (int i = tiles.Count - 1; i > 0; i--)
            {
                int randomIndex = random.Next(i + 1);
                int temp = tiles[i];
                tiles[i] = tiles[randomIndex];
                tiles[randomIndex] = temp;
            }

            for (int i = 0; i < 16; i++)
                boardData[i] = tiles[i];

            for (int i = 0; i < 16; i++)
                isFlipped[i] = false;

            for (int i = 0; i < 16; i++)
                pbTiles[i].Image = Properties.Resources.Tile_Back;
            
            System.Diagnostics.Debug.WriteLine("洗牌完成");
        }

        /// <summary>
        /// PictureBox 滑鼠按下事件處理
        /// </summary>
        private void PbTile_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox tile = (PictureBox)sender;
            int index = (int)tile.Tag;
            
            System.Diagnostics.Debug.WriteLine($"\n點擊 PictureBox[{index}]");
            System.Diagnostics.Debug.WriteLine($"  - 已翻: {isFlipped[index]}");
            System.Diagnostics.Debug.WriteLine($"  - 棋子值: {boardData[index]}");

            if (isFlipped[index])
            {
                System.Diagnostics.Debug.WriteLine($"  - 跳過（已翻過）");
                return;
            }

            isFlipped[index] = true;

            if (p1Side == 0)
            {
                p1Side = boardData[index] > 0 ? 1 : -1;
                System.Diagnostics.Debug.WriteLine($"  - 設定玩家1陣營: {(p1Side == 1 ? "紅" : "藍")}");
            }

            LoadTileImage(index);
            
            currentPlayer = (currentPlayer == 1) ? 2 : 1;
            UpdatePlayerStatus();
            
            System.Diagnostics.Debug.WriteLine($"  - 現在輪到玩家 {currentPlayer}");
        }

        /// <summary>
        /// 根據棋子數值載入對應的圖片
        /// </summary>
        private void LoadTileImage(int index)
        {
            int tileValue = boardData[index];
            Image tileImage = null;

            if (tileValue > 0)
            {
                switch (tileValue)
                {
                    case 1:
                        tileImage = Properties.Resources.Red_Intern_1;
                        break;
                    case 2:
                        tileImage = Properties.Resources.Red_Junior_2;
                        break;
                    case 3:
                        tileImage = Properties.Resources.Red_Senior_3;
                        break;
                    case 4:
                        tileImage = Properties.Resources.Red_Leader_4;
                        break;
                    case 5:
                        tileImage = Properties.Resources.Red_Manager_5;
                        break;
                    case 6:
                        tileImage = Properties.Resources.Red_GM_6;
                        break;
                    case 7:
                        tileImage = Properties.Resources.Red_Boss_7;
                        break;
                }
            }
            else
            {
                switch (Math.Abs(tileValue))
                {
                    case 1:
                        tileImage = Properties.Resources.Blue_Intern_1;
                        break;
                    case 2:
                        tileImage = Properties.Resources.Blue_Junior_2;
                        break;
                    case 3:
                        tileImage = Properties.Resources.Blue_Senior_3;
                        break;
                    case 4:
                        tileImage = Properties.Resources.Blue_Leader_4;
                        break;
                    case 5:
                        tileImage = Properties.Resources.Blue_Manager_5;
                        break;
                    case 6:
                        tileImage = Properties.Resources.Blue_GM_6;
                        break;
                    case 7:
                        tileImage = Properties.Resources.Blue_Boss_7;
                        break;
                }
            }

            if (tileImage != null)
            {
                pbTiles[index].Image = tileImage;
                System.Diagnostics.Debug.WriteLine($"  - 已更新圖片");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"  - 警告：找不到圖片，值: {tileValue}");
            }
        }

        /// <summary>
        /// 更新玩家狀態顯示
        /// </summary>
        private void UpdatePlayerStatus()
        {
            string sideInfo = "";
            Color textColor = Color.Black;

            // 判斷目前玩家的陣營
            int currentPlayerSide = 0;
            if (currentPlayer == 1)
            {
                currentPlayerSide = p1Side;
            }
            else // currentPlayer == 2
            {
                currentPlayerSide = p1Side == 1 ? -1 : 1;
            }

            // 根據陣營設定顯示文字和顏色
            if (currentPlayerSide != 0)
            {
                if (currentPlayerSide == 1)
                {
                    sideInfo = "（紅方）";
                    textColor = Color.Red;
                }
                else
                {
                    sideInfo = "（藍方）";
                    textColor = Color.Blue;
                }
            }

            lblStatus.Text = $"玩家 {currentPlayer}{sideInfo}";
            lblStatus.ForeColor = textColor;
        }

        private void btnExplain_Click(object sender, EventArgs e)
        {
            string rules = "【公司職級之戰 - 雇用規則】\n\n" +
                   "1. 首先，翻開任意一張牌以決定您的陣營 (紅方/藍方)。\n" +
                   "2. 每回合，您可以翻開一張牌或是移動一格。\n" +
                   "3. 職級高者贏 (7 > 6 > 5 > 4 > 3 > 2 > 1)。\n" +
                   "4. 特殊規則：實習生 (1 級) 能夠吃掉董事長 (7 級)。\n" +
                   "5. 吃光對方所有的棋子便獲勝！";
            MessageBox.Show(rules, "雇用指南");
        }
    }
}
