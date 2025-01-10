//main.cpp

#include <cmath>
#include <SFML/Window.hpp>
#include <SFML/Graphics.hpp>
#include <GL/glew.h>
#include <GL/gl.h>
#include <iostream>
#include <random>
#include <set>
#include "model.h"
#include "shader.h"
#include "glm/gtc/matrix_transform.hpp"
#include "glm/gtc/type_ptr.hpp"

const std::string tree_model_path = "data/tree.obj";
const std::string tree_texture_path = "data/tree.png";

const std::string present_model_path = "data/present.obj";
const std::string present_texture_path = "data/present.png";

const std::string floor_model_path = "data/floor.obj";
const std::string floor_texture_path = "data/floor.png";

const std::string airship_model_path = "data/airship.obj";
const std::string airship_texture_path = "data/airship.png";

const std::string target_model_path = "data/snowman.obj";
const std::string target_texture_path = "data/snowman.png";

Model tree_model;
Model floor_model;
Model airship_model;
Model present_model;
Model target_model;

struct Camera {
	glm::vec3 cameraPos;
	glm::vec3 cameraFront;
	glm::vec3 cameraUp;
};

Camera free_camera { 
	glm::vec3(0.0f, 1.0f, 3.0f),
	glm::vec3(0.0f, 0.0f, -1.0f),
	glm::vec3(0.0f, 1.0f, 0.0f),
};

Camera airship_camera;

Camera* camera = &airship_camera;

float yaw = -90.0f;
float pitch = 0.0f;

struct Light {
	glm::vec4 position;       // Позиция источника света
	glm::vec3 spotDirection;  // Направление прожектора
	float spotCosCutoff;      // Косинус угла отсечения
	float spotExponent;       // Коэффициент экспоненциального затухания
	glm::vec3 attenuation;    // Коэффициенты затухания (constant, linear, quadratic)
	glm::vec4 ambient;        // Фоновая составляющая
	glm::vec4 diffuse;        // Рассеянная составляющая
	glm::vec4 specular;       // Зеркальная составляющая
};

Light projector = {
	glm::vec4(0.0f, 10.0f, 0.0f, 1.0f),  // Позиция прожектора (например, на высоте 5, по оси Y)
	glm::vec3(0.0f, -1.0f, 0.0f),  // Направление светового луча вниз по оси Y
	cos(glm::radians(20.0f)),  // Угол отсечения 30 градусов (косинус угла отсечения)
	2.0f,  // Коэффициент экспоненциального затухания (можно регулировать для более мягкого или резкого падения света)
	glm::vec3(1.f, 0.01f, 0.001f),  // Коэффициенты затухания: (constant, linear, quadratic)
	glm::vec4(0.1f, 0.1f, 0.1f, 1.0f),  // Фоновая составляющая (слабое освещение)
	glm::vec4(1.f, 1.f, 1.f, 1.0f),  // Рассеянная составляющая (освещает объекты белым светом)
	glm::vec4(1.0f, 1.0f, 1.0f, 1.0f)  // Зеркальная составляющая (белый свет для отражений)
};

Light light = {
	glm::vec4(0.0f, 15.0f, 0.0f, 1.0f),  // Позиция прожектора (например, на высоте 5, по оси Y)
	glm::vec3(0.0f, -1.0f, 0.0f),  // Направление светового луча вниз по оси Y
	cos(glm::radians(20.0f)),  // Угол отсечения 30 градусов (косинус угла отсечения)
	1.0f,  // Коэффициент экспоненциального затухания (можно регулировать для более мягкого или резкого падения света)
	glm::vec3(1.f, 0.1f, 0.01f),  // Коэффициенты затухания: (constant, linear, quadratic)
	glm::vec4(0.1f, 0.1f, 0.1f, 1.0f),  // Фоновая составляющая (слабое освещение)
	glm::vec4(1.f, 1.f, 1.f, 1.0f),  // Рассеянная составляющая (освещает объекты белым светом)
	glm::vec4(1.0f, 1.0f, 1.0f, 1.0f)  // Зеркальная составляющая (белый свет для отражений)
};

// ID шейдерной программы
GLuint Program;

void InitShader() {
	Program = load_shaders("shaders/toon_shader.vert", "shaders/toon_shader.frag");
}

glm::vec3 airship_position = glm::vec3(0.0f, 3.0f, 0.0f);
bool airship_dir = +1;
glm::vec3 present_position;
std::vector<glm::vec3> targets;
bool present_exists = false;
float target_radius = 0.5f;
float present_radius = 0.5f;
constexpr int TARGETS_COUNT = 5;
int kill_count = 0;
bool freeze = false;

void SpawnNewTarget() {
	static int border = 20;
	static std::random_device dev;
	static std::mt19937 rng(dev());
	static std::uniform_int_distribution<std::mt19937::result_type> dist(0, 2 * border);

	while (true) {
		bool sucess = true;
		int rval = dist(rng) - border;
		if (rval == 0) continue;
		for (auto& target : targets) {
			if (target.x == rval) {
				sucess = false;
				break;
			}
		}

		if (sucess) {
			targets.emplace_back(rval, 0, -0.15);
			return;
		}
	}
}

void Update() {
	static float time = 0;
	static float eps = 1e-4;
	static float airship_speed = 0.065;
	static float present_fall_speed = 0.065;
	constexpr static float delta_time_to_turn = 650;
	static float next_turn_time = delta_time_to_turn;
	if (freeze) return;
	++time;

	// update airship position
	if (abs((time + delta_time_to_turn / 2) - next_turn_time) < eps) {
		next_turn_time += delta_time_to_turn;
		airship_dir = !airship_dir;
	}
	if (airship_dir)
		airship_position[0] += airship_speed;
	else
		airship_position[0] -= airship_speed;
	projector.position = glm::vec4(airship_position, 1.0f);

	// update airship camera position
	if (camera == &airship_camera) {
		glm::vec3 new_camera_pos = airship_position;
		new_camera_pos.y += 5;
		new_camera_pos.x -= airship_dir ? 7 : -7;
		camera->cameraPos = new_camera_pos;
		camera->cameraFront = glm::vec3((airship_dir ? 1.f : -1.f), -1.f, .0f);
		camera->cameraUp = glm::vec3((airship_dir ? 1.f : -1.f), 1.f, .0f);
	}

	// update present position
	if (present_exists) {
		present_position.y -= present_fall_speed;
		if (present_position.y < 0)
			present_exists = false;
		else {
			for (auto it = targets.begin(); it != targets.end(); ++it) {
				if (glm::distance(present_position, *it) <
					target_radius * target_radius + present_radius * present_radius) {
					targets.erase(it);
					++kill_count;
					SpawnNewTarget();
					break;
				}
			}
		}
	}
}

void InitScene() {
	for (int i = 0; i < TARGETS_COUNT; ++i) {
		SpawnNewTarget();
	}
}

void InitModels() {
	tree_model = Model(tree_model_path, tree_texture_path);
	floor_model = Model(floor_model_path, floor_texture_path);
	airship_model = Model(airship_model_path, airship_texture_path);
	present_model = Model(present_model_path, present_texture_path);
	target_model = Model(target_model_path, target_texture_path);
}

void Init() {
	// Шейдеры
	InitShader();
	glEnable(GL_DEPTH_TEST);
	glClearColor(0.5, 0.5, 0.5, 0.0);
	InitModels();
}

float angleX = 0.0f;
float angleY = 0.0f;

float aspectRatio;

void DrawModel(const Model& object, const glm::mat4& model, GLuint Program) {
	const glm::mat3 normalMatrix = glm::transpose(glm::inverse(glm::mat3(model)));
	glUniformMatrix4fv(glGetUniformLocation(Program, "transform.model"), 1, GL_FALSE, glm::value_ptr(model));
	glUniformMatrix3fv(glGetUniformLocation(Program, "transform.normal"), 1, GL_FALSE, glm::value_ptr(normalMatrix));
	object.display_model(Program);
}

void Draw() {
	glUseProgram(Program); // Устанавливаем шейдерную программу текущей
	glm::vec3 front;
	front.x = cos(glm::radians(yaw)) * cos(glm::radians(pitch));
	front.y = sin(glm::radians(pitch));
	front.z = sin(glm::radians(yaw)) * cos(glm::radians(pitch));
	free_camera.cameraFront = glm::normalize(front);

	static float time = 0;
	time += 0.1f;
	glUniform1f(glGetUniformLocation(Program, "time"), time);

	glm::mat4 model;

	glm::mat4 view = glm::lookAt(camera->cameraPos, camera->cameraPos + camera->cameraFront, camera->cameraUp);
	glUniformMatrix4fv(glGetUniformLocation(Program, "view"), 1, GL_FALSE, glm::value_ptr(view));
	glm::mat4 projection = glm::perspective(glm::radians(45.0f), aspectRatio, 0.1f, 100.0f);
	glUniformMatrix4fv(glGetUniformLocation(Program, "transform.viewProjection"), 1, GL_FALSE, glm::value_ptr(projection * view));
	glUniform3fv(glGetUniformLocation(Program, "transform.viewPosition"), 1, glm::value_ptr(camera->cameraPos));

	// Projector
	glUniform4fv(glGetUniformLocation(Program, "projector.position"), 1, glm::value_ptr(projector.position));
	glUniform4fv(glGetUniformLocation(Program, "projector.ambient"), 1, glm::value_ptr(projector.ambient));
	glUniform4fv(glGetUniformLocation(Program, "projector.diffuse"), 1, glm::value_ptr(projector.diffuse));
	glUniform4fv(glGetUniformLocation(Program, "projector.specular"), 1, glm::value_ptr(projector.specular));
	glUniform3fv(glGetUniformLocation(Program, "projector.attenuation"), 1, glm::value_ptr(projector.attenuation));
	glUniform3fv(glGetUniformLocation(Program, "projector.spotDirection"), 1, glm::value_ptr(projector.spotDirection));
	glUniform1f(glGetUniformLocation(Program, "projector.spotCosCutoff"), projector.spotCosCutoff);
	glUniform1f(glGetUniformLocation(Program, "projector.spotExponent"), projector.spotExponent);

	// Light
	glUniform4fv(glGetUniformLocation(Program, "light.position"), 1, glm::value_ptr(light.position));
	glUniform4fv(glGetUniformLocation(Program, "light.ambient"), 1, glm::value_ptr(light.ambient));
	glUniform4fv(glGetUniformLocation(Program, "light.diffuse"), 1, glm::value_ptr(light.diffuse));
	glUniform4fv(glGetUniformLocation(Program, "light.specular"), 1, glm::value_ptr(light.specular));


	glUniform1i(glGetUniformLocation(Program, "material.texture"), 0);
	glUniform4f(glGetUniformLocation(Program, "material.ambient"), 1.0f, 1.0f, 1.0f, 1.0f);
	glUniform4f(glGetUniformLocation(Program, "material.diffuse"), 1.0f, 1.0f, 1.0f, 1.0f);
	glUniform4f(glGetUniformLocation(Program, "material.specular"), 1.0f, 1.0f, 1.0f, 1.0f);
	glUniform4f(glGetUniformLocation(Program, "material.emission"), 0.0f, 0.0f, 0.0f, 1.0f);
	glUniform1f(glGetUniformLocation(Program, "material.shininess"), 32.0f);


	// XMAS TREE
	glUniform1i(glGetUniformLocation(Program, "applyWave"), 1);
	model = glm::mat4(1.0f);
	model = glm::rotate(model, glm::radians(90.0f), glm::vec3(-1.0f, 0.0f, 0.0f));
	model = glm::scale(model, glm::vec3(0.01f, 0.01f, 0.01f));
	DrawModel(tree_model, model, Program);
	glUniform1i(glGetUniformLocation(Program, "applyWave"), 0);

	// PRESENT
	if (present_exists) {
		model = glm::translate(glm::mat4(1.0f), present_position);
		model = glm::scale(model, glm::vec3(0.2f, 0.2f, 0.2f));
		DrawModel(present_model, model, Program);
	}
	
	// FLOOR
	for (int i = 0; i <= 0; ++i) {
		//model = glm::translate(glm::mat4(1.0f), glm::vec3(i * 6.8, 0, 0));
		model = glm::translate(glm::mat4(1.0f), glm::vec3(0, 0, 0));
		model = glm::scale(model, glm::vec3(10.0f, 10.0f, 10.0f));
		DrawModel(floor_model, model, Program);
	}

	// AIRSHIP
	model = glm::translate(glm::mat4(1.0f), airship_position);
	model = glm::rotate(model, glm::radians(airship_dir ? 180.0f : 0.0f), glm::vec3(0.0f, 1.0f, 0.0f));
	model = glm::scale(model, glm::vec3(.3f, .3f, .3f));
	DrawModel(airship_model, model, Program);

	// TARGETS
	for (auto& target : targets) {
		model = glm::translate(glm::mat4(1.0f), target);
		model = glm::scale(model, glm::vec3(0.1f, 0.1f, 0.1f));
		DrawModel(target_model, model, Program);
	}
	
	glUseProgram(0); // Отключаем шейдерную программу
}


// Освобождение шейдеров
void ReleaseShader() {
	// Передавая ноль, мы отключаем шейдерную программу
	glUseProgram(0);
	// Удаляем шейдерную программу
	glDeleteProgram(Program);
}

void Release() {
	// Шейдеры
	ReleaseShader();

	tree_model.release();
	floor_model.release();
	airship_model.release();
	present_model.release();
	target_model.release();
}

void HandleKeyboardInput() {
	constexpr float cameraSpeed = 0.3f;
	float cameraShiftScale = 0.5f;
	float rotationSpeed = 0.75f;
	constexpr float lightSpeed = 0.2f;
	static int change_camera_cool_down = 0;
	static int freeze_cool_down = 0;
	static int projector_cool_down = 0;

	if (change_camera_cool_down > 0)
		--change_camera_cool_down;
	if (freeze_cool_down > 0)
		--freeze_cool_down;
	if (projector_cool_down > 0)
		--projector_cool_down;

	if (sf::Keyboard::isKeyPressed(sf::Keyboard::LShift)) {
		cameraShiftScale *= 2;
		rotationSpeed *= 2;
	}

	if (sf::Keyboard::isKeyPressed(sf::Keyboard::Q) && !change_camera_cool_down) {
		change_camera_cool_down = 20;
		if (camera == &free_camera)
			camera = &airship_camera;
		else
			camera = &free_camera;
	}

	if (sf::Keyboard::isKeyPressed(sf::Keyboard::Space) && !present_exists) {
		present_exists = true;
		present_position = airship_position;
		present_position.y -= 0.2;
	}

	if (sf::Keyboard::isKeyPressed(sf::Keyboard::Tab) && !freeze_cool_down) {
		freeze = !freeze;
		freeze_cool_down = 20;
	}

	if (sf::Keyboard::isKeyPressed(sf::Keyboard::L) && !projector_cool_down) {
		if (projector.spotCosCutoff == cos(glm::radians(20.0f)))
			projector.spotCosCutoff = cos(glm::radians(0.0f));
		else
			projector.spotCosCutoff = cos(glm::radians(20.0f));

		projector_cool_down = 20;
	}

	if (camera == &free_camera) {
		if (sf::Keyboard::isKeyPressed(sf::Keyboard::W)) free_camera.cameraPos += cameraSpeed * free_camera.cameraFront * cameraShiftScale;
		if (sf::Keyboard::isKeyPressed(sf::Keyboard::S)) free_camera.cameraPos -= cameraSpeed * free_camera.cameraFront * cameraShiftScale;
		if (sf::Keyboard::isKeyPressed(sf::Keyboard::A)) free_camera.cameraPos -= glm::normalize(glm::cross(free_camera.cameraFront, free_camera.cameraUp)) * cameraSpeed * cameraShiftScale;
		if (sf::Keyboard::isKeyPressed(sf::Keyboard::D)) free_camera.cameraPos += glm::normalize(glm::cross(free_camera.cameraFront, free_camera.cameraUp)) * cameraSpeed * cameraShiftScale;
	

		if (sf::Keyboard::isKeyPressed(sf::Keyboard::Up)) pitch += rotationSpeed;
		if (sf::Keyboard::isKeyPressed(sf::Keyboard::Down)) pitch -= rotationSpeed;
		if (sf::Keyboard::isKeyPressed(sf::Keyboard::Left)) yaw -= rotationSpeed;
		if (sf::Keyboard::isKeyPressed(sf::Keyboard::Right)) yaw += rotationSpeed;
	}

	if (pitch > 89.0f) pitch = 89.0f;
	if (pitch < -89.0f) pitch = -89.0f;
}

int main() {
	sf::Window window(sf::VideoMode(900, 900), "My OpenGL window", sf::Style::Default, sf::ContextSettings(24));
	window.setVerticalSyncEnabled(true);
	window.setActive(true);
	glewInit();
	Init();
	InitScene();

	while (window.isOpen()) {
		sf::Event event;
		while (window.pollEvent(event)) {
			if (event.type == sf::Event::Closed) { window.close(); }
			else if (event.type == sf::Event::Resized) { glViewport(0, 0, event.size.width, event.size.height); }
		}
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
		sf::Vector2u windowSize = window.getSize();
		aspectRatio = static_cast<float>(windowSize.x) / static_cast<float>(windowSize.y);
		HandleKeyboardInput();
		Update();
		Draw();
		window.setTitle("kill count " + std::to_string(kill_count));
		window.display();
	}
	Release();
	return 0;
}

//model.h
#ifndef MODEL_H
#define MODEL_H

#include <SFML/Window.hpp>
#include <SFML/Graphics.hpp>
#include <GL/glew.h>
#include <GL/gl.h>
#include <string>
#include <fstream>
#include <sstream>
#include <iostream>
#include "glm/glm.hpp"
#include <glm/gtc/matrix_transform.hpp>

struct Vertex {
	glm::vec3 position;
	glm::vec3 normal;
	glm::vec2 tex_coords;

	Vertex(float pos_x, float pos_y, float pos_z) :
		position(glm::vec3(pos_x, pos_y, pos_z)) {}
};

struct Texture {
	GLuint id = -1;
	sf::Texture texture;
};

class Mesh {
	void setup_mesh() {
		glGenVertexArrays(1, &VAO);
		glGenBuffers(1, &VBO);
		glGenBuffers(1, &EBO);
		//glGenBuffers(1, &instanceVBO);

		glBindVertexArray(VAO);
		glBindBuffer(GL_ARRAY_BUFFER, VBO);
		glBufferData(GL_ARRAY_BUFFER, vertices.size() * sizeof(Vertex), &vertices[0], GL_STATIC_DRAW);
		glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO);
		glBufferData(GL_ELEMENT_ARRAY_BUFFER, indices.size() * sizeof(GLuint), &indices[0], GL_STATIC_DRAW);

		glEnableVertexAttribArray(0);
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)0);
		glEnableVertexAttribArray(1);
		glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, normal));
		glEnableVertexAttribArray(2);
		glVertexAttribPointer(2, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, tex_coords));

		glBindVertexArray(0);
	}

	void setup_texture(const std::string& text_path) {
		sf::Texture tex;
		tex.loadFromFile(text_path);
		tex.setRepeated(true);
		texture = { 0, tex };
	}

	void release() {
		glBindBuffer(GL_ARRAY_BUFFER, 0);
		glDeleteBuffers(1, &VBO);
		glDeleteBuffers(1, &EBO);
		//glDeleteBuffers(1, &instanceVBO);
		glDeleteVertexArrays(1, &VAO);
	}

public:
	std::vector<Vertex> vertices;
	std::vector<GLuint> indices;
	Texture texture;
	GLuint VAO, VBO, EBO, instanceVBO;

	Mesh() = default;

	friend class ModelData;
	friend class Model;

	void display_mesh(GLuint shader_id) const {
		if (texture.id != -1) {
			glActiveTexture(GL_TEXTURE0);
			glUniform1i(glGetUniformLocation(shader_id, "material.texture"), 0);
			sf::Texture::bind(&texture.texture);
		}
		glBindVertexArray(VAO);

		//CalculateOrbitTransform(orbit_transform);
		//glBindBuffer(GL_ARRAY_BUFFER, instanceVBO);
		//glBufferData(GL_ARRAY_BUFFER, sizeof(glm::mat4) * NUM_PLANETS, orbit_transform, GL_STATIC_DRAW);
		//
		//for (int i = 0; i < 4; i++) {
		//	glEnableVertexAttribArray(3 + i);
		//	glVertexAttribPointer(3 + i, 4, GL_FLOAT, GL_FALSE, sizeof(glm::mat4), (void*)(sizeof(glm::vec4) * i));
		//	glVertexAttribDivisor(3 + i, 1);
		//}

		//glBindBuffer(GL_ARRAY_BUFFER, 0);

		//glDrawElementsInstanced(GL_TRIANGLES, (GLuint)indices.size(), GL_UNSIGNED_INT, 0, NUM_PLANETS);
		glDrawElements(GL_TRIANGLES, (GLuint)indices.size(), GL_UNSIGNED_INT, 0);
		glBindVertexArray(0);
		if (texture.id != -1)
			sf::Texture::bind(NULL);
	}
};

std::vector<std::string> split(const std::string& str, char sep = ' ') {
	std::vector<std::string> res;
	std::stringstream iss(str);
	std::string word;
	while (std::getline(iss, word, sep)) {
		if (word.empty())
			continue;
		res.push_back(word);
	}
	return res;
}

struct ModelData {
	std::vector<Mesh> meshes;

	Vertex process_vertex(const std::string& vert, const std::vector<glm::vec3>& vert_positions,
		const std::vector<glm::vec3>& vert_normals, const std::vector<glm::vec2>& vert_tex_coords) {
		GLuint vertex_ind, norm_ind, tex_coord_ind;
		std::istringstream iss(vert);

		iss >> vertex_ind;
		Vertex res_vert(vert_positions[--vertex_ind].x,
			vert_positions[vertex_ind].y, vert_positions[vertex_ind].z);
		char ch1 = iss.peek();
		if (ch1 == '/') {
			iss.ignore();
			ch1 = iss.peek();
			if (ch1 == '/') {
				iss.ignore();
				iss >> norm_ind;
				res_vert.normal = vert_normals[--norm_ind];
			}
			else if (isdigit(ch1)) {
				iss >> tex_coord_ind;
				res_vert.tex_coords = vert_tex_coords[--tex_coord_ind];
				ch1 = iss.peek();
				if (ch1 == '/') {
					iss.ignore();
					iss >> norm_ind;
					res_vert.normal = vert_normals[--norm_ind];
				}
			}
		}

		return res_vert;
	}

	void load_model(const std::string& file_name, const std::string& tex_path) {
		std::ifstream file(file_name);
		if (!file.is_open()) {
			std::cerr << "Failed to open file: " << file_name << std::endl;
			return;
		}

		bool is_new_mesh = false;
		std::vector<glm::vec3> vert_positions;
		std::vector<glm::vec3> vert_normals;
		std::vector<glm::vec2> vert_tex_coords;
		std::vector<GLuint> indices;
		Mesh cur_mesh;
		std::string line;

		while (std::getline(file, line)) {
			std::istringstream iss(line);
			std::string type;
			iss >> type;
			if (type.empty() || type[0] == '#') {
				continue;
			}
			if (type == "v") {
				if (is_new_mesh) {
					cur_mesh.indices = indices;
					meshes.push_back(cur_mesh);
					cur_mesh = Mesh();
					indices.clear();
					is_new_mesh = false;
				}
				float x, y, z;
				iss >> x >> y >> z;
				vert_positions.emplace_back(x, y, z);
			}
			else if (type == "vn") {
				float x, y, z;
				iss >> x >> y >> z;
				vert_normals.emplace_back(x, y, z);
			}
			else if (type == "vt") {
				double x, y;
				iss >> x >> y;
				vert_tex_coords.emplace_back(x, y);
			}
			else if (type == "f") {
				if (!is_new_mesh)
					is_new_mesh = true;
				auto spl = split(line);
				for (int i = 3; i < spl.size(); ++i) {
					cur_mesh.vertices.push_back(process_vertex(spl[1], vert_positions,
						vert_normals, vert_tex_coords));
					indices.push_back(indices.size());
					cur_mesh.vertices.push_back(process_vertex(spl[i - 1], vert_positions,
						vert_normals, vert_tex_coords));
					indices.push_back(indices.size());
					cur_mesh.vertices.push_back(process_vertex(spl[i], vert_positions,
						vert_normals, vert_tex_coords));
					indices.push_back(indices.size());
				}
			}
		}
		file.close();

		cur_mesh.indices = indices;
		meshes.push_back(cur_mesh);
		for (Mesh& mesh : meshes) {
			mesh.setup_mesh();
			if (!tex_path.empty())
				mesh.setup_texture(tex_path);
		}
	}

	void release() {
		for (Mesh& mesh : meshes)
			mesh.release();
	}
};

struct Model {
	ModelData data;

	Model() = default;

	Model(std::string const& file_path, std::string const& tex_path) {
		data.load_model(file_path, tex_path);
	}

	void display_model(GLuint shader_id) const {
		for (const Mesh& mesh : data.meshes)
			mesh.display_mesh(shader_id);
	}

	void release() {
		for (Mesh& mesh : data.meshes)
			mesh.release();
	}
};
#endif

//shader.h
#ifndef SHADER_H
#define SHADER_H

#include <string>
#include <fstream>
#include <sstream>
#include <iostream>

void check_compile_errors(GLuint shader_ID, std::string type)
{
	GLint status;
	GLchar info_log[1024];
	if (type == "PROGRAM")
	{
		glGetProgramiv(shader_ID, GL_LINK_STATUS, &status);
		if (!status)
		{
			glGetProgramInfoLog(shader_ID, 1024, NULL, info_log);
			std::cout << "Linking error occurred, type: " << type << "\n" << info_log << "\n -- --------------------------------------------------- -- " << std::endl;
		}
	}
	else
	{
		glGetShaderiv(shader_ID, GL_COMPILE_STATUS, &status);
		if (!status)
		{
			glGetShaderInfoLog(shader_ID, 1024, NULL, info_log);
			std::cout << "Shader compilation error occurred, type: " << type << "\n" << info_log << "\n -- --------------------------------------------------- -- " << std::endl;
		}
	}
}

GLuint load_shaders(const char* vertex_shad_path, const char* fragment_shad_path)
{
	std::string vertex_code;
	std::string fragment_code;

	std::ifstream vert_shad_file;
	std::ifstream frag_shad_file;

	vert_shad_file.exceptions(std::ifstream::failbit | std::ifstream::badbit);
	frag_shad_file.exceptions(std::ifstream::failbit | std::ifstream::badbit);
	try
	{
		vert_shad_file.open(vertex_shad_path);
		frag_shad_file.open(fragment_shad_path);
		std::stringstream vert_shader_stream, frag_shader_stream;

		vert_shader_stream << vert_shad_file.rdbuf();
		frag_shader_stream << frag_shad_file.rdbuf();

		vert_shad_file.close();
		frag_shad_file.close();

		vertex_code = vert_shader_stream.str();
		fragment_code = frag_shader_stream.str();
	}
	catch (std::ifstream::failure& err)
	{
		std::cout << "Error occurred while reading shader file: " << err.what() << std::endl;
	}
	const char* vertex_code_char = vertex_code.c_str();
	const char* fragment_code_char = fragment_code.c_str();
	unsigned int vertex, fragment;

	// vertex shader
	vertex = glCreateShader(GL_VERTEX_SHADER);
	glShaderSource(vertex, 1, &vertex_code_char, NULL);
	glCompileShader(vertex);
	check_compile_errors(vertex, "VERTEX");

	// fragment Shader
	fragment = glCreateShader(GL_FRAGMENT_SHADER);
	glShaderSource(fragment, 1, &fragment_code_char, NULL);
	glCompileShader(fragment);
	check_compile_errors(fragment, "FRAGMENT");

	// shader Program
	GLuint ID = glCreateProgram();
	glAttachShader(ID, vertex);
	glAttachShader(ID, fragment);
	glLinkProgram(ID);
	check_compile_errors(ID, "PROGRAM");

	glDeleteShader(vertex);
	glDeleteShader(fragment);

	return ID;
}

#endif