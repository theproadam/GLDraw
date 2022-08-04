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
using System.Runtime.InteropServices;
using System.IO;

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
        [DllImport("winmm.dll")]
        public static extern uint timeBeginPeriod(uint uMilliseconds);

        [DllImport("winmm.dll")]
        public static extern uint timeEndPeriod(uint uMilliseconds);

        Thread T;
        bool DontStop = true;
        double TickRate;
        double NextTimeToFire = 0;
        bool finished = false;
        public object RenderLock = new object();
        bool useSleep = false;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="useSleepNotBlock">Enables timeBeginPeriod and timeEndPeriod for allow the CPU to sleep, thus highly reducing CPU usage. This may
        /// have unintended consequences for systems running versions of windows before the Win 10 2004 update </param>
        public void Start(bool useSleepNotBlock = false)
        {
            if (T != null && T.IsAlive)
                return;

            useSleep = useSleepNotBlock;

            DontStop = true;
            T = new Thread(RenderCode);
            T.Start();
        }

        public void Stop()
        {
            DontStop = false;
        }

        public void StartOnSameThread()
        {
            RenderCode();
        }

        void RenderCode()
        {
            Stopwatch sw = new Stopwatch();
    
            try
            {
                if (useSleep)
                    timeBeginPeriod(1);

                sw.Start();
                while (DontStop)
                {
                    if (!DontStop)
                        break;

                    if (sw.Elapsed.TotalMilliseconds >= NextTimeToFire)
                    {
                        NextTimeToFire = sw.Elapsed.TotalMilliseconds + TickRate;
                        lock (RenderLock)
                        {
                            RenderFrame();
                        }
                    }
                    else if (useSleep)
                        Thread.Sleep(1);

                }

                sw.Stop();
                sw.Reset();
            }
            finally
            {
                if (useSleep)
                    timeEndPeriod(1);
            }

            finished = true;
        }
    }


    public class STLImporter
    {
        //WARNING: This STL Importer has issues importing ASCII Files on certain computers running Windows 10.
        public string STLHeader { get; private set; }
        public STLFormat STLType { get; private set; }
        public uint TriangleCount { get; private set; }
        public Triangle[] AllTriangles { get; private set; }

        public STLImporter(string TargetFile)
        {
            // Verify That The File Exists
            if (!File.Exists(TargetFile))
                throw new System.IO.FileNotFoundException("Target File Does Not Exist!", "Error!");

            // Load The File Into The Memory as ASCII
            string[] allLinesASCII = File.ReadAllLines(TargetFile);

            // Detect if STL File is ASCII or Binary
            bool ASCII = isAscii(allLinesASCII);

            // Insert Comment Here
            if (ASCII)
            {
                STLType = STLFormat.ASCII;
                AllTriangles = ASCIISTLOpen(allLinesASCII);
            }
            else
            {
                STLType = STLFormat.Binary;
                AllTriangles = BinarySTLOpen(TargetFile);
            }

        }

        Triangle[] BinarySTLOpen(string TargetFile)
        {
            List<Triangle> Triangles = new List<Triangle>();

            byte[] fileBytes = File.ReadAllBytes(TargetFile);
            byte[] header = new byte[80];

            for (int b = 0; b < 80; b++)
                header[b] = fileBytes[b];

            STLHeader = System.Text.Encoding.UTF8.GetString(header);

            uint NumberOfTriangles = System.BitConverter.ToUInt32(fileBytes, 80);
            TriangleCount = NumberOfTriangles;

            for (int i = 0; i < NumberOfTriangles; i++)
            {
                // Read The Normal Vector
                float normalI = System.BitConverter.ToSingle(fileBytes, 84 + i * 50);
                float normalJ = System.BitConverter.ToSingle(fileBytes, (1 * 4) + 84 + i * 50);
                float normalK = System.BitConverter.ToSingle(fileBytes, (2 * 4) + 84 + i * 50);

                // Read The XYZ Positions of The First Vertex
                float vertex1x = System.BitConverter.ToSingle(fileBytes, 3 * 4 + 84 + i * 50);
                float vertex1y = System.BitConverter.ToSingle(fileBytes, 4 * 4 + 84 + i * 50);
                float vertex1z = System.BitConverter.ToSingle(fileBytes, 5 * 4 + 84 + i * 50);

                // Read The XYZ Positions of The Second Vertex
                float vertex2x = System.BitConverter.ToSingle(fileBytes, 6 * 4 + 84 + i * 50);
                float vertex2y = System.BitConverter.ToSingle(fileBytes, 7 * 4 + 84 + i * 50);
                float vertex2z = System.BitConverter.ToSingle(fileBytes, 8 * 4 + 84 + i * 50);

                // Read The XYZ Positions of The Third Vertex
                float vertex3x = System.BitConverter.ToSingle(fileBytes, 9 * 4 + 84 + i * 50);
                float vertex3y = System.BitConverter.ToSingle(fileBytes, 10 * 4 + 84 + i * 50);
                float vertex3z = System.BitConverter.ToSingle(fileBytes, 11 * 4 + 84 + i * 50);

                // Read The Attribute Byte Count
                int Attribs = System.BitConverter.ToInt16(fileBytes, 12 * 4 + 84 + i * 50);

                // Create a Triangle
                Triangle T = new Triangle();

                // Save all the Data Into Said Triangle
                T.normals = new Vector3(normalI, normalK, normalJ);
                T.vertex1 = new Vector3(vertex1x, vertex1z, vertex1y);
                T.vertex2 = new Vector3(vertex2x, vertex2z, vertex2y);//Possible Error?
                T.vertex3 = new Vector3(vertex3x, vertex3z, vertex3y);

                // Add The Triangle
                Triangles.Add(T);
            }

            return Triangles.ToArray();
        }

        Triangle[] ASCIISTLOpen(string[] ASCIILines)
        {
            STLHeader = ASCIILines[0].Replace("solid ", "");

            uint tCount = 0;
            List<Triangle> Triangles = new List<Triangle>();

            foreach (string s in ASCIILines)
                if (s.Contains("facet normal"))
                    tCount++;

            TriangleCount = tCount;

            for (int i = 0; i < tCount * 7; i += 7)
            {
                string n = ASCIILines[i + 1].Trim().Replace("facet normal", "").Replace("  ", " ");

                // Read The Normal Vector
                float normalI = float.Parse(n.Split(' ')[1]);
                float normalJ = float.Parse(n.Split(' ')[2]);
                float normalK = float.Parse(n.Split(' ')[3]);

                string v1 = ASCIILines[i + 3].Split('x')[1].Replace("  ", " ");


                // Read The XYZ Positions of The First Vertex
                float vertex1x = float.Parse(v1.Split(' ')[1]);
                float vertex1y = float.Parse(v1.Split(' ')[2]);
                float vertex1z = float.Parse(v1.Split(' ')[3]);

                string v2 = ASCIILines[i + 4].Split('x')[1].Replace("  ", " ");

                // Read The XYZ Positions of The Second Vertex
                float vertex2x = float.Parse(v2.Split(' ')[1]);
                float vertex2y = float.Parse(v2.Split(' ')[2]);
                float vertex2z = float.Parse(v2.Split(' ')[3]);

                string v3 = ASCIILines[i + 5].Split('x')[1].Replace("  ", " ");

                // Read The XYZ Positions of The Third Vertex
                float vertex3x = float.Parse(v3.Split(' ')[1]);
                float vertex3y = float.Parse(v3.Split(' ')[2]);
                float vertex3z = float.Parse(v3.Split(' ')[3]);

                // Create a Triangle
                Triangle T = new Triangle();

                // Save all the Data Into Said Triangle
                T.normals = new Vector3(normalI, normalK, normalJ);
                T.vertex1 = new Vector3(vertex1x, vertex1z, vertex1y);
                T.vertex2 = new Vector3(vertex2x, vertex2z, vertex2y);
                T.vertex3 = new Vector3(vertex3x, vertex3z, vertex3y);

                // Add The Triangle
                Triangles.Add(T);
            }

            return Triangles.ToArray();
        }

        bool isAscii(string[] Lines)
        {
            string[] Keywords = new string[] { "facet", "solid", "outer", "loop", "vertex", "endloop", "endfacet" };
            int Det = 0;

            foreach (string s in Lines)
            {
                foreach (string ss in Keywords)
                {
                    if (s.Contains(ss))
                    {
                        Det++;
                    }
                }
            }

            if (Det > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public enum STLFormat
        {
            ASCII,
            Binary
        }

        public static float[] AverageUpFaceNormalsAndOutputVertexBuffer(Triangle[] Input, float CutoffAngle)
        {
            Vector3[] VERTEX_DATA = new Vector3[Input.Length * 3];
            Vector3[] VERTEX_NORMALS = new Vector3[Input.Length * 3];
            int[] N_COUNT = new int[Input.Length * 3];

            for (int i = 0; i < Input.Length; i++)
            {
                VERTEX_DATA[i * 3] = Input[i].vertex1;
                VERTEX_DATA[i * 3 + 1] = Input[i].vertex2;
                VERTEX_DATA[i * 3 + 2] = Input[i].vertex3;
            }

            CutoffAngle *= (float)(Math.PI / 180f);
            CutoffAngle = (float)Math.Cos(CutoffAngle);

            for (int i = 0; i < VERTEX_DATA.Length; i++)
            {
                for (int j = 0; j < VERTEX_DATA.Length; j++)
                {
                    if (Vector3.Compare(VERTEX_DATA[j], VERTEX_DATA[i]) && Vector3.Dot(Input[i / 3].normals, Input[j / 3].normals) > CutoffAngle)
                    {
                        VERTEX_NORMALS[i] += Input[j / 3].normals;
                        N_COUNT[i]++;
                    }
                }
            }

            for (int i = 0; i < N_COUNT.Length; i++)
            {
                if (N_COUNT[i] != 0)
                    VERTEX_NORMALS[i] /= N_COUNT[i];
            }

            float[] Output = new float[VERTEX_DATA.Length * 6];

            for (int i = 0; i < VERTEX_DATA.Length; i++)
            {
                Output[i * 6 + 0] = VERTEX_DATA[i].x;
                Output[i * 6 + 1] = VERTEX_DATA[i].y;
                Output[i * 6 + 2] = VERTEX_DATA[i].z;
                Output[i * 6 + 3] = VERTEX_NORMALS[i].x;
                Output[i * 6 + 4] = VERTEX_NORMALS[i].y;
                Output[i * 6 + 5] = VERTEX_NORMALS[i].z;

            }

            return Output;
        }

        public static float[] FaceNormalsToVertexNormals(Triangle[] Input)
        {
            Vector3[] VERTEX_DATA = new Vector3[Input.Length * 3];
            Vector3[] VERTEX_NORMALS = new Vector3[Input.Length];
            int[] N_COUNT = new int[Input.Length * 3];

            for (int i = 0; i < Input.Length; i++)
            {
                VERTEX_DATA[i * 3] = Input[i].vertex1;
                VERTEX_DATA[i * 3 + 1] = Input[i].vertex2;
                VERTEX_DATA[i * 3 + 2] = Input[i].vertex3;
                VERTEX_NORMALS[i] = Input[i].normals;
            }


            float[] Output = new float[VERTEX_DATA.Length * 6];

            for (int i = 0; i < VERTEX_DATA.Length; i++)
            {
                Output[i * 6 + 0] = VERTEX_DATA[i].x;
                Output[i * 6 + 1] = VERTEX_DATA[i].y;
                Output[i * 6 + 2] = VERTEX_DATA[i].z;
                Output[i * 6 + 3] = VERTEX_NORMALS[i / 3].x;
                Output[i * 6 + 4] = VERTEX_NORMALS[i / 3].y;
                Output[i * 6 + 5] = VERTEX_NORMALS[i / 3].z;

            }

            return Output;
        }


    }

    public class Triangle
    {
        public Vector3 normals;
        public Vector3 vertex1;
        public Vector3 vertex2;
        public Vector3 vertex3;



    }
}
