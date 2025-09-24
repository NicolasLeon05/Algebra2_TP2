using System;
using UnityEngine;

namespace CustomMath
{
    [Serializable]
    public struct MyPlane
    {
        #region Variables
        private Vector3 normal;
        private float distance;

        public Vector3 Normal
        {
            get { return normal; }
            set { normal = value; }
        }

        public float Distance
        {
            get { return distance; }
            set { distance = value; }
        }
        #endregion

        #region Constructors
        public MyPlane(Vector3 inNormal, Vector3 inPoint)
        {
            throw new NotImplementedException();
        }

        public MyPlane(Vector3 inNormal, float d)
        {
            throw new NotImplementedException();
        }

        public MyPlane(Vector3 a, Vector3 b, Vector3 c)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Functions
        public void SetNormalAndPosition(Vector3 inNormal, Vector3 inPoint)
        {
            throw new NotImplementedException();
        }

        public void Set3Points(Vector3 a, Vector3 b, Vector3 c)
        {
            throw new NotImplementedException();
        }

        public void Flip()
        {
            throw new NotImplementedException();
        }

        public void Translate(Vector3 translation)
        {
            throw new NotImplementedException();
        }

        public static MyPlane Translate(MyPlane plane, Vector3 translation)
        {
            throw new NotImplementedException();
        }

        public Vector3 ClosestPointOnPlane(Vector3 point)
        {
            throw new NotImplementedException();
        }

        public float GetDistanceToPoint(Vector3 point)
        {
            throw new NotImplementedException();
        }

        public bool GetSide(Vector3 point)
        {
            throw new NotImplementedException();
        }

        public bool SameSide(Vector3 inPt0, Vector3 inPt1)
        {
            throw new NotImplementedException();
        }

        public bool Raycast(Ray ray, out float enter)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Internals
        public override string ToString()
        {
            return $"(normal:{normal}, distance:{distance})";
        }
        #endregion
    }
}
