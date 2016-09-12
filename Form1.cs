using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        Form2 f2 = new Form2();
        Form3 f3 = new Form3();
        Form4 f4 = new Form4();

        List<Circle> circles = new List<Circle>(); //массив для хранения вершин
        List<Line> lines = new List<Line>(); // массив для хранения ребр
        List<Line> redlines = new List<Line>(); // массив для хранения красных линий
        List<Line> lines2 = new List<Line>();

        Circle circletomove = null; // вершина для перемещения
        Circle FirstPoint = null; // для рисования линий, первая вершина
        Circle SecondPoint = null;
        Circle ThirdPoint = null;
        Circle FourthPoint = null;

        int[] stack = new int[50];
        int CirclesCount;

        int Counter = 1; // cчетчик для номеров вершин

        bool CanMove = false; // для перемещения
        bool Remove = false; // для удаления
        bool DrawCircle = false;// рисование вершин
        bool DrawLines = false; // рисование линий
        bool ContinueMove = false;
        bool YouCanSelect = false;// задать длину
        bool WeHaveIt = false; // для нахождения нужной вершины, для перемещения
        bool Min = false;
        bool Max = false;

        Graphics g;
        Graphics h;
        Pen p = new Pen(Color.Black, 2);
        Pen p1 = new Pen(Color.Black, 1);
        Pen pen = new Pen(Color.Red, 3);
        Font f = new Font("Arial", 10);

        public Form1()
        {
            InitializeComponent();
            h = this.pictureBox1.CreateGraphics();
            h.SmoothingMode = SmoothingMode.HighQuality;
        }

        private void reColor() //обнуление цветов
        {
            toolStripButton1.BackColor = toolStrip1.BackColor;
            toolStripButton2.BackColor = toolStrip1.BackColor;
            toolStripButton3.BackColor = toolStrip1.BackColor;
            toolStripButton4.BackColor = toolStrip1.BackColor;
            toolStripButton5.BackColor = toolStrip1.BackColor;
            toolStripButton7.BackColor = toolStrip1.BackColor;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            SolidBrush b = new SolidBrush(colorDialog1.Color);
            g.Clear(pictureBox1.BackColor);
            foreach (Line l in lines)
            {
                g.DrawLine(p, l.P1.X, l.P1.Y, l.P2.X, l.P2.Y);
                int h = (l.P1.X + l.P2.X) / 2;
                int h2 = (l.P1.Y + l.P2.Y) / 2;
                g.DrawString(Convert.ToString(l.D), f, Brushes.Black, h, h2 - 20);

            }
            if (Min || Max)
            {
                foreach (Line l2 in redlines)
                {
                    g.DrawLine(pen, l2.P1.X, l2.P1.Y, l2.P2.X, l2.P2.Y);
                }
            }
            foreach (Circle c in circles)
            {
                Rectangle rect = new Rectangle(c.X - c.R / 2, c.Y - c.R / 2, c.R,c.R);
                g.FillEllipse(b, rect);
                g.DrawEllipse(p1, rect);
                if (c.N < 10)
                    g.DrawString(Convert.ToString(c.N), f, Brushes.Black, c.X - 5, c.Y - 7);
                else g.DrawString(Convert.ToString(c.N), f, Brushes.Black, c.X - 9, c.Y - 7);
            }
        }

        private void table()
        {
            dataGridView1.Columns.Add(new DataGridViewColumn(new DataGridViewTextBoxCell()));
            dataGridView1.Rows.Add();
            dataGridView1.RowValidating += new DataGridViewCellCancelEventHandler(dataGridView1_RowValidating);
            dataGridView2.Columns.Add(new DataGridViewColumn(new DataGridViewTextBoxCell()));
            dataGridView2.Rows.Add();
            dataGridView2.RowValidating += new DataGridViewCellCancelEventHandler(dataGridView2_RowValidating);
            dataGridView3.Columns.Add(new DataGridViewColumn(new DataGridViewTextBoxCell()));
            dataGridView3.Rows.Add();
            dataGridView3.RowValidating += new DataGridViewCellCancelEventHandler(dataGridView3_RowValidating);
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Height = 30;
                dataGridView1.Columns[i].Width = 30;
                dataGridView2.Rows[i].Height = 30;
                dataGridView2.Columns[i].Width = 30;
                dataGridView3.Rows[i].Height = 30;
                dataGridView3.Columns[i].Width = 30;
            }
        }


        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            toolStripLabel1.Text = "( x = " + Convert.ToString(e.X) + ", " + "y = " + Convert.ToString(e.Y) + " )";

            if ((WeHaveIt) && (circletomove != null) && (CanMove) && (ContinueMove == false)) // перемещение вершины
            {
                circletomove.X = e.X;
                circletomove.Y = e.Y;
                if (e.X < 15) circletomove.X = 15;
                if (e.Y < 15) circletomove.Y = 15;
                if (e.X > 844) circletomove.X = 825;
                if (e.Y > 505) circletomove.Y = 465;
                if (e.X > 515 && e.Y < 65) circletomove.X = 500;
                pictureBox1.Refresh();
                label1.Text = "";
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (DrawCircle) //рисование вершины
            {
                Circle c = new Circle(e.X, e.Y, 30, Counter);
                circles.Add(c);
                table();
                pictureBox1.Refresh();
                Counter++;
                label1.Text = "";
            }
            else if (DrawLines) // рисование линии
            {
                toolStripLabel2.Text = "Выберите первую вершину";
                label1.Text = "";
                if (FirstPoint == null)
                {
                    foreach (Circle c1 in circles)
                    {
                        if ((e.X < c1.X + (c1.R / 2)) && (e.X > c1.X - (c1.R / 2)) &&
                            (e.Y < c1.Y + (c1.R / 2)) && (e.Y > c1.Y - (c1.R / 2)))
                        {
                            FirstPoint = c1; // присваивание линии центр окружности, как первую точку\
                            toolStripLabel2.Text = "Выберите вторую вершину";
                            Pen p = new Pen(Color.Magenta, 3);
                            h.DrawEllipse(p, c1.X - c1.R / 2, c1.Y - c1.R / 2, c1.R, c1.R);
                            break;
                        }
                    }
                }
                else
                {
                    foreach (Circle c1 in circles)
                    {
                        if ((e.X < c1.X + (c1.R / 2)) && (e.X > c1.X - (c1.R / 2)) && (e.Y < c1.Y + (c1.R / 2)) && (e.Y > c1.Y - (c1.R / 2)))
                        {
                            if (FirstPoint != c1)
                            {
                                bool WeHaveLine = false;
                                foreach (Line l in lines) // проверка на существование, чтобы не рисовать более одной линии между двумя окружностями
                                {
                                    if ((l.P1 == FirstPoint && l.P2 == c1) || (l.P1 == c1 && l.P2 == FirstPoint))
                                    {
                                        WeHaveLine = true;
                                        break;
                                    }
                                }
                                if (!WeHaveLine)
                                {
                                    lines.Add(new Line(FirstPoint, c1, 1));
                                 
                                    pictureBox1.Refresh();
                                    break;
                                }
                            }
                            FirstPoint = null;
                        }
                    }
                    FirstPoint = null;
                    pictureBox1.Refresh();
                    label1.Text = "";
                }
                minWay();
            }
            else if (Remove) // удаление вершины
            {
                for (int i = 0; i < circles.Count; i++)
                {
                    Circle c1 = circles[i];
                    if ((e.X < c1.X + (c1.R / 2)) && (e.X > c1.X - (c1.R / 2)))
                        if ((e.Y < c1.Y + (c1.R / 2)) && (e.Y > c1.Y - (c1.R / 2)))
                        {
                            int j = 0;
                            while (j != lines.Count)
                            {
                                Line l1 = lines[j];
                                if ((l1.P1 == circles[i]) || (l1.P2 == circles[i]))
                                {
                                    lines.RemoveAt(j);
                                }
                                else
                                {
                                    j++;
                                }
                            }
                            circles.RemoveAt(i);
                            dataGridView1.Rows.RemoveAt(i);
                            dataGridView1.Columns.RemoveAt(i);
                            dataGridView2.Rows.RemoveAt(i);
                            dataGridView2.Columns.RemoveAt(i);
                            dataGridView3.Rows.RemoveAt(i);
                            dataGridView3.Columns.RemoveAt(i);
                        }

                }
                for (int j = 0; j < circles.Count; j++) // перестановка цифр при удалении вершин
                {
                    Circle c2 = circles[j];
                            foreach (Circle c3 in circles)
                            {
                                if (c2.N - 1 > j)
                                {
                                    c2.N--;
                                }
                            }
                }
                pictureBox1.Refresh();
                Counter -= 1;
                if (Counter == 0) Counter = 1;
                minWay();
            }
            else if (YouCanSelect) // Задаваие грани длины
            {
                toolStripLabel2.Text = "Выберите первую вершину";
                label1.Text = "";
                if (FirstPoint == null)
                {
                    foreach (Circle c1 in circles)
                    {
                        if ((e.X < c1.X + (c1.R / 2)) && (e.X > c1.X - (c1.R / 2)) &&
                            (e.Y < c1.Y + (c1.R / 2)) && (e.Y > c1.Y - (c1.R / 2)))
                        {
                            toolStripLabel2.Text = "Выберите вторую вершину";
                            FirstPoint = c1;
                            Pen p = new Pen(Color.ForestGreen, 3);
                            h.DrawEllipse(p, c1.X - c1.R / 2, c1.Y - c1.R / 2, c1.R, c1.R);
                        }
                    }
                }
                else
                {
                    foreach (Circle c1 in circles)
                    {
                        if ((e.X < c1.X + (c1.R / 2)) && (e.X > c1.X - (c1.R / 2)) && (e.Y < c1.Y + (c1.R / 2)) && (e.Y > c1.Y - (c1.R / 2)))
                        {
                            if ((FirstPoint != c1))
                            {
                                foreach (Line l in lines)
                                {
                                    if ((l.P1 == FirstPoint && l.P2 == c1) || (l.P1 == c1 && l.P2 == FirstPoint))
                                    {
                                        f2.ShowDialog();
                                        try
                                        {
                                            if (Convert.ToInt32(f2.textBox1.Text) > 0 && Convert.ToInt32(f2.textBox1.Text) < 99999) 
                                           {
                                               l.D = Convert.ToInt32(f2.textBox1.Text);
                                           }
                                        }
                                        catch { }
                                    }
                                }
                                pictureBox1.Refresh();
                            }
                            FirstPoint = null;
                        }
                    }
                    FirstPoint = null;
                    f2.textBox1.Text = Convert.ToString(1);
                    pictureBox1.Refresh();
                    minWay(); 
                }
            }
            else if (Min)
            {
                toolStripLabel2.Text = "Выберите первую вершину";
                if (FirstPoint == null)
                {
                    foreach (Circle c1 in circles)
                    {
                        if ((e.X < c1.X + (c1.R / 2)) && (e.X > c1.X - (c1.R / 2)) &&
                            (e.Y < c1.Y + (c1.R / 2)) && (e.Y > c1.Y - (c1.R / 2)))
                        {
                            FirstPoint = c1; // присваивание линии центр окружности, как первую точку
                            toolStripLabel2.Text = "Выберите вторую вершину";
                            Pen p = new Pen(Color.DarkRed, 3);
                            h.DrawEllipse(p, c1.X - c1.R / 2, c1.Y - c1.R / 2, c1.R, c1.R);
                            
                        }
                    }
                }
                else
                {
                    foreach (Circle c1 in circles)
                    {
                        if ((e.X < c1.X + (c1.R / 2)) && (e.X > c1.X - (c1.R / 2)) && (e.Y < c1.Y + (c1.R / 2)) && (e.Y > c1.Y - (c1.R / 2)))
                        {
                            if (FirstPoint != c1)
                            {
                                SecondPoint = c1;
                                FourthPoint = SecondPoint;
                                int n = circles.Count;
                                int[,] A = new int[n, n];
                                int[,] D = new int[n, n];
                                int[,] B = new int[n, n];
                                int[,] C = new int[n, n];
                                for (int i = 0; i < n; i++)
                                {
                                    for (int j = 0; j < n; j++)
                                    {
                                        for (int k = 0; k < lines.Count; k++)
                                        {
                                            if (i == j || (circles[i] != lines[k].P1 && circles[j] != lines[k].P1) || (circles[i] != lines[k].P2 && circles[j] != lines[k].P2))
                                            {
                                                A[i, j] = 0;
                                            }
                                            else
                                            {
                                                A[i, j] = lines[k].D;
                                                D[i, j] = A[i, j];

                                            }
                                        }
                                    }
                                }
                                for (int i = 0; i < n; i++)
                                {
                                    for (int j = 0; j < n; j++)
                                    {
                                        for (int k = 0; k < lines.Count; k++)
                                        {
                                            if ((i != j) && (D[i, j] == 0))
                                            {
                                                D[i, j] = 99999;
                                            }
                                            else
                                            {
                                                B[i, j] = D[i, j];
                                                C[i, j] = i + 1;
                                            }
                                        }
                                    }
                                }
                                for (int k = 0; k < circles.Count; k++)
                                {
                                    for (int i = 0; i < circles.Count; i++)
                                    {
                                        for (int j = 0; j < circles.Count; j++)
                                        {
                                            if (i == j)
                                            {
                                                C[i, j] = 0;
                                            }
                                            if (B[i, j] > B[i, k] + B[k, j])
                                            {
                                                C[i, j] = C[k, j];
                                            }
                                            B[i, j] = Math.Min(B[i, j], B[i, k] + B[k, j]);
                                        }
                                    }
                                }
                                for (int i = 0; i < circles.Count; i++)
                                {
                                    for (int j = 0; j < circles.Count; j++)
                                    {
                                        try
                                        {
                                            int fp = FirstPoint.N - 1;
                                            int sp = FourthPoint.N - 1;
                                            string q = Convert.ToString(B[fp, SecondPoint.N - 1]);
                                            if (Convert.ToString(B[fp, SecondPoint.N - 1]) == Convert.ToString(99999) || Convert.ToString(B[fp, SecondPoint.N - 1]) == Convert.ToString(0))
                                            {
                                                label1.Text = "Кратчайший путь из вершины " + FirstPoint.N + " в вершину " + SecondPoint.N + " имеет длину " + "0" + "\nВозможно между выбранными вершинами нету пути.";
                                            }
                                            else
                                            {
                                                label1.Text = "Кратчайший путь из вершины " + FirstPoint.N + " в вершину " + SecondPoint.N + " имеет длину " + q;
                                            }
                                            ThirdPoint = circles[C[fp, sp] - 1];
                                        }
                                        catch { };
                                    }
                                    foreach (Line l in lines)
                                    {
                                        if ((l.P1 == FourthPoint && l.P2 == ThirdPoint) || (l.P1 == ThirdPoint && l.P2 == FourthPoint))
                                        {
                                            redlines.Add(new Line(FourthPoint, ThirdPoint, l.D));
                                        }
                                    }
                                    FourthPoint = ThirdPoint;
                                }

                                FirstPoint = null;
                                SecondPoint = null;
                                pictureBox1.Refresh();
                                redlines.Clear();
                            }
                        }
                    }
                    FirstPoint = null;
                    SecondPoint = null;
                }
            }
            else if (Max)
            {
                if (FirstPoint == null)
                {
                    foreach (Circle c1 in circles)
                    {
                        if ((e.X < c1.X + (c1.R / 2)) && (e.X > c1.X - (c1.R / 2)) &&
                            (e.Y < c1.Y + (c1.R / 2)) && (e.Y > c1.Y - (c1.R / 2)))
                        {
                            FirstPoint = c1; // присваивание линии центр окружности, как первую точку\
                            toolStripLabel2.Text = "Выберите вторую вершину";
                            Pen p = new Pen(Color.Magenta, 3);
                            h.DrawEllipse(p, c1.X - c1.R / 2, c1.Y - c1.R / 2, c1.R, c1.R);
                            break;
                        }
                    }
                }
                else
                {
                    foreach (Circle c1 in circles)
                    {
                        if ((e.X < c1.X + (c1.R / 2)) && (e.X > c1.X - (c1.R / 2)) && (e.Y < c1.Y + (c1.R / 2)) && (e.Y > c1.Y - (c1.R / 2)))
                        {
                            if (FirstPoint != c1)
                            {
                                SecondPoint = c1;
                                    MaxWay();
                            }
                            FirstPoint = null;
                        }
                    }
                    FirstPoint = null;
                    pictureBox1.Refresh();
                    redlines.Clear();
                }
            }
        }
             


        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            ContinueMove = false;
            if (CanMove)
            {
                WeHaveIt = false;
                foreach (Circle c1 in circles)
                {
                    if ((e.X < c1.X + (c1.R / 2)) && (e.X > c1.X - (c1.R / 2)))
                        if ((e.Y < c1.Y + (c1.R / 2)) && (e.Y > c1.Y - (c1.R / 2)))
                        {
                            circletomove = c1;
                            WeHaveIt = true;
                        }
                }
            }
             
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            ContinueMove = true;
        }

        private void toolStripButton1_Click(object sender, EventArgs e) // создать вершину
        {
            DrawCircle = true;
            CanMove = false;
            YouCanSelect = false;
            DrawLines = false;
            WeHaveIt = false;
            Remove = false;
            Min = false;
            Max = false;
            reColor();
            toolStripButton1.BackColor = Color.BlueViolet;
            toolStripLabel2.Text = "";
            label1.Text = "";
        }

        private void toolStripButton2_Click(object sender, EventArgs e) // очистить
        {
            Counter = 1;
            DrawCircle = true;
            YouCanSelect = false;
            DrawLines = false;
            Remove = false;
            CanMove = false;
            Min = false;
            Max = false;
            FirstPoint = null;
            lines = new List<Line>();
            circles = new List<Circle>();
            reColor();
            toolStripButton1.BackColor = Color.BlueViolet;
            toolStripLabel2.Text = "";
            label1.Text = "";
            pictureBox1.Refresh();
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView2.Rows.Clear();
            dataGridView2.Columns.Clear();
            dataGridView3.Rows.Clear();
            dataGridView3.Columns.Clear();
        }

        private void toolStripButton3_Click(object sender, EventArgs e) // создать грань
        {
            DrawLines = true;
            Remove = false;
            CanMove = false;
            YouCanSelect = false;
            DrawCircle = false;
            Min = false;
            Max = false;
            reColor();
            toolStripButton3.BackColor = Color.BlueViolet;
            toolStripLabel2.Text = "Выберите первую вершину";

        }

        private void toolStripButton4_Click(object sender, EventArgs e) // переместить вершину
        {
            CanMove = true;
            DrawCircle = false;
            DrawLines = false;
            YouCanSelect = false;
            Min = false;
            Max = false;
            Remove = false;
            WeHaveIt = false;
            reColor();
            toolStripButton4.BackColor = Color.BlueViolet;
            toolStripLabel2.Text = "";

        }

        private void toolStripButton5_Click(object sender, EventArgs e) // удалить вершину
        {
            DrawCircle = false;
            DrawLines = false;
            YouCanSelect = false;
            Min = false;
            Max = false;
            Remove = true;
            CanMove = false;
            WeHaveIt = false;
            reColor();
            toolStripButton5.BackColor = Color.BlueViolet;
            toolStripLabel2.Text = "";

        }

        private void toolStripButton6_Click(object sender, EventArgs e) // выбрать цвет
        {
            DialogResult result = colorDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                toolStripButton6.BackColor = colorDialog1.Color;
            }
            pictureBox1.Refresh();
            toolStripLabel2.Text = "";

        }

        private void toolStripButton7_Click(object sender, EventArgs e) // длина грани
        {
            YouCanSelect = true;
            CanMove = false;
            DrawCircle = false;
            Min = false;
            Max = false;
            DrawLines = false;
            WeHaveIt = false;
            Remove = false;
            reColor();
            toolStripButton7.BackColor = Color.BlueViolet;
            toolStripLabel2.Text = "Выберите первую вершину";

        }


        private void кратчайшийПутьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Min = true;
            Max = false;
            reColor();
            toolStripLabel2.Text = "Выберите первую вершину";
            YouCanSelect = false;
            CanMove = false;
            DrawCircle = false;
            DrawLines = false;
            Remove = false;
        }
        private void критическийПутьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Min = false;
            Max = true;
            reColor();
            toolStripLabel2.Text = "Выберите первую вершину";
            YouCanSelect = false;
            CanMove = false;
            DrawCircle = false;
            DrawLines = false;
            Remove = false;
        }


        private void dataGridView1_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            dataGridView1.Rows[e.RowIndex].HeaderCell.Value = e.RowIndex.ToString();
            dataGridView1.Columns[e.ColumnIndex].HeaderText = e.ColumnIndex.ToString();
        }
        private void dataGridView2_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            dataGridView2.Rows[e.RowIndex].HeaderCell.Value = e.RowIndex.ToString();
            dataGridView2.Columns[e.ColumnIndex].HeaderText = e.ColumnIndex.ToString();
        }

        private void dataGridView3_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            dataGridView3.Rows[e.RowIndex].HeaderCell.Value = e.RowIndex.ToString();
            dataGridView3.Columns[e.ColumnIndex].HeaderText = e.ColumnIndex.ToString();
        }

        private void minWay()
        {
            int n = circles.Count;
            int[,] A = new int[n, n];
            int[,] D = new int[n, n];
            int[,] B = new int[n, n];
            int[,] C = new int[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    for (int k = 0; k < lines.Count; k++)
                    {
                        if (i == j || (circles[i] != lines[k].P1 && circles[j] != lines[k].P1) || (circles[i] != lines[k].P2 && circles[j] != lines[k].P2))
                        {
                            A[i, j] = 0;
                        }
                        else
                        {
                            A[i, j] = lines[k].D;
                            D[i, j] = A[i, j];

                        }
                    }
                }
            }
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    for (int k = 0; k < lines.Count; k++)
                    {
                        if ((i != j) && (D[i, j] == 0))
                        {
                            D[i, j] = 99999;
                        }
                        else
                        {
                            dataGridView1[i, j].Value = D[i, j];

                            if (Convert.ToInt32(dataGridView1[i, j].Value) == 99999)
                            {
                                dataGridView1[i, j].Value = "∞";
                                this.dataGridView1.DefaultCellStyle.Font = new Font("Tahoma", 10);
                            }
                            B[i, j] = D[i, j];
                            C[i, j] = i + 1;
                            dataGridView3[j, i].Value = C[i, j];
                        }
                    }
                }
            }
            for (int k = 0; k < circles.Count; k++)
            {
                for (int i = 0; i < circles.Count; i++)
                {
                    for (int j = 0; j < circles.Count; j++)
                    {
                            if (i == j)
                            {
                                C[i, j] = 0;
                            }
                            if (B[i, j] > B[i, k] + B[k, j])
                            {
                                C[i, j] = C[k, j];
                            }
                            B[i, j] = Math.Min(B[i, j], B[i, k] + B[k, j]);
                            dataGridView2[i, j].Value = B[i, j];
                            dataGridView3[j, i].Value = C[i, j];
                            if (Convert.ToInt32(dataGridView2[i, j].Value) == 99999)
                            {
                                dataGridView2[i, j].Value = "∞";
                                this.dataGridView2.DefaultCellStyle.Font = new Font("Tahoma", 10);
                            }
                    }
                }
            }
        }


        private void tabControl1_MouseClick(object sender, MouseEventArgs e)
        {
            pictureBox1.Refresh();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = System.Environment.CurrentDirectory;
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, circles);
                bf.Serialize(fs, lines);
                fs.Close();
                

            }
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = System.Environment.CurrentDirectory;
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                circles.Clear();
                lines.Clear();
                
                FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                BinaryFormatter bf = new BinaryFormatter();
                circles = (List<Circle>)bf.Deserialize(fs);
                lines = (List<Line>)bf.Deserialize(fs);
                for (int i = 0; i < circles.Count; i++)
                {
                    table();
                }
                fs.Close();
                foreach (Line l in lines)
                {
                    foreach (Circle c1 in circles)
                    {
                        if (l.P1.X == c1.X && l.P1.Y == c1.Y)
                        {
                            l.P1 = c1;
                        }
                        if (l.P2.X == c1.X && l.P2.Y == c1.Y)
                        {
                            l.P2 = c1;
                        }
                    }
                }
                minWay();
                Counter = circles.Count + 1;
                pictureBox1.Refresh();

            }
        }

        private void справкаToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            f3.ShowDialog();
        }

        private void openToolStripButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = System.Environment.CurrentDirectory;
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                circles.Clear();
                lines.Clear();

                FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                BinaryFormatter bf = new BinaryFormatter();
                circles = (List<Circle>)bf.Deserialize(fs);
                lines = (List<Line>)bf.Deserialize(fs);
                for (int i = 0; i < circles.Count; i++)
                {
                    table();
                }
                fs.Close();
                foreach (Line l in lines)
                {
                    foreach (Circle c1 in circles)
                    {
                        if (l.P1.X == c1.X && l.P1.Y == c1.Y)
                        {
                            l.P1 = c1;
                        }
                        if (l.P2.X == c1.X && l.P2.Y == c1.Y)
                        {
                            l.P2 = c1;
                        }
                    }
                }
                minWay();
                Counter = circles.Count + 1;
                pictureBox1.Refresh();

            }
        }

        private void saveToolStripButton1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = System.Environment.CurrentDirectory;
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, circles);
                bf.Serialize(fs, lines);
                fs.Close();
            }
        }

        private void обАвтореToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            f4.ShowDialog();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (circles.Count != 0)
            {
                DialogResult result = MessageBox.Show("Документ изменен. \nСохранить изменения?",
                    "Сохранение документа", MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.No)
                {
                    e.Cancel = false;
                }
                if (result == DialogResult.Yes)
                {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                    saveFileDialog1.InitialDirectory = System.Environment.CurrentDirectory;
                    saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                        BinaryFormatter bf = new BinaryFormatter();
                        bf.Serialize(fs, circles);
                        bf.Serialize(fs, lines);
                        bf.Serialize(fs, dataGridView1);
                        fs.Close();
                    }
                    if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                    }

                }
                if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        private bool next(int k) //переполнение стэка
        {
            if (stack[k] < CirclesCount)
            {
                stack[k]++;
                return true;
            }
            else return false;

        }
        private bool Check(int k) //проверка на повторение вершин (избавление от циклов)
        {
            bool xx = true;
            for (int i = 0; i < k; i++)
            {
                if (stack[i] == stack[k])
                {
                    xx = false;
                }
            }
            return  xx;

        }
        private bool solution(int k) // поверка на путь
        {
            bool xx = false;

            if (circles[stack[k]].N == FirstPoint.N && circles[stack[0]].N == SecondPoint.N)
                xx = true;
            else xx = false;
                return xx;
        }
        private bool CheckLines(Circle p1, Circle p2) //существование линий
        {
            bool xx = false;
            foreach (Line l1 in lines)
            {
                if ((l1.P1 == p1 && l1.P2 == p2) || (l1.P1 == p2 && l1.P2 == p1))
                {
                    xx = true;
                }

            }
            return xx;
        }
        private int GetLinesLength(Circle p1, Circle p2)
        {
            int a = 0;
            foreach (Line l1 in lines)
            {
                if ((l1.P1 == p1 && l1.P2 == p2) || (l1.P1 == p2 && l1.P2 == p1))
                {
                    a = l1.D;
                }

            }
            return a;
        }

        private void MaxWay()
        {
            int futurelines = 0;
            int maximum = 0;
            stack = new int[circles.Count];
            int[] cr = new int[circles.Count];
            CirclesCount = circles.Count - 1;
            int k = 0;
            stack[k] = -1;
            for (int i = 0; i < stack.Length; i++)
            {
                stack[i] = -1;
            }
            while (k >= 0)
            {
                while (next(k))
                {
                    if (Check(k))
                    {
                        if (k > 0 && CheckLines(circles[stack[k]], circles[stack[k - 1]]))
                        {
                            if (solution(k))
                            {
                                int q = 0;
                                for (int i = 0; i < k; i++)
                                {
                                    q += GetLinesLength(circles[stack[i]], circles[stack[i + 1]]);
                                }
                                if (q > maximum)
                                {
                                    maximum = q;
                                    for (int j = 0; j <= k; j++)
                                        cr[j] = stack[j];
                                    futurelines = k;
                                }
                            }
                            else
                            {
                                if (k < CirclesCount)
                                {
                                    k++;

                                    stack[k] = -1;
                                }
                            }
                        }
                        else
                        {
                            if (k == 0)
                            {
                                if (k < CirclesCount)
                                {
                                    k++;
                                    stack[k] = -1;
                                }
                            }
                        }
                    }
                }
                k--;
            }
            for (int i = 0; i < futurelines; i++)
            {
                redlines.Add(new Line(circles[cr[i]], circles[cr[i + 1]], GetLinesLength(circles[stack[i]], circles[stack[i + 1]])));
            }
            pictureBox1.Refresh();
            if (maximum == 0)
            {
                label1.Text = "Критический путь из вершины " + FirstPoint.N + " в вершину " + SecondPoint.N + " имеет длину " + "0" + "\nВозможно между выбранными вершинами нету пути.";
            }
            else
            {
                label1.Text = "Критический путь из вершины " + FirstPoint.N + " в вершину " + SecondPoint.N + " имеет длину " + maximum;
            }
            
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
        }

    }
}
