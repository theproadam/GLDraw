#version 330 core
out vec4 FragColor;
in vec2 TexCoord;

uniform sampler2D screenTexture;


void main()
{
    vec3 col = texture(screenTexture, TexCoord).rgb;

	float X = (2f * gl_FragCoord.x / 800) - 1f;
    float Y = (2f * gl_FragCoord.y / 800) - 1f;

	X = 1f - 0.5f * X * X;
    Y = X * (1f - 0.5f * Y * Y);

    FragColor = vec4(col * Y, 1.0);
} 