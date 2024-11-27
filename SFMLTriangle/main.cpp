#include <SFML/Graphics.hpp>
#include <GL/glew.h>
#include <iostream>

void SetIcon(sf::Window& wnd);


GLuint Program;
GLint Attrib_vertex;
GLuint VBO;

struct Vertex
{
    GLfloat x;
    GLfloat y;
};

const char* VertexShaderSource = R"(
    #version 330 core
    in vec2 coord;
    void main()
        {
        gl_Position = vec4(coord, 0.0, 1.0);
        }
)";

const char* FragShaderSource = R"(
    #version 330 core
    out vec4 color;
    void main(){color = vec4(0,1,0,1);
}
)";

void checkOpenGLerror()
{
    if (GL_ARRAY_BUFFER != NULL)
    {
        printf("Error: VBO does not disabled");
    }

    
    //glDisableVertexAttribArray
}


// Глобальные переменные это плохо, тут это сделано просто для примера
GLfloat rotate_z = 0;


void InitVBO()
{

    glGenBuffers(1, &VBO);
    //Вершины треугольника
    Vertex triangle[3] =
    {
        {-1.0f, -1.0f},
        {0.0f, 1.0f},
        {1.0f, -1.0f}
    };

    glBindBuffer(GL_ARRAY_BUFFER, VBO);
    glBufferData(GL_ARRAY_BUFFER, sizeof(triangle), triangle, GL_STATIC_DRAW);
    //checkOpenGLerror(); 

}

void ShaderLog(unsigned int shader)
{
    int infologLen = 0;
    glGetShaderiv(shader, GL_INFO_LOG_LENGTH, &infologLen);
    if (infologLen > 1)
    {
        int charsWritten = 0;
        std::vector<char> infoLog(infologLen);
        glGetShaderInfoLog(shader, infologLen, &charsWritten, infoLog.data());
        std::cout << "InfoLog: " << infoLog.data() << std::endl;
    }
}


void InitShader()
{
    // Создаем вершинный шейдер
    GLuint vShader = glCreateShader(GL_VERTEX_SHADER);
    // Передаем исходный код
    glShaderSource(vShader, 1, &VertexShaderSource, NULL);
    // Компилируем шейдер
    glCompileShader(vShader);
    std::cout << "vertex shader \n";
    // Функция печати лога шейдера
    ShaderLog(vShader); //Пример функции есть в лабораторной


    // Создаем фрагментный шейдер
    GLuint fShader = glCreateShader(GL_FRAGMENT_SHADER);
    // Передаем исходный код
    glShaderSource(fShader, 1, &FragShaderSource, NULL);
    // Компилируем шейдер
    glCompileShader(fShader);
    std::cout << "fragment shader \n";
    // Функция печати лога шейдера
    ShaderLog(fShader);

    
    // Создаем программу и прикрепляем шейдеры к ней
    Program = glCreateProgram();
    glAttachShader(Program, vShader);
    glAttachShader(Program, fShader);
    // Линкуем шейдерную программу
    glLinkProgram(Program);
    // Проверяем статус сборки
    int link_ok;
    glGetProgramiv(Program, GL_LINK_STATUS, &link_ok);
    if (!link_ok) {
        std::cout << "error attach shaders \n";
        return;
    }

    // Вытягиваем ID атрибута из собранной программы
    const char* attr_name = "coord"; //имя в шейдере
    Attrib_vertex = glGetAttribLocation(Program, attr_name);
    if (Attrib_vertex == -1) {
        std::cout << "could not bind attrib " << attr_name << std::endl;
        return;
    }
    

}

// В момент инициализации разумно произвести загрузку текстур, моделей и других вещей
void Init() {
    // Очистка буфера тёмно жёлтым цветом
    glClearColor(0.5f, 0.5f, 0.0f, 1.0f);
    InitShader();
    InitVBO();
}

void Draw() {
    glUseProgram(Program); // Устанавливаем шейдерную программу текущей
    glEnableVertexAttribArray(Attrib_vertex); // Включаем массив атрибутов
    glBindBuffer(GL_ARRAY_BUFFER, VBO); // Подключаем VBO  
    // сообщаем OpenGL как он должен интерпретировать вершинные данные. 
    glVertexAttribPointer(Attrib_vertex, 2, GL_FLOAT, GL_FALSE, 0, 0);
    glBindBuffer(GL_ARRAY_BUFFER, 0); // Отключаем VBO
    glDrawArrays(GL_TRIANGLES, 0, 3); // Передаем данные на видеокарту(рисуем)
    glDisableVertexAttribArray(Attrib_vertex);   // Отключаем массив атрибутов    
    glUseProgram(0);   // Отключаем шейдерную программу
    checkOpenGLerror();
}



void SetIcon(sf::Window& wnd)
{
    sf::Image image;

    // Вместо рисования пикселей, можно загрузить иконку из файла (image.loadFromFile("icon.png"))
    image.create(16, 16);
    for (int i = 0; i < 16; ++i)
        for (int j = 0; j < 16; ++j)
            image.setPixel(i, j, {
                (sf::Uint8)(i * 16), (sf::Uint8)(j * 16), 0 });

    wnd.setIcon(image.getSize().x, image.getSize().y, image.getPixelsPtr());
}


void GLAPIENTRY MessageCallback(GLenum source,
    GLenum type,
    GLuint id,
    GLenum severity,
    GLsizei length,
    const GLchar* message,
    const void* userParam)
{
    fprintf(stderr, "GL CALLBACK: %s type = 0x%x, severity = 0x%x, message = %s\n",
        (type == GL_DEBUG_TYPE_ERROR ? "** GL ERROR **" : ""),
        type, severity, message);
}



int main()
{
    sf::RenderWindow window(sf::VideoMode(200, 200), "SFML works!");
    sf::CircleShape shape(100.f);
    shape.setFillColor(sf::Color::Green);

    glewInit();
    void Init();
    void Draw();

    // During init, enable debug output
    glEnable(GL_DEBUG_OUTPUT);


    glDebugMessageCallback(MessageCallback, 0);
    // Ставим иконку (окна с дефолтной картинкой это некрасиво)
    SetIcon(window);
    // Включаем вертикальную синхронизацию (синхронизация частоты отрисовки с частотой кадров монитора, чтобы картинка не фризила, делать это не обязательно)
    window.setVerticalSyncEnabled(true);

    // Активируем окно
    window.setActive(true);
    glewInit();
    // Инициализация
    Init();

    while (window.isOpen())
    {
        sf::Event event;
        while (window.pollEvent(event))
        {
            if (event.type == sf::Event::Closed)
                window.close();

            else if (event.type == sf::Event::Resized)
            {
                // Изменён размер окна, надо поменять и размер области Opengl отрисовки
                glViewport(0, 0, event.size.width, event.size.height);
            }
        }

        // Очистка буферов
        glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
        //printf("%d", GL_position);
        // Рисуем сцену
        Draw();

        // Отрисовывает кадр - меняет передний и задний буфер местами
        window.display();
    }
        //window.clear();
        //window.draw(shape);
        //window.display();
    

    return 0;
}