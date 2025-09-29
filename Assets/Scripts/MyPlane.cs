using System;
using Unity.Mathematics;
using UnityEngine;

namespace CustomMath
{
    [Serializable]
    public struct MyPlane
    {
        #region Variables
        public Vector3 normal;
        public float distance;

        public Vector3 Point
        {
            get { return -distance * normal; }
        }
        #endregion

        #region Constructors
        public MyPlane(Vector3 inNormal, Vector3 inPoint)
        {
            normal = inNormal.normalized;
            distance = -Vector3.Dot(normal, inPoint);
        }

        #endregion

        #region Functions

        public void Flip()
        {
            normal = -normal;
            distance = -distance;
        }

        public void Translate(Vector3 translation)
        {
            distance -= Vector3.Dot(normal, translation);
        }

        public float GetDistanceToPoint(Vector3 point)
        {
            return Vector3.Dot(normal, point) + distance;
        }

        public bool GetSide(Vector3 point)
        {
            return GetDistanceToPoint(point) > 0f;
        }
        #endregion

        #region Internals
        public override string ToString()
        {
            return $"(normal:{normal}, distance:{distance})";
        }

        public override bool Equals(object other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(MyPlane other)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
