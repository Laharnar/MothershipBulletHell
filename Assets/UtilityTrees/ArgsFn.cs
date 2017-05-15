using System;

/// <summary>
/// Allows you to pass different params as a function
/// </summary>
public class ArgsFn {
    private Func<int, float> intFn;
    private Func<object, float> objFn;
    private Func<float, float> floFn;
    private Func<float> fn;
    private int intArg;
    private float floArg;
    private object objArg;

    public ArgsFn(Func<float> fn) {
        this.fn = fn;
    }

    public ArgsFn(Func<int, float> fn, int value) {
        this.intFn = fn;
        this.intArg = value;
    }


    public ArgsFn(Func<float, float> fn, float value) {
        this.floFn = fn;
        this.floArg = value;
    }


    public ArgsFn(Func<object, float> fn, object value) {
        this.objFn = fn;
        this.objArg = value;
    }

    public float Call() {
        if (fn != null) return fn();
        else if (intFn != null) return intFn(intArg);
        else if (objFn != null) return objFn(objArg);
        else if (floFn != null) return floFn(floArg);
        else UnityEngine.Debug.LogWarning("All functions are null.");
        return int.MinValue;
    }
}
