using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Afterlight
{
    public static class MeshFactory
    {
        public static GameObject MeshObject(string name, Mesh mesh, Material material, Transform parent, bool collider = false)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var filter = go.AddComponent<MeshFilter>();
            var renderer = go.AddComponent<MeshRenderer>();
            filter.sharedMesh = mesh;
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = ShadowCastingMode.On;
            renderer.receiveShadows = true;
            if (collider)
            {
                var meshCollider = go.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = mesh;
            }
            return go;
        }

        public static Mesh CreateGabledWalls(float width, float height, float depth, float roofRise)
        {
            float x = width * 0.5f;
            float z = depth * 0.5f;
            var vertices = new List<Vector3>();
            var triangles = new List<int>();

            AddQuad(vertices, triangles,
                new Vector3(-x, 0, z), new Vector3(x, 0, z),
                new Vector3(x, height, z), new Vector3(-x, height, z));
            AddTriangle(vertices, triangles,
                new Vector3(-x, height, z), new Vector3(x, height, z),
                new Vector3(0, height + roofRise, z));

            AddQuad(vertices, triangles,
                new Vector3(x, 0, -z), new Vector3(-x, 0, -z),
                new Vector3(-x, height, -z), new Vector3(x, height, -z));
            AddTriangle(vertices, triangles,
                new Vector3(x, height, -z), new Vector3(-x, height, -z),
                new Vector3(0, height + roofRise, -z));

            AddQuad(vertices, triangles,
                new Vector3(-x, 0, -z), new Vector3(-x, 0, z),
                new Vector3(-x, height, z), new Vector3(-x, height, -z));
            AddQuad(vertices, triangles,
                new Vector3(x, 0, z), new Vector3(x, 0, -z),
                new Vector3(x, height, -z), new Vector3(x, height, z));

            AddQuad(vertices, triangles,
                new Vector3(-x, 0, -z), new Vector3(x, 0, -z),
                new Vector3(x, 0, z), new Vector3(-x, 0, z));

            return BuildMesh("Gabled building walls", vertices, triangles);
        }

        public static Mesh CreateGabledRoof(float width, float height, float depth, float rise, float overhang)
        {
            float x = width * 0.5f + overhang;
            float z = depth * 0.5f + overhang;
            var vertices = new List<Vector3>();
            var triangles = new List<int>();

            Vector3 frontLeft = new Vector3(-x, height, z);
            Vector3 frontRight = new Vector3(x, height, z);
            Vector3 frontRidge = new Vector3(0, height + rise, z);
            Vector3 backLeft = new Vector3(-x, height, -z);
            Vector3 backRight = new Vector3(x, height, -z);
            Vector3 backRidge = new Vector3(0, height + rise, -z);

            AddQuad(vertices, triangles, frontLeft, frontRidge, backRidge, backLeft);
            AddQuad(vertices, triangles, frontRidge, frontRight, backRight, backRidge);
            AddQuad(vertices, triangles, frontLeft, backLeft, backRight, frontRight);
            return BuildMesh("Pitched roof", vertices, triangles);
        }

        public static Mesh CreateEllipsoid(float radiusX, float radiusY, float radiusZ, int segments = 14, int rings = 10)
        {
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            for (int ring = 0; ring <= rings; ring++)
            {
                float v = (float)ring / rings;
                float phi = Mathf.PI * v;
                float y = Mathf.Cos(phi) * radiusY;
                float radial = Mathf.Sin(phi);
                for (int segment = 0; segment <= segments; segment++)
                {
                    float u = (float)segment / segments;
                    float theta = u * Mathf.PI * 2f;
                    vertices.Add(new Vector3(
                        Mathf.Cos(theta) * radial * radiusX,
                        y,
                        Mathf.Sin(theta) * radial * radiusZ));
                }
            }

            for (int ring = 0; ring < rings; ring++)
            {
                for (int segment = 0; segment < segments; segment++)
                {
                    int a = ring * (segments + 1) + segment;
                    int b = a + segments + 1;
                    triangles.Add(a); triangles.Add(b); triangles.Add(a + 1);
                    triangles.Add(a + 1); triangles.Add(b); triangles.Add(b + 1);
                }
            }
            return BuildMesh("Ellipsoid", vertices, triangles);
        }

        public static Mesh CreateTaperedCylinder(float bottomRadius, float topRadius, float height, int segments = 12)
        {
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            for (int i = 0; i <= segments; i++)
            {
                float angle = i * Mathf.PI * 2f / segments;
                float x = Mathf.Cos(angle);
                float z = Mathf.Sin(angle);
                vertices.Add(new Vector3(x * bottomRadius, 0, z * bottomRadius));
                vertices.Add(new Vector3(x * topRadius, height, z * topRadius));
            }

            for (int i = 0; i < segments; i++)
            {
                int a = i * 2;
                int b = a + 1;
                int c = a + 2;
                int d = a + 3;
                triangles.Add(a); triangles.Add(b); triangles.Add(c);
                triangles.Add(c); triangles.Add(b); triangles.Add(d);
            }

            int bottomCenter = vertices.Count;
            vertices.Add(Vector3.zero);
            int topCenter = vertices.Count;
            vertices.Add(new Vector3(0, height, 0));
            for (int i = 0; i < segments; i++)
            {
                int a = i * 2;
                int c = ((i + 1) % segments) * 2;
                triangles.Add(bottomCenter); triangles.Add(c); triangles.Add(a);
                triangles.Add(topCenter); triangles.Add(a + 1); triangles.Add(c + 1);
            }
            return BuildMesh("Tapered cylinder", vertices, triangles);
        }

        public static Mesh CreateTorus(float majorRadius, float minorRadius, int majorSegments = 24, int minorSegments = 8)
        {
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            for (int i = 0; i <= majorSegments; i++)
            {
                float u = (float)i / majorSegments * Mathf.PI * 2f;
                for (int j = 0; j <= minorSegments; j++)
                {
                    float v = (float)j / minorSegments * Mathf.PI * 2f;
                    float radius = majorRadius + minorRadius * Mathf.Cos(v);
                    vertices.Add(new Vector3(radius * Mathf.Cos(u),
                        minorRadius * Mathf.Sin(v), radius * Mathf.Sin(u)));
                }
            }

            for (int i = 0; i < majorSegments; i++)
            {
                for (int j = 0; j < minorSegments; j++)
                {
                    int a = i * (minorSegments + 1) + j;
                    int b = a + minorSegments + 1;
                    triangles.Add(a); triangles.Add(b); triangles.Add(a + 1);
                    triangles.Add(a + 1); triangles.Add(b); triangles.Add(b + 1);
                }
            }
            return BuildMesh("Torus", vertices, triangles);
        }

        public static Mesh CreateRibbon(IReadOnlyList<Vector3> points, float width, float yOffset = 0.02f)
        {
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            if (points == null || points.Count < 2) return new Mesh();

            for (int i = 0; i < points.Count; i++)
            {
                Vector3 direction;
                if (i == 0) direction = points[1] - points[0];
                else if (i == points.Count - 1) direction = points[i] - points[i - 1];
                else direction = points[i + 1] - points[i - 1];

                direction.y = 0;
                direction.Normalize();
                Vector3 side = Vector3.Cross(Vector3.up, direction).normalized * (width * 0.5f);
                Vector3 center = points[i] + Vector3.up * yOffset;
                vertices.Add(center - side);
                vertices.Add(center + side);
            }

            for (int i = 0; i < points.Count - 1; i++)
            {
                int a = i * 2;
                triangles.Add(a); triangles.Add(a + 2); triangles.Add(a + 1);
                triangles.Add(a + 1); triangles.Add(a + 2); triangles.Add(a + 3);
            }
            return BuildMesh("Ribbon", vertices, triangles);
        }

        public static Mesh CreateBoatHull(float length, float width, float height)
        {
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            float halfLength = length * 0.5f;
            float halfWidth = width * 0.5f;
            Vector3 bowTop = new Vector3(0, height, halfLength);
            Vector3 sternTopL = new Vector3(-halfWidth, height, -halfLength);
            Vector3 sternTopR = new Vector3(halfWidth, height, -halfLength);
            Vector3 keelBow = new Vector3(0, 0.15f, halfLength * 0.72f);
            Vector3 keelStern = new Vector3(0, 0.15f, -halfLength * 0.8f);
            Vector3 leftMid = new Vector3(-halfWidth, height * 0.45f, 0);
            Vector3 rightMid = new Vector3(halfWidth, height * 0.45f, 0);

            AddTriangle(vertices, triangles, bowTop, leftMid, keelBow);
            AddQuad(vertices, triangles, leftMid, sternTopL, keelStern, keelBow);
            AddTriangle(vertices, triangles, bowTop, keelBow, rightMid);
            AddQuad(vertices, triangles, rightMid, keelBow, keelStern, sternTopR);
            AddTriangle(vertices, triangles, sternTopL, sternTopR, keelStern);
            AddQuad(vertices, triangles, bowTop, sternTopR, sternTopL, bowTop);
            return BuildMesh("Boat hull", vertices, triangles);
        }

        public static Mesh BuildMesh(string name, List<Vector3> vertices, List<int> triangles)
        {
            var mesh = new Mesh { name = name };
            if (vertices.Count > 65000) mesh.indexFormat = IndexFormat.UInt32;
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        public static void AddQuad(List<Vector3> vertices, List<int> triangles,
            Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            int start = vertices.Count;
            vertices.Add(a); vertices.Add(b); vertices.Add(c); vertices.Add(d);
            triangles.Add(start); triangles.Add(start + 1); triangles.Add(start + 2);
            triangles.Add(start); triangles.Add(start + 2); triangles.Add(start + 3);
        }

        public static void AddTriangle(List<Vector3> vertices, List<int> triangles,
            Vector3 a, Vector3 b, Vector3 c)
        {
            int start = vertices.Count;
            vertices.Add(a); vertices.Add(b); vertices.Add(c);
            triangles.Add(start); triangles.Add(start + 1); triangles.Add(start + 2);
        }
    }
}
