using System;
using UnityEngine;

namespace Parabox.CSG
{
    /// <summary>
    /// Holds information about a single vertex, and provides methods for averaging between many.
    /// <remarks>All values are optional. Where not present a default value will be substituted if necessary.</remarks>
    /// </summary>
    public struct Vertex
    {
        Vector3 m_Position;
        Color m_Color;
        Vector3 m_Normal;
        Vector4 m_Tangent;
        Vector2 m_UV0;
        Vector2 m_UV2;
        Vector4 m_UV3;
        Vector4 m_UV4;
        VertexAttributes m_Attributes;

        /// <value>
        /// The position in model space.
        /// </value>
        public Vector3 Position
        {
            get { return m_Position; }
            set
            {
                HasPosition = true;
                m_Position = value;
            }
        }

        /// <value>
        /// Vertex color *colour :).
        /// </value>
        public Color Colour
        {
            get { return m_Color; }
            set
            {
                HasColor = true;
                m_Color = value;
            }
        }

        /// <value>
        /// Unit vector normal.
        /// </value>
        public Vector3 Normal
        {
            get { return m_Normal; }
            set
            {
                HasNormal = true;
                m_Normal = value;
            }
        }

        /// <value>
        /// Vertex tangent (sometimes called binormal).
        /// </value>
        public Vector4 Tangent
        {
            get { return m_Tangent; }
            set
            {
                HasTangent = true;
                m_Tangent = value;
            }
        }

        /// <value>
        /// UV 0 channel. Also called textures.
        /// </value>
        public Vector2 Uv0
        {
            get { return m_UV0; }
            set
            {
                HasUV0 = true;
                m_UV0 = value;
            }
        }

        /// <value>
        /// UV 2 channel.
        /// </value>
        public Vector2 Uv2
        {
            get { return m_UV2; }
            set
            {
                HasUV2 = true;
                m_UV2 = value;
            }
        }

        /// <value>
        /// UV 3 channel.
        /// </value>
        public Vector4 Uv3
        {
            get { return m_UV3; }
            set
            {
                HasUV3 = true;
                m_UV3 = value;
            }
        }

        /// <value>
        /// UV 4 channel.
        /// </value>
        public Vector4 Uv4
        {
            get { return m_UV4; }
            set
            {
                HasUV4 = true;
                m_UV4 = value;
            }
        }

        /// <summary>
        /// Find if a vertex attribute has been set.
        /// </summary>
        /// <param name="attribute">The attribute or attributes to test for.</param>
        /// <returns>True if this vertex has the specified attributes set, false if they are default values.</returns>
        public bool HasArrays(VertexAttributes attribute)
        {
            return (m_Attributes & attribute) == attribute;
        }

        public bool HasPosition
        {
            get { return (m_Attributes & VertexAttributes.Position) == VertexAttributes.Position; }
            private set { m_Attributes = value ? (m_Attributes | VertexAttributes.Position) : (m_Attributes & ~(VertexAttributes.Position)); }
        }

        public bool HasColor
        {
            get { return (m_Attributes & VertexAttributes.Color) == VertexAttributes.Color; }
            private set { m_Attributes = value ? (m_Attributes | VertexAttributes.Color) : (m_Attributes & ~(VertexAttributes.Color)); }
        }

        public bool HasNormal
        {
            get { return (m_Attributes & VertexAttributes.Normal) == VertexAttributes.Normal; }
            private set { m_Attributes = value ? (m_Attributes | VertexAttributes.Normal) : (m_Attributes & ~(VertexAttributes.Normal)); }
        }

        public bool HasTangent
        {
            get { return (m_Attributes & VertexAttributes.Tangent) == VertexAttributes.Tangent; }
            private set { m_Attributes = value ? (m_Attributes | VertexAttributes.Tangent) : (m_Attributes & ~(VertexAttributes.Tangent)); }
        }

        public bool HasUV0
        {
            get { return (m_Attributes & VertexAttributes.Texture0) == VertexAttributes.Texture0; }
            private set { m_Attributes = value ? (m_Attributes | VertexAttributes.Texture0) : (m_Attributes & ~(VertexAttributes.Texture0)); }
        }

        public bool HasUV2
        {
            get { return (m_Attributes & VertexAttributes.Texture1) == VertexAttributes.Texture1; }
            private set { m_Attributes = value ? (m_Attributes | VertexAttributes.Texture1) : (m_Attributes & ~(VertexAttributes.Texture1)); }
        }

        public bool HasUV3
        {
            get { return (m_Attributes & VertexAttributes.Texture2) == VertexAttributes.Texture2; }
            private set { m_Attributes = value ? (m_Attributes | VertexAttributes.Texture2) : (m_Attributes & ~(VertexAttributes.Texture2)); }
        }

        public bool HasUV4
        {
            get { return (m_Attributes & VertexAttributes.Texture3) == VertexAttributes.Texture3; }
            private set { m_Attributes = value ? (m_Attributes | VertexAttributes.Texture3) : (m_Attributes & ~(VertexAttributes.Texture3)); }
        }

        public void Flip()
        {
            if (HasNormal)
                m_Normal *= -1f;

            if (HasTangent)
                m_Tangent *= -1f;
        }
    }
}