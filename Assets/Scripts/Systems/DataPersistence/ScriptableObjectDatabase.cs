using Sirenix.OdinInspector;
using UnityEngine.Assertions;


public class ScriptableObjectDatabase : Singleton<ScriptableObjectDatabase>
{
    [Required] public ScriptableObjectDatabaseData Data;

    protected override void Awake()
    {
        base.Awake();
        Assert.IsNotNull(Data);
    }
}
