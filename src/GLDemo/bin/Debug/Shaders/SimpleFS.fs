﻿#version 330 core
out vec4 FragColor;
in vec2 TexCoord;

uniform sampler2D texture1;

void main()
{
    //FragColor = vec4(TexCoord.x, TexCoord.y, 0, 1.0);
	FragColor = texture(texture1, TexCoord);
}