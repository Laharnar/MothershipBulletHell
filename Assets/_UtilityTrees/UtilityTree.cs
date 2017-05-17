using System;
using System.Collections.Generic;

public class UtilityNode {
    public Action method;
    List<ArgsFn> scoreFunctions = new List<ArgsFn>();

    public float sum = 0;

    public UtilityNode(Action method) {
        this.method = method;
    }

    public UtilityNode(Action method, params ArgsFn[] argsFn) : this(method) {
        scoreFunctions.AddRange(argsFn);
    }

    public UtilityNode(string tag, Action method, params ArgsFn[] argsFn) : this(method) {
        scoreFunctions.AddRange(argsFn);
    }

    public virtual void Score() {
        float sum = 0;
        for (int i = 0; i < scoreFunctions.Count; i++) {
            float s = scoreFunctions[i].Call();
            sum += s;
        }
        this.sum = sum;
    }

    public void Add(Func<float> fn) {
        scoreFunctions.Add(new ArgsFn(fn));
    }
    public void Add(Func<float, float> fn, float fl) {
        scoreFunctions.Add(new ArgsFn(fn, fl));
    }

    public void Add(ArgsFn fn) {
        scoreFunctions.Add(fn);
    }
}

public class DecisionMaxChoice : DecisionLeaf {
    public DecisionMaxChoice(string context, params UtilityNode[] choices) : base(context, choices) {
    }

    public override void Do() {
        int max = -1;
        for (int i = 0; i < choices.Count; i++) {
            choices[i].Score();
            if (max == -1) {
                max = i;
            } else if (choices[i].sum > choices[max].sum) {
                max = i;
            }
        }

        choices[max].method();
        maxSum = choices[max].sum;
    }

    internal void AddChoice(UtilityNode choice) {
        choices.Add(choice);
    }
}

public class DecisionMaxPositiveChoice : DecisionLeaf {
   

    public DecisionMaxPositiveChoice(string context, params UtilityNode[] choices) : base(context, choices) {
    }

    // consider only max choices or none
    public override void Do() {
        int max = -1;
        for (int i = 0; i < choices.Count; i++) {
            choices[i].Score();
            if (choices[i].sum >= 0) {
                if (max == -1 || choices[i].sum > choices[max].sum)
                    max = i;
            }
        }
        if (max == -1) {
            return;
        }

        choices[max].method();
        maxSum = choices[max].sum;

    }

    internal void AddChoice(UtilityNode choice) {
        choices.Add(choice);
    }
}

public abstract class DecisionLeaf : Decision {
    public List<UtilityNode> choices = new List<UtilityNode>();

    public float maxSum { get; protected set; }
    
    public DecisionLeaf(string context, params UtilityNode[] choices) {
        AddChoices(choices);
    }

    public DecisionLeaf AddChoices(params UtilityNode[] choices) {
        this.choices.AddRange(choices);
        return this;
    }
}

class UtilityList : Decision {

    protected List<DecisionLeaf> children = new List<DecisionLeaf>();

    public override void Do() {

        for (int i = 0; i < children.Count; i++) {
            children[i].Do();
        }
    }

    public void AddDecisions(params DecisionLeaf[] decisions) {
        children.AddRange(decisions);
    }
}

public abstract class Decision {
    public abstract void Do();
}