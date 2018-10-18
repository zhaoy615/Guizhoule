Shader "CookBookShaders/Cover Translucent" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque"}
		LOD 200
		
		ZWrite On
		ZTest greater  //Greater/GEqual/Less/LEqual/Equal/NotEqual/Always/Never/Off 默认是LEqual 如果要绘制的像素的Z值 小余等于深度缓冲区中的值，那么就用新的像素颜色值替换。这里使用 Greater，代表如果当前要渲染的像素 Z值大于 缓冲区中的Z，才渲染，也就是后面的物体覆盖了前面的。
		CGPROGRAM
		#pragma surface surf Lambert alpha 
		//加上alpha让被遮挡的这部分透明显示

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};
		fixed4 _Color;
		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = 0.5f;
		}
		ENDCG
		
		//上面设置了ZTest greater后，只有被遮挡的地方才渲染出来。所以下面把没有遮挡的地方渲染出来。
		ZWrite On
		ZTest LEqual  //Greater/GEqual/Less/LEqual/Equal/NotEqual/Always/Never/Off 默认是LEqual 如果要绘制的像素的Z值 小余等于深度缓冲区中的值，那么就用新的像素颜色值替换。这里使用 Greater，代表如果当前要渲染的像素 Z值大于 缓冲区中的Z，才渲染，也就是后面的物体覆盖了前面的。
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};
		fixed4 _Color;
		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
