using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{

    public partial class Form_Snake : Form
    {
        public Form_Snake()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);
            ControlPaint.DrawGrid(e.Graphics, this.ClientRectangle, new Size(8, 8), Color.White);
        }

        Snake snake = null;
        private void btn_start_Click(object sender, EventArgs e)
        {
            snake = new Snake(8, this.panel1, Over);
            snake.Add(new Label());
            snake.FoodSpeed = 2*Convert.ToInt32(this.cmb_foodspeed.Text);
            timer1.Enabled = true;
            timer1.Interval = 200 * Convert.ToInt32(this.cmb_bodyspeed.Text);
            this.btn_start.Enabled = false;
            this.btn_stop.Enabled = true;
            this.btn_pause.Enabled = true;

        }
        public void Over()
        {

            this.timer1.Enabled = false;
            this.btn_stop.Enabled = false;
            this.btn_pause.Enabled = false;
            this.btn_start.Enabled = true;
            this.btn_pause.Text = "暂停";
            this.statusLabel.Text = "Game Over";
            Delay(2);
            snake.Clear();
            this.statusLabel.Text = "再来！";


        }

        public static bool Delay(int delayTime)
        {
            DateTime now = DateTime.Now;
            int s;
            do
            {
                TimeSpan spand = DateTime.Now - now;
                s = spand.Seconds;
                Application.DoEvents();
            }
            while (s < delayTime);
            return true;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)//取消方向键对控件的焦点的控件，用自己自定义的函数处理各个方向键的处理函数
        {
            switch (keyData)
            {
                case Keys.Up:
                    if (Snake.Direction!=2)
                    {
                        Snake.Direction = 1;
                    }
                    
                    return true;//不继续处理
                case Keys.Down:
                    if (Snake.Direction != 1)
                    {
                        Snake.Direction = 2;
                    }
                    return true;
                case Keys.Left:
                    if (Snake.Direction != 4)
                    {
                        Snake.Direction = 3;
                    }
                    return true;
                case Keys.Right:
                    if (Snake.Direction != 3)
                    {
                        Snake.Direction = 4;
                    }
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (snake != null)
            {
                snake.Move();
                this.statusLabel.Text = string.Format("成绩：{0},随机：X={1},Y={2}", snake.Body.Count, snake.FoodPoint.X, snake.FoodPoint.Y);
            }
        }

        private void btn_pause_Click(object sender, EventArgs e)
        {
            this.timer1.Enabled = !this.timer1.Enabled;
            if (this.timer1.Enabled)
            {
                this.btn_pause.Text = "暂停";
            }
            else
            {
                this.btn_pause.Text = "继续";
            }
        }

        private void btn_stop_Click(object sender, EventArgs e)
        {
            Over();
        }


    }

    public delegate void OverDelegate();
    public class Snake
    {
        public Snake(int between, Panel panel, OverDelegate overHandle)
        {
            this.Between = between;
            this.DrawPanel = panel;
            this.OverHandle = overHandle;

        }

        private Point _foodPoint = new Point(0, 0);
        public Point FoodPoint { 
            get { return _foodPoint; }
            set { _foodPoint = value; }
        }

        public OverDelegate OverHandle { get; set; }

        public Panel DrawPanel { get; set; }

        //方格间距
        public int Between { get; set; }

        public Color BodyColor { get; set; }

        /// <summary>
        /// 1 up 2 down 3 left 4 right
        /// </summary>
        public static int Direction { get; set; }
        public List<Control> Body { get; set; }

        public void AddFood()
        {
            long tick = DateTime.Now.Ticks;
            Random ran = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));

            if (ran.Next(100) % this.FoodSpeed == 0)
            {
                Label food = new Label();

                int xRan = ran.Next(1, this.DrawPanel.Width / this.Between) * this.Between;
                int yRan = ran.Next(1, this.DrawPanel.Height / this.Between) * this.Between;
                FoodPoint = new Point(xRan, yRan);
                var newFood = this.DrawPanel.GetChildAtPoint(FoodPoint);
                if(newFood==null)
                {
                    food.Size = new Size(this.Between, this.Between);
                    food.Location = new Point(xRan, yRan);
                    food.BackColor = Color.Blue;
                    food.BorderStyle = BorderStyle.FixedSingle;
                    

                    this.DrawPanel.Controls.Add(food);
                }

            }

        }

        public void Add(Label item)
        {
            item.BackColor = Color.Green;
            item.Size = new Size(this.Between, this.Between);


            if (Body == null)
            {
                Body = new List<Control>();
                long tick = DateTime.Now.Ticks;
                Random ran = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));

                int xRan = ran.Next(1,this.DrawPanel.Width / this.Between) * this.Between;
                int yRan = ran.Next(1,this.DrawPanel.Height / this.Between) * this.Between;

                item.Text = "*";
                item.Location = new Point(xRan, yRan);
                item.BackColor = Color.Red;
                item.BorderStyle = BorderStyle.FixedSingle;
                Snake.Direction = new Random().Next(1, 5);
            }
            else
            {
                item.Location = Body[Body.Count-1].Location;
            }

            this.Move();
            this.DrawPanel.Controls.Add(item);

            Body.Add(item);
        }

        public void Move()
        {
            AddFood();

            if (this.Body == null || this.Body.Count < 1)
            {
                return;
            }

            Point oldPoint = new Point(0, 0);
            Control food=null;
            for (int i = 0; i < this.Body.Count; i++)
            {
                Point oldPoint2 = new Point(this.Body[i].Location.X, this.Body[i].Location.Y);

                if (i==0)
                {
                    Point newPonit=new Point(0,0);

                    if (Snake.Direction == 1)//up
                    {
                        newPonit = new Point(this.Body[0].Location.X, this.Body[0].Location.Y - this.Between);
                    }
                    else if (Snake.Direction == 2)//down
                    {
                        newPonit = new Point(this.Body[0].Location.X, this.Body[0].Location.Y + this.Between);
                    }
                    else if (Snake.Direction == 3)//left
                    {
                        newPonit = new Point(this.Body[0].Location.X - this.Between, this.Body[0].Location.Y);
                    }
                    else if (Snake.Direction == 4)//right
                    {
                         newPonit= new Point(this.Body[0].Location.X + this.Between, this.Body[0].Location.Y);
                    }

                    food = this.DrawPanel.GetChildAtPoint(newPonit);
                    if (food!=null)
                    {
                        food.Location = new Point(-8, -8);
                    }

                    if (this.Body.Contains(food))
                    {
                        this.OverHandle();
                        return;
                    }

                    if (newPonit.X < 0)
                    {
                        newPonit.X += this.DrawPanel.Width / this.Between * this.Between;
                    }
                    if (newPonit.Y < 0)
                    {
                        newPonit.Y += this.DrawPanel.Height / this.Between * this.Between;
                    }
                    if (newPonit.X > (this.DrawPanel.Width - this.Between))
                    {
                        newPonit.X = 0;
                    }
                    if (newPonit.Y > (this.DrawPanel.Height - this.Between))
                    {
                        newPonit.Y = 0;
                    }

                    //移动到新位置
                    this.Body[0].Location = new Point(newPonit.X, newPonit.Y);

                }
                else
                {
                    this.Body[i].Location = new Point(oldPoint.X,oldPoint.Y);
                }
                
                oldPoint =new Point(oldPoint2.X,oldPoint2.Y);

            }//for end

            if (food != null)
            {
                food.Location = new Point(oldPoint.X, oldPoint.Y);
                food.BackColor = Color.Green;
                this.Body.Add(food);
            }
        }

        internal void Clear()
        {
            this.DrawPanel.Controls.Clear();
        }

        private int _foodSpeed = 5;
        public int FoodSpeed { 
            get { return _foodSpeed; } 
            set { _foodSpeed=value; } 
        }
    }



}
