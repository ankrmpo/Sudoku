using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sudoku
{
    public partial class Form1 : Form
    {
        private List<Button> levels; //gumbi za odabir osnovnih težina
        private List<Button> slevels; //gumbi za odabir posebnih težina
        private DataGridView grid; //sudoku
        private List<DataGridView> grids; //sudoku grid za samurai
        private int cellwidth;
        private int cellheight;
        private int cellnumber;

        public Form1()
        {
            levels = new List<Button>();
            slevels = new List<Button>();
            grids = new List<DataGridView>();

            cellwidth = 45;
            cellheight = 45;
            cellnumber = 9;

            InitializeComponent();
            DoubleBuffered = true; //za smanjenje grafičkih smetnji
        }

        private DataGridView initialize_NewGrid(string ime) //svaki sudoku će imati ova svojstva
        {
            grid = new DataGridView();
            grid.Name = ime;
            grid.AllowUserToResizeColumns = false;
            grid.AllowUserToResizeRows = false;
            grid.AllowUserToAddRows = false;
            grid.RowHeadersVisible = false;
            grid.ColumnHeadersVisible = false;
            grid.MultiSelect = false;
            grid.GridColor = Color.DarkRed;
            grid.DefaultCellStyle.BackColor = Color.WhiteSmoke;
            grid.DefaultCellStyle.SelectionBackColor = Color.Crimson;
            grid.ScrollBars = ScrollBars.None;
            grid.Font = new Font("Calibri", 16.2F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            grid.ForeColor = Color.DarkRed;
            grid.SelectionMode = DataGridViewSelectionMode.CellSelect;
            grid.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(grid_EditingControlShowing);
            return grid;
        }

        private void clear_Buttons() //čisti liste gumbi
        {
            for (int i = 0; i < levels.Count; ++i)
            {
                Button btn1 = levels[i];

                if (this.Controls.Contains(btn1))
                {
                    this.Controls.Remove(btn1);
                }
            }

            for (int i = 0; i < slevels.Count; ++i)
            {
                Button btn2 = slevels[i];

                if (this.Controls.Contains(btn2))
                {
                    this.Controls.Remove(btn2);
                }
            }

            levels.Clear();
            slevels.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            clear_Buttons();
            this.Controls.Remove(grid);
            for(int i = 0; i < grids.Count; ++i)
                this.Controls.Remove(grids[i]);
            this.label1.Visible = true;

            int x = 0;
            for (int i = 0; i < 4; ++i)
            {
                Button gumb = new Button();
                levels.Add(gumb);
                gumb.Size = this.button1.Size;
                if (i < 3) gumb.Location = new Point(600 + x, this.button1.Location.Y);
                else gumb.Location = new Point(levels[1].Location.X, this.button2.Location.Y);
                x += gumb.Size.Width + 10;
                gumb.AutoSize = false;
                gumb.BackColor = Color.Black;
                gumb.ForeColor = Color.DarkRed;
                gumb.FlatAppearance.BorderColor = Color.DarkRed;
                gumb.FlatAppearance.BorderSize = 5;
                gumb.FlatStyle = FlatStyle.Flat;
                gumb.Font = new Font("Algerian", 22);

                if (i == 0)
                {
                    gumb.Text = "EASY";
                    gumb.Name = "easy";
                }

                else if (i == 1)
                {
                    gumb.Text = "MEDIUM";
                    gumb.Name = "medium";
                }

                else if (i == 2)
                {
                    gumb.Text = "HARD";
                    gumb.Name = "hard";
                }

                else
                {
                    gumb.Text = "SPECIAL";
                    gumb.Name = "special";
                }
                
                if (i < 3) gumb.Click += new EventHandler(gumb_Click);
                else gumb.Click += new EventHandler(gumb_SpecialClick);
                gumb.MouseEnter += new EventHandler(gumb_MouseEnter);
                gumb.MouseLeave += new EventHandler(gumb_MouseLeave);
                this.Controls.Add(gumb);
            }
        }

        private void gumb_MouseEnter(object sender, EventArgs e)
        {
            Button gumb = (sender as Button);
            gumb.BackColor = Color.DarkRed;
            gumb.ForeColor = Color.Black;
            gumb.FlatAppearance.BorderColor = Color.Black;
        }

        private void gumb_MouseLeave(object sender, EventArgs e)
        {
            Button gumb = (sender as Button);
            gumb.BackColor = Color.Black;
            gumb.ForeColor = Color.DarkRed;
            gumb.FlatAppearance.BorderColor = Color.DarkRed;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void gumb_Click(object sender, EventArgs e)
        {
            clear_Buttons();
            start_Game(sender);
        }

        private void gumb_SpecialClick(object sender, EventArgs e)
        {
            clear_Buttons();
            show_SpecialLevels();
        }

        private void show_SpecialLevels()
        {
            clear_Buttons();
            int x = 0;
            for (int i = 0; i < 4; ++i)
            {
                Button gumb = new Button();
                slevels.Add(gumb);
                gumb.Size = this.button1.Size;
                if (i == 3) gumb.Location = new Point(slevels[1].Location.X, this.button2.Location.Y);
                else gumb.Location = new Point(600 + x, this.button1.Location.Y);
                x += gumb.Size.Width + 10;
                gumb.AutoSize = false;
                gumb.BackColor = Color.Black;
                gumb.ForeColor = Color.DarkRed;
                gumb.FlatAppearance.BorderColor = Color.DarkRed;
                gumb.FlatAppearance.BorderSize = 5;
                gumb.FlatStyle = FlatStyle.Flat;
                gumb.Font = new Font("Algerian", 22);

                if (i == 0)
                {
                    gumb.Text = "KILLER";
                    gumb.Name = "killer";
                }

                else if (i == 1)
                {
                    gumb.Text = "SAMURAI";
                    gumb.Name = "samurai";
                }

                else if (i == 2)
                {
                    gumb.Text = "16x16";
                    gumb.Name = "16";
                }

                else if (i == 3)
                {
                    gumb.Text = "BACK";
                    gumb.Name = "back";
                }

                if (i == 3) gumb.Click += new EventHandler(button1_Click);
                else gumb.Click += new EventHandler(gumb_Click);
                gumb.MouseEnter += new EventHandler(gumb_MouseEnter);
                gumb.MouseLeave += new EventHandler(gumb_MouseLeave);
                this.Controls.Add(gumb);
            }
        }

        private void start_Game(object sender) //pokreće igru
        {
            Button gumb = (sender as Button);

            if (gumb.Name == "easy" || gumb.Name == "medium" || gumb.Name == "hard") //one sve imaju tablicu 9x9
            {
                initialize_NewGrid("grid");
                grid.Location = new Point(this.label1.Location.X, this.button1.Location.Y - 100);
                start_NormalGame();
            }

            else if (gumb.Name == "16") //tablica 16x16
            {
                initialize_NewGrid("grid");
                cellnumber = 16;
                this.label1.Visible = false;
                grid.Location = new Point(this.label1.Location.X - 200, this.button1.Location.Y - 350);
                start_NormalGame();
            }

            else if (gumb.Name == "samurai") //samurai grid, 5 tablica 9x9
            {
                cellnumber = 9;
                cellwidth = 30;
                cellheight = 30;
                this.label1.Visible = false;
                for (int j = 0; j < 5; ++j)
                {
                    string name = "grid" + j.ToString();
                    DataGridView grid = initialize_NewGrid(name);
                    grids.Add(grid);
                }
               
                start_SamuraiGame();
            }
  
        }

        private void start_NormalGame()
        {
            grid.Size = new Size(cellwidth * cellnumber + 3, cellwidth * cellnumber + 3);

            for (int i = 0; i < cellnumber; ++i)
            {
                DataGridViewTextBoxColumn text = new DataGridViewTextBoxColumn();
                text.MaxInputLength = 1;
                grid.Columns.Add(text);
                grid.Columns[i].Name = (i + 1).ToString();
                grid.Columns[i].Width = cellwidth;
                grid.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                DataGridViewRow row = new DataGridViewRow();
                row.Height = cellheight;
                grid.Rows.Add(row);
            }
            //podebljanje 3x3 odnosno 4x4 podtablica u tablici
            if (cellnumber == 9)
            {
                grid.Columns[2].DividerWidth = 2;
                grid.Columns[5].DividerWidth = 2;
                grid.Rows[2].DividerHeight = 2;
                grid.Rows[5].DividerHeight = 2;
            }

            else if (cellnumber == 16)
            {
                grid.Columns[3].DividerWidth = 2;
                grid.Columns[7].DividerWidth = 2;
                grid.Columns[11].DividerWidth = 2;
                grid.Rows[3].DividerHeight = 2;
                grid.Rows[7].DividerHeight = 2;
                grid.Rows[11].DividerHeight = 2;
            }

            Controls.Add(grid);
        }

        private void start_SamuraiGame()
        {
            Size size = new Size(cellwidth * cellnumber, cellwidth * cellnumber);

            grids[0].Location = new Point(this.label1.Location.X - 300, this.button1.Location.Y - 300);
            grids[1].Location = new Point(grids[0].Location.X + size.Width + 3 * cellwidth, grids[0].Location.Y);
            grids[2].Location = new Point(grids[0].Location.X + 6 * cellwidth, grids[0].Location.Y + 6 * cellwidth);
            grids[3].Location = new Point(grids[0].Location.X, grids[0].Location.Y + size.Width + 3 * cellwidth);
            grids[4].Location = new Point(grids[1].Location.X, grids[3].Location.Y);

            for (int i = 0; i < grids.Count; ++i)
            {
                grids[i].Size = size;
                for (int j = 0; j < cellnumber; ++j)
                {
                    DataGridViewTextBoxColumn text = new DataGridViewTextBoxColumn();
                    text.MaxInputLength = 1;
                    grids[i].Columns.Add(text);
                    grids[i].Columns[j].Name = (i + 1).ToString() + (j + 1).ToString();
                    grids[i].Columns[j].Width = cellwidth;
                    grids[i].Columns[j].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    DataGridViewRow row = new DataGridViewRow();
                    row.Height = cellheight;
                    grids[i].Rows.Add(row);
                }

                grids[i].Rows[0].Cells[0].Style.SelectionBackColor=Color.WhiteSmoke;//treba promijeniti poslije
                grids[i].Columns[2].DividerWidth = 2;
                grids[i].Columns[5].DividerWidth = 2;
                grids[i].Rows[2].DividerHeight = 2;
                grids[i].Rows[5].DividerHeight = 2;

                Controls.Add(grids[i]);
            }                
        }
        //ne smijemo dopustiti unos nekih slova
        private void grid_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(cell_KeyPress);
            TextBox tb = e.Control as TextBox;
            if (tb != null) tb.KeyPress += new KeyPressEventHandler(cell_KeyPress);
        }

        private void cell_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cellnumber == 9 && (!char.IsDigit(e.KeyChar) || (char.IsDigit(e.KeyChar) && char.Equals(e.KeyChar, '0'))))
            {
                e.Handled = true;
            }
            else if (cellnumber == 16)
            {
                if ((!char.IsDigit(e.KeyChar) && !char.Equals(e.KeyChar, 'A') && !char.Equals(e.KeyChar, 'B') && !char.Equals(e.KeyChar, 'C') && !char.Equals(e.KeyChar, 'D') && !char.Equals(e.KeyChar, 'E') && !char.Equals(e.KeyChar, 'F') && !char.Equals(e.KeyChar, 'G')) || (char.IsDigit(e.KeyChar) && char.Equals(e.KeyChar, '0')))
                {
                    e.Handled = true;
                }
            }
        }
    }
}
