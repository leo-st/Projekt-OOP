using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OTTER
{
    /// <summary>
    /// -
    /// </summary>
    
    public partial class BGL : Form
    {
        /* ------------------- */
        #region Environment Variables

        List<Func<int>> GreenFlagScripts = new List<Func<int>>();

        /// <summary>
        /// Uvjet izvršavanja igre. Ako je <c>START == true</c> igra će se izvršavati.
        /// </summary>
        /// <example><c>START</c> se često koristi za beskonačnu petlju. Primjer metode/skripte:
        /// <code>
        /// private int MojaMetoda()
        /// {
        ///     while(START)
        ///     {
        ///       //ovdje ide kod
        ///     }
        ///     return 0;
        /// }</code>
        /// </example>
        public static bool START = true;

        //sprites
        /// <summary>
        /// Broj likova.
        /// </summary>
        public static int spriteCount = 0, soundCount = 0;

        /// <summary>
        /// Lista svih likova.
        /// </summary>
        //public static List<Sprite> allSprites = new List<Sprite>();
        public static SpriteList<Sprite> allSprites = new SpriteList<Sprite>();

        //sensing
        int mouseX, mouseY;
        Sensing sensing = new Sensing();

        //background
        List<string> backgroundImages = new List<string>();
        int backgroundImageIndex = 0;
        string ISPIS = "";

        SoundPlayer[] sounds = new SoundPlayer[1000];
        TextReader[] readFiles = new StreamReader[1000];
        TextWriter[] writeFiles = new StreamWriter[1000];
        bool showSync = false;
        int loopcount;
        DateTime dt = new DateTime();
        String time;
        double lastTime, thisTime, diff;

        #endregion
        /* ------------------- */
        #region Events

        private void Draw(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            try
            {                
                foreach (Sprite sprite in allSprites)
                {                    
                    if (sprite != null)
                        if (sprite.Show == true)
                        {
                            g.DrawImage(sprite.CurrentCostume, new Rectangle(sprite.X, sprite.Y, sprite.Width, sprite.Heigth));
                        }
                    if (allSprites.Change)
                        break;
                }
                if (allSprites.Change)
                    allSprites.Change = false;
            }
            catch
            {
                //ako se doda sprite dok crta onda se mijenja allSprites
                MessageBox.Show("Greška!");
            }
        }

        private void startTimer(object sender, EventArgs e)
        {
            timer1.Start();
            timer2.Start();
            Init();
        }

        private void updateFrameRate(object sender, EventArgs e)
        {
            updateSyncRate();
        }

        /// <summary>
        /// Crta tekst po pozornici.
        /// </summary>
        /// <param name="sender">-</param>
        /// <param name="e">-</param>
        public void DrawTextOnScreen(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            var brush = new SolidBrush(Color.WhiteSmoke);
            string text = ISPIS;

            SizeF stringSize = new SizeF();
            Font stringFont = new Font("Arial", 14);
            stringSize = e.Graphics.MeasureString(text, stringFont);

            using (Font font1 = stringFont)
            {
                RectangleF rectF1 = new RectangleF(0, 0, stringSize.Width, stringSize.Height);
                e.Graphics.FillRectangle(brush, Rectangle.Round(rectF1));
                e.Graphics.DrawString(text, font1, Brushes.Black, rectF1);
            }
        }

        private void mouseClicked(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;
        }

        private void mouseDown(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;            
        }

        private void mouseUp(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = false;
            sensing.MouseDown = false;
        }

        private void mouseMove(object sender, MouseEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;

            //sensing.MouseX = e.X;
            //sensing.MouseY = e.Y;
            //Sensing.Mouse.x = e.X;
            //Sensing.Mouse.y = e.Y;
            sensing.Mouse.X = e.X;
            sensing.Mouse.Y = e.Y;

        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            sensing.Key = e.KeyCode.ToString();
            sensing.KeyPressedTest = true;
        }

        private void keyUp(object sender, KeyEventArgs e)
        {
            sensing.Key = "";
            sensing.KeyPressedTest = false;
        }

        private void Update(object sender, EventArgs e)
        {
            if (sensing.KeyPressed(Keys.Escape))
            {
                START = false;
            }

            if (START)
            {
                this.Refresh();
            }
        }

        #endregion
        /* ------------------- */
        #region Start of Game Methods

        //my
        #region my

        //private void StartScriptAndWait(Func<int> scriptName)
        //{
        //    Task t = Task.Factory.StartNew(scriptName);
        //    t.Wait();
        //}

        //private void StartScript(Func<int> scriptName)
        //{
        //    Task t;
        //    t = Task.Factory.StartNew(scriptName);
        //}

        private int AnimateBackground(int intervalMS)
        {
            while (START)
            {
                setBackgroundPicture(backgroundImages[backgroundImageIndex]);
                Game.WaitMS(intervalMS);
                backgroundImageIndex++;
                if (backgroundImageIndex == 3)
                    backgroundImageIndex = 0;
            }
            return 0;
        }

        private void KlikNaZastavicu()
        {
            foreach (Func<int> f in GreenFlagScripts)
            {
                Task.Factory.StartNew(f);
            }
        }

        #endregion

        /// <summary>
        /// BGL
        /// </summary>
        public BGL()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Pričekaj (pauza) u sekundama.
        /// </summary>
        /// <example>Pričekaj pola sekunde: <code>Wait(0.5);</code></example>
        /// <param name="sekunde">Realan broj.</param>
        public void Wait(double sekunde)
        {
            int ms = (int)(sekunde * 1000);
            Thread.Sleep(ms);
        }

        //private int SlucajanBroj(int min, int max)
        //{
        //    Random r = new Random();
        //    int br = r.Next(min, max + 1);
        //    return br;
        //}

        /// <summary>
        /// -
        /// </summary>
        public void Init()
        {
            if (dt == null) time = dt.TimeOfDay.ToString();
            loopcount++;
            //Load resources and level here
            this.Paint += new PaintEventHandler(DrawTextOnScreen);
            SetupGame();
        }

        /// <summary>
        /// -
        /// </summary>
        /// <param name="val">-</param>
        public void showSyncRate(bool val)
        {
            showSync = val;
            if (val == true) syncRate.Show();
            if (val == false) syncRate.Hide();
        }

        /// <summary>
        /// -
        /// </summary>
        public void updateSyncRate()
        {
            if (showSync == true)
            {
                thisTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                diff = thisTime - lastTime;
                lastTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                double fr = (1000 / diff) / 1000;

                int fr2 = Convert.ToInt32(fr);

                syncRate.Text = fr2.ToString();
            }

        }

        //stage
        #region Stage

        /// <summary>
        /// Postavi naslov pozornice.
        /// </summary>
        /// <param name="title">tekst koji će se ispisati na vrhu (naslovnoj traci).</param>
        public void SetStageTitle(string title)
        {
            this.Text = title;
        }

        /// <summary>
        /// Postavi boju pozadine.
        /// </summary>
        /// <param name="r">r</param>
        /// <param name="g">g</param>
        /// <param name="b">b</param>
        public void setBackgroundColor(int r, int g, int b)
        {
            this.BackColor = Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Postavi boju pozornice. <c>Color</c> je ugrađeni tip.
        /// </summary>
        /// <param name="color"></param>
        public void setBackgroundColor(Color color)
        {
            this.BackColor = color;
        }

        /// <summary>
        /// Postavi sliku pozornice.
        /// </summary>
        /// <param name="backgroundImage">Naziv (putanja) slike.</param>
        public void setBackgroundPicture(string backgroundImage)
        {
            this.BackgroundImage = new Bitmap(backgroundImage);
        }

        /// <summary>
        /// Izgled slike.
        /// </summary>
        /// <param name="layout">none, tile, stretch, center, zoom</param>
        public void setPictureLayout(string layout)
        {
            if (layout.ToLower() == "none") this.BackgroundImageLayout = ImageLayout.None;
            if (layout.ToLower() == "tile") this.BackgroundImageLayout = ImageLayout.Tile;
            if (layout.ToLower() == "stretch") this.BackgroundImageLayout = ImageLayout.Stretch;
            if (layout.ToLower() == "center") this.BackgroundImageLayout = ImageLayout.Center;
            if (layout.ToLower() == "zoom") this.BackgroundImageLayout = ImageLayout.Zoom;
        }

        #endregion

        //sound
        #region sound methods

        /// <summary>
        /// Učitaj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        /// <param name="file">-</param>
        public void loadSound(int soundNum, string file)
        {
            soundCount++;
            sounds[soundNum] = new SoundPlayer(file);
        }

        /// <summary>
        /// Sviraj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        public void playSound(int soundNum)
        {
            sounds[soundNum].Play();
        }

        /// <summary>
        /// loopSound
        /// </summary>
        /// <param name="soundNum">-</param>
        public void loopSound(int soundNum)
        {
            sounds[soundNum].PlayLooping();
        }

        /// <summary>
        /// Zaustavi zvuk.
        /// </summary>
        /// <param name="soundNum">broj</param>
        public void stopSound(int soundNum)
        {
            sounds[soundNum].Stop();
        }

        #endregion

        //file
        #region file methods

        /// <summary>
        /// Otvori datoteku za čitanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToRead(string fileName, int fileNum)
        {
            readFiles[fileNum] = new StreamReader(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToRead(int fileNum)
        {
            readFiles[fileNum].Close();
        }

        /// <summary>
        /// Otvori datoteku za pisanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToWrite(string fileName, int fileNum)
        {
            writeFiles[fileNum] = new StreamWriter(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToWrite(int fileNum)
        {
            writeFiles[fileNum].Close();
        }

        /// <summary>
        /// Zapiši liniju u datoteku.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <param name="line">linija</param>
        public void writeLine(int fileNum, string line)
        {
            writeFiles[fileNum].WriteLine(line);
        }

        /// <summary>
        /// Pročitaj liniju iz datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća pročitanu liniju</returns>
        public string readLine(int fileNum)
        {
            return readFiles[fileNum].ReadLine();
        }

        /// <summary>
        /// Čita sadržaj datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća sadržaj</returns>
        public string readFile(int fileNum)
        {
            return readFiles[fileNum].ReadToEnd();
        }

        #endregion

        //mouse & keys
        #region mouse methods

        /// <summary>
        /// Sakrij strelicu miša.
        /// </summary>
        public void hideMouse()
        {
            Cursor.Hide();
        }

        /// <summary>
        /// Pokaži strelicu miša.
        /// </summary>
        public void showMouse()
        {
            Cursor.Show();
        }

        /// <summary>
        /// Provjerava je li miš pritisnut.
        /// </summary>
        /// <returns>true/false</returns>
        public bool isMousePressed()
        {
            //return sensing.MouseDown;
            return sensing.MouseDown;
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">naziv tipke</param>
        /// <returns></returns>
        public bool isKeyPressed(string key)
        {
            if (sensing.Key == key)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">tipka</param>
        /// <returns>true/false</returns>
        public bool isKeyPressed(Keys key)
        {
            if (sensing.Key == key.ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #endregion
        /* ------------------- */

        /* ------------ GAME CODE START ------------ */

        /* Game variables */
        bool aktivnost=false;
        int brzina=0;
        int i=0;
        int j = 0;
        int z;

        int tempx;
        int tempy;


        /* Initialization */
        Voce ananas;
        Voce banana;
        Voce borovnica;
        Voce limun;
        Voce jabuka;

        Voce yellow;
        Voce green;
        Voce red;
        Voce blue;

        Voce[] polje=new Voce[5];

        Voce[] polje1 = new Voce[5]; 

        Cekic cekic;
        Random rd;

        int brojac_voca=0;
        int brojac_udaraca=0;


        public delegate void MyEventHandler();
        public static event MyEventHandler testEvent;

        public void PromjeniLabelu()
        {
           ISPIS = cekic.Zivot.ToString();
        }

        
        private void SetupGame()
        {
            
            //1. setup stage
            SetStageTitle("PMF");
            //setBackgroundColor(Color.WhiteSmoke);            
            setBackgroundPicture("backgrounds\\back.jpg");
            //none, tile, stretch, center, zoom
            setPictureLayout("stretch");

            //2. add sprites

            rd = new Random();

            j = rd.Next(0, 3);

            if (j == 0)
            {
                ananas = new Voce("sprites\\ananas.jpg", 0, -70, "ananas", true);
            }
            else
            {
                ananas = new Voce("sprites\\ananas.jpg", 0, -70, "ananas", false);
            }

            j = rd.Next(0, 3);
            if (j == 0)
            {
                banana = new Voce("sprites\\banana.jpg", 80, -70, "banana", true);
            }
            else
            {
                banana = new Voce("sprites\\banana.jpg", 80, -70, "banana", false);
            }

            j = rd.Next(0, 3);
            if (j == 0)
            {
                borovnica = new Voce("sprites\\borovnica.jpg", 160, -70, "borovnica", true);
            }
            else
            {
                borovnica = new Voce("sprites\\borovnica.jpg", 160, -70, "borovnica", false);
            }

            j = rd.Next(0, 3);
            if (j == 0)
            {
                jabuka = new Voce("sprites\\jabuka.jpg", 240, -70, "jabuka", true);
            }
            else
            {
                jabuka = new Voce("sprites\\jabuka.jpg", 240, -70, "jabuka", false);
            }

            j = rd.Next(0, 3);
            if (j == 0)
            {
                limun = new Voce("sprites\\limun.png", 320, -70, "limun", true);
            }
            else
            {
                limun = new Voce("sprites\\limun.png", 320, -70, "limun", false);
            }

            cekic = new Cekic("sprites\\cekic.jpg", 200,200, "cekic", 10);
            Game.AddSprite(cekic);

            yellow = new Voce("sprites\\yellow.jpg", 0, -70, "yellow", false);
            red = new Voce("sprites\\red.jpg", 0, -70, "red", false);
            green = new Voce("sprites\\green.jpg", 0, -70, "green", false);
            blue = new Voce("sprites\\blue.png", 0, -70, "blue", false);


            ananas.SetTransparentColor(Color.White);
            yellow.SetTransparentColor(Color.White);

            Game.AddSprite(ananas);
            Game.AddSprite(banana);
            Game.AddSprite(borovnica);
            Game.AddSprite(jabuka);
            Game.AddSprite(limun);


            Game.AddSprite(yellow);
            Game.AddSprite(red);
            Game.AddSprite(green);
            Game.AddSprite(blue);

            polje1[0] = green;
            polje1[1] = yellow;
            polje1[2] = blue;
            polje1[3] = red;
            polje1[4] = yellow;

            

            polje[0] = ananas;
            polje[1] = banana;
            polje[2] = borovnica;
            polje[3] = jabuka;
            polje[4] = limun;

            testEvent += new MyEventHandler(PromjeniLabelu);

            testEvent.Invoke();
            //3. scripts that start
            Game.StartScript(KretanjeCekica);
            Game.StartScript(KretanjeVoca);
            Game.StartScript(Dodir);


        }
        public void Ispis(int a)
        {
            MessageBox.Show("Zivot cekica: " + a.ToString());
        }
        public void Ispis(string a)
        {
            MessageBox.Show("Ime voca: " + a);
        }
        /* Scripts */

        private int Metoda()
        {
            while (START) //ili neki drugi uvjet
            {

                Wait(0.1);
            }
            return 0;
        }
        private int KretanjeCekica()
        {
            while (START)
            {
                if (sensing.MouseDown == false)
                {
                    cekic.X = sensing.Mouse.X;
                    cekic.Y = sensing.Mouse.Y;
                    Wait(0.01);
                }

                else
                {
                    cekic.X = sensing.Mouse.X;
                    cekic.Y = sensing.Mouse.Y;
                    try
                    {
                        cekic.Zivot--;
                        brojac_udaraca++;
                        
                    }
                    catch(Exception e)
                    {
                        START = false;
                        MessageBox.Show("Igra je završena");
                    }
                    //testEvent.Invoke();
                    testEvent.Invoke();
                    Wait(0.1);

                }
                
            }
            
            return 0;
        }
        private int KretanjeVoca()
        {
            while (START)
            {
                if (aktivnost == true)
                {
                    polje[i].Y = polje[i].Y + brzina;
                }
                else
                {
                    brojac_voca++;
                    rd = new Random();
                    i = rd.Next(0, 5);
                    aktivnost = true;
                    brzina = brzina + 2;
                    z = rd.Next(0, 3);
                    if (z == 0)
                    {
                        polje[i].Zivot = true;
                    }
                    else
                    {
                        polje[i].Zivot = false;
                    }
                    

                }
                
                if (polje[i].Y > GameOptions.DownEdge)
                {
                    
                    polje[i].X = rd.Next(0, GameOptions.RightEdge-70);
                    polje[i].Y = -70;
                    try
                    {
                        cekic.Zivot=cekic.Zivot-2;
                    }
                    catch (Exception e)
                    {
                        START = false;
                        Ispis(cekic.Zivot);
                        Ispis(polje[i].Ime);
                        MessageBox.Show("Igra je gotova!\n Broj udaraca:"+brojac_udaraca.ToString()+"\n Broj voca: "+brojac_voca.ToString());
                        

                    }
                    //ISPIS = cekic.Zivot.ToString();

                    testEvent.Invoke();
                    aktivnost = false;

                }
                Wait(0.1);
            }
            
            
            return 0;
        }
        private int Dodir()
        {
            while (START)
            {
                
                if (polje[i].TouchingSprite(cekic) && sensing.MouseDown)
                {
                    cekic.Zivot++;

                   
                    testEvent.Invoke();
                    tempx = polje[i].X;
                    tempy = polje[i].Y;
                    polje[i].Y = -70;
                    polje[i].X = rd.Next(0, GameOptions.RightEdge);
                    aktivnost = false;
                    if (polje[i].Zivot == true)
                    {
                        cekic.Zivot++;
                        
                    }
                    polje1[i].X = tempx;
                    polje1[i].Y = tempy;

                    Wait(0.5);


                    polje1[i].X = 0;
                    polje1[i].Y = -70;
                }
                Wait(0.01);

            }

            return 0;
        }

        /* ------------ GAME CODE END ------------ */


    }
}
