using UnityEngine;
using System.Collections;

public class QuadBatch
{
	protected VertexBatch vertexBatch;
	public VertexBatch VertexBatch
	{
		get { return vertexBatch; }
	}

	public QuadBatch ()
	{
		vertexBatch = new VertexBatch ();
	}

	public QuadBatch ( VertexBatch vb )
	{
		vertexBatch = vb;
	}

	public QuadBatch Duplicate ()
	{
		return new QuadBatch ( vertexBatch.Duplicate () );
	}

	public void Set ( QuadBatch qb )
	{
		vertexBatch = qb.vertexBatch.Duplicate ();
	}

	public Mesh CreateMesh ( Mesh reuse = null )
	{
		if ( Count == 0 )
			return null;

		Mesh mesh = reuse != null ? reuse : new Mesh();
		mesh.triangles = null;
		mesh.vertices = vertexBatch.vertices;
		mesh.uv = vertexBatch.uvs;
		mesh.colors = vertexBatch.colors;
		
		int[] indices = new int[Count*6];
		int index = 0;
		for ( int i = 0; i < Count; i++, index+=6 )
		{
			indices[index + 0] = ( i * 4 ) + 0;
			indices[index + 1] = ( i * 4 ) + 1;
			indices[index + 2] = ( i * 4 ) + 2;
			indices[index + 3] = ( i * 4 ) + 0;
			indices[index + 4] = ( i * 4 ) + 2;
			indices[index + 5] = ( i * 4 ) + 3;
		}

		mesh.triangles = indices;

		return mesh;
	}

	public int Count
	{
		get { return vertexBatch.Count / 4; }
	}

	public void setSize ( int size )
	{
		vertexBatch.setSize ( size * 4 );
	}

	public Rect getBounds ()
	{
		return getBounds ( 0, Count );
	}

	public Rect getBounds ( int index, int count )
	{
		if ( count == 0 )
			return new Rect ( 0, 0, 0, 0 );

		float minX = float.MaxValue;
		float minY = float.MaxValue;
		float maxX = float.MinValue;
		float maxY = float.MinValue;

		count += index;
		for ( int i = index; i < count; i++ )
		{
			Vector3 tl = vertexBatch.vertices[i * 4];
			Vector3 br = vertexBatch.vertices[i * 4 + 2];

			minX = System.Math.Min ( minX, tl.x );
			minY = System.Math.Min ( minY, tl.y );
			maxX = System.Math.Max ( maxX, tl.x );
			maxY = System.Math.Max ( maxY, tl.y );

			minX = System.Math.Min ( minX, br.x );
			minY = System.Math.Min ( minY, br.y );
			maxX = System.Math.Max ( maxX, br.x );
			maxY = System.Math.Max ( maxY, br.y );

// 			// top left vertex x,y
// 			minX = System.Math.Min ( minX, vertexBatch.vertices[i * 4].x );
// 			minY = System.Math.Min ( minY, vertexBatch.vertices[i * 4].y );
// 
// 			// bottom right vertex x,y
// 			maxX = System.Math.Max ( maxX, vertexBatch.vertices[i * 4 + 2].x );
// 			maxY = System.Math.Max ( maxY, vertexBatch.vertices[i * 4 + 2].y );
		}

		return new Rect ( minX, minY, maxX - minX, maxY - minY );
	}

	public void translate ( Vector3 translation )
	{
		translate ( 0, Count, translation );
	}

	public void translate ( int index, int count, Vector3 translation )
	{
		vertexBatch.translate ( index * 4, count * 4, translation );
	}

	public void scramble ( Vector3 amount )
	{
		scramble ( 0, Count, amount );
	}

	public void scramble ( int index, int count, Vector3 amount )
	{
		for ( int i = 0; i < count; i++ )
		{
			Vector3 v = new Vector3
			(
				Random.Range ( -amount.x, amount.x ),
				Random.Range ( -amount.y, amount.y ),
				Random.Range ( -amount.z, amount.z )
			);
			translate ( index + i, 1, v );
		}
	}

	public void scale ( float factor )
	{
		scale ( 0, Count, factor );
	}

	public void scale ( int index, int count, float scale )
	{
		scale -= 1;
		if ( scale != 0 )
			for ( int i = 0; i < count; i++ )
			{
				int vidx = ( index + i ) * 4;
				Vector3 bl = vertexBatch.vertices[vidx + 0];
				Vector3 tl = vertexBatch.vertices[vidx + 1];
				Vector3 tr = vertexBatch.vertices[vidx + 2];
				// Vector3 br = vertexBatch.vertices[vidx + 3];
				Vector2 m = new Vector3 ( tr.x - tl.x, bl.y - tl.y ) * scale;
				vertexBatch.vertices[vidx + 0] += new Vector3 ( -m.x, m.y, 0 );
				vertexBatch.vertices[vidx + 1] += new Vector3 ( -m.x, -m.y, 0 );
				vertexBatch.vertices[vidx + 2] += new Vector3 ( m.x, -m.y, 0 );
				vertexBatch.vertices[vidx + 3] += new Vector3 ( m.x, m.y, 0 );
			}
	}

	public void lerpGeometry ( QuadBatch target, float factor )
	{
		lerpGeometry ( 0, Count, target, factor );
	}

	public void lerpGeometry ( int index, int count, QuadBatch target, float factor )
	{
		for ( int i = 0; i < count; i++ )
		{
			int vidx = ( index + i ) * 4;
			Vector3 bl = vertexBatch.vertices[vidx + 0];
			Vector3 tl = vertexBatch.vertices[vidx + 1];
			Vector3 tr = vertexBatch.vertices[vidx + 2];
			Vector3 br = vertexBatch.vertices[vidx + 3];

			Vector3 tbl = target.vertexBatch.vertices[vidx + 0];
			Vector3 ttl = target.vertexBatch.vertices[vidx + 1];
			Vector3 ttr = target.vertexBatch.vertices[vidx + 2];
			Vector3 tbr = target.vertexBatch.vertices[vidx + 3];

			vertexBatch.vertices[vidx + 0] = Vector3.Lerp ( bl, tbl, factor );
			vertexBatch.vertices[vidx + 1] = Vector3.Lerp ( tl, ttl, factor );
			vertexBatch.vertices[vidx + 2] = Vector3.Lerp ( tr, ttr, factor );
			vertexBatch.vertices[vidx + 3] = Vector3.Lerp ( br, tbr, factor );
		}
	}

	public void setQuad ( int index, Vector3 position, Vector2 dimension, Vector2 uvtl, Vector2 uvbr, Color color )
	{
		// to feed the sprite batch, order of vertices is : bottom left, top left, top right, bottom right
		// (note that uv top/bottom are invert of position top/bottom)
		vertexBatch.setVertex ( index * 4 + 0, position.x, position.y, uvtl.x, uvtl.y, color );
		vertexBatch.setVertex ( index * 4 + 1, position.x, position.y + dimension.y, uvtl.x, uvbr.y, color );
		vertexBatch.setVertex ( index * 4 + 2, position.x + dimension.x, position.y + dimension.y, uvbr.x, uvbr.y, color );
		vertexBatch.setVertex ( index * 4 + 3, position.x + dimension.x, position.y, uvbr.x, uvtl.y, color );
	}

	public void setQuad ( int index, Vector3 position, Vector2 dimension, Vector2 uvtl, Vector2 uvtr, Vector2 uvbl, Vector2 uvbr, Color color )
	{
		// to feed the sprite batch, order of vertices is : bottom left, top left, top right, bottom right
		// (note that uv top/bottom are invert of position top/bottom)
		vertexBatch.setVertex ( index * 4 + 0, position.x, position.y, uvtl.x, uvtl.y, color );
		vertexBatch.setVertex ( index * 4 + 1, position.x, position.y + dimension.y, uvbl.x, uvbl.y, color );
		vertexBatch.setVertex ( index * 4 + 2, position.x + dimension.x, position.y + dimension.y, uvbr.x, uvbr.y, color );
		vertexBatch.setVertex ( index * 4 + 3, position.x + dimension.x, position.y, uvtr.x, uvtr.y, color );
	}

	public void flipUvs ( int index )
	{
		Vector2 uvBR = vertexBatch.uvs[index * 4 + 1];
		vertexBatch.uvs[index * 4 + 1] = vertexBatch.uvs[index * 4 + 3];
		vertexBatch.uvs[index * 4 + 3] = uvBR;
	}

	public void alignVerticesOnIntegers ()
	{
		vertexBatch.alignVerticesOnIntegers ();
	}

	public void colorize ( int index, int count, Color color )
	{
		vertexBatch.colorize ( index * 4, count * 4, color );
	}

	public void setAlpha ( float alpha )
	{
		setAlpha ( 0, Count, alpha );
	}

	public void setAlpha ( int index, int count, float alpha )
	{
		vertexBatch.setAlpha ( index * 4, count * 4, alpha );
	}


	// 	void scaleFromPoint(float centerX, float centerY, float scale)
	// 	{
	// 		vertexBatch.scaleFromPoint ( centerX, centerY, scale );
	// 	}

	// 	public float getQuadX(int index)
	// 	{
	// 		return vertexBatch.getVertexData ( index * 4 + 0, VertexBatch.X_OFFSET );
	// 	}

	// 	public void translateQuadX(int index, float tx)
	// 	{
	// 		float left = vertexBatch.getVertexData ( index * 4 + 0, VertexBatch.X_OFFSET );	// top left
	// 		float right = vertexBatch.getVertexData ( index * 4 + 3, VertexBatch.X_OFFSET ); 	// top right
	// 
	// 		vertexBatch.setVertexData ( index * 4 + 0, VertexBatch.X_OFFSET, left + tx );
	// 		vertexBatch.setVertexData ( index * 4 + 1, VertexBatch.X_OFFSET, left + tx );
	// 		vertexBatch.setVertexData ( index * 4 + 2, VertexBatch.X_OFFSET, right + tx );
	// 		vertexBatch.setVertexData ( index * 4 + 3, VertexBatch.X_OFFSET, right + tx );
	// 	}

	// 	public float getQuadWidth(int index)
	// 	{
	// 		float left = vertexBatch.getVertexData ( index * 4 + 0, VertexBatch.X_OFFSET );	// top left
	// 		float right = vertexBatch.getVertexData ( index * 4 + 3, VertexBatch.X_OFFSET ); 	// top right
	// 		return right - left;
	// 	}
	// 
	// 	public void setQuadWidth(int index, float width)
	// 	{
	// 		float left = vertexBatch.getVertexData ( index * 4 + 0, VertexBatch.X_OFFSET );
	// 		vertexBatch.setVertexData ( index * 4 + 2, VertexBatch.X_OFFSET, left + width );	// bottom right
	// 		vertexBatch.setVertexData ( index * 4 + 3, VertexBatch.X_OFFSET, left + width ); // top right
	// 	}

}