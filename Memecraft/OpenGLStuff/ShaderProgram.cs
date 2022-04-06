namespace Memecraft.OpenGLStuff;

public class ShaderProgram : IDisposable
{
    public readonly int Id;

    public ShaderProgram()
    {
        Id = GL.CreateProgram();
    }
    
    public void AttachShader(Shader shader)
    {
        shader.Compile();
        GL.AttachShader(Id, shader.Id);
        shader.Dispose();
    }
    
    public void Link()
    {
        GL.LinkProgram(Id);
    }
    
    public void Use()
    {
        GL.UseProgram(Id);
    }
    
    public void SetUniform(string name, int value)
    {
        GL.Uniform1(GL.GetUniformLocation(Id, name), value);
    }
    
    public void SetUniform(string name, float value)
    {
        GL.Uniform1(GL.GetUniformLocation(Id, name), value);
    }
    
    public void SetUniform(string name, Vector2 value)
    {
        GL.Uniform2(GL.GetUniformLocation(Id, name), value);
    }
    
    public void SetUniform(string name, Vector3 value)
    {
        GL.Uniform3(GL.GetUniformLocation(Id, name), value);
    }
    
    public void SetUniform(string name, Vector4 value)
    {
        GL.Uniform4(GL.GetUniformLocation(Id, name), value);
    }
    
    public void SetUniform(string name, Matrix4 value)
    {
        GL.UniformMatrix4(GL.GetUniformLocation(Id, name), false, ref value);
    }
    
    ~ShaderProgram()
    {
        ReleaseUnmanagedResources();
    }

    private void ReleaseUnmanagedResources()
    {
        GL.DeleteProgram(Id);
        GL.UseProgram(0);
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    public void Unlink()
    {
        GL.UseProgram(0);
    }
}