﻿#version 330 core
out vec4 FragColor;

in vec3 FragPos;
in vec3 Normals;

uniform vec3 lightPos; 
uniform vec3 viewPos; 
uniform vec3 lightColor;
uniform vec3 objectColor;
uniform int isLight;

void main()
{
	float ambientStrength = 0.1;
    vec3 ambient = ambientStrength * lightColor;

	vec3 norm = normalize(Normals);
    vec3 lightDir = normalize(lightPos - FragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = diff * lightColor;

	float specularStrength = 0.5;
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);  
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 64);
    vec3 specular = specularStrength * spec * lightColor;  

    vec3 result = (ambient + diffuse + specular) * objectColor;
 //   vec3 destCol = vec3(FragPos.x / 100.0 + 0.5, FragPos.y / 100.0 + 0.5, FragPos.z / 100.0 + 0.5);
// vec3 destCol = vec3(FragPos.z / 50 + 0.5, 0, 0);

	FragColor = isLight == 0 ? vec4(result, 1.0) : vec4(lightColor, 1.0f);
    
//	vec3 destCol = vec3(0.5, 0.5, 0.5);

	//FragColor = vec4(destCol + result * 0.01f, 1.0);
}