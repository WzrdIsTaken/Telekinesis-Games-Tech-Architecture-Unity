using UnityEngine;

// Makes an object bob up and down

public static class ObjectBob 
{
    // Call every frame to make an object move up and down on a sine wave
    public static void SineWaveBob(GameObject obj, Transform anchor, float wobblePosSpeed, float wobblePosAmount, float wobbleRotSpeed, float wobbleRotAmount)
    {
        float addToPos = Mathf.Sin(Time.time * wobblePosSpeed) * wobblePosAmount;
        obj.transform.localPosition += Vector3.up * addToPos * Time.deltaTime;

        float xRot = Mathf.Sin(Time.time * wobbleRotSpeed) * wobbleRotAmount;
        float zRot = Mathf.Sin((Time.time - 1f) * wobbleRotSpeed) * wobbleRotAmount;

        obj.transform.localEulerAngles = new Vector3(xRot, anchor.eulerAngles.y, zRot);
    }
}