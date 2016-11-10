using UnityEngine;
using System.Collections;



public struct ColorHSL
{
	public float h; // 0..360
	public float s; // 0..1
	public float l; // 0..1

	public ColorHSL ( float h, float s, float l )
	{
		this.h = h;
		this.s = s;
		this.l = l;
	}

	public void Mutate ( float hBias, float sBias, float lBias )
	{
		h = ( ( h + Random.Range ( -hBias * 360, hBias * 360 ) ) + 360 ) % 360;
		s = Mathf.Clamp ( s + Random.Range ( -sBias, sBias ), 0, 1 );
		l = Mathf.Clamp ( l + Random.Range ( -lBias, lBias ), 0, 1 );
	}
}


public static class ColorUtil
{
	public static readonly Color darkGray = new Color ( 0.25f, 0.25f, 0.25f );
	public static readonly Color gray = new Color ( 0.5f, 0.5f, 0.5f );
	public static readonly Color lightGray = new Color ( 0.75f, 0.75f, 0.75f );

// 	public static readonly Color orange = fromRGB ( 255, 165, 0 );
// 	public static readonly Color olive = fromRGB ( 128, 128, 0 );
// 
// 	public static readonly Color navy = fromRGB ( 0, 0, 128 );
// 	public static readonly Color gold = fromRGB ( 237, 218, 116 );

	public static Color fromRGB ( int r, int g, int b )
	{
		return new Color ( (float) r / 255, (float) g / 255, (float) b / 255, 1 );
	}

	public static Color fromRGBA(int r, int g, int b, int a)
	{
		return new Color ( (float) r / 255, (float) g / 255, (float) b / 255, (float) a / 255 );
	}

	public static Color darker ( Color color, float f )
	{
		float inv = 1 - f;
		return new Color ( color.r * inv, color.g * inv, color.b * inv, color.a );
	}

	public static Color lighter ( Color color, float f )
	{
		return new Color ( color.r + ( 1 - color.r ) * f, color.g + ( 1 - color.g ) * f, color.b + ( 1 - color.b ) * f, color.a );
	}

	public static Color lerp ( Color from, Color to, float f )
	{
		float inv = 1 - f;
		return new Color ( from.r * inv + to.r * f, from.g * inv + to.g * f, from.b * inv + to.b * f, from.a * inv + to.a * f );
	}

	public static Color fromColor ( Color color, float alpha )
	{
		Color c = color;
		c.a = alpha;
		return c;
	}

	public static Color fromRGBA ( int rgba )
	{
		int ir = ( rgba >> 24 ) & 0xFF;
		int ig = ( rgba >> 16 ) & 0xFF;
		int ib = ( rgba >> 8 ) & 0xFF;
		int ia = ( rgba >> 0 ) & 0xFF;
		float r = (float)ir / 255.0f;
		float g = (float)ig / 255.0f;
		float b = (float)ib / 255.0f;
		float a = (float)ia / 255.0f;
		return new Color ( r, g, b, a );
	}

	public static void rgbToHsl ( Color rgb, out ColorHSL hsl )
	{
		float r = rgb.r;
		float g = rgb.g;
		float b = rgb.b;
		float max = Mathf.Max ( Mathf.Max ( r, g ), b );
		float min = Mathf.Min ( Mathf.Min ( r, g ), b );
		float c = max - min;

		float h_ = 0.0f;
		if (c == 0)
		{
			h_ = 0;
		}
		else if (max == r)
		{
			h_ = (float) (g - b) / c;
			if (h_ < 0)
				h_ += 6.0f;
		}
		else if (max == g)
		{
			h_ = (float) (b - r) / c + 2.0f;
		}
		else if (max == b)
		{
			h_ = (float) (r - g) / c + 4.0f;
		}
		float h = 60.0f * h_;

		float l = (max + min) * 0.5f;

		float s;
		if (c == 0)
		{
			s = 0.0f;
		}
		else
		{
			s = c / (1 - Mathf.Abs ( 2.0f * l - 1.0f ));
		}

		hsl = new ColorHSL ( h, s, l );
	}

	public static void hslToRgb ( ColorHSL hsl, out Color rgb )
	{
		float h = hsl.h;
		float s = hsl.s;
		float l = hsl.l;

		float c = ( 1 - Mathf.Abs ( 2.0f * l - 1.0f ) ) * s;
		float h_ = h / 60.0f;
		float h_mod2 = h_;
		if (h_mod2 >= 4.0f)
			h_mod2 -= 4.0f;
		else if (h_mod2 >= 2.0f)
			h_mod2 -= 2.0f;

		float x = c * ( 1 - Mathf.Abs ( h_mod2 - 1 ) );
		float r_, g_, b_;
		if (h_ < 1)
		{
			r_ = c;
			g_ = x;
			b_ = 0;
		}
		else if (h_ < 2)
		{
			r_ = x;
			g_ = c;
			b_ = 0;
		}
		else if (h_ < 3)
		{
			r_ = 0;
			g_ = c;
			b_ = x;
		}
		else if (h_ < 4)
		{
			r_ = 0;
			g_ = x;
			b_ = c;
		}
		else if (h_ < 5)
		{
			r_ = x;
			g_ = 0;
			b_ = c;
		}
		else
		{
			r_ = c;
			g_ = 0;
			b_ = x;
		}

		float m = l - (0.5f * c);

		rgb = new Color ( r_ + m, g_ + m, b_ + m );

//		float m = l - (0.5f * c);
//		int r = (int) ((r_ + m) * (255.0f) + 0.5f);
//		int g = (int) ((g_ + m) * (255.0f) + 0.5f);
//		int b = (int) ((b_ + m) * (255.0f) + 0.5f);
//		return r << 24 | g << 16 | b << 8 | 0xFF;
	}

	public static Color fromGray ( int v )
	{
		return fromRGB ( v, v, v );
	}

	public static Color multiply ( Color a, Color b )
	{
		return new Color ( a.r * b.r, a.g * b.g, a.b * b.b, a.a * b.a );
	}

	public static Color GetRandomHueColor ( float saturation, float lightness )
	{
		ColorHSL hsl = new ColorHSL ()
		{
			h = RandomFloat.Range ( 0, 360 ),
			s = saturation,
			l = lightness
		};
		Color rgb;
		hslToRgb ( hsl, out rgb );
		return rgb;
	}
}