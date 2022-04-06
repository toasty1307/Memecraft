namespace Memecraft.Components;

public interface IUpdatable
{
    bool Enabled { get; set; }
    
    void Update(FrameEventArgs args);
    
    event EventHandler<bool> EnabledChanged;
}