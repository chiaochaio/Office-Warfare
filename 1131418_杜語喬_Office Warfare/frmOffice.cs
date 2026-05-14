using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

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

        // 紀錄目前選中棋子的索引，預設 -1
        private int selectedIndex = -1;

        // 音效播放器（全域，用於管理當前播放的音效）
        private SoundPlayer currentSoundPlayer = null;

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
            
            // 播放遊戲開局語音
            PlaySound(Properties.Resources.Sys_GameStart);
        }

        /// <summary>
        /// 播放音效的輔助方法（新音效優先，會中斷前一個音效）
        /// </summary>
        private void PlaySound(System.IO.Stream soundStream)
        {
            if (soundStream == null)
            {
                System.Diagnostics.Debug.WriteLine("警告：音效資源為空或不存在");
                return;
            }

            try
            {
                // 停止前一個音效
                if (currentSoundPlayer != null)
                {
                    try
                    {
                        currentSoundPlayer.Stop();
                    }
                    catch { }
                }

                // 在背景執行緒播放新音效
                System.Threading.Thread thread = new System.Threading.Thread(() =>
                {
                    try
                    {
                        SoundPlayer player = new SoundPlayer();
                        currentSoundPlayer = player; // 記錄當前播放器
                        player.Stream = soundStream;
                        player.PlaySync(); // 同步播放到完成
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"背景音效播放出錯：{ex.Message}");
                    }
                });
                thread.IsBackground = true;
                thread.Start();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"播放音效時出錯：{ex.Message}");
            }
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

            // ========== 翻牌邏輯 ==========
            if (!isFlipped[index])
            {
                isFlipped[index] = true;

                if (p1Side == 0)
                {
                    p1Side = boardData[index] > 0 ? 1 : -1;
                    System.Diagnostics.Debug.WriteLine($"  - 設定玩家1陣營: {(p1Side == 1 ? "紅" : "藍")}");
                }

                LoadTileImage(index);
                
                currentPlayer = (currentPlayer == 1) ? 2 : 1;
                UpdatePlayerStatus();
                
                // 播放回合切換語音
                PlayTurnSound();
                
                System.Diagnostics.Debug.WriteLine($"  - 現在輪到玩家 {currentPlayer}");
                return;
            }

            // ========== 選取棋子邏輯 ==========
            int currentPlayerSide = GetCurrentPlayerSide();

            // 若點擊已翻開的棋子且屬於當前玩家
            if (isFlipped[index] && boardData[index] != 0)
            {
                int tileOwner = boardData[index] > 0 ? 1 : -1;
                if (tileOwner == currentPlayerSide)
                {
                    selectedIndex = index;
                    System.Diagnostics.Debug.WriteLine($"  - 選中棋子 [{index}]");

                    // 清空所有背景圖片
                    for (int i = 0; i < 16; i++)
                        pbTiles[i].BackgroundImage = null;

                    // 高亮選中的棋子（疊加在棋子圖片上）
                    pbTiles[index].BackgroundImage = Properties.Resources.Tile_Selected;
                    pbTiles[index].BackgroundImageLayout = ImageLayout.Zoom;
                    return;
                }
            }

            // ========== 移動與吃子邏輯 ==========
            if (selectedIndex != -1 && IsAdjacent(selectedIndex, index))
            {
                System.Diagnostics.Debug.WriteLine($"  - 從 [{selectedIndex}] 移動到 [{index}]");

                int attacker = boardData[selectedIndex];
                int defender = boardData[index];

                bool actionSuccess = false;

                // 目標格為空 - 移動
                if (defender == 0)
                {
                    boardData[index] = attacker;
                    boardData[selectedIndex] = 0;
                    LoadTileImage(index);
                    pbTiles[selectedIndex].Image = Properties.Resources.Tile_Empty;
                    
                    System.Diagnostics.Debug.WriteLine($"  - 棋子移動到 [{index}]");

                    actionSuccess = true;
                }
                // 目標格有敵方棋子 - 吃子判定
                else if ((attacker > 0 && defender < 0) || (attacker < 0 && defender > 0))
                {
                    int levelA = Math.Abs(attacker);
                    int levelB = Math.Abs(defender);

                    bool attackSuccess = false;

                    // 規則 A：大吃小
                    if (levelA >= levelB)
                    {
                        attackSuccess = true;
                    }

                    // 規則 B：實習生逆襲（1吃7）
                    if (levelA == 1 && levelB == 7)
                    {
                        attackSuccess = true;
                    }

                    if (attackSuccess)
                    {
                        // 規則 C：同級消失
                        if (levelA == levelB)
                        {
                            System.Diagnostics.Debug.WriteLine($"  - 同級對戰！雙方同歸於盡");
                            boardData[selectedIndex] = 0;
                            boardData[index] = 0;
                            pbTiles[selectedIndex].Image = Properties.Resources.Tile_Empty;
                            pbTiles[index].Image = Properties.Resources.Tile_Empty;
                            // 播放平手音效
                            PlaySound(Properties.Resources.Voice_Eat_Tie);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"  - 攻擊成功！Lv{levelA} 吃掉 Lv{levelB}");
                            boardData[index] = attacker;
                            boardData[selectedIndex] = 0;
                            LoadTileImage(index);
                            pbTiles[selectedIndex].Image = Properties.Resources.Tile_Empty;
                            
                            // 實習生逆襲特殊音效
                            if (levelA == 1 && levelB == 7)
                            {
                                PlaySound(Properties.Resources.Voice_Eat_Counter);
                            }
                        }

                        actionSuccess = true;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"  - 攻擊失敗！Lv{levelA} 無法吃掉 Lv{levelB}");
                        // 攻擊失敗，不算有效回合
                    }
                }
                else
                {
                    // 目標格為自己的棋子，取消選擇
                    System.Diagnostics.Debug.WriteLine($"  - 目標是自己的棋子，取消選擇");
                }

                // 清空高亮
                ClearSelection();

                // 只在有效動作時才切換玩家
                if (actionSuccess)
                {
                    currentPlayer = (currentPlayer == 1) ? 2 : 1;
                    UpdatePlayerStatus();
                    
                    // 播放回合切換語音
                    PlayTurnSound();
                }
            }
            else if (selectedIndex != -1 && index != selectedIndex)
            {
                System.Diagnostics.Debug.WriteLine($"  - 目標不相鄰，取消選擇");
                ClearSelection();
            }

            // 檢查遊戲是否結束
            CheckGameOver();
        }

        /// <summary>
        /// 播放回合切換語音
        /// </summary>
        private void PlayTurnSound()
        {
            // 根據 lblStatus 的文字顏色來決定播放哪個語音
            if (lblStatus.ForeColor == Color.Red)
            {
                // 紅色文字 = 紅方回合
                PlaySound(Properties.Resources.Sys_TurnRed);
            }
            else if (lblStatus.ForeColor == Color.Blue)
            {
                // 藍色文字 = 藍方回合
                PlaySound(Properties.Resources.Sys_TurnBlue);
            }
        }

        /// <summary>
        /// 清空棋子選擇狀態
        /// </summary>
        private void ClearSelection()
        {
            selectedIndex = -1;
            for (int i = 0; i < 16; i++)
                pbTiles[i].BackgroundImage = null;
        }

        /// <summary>
        /// 取得目前玩家的陣營（1 = 紅方，-1 = 藍方）
        /// </summary>
        private int GetCurrentPlayerSide()
        {
            if (currentPlayer == 1)
                return p1Side;
            else
                return p1Side == 1 ? -1 : 1;
        }

        /// <summary>
        /// 判斷一維陣列索引在 4x4 棋盤中是否相鄰
        /// </summary>
        private bool IsAdjacent(int idx1, int idx2)
        {
            int row1 = idx1 / 4;
            int col1 = idx1 % 4;
            int row2 = idx2 / 4;
            int col2 = idx2 % 4;

            int rowDiff = Math.Abs(row1 - row2);
            int colDiff = Math.Abs(col1 - col2);

            return (rowDiff + colDiff) == 1;
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
            else if (tileValue < 0)
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
            int currentPlayerSide = GetCurrentPlayerSide();

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
            // 播放讀取規章語音
            PlaySound(Properties.Resources.Sys_ReadRules);
            
            string rules = "【公司職級之戰 - 雇用規則】\n\n" +
                   "1. 首先，翻開任意一張牌以決定您的陣營 (紅方/藍方)。\n" +
                   "2. 每回合，您可以翻開一張牌或是移動一格。\n" +
                   "3. 職級高者贏 (7 > 6 > 5 > 4 > 3 > 2 > 1)。\n" +
                   "4. 特殊規則：實習生 (1 級) 能夠吃掉董事長 (7 級)。\n" +
                   "5. 吃光對方所有的棋子便獲勝！";
            MessageBox.Show(rules, "雇用指南");
        }

        /// <summary>
        /// 檢查遊戲是否結束
        /// </summary>
        private void CheckGameOver()
        {
            int redCount = 0;   // 紅方棋子數
            int blueCount = 0;  // 藍方棋子數

            // 統計剩餘棋子
            for (int i = 0; i < 16; i++)
            {
                if (boardData[i] > 0)
                    redCount++;
                else if (boardData[i] < 0)
                    blueCount++;
            }

            System.Diagnostics.Debug.WriteLine($"  - 紅方棋子數: {redCount}, 藍方棋子數: {blueCount}");

            // 判定勝負
            if (redCount == 0)
            {
                System.Diagnostics.Debug.WriteLine("========== 遊戲結束：藍方獲勝 ==========");
                // 播放藍方勝利語音
                PlaySound(Properties.Resources.Voice_WinBlue);
                MessageBox.Show("藍方獲勝！", "遊戲結束");
                RestartGame();
                return;
            }

            if (blueCount == 0)
            {
                System.Diagnostics.Debug.WriteLine("========== 遊戲結束：紅方獲勝 ==========");
                // 播放紅方勝利語音
                PlaySound(Properties.Resources.Voice_WinRed);
                MessageBox.Show("紅方獲勝！", "遊戲結束");
                RestartGame();
                return;
            }

            // 進階困敵判定：檢查是否無法繼續遊戲
            if (IsGameStalemate())
            {
                System.Diagnostics.Debug.WriteLine("========== 遊戲結束：僵局 ==========");
                MessageBox.Show($"遊戲無法繼續！玩家 {currentPlayer} 無法移動。", "遊戲結束");
                RestartGame();
            }
        }

        /// <summary>
        /// 檢查是否進入僵局（無法移動且無牌可翻）
        /// </summary>
        private bool IsGameStalemate()
        {
            int currentPlayerSide = GetCurrentPlayerSide();
            bool hasUnflippedTiles = false;
            bool canMove = false;

            // 檢查是否還有蓋著的牌
            for (int i = 0; i < 16; i++)
            {
                if (!isFlipped[i])
                {
                    hasUnflippedTiles = true;
                    break;
                }
            }

            // 如果還有蓋著的牌，就還能翻牌，遊戲不算僵局
            if (hasUnflippedTiles)
                return false;

            // 檢查當前玩家是否有可移動的棋子
            for (int i = 0; i < 16; i++)
            {
                // 找到屬於當前玩家的棋子
                if (boardData[i] != 0 && isFlipped[i])
                {
                    int tileOwner = boardData[i] > 0 ? 1 : -1;
                    if (tileOwner != currentPlayerSide)
                        continue;

                    // 檢查四個相鄰方向
                    int[] adjacentIndices = GetAdjacentIndices(i);
                    foreach (int adjIndex in adjacentIndices)
                    {
                        if (adjIndex == -1)
                            continue;

                        int adjacentTile = boardData[adjIndex];

                        // 相鄰格為空，可以移動
                        if (adjacentTile == 0)
                        {
                            canMove = true;
                            break;
                        }

                        // 相鄰格有敵方棋子，檢查是否能吃
                        if ((boardData[i] > 0 && adjacentTile < 0) || (boardData[i] < 0 && adjacentTile > 0))
                        {
                            int levelA = Math.Abs(boardData[i]);
                            int levelB = Math.Abs(adjacentTile);

                            // 規則 A：大吃小
                            if (levelA >= levelB)
                            {
                                canMove = true;
                                break;
                            }

                            // 規則 B：實習生逆襲
                            if (levelA == 1 && levelB == 7)
                            {
                                canMove = true;
                                break;
                            }
                        }
                    }

                    if (canMove)
                        break;
                }
            }

            return !canMove;
        }

        /// <summary>
        /// 取得相鄰的四個格子索引
        /// </summary>
        private int[] GetAdjacentIndices(int index)
        {
            int row = index / 4;
            int col = index % 4;
            int[] adjacent = new int[4] { -1, -1, -1, -1 }; // 上、下、左、右

            // 上
            if (row > 0)
                adjacent[0] = (row - 1) * 4 + col;

            // 下
            if (row < 3)
                adjacent[1] = (row + 1) * 4 + col;

            // 左
            if (col > 0)
                adjacent[2] = row * 4 + (col - 1);

            // 右
            if (col < 3)
                adjacent[3] = row * 4 + (col + 1);

            return adjacent;
        }

        /// <summary>
        /// 重新開局
        /// </summary>
        private void RestartGame()
        {
            // 重置遊戲狀態
            selectedIndex = -1;
            currentPlayer = 1;
            p1Side = 0;

            // 重新洗牌並初始化
            ShuffleBoard();

            // 更新玩家狀態顯示
            UpdatePlayerStatus();

            System.Diagnostics.Debug.WriteLine("========== 遊戲已重新開始 ==========\n");
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("========== 玩家按下重新開始 ==========");
            
            // 播放重置語音
            PlaySound(Properties.Resources.Sys_ResetGame);
            
            // 清空選擇狀態
            ClearSelection();
            
            // 重新開局
            RestartGame();
        }
    }
}
