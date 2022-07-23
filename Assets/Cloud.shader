Shader "Hidden/Cloud"
{
    Properties
    {
		g_CloudVolume("", 3D) = "" {}
		g_ShadowTex("", any) = "" {}
    }
    SubShader
    {
        Cull Back ZWrite Off ZTest Always Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#include "UnityCG.cginc"
			#include "UnityShadowLibrary.cginc"

#define MOAR_SAMPLES 0
#define VOLUME_SHADOW_MAP 1
#define ATMOSPHERE_FOG 0
#define FANCY_VOLUME 0

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = v.vertex;
                o.uv = float4(v.vertex.x * 0.5 + 0.5, v.vertex.y * -0.5 + 0.5, v.vertex.x, -v.vertex.y);
                return o;
            }

            sampler2D _CameraDepthTexture;
			sampler2D g_ShadowTex;
			sampler3D g_CloudVolume;
			float2 g_Density;
			float3 g_LocalCamera;
            float4 g_Player;
			float4 g_LightVecs[8];
			float4x4 g_World, g_WorldView, g_InvWorldViewProjection;

			float rand(float2 co)
			{
				return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
			}

			float remap(float val, float minOrg, float maxOrg, float minNew, float maxNew)
			{
				return clamp(minNew + (val - minOrg) / (maxOrg - minOrg) * (maxNew - minNew), min(minNew, maxNew), max(minNew, maxNew));
			}

			// Returns cloud density at pos
			float SampleCloud(float3 pos, float lod)
			{
				float height = saturate(pos.y / 0.5);
				float c = -(height - 1) * (height - 1) * (height - 1) * 0.0675;
				float3 dir = pos - g_Player.xyz;
				dir.y *= 0.1;
				float3 dist = length(dir);
				dir /= dist;
				float d = tex3Dlod(g_CloudVolume, float4(pos.xyz * float3(1.0, 20.0 / 15.0, 1.0) + dir * saturate(1 - dist * 3) * -0.015 * g_Player.w + _Time.y * float3(0.2,1,0.1) * -0.05, lod * 0.5)).r;
				
				return c * (g_Density.y + d * g_Density.x);
			}

			float4 applyFog(float4 col, float viewDist)
			{
				float fog = max(0, viewDist - 40) * 0.02480597;
				col.rgb = lerp(unity_FogColor.rgb, col.rgb, exp(-fog * fog));
				return col;
			}

			float sampleShadow(float4 rayPos)
			{
				float4 wpos = mul(g_World, float4(rayPos.xyz, 1));
				float3 fromCenter0 = wpos.xyz - unity_ShadowSplitSpheres[0].xyz;
				float3 fromCenter1 = wpos.xyz - unity_ShadowSplitSpheres[1].xyz;
				float3 fromCenter2 = wpos.xyz - unity_ShadowSplitSpheres[2].xyz;
				float3 fromCenter3 = wpos.xyz - unity_ShadowSplitSpheres[3].xyz;
				float4 distances2 = float4(dot(fromCenter0, fromCenter0), dot(fromCenter1, fromCenter1), dot(fromCenter2, fromCenter2), dot(fromCenter3, fromCenter3));
				fixed4 weights = float4(distances2 < unity_ShadowSplitSqRadii);
				weights.yzw = saturate(weights.yzw - weights.xyz);

				float4 coord = mul(unity_WorldToShadow[int(dot(weights, float4(0, 1, 2, 3)))], wpos);;
				return step(tex2Dlod(g_ShadowTex, float4(coord.xy, 0, 0)).r, coord.z) * 0.002 + 0.998;
			}

			// Henyey - Greenstein phase function
			float HG(float cosAngle, float eccentricity)
			{
				return ((1.0 - eccentricity * eccentricity) / pow((1.0 + eccentricity * eccentricity - 2.0 * eccentricity * cosAngle), 3.0 / 2.0));
			}

			// Raymarching for volumetric clouds, some ideas taken from this UE4 blog post https://shaderbits.com/blog/creating-volumetric-ray-marcher
			// Lighting model based on Horizon: Zero Dawn's cloud system: https://www.guerrilla-games.com/read/nubis-realtime-volumetric-cloudscapes-in-a-nutshell
			float4 frag(v2f i) : SV_Target0
			{
				// Box in object space of terrain which contains the clouds
				static float3 sizeMin = float3(-3, 0.0, -3);
				static float3 sizeMax = float3(3, 0.6, 3);

				static float density = 150;
				static float3 color = float3(0.8, 0.8, 0.8);
				static float3 ambient = float3(0.05, 0.025, 0.03);
				static float3 absorption = float3(0.15, 0.07, 0.05);

				// Tweak for quality ~ performance
#if MOAR_SAMPLES
				static uint maxSteps = 512;
				static uint lightSteps = 6; // max is g_LightVecs.Length
				static float stepSize = 4.0 / 512;
#else
				static uint maxSteps = 32;
				static uint lightSteps = 4;
				static float stepSize = 32.0 / 512;
#endif
				static float edgeFact = 50;
				static float samFact = -density * stepSize;
				static float3 shadowFact = -density / absorption;

				float2 pos = i.uv.zw;
				float4 unproj = mul(g_InvWorldViewProjection, float4(pos, 1, 1));
				float4 rayDir = float4(unproj.xyz / unproj.w - g_LocalCamera, 0);
				float3 invRayDir = 1 / rayDir.xyz;

				// AABB intersection test (Well, we're in object space, so it would be an OBB in world space)
				float3 intersect1 = (sizeMin - g_LocalCamera) * invRayDir;
				float3 intersect2 = (sizeMax - g_LocalCamera) * invRayDir;
				float3 minI = min(intersect1, intersect2);
				float3 maxI = max(intersect1, intersect2);
				float3 t = float3(max(0, max(minI.x, max(minI.y, minI.z))), min(maxI.x, min(maxI.y, maxI.z)), 0);
				clip(t.y - t.x); // Discard if no intersection

				// Normalization delayed after intersection test passed
				float rayDirLength = length(rayDir);
				t *= rayDirLength;
				rayDir.xyz *= 1 / rayDirLength;

				// Random jitter to hide undersampling artifacts
				float jitter = rand(i.uv.xy + _Time.y % 1);
				// Ray stops at opaque geometry (calculated from depth texture)
				float depth = tex2D(_CameraDepthTexture, i.uv);
#if defined(UNITY_REVERSED_Z)
				depth = 1.0 - depth;
#endif
				unproj = mul(g_InvWorldViewProjection, float4(pos, depth * 2 - 1, 1));
				t.y = min(2, min(t.y, distance(unproj.xyz / unproj.w, g_LocalCamera)));
				t.x += jitter * stepSize;
				t.z = (t.y - t.x) / stepSize;
				clip(t.z); // Discard if no intersection due to opaque geometry

				float cosAngle = dot(rayDir.xyz, g_LightVecs[0].xyz / g_LightVecs[0].w);
				float inScatter = max(HG(cosAngle, -0.2),  0.1 * HG(cosAngle, 0.7)); // Mostly isotropic scattering some silverlining
				float powderFact = 1 * saturate(-cosAngle); // Darkening of edges when looking away from sun
				float3 baseCol = color * inScatter;

				uint steps = min(maxSteps, t.z);
				float4 rayPos = float4(g_LocalCamera + rayDir.xyz * t.x, 1);
				rayDir *= stepSize;

				// View distance in view space for simple fog
#if ATMOSPHERE_FOG
				rayPos.w = length(mul(g_WorldView, rayPos).xyz);
				rayDir.w = length(mul(g_WorldView, rayDir).xyz);
#endif

				// Raymarching
				float4 dst = 0.0;
				for (uint s = 0; s < maxSteps; s++)
				{
					if (s >= steps)
						break;

					float lod = s / float(maxSteps);
					float sam = SampleCloud(rayPos.xyz, lod);
					if (sam > 0.0001) // Branching is evil, I know
					{
						float dl = 0;

#if VOLUME_SHADOW_MAP
						dl = 1 - sampleShadow(rayPos);

						//if (dl < 0.5)
#endif
						for (uint i1 = 0; i1 < lightSteps; i1++)
							dl += SampleCloud(rayPos.xyz + g_LightVecs[i1].xyz, lod) * g_LightVecs[i1].w;

						float4 src = float4((ambient + baseCol * exp(dl * shadowFact)) * (1 - powderFact * exp(-sam * edgeFact)), 1 - exp(sam * samFact));
#if ATMOSPHERE_FOG
						//UNITY_APPLY_FOG(rayPos.w, src);
						src = applyFog(src, rayPos.w);
#endif
						src.rgb *= src.a;
						dst += src * (1 - dst.a); // Blending

						if (dst.a > 0.99)
							return dst; // Early exit when alpha has reached a threshold (Also skip fracional step)
					}

					rayPos += rayDir;
				}

				// fracional step for smoothing the intersections to other geometry and the box, copy'n'paste the loop with additional factor
				if (t.z < float(maxSteps))
				{
					float fracStep = frac(t.z);
					float lod = steps / float(maxSteps);
					float sam = SampleCloud(rayPos.xyz, lod);
					if (sam > 0.0001)
					{
						float dl = 0;

#if VOLUME_SHADOW_MAP
						dl = 1 - sampleShadow(rayPos);

						//if (dl < 0.5)
#endif
						for (uint i1 = 0; i1 < lightSteps; i1++)
							dl += SampleCloud(rayPos.xyz + g_LightVecs[i1].xyz, lod) * g_LightVecs[i1].w;

						float4 src = float4((ambient + baseCol * exp(dl * shadowFact)) * (1 - powderFact * exp(-sam * edgeFact)), 1 - exp(sam * samFact * fracStep));
#if ATMOSPHERE_FOG
						//UNITY_APPLY_FOG(rayPos.w, src);
						src = applyFog(src, rayPos.w);
#endif
						src.rgb *= src.a;
						dst += src * (1 - dst.a);
					}
				}

				return dst; // Already alpha premultipied
			}

            ENDCG
        }
    }
}
