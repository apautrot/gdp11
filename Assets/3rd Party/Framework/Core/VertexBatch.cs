using UnityEngine;
using System.Collections;

public class VertexBatch
{
	public Vector3[] vertices = new Vector3[0];
	public Color[] colors = new Color[0];
	public Vector2[] uvs = new Vector2[0];

	public VertexBatch Duplicate ()
	{
		VertexBatch vb = new VertexBatch ();
		
		vb.vertices = new Vector3[vertices.Length];
		vb.colors = new Color[colors.Length];
		vb.uvs = new Vector2[uvs.Length];
		
		System.Array.Copy ( vertices, vb.vertices, vertices.Length );
		System.Array.Copy ( colors, vb.colors, colors.Length );
		System.Array.Copy ( uvs, vb.uvs, uvs.Length );

		return vb;
	}

	public int Count
	{
		get { return vertices.Length; }
	}

	public void setSize ( int size )
	{
		Vector3[] vertices = new Vector3[size];
		Color[] colors = new Color[size];
		Vector2[] uvs = new Vector2[size];

		int previousCount = Count;
		int copyCount = System.Math.Min ( previousCount, size );

		if ( copyCount > 0 )
		{
			System.Array.Copy ( this.vertices, vertices, copyCount );
			System.Array.Copy ( this.colors, colors, copyCount );
			System.Array.Copy ( this.uvs, uvs, copyCount );
		}

		this.vertices = vertices;
		this.colors = colors;
		this.uvs = uvs;
	}

	public Vector2 getDimension ()
	{
		Vector2 boundingRectMin = new Vector2 ();
		Vector2 boundingRectMax = new Vector2 ();
		computeXYBoundingBox ( 0, Count, ref boundingRectMin, ref boundingRectMax );
		return new Vector2 ( boundingRectMax.x - boundingRectMin.x, boundingRectMax.y - boundingRectMin.y );
	}

	public void computeXYBoundingBox ( int index, int count, ref Vector2 min, ref Vector2 max )
	{
		min.x = float.MaxValue;
		min.y = float.MaxValue;
		max.x = float.MinValue;
		max.y = float.MinValue;

		for ( int i = index; i < index + count; i++ )
		{
			float x = vertices[i].x;
			float y = vertices[i].y;
			min.x = System.Math.Min ( min.x, x );
			min.y = System.Math.Min ( min.y, y );
			max.x = System.Math.Max ( max.x, x );
			max.y = System.Math.Max ( max.y, y );
		}
	}

	public void translate ( Vector3 translation )
	{
		translate ( 0, Count, translation );
	}

	public void translate ( int index, int count, Vector3 translation )
	{
		float tx = translation.x;
		float ty = translation.y;
		float tz = translation.z;
		for ( int i = index; i < index + count; i++ )
		{
			vertices[i].x += tx;
			vertices[i].y += ty;
			vertices[i].z += tz;
		}
	}

	public void colorize ( int index, int count, Color color )
	{
		for ( int i = index; i < index + count; i++ )
			colors[i] = color;
	}

	public void setAlpha ( int index, int count, float alpha )
	{
		for ( int i = index; i < index + count; i++ )
			colors[i].a = alpha;
	}

	public void setVertex ( int index, float x, float y, float u, float v, Color color )
	{
		vertices[index].x = x;
		vertices[index].y = y;
		colors[index] = color;
		uvs[index].x = u;
		uvs[index].y = v;
	}

	public void alignVerticesOnIntegers ()
	{
		for ( int i = 0; i < Count; i++ )
		{
			vertices[i].x = (int)vertices[i].x;
			vertices[i].y = (int)vertices[i].y;
		}
	}

	// 	public float getVertexData(int index, int dataOffset)
	// 	{
	// 		return vertices[index * FLOAT_PER_VERTEX + dataOffset];
	// 	}
	// 
	// 	public void setVertexData(int index, int dataOffset, float value)
	// 	{
	// 		vertices[index * FLOAT_PER_VERTEX + dataOffset] = value;
	// 	}
	// 
	// 	public void scaleFromPoint(int index, int size, float centerX, float centerY, float scale)
	// 	{
	// 		index *= FLOAT_PER_VERTEX;
	// 		for (int i = 0; i < size; i++)
	// 		{
	// 			float x = vertices[index + X_OFFSET];
	// 			float y = vertices[index + Y_OFFSET];
	// 			float dx = centerX - x;
	// 			float dy = centerY - y;
	// 			x += dx * scale;
	// 			y += dy * scale;
	// 			vertices[index + X_OFFSET] = x;
	// 			vertices[index + Y_OFFSET] = y;
	// 			index += FLOAT_PER_VERTEX;
	// 		}
	// 	}
	// 
	// 	public void scaleFromPoint(float centerX, float centerY, float scale)
	// 	{
	// 		scaleFromPoint ( 0, size, centerX, centerY, scale );
	// 	}
}