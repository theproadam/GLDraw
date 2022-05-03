using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using glcore.Types;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Diagnostics;

namespace GLDemo
{
    public class InputManager
    {
        bool rdown = false, ldown = false, udown = false, bdown = false;
        Vector2 KeyDelta = new Vector2(0, 0);
        Form sourceForm;

        bool CursorHook = false, mmbdown = false;
        int MMBDeltaX, MMBDeltaY;

        public Vector3 cameraPosition = new Vector3(0, 0, 0);
        public Vector3 cameraRotation = new Vector3(0, 0, 0);
        Form targetForm;

        public InputManager(Form targetForm)
        {
            sourceForm = targetForm;

            this.targetForm = targetForm;

            targetForm.KeyDown += targetForm_KeyDown;
            targetForm.KeyUp += targetForm_KeyUp;
            targetForm.MouseClick += targetForm_MouseClick;

        }

        void targetForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Cursor.Position = new Point(targetForm.PointToScreen(Point.Empty).X + targetForm.ClientSize.Width / 2, targetForm.PointToScreen(Point.Empty).Y + targetForm.ClientSize.Height / 2);
                Cursor.Hide();
                CursorHook = true;
            }
        }

        void targetForm_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        void targetForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D)
            {
                rdown = false;
            }

            if (e.KeyCode == Keys.A)
            {
                ldown = false;
            }

            if (e.KeyCode == Keys.W)
            {
                udown = false;
            }

            if (e.KeyCode == Keys.S)
            {
                bdown = false;
            }
        }

        void targetForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D)
            {
                rdown = true;
            }

            if (e.KeyCode == Keys.A)
            {
                ldown = true;
            }

            if (e.KeyCode == Keys.W)
            {
                udown = true;
            }

            if (e.KeyCode == Keys.S)
            {
                bdown = true;
            }

            if (e.KeyCode == Keys.Escape)
            {
                Cursor.Show();
                CursorHook = false;
            }
        }

        public void CalcualteKeyboardInput(float deltaTime)
        {
            if (rdown | ldown)
            {
                if (rdown)
                {
                    if (KeyDelta.x > 0)
                    {
                        KeyDelta.x = 0;
                    }
                    KeyDelta.x--;
                }
                else if (ldown)
                {
                    if (KeyDelta.x < 0)
                    {
                        KeyDelta.x = 0;
                    }
                    KeyDelta.x++;
                }
            }
            else
            {
                KeyDelta.x = 0;
            }

            if (udown | bdown)
            {
                if (udown)
                {
                    if (KeyDelta.y > 0)
                    {
                        KeyDelta.y = 0;
                    }
                    KeyDelta.y--;
                }
                else if (bdown)
                {
                    if (KeyDelta.y < 0)
                    {
                        KeyDelta.y = 0;
                    }
                    KeyDelta.y++;
                }
            }
            else
            {
                KeyDelta.y = 0;
            }

            cameraPosition = Pan3D(cameraPosition, cameraRotation, (KeyDelta.x / 32f) * deltaTime, 0, (KeyDelta.y / 32f) * deltaTime);
        }

        public static Vector3 Pan3D(Vector3 Input, Vector3 Rotation, float deltaX, float deltaY, float deltaZ = 0)
        {
            Vector3 I = Input;
            Vector3 RADS = new Vector3(0f, Rotation.y / 57.2958f, Rotation.z / 57.2958f);

            float sinX = (float)Math.Sin(RADS.z); //0
            float sinY = (float)Math.Sin(RADS.y); //0


            float cosX = (float)Math.Cos(RADS.z); //0
            float cosY = (float)Math.Cos(RADS.y); //0

            float XAccel = (cosX * -deltaX + (sinY * deltaY) * sinX) + (sinX * -deltaZ) * cosY;
            float YAccel = (cosY * deltaY) + (sinY * deltaZ);
            float ZAccel = (sinX * deltaX + (sinY * deltaY) * cosX) + (cosX * -deltaZ) * cosY;

            I = I + new Vector3(XAccel, YAccel, ZAccel);

            return I;
        }


        public void CalculateMouseInput()
        {
            int MouseX = 0;
            int MouseY = 0;

            if (CursorHook)
            {
                int cursorX = Cursor.Position.X;
                int cursorY = Cursor.Position.Y;

                int sourceX = 0;
                int sourceY = 0;

                sourceForm.Invoke((Action)delegate()
                {
                    sourceX = sourceForm.PointToScreen(Point.Empty).X + sourceForm.ClientSize.Width / 2;
                    sourceY = sourceForm.PointToScreen(Point.Empty).Y + sourceForm.ClientSize.Height / 2;
                });

                MouseX = cursorX - sourceX;
                MouseY = cursorY - sourceY;

                Cursor.Position = new Point(sourceX, sourceY);
                cameraRotation += new Vector3(0, MouseY / 8f, MouseX / 8f);
            }
            else if (mmbdown)// & !requestHome)
            {
                int cursorX = Cursor.Position.X;
                int cursorY = Cursor.Position.Y;

                MouseX = cursorX - MMBDeltaX;
                MouseY = cursorY - MMBDeltaY;
                MMBDeltaX = cursorX; MMBDeltaY = cursorY;

                cameraPosition = Pan3D(cameraPosition, cameraRotation, MouseX / 8f, MouseY / 8f);
            }
        }

        public Matrix3x3 CreateCameraRotationMatrix()
        {
            return Matrix3x3.RollMatrix(-cameraRotation.y) * Matrix3x3.PitchMatrix(-cameraRotation.z);
        }

        public Matrix4x4 CreateCameraRotationMatrix4x4()
        {
            return Matrix4x4.RollMatrix(-cameraRotation.y) * Matrix4x4.PitchMatrix(-cameraRotation.z);
        }

        public Matrix4x4 CreateViewMatrix()
        {
            return (Matrix4x4.RollMatrix(-cameraRotation.y) * Matrix4x4.PitchMatrix(-cameraRotation.z)) * Matrix4x4.TranslationMatrix(-cameraPosition);
        }
    }

    public class RenderThread
    {
        Thread T;
        bool DontStop = true;
        double TickRate;
        double NextTimeToFire = 0;
        bool finished = false;
        public object RenderLock = new object();

        public bool isStopped
        {
            get { return finished; }
        }

        public bool isAlive
        {
            get { return T.IsAlive; }
        }

        public RenderThread(float TargetFrameTime)
        {
            TickRate = TargetFrameTime;
        }

        public RenderThread(int TargetFrameRate)
        {
            TickRate = 1000f / (float)TargetFrameRate;
        }

        public delegate void TimerFire();
        public event TimerFire RenderFrame;

        public void SetTickRate(float TickRateInMs)
        {
            TickRate = TickRateInMs;
            NextTimeToFire = 0;
        }

        public void Start()
        {
            DontStop = true;
            T = new Thread(RenderCode);
            T.Start();
        }

        public void Abort()
        {
            T.Abort();
        }

        public void Stop()
        {
            DontStop = false;
        }

        void RenderCode()
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();
            while (DontStop)
            {
                if (sw.Elapsed.TotalMilliseconds >= NextTimeToFire)
                {
                    NextTimeToFire = sw.Elapsed.TotalMilliseconds + TickRate;
                    lock (RenderLock)
                    {
                        RenderFrame();
                    }
                }
            }

            sw.Stop();
            sw.Reset();

            finished = true;
        }
    }

    public unsafe static class Utilities
    {
        /// <summary>
        /// Smoothnes Vertex Data
        /// </summary>
        /// <param name="input">XYZ IJK vertex Data</param>
        /// <returns></returns>
        public static float[] smoothData(float[] input, float scale = 0.1f)
        {
            //XYZ IJK

            int vert = input.Length / 6;
            int tris = vert / 3;

            float[] results = new float[input.Length * 4];


            int pos = 0;

            fixed (float* rptr = results)
            {
                fixed (float* fptr = input)
                {
                    for (int i = 0; i < tris; i++)
                    {
                        ComputeTris((Vector3*)(fptr + i * 18), (Vector3*)(rptr) + pos, scale);
                        pos += 24;
                    }
            
                }
            }

            return results;
        }

        static void ComputeTris(Vector3* posi, Vector3* output, float scale)
        {
            Vector3 V1 = posi[0], N1 = posi[1];
            Vector3 V2 = posi[2], N2 = posi[3];
            Vector3 V3 = posi[4], N3 = posi[5];

            //Triangle 1
            Vector3 p12 = (V1 + V2) / 2f;
            Vector3 p13 = (V1 + V3) / 2f;
            Vector3 p23 = (V2 + V3) / 2f;

            Vector3 n12 = Vector3.Normalize((N1 + N2) / 2f);
            Vector3 n13 = Vector3.Normalize((N1 + N3) / 2f);
            Vector3 n23 = Vector3.Normalize((N2 + N3) / 2f);

            p12 = p12 + n12 * scale;
            p13 = p13 + n13 * scale;
            p23 = p23 + n23 * scale;

            //first triangle
            output[0] = V1;
            output[1] = N1;

            output[2] = p12;
            output[3] = n12;

            output[4] = p13;
            output[5] = n13;

            //second triangle
            output[6] = V2;
            output[7] = N2;

            output[8] = p23;
            output[9] = n23;

            output[10] = p12;
            output[11] = n12;

            //third triangle
            output[12] = V3;
            output[13] = N3;

            output[14] = p13;
            output[15] = n13;

            output[16] = p23;
            output[17] = n23;

            //fourth triangle (middle)
            output[18] = p12;
            output[19] = n12;

            output[20] = p23;
            output[21] = n23;

            output[22] = p13;
            output[23] = n13;
        }
    }
}
