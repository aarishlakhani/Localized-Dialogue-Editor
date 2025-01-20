using UnityEngine;
using XNode;

public class StartNode : BaseNode
{
    [Output] public int exit; // Exit port to the first dialogue node

    public override string GetString()
    {
        return "Start";
    }
}