using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace particleLife
{
    public partial class lifeSim : Form
    {
        int particleSize = 5;
        int particleCount = 100;

        List<Particle> redParticles = new List<Particle>();
        List<Particle> greenParticles = new List<Particle>();
        List<Particle> blueParticles = new List<Particle>();
        List<Particle> yellowParticles = new List<Particle>();

        public class ParticleType
        {
            public List<int> staticForces { get; set; }
            public Color particleColor { get; set; }
        }

        public class Particle
        {
            public int x {get; set;}
            public int y {get; set;}
            public ParticleType type { get; set;}
            public Vector2 velocity { get; set; } // Velocity vector for particle movement

            public Particle()
            {
                velocity = new Vector2(); // Initialize velocity with default (0, 0)
            }
        }

        public class Vector2
        {
            public float X { get; set; }
            public float Y { get; set; }

            public Vector2(float x, float y)
            {
                X = x;
                Y = y;
            }

            // Add a default constructor to initialize to (0, 0)
            public Vector2()
            {
                X = 0;
                Y = 0;
            }
            public static Vector2 Zero => new Vector2(0, 0);

            public static Vector2 operator +(Vector2 a, Vector2 b)
            {
                if (a == null)
                    throw new ArgumentNullException(nameof(a));
                if (b == null)
                    throw new ArgumentNullException(nameof(b));

                return new Vector2(a.X + b.X, a.Y + b.Y);
            }
            public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.X - b.X, a.Y - b.Y);
            public static Vector2 operator *(Vector2 a, float scalar) => new Vector2(a.X * scalar, a.Y * scalar);
            public static Vector2 operator /(Vector2 a, float scalar) => new Vector2(a.X / scalar, a.Y / scalar);

            public float Length() => (float)Math.Sqrt(X * X + Y * Y);
            public Vector2 Normalize() => this / Length();
        }

        public List<int> GiveStaticForces()
        {
            Random rnd = new Random();

            int redForce = rnd.Next(-4, 4);
            int blueForce = rnd.Next(-4, 4);
            int greenForce = rnd.Next(-4, 4);
            int yellowForce = rnd.Next(-4, 4);


            List<int> forces = new List<int>();
            forces.Add(redForce);
            forces.Add(blueForce);
            forces.Add(greenForce);
            forces.Add(yellowForce);

            return forces;
        }
        public lifeSim()
        {
            InitializeComponent();
            GameInit();
        }

        private void GameInit()
        {
            Form1_Resize();
            redParticles.Clear();
            blueParticles.Clear();
            greenParticles.Clear();
            yellowParticles.Clear();

            ParticleType redParticle = new ParticleType();
            redParticle.staticForces = GiveStaticForces();
            redParticle.particleColor = Color.Red;
            
            ParticleType blueParticle = new ParticleType();
            blueParticle.staticForces = GiveStaticForces();
            blueParticle.particleColor = Color.Red;

            ParticleType greenParticle = new ParticleType();
            greenParticle.staticForces = GiveStaticForces();
            greenParticle.particleColor = Color.Red;

            ParticleType yellowParticle = new ParticleType();
            yellowParticle.staticForces = GiveStaticForces();
            yellowParticle.particleColor = Color.Red;
            

            Random rnd = new Random();
            for(int i = 0; i < particleCount; i++)
            {
                Particle temp = new Particle();
                temp.x = rnd.Next(0, this.ClientSize.Width - 3);
                temp.y = rnd.Next(0, this.ClientSize.Height - 3);
                temp.type = redParticle;

                redParticles.Add(temp);
            }

            for (int i = 0; i < particleCount; i++)
            {
                Particle temp = new Particle();
                temp.x = rnd.Next(0, this.ClientSize.Width - 3);
                temp.y = rnd.Next(0, this.ClientSize.Height - 3);
                temp.type = blueParticle;

                blueParticles.Add(temp);
            }

            for (int i = 0; i < particleCount; i++)
            {
                Particle temp = new Particle();
                temp.x = rnd.Next(0, this.ClientSize.Width - 3);
                temp.y = rnd.Next(0, this.ClientSize.Height - 3);
                temp.type = greenParticle;

                greenParticles.Add(temp);
            }

            for (int i = 0; i < particleCount; i++)
            {
                Particle temp = new Particle();
                temp.x = rnd.Next(0, this.ClientSize.Width - 3);
                temp.y = rnd.Next(0, this.ClientSize.Height - 3);
                temp.type = yellowParticle;

                yellowParticles.Add(temp);
            }
        }

        private void Form1_Resize()
        {
            // Ensure the form stays maximized
            if (this.WindowState != FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
        }

        private void MoveParticles()
        {
            float speed = 1.0f; // Adjust as needed
            Random rnd = new Random(); // Move this outside the loop for better randomness
            foreach (var particle in redParticles.Concat(blueParticles).Concat(greenParticles).Concat(yellowParticles))
            {
                // Calculate acceleration based on static forces of the particle type
                Vector2 acceleration = Vector2.Zero;
                foreach (var force in particle.type.staticForces)
                {
                    Vector2 direction = new Vector2((float)Math.Cos(force), (float)Math.Sin(force)); // Convert force angle to direction
                    acceleration += direction; // Accumulate all forces
                }

                // Update velocity based on acceleration
                particle.velocity += acceleration.Normalize() * speed;

                // Update position based on velocity
                particle.x = (int)(particle.x + particle.velocity.X);
                particle.y = (int)(particle.y + particle.velocity.Y);

                // Wrap particles around the screen if they go out of bounds (optional)
                WrapParticleAroundScreen(particle);
            }
        }

        // Optional: Wrap particles around the screen if they go out of bounds
        private void WrapParticleAroundScreen(Particle particle)
        {
            if (particle.x < 0)
                particle.x = this.ClientSize.Width;
            else if (particle.x > this.ClientSize.Width)
                particle.x = 0;

            if (particle.y < 0)
                particle.y = this.ClientSize.Height;
            else if (particle.y > this.ClientSize.Height)
                particle.y = 0;
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            MoveParticles();
            Refresh();
            Form1_Resize();
        }

        private void lifeSim_Paint(object sender, PaintEventArgs e)
        {
            foreach (var p in redParticles)
            {
                Brush temp = new SolidBrush(Color.Red);
                e.Graphics.FillRectangle(temp, p.x, p.y, particleSize, particleSize);
            }

            foreach (var p in blueParticles)
            {
                Brush temp = new SolidBrush(Color.Blue);
                e.Graphics.FillRectangle(temp, p.x, p.y, particleSize, particleSize);
            }

            foreach (var p in greenParticles)
            {
                Brush temp = new SolidBrush(Color.Green);
                e.Graphics.FillRectangle(temp, p.x, p.y, particleSize, particleSize);
            }

            foreach (var p in yellowParticles)
            {
                Brush temp = new SolidBrush(Color.Yellow);
                e.Graphics.FillRectangle(temp, p.x, p.y, particleSize, particleSize);
            }
        }

        private void lifeSim_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.R:
                    GameInit();
                    break;
            }
        }
    }
}
