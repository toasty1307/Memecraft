using Memecraft.Components.Test;

namespace Memecraft.Components;

public class GameManager : Component<GameManager>
{
    public GameManager(MemecraftWindow window) : base(window)
    {
#if DEBUG
        new TestTriangle(window);
#else

#endif
    }
}