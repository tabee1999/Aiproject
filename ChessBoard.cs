//INSTANT C# NOTE: Formerly VB.NET project-level imports:
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

public partial class ChessBoard
{
	private Graphics gr;
	private Bitmap bmp;
	private bool[,] mCells = new bool[8, 8];
	private System.Collections.Generic.List<Queen> Queens = new System.Collections.Generic.List<Queen>();
	private System.Collections.Generic.List<bool[,]> mSolutions = new System.Collections.Generic.List<bool[,]>();
	private bool mUserPlay = false;
	private bool Playing = false;

	[System.ComponentModel.Browsable(false)]
	public bool[,] Cells
	{
		get
		{
			return mCells;
		}
		set
		{
			mCells = value;
		}
	}

	public System.Collections.Generic.List<bool[,]> Solutions
	{
		get
		{
			return mSolutions;
		}
		set
		{
			mSolutions = value;
		}
	}

	public bool UserPlay
	{
		get
		{
			return mUserPlay;
		}
		set
		{
			mUserPlay = value;
		}
	}

	public ChessBoard()
	{
		InitializeComponent();
		DrawBoard();
	}

	private void ChessBoard_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
	{
		if (e.Button == System.Windows.Forms.MouseButtons.Left)
		{
			if (! Playing)
			{
				Queens.Clear();
				ResetCells();
				Playing = true;
				DrawBoard();
			}
			mUserPlay = true;
			byte colIndex = (e.Location.X - this.Left) / 80;
			byte rowIndex = (e.Location.Y - this.Top) / 80;
			Int16 index = Exists(ref new Queen(rowIndex, colIndex));
			if (Queens.Count < 8)
			{
				if (index > -1)
				{
					mCells[rowIndex, colIndex] = false;
					Queens.RemoveAt(index);
				}
				else
				{
					mCells[rowIndex, colIndex] = true;
					Queens.Add(new Queen(rowIndex, colIndex));
				}
			}
			else
			{
				if (index > -1)
				{
					mCells[rowIndex, colIndex] = false;
					Queens.RemoveAt(index);
				}
			}
			DrawBoard();
		}
	}

	private Int16 Exists(ref Queen Queen)
	{
		if (Queens.Count == 0)
		{
			return -1;
		}
		for (byte i = 0; i < Queens.Count; i++)
		{
			if (Queens[i].Row == Queen.Row && Queens[i].Column == Queen.Column)
			{
				return i;
			}
		}
		return -1;
	}

	private void ChessBoard_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
	{
		if (bmp != null)
		{
			this.BackgroundImage = bmp;
		}
	}

	private void ChessBoard_Resize(object sender, System.EventArgs e)
	{
		if (bmp != null)
		{
			DrawBoard();
		}
	}

	public void DrawBoard()
	{
		if (this.Width > 0 && this.Height > 0)
		{
			bmp = new Bitmap(this.Width, this.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
			gr = Graphics.FromImage(bmp);
			System.Drawing.Drawing2D.LinearGradientBrush br = null;
			RectangleF rect = new RectangleF();
			bool flip = true;
			for (float i = 0; i <= 7; i++)
			{
				for (float j = 0; j <= 7; j++)
				{
					rect = new RectangleF((float)(j * System.Convert.ToSingle(bmp.Width / 8.0)), (float)(i * System.Convert.ToSingle(bmp.Height / 8.0)), System.Convert.ToSingle(bmp.Width / 8.0), System.Convert.ToSingle(bmp.Height / 8.0));
					if (flip)
					{
						br = new System.Drawing.Drawing2D.LinearGradientBrush(rect, Color.White, Color.White, System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal);
					}
					else
					{
						br = new System.Drawing.Drawing2D.LinearGradientBrush(rect, Color.Black, Color.White, System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal);
					}
					gr.FillRectangle(br, rect);
					flip = ((j == 7) ? flip : ! flip);
					if (mCells[i, j])
					{
						if (mUserPlay)
						{
							Int16 index = Exists(ref new Queen(i, j));
							if (index > 0)
							{
								if (Queens[index] != null)
								{
									if (! (CheckAll(index)))
									{
										gr.FillEllipse(new System.Drawing.Drawing2D.LinearGradientBrush(rect, Color.Red, Color.Blue, System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal), rect);
									}
									else
									{
										gr.FillEllipse(new System.Drawing.Drawing2D.LinearGradientBrush(rect, Color.Gold, Color.Pink, System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal), rect);
									}
								}
							}
							else
							{
								gr.FillEllipse(new System.Drawing.Drawing2D.LinearGradientBrush(rect, Color.Gold, Color.Pink, System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal), rect);
							}
						}
						else
						{
							gr.FillEllipse(new System.Drawing.Drawing2D.LinearGradientBrush(rect, Color.Gold, Color.Pink, System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal), rect);
						}
						gr.DrawString("Queen", this.Font, Brushes.Black, rect);
					}
				}
			}
			this.Invalidate();
		}
	}

	public void ResetCells()
	{
		for (byte i = 0; i <= 7; i++)
		{
			for (byte j = 0; j <= 7; j++)
			{
				mCells[i, j] = false;
			}
		}
	}

	public void FindSolution()
	{
		ResetCells();
		for (byte i = 0; i <= 7; i++)
		{
			byte j = 0;
			if (i >= 0 && i < 8 / 2.0)
			{
				j = (8 / 2.0 + 2 * i - 1) % 8;
			}
			else
			{
				j = (8 / 2.0 + 2 * i + 2) % 8;
			}
			mCells[i, j] = true;
		}
		DrawBoard();
	}

	private bool CheckAll(int Level)
	{
		for (int i = Level; i >= 0; i--)
		{
			for (int j = i - 1; j >= 0; j--)
			{
				if (Queens[i].Row == Queens[j].Row || Queens[i].Column == Queens[j].Column || Queens[i].Row + Queens[i].Column == Queens[j].Row + Queens[j].Column | Queens[i].Row - Queens[j].Row == Queens[i].Column - Queens[j].Column)
				{
					return false;
				}
			}
		}
		return true;
	}

	private void MoveQueen(int Level)
	{
		if (Level > 7)
		{
			for (int j = 0; j <= 7; j++)
			{
				for (int i = 0; i <= 7; i++)
				{
					if ((Queens[j].Row == j) & (Queens[j].Column == i))
					{
						mCells[i, j] = true;
					}
					else
					{
						mCells[i, j] = false;
					}
				}
			}
			Solutions.Add(mCells.Clone());
			return;
		}
		for (int j = 0; j <= 7; j++)
		{
			if (Level < 8)
			{
				Queens[Level].Row = Level;
				Queens[Level].Column = j;
				if (CheckAll(Level))
				{
					MoveQueen(Level + 1);
				}
			}
		}
	}

	public void GetSolutions()
	{
		mUserPlay = false;
		Playing = false;
		Queens.Clear();
		ResetCells();
		DrawBoard();
		for (int j = 0; j <= 7; j++)
		{
			Queens.Add(new Queen());
		}
		for (int i = 0; i <= 7; i++)
		{
			Queens[0].Row = 0;
			Queens[0].Column = i;
			MoveQueen(1);
		}
	}
}