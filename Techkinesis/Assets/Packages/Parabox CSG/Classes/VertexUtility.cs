using System;
using System.Collections.Generic;
using UnityEngine;

namespace Parabox.CSG
{
    static class VertexUtility
    {
        /// <summary>
        /// Allocate and fill all attribute arrays. This method will fill all arrays, regardless of whether or not real data populates the values (check what attributes a Vertex contains with HasAttribute()).
        /// </summary>
        /// <remarks>
        /// If you are using this function to rebuild a mesh, use SetMesh instead. SetMesh handles setting null arrays where appropriate for you.
        /// </remarks>
        /// <seealso cref="SetMesh"/>
        /// <param name="vertices">The source vertices.</param>
        /// <param name="position">A new array of the vertex position values.</param>
        /// <param name="color">A new array of the vertex color values.</param>
        /// <param name="uv0">A new array of the vertex uv0 values.</param>
        /// <param name="normal">A new array of the vertex normal values.</param>
        /// <param name="tangent">A new array of the vertex tangent values.</param>
        /// <param name="uv2">A new array of the vertex uv2 values.</param>
        /// <param name="uv3">A new array of the vertex uv3 values.</param>
        /// <param name="uv4">A new array of the vertex uv4 values.</param>
        public static void GetArrays(
            IList<Vertex> vertices,
            out Vector3[] position,
            out Color[] color,
            out Vector2[] uv0,
            out Vector3[] normal,
            out Vector4[] tangent,
            out Vector2[] uv2,
            out List<Vector4> uv3,
            out List<Vector4> uv4)
        {
            GetArrays(vertices, out position, out color, out uv0, out normal, out tangent, out uv2, out uv3, out uv4, VertexAttributes.All);
        }

        /// <summary>
        /// Allocate and fill the requested attribute arrays.
        /// </summary>
        /// <remarks>
        /// If you are using this function to rebuild a mesh, use SetMesh instead. SetMesh handles setting null arrays where appropriate for you.
        /// </remarks>
        /// <seealso cref="SetMesh"/>
        /// <param name="vertices">The source vertices.</param>
        /// <param name="position">A new array of the vertex position values if requested by the attributes parameter, or null.</param>
        /// <param name="color">A new array of the vertex color values if requested by the attributes parameter, or null.</param>
        /// <param name="uv0">A new array of the vertex uv0 values if requested by the attributes parameter, or null.</param>
        /// <param name="normal">A new array of the vertex normal values if requested by the attributes parameter, or null.</param>
        /// <param name="tangent">A new array of the vertex tangent values if requested by the attributes parameter, or null.</param>
        /// <param name="uv2">A new array of the vertex uv2 values if requested by the attributes parameter, or null.</param>
        /// <param name="uv3">A new array of the vertex uv3 values if requested by the attributes parameter, or null.</param>
        /// <param name="uv4">A new array of the vertex uv4 values if requested by the attributes parameter, or null.</param>
        /// <param name="attributes">A flag with the MeshAttributes requested.</param>
        /// <seealso cref="HasArrays"/>
        public static void GetArrays(
            IList<Vertex> vertices,
            out Vector3[] position,
            out Color[] color,
            out Vector2[] uv0,
            out Vector3[] normal,
            out Vector4[] tangent,
            out Vector2[] uv2,
            out List<Vector4> uv3,
            out List<Vector4> uv4,
            VertexAttributes attributes)
        {
            if (vertices == null)
                throw new ArgumentNullException("vertices");

            int vc = vertices.Count;
            var first = vc < 1 ? new Vertex() : vertices[0];

            bool hasPosition = ((attributes & VertexAttributes.Position) == VertexAttributes.Position) && first.HasPosition;
            bool hasColor = ((attributes & VertexAttributes.Color) == VertexAttributes.Color) && first.HasColor;
            bool hasUv0 = ((attributes & VertexAttributes.Texture0) == VertexAttributes.Texture0) && first.HasUV0;
            bool hasNormal = ((attributes & VertexAttributes.Normal) == VertexAttributes.Normal) && first.HasNormal;
            bool hasTangent = ((attributes & VertexAttributes.Tangent) == VertexAttributes.Tangent) && first.HasTangent;
            bool hasUv2 = ((attributes & VertexAttributes.Texture1) == VertexAttributes.Texture1) && first.HasUV2;
            bool hasUv3 = ((attributes & VertexAttributes.Texture2) == VertexAttributes.Texture2) && first.HasUV3;
            bool hasUv4 = ((attributes & VertexAttributes.Texture3) == VertexAttributes.Texture3) && first.HasUV4;

            position = hasPosition ? new Vector3[vc] : null;
            color = hasColor ? new Color[vc] : null;
            uv0 = hasUv0 ? new Vector2[vc] : null;
            normal = hasNormal ? new Vector3[vc] : null;
            tangent = hasTangent ? new Vector4[vc] : null;
            uv2 = hasUv2 ? new Vector2[vc] : null;
            uv3 = hasUv3 ? new List<Vector4>(vc) : null;
            uv4 = hasUv4 ? new List<Vector4>(vc) : null;

            for (int i = 0; i < vc; i++)
            {
                if (hasPosition)
                    position[i] = vertices[i].Position;
                if (hasColor)
                    color[i] = vertices[i].Colour;
                if (hasUv0)
                    uv0[i] = vertices[i].Uv0;
                if (hasNormal)
                    normal[i] = vertices[i].Normal;
                if (hasTangent)
                    tangent[i] = vertices[i].Tangent;
                if (hasUv2)
                    uv2[i] = vertices[i].Uv2;
                if (hasUv3)
                    uv3.Add(vertices[i].Uv3);
                if (hasUv4)
                    uv4.Add(vertices[i].Uv4);
            }
        }

        public static Vertex[] GetVertices(this Mesh mesh)
        {
            if (mesh == null)
                return null;

            int vertexCount = mesh.vertexCount;
            Vertex[] v = new Vertex[vertexCount];

            Vector3[] positions = mesh.vertices;
            Color[] colors = mesh.colors;
            Vector3[] normals = mesh.normals;
            Vector4[] tangents = mesh.tangents;
            Vector2[] uv0s = mesh.uv;
            Vector2[] uv2s = mesh.uv2;
            List<Vector4> uv3s = new List<Vector4>();
            List<Vector4> uv4s = new List<Vector4>();
            mesh.GetUVs(2, uv3s);
            mesh.GetUVs(3, uv4s);

            bool _hasPositions = positions != null && positions.Length == vertexCount;
            bool _hasColors = colors != null && colors.Length == vertexCount;
            bool _hasNormals = normals != null && normals.Length == vertexCount;
            bool _hasTangents = tangents != null && tangents.Length == vertexCount;
            bool _hasUv0 = uv0s != null && uv0s.Length == vertexCount;
            bool _hasUv2 = uv2s != null && uv2s.Length == vertexCount;
            bool _hasUv3 = uv3s.Count == vertexCount;
            bool _hasUv4 = uv4s.Count == vertexCount;

            for (int i = 0; i < vertexCount; i++)
            {
                v[i] = new Vertex();

                if (_hasPositions)
                    v[i].Position = positions[i];

                if (_hasColors)
                    v[i].Colour = colors[i];

                if (_hasNormals)
                    v[i].Normal = normals[i];

                if (_hasTangents)
                    v[i].Tangent = tangents[i];

                if (_hasUv0)
                    v[i].Uv0 = uv0s[i];

                if (_hasUv2)
                    v[i].Uv2 = uv2s[i];

                if (_hasUv3)
                    v[i].Uv3 = uv3s[i];

                if (_hasUv4)
                    v[i].Uv4 = uv4s[i];
            }

            return v;
        }

        /// <summary>
        /// Replace mesh values with vertex array. Mesh is cleared during this function, so be sure to set the triangles after calling.
        /// </summary>
        /// <param name="mesh">The target mesh.</param>
        /// <param name="vertices">The vertices to replace the mesh attributes with.</param>
        public static void SetMesh(Mesh mesh, IList<Vertex> vertices)
        {
            if (mesh == null)
                throw new ArgumentNullException("mesh");

            if (vertices == null)
                throw new ArgumentNullException("vertices");

#pragma warning disable IDE0018 // Inline variable declaration
            Vector3[] positions = null;
            Color[] colors = null;
            Vector2[] uv0s = null;
            Vector3[] normals = null;
            Vector4[] tangents = null;
            Vector2[] uv2s = null;
            List<Vector4> uv3s = null;
            List<Vector4> uv4s = null;
#pragma warning restore IDE0018 // Inline variable declaration

            GetArrays(vertices, out positions,
                out colors,
                out uv0s,
                out normals,
                out tangents,
                out uv2s,
                out uv3s,
                out uv4s);

            mesh.Clear();

            Vertex first = vertices[0];

            if (first.HasPosition) mesh.vertices = positions;
            if (first.HasColor) mesh.colors = colors;
            if (first.HasUV0) mesh.uv = uv0s;
            if (first.HasNormal) mesh.normals = normals;
            if (first.HasTangent) mesh.tangents = tangents;
            if (first.HasUV2) mesh.uv2 = uv2s;
            if (first.HasUV3)
                if (uv3s != null)
                    mesh.SetUVs(2, uv3s);
            if (first.HasUV4)
                if (uv4s != null)
                    mesh.SetUVs(3, uv4s);
        }

        /// <summary>
        /// Linearly interpolate between two vertices.
        /// </summary>
        /// <param name="x">Left parameter.</param>
        /// <param name="y">Right parameter.</param>
        /// <param name="weight">The weight of the interpolation. 0 is fully x, 1 is fully y.</param>
        /// <returns>A new vertex interpolated by weight between x and y.</returns>
        public static Vertex Mix(this Vertex x, Vertex y, float weight)
        {
            float i = 1f - weight;

            Vertex v = new Vertex
            {
                Position = x.Position * i + y.Position * weight
            };

            if (x.HasColor && y.HasColor)
                v.Colour = x.Colour * i + y.Colour * weight;
            else if (x.HasColor)
                v.Colour = x.Colour;
            else if (y.HasColor)
                v.Colour = y.Colour;

            if (x.HasNormal && y.HasNormal)
                v.Normal = x.Normal * i + y.Normal * weight;
            else if (x.HasNormal)
                v.Normal = x.Normal;
            else if (y.HasNormal)
                v.Normal = y.Normal;

            if (x.HasTangent && y.HasTangent)
                v.Tangent = x.Tangent * i + y.Tangent * weight;
            else if (x.HasTangent)
                v.Tangent = x.Tangent;
            else if (y.HasTangent)
                v.Tangent = y.Tangent;

            if (x.HasUV0 && y.HasUV0)
                v.Uv0 = x.Uv0 * i + y.Uv0 * weight;
            else if (x.HasUV0)
                v.Uv0 = x.Uv0;
            else if (y.HasUV0)
                v.Uv0 = y.Uv0;

            if (x.HasUV2 && y.HasUV2)
                v.Uv2 = x.Uv2 * i + y.Uv2 * weight;
            else if (x.HasUV2)
                v.Uv2 = x.Uv2;
            else if (y.HasUV2)
                v.Uv2 = y.Uv2;

            if (x.HasUV3 && y.HasUV3)
                v.Uv3 = x.Uv3 * i + y.Uv3 * weight;
            else if (x.HasUV3)
                v.Uv3 = x.Uv3;
            else if (y.HasUV3)
                v.Uv3 = y.Uv3;

            if (x.HasUV4 && y.HasUV4)
                v.Uv4 = x.Uv4 * i + y.Uv4 * weight;
            else if (x.HasUV4)
                v.Uv4 = x.Uv4;
            else if (y.HasUV4)
                v.Uv4 = y.Uv4;

            return v;
        }

        /// <summary>
        /// Transform a vertex into world space.
        /// </summary>
        /// <param name="transform">The transform to apply.</param>
        /// <param name="vertex">A model space vertex.</param>
        /// <returns>A new vertex in world coordinate space.</returns>
        public static Vertex TransformVertex(this Transform transform, Vertex vertex)
        {
            var v = new Vertex();

            if (vertex.HasArrays(VertexAttributes.Position))
                v.Position = transform.TransformPoint(vertex.Position);

            if (vertex.HasArrays(VertexAttributes.Color))
                v.Colour = vertex.Colour;

            if (vertex.HasArrays(VertexAttributes.Normal))
                v.Normal = transform.TransformDirection(vertex.Normal);

            if (vertex.HasArrays(VertexAttributes.Tangent))
                v.Tangent = transform.rotation * vertex.Tangent;

            if (vertex.HasArrays(VertexAttributes.Texture0))
                v.Uv0 = vertex.Uv0;

            if (vertex.HasArrays(VertexAttributes.Texture1))
                v.Uv2 = vertex.Uv2;

            if (vertex.HasArrays(VertexAttributes.Texture2))
                v.Uv3 = vertex.Uv3;

            if (vertex.HasArrays(VertexAttributes.Texture3))
                v.Uv4 = vertex.Uv4;

            return v;
        }
    }
}