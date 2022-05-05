using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using glcore;
using glcore.Types;
using glextras;

namespace GLDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        BlitData formData;
        Shader myShader;
        GLBuffer square;

        Shader teapotShader;
        GLBuffer teapot;
        GLTexture myTexture;

        Shader phongShader;
        GLBuffer teapotLightning;

        GLFramebuffer fbTest;

        float rotPos = 0;

        Matrix4x4 model, view, projection;

        InputManager inputManager;
        RenderThread RT;
        Stopwatch deltaTimeSW = new Stopwatch();

        static string vertexShaderSource
        {
            get
            {
                return @"#version 330 core
                layout (location = 0) in vec3 aPos;
                void main()
                {
                   gl_Position = vec4(aPos.x, aPos.y, aPos.z, 1.0);
                }";
            }
        }

        static string fragmentShaderSource
        {
            get
            {
                return @"#version 330 core
                    out vec4 FragColor;
                    void main()
                    {
                       FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
                    }";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.FormClosing += Form1_FormClosing;
            this.Resize += Form1_Resize;
            this.ClientSize = new Size(800, 800);
            formData = new BlitData(this);
            inputManager = new InputManager(this);
            inputManager.cameraPosition = new Vector3(0, 0, -1.5f);

            if (!Shader.Compile(vertexShaderSource, fragmentShaderSource, out myShader))
                throw new Exception("Failed to compile! Reason:\n" + Shader.CompileLog);

            if (!Shader.Compile(File.ReadAllText(@"Shaders\SimpleVS.vs"), File.ReadAllText(@"Shaders\SimpleFS.fs"), out teapotShader))
                throw new Exception("Failed to compile! Reason:\n" + Shader.CompileLog);

            if (!Shader.Compile(File.ReadAllText(@"Shaders\phongVS.vs"), File.ReadAllText(@"Shaders\phongFS.fs"), out phongShader))
                throw new Exception("Failed to compile! Reason:\n" + Shader.CompileLog);

            myTexture = new GLTexture(new Bitmap("sampleTexture.png"), true);
            teapotShader.LinkTexture("texture1", myTexture);
            
            square = new GLBuffer(new float[]
            {
                 0.5f, 0.5f, 0.0f,   // top right
		         0.5f, -0.5f, 0.0f,  // bottom right
		        -0.5f, 0.5f, 0.0f,    // top left 
                -0.5f, 0.5f, 0.0f,    // top left 
                0.5f, -0.5f, 0.0f,  // bottom right
                -0.5f, -0.5f, 0.0f  // bottom left
            }, typeof(Vector3));

            
            STLImporter sImport = new STLImporter("teapot.stl");
            teapotLightning = new GLBuffer(STLImporter.AverageUpFaceNormalsAndOutputVertexBuffer(sImport.AllTriangles, 45), phongShader);


           // teapotLightning.Dispose();
           // MessageBox.Show("Test");

           // teapot = ToGLBuffer.ToBuffer(sImport);
           // teapot = square;

            if (false)
            teapot = new GLBuffer(new float[]
            {
                 0.5f, 0.5f, 0.0f, 1.0f, 1.0f,   // top right
		         0.5f, -0.5f, 0.0f, 1.0f, 0.0f,  // bottom right
		        -0.5f, 0.5f, 0.0f, 0.0f, 1.0f,    // top left 
                -0.5f, 0.5f, 0.0f, 0.0f, 1.0f,    // top left 
                0.5f, -0.5f, 0.0f, 1.0f, 0.0f,  // bottom right
                -0.5f, -0.5f, 0.0f, 0.0f, 0.0f  // bottom left
            }, typeof(Vector3), typeof(Vector2));


            if (false)
            teapot = new GLBuffer(new float[]
            {
                 -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
                 0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
                 0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                 0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
                -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

                -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
                 0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
                 0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
                 0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
                -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
                -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

                -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
                -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
                -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

                 0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
                 0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                 0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                 0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                 0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
                 0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

                -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                 0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
                 0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
                 0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
                -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
                -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

                -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
                 0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                 0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
                 0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
                -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
                -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
            }, teapotShader);


            fbTest = new GLFramebuffer(800, 800);

            RT = new RenderThread(144);
            RT.RenderFrame += RT_RenderFrame;

           // RT.Start();
        }

        void Form1_Resize(object sender, EventArgs e)
        {
            if (formData != null)
            formData.Resize(this.ClientSize.Width, this.ClientSize.Height);
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            RT.Abort();
        }

        void RT_RenderFrame()
        {
            deltaTimeSW.Stop();
            float deltaTime = (float)deltaTimeSW.Elapsed.TotalMilliseconds;
            deltaTimeSW.Restart();

            inputManager.CalculateMouseInput();
            inputManager.CalcualteKeyboardInput(deltaTime / 66.66f);

            if (!this.IsDisposed)
                this.Invoke((Action)delegate()
                {
                    GL.Clear(0.2f, 0.3f, 0.3f, 1.0f);
               //     GL.Clear(fbTest, 0.2f, 0.3f, 0.3f, 1.0f);

                    model = new Matrix4x4(true);
                    view = inputManager.CreateViewMatrix();
                    projection = Matrix4x4.PerspectiveMatrix(90, ClientSize.Width, ClientSize.Height, 0.1f, 100);

                    phongShader.SetValue("model", model);
                    phongShader.SetValue("view", view);
                    phongShader.SetValue("projection", projection);

                    GLError error = GL.CheckError();
                    if (error != GLError.GL_NO_ERROR)
                    {
                        MessageBox.Show("GLDraw Error: " + error);
                        Application.Exit();
                    }

                    GL.Draw(teapotLightning, phongShader);

                   // GL.Draw(fbTest, teapotLightning, phongShader);
                    GL.Blit(formData);
                    this.Text = deltaTime.ToString() + "ms" +", " + inputManager.cameraPosition + ", " + inputManager.cameraRotation;
                });

            rotPos += 0.5f;
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            RT.Start();
            return;
            Stopwatch sw = new Stopwatch();

            sw.Start();
            GL.Clear(0.2f, 0.3f, 0.3f, 1.0f);



            model = new Matrix4x4(true);
            view = Matrix4x4.TranslationMatrix(new Vector3(0, 0, -50));

            Matrix4x4 mat44 = new Matrix4x4(true);
            mat44.X2Y2 = -1;
            mat44.X3Y2 = -1;

           // projection = Matrix4x4.PerspectiveMatrix(90, 90, 0.3f, 1000) * mat44;




            teapotShader.SetValue("model", model);
            teapotShader.SetValue("view", view);
            teapotShader.SetValue("projection", projection);

         //   GL.Draw(square, myShader);

            GL.Draw(teapot, teapotShader);



            GL.Blit(formData);
            sw.Stop();


            
            this.Text = sw.Elapsed.TotalMilliseconds.ToString() + ", " + GL.CheckError().ToString();
            
        }
    }
}
