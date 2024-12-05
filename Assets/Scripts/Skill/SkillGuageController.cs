using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class SkillGuageController : MonoBehaviour
{
    private const float TWO_PI = Mathf.PI * 2;

    private static readonly int[] Triangles = new int[]
    {
        4,3,5,  5,3,0,
        3,2,0,  0,2,1,
        5,0,6,  6,0,7,
        0,9,7,  7,9,8,
    };

    private static readonly int[] TrianglesClockwise = new int[]
    {
        2,3,1,  1,3,0,
        3,4,0,  0,4,5,
        9,0,8,  8,0,7,
        0,5,7,  7,5,6,
    };

    private enum StartPosition
    {
        Right = 0,
        Top,
        Left,
        Bottom,
    }

    private Mesh _mesh = null;
    private Vector3[] _vertices = null;
    private Vector2[] _uv = null;

    [SerializeField, Range(0.0f, 1.0f)]
    private float _value = 0f;

    [SerializeField]
    private StartPosition _startPosition = StartPosition.Right;

    [SerializeField]
    private bool _clockwise = false;

    [SerializeField]
    private Texture2D _texture = null;

    void Start()
    {
        CreateMesh();
    }

    void Update()
    {
        UpdateMesh();
    }

    /// <summary>
    /// Mesh���X�V
    /// </summary>
    private void UpdateMesh()
    {
        for (int i = 0; i < _vertices.Length; i++)
        {
            if (i != 0)
            {
                var val = Mathf.Clamp(_value, 0, 0.125f * (i - 1));
                var rad = val * TWO_PI * (_clockwise ? -1 : 1) + Mathf.PI * ((int)_startPosition * 0.5f);

                // normalized rad
                rad = rad - TWO_PI * (int)(rad / TWO_PI);
                if (rad < 0.0f)
                {
                    rad += TWO_PI;
                }

                // rad in top
                if (ValueInRange(rad, Mathf.PI * 0.25f, Mathf.PI * 0.75f))
                {
                    _vertices[i].y = 0.5f;
                    _vertices[i].x = _vertices[i].y / Mathf.Tan(rad);
                }
                // rad in left
                else if (ValueInRange(rad, Mathf.PI * 0.75f, Mathf.PI * 1.25f))
                {
                    _vertices[i].x = -0.5f;
                    _vertices[i].y = Mathf.Tan(rad) * _vertices[i].x;
                }
                // rad in bottom
                else if (ValueInRange(rad, Mathf.PI * 1.25f, Mathf.PI * 1.75f))
                {
                    _vertices[i].y = -0.5f;
                    _vertices[i].x = _vertices[i].y / Mathf.Tan(rad);
                }
                // rad in right
                else
                {
                    _vertices[i].x = 0.5f;
                    _vertices[i].y = Mathf.Tan(rad) * _vertices[i].x;
                }
            }

            _uv[i].x = _vertices[i].x + 0.5f;
            _uv[i].y = _vertices[i].y + 0.5f;
        }

        _mesh.vertices = _vertices;
        _mesh.uv = _uv;
        _mesh.triangles = _clockwise ? TrianglesClockwise : Triangles;
    }

    /// <summary>
    /// �l��min�`max�͈͓̔��ɂ��邩�`�F�b�N�B
    /// value �� min, max �Ɠ����l�̏ꍇ��true�B
    /// </summary>
    private bool ValueInRange(float value, float min, float max)
    {
        return min <= value && value <= max;
    }

    /// <summary>
    /// �`��pMesh����
    /// </summary>
    [ContextMenu("Reset Mesh")]
    private void CreateMesh()
    {
        var renderer = gameObject.GetComponent<MeshRenderer>();
        var meshFilter = gameObject.GetComponent<MeshFilter>();

        int length = 10;
        _vertices = new Vector3[length];
        _uv = new Vector2[length];

        var material = new Material(Shader.Find("Mobile/Particles/Alpha Blended"))
        {
            name = "material"
        };
        material.SetTexture("_MainTex", _texture);

        _mesh = meshFilter.sharedMesh = new Mesh();
        renderer.sharedMaterial = material;

        UpdateMesh();
    }
}
