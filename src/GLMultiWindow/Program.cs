using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using glcore;
using glcore.Types;
using glextras;
using glextras.stl;

namespace GLMultiWindow
{
    class Program
    {
        static IntPtr CreateForm(int index, Size[] resize, bool[] flag)
        {
            IntPtr handle = IntPtr.Zero;

            Task.Run(delegate()
            {
                Form form = new Form();
                form.ClientSize = startupSize;
                form.FormClosing += form_FormClosing;
                handle = form.Handle;
                resize[index] = form.ClientSize;

                form.Resize += delegate(object sender, EventArgs e)
                {
                    lock (RT.RenderLock)
                    {
                        resize[index] = form.ClientSize;
                        flag[index] = true;
                    }
                };

                form.ShowDialog();
            });

            //await until handle is generated
            while (handle == IntPtr.Zero) ;

            return handle;
        }

        static void form_FormClosing(object sender, FormClosingEventArgs e)
        {
            lock (RT.RenderLock)
            {
                RT.Stop();
            }
        }

        static Size startupSize = new Size(800, 600);

        static object threadLocking = new object();

        static Shader basicShader;
        static GLBuffer teapotObject;
        
        static RenderThread RT;
        static BlitData blt1, blt2;

        static Vector3 cameraPosition;
        static Vector3 cameraRotation;


        static Size[] resizeData = new Size[2];
        static bool[] resizeFlag = new bool[2];


        static float angle = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("Initializng Forms");
            IntPtr handle1 = CreateForm(0, resizeData, resizeFlag), handle2 = CreateForm(1, resizeData, resizeFlag);

            Console.WriteLine("Initializing OpenGL");
            blt1 = new BlitData(handle1); blt2 = new BlitData(handle2);

            Console.WriteLine("Linking Contexts");
            blt1.MakeCurrent();
            blt1.LinkTo(blt2);

            Console.WriteLine("Compiling Shaders");
            if (!Shader.Compile(File.ReadAllText(@"Shaders\simpleVS.glsl"), File.ReadAllText(@"Shaders\simpleFS.glsl"), out basicShader))
            {
                Console.WriteLine("Failed To Compile Shader:\n " + Shader.CompileLog);
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Importing Models");
            STLImporter sImport = new STLImporter("teapot.stl");
            float[] vertexData = STLImporter.AverageUpFaceNormalsAndOutputVertexBuffer(sImport.AllTriangles, 45);

            Console.WriteLine("Creating Vertex Buffer Objects");
            teapotObject = new GLBuffer(vertexData, basicShader);


            Console.WriteLine("Starting Render Thread");
            RT = new RenderThread(144);
            RT.RenderFrame += RT_RenderFrame;

            RT.StartOnSameThread();

            Console.WriteLine("\nRendering has stopped...");

        //    Console.ReadLine();
        }

        static void RT_RenderFrame()
        {
            //handle resize requests
            if (resizeFlag[0]) blt1.Resize(resizeData[0].Width, resizeData[0].Height);
            if (resizeFlag[1]) blt2.Resize(resizeData[1].Width, resizeData[1].Height);
            resizeFlag[0] = false;
            resizeFlag[1] = false;


            GL.Clear(blt1, 1, 0.5f, 1, 1);
            GL.Clear(blt2, 1, 1, 1, 1);

            //set cameraPosition, Rotation
            cameraPosition = new Vector3(-40 * (float)Math.Sin(angle * Math.PI / 180d), 35.0f, -40 * (float)Math.Cos(angle * Math.PI / 180d));
            cameraRotation = new Vector3(0, 28.75f, angle); //look left = negative

            Matrix4x4 model = new Matrix4x4(true);
            Matrix4x4 view = (Matrix4x4.RollMatrix(-cameraRotation.y) * Matrix4x4.PitchMatrix(-cameraRotation.z)) * Matrix4x4.TranslationMatrix(-cameraPosition);

            Matrix4x4 projection1 = Matrix4x4.PerspectiveMatrix(90, resizeData[0].Width, resizeData[0].Height, 0.1f, 100);
            Matrix4x4 projection2 = Matrix4x4.PerspectiveMatrix(90, resizeData[1].Width, resizeData[1].Height, 0.1f, 100);

            basicShader.SetValue("model", model);
            basicShader.SetValue("view", view);

            //render one
            blt1.MakeCurrent();
            basicShader.SetValue("projection", projection1);  
            GL.Draw(teapotObject, basicShader);

            blt2.MakeCurrent();
            basicShader.SetValue("projection", projection2);
            GL.Draw(teapotObject, basicShader);


            GLError error = GL.CheckError();
            if (error != GLError.GL_NO_ERROR)
            {
                MessageBox.Show("GLDraw Error: " + error);
                Application.Exit();
            }


            //blit both!
            GL.Blit(blt1);
            GL.Blit(blt2);

            angle += 0.5f;
        }

    }
}
