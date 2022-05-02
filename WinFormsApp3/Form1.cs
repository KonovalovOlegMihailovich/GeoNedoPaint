using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp3
{
    public partial class Form1 : Form
    {
        private Dictionary<Button, ClickHander> buttons;
        private Dictionary<ClickHander, Dictionary <Button, method>> methods;
        private delegate void method();
        private delegate void ClickHander(MouseEventArgs e);
        method lm = () => { };
        ClickHander figure = Zero; // Если ничего не выбрано, то ничего не делается =D
        private Graphics g;
        private Pen p = new Pen(Color.Black, 2f);
        private SolidBrush s = new SolidBrush(Color.AliceBlue); 
        private Pen p2 = new Pen(Color.Black, 1.5f);
        private Point[] points;

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e) => points[0] = new Point(e.X, e.Y);

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        { 
            int minx,maxx,miny,maxy;
            minx = Math.Min(points[0].X, e.X);
            miny = Math.Min(points[0].Y, e.Y);
            maxx = Math.Max(points[0].X, e.X);
            maxy = Math.Max(points[0].Y, e.Y);
            points[0] = new Point(minx, miny);
            points[1] = new Point(minx, maxy);
            points[2] = new Point(maxx, maxy);
            points[3] = new Point(maxx, miny);
            points[4] = points[0];
            figure(e);
            lm();
            points = new Point[points.Length];
        }

        private void button12_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
        }

        private void button13_Click(object sender, EventArgs e) => Application.Exit();

        private void Rectangle(MouseEventArgs e) // Прямоугольник
        {
            g.DrawPolygon(p, points);
        }

        private void Ellipse(MouseEventArgs e) // Эллипс
        {
            int width = points[0].len(points[3]);
            int height = points[0].len(points[1]);
            Point zero = new Point(Math.Min(points[0].X, points[2].X), Math.Min(points[0].Y,points[2].Y));
            g.DrawEllipse(p, zero.X, zero.Y, width, height);
        }

        private void Triangle(MouseEventArgs e) // Триугольник
        {
            Point[] P = (Point []) points.Clone();
            P[0] = points[0].middle(points[3]);
            P[3] = P[0];
            P[4] = P[0];
            g.DrawLines(p, P);
        }

        private void Rhombus(MouseEventArgs e) // Ромб
        {
            Point []k = new Point[5];
            for (int i = 0; i < 5; i++)
            {
                k[i] = points[i % 4].middle(points[(i + 1) % 4]);
            }
            points = k;
            g.DrawLines(p, k);
        }

        private void Trapezoid(MouseEventArgs e) // Трапецию со своей точки зрения я вижу только равнобокой =)
        {
            int l = points[0].len(points[3]) / 4;
            points[0].X += l;
            points[3].X -= l;
            points[4] = points[0];
            g.DrawLines(p, points);
        }
        
        private static void Zero(MouseEventArgs e) { }

        private void buttons_Click(object sender, EventArgs e) // Действие для нажатия любой кнопки отвечающей за фигуры
        {
            lm = () => { };
            foreach (Button el in methods[figure].Keys)
                el.Enabled = false;
            figure = buttons[(Button) sender];
            foreach (Button el in methods[figure].Keys)
                el.Enabled = true;
        }

        private int t = 3;

        private void vhatching_Rectangle()
        {
            Point a, b;
            a = new Point(points[0].X, points[0].Y);
            b = new Point(points[1].X, points[1].Y - 1);
            while(a.X <= points[3].X)
            {
                g.DrawLine(p2, a, b);
                a.X += t;
                b.X += t;
            }
        }

        private void hhatching_Rectangle()
        {
            Point a, b;
            a = new Point(points[0].X, points[0].Y);
            b = new Point(points[3].X, points[3].Y);
            while (a.Y <= points[2].Y)
            {
                g.DrawLine(p2, a, b);
                a.Y +=t;
                b.Y += t;
            }
        }

        private void fill_Rectangle() => g.FillPolygon(s, points);

        private void fill_Ellipse()
        {
            int width = points[0].len(points[3]);
            int height = points[0].len(points[1]);
            g.FillEllipse(s, points[0].X, points[0].Y, width, height);
        }
        
        private void fill_lines()
        {
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddLines(points);   
            g.FillPath(s, path);
        }

        private void clip_Ellipse()
        {
            Region rgn = new Region(new Rectangle(points[0].X, points[0].Y, points[0].len(points[3]) + 1, points[0].len(points[1]) + 1));
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            int width = points[0].len(points[3]);
            int height = points[0].len(points[1]);
            path.AddEllipse(points[0].X, points[0].Y, width, height);
            rgn.Exclude(path);
            g.FillRegion(Brushes.White, rgn);
        }

        private void vhatching_Ellipse()
        {
            vhatching_Rectangle();
            clip_Ellipse();
        }

        private void hhatching_Ellipse()
        {
            hhatching_Rectangle();
            clip_Ellipse();
        }

        public Form1()
        {
            InitializeComponent();
            g = pictureBox1.CreateGraphics();
            points = new Point[5];
            Rectangle s = Screen.PrimaryScreen.Bounds;
            Size = new Size(s.Width, s.Height);
            buttons = new Dictionary<Button, ClickHander>
            {
                [button1] = this.Ellipse,
                [button3] = this.Triangle,
                [button4] = this.Rectangle,
                [button5] = this.Rhombus,
                [button6] = Zero,
                [button7] = this.Trapezoid
            };
            methods = new Dictionary<ClickHander, Dictionary<Button, method>>()
            {
                [Ellipse] = new Dictionary<Button, method>()
                {
                    [button2] = () => { },
                    [button8] = hhatching_Ellipse,
                    [button9] = vhatching_Ellipse,
                    [button10] = fill_Ellipse
                },
                [Triangle] = new Dictionary<Button, method>()
                {
                    [button2] = () => { },
                    [button10] = fill_lines
                },
                [Rectangle] = new Dictionary<Button, method>()
                {
                    [button2] = () => { },
                    [button8] = hhatching_Rectangle,
                    [button9] = vhatching_Rectangle,
                    [button10] = fill_Rectangle
                },
                [Rhombus] = new Dictionary<Button, method>()
                {
                    [button2] = () => { },
                    [button10] = fill_lines
                },
                [Trapezoid] = new Dictionary<Button, method>()
                {
                    [button2] = () => { },
                    [button10] = fill_lines
                },
                [Zero] = new Dictionary<Button, method>() { }
            };
            
        }

        private void button2_Click(object sender, EventArgs e) => lm = methods[figure][(Button)sender];

        private void button6_Click(object sender, EventArgs e)
        {
            foreach (Button el in methods[Rectangle].Keys)
                el.Enabled = false;
            figure = Zero;
            lm = () => { };
        }
    }
    public static class ext
    {
        // функция для нахождения точки посереди отрезка p1-p2
        public static Point middle(this Point p1, Point p2) => new Point((int)((p1.X + p2.X) / 2), (int)((p1.Y + p2.Y) / 2));
        // функция для нахождения длины отрезка p1-p2
        public static int len(this Point p1, Point p2) => (int)(Math.Sqrt((double)(Math.Pow((p1.X - p2.X), 2) + Math.Pow((p1.Y - p2.Y), 2))));
    }
}
