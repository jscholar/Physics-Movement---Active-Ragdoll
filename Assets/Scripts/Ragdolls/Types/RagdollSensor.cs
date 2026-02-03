using System;
using System.Collections.Generic;
using UnityEngine;

public class ReadingStore
{
    private readonly Dictionary<Type, object> _data = new();

    public Reading GetOrCreate<Reading>() where Reading : class, new()
    {
        if (_data.TryGetValue(typeof(Reading), out var obj)) return (Reading)obj;
        Reading reading = new Reading();
        _data[typeof(Reading)] = reading;
        return reading;
    }

    /**
     * Gets the sensor of type Reading if it exists in the store.
     * Returns true if found, false otherwise.
     * 
     * Value is set to the found sensor or null if not found.
     */
    public bool TryGet<Reading>(out Reading reading) where Reading: class
    {
        if (_data.TryGetValue(typeof(Reading), out var obj)) {
            reading = (Reading)obj;
            return true;
        }

        reading = null;
        return false;
    }

    public void Log()
    {
        foreach (var kvp in _data) {
            Debug.Log($"ReadingStore contains: {kvp.Key.Name}");
        }
    }
}

public abstract class RagdollSensor: MonoBehaviour {
    public abstract void GetReading(RagdollContext context);
}

public class CoMReading {
    public Vector3 value;
}

public enum BalanceState {
    Balanced,
    Unbalanced,
    Stepping,
    Falling
}

public class RaibertSensorReading
{
    public bool hasLeftFootTarget;
    public RaycastHit leftFootTarget;

    public bool hasRightFootTarget;
    public RaycastHit rightFootTarget;

    public BalanceState balanceState;
}
