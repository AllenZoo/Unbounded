public class PlayerSingleton : Singleton<PlayerSingleton>
{
    public StatComponent GetPlayerStatComponent()
    {
        return GetComponentInChildren<StatComponent>();
    }
}
