using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using glcore;
using glcore.Types;
using glcore.gl;

namespace glcore.Extras
{
    public static class LineShader
    {
        internal static Shader lineShader;

        public static void Initialize()
        {
            if (!Shader.Compile(VSS, FSS, out lineShader))
                throw new Exception("Failed To Compile LineShader!\nError:\n" + Shader.CompileLog);
        }

        static string VSS
        {
            get
            {
                return @"#version 330 core
uniform vec3 v1;
uniform vec3 v2;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
	vec3 aPos = gl_VertexID == 0 ? v1 : v2;
	gl_Position = projection * view * model * vec4(aPos, 1.0f);
}";
            }
        }

        static string FSS
        {
            get
            {
                return @"#version 330 core
out vec4 FragColor;
uniform vec4 color;

void main()
{
    FragColor = color;
}";
            }
        }

        public static void Line3D(Vector3 F, Vector3 T, Vector4 color4f, float thickness, Matrix4x4 projMat, Matrix4x4 view, Matrix4x4 model)
        {
            //GL.Draw(null, lineShader);
            lineShader.SetValue("v1", F);
            lineShader.SetValue("v2", T);
            lineShader.SetValue("model", model);
            lineShader.SetValue("view", view);
            lineShader.SetValue("projection", projMat);
            lineShader.SetValue("color", color4f);
            GLFunc.glLineWidth(thickness);

            const uint GL_LINES = 0x0001;
            GLFunc.glUseProgram(lineShader.shaderProgram);
            GLFunc.glDrawArrays(GL_LINES, 0, 2);
        }
    }
}
