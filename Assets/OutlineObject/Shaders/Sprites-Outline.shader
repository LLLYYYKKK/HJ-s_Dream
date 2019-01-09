// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Sprites/Outline"
{
	Properties
	{
		_OutlineColor ("Outline Color", Color) = (1,1,1,1)
		_OutlineSize  ("Outline Size",  Int)   = 1
		_OutlineBlur  ("Outline Blur",  Int)   = 0

		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		[HideInInspector] _Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex SpriteVert
			#pragma fragment OutlineSpriteFrag
			#pragma target 4.0
			#pragma multi_compile_instancing
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnitySprites.cginc"

			float4 _MainTex_TexelSize;
			float4 _OutlineColor;
			int    _OutlineSize;
			int    _OutlineBlur;

			float2 _inTexcoord;
			float2 _pixelTexcoord;
            float  _pixelAlpha;
            int    _strokeThickness;
            fixed4 _outColor;

            float DistanceBetween (int x1, int y1, int x2, int y2)
            {
            	int deltaX = x2 - x1;
            	int deltaY = y2 - y1;

            	return sqrt (deltaX * deltaX + deltaY * deltaY);
            }

            bool HasPixelAt (int x, int y)
            {
            	_pixelTexcoord.x = _inTexcoord.x + (x * _MainTex_TexelSize.x);
				_pixelTexcoord.y = _inTexcoord.y + (y * _MainTex_TexelSize.y);

				_pixelAlpha = SampleSpriteTexture (_pixelTexcoord).a;

				return (_pixelAlpha > 0);
            }

            bool TrySetPixelAt (int x, int y)
            {
            	if (!HasPixelAt (x, y))
            		return false;

        		_outColor      = _OutlineColor;
				_outColor.rgb *= _OutlineColor.a;

				// Get the distance from the current transparent pixel to the closest opaque pixel.
				float distanceToClosestPixel = DistanceBetween (0, 0, x, y);

				if (distanceToClosestPixel > _OutlineSize)
					_outColor.rgba = 0;
				else if (distanceToClosestPixel > _strokeThickness)
					_outColor.rgba /= pow (2, (distanceToClosestPixel - _strokeThickness) / 0.7);

				return true;
            }

			fixed4 OutlineSpriteFrag (v2f IN) : SV_Target {
				_inTexcoord      = IN.texcoord;
				_strokeThickness = _OutlineSize - min (_OutlineBlur, _OutlineSize - 1);

				int i, j, gridSize, dir = -1;

				for (int gridRadius = 1; gridRadius <= _OutlineSize; gridRadius++)
				{
					gridSize = gridRadius * 2;
					j        = 0;

					for (i = 0; i < gridSize; i++)
					{
						j   +=  i * dir;
						dir *= -1;

						if (TrySetPixelAt (-gridRadius, j) || TrySetPixelAt (j, gridRadius) || TrySetPixelAt (gridRadius, -j) || TrySetPixelAt (-j, -gridRadius))
							return _outColor;
					}
				}

				return _outColor;
			}
		ENDCG
		}
	}
}
