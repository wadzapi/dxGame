using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace MarsDX
{
    public struct UVSprWSprH
    {
        public Vector4 data;

        public UVSprWSprH(Vector2 UV, float SprW, float SprH)
        {
            data = new Vector4(UV.X, UV.Y, SprW, SprH);
        }
        public UVSprWSprH(float U, float W, float SprW, float SprH)
        {
            data = new Vector4(U, W, SprW, SprH); 
        }
    }

    public enum StreamSourceFrequency //Частоты вередачи vb в шедер(HWInstancing)
    {
        IndexedObjectData = (1 << 30),
        InstanceData = (2 << 30)
    }

    public struct InstanceData
    {
        public Vector4 model_matrix0;
        public Vector4 model_matrix1;
        public Vector4 model_matrix2;
        public Vector4 model_matrix3;
        public Vector4 uVSprWSprH;

        public InstanceData(Matrix m, UVSprWSprH UVwh)
        {
            this.model_matrix0 = new Vector4(m.M11, m.M12, m.M13, m.M14);
            this.model_matrix1 = new Vector4(m.M21, m.M22, m.M23, m.M24);
            this.model_matrix2 = new Vector4(m.M31, m.M32, m.M33, m.M34);
            this.model_matrix3 = new Vector4(m.M41, m.M42, m.M43, m.M44);
            this.uVSprWSprH = UVwh.data;
        }

        public static readonly VertexFormats Format =  VertexFormats.Texture1| VertexFormats.Texture2| VertexFormats.Texture3| VertexFormats.Texture4| VertexFormats.Texture5;
        public static readonly VertexElement[] Declaration = new VertexElement[]
		{
            new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
            new VertexElement(0, 12, DeclarationType.Float1, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
            new VertexElement(1, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 1),
            new VertexElement(1, 16, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 2),
            new VertexElement(1, 32, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 3),
            new VertexElement(1, 48, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 4),
            new VertexElement(1, 64, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 5),
			VertexElement.VertexDeclarationEnd
		};
    }

    public struct GeometryData
    {
        public Vector3 Position;
        public float Corner;

        public GeometryData(CustomVertex.PositionOnly vert, float corner)
        {
            this.Position = vert.Position;
            this.Corner = corner;
        }

        public static readonly VertexFormats Format = VertexFormats.Position| VertexFormats.Texture0;
    }
}
