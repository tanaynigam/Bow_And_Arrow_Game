// simple "dissolving" shader by genericuser (radware.wordpress.com)
// clips materials, using an image as guidance.
// use clouds or random noise as the slice guide for best results.
  Shader "Custom Shaders/Dissolving" {
    Properties {
      _MainTex ("Texture (RGB)", 2D) = "white" {}
	  _ColorTint("Tint", Color) = (1.0, 1.0, 1.0, 1.0)
      _SliceAmount ("Slice Amount", Range(0.0, 1.0)) = 0.5
    }
    SubShader {
      Tags { "RenderType" = "Opaque" }
      Cull Off
      CGPROGRAM
      //if you're not planning on using shadows, remove "addshadow" for better performance
      #pragma surface surf Lambert addshadow
      struct Input {
          float2 uv_MainTex;
		  float4 _ColorTint;
          float _SliceAmount;
      };
      sampler2D _MainTex;
      float _SliceAmount;
	  float4 _ColorTint;
      void surf (Input IN, inout SurfaceOutput o) {
          clip(IN.uv_MainTex.x - _SliceAmount);
          o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb * _ColorTint;
      }
      ENDCG
    } 
    Fallback "Diffuse"
  }