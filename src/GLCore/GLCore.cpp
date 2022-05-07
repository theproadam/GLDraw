#include "GLCore.h"
#include <windows.h>
#include <gl\GL.h>

#define GL_FRAGMENT_SHADER 0x8B30
#define GL_VERTEX_SHADER 0x8B31
#define GL_COMPILE_STATUS 0x8B81
#define GL_LINK_STATUS 0x8B82
#define GL_ARRAY_BUFFER 0x8892
#define GL_STATIC_DRAW 0x88E4
#define GL_ELEMENT_ARRAY_BUFFER 0x8893
#define GL_TRIANGLES 0x0004
#define GL_UNSIGNED_INT 0x1405
#define GL_TEXTURE0 0x84C0
#define GL_ACTIVE_ATTRIBUTES 0x8B89
#define GL_ACTIVE_UNIFORMS 0x8B86
#define GL_FRAMEBUFFER                    0x8D40
#define GL_RENDERBUFFER                   0x8D41
#define GL_DEPTH24_STENCIL8               0x88F0
#define GL_DEPTH_STENCIL_ATTACHMENT       0x821A
#define GL_FRAMEBUFFER_COMPLETE           0x8CD5
#define GL_COLOR_ATTACHMENT0              0x8CE0

typedef signed   long  int     GLsizeiptr;

//FUNCTIONS
GLuint(__stdcall *glCreateShader)(GLenum);
void(__stdcall *glShaderSource)(GLuint, GLsizei, char**, const GLint*);
void(__stdcall *glCompileShader)(GLuint shader);

void(__stdcall *glGetShaderiv)(GLuint shader, GLenum pname, GLint *params);
void(__stdcall *glGetShaderInfoLog)(GLuint shader, GLsizei maxLength, GLsizei *length, char *infoLog);

GLuint(__stdcall *glCreateProgram)(void);
void(__stdcall *glAttachShader)(GLuint program, GLuint shader);
void(__stdcall *glLinkProgram)(GLuint program);
void(__stdcall *glGetProgramiv)(GLuint program, GLenum pname, GLint *params);
void(__stdcall *glDeleteShader)(GLuint shader);
void(__stdcall *glGetProgramInfoLog)(GLuint program, GLsizei maxLength, GLsizei *length, char *infoLog);

void(__stdcall *glGenVertexArrays)(GLsizei n, GLuint *arrays);
void(__stdcall *glGenBuffers)(GLsizei n, GLuint * buffers);
void(__stdcall *glBindVertexArray)(GLuint _array);

void(__stdcall *glBindBuffer)(GLenum target, GLuint buffer);
void(__stdcall *glBufferData)(GLenum target, GLsizeiptr size, void* data, GLenum usage);

void(__stdcall *glVertexAttribPointer)(GLuint index, GLint size, GLenum type, GLboolean normalized, GLsizei stride, void * pointer);
void(__stdcall *glEnableVertexAttribArray)(GLuint index);

void(__stdcall *glUseProgram)(GLuint program);

//void(__stdcall *glDrawElements)(GLenum mode, GLsizei count, GLenum type, void * indices);



GLint(__stdcall *glGetUniformLocation)(GLuint program, char *name);

void(__stdcall *glUniformMatrix4fv)(GLint location, GLsizei count, GLboolean transpose, GLfloat *value);

void(__stdcall *glUniform3fv)(GLint location, GLsizei count, GLfloat *value);
void(__stdcall *glUniformMatrix3fv)(GLint location, GLsizei count, GLboolean transpose, GLfloat *valu);

void(__stdcall *glGenerateMipmap)(GLenum target);

void(__stdcall *glUniform1i)(GLint location, GLint v0);
void(__stdcall *glActiveTexture)(GLenum texture);
void(__stdcall *glGetActiveUniform)(GLuint program, GLuint index, GLsizei bufSize, GLsizei *length, GLint *size, GLenum *type, char *name);
void(__stdcall *glGetActiveAttrib)(GLuint program, GLuint index, GLsizei bufSize, GLsizei *length, GLint *size, GLenum *type, char *name);

void(__stdcall *glDeleteVertexArrays)(GLsizei n, GLuint *arrays);
void(__stdcall *glDeleteBuffers)(GLsizei n, GLuint * buffers);

void(__stdcall *glGenFramebuffers)(GLsizei n, GLuint *ids);
void(__stdcall *glBindFramebuffer)(GLenum target, GLuint framebuffer);
void(__stdcall *glGenRenderbuffers)(GLsizei n, GLuint *renderbuffers);
GLenum(__stdcall *glCheckFramebufferStatus)(GLenum target);
void(__stdcall *glBindRenderbuffer)(GLenum target, GLuint renderbuffer);
void(__stdcall *glRenderbufferStorage)(GLenum target, GLenum internalformat, GLsizei width, GLsizei height);
void(__stdcall *glFramebufferRenderbuffer)(GLenum target, GLenum attachment, GLenum renderbuffertarget, GLuint renderbuffer);
void(__stdcall *glFramebufferTexture2D)(GLenum target, GLenum attachment, GLenum textarget, GLuint texture, GLint level);
bool(__stdcall *wglChoosePixelFormatARB)(void* hDC, int* piAttribIList, float* pfAttribFList, unsigned int nMaxFormats, int* piFormats, unsigned int* nNumFormats);
void(__stdcall *glBlitFramebuffer)(GLint srcX0, GLint srcY0, GLint srcX1, GLint srcY1, GLint dstX0, GLint dstY0, GLint dstX1, GLint dstY1, GLbitfield mask, GLenum filter);

#define GL_DRAW_FRAMEBUFFER_BINDING       0x8CA6
#define GL_RENDERBUFFER_BINDING           0x8CA7
#define GL_READ_FRAMEBUFFER               0x8CA8
#define GL_DRAW_FRAMEBUFFER               0x8CA9

char *vertexShaderSource = "#version 330 core\n"
"layout (location = 0) in vec3 aPos;\n"
"void main()\n"
"{\n"
"   gl_Position = vec4(aPos.x, aPos.y, aPos.z, 1.0);\n"
"}\0";
char *fragmentShaderSource = "#version 330 core\n"
"out vec4 FragColor;\n"
"void main()\n"
"{\n"
"   FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);\n"
"}\n\0";

#define uint unsigned int

bool initialized = false;

enum DataType
{
	vec3 = 0,// = 4 * 3,
	vec2 = 1,// = 4 * 2,
	fp32 = 2,// = 4,
	int32 = 3,// = 4,
	int2 = 4,// = 4 * 2,
	byte4 = 5,// = 4,
	mat4 = 6,
	mat3 = 7,
	sampler2D = 8,
	sampler1D = 9,
	samplerCube = 10
};

extern "C"
{
	DLL int wglSetPixelFormat(void* DC, int* aList, float* fList, unsigned int nMaxFormats, int* nPixelFormat2, unsigned int* nNumFormat)
	{
		bool result = wglChoosePixelFormatARB(DC, aList, fList, nMaxFormats, nPixelFormat2, nNumFormat);

		return result ? 1 : 0;
	}

	DLL void GetActiveUniform(unsigned int program, unsigned int index, int bufSize, int* length, int* size, int* type, char* name)
	{
		glGetActiveUniform(program, index, bufSize, length, size, (GLenum*)type, name);
	}

	DLL void GetActiveAttrib(unsigned int program, unsigned int index, int bufSize, int* length, int* size, int* type, char* name)
	{
		glGetActiveAttrib(program, index, bufSize, length, size, (GLenum*)type, name);
	}

	DLL void GetShaderConfig(unsigned int program, int* attributes, int* uniforms)
	{
		glUseProgram(program);

		glGetProgramiv(program, GL_ACTIVE_ATTRIBUTES, attributes);
		glGetProgramiv(program, GL_ACTIVE_UNIFORMS, uniforms);
	}

	DLL unsigned int CompileShaders(byte* vsData, byte* fsData, long* errorCheck, char* infoLog)
	{
		char* vD = (char*)vsData;
		char* fD = (char*)fsData;

		unsigned int vertexShader = glCreateShader(GL_VERTEX_SHADER);

		glShaderSource(vertexShader, 1, &vD, NULL);
		glCompileShader(vertexShader);


		
	
		// check for shader compile errors
		int success;
		glGetShaderiv(vertexShader, GL_COMPILE_STATUS, &success);
		if (!success)
		{
			glGetShaderInfoLog(vertexShader, 512, NULL, infoLog);
			*errorCheck = 1;
			return 0;
		}

		

		// fragment shader
		unsigned int fragmentShader = glCreateShader(GL_FRAGMENT_SHADER);
		glShaderSource(fragmentShader, 1, &fD, NULL);
		glCompileShader(fragmentShader);
		// check for shader compile errors
		glGetShaderiv(fragmentShader, GL_COMPILE_STATUS, &success);
		if (!success)
		{
			glGetShaderInfoLog(fragmentShader, 512, NULL, infoLog);
			*errorCheck = 2;
			return 0;
		}

		// link shaders
		unsigned int shaderProgram = glCreateProgram();
		glAttachShader(shaderProgram, vertexShader);
		glAttachShader(shaderProgram, fragmentShader);
		glLinkProgram(shaderProgram);
		// check for linking errors
		glGetProgramiv(shaderProgram, GL_LINK_STATUS, &success);
		if (!success) {
			glGetProgramInfoLog(shaderProgram, 512, NULL, infoLog);
			*errorCheck = 3;
			return 0;
		}

		glDeleteShader(vertexShader);
		glDeleteShader(fragmentShader);

		return shaderProgram;
	}

	DLL void CreateBufferIndices(unsigned int* VAO, unsigned int* VBO, unsigned int* EBO, int vertSize, void* vertices, int indiSize, void* indices)
	{
		glGenVertexArrays(1, VAO);
		glGenBuffers(1, VBO);
		glGenBuffers(1, EBO);
		// bind the Vertex Array Object first, then bind and set vertex buffer(s), and then configure vertex attributes(s).
		glBindVertexArray(*VAO);

		glBindBuffer(GL_ARRAY_BUFFER, *VBO);
		glBufferData(GL_ARRAY_BUFFER, vertSize, vertices, GL_STATIC_DRAW);

		glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, *EBO);
		glBufferData(GL_ELEMENT_ARRAY_BUFFER, indiSize, indices, GL_STATIC_DRAW);

		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 3 * sizeof(float), (void*)0);
		glEnableVertexAttribArray(0);

		// note that this is allowed, the call to glVertexAttribPointer registered VBO as the vertex attribute's bound vertex buffer object so afterwards we can safely unbind
		glBindBuffer(GL_ARRAY_BUFFER, 0);


		// remember: do NOT unbind the EBO while a VAO is active as the bound element buffer object IS stored in the VAO; keep the EBO bound.
		//glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, 0);

		// You can unbind the VAO afterwards so other VAO calls won't accidentally modify this VAO, but this rarely happens. Modifying other
		// VAOs requires a call to glBindVertexArray anyways so we generally don't unbind VAOs (nor VBOs) when it's not directly necessary.
		glBindVertexArray(0);
	}

	DLL void CreateBuffer(unsigned int* VAO, unsigned int* VBO, int vertSize, void* vertices, int Stride, int attribs, int* attribSize)
	{
		glGenVertexArrays(1, VAO);
		glGenBuffers(1, VBO);
		// bind the Vertex Array Object first, then bind and set vertex buffer(s), and then configure vertex attributes(s).
		glBindVertexArray(*VAO);

		glBindBuffer(GL_ARRAY_BUFFER, *VBO);
		glBufferData(GL_ARRAY_BUFFER, vertSize, vertices, GL_STATIC_DRAW);

		int offset = 0;
		
		for (int i = 0; i < attribs; i++)
		{
			glVertexAttribPointer(i, attribSize[i], GL_FLOAT, GL_FALSE, Stride * sizeof(float), (void*)(offset * sizeof(float)));
			glEnableVertexAttribArray(i);
			offset += attribSize[i];
		}


		//glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 3 * sizeof(float), (void*)0);
		//glEnableVertexAttribArray(0);

		// note that this is allowed, the call to glVertexAttribPointer registered VBO as the vertex attribute's bound vertex buffer object so afterwards we can safely unbind
		glBindBuffer(GL_ARRAY_BUFFER, 0);


		// remember: do NOT unbind the EBO while a VAO is active as the bound element buffer object IS stored in the VAO; keep the EBO bound.
		//glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, 0);

		// You can unbind the VAO afterwards so other VAO calls won't accidentally modify this VAO, but this rarely happens. Modifying other
		// VAOs requires a call to glBindVertexArray anyways so we generally don't unbind VAOs (nor VBOs) when it's not directly necessary.
		glBindVertexArray(0);
	}

	DLL void DeleteBuffer(uint VAO, uint VBO)
	{
		glDeleteVertexArrays(1, &VAO);
		glDeleteBuffers(1, &VBO);
	}

	DLL void DeleteTexture(uint textures)
	{
		glDeleteTextures(1, &textures);
	}

	DLL int LinkTexture(char* name, unsigned int shaderProgram, unsigned int textureID, unsigned int textureUnit)
	{
		glUseProgram(shaderProgram);
		GLint location = glGetUniformLocation(shaderProgram, name);

		glUniform1i(location, textureUnit);
		
		return location;
	}

	DLL void CreateTexture(unsigned int* textureID, int width, int height, int stride, void* data)
	{
		glGenTextures(1, textureID);
		glBindTexture(GL_TEXTURE_2D, *textureID);

		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);

		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);

		glTexImage2D(GL_TEXTURE_2D, 0, stride == 4 ? GL_RGBA : GL_RGB, width, height, 0, stride == 4 ? GL_RGBA : GL_RGB, GL_UNSIGNED_BYTE, data);
		glGenerateMipmap(GL_TEXTURE_2D);
	}

	DLL void ChangeCurrentFrameBuffer(unsigned int newFBO)
	{
		glBindFramebuffer(GL_FRAMEBUFFER, newFBO);
	}

	DLL void ChangeFrameBufferTexture(unsigned int text)
	{
		glBindTexture(GL_TEXTURE_2D, text);
	}

	DLL void CopyFrameBuffer(uint fbo1, uint fbo2, uint tex1, uint tex2, int width, int height)
	{

		int c;

		c = glGetError();

		glBindFramebuffer(GL_FRAMEBUFFER, fbo1);
		glBindFramebuffer(GL_DRAW_FRAMEBUFFER, fbo1);

		c = glGetError();

		glFramebufferTexture2D(GL_TEXTURE_2D, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, tex1, 0);

		c = glGetError();

		glDrawBuffer(GL_COLOR_ATTACHMENT0);

		c = glGetError();

		glBindFramebuffer(GL_READ_FRAMEBUFFER, fbo2);
		glFramebufferTexture2D(GL_TEXTURE_2D, GL_COLOR_ATTACHMENT0 + 1, GL_TEXTURE_2D, tex2, 0);
		glReadBuffer(GL_COLOR_ATTACHMENT0 + 1);

		c = glGetError();

		glBlitFramebuffer(0, 0, width, height,
			0, 0, width, height,
			GL_COLOR_BUFFER_BIT, GL_NEAREST);

		c = glGetError();

		glBindTexture(GL_TEXTURE_2D, 0);
		glBindFramebuffer(GL_READ_FRAMEBUFFER, 0);
		glBindFramebuffer(GL_DRAW_FRAMEBUFFER, 0);
	}

	DLL int CreateFrameBuffer(uint* rbo, uint* framebuffer, uint* texColBuf, uint width, uint height)
	{
		glGenFramebuffers(1, framebuffer);
		glBindFramebuffer(GL_FRAMEBUFFER, *framebuffer);


		glGenTextures(1, texColBuf);
		glBindTexture(GL_TEXTURE_2D, *texColBuf);
		glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, width, height, 0, GL_RGB, GL_UNSIGNED_BYTE, NULL);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);

		glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, *texColBuf, 0);


		glGenRenderbuffers(1, rbo);
		glBindRenderbuffer(GL_RENDERBUFFER, *rbo);
		glRenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH24_STENCIL8, width, height);
		glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_DEPTH_STENCIL_ATTACHMENT, GL_RENDERBUFFER, *rbo);

		
		// now that we actually created the framebuffer and added all attachments we want to check if it is actually complete now
		if (glCheckFramebufferStatus(GL_FRAMEBUFFER) != GL_FRAMEBUFFER_COMPLETE)
		{
			return -1;
		}

		//switch back to default framebuffer!
		glBindFramebuffer(GL_FRAMEBUFFER, 0);

		return 0;
	}

	DLL int SetValue(char* name, unsigned int shaderProgram, int type, void* data)
	{
		//probably shouldnt be doing this each time
		glUseProgram(shaderProgram);

		GLint location = glGetUniformLocation(shaderProgram, name);
		DataType t = (DataType)type;

		if (t == mat4)
		{
			glUniformMatrix4fv(location, 1, true, (float*)data);
		}
		else if (t == mat3)
		{
			glUniformMatrix3fv(location, 1, true, (float*)data);
		}
		else if (t == vec3)
		{
			glUniform3fv(location, 1, (float*)data); 
		}
		else if (t == int32)
		{
			glUniform1i(location, *(int*)data);
		}

		return location;
	}

	DLL void BindTextures(unsigned int* textureIDs, int textureCount)
	{
		for (int i = 0; i < textureCount; i++)
		{
			glActiveTexture(GL_TEXTURE0 + i); //glActiveTexture(GL_TEXTURE0);
			glBindTexture(GL_TEXTURE_2D, textureIDs[i]);
		}
	}

	DLL void Draw(unsigned int shaderProgram, unsigned int start, unsigned int stop, unsigned int VAO)
	{
		glUseProgram(shaderProgram);
		glBindVertexArray(VAO);
		//glBindFramebuffer(GL_FRAMEBUFFER, 0);

		int hasINDICES = 0;

	//	if (hasINDICES)
	//		glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_INT, 0);
	//	else

		glDrawArrays(GL_TRIANGLES, start, stop);

		//glBindVertexArray(0);
	}

	DLL int GetError()
	{
		return glGetError();
	}

	DLL int Initialize()
	{
		if (initialized)
			return 1;


		if ((glCreateShader = (GLuint(__stdcall *)(GLenum))wglGetProcAddress("glCreateShader")) == NULL) return 0;		
		if ((glShaderSource = (void(__stdcall *)(GLuint, GLsizei, char**, const GLint*))wglGetProcAddress("glShaderSource")) == NULL) return 0;

		if ((glCompileShader = (void(__stdcall *)(GLuint shader))wglGetProcAddress("glCompileShader")) == NULL)
			return 0;
		
		if ((glGetShaderiv = (void(__stdcall *)(GLuint shader, GLenum pname, GLint *params))wglGetProcAddress("glGetShaderiv")) == NULL)
			return 0;

		if ((glGetShaderInfoLog = (void(__stdcall *)(GLuint shader, GLsizei maxLength, GLsizei *length, char *infoLog))wglGetProcAddress("glGetShaderInfoLog")) == NULL)
			return 0;

		if ((glCreateProgram = (GLuint(__stdcall *)())wglGetProcAddress("glCreateProgram")) == NULL)
			return 0;

		if ((glAttachShader = (void(__stdcall *)(GLuint, GLuint))wglGetProcAddress("glAttachShader")) == NULL)
			return 0;

		if ((glLinkProgram = (void(__stdcall *)(GLuint))wglGetProcAddress("glLinkProgram")) == NULL)
			return 0;

		if ((glGetProgramiv = (void(__stdcall *)(GLuint, GLenum, GLint*))wglGetProcAddress("glGetProgramiv")) == NULL)
			return 0;

		if ((glDeleteShader = (void(__stdcall *)(GLuint))wglGetProcAddress("glDeleteShader")) == NULL)
			return 0;

		if ((glGetProgramInfoLog = (void(__stdcall *)(GLuint, GLsizei, GLsizei*, char*))wglGetProcAddress("glGetProgramInfoLog")) == NULL)
			return 0;

		if ((glGenVertexArrays = (void(__stdcall *)(GLsizei, GLuint*))wglGetProcAddress("glGenVertexArrays")) == NULL) return 0;
		if ((glGenBuffers = (void(__stdcall *)(GLsizei, GLuint*))wglGetProcAddress("glGenBuffers")) == NULL) return 0;
		if ((glBindVertexArray = (void(__stdcall *)(GLuint))wglGetProcAddress("glBindVertexArray")) == NULL) return 0;		
		if ((glBindBuffer = (void(__stdcall *)(GLenum, GLuint))wglGetProcAddress("glBindBuffer")) == NULL) return 0;
		if ((glBufferData = (void(__stdcall *)(GLenum, GLsizeiptr, void*, GLenum))wglGetProcAddress("glBufferData")) == NULL) return 0;

		if ((glVertexAttribPointer = (void(__stdcall *)(GLuint, GLint, GLenum, GLboolean, GLsizei, void*))wglGetProcAddress("glVertexAttribPointer")) == NULL) return 0;
		if ((glEnableVertexAttribArray = (void(__stdcall *)(GLuint))wglGetProcAddress("glEnableVertexAttribArray")) == NULL) return 0;
		if ((glUseProgram = (void(__stdcall *)(GLuint))wglGetProcAddress("glUseProgram")) == NULL) return 0;

		//if ((glDrawElements = (void(__stdcall *)(GLenum, GLsizei, GLenum, void*))wglGetProcAddress("glDrawElements")) == NULL) return 0;
		
		

		if ((glGetUniformLocation = (GLint(__stdcall *)(GLuint, char*))wglGetProcAddress("glGetUniformLocation")) == NULL)
			return 0;

		if ((glUniformMatrix4fv = (void(__stdcall *)(GLint, GLsizei, GLboolean, GLfloat*))wglGetProcAddress("glUniformMatrix4fv")) == NULL)
			return 0;

		if ((glUniform3fv = (void(__stdcall *)(GLint, GLsizei, GLfloat*))wglGetProcAddress("glUniform3fv")) == NULL)
			return 0;

		if ((glUniformMatrix3fv = (void(__stdcall *)(GLint, GLsizei, GLboolean, GLfloat*))wglGetProcAddress("glUniformMatrix3fv")) == NULL)
			return 0;

		if ((glGenerateMipmap = (void(__stdcall *)(GLenum))wglGetProcAddress("glGenerateMipmap")) == NULL)
			return 0;

		if ((glUniform1i = (void(__stdcall *)(GLint, GLint))wglGetProcAddress("glUniform1i")) == NULL)
			return 0;

		if ((glActiveTexture = (void(__stdcall *)(GLenum))wglGetProcAddress("glActiveTexture")) == NULL)
			return 0;

		if ((glGetActiveUniform = (void(__stdcall *)(GLuint, GLuint, GLsizei, GLsizei*, GLint*, GLenum*, char*))wglGetProcAddress("glGetActiveUniform")) == NULL)
			return 0;

		if ((glGetActiveAttrib = (void(__stdcall *)(GLuint, GLuint, GLsizei, GLsizei*, GLint*, GLenum*, char*))wglGetProcAddress("glGetActiveAttrib")) == NULL)
			return 0;

		if ((glDeleteVertexArrays = (void(__stdcall *)(GLsizei, GLuint*))wglGetProcAddress("glDeleteVertexArrays")) == NULL)
			return 0;

		if ((glDeleteBuffers = (void(__stdcall *)(GLsizei, GLuint*))wglGetProcAddress("glDeleteBuffers")) == NULL)
			return 0;

		if ((glGenFramebuffers = (void(__stdcall *)(GLsizei, GLuint*))wglGetProcAddress("glGenFramebuffers")) == NULL)
			return 0;

		if ((glBindFramebuffer = (void(__stdcall *)(GLenum, GLuint))wglGetProcAddress("glBindFramebuffer")) == NULL)
			return 0;

		if ((glGenRenderbuffers = (void(__stdcall *)(GLsizei, GLuint*))wglGetProcAddress("glGenRenderbuffers")) == NULL)
			return 0;

		if ((glCheckFramebufferStatus = (GLenum(__stdcall *)(GLenum))wglGetProcAddress("glCheckFramebufferStatus")) == NULL)
			return 0;

		if ((glBindRenderbuffer = (void(__stdcall *)(GLenum, GLuint))wglGetProcAddress("glBindRenderbuffer")) == NULL)
			return 0;

		if ((glRenderbufferStorage = (void(__stdcall *)(GLenum, GLenum, GLsizei, GLsizei))wglGetProcAddress("glRenderbufferStorage")) == NULL)
			return 0;

		if ((glFramebufferRenderbuffer = (void(__stdcall *)(GLenum, GLenum, GLenum, GLuint))wglGetProcAddress("glFramebufferRenderbuffer")) == NULL)
			return 0;

		if ((glFramebufferTexture2D = (void(__stdcall *)(GLenum, GLenum, GLenum, GLuint, GLint))wglGetProcAddress("glFramebufferTexture2D")) == NULL)
			return 0;


		if ((wglChoosePixelFormatARB = (bool(__stdcall *)(void*, int*, float*, unsigned int, int*, unsigned int*))wglGetProcAddress("wglChoosePixelFormatARB")) == NULL)
			return 0;

		if ((glBlitFramebuffer = (void(__stdcall *)(GLint, GLint, GLint, GLint, GLint, GLint, GLint, GLint, GLbitfield, GLenum))wglGetProcAddress("glBlitFramebuffer")) == NULL)
			return 0;

		glEnable(GL_DEPTH_TEST);
		glEnable(0x809D); //Multisample

	//	glCullFace(GL_FRONT);

		initialized = true;

		return 1;
	}
}