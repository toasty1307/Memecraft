using Serilog;

namespace Memecraft.Components;

public class Component<T> : Component where T: Component<T>
{
    public ILogger Logger { get; }
    public Component(MemecraftWindow window) : base(window)
    {
        Logger = window.Logger.ForContext<T>();
    }
}

public class Component : IUpdatable, IRenderable, IDisposable
{
    private bool _enabled = true;
    private bool _visible = true;
    public MemecraftWindow Window { get; }

    public Component(MemecraftWindow window)
    {
        Window = window;
        window.Components.Add(this);
    }

    public bool Enabled
    {
        get => _enabled;
        set
        {
            EnabledChanged?.Invoke(this, value);
            _enabled = value;
        }
    }

    public virtual void Update(FrameEventArgs args)
    {
    }

    public event EventHandler<bool> EnabledChanged;

    public bool Visible
    {
        get => _visible;
        set
        {
            VisibleChanged?.Invoke(this, value);
            _visible = value;
        }
    }

    public virtual void Render(FrameEventArgs args)
    {
    }

    public event EventHandler<bool> VisibleChanged;

    public virtual void Dispose()
    {
        Window.Components.Remove(this);
        GC.SuppressFinalize(this);
    }
}