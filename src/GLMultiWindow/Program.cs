﻿using System;
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
                //form.ClientSize = startupSize;
                form.ClientSize = resize[index];

                form.FormClosing += form_FormClosing;
                handle = form.Handle;
              //  resize[index] = form.ClientSize;

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
        static GLBuffer sphereObject;

        static RenderThread RT;
        static BlitData blt1, blt2;

        static Vector3 cameraPosition;
        static Vector3 cameraRotation;

        static Size[] resizeData = new Size[2] { new Size(800,600), new Size(800,600)};
        static bool[] resizeFlag = new bool[2];

        static float angle = 0;

        static GLBuffer ImportModel(string path)
        {
            STLImporter sImport = new STLImporter(path); //coord_system
            float[] vertexData = STLImporter.AverageUpFaceNormalsAndOutputVertexBuffer(sImport.AllTriangles, 45f);

            return new GLBuffer(vertexData, basicShader);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Initializng Forms");
            IntPtr handle1 = CreateForm(0, resizeData, resizeFlag), handle2 = CreateForm(1, resizeData, resizeFlag);

            Console.WriteLine("Initializing OpenGL");
            blt1 = new BlitData(handle1, 1); blt2 = new BlitData(handle2, 1);

            Console.WriteLine("Linking Contexts");
          //  blt1.MakeCurrent();
            blt1.LinkTo(blt2);

            Console.WriteLine("Compiling Shaders");
            if (!Shader.Compile(File.ReadAllText(@"Shaders\simpleVS.glsl"), File.ReadAllText(@"Shaders\simpleFS.glsl"), out basicShader))
            {
                Console.WriteLine("Failed To Compile Shader:\n " + Shader.CompileLog);
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Importing Models");
            teapotObject = ImportModel("teapot.stl");
            sphereObject = ImportModel("sphere.stl");


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

           // if (resizeFlag[0]) blt1.Resize(resizeData[0].Width, resizeData[0].Height);
          //  if (resizeFlag[1]) blt2.Resize(resizeData[1].Width, resizeData[1].Height);
          //  resizeFlag[0] = false;
          //  resizeFlag[1] = false;


            GL.Clear(blt1, 1, 0.5f, 1, 1);
            GL.Clear(blt2, 1, 1, 0.5f, 1);

            //set cameraPosition, Rotation
            cameraPosition = new Vector3(-40 * (float)Math.Sin(angle * Math.PI / 180d), 35.0f, -40 * (float)Math.Cos(angle * Math.PI / 180d));
          //  cameraPosition = new Vector3(-40 * (float)Math.Sin(angle * Math.PI / 180d), 0f, -40 * (float)Math.Cos(angle * Math.PI / 180d));


            cameraRotation = new Vector3(0, 28.75f, angle); //look left = negative
          //  cameraRotation = new Vector3(0, 0f, angle); //look left = negative


            Matrix4x4 model = new Matrix4x4(true);
            Matrix4x4 view = (Matrix4x4.RollMatrix(-cameraRotation.y) * Matrix4x4.PitchMatrix(-cameraRotation.z)) * Matrix4x4.TranslationMatrix(-cameraPosition);

            Matrix4x4 projection1 = Matrix4x4.PerspectiveMatrix(90, resizeData[0].Width, resizeData[0].Height, 0.1f, 100);
            Matrix4x4 projection2 = Matrix4x4.PerspectiveMatrix(90, resizeData[1].Width, resizeData[1].Height, 0.1f, 100);
           
            basicShader.SetValue("model", model);
            basicShader.SetValue("view", view);
            

            //setlighning
            basicShader.SetValue("lightPos", new Vector3(-15, 10, -15) * 10f);
            basicShader.SetValue("viewPos", cameraPosition);
            basicShader.SetValue("lightColor", new Vector3(1.0f, 1.0f, 1.0f));
            basicShader.SetValue("isLight", (int)0);

            //render one
            blt1.MakeCurrent();
            blt1.Resize(resizeData[0].Width, resizeData[0].Height);
            basicShader.SetValue("projection", projection1);
            basicShader.SetValue("objectColor", new Vector3(1.0f, 0.5f, 0f));
            GL.Draw(teapotObject, basicShader);

            //drawSphere

            model = Matrix4x4.TranslationMatrix(new Vector3(-15, 10, -15) * 2f) * Matrix4x4.ScaleMatrix(0.25f);
            basicShader.SetValue("objectColor", new Vector3(1.0f, 1.0f, 1.0f));
            basicShader.SetValue("model", model);
            basicShader.SetValue("isLight", (int)1);
            GL.Draw(sphereObject, basicShader);
            GL.Blit(blt1);


            model = new Matrix4x4(true);
            basicShader.SetValue("model", model);
            basicShader.SetValue("isLight", (int)0);

            //render two
            blt2.MakeCurrent();
            blt2.Resize(resizeData[1].Width, resizeData[1].Height);
            basicShader.SetValue("projection", projection2);
            basicShader.SetValue("objectColor", new Vector3(0.5f, 0.5f, 1f));
            GL.Draw(teapotObject, basicShader);

            model = Matrix4x4.TranslationMatrix(new Vector3(-15, 10, -15) * 2f) * Matrix4x4.ScaleMatrix(0.25f);
            basicShader.SetValue("objectColor", new Vector3(1.0f, 1.0f, 1.0f));
            basicShader.SetValue("model", model);
            basicShader.SetValue("isLight", (int)1);
            GL.Draw(sphereObject, basicShader);

            GL.Blit(blt2);

            GLError error = GL.CheckError();
            if (error != GLError.GL_NO_ERROR)
            {
                MessageBox.Show("GLDraw Error: " + error);
                Application.Exit();
            }

            angle += 0.5f; 
        }

    }
}
