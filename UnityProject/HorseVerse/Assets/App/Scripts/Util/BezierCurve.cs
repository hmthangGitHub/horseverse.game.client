using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve : MonoBehaviour
{
    //Has to be at least 4 so-called control points
    public Transform startPoint;
    public Transform endPoint;
    public Transform controlPointStart;
    public Transform controlPointEnd;

    //Easier to use ABCD for the positions of the points so they are the same as in the tutorial image
    Vector3 A, B, C, D;

    public void init()
    {
        OnDrawGizmos();
    }

    //Display without having to press play
    void OnDrawGizmos()
    {
        if (startPoint == default || controlPointStart == default || endPoint == default || controlPointEnd == default) return;
        A = startPoint.position;
        B = controlPointStart.position;
        C = controlPointEnd.position;
        D = endPoint.position;

        //The Bezier curve's color
        Gizmos.color = Color.white;

        //The start position of the line
        Vector3 lastPos = A;

        //The resolution of the line
        //Make sure the resolution is adding up to 1, so 0.3 will give a gap at the end, but 0.2 will work
        float resolution = 0.02f;

        //How many loops?
        int loops = Mathf.FloorToInt(1f / resolution);

        for (int i = 1; i <= loops; i++)
        {
            //Which t position are we at?
            float t = i * resolution;

            //Find the coordinates between the control points with a Catmull-Rom spline
            Vector3 newPos = DeCasteljausAlgorithm(t);

            //Draw this line segment
            Gizmos.DrawLine(lastPos, newPos);

            //Save this pos so we can draw the next line segment
            lastPos = newPos;
        }

        //Also draw lines between the control points and endpoints
        Gizmos.color = Color.green;

        Gizmos.DrawLine(A, B);
        Gizmos.DrawLine(C, D);

        DivideCurveIntoSteps();
    }

    //The De Casteljau's Algorithm
    public Vector3 DeCasteljausAlgorithm(float t)
    {
        //Linear interpolation = lerp = (1 - t) * A + t * B
        //Could use Vector3.Lerp(A, B, t)

        //To make it faster
        float oneMinusT = 1f - t;

        //Layer 1
        Vector3 Q = oneMinusT * A + t * B;
        Vector3 R = oneMinusT * B + t * C;
        Vector3 S = oneMinusT * C + t * D;

        //Layer 2
        Vector3 P = oneMinusT * Q + t * R;
        Vector3 T = oneMinusT * R + t * S;

        //Final interpolated position
        Vector3 U = oneMinusT * P + t * T;

        return U;
    }

    Color[] colorsArray = { Color.white, Color.red, Color.blue, Color.magenta, Color.black };

    void DivideCurveIntoSteps()
    {
        //Find the total length of the curve
        float totalLength = GetLengthSimpsons(0f, 1f);
        
        //How many sections do we want to divide the curve into
        int parts = 10;

        //What's the length of one section?
        float sectionLength = totalLength / (float)parts;

        //Init the variables we need in the loop
        float currentDistance = 0f + sectionLength;

        //The curve's start position
        Vector3 lastPos = A;

        //The Bezier curve's color
        //Need a seed or the line will constantly change color
        Random.InitState(12345);

        int lastRandom = Random.Range(0, colorsArray.Length);

        for (int i = 1; i <= parts; i++)
        {
            //Use Newton–Raphsons method to find the t value from the start of the curve 
            //to the end of the distance we have
            float t = FindTValue(currentDistance, totalLength);

            //Get the coordinate on the Bezier curve at this t value
            Vector3 pos = DeCasteljausAlgorithm(t);


            //Draw the line with a random color
            int newRandom = Random.Range(0, colorsArray.Length);

            //Get a different random number each time
            while (newRandom == lastRandom)
            {
                newRandom = Random.Range(0, colorsArray.Length);
            }

            lastRandom = newRandom;

            Gizmos.color = colorsArray[newRandom];

            Gizmos.DrawLine(lastPos, pos);


            //Save the last position
            lastPos = pos;

            //Add to the distance traveled on the line so far
            currentDistance += sectionLength;
        }
    }

    float GetLengthSimpsons(float tStart, float tEnd)
    {
        //This is the resolution and has to be even
        int n = 20;

        //Now we need to divide the curve into sections
        float delta = (tEnd - tStart) / (float)n;

        //The main loop to calculate the length

        //Everything multiplied by 1
        float endPoints = GetArcLengthIntegrand(tStart) + GetArcLengthIntegrand(tEnd);

        //Everything multiplied by 4
        float x4 = 0f;
        for (int i = 1; i < n; i += 2)
        {
            float t = tStart + delta * i;

            x4 += GetArcLengthIntegrand(t);
        }

        //Everything multiplied by 2
        float x2 = 0f;
        for (int i = 2; i < n; i += 2)
        {
            float t = tStart + delta * i;

            x2 += GetArcLengthIntegrand(t);
        }

        //The final length
        float length = (delta / 3f) * (endPoints + 4f * x4 + 2f * x2);

        return length;
    }

    Vector3 DeCasteljausAlgorithmDerivative(float t)
    {
        Vector3 dU = t * t * (-3f * (A - 3f * (B - C) - D));

        dU += t * (6f * (A - 2f * B + C));

        dU += -3f * (A - B);

        return dU;
    }

    float GetArcLengthIntegrand(float t)
    {
        //The derivative at this point (the velocity vector)
        Vector3 dPos = DeCasteljausAlgorithmDerivative(t);

        //This the how it looks like in the YouTube videos
        //float xx = dPos.x * dPos.x;
        //float yy = dPos.y * dPos.y;
        //float zz = dPos.z * dPos.z;

        //float integrand = Mathf.Sqrt(xx + yy + zz);

        //Same as above
        float integrand = dPos.magnitude;

        return integrand;
    }

    float FindTValue(float d, float totalLength)
    {
        //Need a start value to make the method start
        //Should obviously be between 0 and 1
        //We can say that a good starting point is the percentage of distance traveled
        //If this start value is not working you can use the Bisection Method to find a start value
        //https://en.wikipedia.org/wiki/Bisection_method
        float t = d / totalLength;

        //Need an error so we know when to stop the iteration
        float error = 0.001f;

        //We also need to avoid infinite loops
        int iterations = 0;

        while (true)
        {
            //Newton's method
            float tNext = t - ((GetLengthSimpsons(0f, t) - d) / GetArcLengthIntegrand(t));

            //Have we reached the desired accuracy?
            if (Mathf.Abs(tNext - t) < error)
            {
                break;
            }

            t = tNext;

            iterations += 1;

            if (iterations > 1000)
            {
                break;
            }
        }

        return t;
    }

    public Vector3 DeCasteljausAlgorithmWithDistance(float distance, float total)
    {
        if (total == 0) return Vector3.zero;
        float t = FindTValue(distance, total);

        //Get the coordinate on the Bezier curve at this t value
        Vector3 pos = DeCasteljausAlgorithm(t);

        return pos;
    }

    public float GetTotalLength()
    {
        float totalLength = GetLengthSimpsons(0f, 1f);
        return totalLength;
    }
}
