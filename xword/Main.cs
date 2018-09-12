using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Configuration;
using System.Data.SqlClient;

namespace xword
{
    public partial class Main : Form
    {
        Clues clue_window = new Clues();
        List<id_cells> idc = new List<id_cells>();

        public Main()
        {
            buildWordList();
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // TODO: Replace this with function to fecth the words from the database.
        private void buildWordList()
        {
            var databaseConnection = DBConnection.Instance();
            databaseConnection.ConnectionString = Properties.Settings.Default.connection_string;
            if (String.IsNullOrEmpty(databaseConnection.ConnectionString))
            {
                MessageBox.Show("Cannot connect to database, please set the correct connection string and start the app again.");
       
            }
            var isDbConnected = databaseConnection.IsConnected();
            if (isDbConnected)
            {
                string wordQuery = "SELECT * FROM bag_of_words";
               
                var wordQueryCommand = new SqlCommand(wordQuery, databaseConnection.Connection);
                var reader = wordQueryCommand.ExecuteReader();
                while (reader.Read())
                {
                    var positionX = reader.GetInt32(1); // x
                    var positionY = reader.GetInt32(2); // y
                    var direction = reader.GetString(3); // direction
                    var number = reader.GetInt32(4).ToString(); // number
                    var word = reader.GetString(5); // word
                    var clue = reader.IsDBNull(6) ? "" : reader.GetString(6); // clue
                    idc.Add(new id_cells(positionX, positionY, direction, number, word, clue));
                    clue_window.clue_table.Rows.Add(new String[] { number, direction, clue });
                }
            }
        }

    

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeBoard();
            clue_window.SetDesktopLocation(this.Location.X + this.Width + 1, this.Location.Y);
            clue_window.StartPosition = FormStartPosition.Manual;

            clue_window.Show();
            clue_window.clue_table.AutoResizeColumns();
        }

        private void InitializeBoard()
        {
            board.BackgroundColor = Color.Khaki;
            board.DefaultCellStyle.BackColor = Color.Khaki;

            for (int i = 0; i < 21; i++)
                board.Rows.Add();

            //set width of column
            foreach (DataGridViewColumn c in board.Columns)
                c.Width = board.Width / board.Columns.Count;

            //set width of row
            foreach (DataGridViewRow r in board.Rows)
                r.Height = board.Height / board.Rows.Count;

            for (int row = 0; row < board.Rows.Count; row++)
            {
                for (int col = 0; col < board.Columns.Count; col++)
                    board[col, row].ReadOnly = true;
            }

            foreach(id_cells i in idc)
            {
                int start_col = i.X;
                int start_row = i.Y;
                char[] word = i.word.ToCharArray();

                for (int j= 0; j < word.Length; j++)
                {
                    if (i.direction == "across")
                        formatCell(start_row, start_col + j, word[j].ToString());

                    if (i.direction == "down")
                        formatCell(start_row + j, start_col, word[j].ToString());
                }
            }
        }
        
        private void formatCell(int row, int col, string letter)
        {
            DataGridViewCell c = board[col, row];
            c.Style.BackColor = Color.White;
            Font f = new Font(FontFamily.GenericSansSerif, 12);
            c.Style.Font = f;
            c.ReadOnly = false;
            c.Style.SelectionBackColor = Color.Gray;
            c.Tag = letter;
        }

        private void Form1_LocationChanged(object sender, EventArgs e)
        {
            clue_window.SetDesktopLocation(this.Location.X + this.Width + 1, this.Location.Y);
        }

        private void board_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //truncate to one letter
            try
            {
                if (board[e.ColumnIndex, e.RowIndex].Value.ToString().Length > 1)
                    board[e.ColumnIndex, e.RowIndex].Value = board[e.ColumnIndex, e.RowIndex].Value.ToString().Substring(0, 1);
            }
            catch { }

            //format color if correct
            try
            {
                if (board[e.ColumnIndex, e.RowIndex].Value.ToString().Equals(board[e.ColumnIndex, e.RowIndex].Tag.ToString()))
                    board[e.ColumnIndex, e.RowIndex].Style.ForeColor = Color.DarkGreen;
                else
                    board[e.ColumnIndex, e.RowIndex].Style.ForeColor = Color.Red;
            }
            catch { }
        }

        private void openPuzzleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buildWordList();
            InitializeBoard();

        }

        private void board_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            String number = "";
            
            // foreach (item c in List of items)
            if (idc.Any(c => (number = c.number) != "" && c.X == e.ColumnIndex && c.Y == e.RowIndex))
            {
                Rectangle r = new Rectangle(e.CellBounds.X, e.CellBounds.Y, e.CellBounds.Width, e.CellBounds.Height);
                e.Graphics.FillRectangle(Brushes.White, r);
                Font f = new Font(e.CellStyle.Font.FontFamily, 7);
                e.Graphics.DrawString(number, f, Brushes.Black, r);
                e.PaintContent(e.ClipBounds);
                e.Handled = true;
            }
        }
    }

    public class id_cells
    {
        public int X;
        public int Y;
        public String direction;
        public String number;
        public String word;
        public String clue;

        public id_cells(int x, int y, String d, String n, String w, String c)
        {
            this.X = x;
            this.Y = y;
            this.direction = d;
            this.number = n;
            this.word = w;
            this.clue = c;
        }
    }//end class
}
