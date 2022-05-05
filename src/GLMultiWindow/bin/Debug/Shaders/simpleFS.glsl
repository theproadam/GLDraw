#version 330 core
out vec4 FragColor;
in vec3 Normals;

void main()
{
    FragColor = vec4(Normals.x * 0.5 + 0.5, Normals.y * 0.5 + 0.5, Normals.z * 0.5 + 0.5, 1.0);
}