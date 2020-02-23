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

        //gumbi i labeli za vrijeme, bilješke i kraj igre
        private Button notesb;
        private Label notesl;
        private Label time;
        private Timer t;
        private int timeElapsed;
        private bool on;
        private Label congratsl;
        private PictureBox congratsp;

        //matrice igre
        private int[,] matrica9;
        private int[,] matrica16;
        //generirane matrice
        private int[,] gmatrica9;
        private int[,] gmatrica16;

        public Form1()
        {
            levels = new List<Button>();
            slevels = new List<Button>();
            grids = new List<DataGridView>();
            t = new Timer();

            cellwidth = 45;
            cellheight = 45;
            cellnumber = 9;
            timeElapsed = 0;

            matrica9 = new int[9, 9];
            matrica16 = new int[16, 16];
            gmatrica9 = new int[9, 9];
            gmatrica16 = new int[16, 16];

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
            grid.CellValueChanged += new DataGridViewCellEventHandler(cell_CellValueChanged);
            return grid;
        }

        private void clear_All() //čisti gumbe i grid za ponovni početak igre
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

            this.Controls.Remove(grid);
            for (int i = 0; i < grids.Count; ++i)
                this.Controls.Remove(grids[i]);

            this.Controls.Remove(notesb);
            this.Controls.Remove(notesl);
            on = false;

            if(t.Enabled == true) t.Stop();
            this.Controls.Remove(time);
            timeElapsed = 0;

            Array.Clear(matrica9, 0, matrica9.Length);
            Array.Clear(matrica16, 0, matrica16.Length);
            Array.Clear(gmatrica9, 0, gmatrica9.Length);
            Array.Clear(gmatrica16, 0, gmatrica16.Length);

            this.label1.Visible = true;
            this.Controls.Remove(congratsl);
            this.Controls.Remove(congratsp);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            clear_All();

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
            clear_All();
            start_Game(sender);
        }

        private void gumb_SpecialClick(object sender, EventArgs e)
        {
            clear_All();
            show_SpecialLevels();
        }

        private void show_SpecialLevels() //prikazuje posebne levele
        {
            clear_All();
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

        private void initialize_Notes() //stvara label i gumb za bilješke
        {
            notesb = new Button();
            notesl = new Label();

            notesl.Name = "notesl";
            notesl.Text = "TOGGLE NOTES:";
            notesl.Size = this.button1.Size;
            notesl.AutoSize = true;
            notesl.BackColor = Color.Black;
            notesl.ForeColor = Color.DarkRed;
            notesl.Font = new Font("Algerian", 22);
            notesl.TextAlign = ContentAlignment.MiddleCenter;

            notesb.Name = "notesb";
            notesb.Text = "OFF";
            notesb.Size = new Size((int)this.button1.Size.Width / 3, (int)this.button1.Size.Height / 3);
            notesb.AutoSize = false;
            notesb.BackColor = Color.Black;
            notesb.ForeColor = Color.DarkRed;
            notesb.FlatAppearance.BorderColor = Color.DarkRed;
            notesb.FlatAppearance.BorderSize = 2;
            notesb.FlatStyle = FlatStyle.Flat;
            notesb.TextAlign = ContentAlignment.MiddleCenter;
            notesb.Font = new Font("Algerian", 20);
            notesb.Click += new EventHandler(notes_Click);
            notesb.MouseEnter += new EventHandler(gumb_MouseEnter);
            notesb.MouseLeave += new EventHandler(gumb_MouseLeave);

            this.Controls.Add(notesl);
            this.Controls.Add(notesb);
        }

        private void initialize_Time() //stvara label za prikaz vremena
        {
            time = new Label();
            time.Name = "time";
            TimeSpan timespan = TimeSpan.FromMilliseconds(timeElapsed);
            time.Text = "Time: " + timespan.ToString(@"mm\:ss");
            time.Size = this.button1.Size;
            time.AutoSize = true;
            time.BackColor = Color.Black;
            time.ForeColor = Color.DarkRed;
            time.Font = new Font("Algerian", 22);
            time.TextAlign = ContentAlignment.MiddleCenter;

            t = new Timer();
            t.Tick += new EventHandler(t_Tick);
            t.Interval = 1000;
            t.Start();

            this.Controls.Add(time);
        }

        private void start_Game(object sender) //pokreće igru
        {
            initialize_Notes();
            initialize_Time();

            Button gumb = (sender as Button);

            if (gumb.Name == "easy" || gumb.Name == "medium" || gumb.Name == "hard" || gumb.Name == "killer") //one sve imaju tablicu 9x9
            {
                generate_sudoku9(sender); //generiramo sudoku igru 9x9 ovisno o težini
                initialize_NewGrid("grid");
                cellwidth = 45;
                cellheight = 45;
                cellnumber = 9;
                grid.Location = new Point(this.label1.Location.X, this.button1.Location.Y - 150);
                start_NormalGame();
            }

            else if (gumb.Name == "16") //tablica 16x16
            {
                generate_sudoku16(); //generiramo sudoku igru 16x16
                initialize_NewGrid("grid");
                cellwidth = 35;
                cellheight = 35;
                cellnumber = 16;
                this.label1.Visible = false;
                grid.Location = new Point(this.label1.Location.X - 100, this.button1.Location.Y - 300);
                start_NormalGame();
            }

            else if (gumb.Name == "samurai") //samurai grid, 5 tablica 9x9
            {
                cellwidth = 30;
                cellheight = 30;
                cellnumber = 9;
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
            notesl.Location = new Point(this.label1.Location.X, this.grid.Location.Y + grid.Size.Height + 20);
            notesb.Location = new Point(notesl.Location.X + notesl.Size.Width + 5, notesl.Location.Y);
            time.Location = new Point(notesl.Location.X, notesl.Location.Y + notesl.Size.Height + 5);

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

            grids[0].Location = new Point(this.label1.Location.X - 200, this.button1.Location.Y - 350);
            grids[1].Location = new Point(grids[0].Location.X + size.Width + 3 * cellwidth, grids[0].Location.Y);
            grids[2].Location = new Point(grids[0].Location.X + 6 * cellwidth, grids[0].Location.Y + 6 * cellwidth);
            grids[3].Location = new Point(grids[0].Location.X, grids[0].Location.Y + size.Width + 3 * cellwidth);
            grids[4].Location = new Point(grids[1].Location.X, grids[3].Location.Y);

            notesl.Location = new Point(grids[0].Location.X, grids[4].Location.Y + size.Height + 20);
            notesb.Location = new Point(notesl.Location.X + notesl.Size.Width + 5, notesl.Location.Y);
            time.Location = new Point(notesl.Location.X, notesl.Location.Y + notesl.Size.Height + 5);

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

                grids[i].DefaultCellStyle.SelectionBackColor = Color.WhiteSmoke;
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
                return;
            }
            else if (cellnumber == 16)
            {
                if ((!char.IsDigit(e.KeyChar) && !char.Equals(e.KeyChar, 'A') && !char.Equals(e.KeyChar, 'B') && !char.Equals(e.KeyChar, 'C') && !char.Equals(e.KeyChar, 'D') && !char.Equals(e.KeyChar, 'E') && !char.Equals(e.KeyChar, 'F') && !char.Equals(e.KeyChar, 'G')) || (char.IsDigit(e.KeyChar) && char.Equals(e.KeyChar, '0')))
                {
                    e.Handled = true;
                    return;
                }
            }
        }

        //spremanje unešenih podataka u matrice i provjera je li igra gotova
        private void cell_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView grid = (sender as DataGridView);
            DataGridViewCell cell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (on) //ako su uključene bilješke, onda samo želimo zapisati moguće vrijednosti, ali ih ne spremamo
            {
                //dopustiti unos više vrijednosti u jednu ćeliju
            }

            else //odlučili smo se za vrijednost, spremamo je u matricu
            {
                //dopustiti samo jednu vrijednost u svaku ćeliju, to je defaultno
                if (cellnumber == 9)
                {
                    matrica9[e.RowIndex, e.ColumnIndex] = Convert.ToInt32(cell.Value);
                }

                else if (cellnumber==16)
                {
                    matrica16[e.RowIndex, e.ColumnIndex] = Convert.ToInt32(cell.Value);
                }

                bool gotovo = check_IfDone();
                if (gotovo) show_Congratulations();
            }
        }

        //promijeni se stil unosa kada je gumb notes kliknut
        private void notes_Click(object sender, EventArgs e)
        {
            if (notesb.Text == "OFF")
            {
                notesb.Text = "ON";
                on = true;
            }
            else if (notesb.Text == "ON")
            {
                notesb.Text = "OFF";
                on = false;
            }
        }

        //računanje vremena
        void t_Tick(object sender, EventArgs e)
        {
            timeElapsed += t.Interval;
            TimeSpan timespan = TimeSpan.FromMilliseconds(timeElapsed);
            time.Text = "Time: " + timespan.ToString(@"mm\:ss");
        }

        //uspoređuje generiranu matricu i matricu iz igre
        private bool check_IfDone()
        {
            if (cellnumber == 9 && matrica9.Cast<int>().SequenceEqual(gmatrica9.Cast<int>())) return true;
            else if (cellnumber == 16 && matrica16.Cast<int>().SequenceEqual(gmatrica16.Cast<int>())) return true;
            else return false;
        }

        //čestitke ako korisnik pobijedi
        private void show_Congratulations()
        {
            clear_All();
            this.label1.Visible = false;

            congratsp = new PictureBox();
            congratsp.Name = "congratsp";
            congratsp.Size = new Size(1280, 750);
            congratsp.Location = new Point(this.label1.Location.X - 400, this.label1.Location.Y);
            congratsp.Image = Properties.Resources.fireworks;
            this.Controls.Add(congratsp);

            congratsl = new Label();
            congratsl.Name = "congratsl";
            congratsl.Text = "CONGRATULATIONS\nYOU WON!";
            congratsl.Location = new Point(this.label1.Location.X, this.label1.Location.Y - 100);
            congratsl.Size = this.button1.Size;
            congratsl.AutoSize = true;
            congratsl.BackColor = Color.Black;
            congratsl.ForeColor = Color.DarkRed;
            congratsl.Font = new Font("Algerian", 32);
            congratsl.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(congratsl);
        }

        //generiranje igre
        private void generate_sudoku9(object sender) //sender nam treba da vidimo koje je težine
        {

        }

        private void generate_sudoku16()
        {

        }
    }
}
