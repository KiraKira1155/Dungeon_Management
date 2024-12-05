using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

namespace DebugDraw
{
    public static class DebugDrawLine
    {
        /// <summary>
        /// 四角形の描画
        /// </summary>
        /// <param name="point">中心点</param>
        /// <param name="size">中心点からの距離</param>
        /// <param name="lineColor">描画色</param>
        [BurstCompile]
        public static void DrawBox(Vector2 point, Vector2 size, Color lineColor)
        {
            var box = new Box()
            {
                point = point,
                size = size
            };

            var rightUp = box.RightUp();
            var rightDown = box.RightDown();
            var leftUp = box.LeftUp();
            var leftDown = box.LeftDown();

            Debug.DrawLine(leftUp, rightUp, lineColor);
            Debug.DrawLine(rightUp, rightDown, lineColor);
            Debug.DrawLine(rightDown, leftDown, lineColor);
            Debug.DrawLine(leftDown, leftUp, lineColor);
        }

        /// <summary>
        /// 円の描画
        /// </summary>
        /// <param name="point">中心点</param>
        /// <param name="radius">半径</param>
        /// <param name="segments">描画数、多いほどきれいな円状になる</param>
        /// <param name="lineColor">描画色</param>
        [BurstCompile]
        public static void DrawCircle(Vector2 point, float radius, int segments, Color lineColor)
        {
            var circle = new Circle()
            {
                point = point,
                radius = radius,
                segments = segments,
                angle = 0
            };

            var angleStep = 360.0f / segments;
            var previousPoint = circle.DrawPoint();
            var newPoint = Vector2.zero;

            for (int i = 0; i < segments; i++)
            {
                circle.angle += angleStep;
                newPoint = circle.DrawPoint();
                Debug.DrawLine(previousPoint, newPoint, lineColor);
                previousPoint = newPoint;
            }
        }

        /// <summary>
        /// 扇形の描画
        /// </summary>
        /// <param name="point">中心点</param>
        /// <param name="radius">半径</param>
        /// <param name="angle">角度</param>
        /// <param name="startAngle">開始角度 右側が０℃</param>
        /// <param name="segments">描画数、多いほどきれいな円状になる</param>
        /// <param name="lineColor">描画色</param>
        [BurstCompile]
        public static void DrawFanShape(Vector2 point, float radius, float angle, float startAngle, int segments, Color lineColor)
        {
            var fanShape = new FanShape()
            {
                point = point,
                radius = radius,
                segments = segments,
                currentAngle = startAngle,
            };

            var angleStep = angle / segments;
            var previousPoint = fanShape.DrawPoint();
            var newPoint = Vector2.zero;

            // 中心点から扇形の最初の点までの線
            Debug.DrawLine(point, previousPoint, lineColor);

            for (int i = 0; i < segments; i++)
            {
                fanShape.currentAngle += angleStep;
                newPoint = fanShape.DrawPoint();
                Debug.DrawLine(previousPoint, newPoint, lineColor);
                previousPoint = newPoint;
            }

            // 円弧の最後の点から中心点までの線
            Debug.DrawLine(previousPoint, point, lineColor);
        }
    }

    [BurstCompile]
    public struct Box
    {
        public Vector2 point;
        public Vector2 size;

        public readonly Vector2 RightUp()
        {
            return new Vector2(point.x +size.x / 2, point.y + size.y / 2);
        }

        public readonly Vector2 RightDown()
        {
            return new Vector2(point.x + size.x / 2, point.y - size.y / 2);
        }

        public readonly Vector2 LeftUp()
        {
            return new Vector2(point.x - size.x / 2, point.y + size.y / 2);
        }

        public readonly Vector2 LeftDown()
        {
            return new Vector2(point.x - size.x / 2, point.y - size.y / 2);
        }
    }

    [BurstCompile]
    public struct Circle
    {
        public Vector2 point;
        public float radius;
        public float segments;
        public float angle;

        public readonly Vector2 DrawPoint()
        {
            return point + new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle) * radius, Mathf.Sin(Mathf.Deg2Rad * angle) * radius);
        }
    }

    [BurstCompile]
    public struct FanShape
    {
        public Vector2 point;
        public float radius;
        public int segments;
        public float currentAngle;

        public readonly Vector2 DrawPoint()
        {
            return point + new Vector2(Mathf.Cos(Mathf.Deg2Rad * currentAngle) * radius, Mathf.Sin(Mathf.Deg2Rad * currentAngle) * radius);
        }
    }
}
