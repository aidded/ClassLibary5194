using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using ClassLibrary2361;
using PhysicalSimulations;
using System.Xml.Serialization;
using System.Threading;
namespace Example
{
    class MyApplication
    {
        static FieldSimulation CV = new FieldSimulation();
        static int Pancake = 1;
        static Thread[] Hs;
        [STAThread]
        public static void Main()
        {
            Console.WriteLine("How many threads to run?");
            int NoThreadsToRun = int.Parse(Console.ReadLine());
            Console.WriteLine("Write Results or Speed regularly? Type Y to write results");
            bool f = false;
            if (Console.ReadLine().Contains("y"))
            {
                f = true;
            }
            GameWindowEventAddingAndRunning(NoThreadsToRun,f);
        }

        private static void GameWindowEventAddingAndRunning(int NoThreadsToRun,bool WriteResults)
        {
            using (var game = new GameWindow())
            {
                game.Load += (sender, e) =>
                {
                    // setup settings, load textures, sounds
                    LoadSimu(NoThreadsToRun,WriteResults);
                };

                game.Resize += (sender, e) => GL.Viewport(0, 0, game.Width, game.Height);

                game.UpdateFrame += (sender, e) => UpdateSimu();

                game.RenderFrame += (sender, e) =>
                {
                    Render(game);
                };

                // Run the game at 60 updates per second
                game.Run(200, 60);
            }
        }

        private static void Render(GameWindow game)
        {
            // render graphics
            InitGraphics(game);
            RenderBloops();
            RenderFood();
            game.SwapBuffers();
        }

        private static void InitGraphics(GameWindow game)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-34d * ((double)game.Width) / ((double)game.Height), 154d * ((double)game.Width) / ((double)game.Height), -34, 154, 0.0, 700.0);
        }

        private static void RenderBloops()
        {
            foreach (InTime<BleepyBloop> V in CV.Turnip.Generations[CV.Turnip.Generations.Count - 1].Bloops)
            {
                GL.Begin(PrimitiveType.Points);
                GL.Color3(Color.Orange);
                GL.Vertex2(V.AtFrame[V.AtFrame.Count - 1].Position.x, V.AtFrame[V.AtFrame.Count - 1].Position.y);
                GL.End();
                GL.Begin(PrimitiveType.LineLoop);
                for (double r = 0; r < 7; r += 0.5)
                {
                    GL.Vertex2(V.AtFrame[V.AtFrame.Count - 1].Position.x +
                        (Math.Sqrt(V.AtFrame[V.AtFrame.Count - 1].Food) / 7d + 0.5) * Math.Sin(r),
                        V.AtFrame[V.AtFrame.Count - 1].Position.y +
                        (Math.Sqrt(V.AtFrame[V.AtFrame.Count - 1].Food) / 7d + 0.5) * Math.Cos(r));
                }
                GL.End();
            }
        }

        private static void RenderFood()
        {
            foreach (Food V in CV.Turnip.Generations[CV.Turnip.Generations.Count - 1].Foods)
            {
                if (V.Size != 0)
                {
                    GL.Begin(PrimitiveType.Points);
                    GL.Color3(Color.Green);
                    GL.Vertex2(V.Position.x, V.Position.y);
                    GL.End();
                    GL.Begin(PrimitiveType.LineLoop);
                    for (double r = 0; r < 7; r += 0.5)
                    {
                        GL.Vertex2(V.Position.x + Math.Sqrt(V.Size / 15d) * Math.Sin(r), V.Position.y + Math.Sqrt(V.Size / 15d) * Math.Cos(r));
                    }
                    GL.End();
                }
            }
        }

        private static void UpdateSimu()
        {
            for (int i = 0; i < 8; i++)
            {
                if (CV.Step(Pancake)) Pancake++;
            }
        }

        private static void LoadSimu(int NumberOfThreads,bool F)
        {
            Field.NumThread = NumberOfThreads;
            CV.init(32,F);
            Console.BufferWidth = 500;
            Console.BufferHeight = 2500;
            int n = NumberOfThreads;
            Hs = new Thread[n];
            for (int i = 0; i < n; i++)
            {
                Hs[i] = new Thread(() => Thread.Sleep(1));
            }
            CV.Hs = Hs;
        }
    }
}