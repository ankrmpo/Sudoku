/* MUST READ BEFORE USE:
 * Što se normanih levela tiče, bilješke ne rade savršeno, tj brojevi pišu jedan do drugoga umjesto s razmakom
 * jer mi se pojavljivao Stack Overflow kad sam pokušala samo dodati razmak i novu bilješku. Zato se i sve izbriše
 * kad se ponovno ide unosit vrijednost u tu ćeliju.
 * Napravila sam tri tipa matrice matrica, gmatrica i pgmatrica. Mislila sam da u gmatricu stavimo generirani
 * riješeni sudoku, pa onda u pgmatricu postavimo svugdje 0 osim na mjestima koje ćemo ostavit otkrivenima na
 * početku igre. A matrica je onda matrica igre, u nju idu ta otkrivena polja i ona koja mi unosimo.
 * Za 9x9 sam stavila da su matrice tipa int, a 16x16 string zbog slova koja možemo unosit.
 * Onda se tijekom igre uspoređuju matrica i gmatrica nakon svake promjene vrijednosti ćelije i igra završava
 * kad su jednake, ali ne čim unesemo zadnju točnu vrijednost nego treba još jednom kliknut zbog funkcije u kojoj
 * je to napravljeno (jebiga, nisam znala drukčije).
 * To bi sve trebalo radit i za killer sudoku jer samo kad generiramo matricu treba onda nekako označiti ta polja
 * u kojima će biti sume.
 * A samurai je sjeban jer imamo 5 datagridview-ova. Morale bismo nekako restringirati da se samo jedno polje u tih
 * 5 različitih može označiti u jednom trenutku što je problematično jer ispada da svaki datagridview mora imati
 * defaultno polje koje je na početku označeno. To je ovo polje crvene boje u ostalim levelima, a nema ga u samurai
 * jer sam samo postavila da je boje pozadine. Inače bi vidjela više crvenih polja u isto vrijeme.
 * Još veći problem je taj jedan koji se preklapa s ostalima. Uopće ne znam kak bismo to mogle brzo i jednostavno
 * smislit.
 * Pitanja? XD
 */



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
        private Label congratst;
        private PictureBox congratsp;

        //matrice igre
        private int[,] matrica9;
        private string[,] matrica16;

        //generirane matrice, pg će biti pomoćne u kojima će sve vrijednosti biti 0, osim onih koje se pojavljuju na početku sudokua
        private int[,] gmatrica9;
        private int[,] pgmatrica9;
        private string[,] gmatrica16;
        private string[,] pgmatrica16;

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
            matrica16 = new string[16, 16];
            gmatrica9 = new int[9, 9];
            gmatrica16 = new string[16, 16];
            pgmatrica9 = new int[9, 9];
            pgmatrica16 = new string[16, 16];

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
            grid.Font = new Font("Calibri", 16F, FontStyle.Bold);
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

            Array.Clear(matrica9, 0, matrica9.Length);
            Array.Clear(matrica16, 0, matrica16.Length);
            Array.Clear(gmatrica9, 0, gmatrica9.Length);
            Array.Clear(gmatrica16, 0, gmatrica16.Length);
            Array.Clear(pgmatrica9, 0, pgmatrica9.Length);
            Array.Clear(pgmatrica16, 0, pgmatrica16.Length);

            this.label1.Visible = true;
            this.Controls.Remove(congratsl);
            this.Controls.Remove(congratst);
            this.Controls.Remove(congratsp);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            clear_All();
            timeElapsed = 0;

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

                // Test
                for (int r = 0; r < 9; ++r)
                {
                    for (int c = 0; c < 9; ++c)
                    {
                        System.Console.Write(gmatrica9[r, c]);
                        System.Console.Write("\t");
                    }
                    System.Console.Write("\n");
                }

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
                cellwidth = 42;
                cellheight = 42;
                cellnumber = 16;
                this.label1.Visible = false;
                grid.Location = new Point(this.label1.Location.X - 120, this.button1.Location.Y - 350);
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
            for (int i = 0; i < cellnumber; ++i)
                for (int j = 0; j < cellnumber; ++j)
                {
                    if (cellnumber == 9 && pgmatrica9[i, j] != 0)
                    {
                        grid.Rows[i].Cells[j].Value = pgmatrica9[i, j];
                        grid.Rows[i].Cells[j].ReadOnly = true;
                    }
                    else if (cellnumber == 16 && pgmatrica16[i, j] != "0")
                    {
                        grid.Rows[i].Cells[j].Value = pgmatrica16[i, j];
                        grid.Rows[i].Cells[j].ReadOnly = true;
                    }
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

            if (on) //ako su uključene bilješke, onda samo želimo zapisati moguće vrijednosti, ne spremamo u matricu
            {
                foreach (DataGridViewTextBoxColumn column in grid.Columns)
                {
                    column.MaxInputLength = 3;
                }
            }
            else if (!on)
            {
                grid.Font = new Font("Calibri", 16F, FontStyle.Bold);
                grid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                foreach (DataGridViewTextBoxColumn column in grid.Columns)
                {
                    column.MaxInputLength = 1;
                }
            }
        }

        //spremanje unešenih podataka u matrice i provjera je li igra gotova
        private void cell_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView grid = (sender as DataGridView);
            DataGridViewCell cell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (!on) //odlučili smo se za vrijednost, spremamo je u matricu
            {
                cell.Style.Font = new Font("Calibri", 16F, FontStyle.Bold);
                if (cellnumber == 9)
                {
                    matrica9[e.RowIndex, e.ColumnIndex] = Convert.ToInt32(cell.Value);
                }

                else if (cellnumber == 16)
                {
                    matrica16[e.RowIndex, e.ColumnIndex] = cell.Value.ToString();
                }

                bool gotovo = check_IfDone();
                if (gotovo) show_Congratulations();
            }

            else if(on) //ne spremamo
            {
                cell.Style.Font = new Font("Calibri", 13F);
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
            else if (cellnumber == 16 && matrica16.Cast<string>().SequenceEqual(gmatrica16.Cast<string>())) return true;
            else return false;
        }

        //čestitke ako korisnik pobijedi
        private void show_Congratulations()
        {
            clear_All();
            this.label1.Visible = false;

            congratsp = new PictureBox();
            congratsp.Name = "congratsp";
            congratsp.Size = new Size(800, 533);
            congratsp.Location = new Point(this.label1.Location.X - 200, this.label1.Location.Y + 50);
            congratsp.Image = Properties.Resources.fireworks;
            this.Controls.Add(congratsp);

            congratsl = new Label();
            congratsl.Name = "congratsl";
            congratsl.Text = "CONGRATULATIONS,\nYOU WON!";
            congratsl.Location = new Point(this.label1.Location.X, 70);
            congratsl.Size = this.button1.Size;
            congratsl.AutoSize = true;
            congratsl.BackColor = Color.Black;
            congratsl.ForeColor = Color.DarkRed;
            congratsl.Font = new Font("Algerian", 32);
            congratsl.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(congratsl);

            congratst = new Label();
            congratst.Name = "congratst";
            TimeSpan timespan = TimeSpan.FromMilliseconds(timeElapsed);
            congratst.Text = "YOUR TIME: " + timespan.ToString(@"mm\:ss");
            congratst.Location = new Point(congratsp.Location.X, congratsp.Location.Y + congratsp.Size.Height + 10);
            congratst.Size = this.button1.Size;
            congratst.AutoSize = true;
            congratst.BackColor = Color.Black;
            congratst.ForeColor = Color.DarkRed;
            congratst.Font = new Font("Algerian", 32);
            congratst.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(congratst);
        }

        //generiranje igre
        private void generate_sudoku9(object sender) //sender nam treba da vidimo koje je težine
        {
<<<<<<< HEAD
            // Koristimo bolji algoritam od klasičnog koji backtrackingom puni celiju po celiju redom
            // Ovdje se popune dijagonalni kvadrati, a zatim ostatak rekurzivno
            generateDiagonals9();
            generateRemaining9(0, 3);
=======
>>>>>>> 157f95dce327af0d2e75be986c431e21383000db

        }

        private void generate_sudoku16()
        {

        }

        private void generateDiagonals9()
        {
            for(int i = 0; i < 9; i = i + 3)
                fillSquare9(i, i);
        }

        private void fillSquare9(int row, int column)
        {
            int num;
            for(int i = 0; i < 3; ++i)
            {
                for(int j = 0; j < 3; ++j)
                {
                    do
                    {
                        Random rnd = new Random();
                        num = rnd.Next(1, 10);
                    }
                    while (valueInSquare9(num, row, column));

                    gmatrica9[row + i, column + j] = num;
                }
            }
        }
        
        private bool generateRemaining9(int i, int j)
        {
            if(j >= 9 && i < 8)
            {
                ++i;
                j = 0;
            }
            if (i >= 9 && j >= 9)
                return true;

            if(i < 3)
            {
                if (j < 3)
                    j = 3;
            }
            else if(i < 6)
            {
                if (j == (int)(i / 3) * 3)
                    j = j + 3;
            }
            else
            {
                if(j == 6)
                {
                    i = i + 1;
                    j = 0;
                    if (i >= 9)
                        return true;
                }
            }

            for(int num = 1; num <= 9; ++num)
            {
                if(!valueInRow9(num, i) && !valueInColumn9(num, j) && !valueInSquare9(num, i, j))
                {
                    gmatrica9[i, j] = num;
                    if (generateRemaining9(i, j + 1))
                        return true;

                    gmatrica9[i, j] = 0;
                }
            }

            return false;
        }
        private bool valueInRow9(int val, int r)
        {
            int column;

            for(column = 0; column < 9; ++column)
                if (gmatrica9[r, column] == val)
                    return true;

            return false;
        }

        private bool valueInColumn9(int val, int c)
        {
            int row;

            for (row = 0; row < 9; ++row)
                if (gmatrica9[row, c] == val)
                    return true;

            return false;
        }

        private bool valueInSquare9(int val, int r, int c)
        {
            int i, j;

            if(r < 3 && c < 3) // Prvi kvadrat
            {
                for (i = 0; i < 3; ++i)
                    for (j = 0; j < 3; ++j)
                        if (gmatrica9[i, j] == val)
                            return true;
            }
            else if(r < 3 && c >= 3 && c < 6) // Drugi kvadrat
            {
                for (i = 0; i < 3; ++i)
                    for (j = 3; j < 6; ++j)
                        if (gmatrica9[i, j] == val)
                            return true;
            }
            else if (r < 3 && c >= 6 && c < 9) // Treci kvadrat
            {
                for (i = 0; i < 3; ++i)
                    for (j = 6; j < 9; ++j)
                        if (gmatrica9[i, j] == val)
                            return true;
            }
            else if (r >= 3 && r < 6 && c < 3) // Cetvrti kvadrat
            {
                for (i = 3; i < 6; ++i)
                    for (j = 0; j < 3; ++j)
                        if (gmatrica9[i, j] == val)
                            return true;
            }
            else if (r >= 3 && r < 6 && c >= 3 && c < 6) // Peti kvadrat
            {
                for (i = 3; i < 6; ++i)
                    for (j = 3; j < 6; ++j)
                        if (gmatrica9[i, j] == val)
                            return true;
            }
            else if (r >= 3 && r < 6 && c >= 6 && c < 9) // Sesti kvadrat
            {
                for (i = 3; i < 6; ++i)
                    for (j = 6; j < 9; ++j)
                        if (gmatrica9[i, j] == val)
                            return true;
            }
            else if (r >= 6 && r < 9 && c < 3) // Sedmi kvadrat
            {
                for (i = 6; i < 9; ++i)
                    for (j = 0; j < 3; ++j)
                        if (gmatrica9[i, j] == val)
                            return true;
            }
            else if (r >= 6 && r < 9 && c >= 3 && c < 6) // Osmi kvadrat
            {
                for (i = 6; i < 9; ++i)
                    for (j = 3; j < 6; ++j)
                        if (gmatrica9[i, j] == val)
                            return true;
            }
            else if (r >= 6 && r < 9 && c >= 6 && c < 9) // Deveti kvadrat
            {
                for (i = 6; i < 9; ++i)
                    for (j = 6; j < 9; ++j)
                        if (gmatrica9[i, j] == val)
                            return true;
            }

            return false;
        }

        private bool matrixFull9()
        {
            for (int i = 0; i < 9; ++i)
                for (int j = 0; j < 9; ++j)
                    if (gmatrica9[i, j] == 0)
                        return false;

            return true;
        }
    }
}
