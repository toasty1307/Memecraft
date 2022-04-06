namespace Memecraft.Components;

public interface IRenderable
{
    bool Visible { get; set; }
    void Render(FrameEventArgs args);
    event EventHandler<bool> VisibleChanged; 
}