namespace Memecraft.OpenGLStuff;

public class Shader : IDisposable
{
    public readonly int Id;

    public Shader(ShaderType shaderType)
    {
        Id = GL.CreateShader(shaderType);
    }

    public void Compile()
    {
        GL.CompileShader(Id);
    }
    
    ~Shader()
    {
        ReleaseUnmanagedResources();
    }
    
    public void Delete()
    {
        GL.DeleteShader(Id);
    }
    
    public void SetSource(string source)
    {
        GL.ShaderSource(Id, source);
    }
    
    public static Shader FromFile(string path, ShaderType shaderType)
    {
        var shader = new Shader(shaderType);
        shader.SetSource(File.ReadAllText(path));
        return shader;
    }

    private void ReleaseUnmanagedResources()
    {
        Delete();
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }
}