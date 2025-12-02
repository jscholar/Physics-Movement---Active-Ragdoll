using System.Collections.Generic;
using UnityEngine;

public enum ForceType
{
    Force,
    Torque
}

public class MotorOutput
{
    // Force applied to the root rigidbody for steering purposes.
    // APPLED AS A FORCE
    public Vector3 locomotiveForce;

    public List<LocalizedForce> localizedForces;

    public MotorOutput()
    {
        locomotiveForce = Vector3.zero;
        localizedForces = new List<LocalizedForce>();
    }
}

public struct LocalizedForce
{
    public string _debug_ForceName;
    public Vector3 force;
    public ForceType forceType;
    public ForceMode forceMode;
    public Rigidbody rb;

    public void Log()
    {
        // Plan (pseudocode):
        // 1. Build a readable log message containing:
        //    - _debug_ForceName
        //    - force vector (formatted)
        //    - forceType (Force or Torque)
        //    - forceMode (Unity's ForceMode enum)
        //    - rb info: null or rb.name and rb.mass and associated GameObject name
        // 2. If rb != null, use Debug.Log(message, rb.gameObject) to allow Unity to ping the object in the console.
        // 3. Otherwise use Debug.Log(message).
        //
        // Implementation notes:
        // - Use string interpolation for clarity.
        // - Format floats to reasonable precision for readability.
        // - Keep the message compact but informative.

        string forceStr = force.ToString("F3");
        string rbInfo;
        if (rb != null) {
            // Try to include useful details from the Rigidbody and its GameObject
            string rbName = string.IsNullOrEmpty(rb.name) ? "(no name)" : rb.name;
            string goName = rb.gameObject != null ? rb.gameObject.name : "(no gameObject)";
            rbInfo = $"{rbName}, gameObject={goName}, mass={rb.mass:F3}";
        }
        else {
            rbInfo = "null";
        }

        string message = $"[LocalizedForce] Name='{_debug_ForceName}', Force={forceStr}, Type={forceType}, Mode={forceMode}, RB={rbInfo}, MAG={force.magnitude}";

        if (rb != null && rb.gameObject != null) {
            Debug.Log(message, rb.gameObject);
        }
        else {
            Debug.Log(message);
        }
    }
}