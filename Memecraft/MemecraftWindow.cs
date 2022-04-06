using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using Memecraft.Components;
using Serilog;
using Serilog.Events;

namespace Memecraft;

public class MemecraftWindow : GameWindow
{
    public ILogger Logger { get; }
    public ObservableCollection<Component> Components { get; }
    public GameManager GameManager { get; private set; }
    
    private readonly List<IUpdatable> _enabledComponents = new();
    private readonly List<IRenderable> _visibleComponents = new();

    public MemecraftWindow() :
        base
        (
            new GameWindowSettings(),
            new NativeWindowSettings
            {
                Title = "Memecraft",
                Size = new Vector2i(960, 540),
                APIVersion = new Version(4, 6),
                Profile = ContextProfile.Core
            }
        )
    {
        Logger = new LoggerConfiguration()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3} {SourceContext}] {Message:lj}{NewLine}{Exception}")
            .Enrich.FromLogContext()
            .MinimumLevel.Debug()
            .CreateLogger()
            .ForContext<MemecraftWindow>();
        
        Components = new ObservableCollection<Component>();
        Components.CollectionChanged += ComponentsOnCollectionChanged;
    }

    private void ComponentsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (var updatable in e.NewItems.OfType<IUpdatable>())
            {
                _enabledComponents.Add(updatable);
                updatable.EnabledChanged += UpdatableOnEnabledChanged;
            }
            
            foreach (var renderable in e.NewItems.OfType<IRenderable>())
            {
                _visibleComponents.Add(renderable);
                renderable.VisibleChanged += RenderableOnVisibleChanged;
            }
        }
        
        if (e.OldItems != null)
        {
            foreach (var updatable in e.OldItems.OfType<IUpdatable>())
            {
                _enabledComponents.Remove(updatable);
                updatable.EnabledChanged -= UpdatableOnEnabledChanged;
            }
            
            foreach (var renderable in e.OldItems.OfType<IRenderable>())
            {
                _visibleComponents.Remove(renderable);
                renderable.VisibleChanged -= RenderableOnVisibleChanged;
            }
        }
    }

    private void RenderableOnVisibleChanged(object sender, bool e)
    {
        if (e)
            _visibleComponents.Add(sender as IRenderable);
        else
            _visibleComponents.Remove(sender as IRenderable);
    }

    private void UpdatableOnEnabledChanged(object sender, bool e)
    {
        if (e)
            _enabledComponents.Add(sender as IUpdatable);
        else
            _enabledComponents.Remove(sender as IUpdatable);
    }

    protected override void OnLoad()
    {
        GL.ClearColor(Color4.CornflowerBlue); 
        
        GL.Enable(EnableCap.DebugOutput);
        GL.Enable(EnableCap.DebugOutputSynchronous);
        
        GL.DebugMessageCallback(DebugCallback, IntPtr.Zero);
        
        GameManager = new GameManager(this);
    }
    
    private void DebugCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length,
        IntPtr message, IntPtr _)
    {
        var messageString = Marshal.PtrToStringAnsi(message, length);

        var severityString = severity switch
        {
            DebugSeverity.DebugSeverityNotification => "Notification",
            DebugSeverity.DebugSeverityHigh => "High",
            DebugSeverity.DebugSeverityMedium => "Medium",
            DebugSeverity.DebugSeverityLow => "Low",
            _ => "Unknown"
        };

        var sourceString = source switch
        {
            DebugSource.DebugSourceApi => "API",
            DebugSource.DebugSourceWindowSystem => "Window System",
            DebugSource.DebugSourceShaderCompiler => "Shader Compiler",
            DebugSource.DebugSourceThirdParty => "Third Party",
            DebugSource.DebugSourceApplication => "Application",
            DebugSource.DebugSourceOther => "Other",
            _ => "Unknown"
        };

        var logEvent = type switch
        {
            DebugType.DontCare => LogEventLevel.Debug,
            DebugType.DebugTypeError => LogEventLevel.Error,
            DebugType.DebugTypeOther => LogEventLevel.Information,
            DebugType.DebugTypeMarker => LogEventLevel.Information,
            DebugType.DebugTypePopGroup => LogEventLevel.Information,
            DebugType.DebugTypePushGroup => LogEventLevel.Information,
            DebugType.DebugTypePortability => LogEventLevel.Warning,
            DebugType.DebugTypePerformance => LogEventLevel.Warning,
            DebugType.DebugTypeUndefinedBehavior => LogEventLevel.Error,
            DebugType.DebugTypeDeprecatedBehavior => LogEventLevel.Error,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        Logger.Write(logEvent, "[OpenGL] [{Source} {Id} {Severity}] {Message}", sourceString, id, severityString,
            messageString);

        if (logEvent == LogEventLevel.Error)
            throw new Exception(messageString);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        foreach (var component in _enabledComponents.ToList())
            component.Update(args);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        foreach (var component in _visibleComponents.ToList())
            component.Render(args);

        SwapBuffers();
        base.OnRenderFrame(args);
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        GL.Viewport(0, 0, e.Size.X, e.Size.Y);
        base.OnResize(e);
    }
}