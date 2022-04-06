using Memecraft.OpenGLStuff;

namespace Memecraft.Components.Test;

public class TestTriangle : Component<TestTriangle>
{
    private readonly ShaderProgram _shaderProgram;
    private readonly int _vertexArray;

    public unsafe TestTriangle(MemecraftWindow window) : base(window)
    {
        _shaderProgram = new ShaderProgram();
        var vertexShader = Shader.FromFile("Assets/Shaders/Test/TestTriangle/Vertex.glsl", ShaderType.VertexShader);
        var fragmentShader = Shader.FromFile("Assets/Shaders/Test/TestTriangle/Fragment.glsl", ShaderType.FragmentShader);
        _shaderProgram.AttachShader(vertexShader);
        _shaderProgram.AttachShader(fragmentShader);
        _shaderProgram.Link();

        float[] vertices = 
        {
            -0.2f, -0.2f, 0.0f,
            0.2f, -0.2f, 0.0f,
            0.0f,  0.2f, 0.0f,
            -0.3f, -0.3f, 0.0f,
            0.8f, -0.8f, 0.0f,
            0.0f, -0.8f, 0.0f,
        };
        
        int vertexBuffer = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
        fixed (float* ptr = &vertices[0])
        {
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), (IntPtr)ptr, BufferUsageHint.StaticDraw);
        }
        
        _vertexArray = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArray);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
    }

    public override void Render(FrameEventArgs args)
    {
        _shaderProgram.Use();
        GL.BindVertexArray(_vertexArray);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        GL.BindVertexArray(0);
        
        _shaderProgram.Unlink();
    }
    
    ~TestTriangle()
    {
        ReleaseUnmanagedResources();
    }

    private void ReleaseUnmanagedResources()
    {
        _shaderProgram.Dispose();
    }

    public override void Dispose()
    {
        ReleaseUnmanagedResources();
        base.Dispose();
    }
}