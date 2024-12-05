using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Player
{
    public static class PlayerAutoAttackGizmos
    {
#if UNITY_EDITOR
        private static readonly int TRIANGLE_COUNT = 12;
        private static readonly Color MESH_COLOR = new Color(255, 0, 0, 0.5f);

        private static List<Vector3> vertices;
        private static float radian;
        private static float startRadian;
        private static float incRadian;
        private static float currentRadian;
        private static Mesh mesh;
        private static List<int> triangleIndexes;

        [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
        public static void DrawPointGizmos(PlayerAutoAttackCollider playerSetting, GizmoType type)
        {
            if (playerSetting.autoAttackDistance <= 0.0f)
            {
                return;
            }

            if (playerSetting.autoAttackAngle > 0.0f)
            {
                Gizmos.color = MESH_COLOR;
                Gizmos.DrawMesh(CreateFanMesh(playerSetting.autoAttackAngle, TRIANGLE_COUNT), playerSetting.attackTransform.position + playerSetting.attackPos, playerSetting.attackTransform.rotation * playerSetting.initAngle, Vector3.one * playerSetting.autoAttackDistance);
            }

        }

        private static Vector3[] CreateFanVertices(float angle, int triangleCnt)
        {
            angle = Mathf.Min(angle, 360.0f);

            vertices = new List<Vector3>(triangleCnt + 2);

            vertices.Add(Vector3.zero);

            // Mathf.Sin()とMathf.Cos()で使用するのは角度ではなくラジアンなので変換しておく。
            radian = angle * Mathf.Deg2Rad;
            startRadian = -radian / 2;
            incRadian = radian / triangleCnt;

            for (int i = 0; i < triangleCnt + 1; ++i)
            {
                currentRadian = startRadian + (incRadian * i);

                Vector3 vertex = new Vector3(Mathf.Sin(currentRadian), 0.0f, Mathf.Cos(currentRadian));
                vertices.Add(vertex);
            }

            return vertices.ToArray();
        }

        private static Mesh CreateFanMesh(float angle, int triangleCnt)
        {
            mesh = new Mesh();

            triangleIndexes = new List<int>(triangleCnt * 3);

            for (int i = 0; i < triangleCnt; ++i)
            {
                triangleIndexes.Add(0);
                triangleIndexes.Add(i + 1);
                triangleIndexes.Add(i + 2);
            }

            mesh.vertices = CreateFanVertices(angle, triangleCnt);
            mesh.triangles = triangleIndexes.ToArray();

            mesh.RecalculateNormals();

            return mesh;
        }
#endif
    }
}