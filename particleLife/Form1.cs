using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static particleLife.lifeSim;

namespace particleLife
{
    public partial class lifeSim : Form
    {
        int particleSize = 2;
        int particleCount = 200;
        int staticStrength = 3;

        int frameCounter = 0;

        ParticleType redParticle = new ParticleType();
        ParticleType blueParticle = new ParticleType();
        ParticleType greenParticle = new ParticleType();
        ParticleType yellowParticle = new ParticleType();

        List<Particle> redParticles = new List<Particle>();
        List<Particle> greenParticles = new List<Particle>();
        List<Particle> blueParticles = new List<Particle>();
        List<Particle> yellowParticles = new List<Particle>();
        List<Particle> netParticles = new List<Particle>();

        public class ParticleType
        {
            public Dictionary<string, int> staticForces { get; set; }
            public Color particleColor { get; set; }

            public ParticleType()
            {
                staticForces = new Dictionary<string, int>();
            }
        }

        public class Particle
        {
            public int x { get; set; }
            public int y { get; set; }
            public ParticleType type { get; set; }
            public Vector2 velocity { get; set; }

            // Constructor
            public Particle()
            {
                velocity = Vector2.Zero; // Or initialize with some default value
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

            public static Vector2 Zero => new Vector2(0, 0);

            // Operator overloads for vector arithmetic

            public static Vector2 operator +(Vector2 a, Vector2 b)
            {
                if (a == null || b == null)
                {
                    throw new ArgumentNullException("Both vectors must be non-null for addition.");
                    // Or handle gracefully by returning Vector2.Zero or some other default behavior
                }

                return new Vector2(a.X + b.X, a.Y + b.Y);
            }

            public static Vector2 operator -(Vector2 a, Vector2 b)
            {
                if (a == null || b == null)
                {
                    throw new ArgumentNullException("Both vectors must be non-null for subtraction.");
                    // Or handle gracefully by returning Vector2.Zero or some other default behavior
                }

                return new Vector2(a.X - b.X, a.Y - b.Y);
            }

            public static Vector2 operator *(Vector2 a, float scalar)
            {
                if (a == null)
                    throw new ArgumentNullException(nameof(a), "Vector a cannot be null.");

                return new Vector2(a.X * scalar, a.Y * scalar);
            }

            public static Vector2 operator /(Vector2 a, float scalar)
            {
                if (a == null)
                    throw new ArgumentNullException(nameof(a), "Vector a cannot be null.");

                return new Vector2(a.X / scalar, a.Y / scalar);
            }

            // Additional vector operations

            public float Length() => (float)Math.Sqrt(X * X + Y * Y);

            public Vector2 Normalize() => this / Length();
        }

        public Dictionary<string, int> GiveStaticForces()
        {
            Random rnd = new Random();

            int redForce = rnd.Next(-staticStrength, staticStrength + 1);
            int blueForce = rnd.Next(-staticStrength, staticStrength + 1);
            int greenForce = rnd.Next(-staticStrength, staticStrength + 1);
            int yellowForce = rnd.Next(-staticStrength, staticStrength + 1);

            Dictionary<string, int> forces = new Dictionary<string, int>();
            forces.Add("red", redForce);
            forces.Add("blue", blueForce);
            forces.Add("green", greenForce);
            forces.Add("yellow", yellowForce);

            return forces;
        }
        public lifeSim()
        {
            InitializeComponent();
            GameInit();
        }

        private void GameInit()
        {
            //Form1_Resize();

            //Clear Paritcles
            redParticles.Clear();
            blueParticles.Clear();
            greenParticles.Clear();
            yellowParticles.Clear();
            netParticles.Clear();

            //Set Interative Forces
            redParticle.staticForces = GiveStaticForces();
            blueParticle.staticForces = GiveStaticForces();
            greenParticle.staticForces = GiveStaticForces();
            yellowParticle.staticForces = GiveStaticForces();

            //Initialize particles
            InitializeParticles(redParticles, redParticle, particleCount, Color.Red);
            InitializeParticles(blueParticles, blueParticle, particleCount, Color.Blue);
            InitializeParticles(greenParticles, greenParticle, particleCount, Color.Green);
            InitializeParticles(yellowParticles, yellowParticle, particleCount, Color.Yellow);
        }

        private void InitializeParticles(List<Particle> particles, ParticleType type, int count, Color color)
        {
            particles.Clear();

            Random rnd = new Random();
            for (int i = 0; i < count; i++)
            {
                Particle temp = new Particle();
                temp.x = rnd.Next(0, this.ClientSize.Width - 3);
                temp.y = rnd.Next(0, this.ClientSize.Height - 3);
                temp.type = type;
                temp.type.particleColor = color;
                temp.velocity = Vector2.Zero; // Ensure velocity is initialized

                particles.Add(temp);
                netParticles.Add(temp);
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
            float maxForceMagnitude = 0.05f; // Maximum magnitude of interaction force
            float speed = 3.0f; // Adjust as needed

            foreach (var particle in netParticles)
            {
                Vector2 totalForce = Vector2.Zero;

                // Calculate interaction forces from other particles
                foreach (var otherParticle in netParticles)
                {
                    if (particle != otherParticle && Math.Abs(otherParticle.x - particle.x) <= 350 && Math.Abs(otherParticle.y - particle.y) <= 350)
                    {
                        // Determine interaction force based on particle types
                        string otherTypeKey = GetParticleTypeKey(otherParticle.type);
                        int force = particle.type.staticForces[otherTypeKey];

                        // Calculate force direction (vector pointing from otherParticle to particle)
                        Vector2 forceDirection = new Vector2(otherParticle.x - particle.x, otherParticle.y - particle.y);

                        // Calculate force magnitude based on force value and distance (if needed)
                        float distance = forceDirection.Length();
                        if (distance > 0)
                        {
                            float forceMagnitude = force * maxForceMagnitude / distance; // Adjust force magnitude based on distance
                            totalForce += forceDirection.Normalize() * forceMagnitude;
                        }
                    }
                }

                // Update velocity based on total force
                particle.velocity += totalForce * speed;

                // Update position based on velocity
                particle.x = (int)(particle.x + particle.velocity.X);
                particle.y = (int)(particle.y + particle.velocity.Y);

                WallCollision(particle);
                // Wrap particles around the screen if they go out of bounds (optional)
                //WrapParticleAroundScreen(particle);

                if (frameCounter % 10 == 0)
                {
                    DecellerateParticles(particle);
                }
            }
        }

        private void DecellerateParticles(Particle particle)
        {
            if (particle.velocity.X > 0)
            {
                particle.velocity.X--;
            }

            if (particle.velocity.X < 0)
            {
                particle.velocity.X++;
            }

            if (particle.velocity.Y > 0)
            {
                particle.velocity.Y--;
            }

            if (particle.velocity.Y < 0)
            {
                particle.velocity.Y++;
            }
        }

        private string GetParticleTypeKey(ParticleType type)
        {
            if (ReferenceEquals(type, redParticle))
            {
                return "red";
            }
            else if (ReferenceEquals(type, blueParticle))
            {
                return "blue";
            }
            else if (ReferenceEquals(type, greenParticle))
            {
                return "green";
            }
            else if (ReferenceEquals(type, yellowParticle))
            {
                return "yellow";
            }
            else
            {
                throw new ArgumentException("Unknown ParticleType encountered");
            }
        }

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

        private void WallCollision(Particle particle)
        {
            if (particle.x < 0)
            {
                particle.x = 0;
                particle.velocity.X *= -1f;
            }

            if (particle.y < 0)
            {
                particle.y = 0;
                particle.velocity.Y *= -1f;
            }

            if (particle.x > this.ClientSize.Width)
            {
                particle.x = this.ClientSize.Width;
                particle.velocity.X *= -1f;
            }

            if (particle.y > this.ClientSize.Height)
            {
                particle.y = this.ClientSize.Height;
                particle.velocity.Y *= -1f;
            }
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            frameCounter++;
            MoveParticles();
            //Form1_Resize();
            Refresh();
        }

        private void lifeSim_Paint(object sender, PaintEventArgs e)
        {
            DrawParticles(e.Graphics, redParticles, Color.Red);
            DrawParticles(e.Graphics, blueParticles, Color.Blue);
            DrawParticles(e.Graphics, greenParticles, Color.Green);
            DrawParticles(e.Graphics, yellowParticles, Color.Yellow);
        }

        private void DrawParticles(Graphics graphics, List<Particle> particles, Color color)
        {
            using (Brush brush = new SolidBrush(color))
            {
                foreach (var particle in particles)
                {
                    graphics.FillRectangle(brush, particle.x, particle.y, particleSize, particleSize);
                }
            }
        }

        private void lifeSim_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.R:
                    GameInit();
                    break;
            }
        }
    }
}
