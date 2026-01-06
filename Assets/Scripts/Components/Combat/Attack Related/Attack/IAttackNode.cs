using System.Collections.Generic;

public interface IAttackNode
{
     IEnumerable<IAttackNode> GetChildren();
}
