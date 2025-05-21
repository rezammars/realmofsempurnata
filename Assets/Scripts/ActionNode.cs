using UnityEngine;
using System;

public class ActionNode : BTNode
{
    private Func<NodeState> action;

    public ActionNode(Func<NodeState> action)
    {
        this.action = action;
    }

    public override NodeState Evaluate()
    {
        state = action.Invoke();
        return action();
    }
}