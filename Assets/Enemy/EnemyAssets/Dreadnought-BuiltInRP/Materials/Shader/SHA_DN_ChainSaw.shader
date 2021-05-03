// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "UModeler/DN/ChainSaw"
{
	Properties
	{
		_TintColor("TintColor", Color) = (0,0,0,0)
		[HDR]_EmissiveColor("EmissiveColor", Color) = (45.25483,5.679038,0,0)
		_HeightIntensity("HeightIntensity", Range( 0 , 1)) = 1
		_ChainSawRes("ChainSawRes", Float) = 20
		_ChainSaw_Speed("ChainSaw_Speed", Float) = 0
		[Toggle(_TRANSFORMSWITCH_ON)] _TransformSwitch("TransformSwitch", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _TRANSFORMSWITCH_ON
		#pragma surface surf Standard keepalpha noshadow exclude_path:deferred vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _ChainSawRes;
		uniform float _ChainSaw_Speed;
		uniform float _HeightIntensity;
		uniform float4 _TintColor;
		uniform float4 _EmissiveColor;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float temp_output_14_0 = ( ( frac( ( ( v.texcoord.xy.y * _ChainSawRes ) + ( _Time.y * _ChainSaw_Speed ) ) ) + -0.5 ) * 2.0 );
			#ifdef _TRANSFORMSWITCH_ON
				float staticSwitch38 = abs( temp_output_14_0 );
			#else
				float staticSwitch38 = temp_output_14_0;
			#endif
			float3 ase_vertexNormal = v.normal.xyz;
			float temp_output_35_0 = ( 1.0 - abs( ( ( v.texcoord.xy.x + -0.5 ) * 2.0 ) ) );
			v.vertex.xyz += ( ( ( staticSwitch38 * _HeightIntensity ) * ase_vertexNormal ) * temp_output_35_0 );
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Albedo = _TintColor.rgb;
			float temp_output_35_0 = ( 1.0 - abs( ( ( i.uv_texcoord.x + -0.5 ) * 2.0 ) ) );
			o.Emission = ( _EmissiveColor * temp_output_35_0 ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
}
/*ASEBEGIN
Version=18701
1164;11;1396;1066;3362.795;283.6988;1.416561;True;False
Node;AmplifyShaderEditor.RangedFloatNode;26;-2611.171,289.028;Inherit;False;Property;_ChainSaw_Speed;ChainSaw_Speed;4;0;Create;True;0;0;False;0;False;0;12.46;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;22;-2585.171,73.02809;Inherit;False;Property;_ChainSawRes;ChainSawRes;3;0;Create;True;0;0;False;0;False;20;0.53;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;11;-2626.526,-65.18861;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;24;-2554.171,187.0281;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-2380.172,9.028123;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-2308.172,182.0281;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;23;-2166.171,79.02809;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;20;-1987.172,91.0281;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;13;-1812.172,90.0281;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;-0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-1681.172,91.0281;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;32;-1591.209,707.8547;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;-0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;15;-1858.172,349.0279;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-1640.172,200.0281;Inherit;False;Property;_HeightIntensity;HeightIntensity;2;0;Create;True;0;0;False;0;False;1;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;38;-1670.151,314.9268;Inherit;False;Property;_TransformSwitch;TransformSwitch;5;0;Create;True;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-1460.209,708.8547;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;34;-1318.209,710.8547;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;27;-1163.171,354.0279;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-1373.171,92.0281;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-940.1702,218.028;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;35;-1142.568,750.3077;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;16;-925.251,-32.22089;Inherit;False;Property;_EmissiveColor;EmissiveColor;1;1;[HDR];Create;True;0;0;False;0;False;45.25483,5.679038,0,0;4.541205,0.4992948,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;17;-919.251,-221.2209;Inherit;False;Property;_TintColor;TintColor;0;0;Create;True;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-760.5676,516.3078;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-452.2481,142.9216;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;47;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;UModeler/DN/ChainSaw;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;False;0;False;Opaque;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;21;0;11;2
WireConnection;21;1;22;0
WireConnection;25;0;24;0
WireConnection;25;1;26;0
WireConnection;23;0;21;0
WireConnection;23;1;25;0
WireConnection;20;0;23;0
WireConnection;13;0;20;0
WireConnection;14;0;13;0
WireConnection;32;0;11;1
WireConnection;15;0;14;0
WireConnection;38;1;14;0
WireConnection;38;0;15;0
WireConnection;33;0;32;0
WireConnection;34;0;33;0
WireConnection;18;0;38;0
WireConnection;18;1;19;0
WireConnection;28;0;18;0
WireConnection;28;1;27;0
WireConnection;35;0;34;0
WireConnection;36;0;28;0
WireConnection;36;1;35;0
WireConnection;37;0;16;0
WireConnection;37;1;35;0
WireConnection;47;0;17;0
WireConnection;47;2;37;0
WireConnection;47;11;36;0
ASEEND*/
//CHKSM=DD17C10B5D812B0A4665A19FD6E6A7041228EF81