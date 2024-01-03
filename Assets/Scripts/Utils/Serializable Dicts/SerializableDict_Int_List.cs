using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDict_Int_List
{
    [System.Serializable]
    public class ConditionList
    {
        public int key;
        public List<ICondition> value;
    }

    public List<ConditionList> slotRulesList;
}
