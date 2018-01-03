// 
//   NatCorder
//   Copyright (c) 2017 Yusuf Olokoba
//

Shader "Hidden/NatCorder/Transform" {
    Properties {
		_MainTex ("Texture", 2D) = "white" {}
	}
    SubShader {
        Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;

            fixed4 frag (v2f_img i) : SV_Target	{
				return tex2D(_MainTex, half2(i.uv.x, 1.0 - i.uv.y));
			}
            ENDCG
        }
    }
}