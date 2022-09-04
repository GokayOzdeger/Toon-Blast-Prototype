public abstract class SingletonGameStateListener<T> : MonoGameStateListener where T : class
{
    public static T Instance { get; private set; }

    protected override void Awake()
    {
        Instance = this as T;
        base.Awake();
    }
}