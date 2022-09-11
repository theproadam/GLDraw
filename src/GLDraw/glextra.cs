using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using glcore;
using glcore.Types;
using glcore.gl;

using glcore.Enums;

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

        static string VSSt
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
uniform float zOffset;

void main()
{
	vec3 aPos = gl_VertexID == 0 ? v1 : v2;
//    vec4 eye = view * model * vec4(aPos, 1.0f);
//    eye.w += zOffset;
//	gl_Position = projection * eye;

    vec4 eye = projection * view * model * vec4(aPos, 1.0f);
    eye.z -= zOffset;
	gl_Position = eye;


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

        public static void Line3D(Vector3 F, Vector3 T, Vector4 color4f, float thickness, Matrix4x4 projMat, Matrix4x4 view, Matrix4x4 model, bool ZOffset = false)
        {
           //// if (ZOffset)
           // {
           //     GLFunc.glEnable(GLEnum.GL_POLYGON_OFFSET_FILL);
           //     GLFunc.glPolygonOffset(10f, -5.0f);
           //     GLFunc.glEnable(GLEnum.GL_POLYGON_OFFSET_LINE);
           //     GLFunc.glPolygonOffset(10f, -5.0f);
           // }

            //GL.Draw(null, lineShader);
            lineShader.SetValue("v1", F);
            lineShader.SetValue("v2", T);
            lineShader.SetValue("model", model);
            lineShader.SetValue("view", view);
            lineShader.SetValue("projection", projMat);
            lineShader.SetValue("color", color4f);
            lineShader.SetValue("zOffset", ZOffset ? 0.01f : 0.0f);

            GLFunc.glLineWidth(thickness);

            const uint GL_LINES = 0x0001;
            GLFunc.glUseProgram(lineShader.shaderProgram);
            GLFunc.glDrawArrays(GL_LINES, 0, 2);

            //GLFunc.glDisable(GLEnum.GL_POLYGON_OFFSET_FILL);
            //GLFunc.glDisable(GLEnum.GL_POLYGON_OFFSET_LINE);
        }

        public static void Lines3D(Vector3[] arr, Vector4 color4f, float thickness, Matrix4x4 projMat, Matrix4x4 view, Matrix4x4 model, bool ZOffset = false)
        {

            lineShader.SetValue("model", model);
            lineShader.SetValue("view", view);
            lineShader.SetValue("projection", projMat);
            lineShader.SetValue("color", color4f);
            lineShader.SetValue("zOffset", ZOffset ? 0.01f : 0.0f);

            GLFunc.glLineWidth(thickness);

            const uint GL_LINES = 0x0001;
            GLFunc.glUseProgram(lineShader.shaderProgram);

            for (int i = 0; i < arr.Length - 1; i++)
            {
                lineShader.SetValue("v1", arr[i]);
                lineShader.SetValue("v2", arr[i + 1]);
                GLFunc.glDrawArrays(GL_LINES, 0, 2);
            }          
        }
    }
}
