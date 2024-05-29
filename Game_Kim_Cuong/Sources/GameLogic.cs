using Accessibility;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace  Game_Kim_Cuong.Sources
{
    public class GameLogic
    {
        private BoardGame boardGame;

        private List<Diamond> EmptyDiamond = new List<Diamond>();
        private Stack<Diamond> IsMoved = new Stack<Diamond>();
        private List<Diamond> DiamondAboveEaten = new List<Diamond>();

        public GameLogic (BoardGame boardGame)
        {
            this.boardGame = boardGame;
            this.EmptyDiamond = new List<Diamond>();
        }

        public void SwapDiamond(Diamond _d1, Diamond _d2)
        {
            Image temp = _d2.BackgroundImage;
            Diamond.DiamondTag tag_temp = _d2.bTag;

            _d2.ChangeImage(_d1.BackgroundImage, _d1.bTag);
            _d1.ChangeImage(temp, tag_temp);

        }


        public async Task DestroyMatchingDiamond(Diamond _d1, Diamond _d2)
        {
            bool isDestroy = false;
            Diamond.DiamondTag tmp_d1 = _d1.bTag;
            Diamond.DiamondTag tmp_d2 = _d2.bTag;

            void CheckRow(Diamond _d, Diamond.DiamondTag d_tag)
            {
                List<Diamond> temp = new List<Diamond>();
                temp.Add(_d);
                for (int i = _d.point.Y + 1; i < this.boardGame.c_col; i++ )
                {
                    if (d_tag == this.boardGame.diamondList[_d.point.X][i].bTag)
                    {
                        temp.Add(this.boardGame.diamondList[_d.point.X][i]);
                        if (_d.point.X - 1 >= 0)
                            this.DiamondAboveEaten.Add(this.boardGame.diamondList[_d.point.X - 1][i]);
                    }
                    else break;
                }

                for (int i = _d.point.Y - 1; i >= 0; i--)
                {
                    if (d_tag == this.boardGame.diamondList[_d.point.X][i].bTag)
                    {
                        temp.Add(this.boardGame.diamondList[_d.point.X][i]);
                        if (_d.point.X - 1 >= 0)
                            this.DiamondAboveEaten.Add(this.boardGame.diamondList[_d.point.X - 1][i]);
                    }
                    else break;
                }

                if (temp.Count >= 3)
                {
                    isDestroy = true;
                    foreach (Diamond d in temp)
                    {
                        if (!this.EmptyDiamond.Contains(d)) this.EmptyDiamond.Add(d);
                        d.HighLight(true);
                        
                    }
                }
            }

            void CheckCol(Diamond _d, Diamond.DiamondTag d_tag)
            {
                List<Diamond> temp = new List<Diamond>();
                temp.Add(_d);
                int i;
                for (i = _d.point.X + 1; i < this.boardGame.c_row; i++)
                {
                    if (d_tag == this.boardGame.diamondList[i][_d.point.Y].bTag)
                    {
                        temp.Add(this.boardGame.diamondList[i][_d.point.Y]);
                    }
                    else break;
                }

                for (i = _d.point.X - 1; i >= 0; i--)
                {
                    if (d_tag == this.boardGame.diamondList[i][_d.point.Y].bTag)
                    {
                        temp.Add(this.boardGame.diamondList[i][_d.point.Y]);
                    }
                    else break;
                }

                if (i - 1 >= 0) this.DiamondAboveEaten.Add(this.boardGame.diamondList[i - 1][_d.point.Y]);

                if (temp.Count >= 3)
                {
                    isDestroy = true;
                    foreach (Diamond d in temp)
                    {
                        if (!this.EmptyDiamond.Contains(d)) this.EmptyDiamond.Add(d);
                        d.HighLight(true);
                        
                    }
                }
            }

            CheckCol(_d1, tmp_d1);
            CheckCol(_d2, tmp_d2);
            CheckRow(_d1, tmp_d1);
            CheckRow(_d2, tmp_d2);

            if (!isDestroy) SwapDiamond(_d1, _d2);
            else
            {
                await FillTheEmptyDiamond();
            }
        }
            

        private async Task FillTheEmptyDiamond()
        {
            if (this.EmptyDiamond.Count == 0) return;
            //Sort List EmptyDiamond
            int _min;
            for (int i = 0; i < this.EmptyDiamond.Count - 1; i++)
            {
                _min = i;
                for (int j = i + 1; j < this.EmptyDiamond.Count; j++)
                {
                    if (this.EmptyDiamond[j].point.X < this.EmptyDiamond[_min].point.X) _min = j;
                }

                if (_min != i)
                {
                    Diamond temp = this.EmptyDiamond[i];
                    this.EmptyDiamond[i] = this.EmptyDiamond[_min];
                    this.EmptyDiamond[_min] = temp;
                }
            }

            List<Diamond> clone = new List<Diamond> ();
            await Task.Delay(500);
            foreach (Diamond d in this.EmptyDiamond)
            {
                int _dX = d.point.X;
                int _dY = d.point.Y;
                
                //if (this.IsMoved.Contains(this.boardGame.diamondList[_dX][_dY]))
                    this.IsMoved.Push(this.boardGame.diamondList[_dX][_dY]);
                this.boardGame.diamondList[_dX][_dY].HighLight(false);

                for (int i = _dX; i > 0; i--)
                {
                    SwapDiamond(this.boardGame.diamondList[i][_dY], this.boardGame.diamondList[i - 1][_dY]);
                    //if (this.IsMoved.Contains(this.boardGame.diamondList[i - 1][_dY]))
                    this.IsMoved.Push(this.boardGame.diamondList[i - 1][_dY]);
                    this.boardGame.diamondList[i - 1][_dY].HighLight(false);
                }

                int rand;
                Diamond.DiamondTag tag;
                do
                {
                    rand = this.boardGame.RandNumInt(1, 8);
                    if (rand == 1) tag = Diamond.DiamondTag.Hutao;
                    else if (rand == 2) tag = Diamond.DiamondTag.Ayaka;
                    else if (rand == 3) tag = Diamond.DiamondTag.Ganyu;
                    else if (rand == 4) tag = Diamond.DiamondTag.Lumine;
                    else if (rand == 5) tag = Diamond.DiamondTag.Keqing;
                    else if (rand == 6) tag = Diamond.DiamondTag.Yoimiya;
                    else if (rand == 7) tag = Diamond.DiamondTag.Xiao;
                    else if (rand == 8) tag = Diamond.DiamondTag.Jean;
                    else tag = Diamond.DiamondTag.Hutao;

                } while (tag == this.boardGame.diamondList[1][_dY].bTag);
                this.boardGame.diamondList[0][_dY].ChangeImage(this.boardGame.MyAssets.ElementAt(rand - 1), tag);

                List<Diamond> tmp = new List<Diamond>();
                tmp.Add(d);
                for (int i = _dX + 1; i < this.boardGame.c_col; i++)
                {
                    if (d.bTag == this.boardGame.diamondList[i][_dY].bTag)
                    {
                        tmp.Add(this.boardGame.diamondList[i][_dY]);
                    }
                    else break;
                }

                for (int i = _dX - 1; i >= 0; i--)
                {
                    if (d.bTag == this.boardGame.diamondList[i][_dY].bTag)
                    {
                        tmp.Add(this.boardGame.diamondList[i][_dY]);
                    } else break;
                }

                if (tmp.Count >= 3)
                {
                    foreach (Diamond _d in tmp)
                    {
                        _d.HighLight(true);
                        
                        clone.Add(_d);
                    }
                }
            }
            await Task.Delay(500);


            this.EmptyDiamond = new List<Diamond>();

            this.EmptyDiamond.AddRange(clone); 
            await FillTheEmptyDiamond();
            this.RecheckNewDiamond();
        }

        private async void RecheckNewDiamond()
        {
            if (this.IsMoved.Count == 0)
            {
                return;
            }

            while (this.IsMoved.Count > 0)
            {
                Diamond _d = this.IsMoved.Pop();
                List<Diamond> temp = new List<Diamond>();
                temp.Add(_d);
                for (int i = _d.point.Y + 1; i < this.boardGame.c_col; i++)
                {
                    if (_d.bTag == this.boardGame.diamondList[_d.point.X][i].bTag)
                    {
                        temp.Add(this.boardGame.diamondList[_d.point.X][i]);
                    }
                    else break;
                }

                for (int i = _d.point.Y - 1; i >= 0; i--)
                {
                    if (_d.bTag == this.boardGame.diamondList[_d.point.X][i].bTag)
                    {
                        temp.Add(this.boardGame.diamondList[_d.point.X][i]);
                    }
                    else break;
                }

                if (temp.Count >= 3)
                {
                    foreach (Diamond d in temp)
                    {
                        d.HighLight(true);
                        if (!this.EmptyDiamond.Contains(d))
                        this.EmptyDiamond.Add(d);
                    }
                }
                
            }

            await FillTheEmptyDiamond();
        }

        #region HINT
        private bool valid(int x)
        {
            if (x >= 0 && x < this.boardGame.c_col) return true;
            return false;
        }


        private Diamond getDiamond(int x, int y)
        {
            return this.boardGame.diamondList[x][y];
        }


        private bool HintCheckCase1(Diamond d, List<Diamond> hint)
        {
            List<Diamond> temp = new List<Diamond>();
            temp.Add(d);
            for (int i = 2; i <= 3; i++)
            {
                if (valid(d.point.Y + i))
                {
                    if (d.bTag == getDiamond(d.point.X, d.point.Y + i).bTag) temp.Add(getDiamond(d.point.X, d.point.Y + i));
                    else break;
                }
                else break;
            }

            if (temp.Count >= 3)
            {
                foreach (Diamond _d in temp) hint.Add(_d);
                return true;
            }

            temp = new List<Diamond>();
            temp.Add(d);

            for (int i = 2; i <= 3; i++)
            {
                if (valid(d.point.Y - i))
                {
                    if (d.bTag == getDiamond(d.point.X, d.point.Y - i).bTag) temp.Add(getDiamond(d.point.X, d.point.Y - i));
                    else break;
                }
                else break;
            }

            if (temp.Count >= 3)
            {
                foreach(Diamond _d in temp) hint.Add(_d);
                return true;
            }

            return false;
        }
        private bool HintCheckCase2(Diamond d, List<Diamond> hint)
        {
            List<Diamond> temp = new List<Diamond>();
            temp.Add(d);
            for (int i = 2; i <= 3; i++)
            {
                if (valid(d.point.X + i))
                {
                    if (d.bTag == getDiamond(d.point.X + i, d.point.Y).bTag) temp.Add(getDiamond(d.point.X + i, d.point.Y));
                    else break;
                }
                else break;
            }

            if (temp.Count >= 3)
            {
                foreach (Diamond _d in temp) hint.Add(_d);
                return true;
            }

            temp = new List<Diamond>();
            temp.Add(d);

            for (int i = 2; i <= 3; i++)
            {
                if (valid(d.point.X - i))
                {
                    if (d.bTag == getDiamond(d.point.X - i, d.point.Y).bTag) temp.Add(getDiamond(d.point.X - i, d.point.Y));
                    else break;
                }
                else break;
            }

            if (temp.Count >= 3)
            {
                foreach (Diamond _d in temp) hint.Add(_d);
                return true;
            }

            return false;
        }
        private bool HintCheckCase3(Diamond d, List<Diamond> hint)
        {
            if (!valid(d.point.X - 1)) return false;
            List<Diamond> temp = new List<Diamond>();
            temp.Add(d);
            for (int i = 1; i <= 2; i++)
            {
                if (valid(d.point.Y + i))
                {
                    if (d.bTag == getDiamond(d.point.X - 1, d.point.Y + i).bTag) temp.Add(getDiamond(d.point.X - 1, d.point.Y + i));
                    else break;
                }
                else break;
            }

            for (int i = 1; i <= 2; i++)
            {
                if (valid(d.point.Y - i))
                {
                    if (d.bTag == getDiamond(d.point.X - 1, d.point.Y - i).bTag) temp.Add(getDiamond(d.point.X - 1, d.point.Y - i));
                    else break;
                }
                else break;
            }

            if (temp.Count >= 3)
            {
                foreach (Diamond _d in temp) hint.Add(_d);
                return true;
            }

            return false;
        }
        private bool HintCheckCase4(Diamond d, List<Diamond> hint)
        {
            if (!valid(d.point.X + 1)) return false;
            List<Diamond> temp = new List<Diamond>();
            temp.Add(d);
            for (int i = 1; i <= 2; i++)
            {
                if (valid(d.point.Y + i))
                {
                    if (d.bTag == getDiamond(d.point.X + 1, d.point.Y + i).bTag) temp.Add(getDiamond(d.point.X + 1, d.point.Y + i));
                    else break;
                }
                else break;
            }

            for (int i = 1; i <= 2; i++)
            {
                if (valid(d.point.Y - i))
                {
                    if (d.bTag == getDiamond(d.point.X + 1, d.point.Y - i).bTag) temp.Add(getDiamond(d.point.X + 1, d.point.Y - i));
                    else break;
                }
                else break;
            }

            if (temp.Count >= 3)
            {
                foreach (Diamond _d in temp) hint.Add(_d);
                return true;
            }

            return false;
        }
        private bool HintCheckCase5(Diamond d, List<Diamond> hint)
        {
            if (!valid(d.point.Y + 1)) return false;
            List<Diamond> temp = new List<Diamond>();
            temp.Add(d);
            for (int i = 1; i <= 2; i++)
            {
                if (valid(d.point.X + i))
                {
                    if (d.bTag == getDiamond(d.point.X + i, d.point.Y + 1).bTag) temp.Add(getDiamond(d.point.X + i, d.point.Y + 1));
                    else break;
                }
                else break;
            }

            for (int i = 1; i <= 2; i++)
            {
                if (valid(d.point.X - i))
                {
                    if (d.bTag == getDiamond(d.point.X - i, d.point.Y + 1).bTag) temp.Add(getDiamond(d.point.X - i, d.point.Y + 1));
                    else break;
                }
                else break;
            }

            if (temp.Count >= 3)
            {
                foreach (Diamond _d in temp) hint.Add(_d);
                return true;
            }

            return false;
        }
        private bool HintCheckCase6(Diamond d, List<Diamond> hint)
        {
            if (!valid(d.point.Y - 1)) return false;
            List<Diamond> temp = new List<Diamond>();
            temp.Add(d);
            for (int i = 1; i <= 2; i++)
            {
                if (valid(d.point.X + i))
                {
                    if (d.bTag == getDiamond(d.point.X + i, d.point.Y - 1).bTag) temp.Add(getDiamond(d.point.X + i, d.point.Y - 1));
                    else break;
                }
                else break;
            }

            for (int i = 1; i <= 2; i++)
            {
                if (valid(d.point.X - i))
                {
                    if (d.bTag == getDiamond(d.point.X - i, d.point.Y - 1).bTag) temp.Add(getDiamond(d.point.X - i, d.point.Y - 1));
                    else break;
                }
                else break;
            }

            if (temp.Count >= 3)
            {
                foreach (Diamond _d in temp) hint.Add(_d);
                return true;
            }

            return false;
        }

        public List<Diamond> Hint()
        {
            List<Diamond> hint = new List<Diamond>();
            bool isBreak = false;
            foreach (List<Diamond> row in this.boardGame.diamondList)
            {
                if (isBreak) break;
                foreach (Diamond d in row)
                {
                    if (HintCheckCase1(d, hint) 
                        || HintCheckCase2(d, hint)
                        || HintCheckCase3(d, hint)
                        || HintCheckCase4(d, hint)
                        || HintCheckCase5(d, hint)
                        || HintCheckCase6(d, hint))
                    {
                        isBreak = true;
                        break;
                    }
                    
                }
            }

            foreach(Diamond d in hint)
            {
                d.HighLight(true);
            }

            return hint;
        }

        #endregion 
    }
}