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
            this.ClientSize = new Size(800, 800);
            formData = new BlitData(this);
            inputManager = new InputManager(this);
           // inputManager.cameraPosition = new Vector3(0, 0, -10);

            if (!Shader.Compile(vertexShaderSource, fragmentShaderSource, out myShader))
                throw new Exception("Failed to compile! Reason:\n" + Shader.CompileLog);

            if (!Shader.Compile(File.ReadAllText(@"Shaders\SimpleVS.vs"), File.ReadAllText(@"Shaders\SimpleFS.fs"), out teapotShader))
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

           // teapot = ToGLBuffer.ToBuffer(sImport);
           // teapot = square;

          //  if (false)
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
            }, typeof(Vector3), typeof(Vector2));


            RT = new RenderThread(144);
            RT.RenderFrame += RT_RenderFrame;

           // RT.Start();
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

                    model = new Matrix4x4(true);
                    model = Matrix4x4.TranslationMatrix(-inputManager.cameraPosition);
                //    view = new Matrix4x4(true);




                    Matrix4x4 mat44 = new Matrix4x4(true);


                    mat44.X2Y2 = -1;
                  //  mat44.X2Y3 = -1;
                 //   mat44.X3Y2 = -1;


                //    projection = Matrix4x4.PerspectiveMatrix123(90, 90, 10f, 1000f);// *mat44;
                    projection = Matrix4x4.PerspectiveMatrix(90, 90, 0.1f, 100) * mat44;
                 //   MessageBox.Show(projection.ToString());

                //    MessageBox.Show(projection.ToString());

                 //   projection = projection * mat44;

                  //  MessageBox.Show(projection.ToString());

                //    projection = new Matrix4x4(true);

                  //  MessageBox.Show(projection.ToString());

                  //  view = view * mat44;


                //    

                //    teapotShader.SetValue("pos", new Vector3(inputManager.cameraPosition.x, inputManager.cameraPosition.y, inputManager.cameraPosition.z));
                    teapotShader.SetValue("pos", Matrix4x4.TranslationMatrix(-inputManager.cameraPosition));
                    teapotShader.SetValue("camRot", inputManager.CreateCameraRotationMatrix4x4());
                 //   teapotShader.SetValue("camRot", new Matrix4x4(true));
                //    teapotShader.SetValue("camRot", Matrix4x4.RollMatrix(inputManager.cameraRotation.z));


                   // projection = Matrix4x4.OrthographicMatrix(2, 2, 0.1f, 100);
                  //  MessageBox.Show(projection.ToString());


                 //   projection = Matrix4x4.PerspectiveMatrix123(90, 90, 0.1f, 1000);

                //    MessageBox.Show(projection.ToString());

                 //   projection = Matrix4x4.OrthographicMatrix(1, -1, 1, -1, 0.1f, 100);
               //     MessageBox.Show(projection.ToString());

                 //   teapotShader.SetValue("model", model);
                //    teapotShader.SetValue("view", view);
                    teapotShader.SetValue("projection", projection);

                    GLError ERR = GL.CheckError();
                    if (ERR != GLError.GL_NO_ERROR)
                    {
                        MessageBox.Show("GLDraw Error: " + ERR);
                        Application.Exit();
                    }

                    GL.Draw(teapot, teapotShader);
                    GL.Blit(formData);
                    this.Text = deltaTime.ToString() + ", " + " -> " + inputManager.cameraPosition;
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
