Shader "Custom/HexGrid2" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_SecondTex ("Albedo (RGB)", 2D) = "white" {}
		_ThirdTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0
		_Transparency("Transparency", Range(0,1)) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 100

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _SecondTex;
		sampler2D _ThirdTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		float _Transparency;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 MainCol  = tex2D (_MainTex, IN.uv_MainTex);
			fixed4 SecondCol;//= tex2D (_SecondTex, float2(0,0));
			fixed4 c = MainCol;
			o.Albedo = c.rgb* _Color;
			// Draw a regular polygon with N sides in the center of the texture space (0, 1) x (0, 1)
			int N = 6; // Hexagon if N == 6
			float pi = 3.1415926535897932;

			//calculating the number of needed hexagons to fill the quad
			//basis vector for the hex grid
			float2 basic1 =float2(3,sqrt(3.0)/2);
			
			//raduis
			float basicR = 0.03;

			//con't of calculation to calculate the number of cells along each axis
			basic1 = basic1*basicR;
			
			int xCells = ceil(1/basic1.x);
			int yCells = ceil(1/basic1.y)/4;
			//a few extra horizontal cells to generate all the vertical cells

			float diff = 1;// smoothstep(0, thickness, abs(real_r -r));

			//offsets for shifting the hexes
			float yeven = 0.1;
			float xodd = 0.085;
			float yodd = 0.04;
			xCells = xCells + 1;
			for(int i = 0; i <= xCells; i++)
			{
				for(int j =0; j <= yCells; j++)
				{
				// Get texture coordinates of the current fragment, which will be the point's coordinates
					float2 pos = IN.uv_MainTex;
					//positioning the hexes
					if(i%2 == 0)
					{
						pos += float2(-0.06-(i*xodd),-0.05-(j*yeven));
						pos *= 10.0;				
					}
					else
					{
						pos += float2(-0.06-(i*xodd),-0.06-(j*yeven)-yodd);
						pos *= 10.0;
					}
					// Compute polar coordinates of the current point in relation to the origin
					// We get an angle theta and radius r that describe the point in polar coordinates
					float theta = atan2(pos.x, pos.y);
					float r = sqrt(pos.x*pos.x + pos.y*pos.y);
					// Map the polar coordinates to an angle between [-pi/6, pi/6] to make our life easier
					float angle = theta - 2.0 * pi / N*floor((N*theta + pi) / (2 * pi));
					// Compute the radius that a point with this angle should have
					float real_r = cos((2 * pi) / N) / cos(angle);
					// Draw the point by comparing the readius the point should have with the actual radius
					// We are basically checking how far the point is from an edge of the polygon
					float thickness = 0.02;
					// smoothstep maps [0. thickness] to [0, 1]
					float diff2 = smoothstep(0, thickness, abs(real_r -r));
					if (diff > diff2)
					{
					//updating if there is a new hex to be added
					 diff = diff2;
					}
					//adding textures to the hexes depending on their position on the map
					if(r < real_r )
					{
						SecondCol  = tex2D (_MainTex, float2(r*cos(theta), r*sin(theta)));  
						if (i >= xCells / 4 && i <= (xCells / 4) + (xCells / 2))
						{
							if (j >= yCells / 4 && j <= (yCells / 4) + (yCells / 2))
							{
								SecondCol  = tex2D (_SecondTex, float2(r*cos(theta), r*sin(theta)));
							}
				    }
						else if (i >= xCells / 8 && i <= (xCells / 4) || i >= (xCells / 4) + (xCells / 2) && i < (xCells - xCells / 8))
						{
							if (j >= yCells / 8 && j <= (yCells / 4) || j >= (yCells / 4) + (yCells / 2) && j < (yCells - yCells / 8))
							{
								SecondCol  = tex2D (_ThirdTex, float2(r*cos(theta), r*sin(theta)));  
							}
						}
					}
				}
			}
			//	

			// Set output fragment
			o.Metallic = _Metallic;
			o.Albedo = (SecondCol) *float3(diff, diff, diff);
			//o.Albedo = float3(diff, diff, diff);
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
