using UnityEngine;
using System.Collections.Generic;

public class SequenceNode : BTNode
{
    private List<BTNode> children;

    public SequenceNode(List<BTNode> children)
    {
        this.children = children;
    }

    public override NodeState Evaluate()
    {
        bool anyRunning = false; // <- INI yang harus ditambahkan di sini

        foreach (BTNode node in children)
        {
            switch (node.Evaluate())
            {
                case NodeState.Failure:
                    state = NodeState.Failure;
                    return state;
                case NodeState.Running:
                    anyRunning = true;
                    break;
            }
        }

        state = anyRunning ? NodeState.Running : NodeState.Success;
        return state;
    }
}