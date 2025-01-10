#version 330 core

#define VERT_POSITION 0
#define VERT_NORMAL 1
#define VERT_TEXCOORD 2

layout (location = VERT_POSITION) in vec3 position;
layout (location = VERT_NORMAL) in vec3 normal;
layout (location = VERT_TEXCOORD) in vec2 texcoord;

uniform struct Transform {
    mat4 model;
    mat4 viewProjection;
    mat3 normal;
    vec3 viewPosition;
} transform;

uniform int applyWave;
uniform float time;

uniform struct SpotLight {
    vec4 position;
    vec4 ambient;
    vec4 diffuse;
    vec4 specular;
    vec3 attenuation;
    vec3 spotDirection;
    float spotCosCutoff;
    float spotExponent;
} projector;

uniform struct DirLight {
    vec4 position;
    vec4 ambient;
    vec4 diffuse;
    vec4 specular;
} light;

out Vertex {
    vec2 texcoord;
    vec3 normal;
    vec3 projectorDir;
	vec3 lightDir;
    vec3 viewDir;
    float distance;
} Vert;

void main() {
    vec4 vertex = transform.model * vec4(position, 1.0);
	
	 if (applyWave == 1) { 
		float waveAmplitude = 0.05; // Амплитуда колыхания 
		float waveSpeed = 0.4; // Скорость колыхания 
 
		vertex.x += sin(vertex.y + time * waveSpeed) * waveAmplitude; 
		vertex.z += cos(vertex.y + time * waveSpeed) * waveAmplitude * 0.5; 
	} 
	
	gl_Position = transform.viewProjection * vertex;
	Vert.texcoord = vec2(texcoord.x, 1.0f - texcoord.y);
	Vert.normal = transform.normal * normal;
	Vert.viewDir = normalize(transform.viewPosition - vec3(vertex));
	
	
    vec4 projectorDir = projector.position - vertex;
	Vert.distance = length(projectorDir);
    Vert.projectorDir = vec3(projectorDir);
    
    Vert.lightDir = vec3(light.position);
}