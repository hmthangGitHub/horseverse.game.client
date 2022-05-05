using System.Collections;
using System.Collections.Generic;
using FixRound;
using UnityEngine;

namespace Core{
    public static class MathCustom
    {
        public static float GetAngleFromNagative180To180(Vector3 direct1, Vector3 direct2)
        {
            return Vector3.Angle(direct1, direct2) * Mathf.Sign(Vector3.Cross(direct1, direct2).y);
        }

        public static Vector3 GetDirectionWithAngleAndCurrentDirection(float angle, Vector3 currentDirection)
        {
            return Quaternion.AngleAxis(angle, Vector3.up) * currentDirection;
        }

        public static Vector3 GetDirectionFromEulerAngle(Vector3 eulerAngle)
        {
            Quaternion rotate = Quaternion.Euler(eulerAngle);
            return GetDirectionFromQuaternion(rotate);
        }

        public static Vector3 GetDirectionFromQuaternion(Quaternion rotate)
        {
            return rotate * Vector3.forward;
        }

        public static Vector3 GetEulerAngleFromDirect(Vector3 direct)
        {
            Quaternion rotate = Quaternion.LookRotation(direct);
            return rotate.eulerAngles;
        }

        #region colider
        public static bool CollideBetween2Circle(Vector3 position1, Vector3 position2, float radius1, float radius2){
            return Vector3.Distance(position1, position2) <= radius1 + radius2;
        }
        #endregion

        public static bool CheckInArc(Vector3 rootDirection, Vector3 rootPosition, Vector3 targetPosition, float radiusOfTarget, float range, float angle)
        {
            float distance = FloatValue.Round(Vector3.Distance(rootPosition, targetPosition));
            if (distance > FloatValue.Round(range + radiusOfTarget))
                return false;

            Vector3 direction = Vector3Value.Round(targetPosition - rootPosition);
            float halfAngle = FloatValue.Round(angle / 2f);
            float checkAngle = FloatValue.Round(Vector3.Angle(direction, rootDirection));
            float offsetAngle = FloatValue.Round(Mathf.Abs(checkAngle - halfAngle));
            float oppositeEdge = FloatValue.Round(Mathf.Sin(offsetAngle * FloatValue.Round(Mathf.PI) / 180) * distance);

            if (checkAngle <= halfAngle)
            {
                return true;
            }
            else if (offsetAngle >= 90)
            {
                return false;
            }
            else if (oppositeEdge <= FloatValue.Round(radiusOfTarget))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CheckInRound(Vector3 rootPosition, Vector3 targetPosition, float radiusOfTarget, float range)
        {
            float distance = FloatValue.Round(Vector3.Distance(rootPosition, targetPosition));
            return distance <= FloatValue.Round(range + radiusOfTarget);
        }

        public static bool CheckInStraight(Vector3 rootDirection, Vector3 rootPosition, Vector3 targetPosition, float radiusOfTarget, float range, float width)
        {
            float _radiusTrigger = FloatValue.Round(radiusOfTarget + width / 2);
            return CheckInArc(rootDirection, rootPosition, targetPosition, _radiusTrigger, range, 0f);
        }

        public static bool FindPointInFullParabol(Vector3 start, Vector3 end, float height, float percent, out Vector3 point)
        {
            if (start == end || height <= 0)
            {
                point = Vector3.zero;
                return false;
            }

            var startTemp = start;
            startTemp.y = 0;
            var endTemp = end;
            endTemp.y = 0;
            float distanceX = Vector3Value.Distance(startTemp, endTemp);
            distanceX = distanceX <= 0 ? 0.1f : distanceX;
            float distanceY = end.y - start.y;

            float x0 = 0;
            float y0 = 0;
            float x1 = (x0 + distanceX) / 2;
            float y1 = (y0 + distanceY) / 2 + height;
            float x2 = distanceX;
            float y2 = distanceY;

            float a = ((y1 - y0) - (x1 - x0) * (y2 - y1) / (x2 - x1)) / ((x0 - x2) * (x1 - x0));
            float b = ((y2 - y1) - a * (x2 * x2 - x1 * x1)) / (x2 - x1);
            float c = y1 - a * x1 * x1 - b * x1;

            float distance = Vector3Value.Distance(start, end);
            Vector3 direct = Vector3Value.Normalize(end, start);
            point = Vector3Value.Round(start + direct * distance * percent);
            float x = distanceX * percent;
            point.y = FloatValue.Round(a * Mathf.Pow(x, 2) + b * x + c);

            return true;
        }
    }
}

