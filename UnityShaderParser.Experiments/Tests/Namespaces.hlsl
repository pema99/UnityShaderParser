// Basic namespace declaration and access
namespace Math
{
    float Add(float a, float b) { return a + b; }
    float Multiply(float a, float b) { return a * b; }
}

[Test]
void Namespace_BasicAccess()
{
    float result = Math::Add(3.0, 4.0);
    ASSERT(result == 7.0);
    
    result = Math::Multiply(3.0, 4.0);
    ASSERT(result == 12.0);
}

// Global function with same name as namespaced function
float Add(float a, float b) { return a - b; }

[Test]
void Namespace_GlobalVsNamespaced()
{
    // Global function subtracts
    float globalResult = Add(10.0, 3.0);
    ASSERT(globalResult == 7.0);
    
    // Namespaced function adds
    float namespacedResult = Math::Add(10.0, 3.0);
    ASSERT(namespacedResult == 13.0);
}

// Nested namespaces
namespace Graphics
{
    namespace Color
    {
        float3 Red() { return float3(1.0, 0.0, 0.0); }
        float3 Green() { return float3(0.0, 1.0, 0.0); }
    }
    
    namespace Transform
    {
        float3 Scale(float3 v, float s) { return v * s; }
    }
}

[Test]
void Namespace_NestedAccess()
{
    float3 red = Graphics::Color::Red();
    ASSERT(red.x == 1.0 && red.y == 0.0 && red.z == 0.0);
    
    float3 green = Graphics::Color::Green();
    ASSERT(green.x == 0.0 && green.y == 1.0 && green.z == 0.0);
    
    float3 scaled = Graphics::Transform::Scale(float3(2.0, 3.0, 4.0), 2.0);
    ASSERT(scaled.x == 4.0 && scaled.y == 6.0 && scaled.z == 8.0);
}

// Variables in namespaces
namespace Constants
{
    static const float PI = 3.14159;
    static const float E = 2.71828;
    static const int MAX_SIZE = 100;
}

[Test]
void Namespace_VariableAccess()
{
    float circumference = 2.0 * Constants::PI * 5.0;
    ASSERT(abs(circumference - 31.4159) < 0.001);
    
    float expValue = Constants::E;
    ASSERT(abs(expValue - 2.71828) < 0.001);
    
    int size = Constants::MAX_SIZE;
    ASSERT(size == 100);
}

// Multiple namespaces with same names for different things
namespace Physics
{
    float Force(float mass, float acceleration) { return mass * acceleration; }
}

namespace Math
{
    float Force(float a) { return a * a; } // Different signature
}

[Test]
void Namespace_SameNameDifferentNamespaces()
{
    float physicsForce = Physics::Force(10.0, 2.0);
    ASSERT(physicsForce == 20.0);
    
    float mathForce = Math::Force(5.0);
    ASSERT(mathForce == 25.0);
}

// Shadowing - namespace function shadows global
float Calculate(float x) { return x + 1.0; }

namespace Utils
{
    float Calculate(float x) { return x * 2.0; }
}

[Test]
void Namespace_Shadowing()
{
    // Global function
    float global = Calculate(5.0);
    ASSERT(global == 6.0);
    
    // Namespaced function
    float namespaced = Utils::Calculate(5.0);
    ASSERT(namespaced == 10.0);
}

// Namespace with structs
namespace Geometry
{
    struct Point
    {
        float x;
        float y;
    };
    
    struct Rectangle
    {
        Point min;
        Point max;
    };
    
    float Area(Rectangle r)
    {
        return (r.max.x - r.min.x) * (r.max.y - r.min.y);
    }
}

[Test]
void Namespace_Structs()
{
    Geometry::Point p1;
    p1.x = 0.0;
    p1.y = 0.0;
    
    Geometry::Point p2;
    p2.x = 5.0;
    p2.y = 3.0;
    
    Geometry::Rectangle rect;
    rect.min = p1;
    rect.max = p2;
    
    float area = Geometry::Area(rect);
    ASSERT(area == 15.0);
}

// Nested namespace with shadowing
namespace Outer
{
    float Value() { return 10.0; }
    
    namespace Inner
    {
        float Value() { return 20.0; }
        
        namespace Deep
        {
            float Value() { return 30.0; }
        }
    }
}

[Test]
void Namespace_NestedShadowing()
{
    float outer = Outer::Value();
    ASSERT(outer == 10.0);
    
    float inner = Outer::Inner::Value();
    ASSERT(inner == 20.0);
    
    float deep = Outer::Inner::Deep::Value();
    ASSERT(deep == 30.0);
}

// Multiple functions in same namespace
namespace Operations
{
    float Add(float a, float b) { return a + b; }
    float Sub(float a, float b) { return a - b; }
    float Mul(float a, float b) { return a * b; }
    float Div(float a, float b) { return a / b; }
}

[Test]
void Namespace_MultipleFunctions()
{
    ASSERT(Operations::Add(10.0, 5.0) == 15.0);
    ASSERT(Operations::Sub(10.0, 5.0) == 5.0);
    ASSERT(Operations::Mul(10.0, 5.0) == 50.0);
    ASSERT(abs(Operations::Div(10.0, 5.0) - 2.0) < 0.001);
}

// Overloaded functions in namespace
namespace Vector
{
    float Length(float2 v) { return sqrt(v.x * v.x + v.y * v.y); }
    float Length(float3 v) { return sqrt(v.x * v.x + v.y * v.y + v.z * v.z); }
    float Length(float4 v) { return sqrt(v.x * v.x + v.y * v.y + v.z * v.z + v.w * v.w); }
}

[Test]
void Namespace_Overloading()
{
    float len2 = Vector::Length(float2(3.0, 4.0));
    ASSERT(abs(len2 - 5.0) < 0.001);
    
    float len3 = Vector::Length(float3(1.0, 2.0, 2.0));
    ASSERT(abs(len3 - 3.0) < 0.001);
    
    float len4 = Vector::Length(float4(2.0, 2.0, 2.0, 2.0));
    ASSERT(abs(len4 - 4.0) < 0.001);
}

// Same struct name in different namespaces
namespace Rendering
{
    struct Color { float r; float g; float b; };
    Color MakeColor(float r, float g, float b)
    {
        Color c;
        c.r = r;
        c.g = g;
        c.b = b;
        return c;
    }
}

namespace Physics
{
    struct Color { float intensity; float wavelength; };
    Color MakeColor(float i, float w)
    {
        Color c;
        c.intensity = i;
        c.wavelength = w;
        return c;
    }
}

[Test]
void Namespace_SameStructName()
{
    Rendering::Color renderColor = Rendering::MakeColor(1.0, 0.5, 0.0);
    ASSERT(renderColor.r == 1.0);
    ASSERT(renderColor.g == 0.5);
    ASSERT(renderColor.b == 0.0);
    
    Physics::Color physicsColor = Physics::MakeColor(100.0, 550.0);
    ASSERT(physicsColor.intensity == 100.0);
    ASSERT(physicsColor.wavelength == 550.0);
}

// Global variable vs namespaced variable
static const float GRAVITY = 9.8;

namespace Physics
{
    static const float GRAVITY = 10.0;
}

[Test]
void Namespace_GlobalVsNamespacedVariable()
{
    float globalGravity = GRAVITY;
    ASSERT(abs(globalGravity - 9.8) < 0.001);
    
    float namespacedGravity = Physics::GRAVITY;
    ASSERT(abs(namespacedGravity - 10.0) < 0.001);
}

// Namespace with arrays
namespace Data
{
    static const float values[3] = { 1.0, 2.0, 3.0 };
    
    float GetValue(int index)
    {
        return values[index];
    }
    
    float Sum()
    {
        return values[0] + values[1] + values[2];
    }
}

[Test]
void Namespace_Arrays()
{
    ASSERT(Data::GetValue(0) == 1.0);
    ASSERT(Data::GetValue(1) == 2.0);
    ASSERT(Data::GetValue(2) == 3.0);
    
    float sum = Data::Sum();
    ASSERT(sum == 6.0);
}

// Nested namespaces with same function name at different levels
namespace Level1
{
    float Compute(float x) { return x + 10.0; }
    
    namespace Level2
    {
        float Compute(float x) { return x + 20.0; }
        
        namespace Level3
        {
            float Compute(float x) { return x + 30.0; }
        }
    }
}

[Test]
void Namespace_MultipleLevelsSameName()
{
    float result1 = Level1::Compute(5.0);
    ASSERT(result1 == 15.0);
    
    float result2 = Level1::Level2::Compute(5.0);
    ASSERT(result2 == 25.0);
    
    float result3 = Level1::Level2::Level3::Compute(5.0);
    ASSERT(result3 == 35.0);
}

// Namespace with typedef
namespace Types
{
    typedef float2 Vec2;
    typedef float3 Vec3;
    typedef float4 Vec4;
    
    Vec3 MakeVec3(float x, float y, float z)
    {
        return float3(x, y, z);
    }
}

[Test]
void Namespace_Typedef()
{
    Types::Vec3 v = Types::MakeVec3(1.0, 2.0, 3.0);
    ASSERT(v.x == 1.0 && v.y == 2.0 && v.z == 3.0);
}

// Functions calling other functions in same namespace
namespace Chain
{
    float StepA(float x) { return x * 2.0; }
    float StepB(float x) { return StepA(x) + 5.0; }
    float StepC(float x) { return StepB(x) * 3.0; }
}

[Test]
void Namespace_InternalCalls()
{
    // StepC calls StepB which calls StepA
    float result = Chain::StepC(10.0);
    // StepA(10) = 20
    // StepB(10) = 20 + 5 = 25
    // StepC(10) = 25 * 3 = 75
    ASSERT(result == 75.0);
}

// Namespace calling global function
float GlobalHelper(float x) { return x + 100.0; }

namespace Mixed
{
    float UseGlobal(float x)
    {
        return GlobalHelper(x) * 2.0;
    }
}

[Test]
void Namespace_CallGlobal()
{
    float result = Mixed::UseGlobal(50.0);
    // GlobalHelper(50) = 150
    // 150 * 2 = 300
    ASSERT(result == 300.0);
}

// Multiple variables in namespace
namespace Config
{
    static const int WIDTH = 1920;
    static const int HEIGHT = 1080;
    static const float ASPECT_RATIO = 1.777;
    static const bool FULLSCREEN = true;
}

[Test]
void Namespace_MultipleVariables()
{
    ASSERT(Config::WIDTH == 1920);
    ASSERT(Config::HEIGHT == 1080);
    ASSERT(abs(Config::ASPECT_RATIO - 1.777) < 0.001);
    ASSERT(Config::FULLSCREEN == true);
}

// Namespace with mix of functions and variables
namespace Scene
{
    static const float3 AMBIENT_LIGHT = float3(0.1, 0.1, 0.1);
    
    float3 ApplyAmbient(float3 color)
    {
        return color + AMBIENT_LIGHT;
    }
    
    float3 AdjustExposure(float3 color, float exposure)
    {
        return color * exposure;
    }
}

[Test]
void Namespace_MixedContent()
{
    float3 color = float3(0.5, 0.3, 0.2);
    float3 withAmbient = Scene::ApplyAmbient(color);
    
    ASSERT(abs(withAmbient.x - 0.6) < 0.001);
    ASSERT(abs(withAmbient.y - 0.4) < 0.001);
    ASSERT(abs(withAmbient.z - 0.3) < 0.001);
    
    float3 exposed = Scene::AdjustExposure(withAmbient, 2.0);
    ASSERT(abs(exposed.x - 1.2) < 0.001);
    ASSERT(abs(exposed.y - 0.8) < 0.001);
    ASSERT(abs(exposed.z - 0.6) < 0.001);
}

// Deeply nested namespace access
namespace A
{
    namespace B
    {
        namespace C
        {
            namespace D
            {
                float DeepFunction(float x) { return x * 100.0; }
                static const float DEEP_VALUE = 42.0;
            }
        }
    }
}

[Test]
void Namespace_DeeplyNested()
{
    float result = A::B::C::D::DeepFunction(5.0);
    ASSERT(result == 500.0);
    
    float value = A::B::C::D::DEEP_VALUE;
    ASSERT(value == 42.0);
}

// Namespace with function returning struct
namespace Factory
{
    struct Product
    {
        float price;
        int quantity;
    };
    
    Product Create(float p, int q)
    {
        Product prod;
        prod.price = p;
        prod.quantity = q;
        return prod;
    }
    
    float TotalValue(Product p)
    {
        return p.price * p.quantity;
    }
}

[Test]
void Namespace_StructReturn()
{
    Factory::Product item = Factory::Create(10.5, 3);
    ASSERT(abs(item.price - 10.5) < 0.001);
    ASSERT(item.quantity == 3);
    
    float total = Factory::TotalValue(item);
    ASSERT(abs(total - 31.5) < 0.001);
}

// Same variable name in nested namespaces
namespace Parent
{
    static const float VALUE = 1.0;
    
    namespace Child
    {
        static const float VALUE = 2.0;
        
        namespace GrandChild
        {
            static const float VALUE = 3.0;
        }
    }
}

[Test]
void Namespace_VariableShadowingNested()
{
    ASSERT(Parent::VALUE == 1.0);
    ASSERT(Parent::Child::VALUE == 2.0);
    ASSERT(Parent::Child::GrandChild::VALUE == 3.0);
}

// Namespace with matrix operations
namespace Matrix
{
    float2x2 Identity2x2()
    {
        return float2x2(1, 0, 0, 1);
    }
    
    float2x2 Scale2x2(float s)
    {
        return float2x2(s, 0, 0, s);
    }
}

[Test]
void Namespace_MatrixOperations()
{
    float2x2 identity = Matrix::Identity2x2();
    ASSERT(identity[0][0] == 1.0 && identity[0][1] == 0.0);
    ASSERT(identity[1][0] == 0.0 && identity[1][1] == 1.0);
    
    float2x2 scaled = Matrix::Scale2x2(3.0);
    ASSERT(scaled[0][0] == 3.0 && scaled[0][1] == 0.0);
    ASSERT(scaled[1][0] == 0.0 && scaled[1][1] == 3.0);
}

// Test calling namespaced function from another namespaced function
namespace HelperNS
{
    float Double(float x) { return x * 2.0; }
}

namespace UserNS
{
    float Quadruple(float x)
    {
        return HelperNS::Double(HelperNS::Double(x));
    }
}

[Test]
void Namespace_CrossNamespaceCall()
{
    float result = UserNS::Quadruple(5.0);
    ASSERT(result == 20.0);
}

// Namespace with enum-like constants
namespace ErrorCodes
{
    static const int SUCCESS = 0;
    static const int FILE_NOT_FOUND = 1;
    static const int ACCESS_DENIED = 2;
    static const int INVALID_ARGUMENT = 3;
}

[Test]
void Namespace_EnumLikeConstants()
{
    int code = ErrorCodes::SUCCESS;
    ASSERT(code == 0);
    
    code = ErrorCodes::FILE_NOT_FOUND;
    ASSERT(code == 1);
    
    code = ErrorCodes::ACCESS_DENIED;
    ASSERT(code == 2);
    
    code = ErrorCodes::INVALID_ARGUMENT;
    ASSERT(code == 3);
}

// Struct with methods in namespace
namespace Shapes
{
    struct Circle
    {
        float radius;
        
        float Area()
        {
            return 3.14159 * radius * radius;
        }
        
        float Circumference()
        {
            return 2.0 * 3.14159 * radius;
        }
    };
}

[Test]
void Namespace_StructMethods()
{
    Shapes::Circle c;
    c.radius = 5.0;
    
    float area = c.Area();
    ASSERT(abs(area - 78.539) < 0.01);
    
    float circum = c.Circumference();
    ASSERT(abs(circum - 31.4159) < 0.001);
}

// Nested namespace with struct methods
namespace Engine
{
    namespace Components
    {
        struct Transform
        {
            float3 position;
            float3 scale;
            
            void SetPosition(float x, float y, float z)
            {
                position = float3(x, y, z);
            }
            
            float3 GetScaledPosition()
            {
                return position * scale;
            }
        };
    }
}

[Test]
void Namespace_NestedStructMethods()
{
    Engine::Components::Transform t;
    t.scale = float3(2.0, 2.0, 2.0);
    t.SetPosition(1.0, 2.0, 3.0);
    
    ASSERT(t.position.x == 1.0);
    ASSERT(t.position.y == 2.0);
    ASSERT(t.position.z == 3.0);
    
    float3 scaled = t.GetScaledPosition();
    ASSERT(scaled.x == 2.0 && scaled.y == 4.0 && scaled.z == 6.0);
}