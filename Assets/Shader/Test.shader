Shader "Custom/Test"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
			float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {

			const int NUM_COLORS = 4;
			float3 color[NUM_COLORS];
			color[0] = float3(1, 0, 0);
			color[1] = float3(0, 1, 0);
			color[2] = float3(1, 1, 0);
			color[3] = float3(0, 0, 1);


			float value = IN.worldPos.y / 16;
			// A static array of 4 colors:  (blue,   green,  yellow,  red) using {r,g,b} for each.

			int idx1;        // |-- Our desired color will be between these two indexes in "color".
			int idx2;        // |
			float fractBetween = 0;  // Fraction between "idx1" and "idx2" where our value is.

			if (value <= 0) { idx1 = idx2 = 0; }    // accounts for an input <=0
			else if (value >= 1) { idx1 = idx2 = NUM_COLORS - 1; }    // accounts for an input >=0
			else
			{
				value = value * (NUM_COLORS - 1);        // Will multiply value by 3.
				idx1 = floor(value);                  // Our desired color will be after this index.
				idx2 = idx1 + 1;                        // ... and before this index (inclusive).
				fractBetween = value - float(idx1);    // Distance between the two indexes (0-1).
			}
			float3 fCol;
			fCol.x = (color[idx2].x - color[idx1].x)*fractBetween + color[idx1].x;
			fCol.y = (color[idx2].y - color[idx1].y)*fractBetween + color[idx1].y;
			fCol.z = (color[idx2].z - color[idx1].z)*fractBetween + color[idx1].z;

            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = fCol;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
