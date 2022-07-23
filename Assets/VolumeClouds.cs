using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VolumeClouds : MonoBehaviour
{
    public Transform player;
    public Light sun;
    public ComputeShader volumeGenerator;
    public Shader volumeShader;
    public int volumeRes = 256;
    public int shadowMapRes = 1024;
    public int renderQueue = 3400;
    
    [Range(0, 5)]
    public float cloudDensity = 1;
    [Range(0, 5)]
    public float fogDensity = 1;
    public bool procedualWeather;

    private Camera cam;
    private RenderTexture tex3D, texShadow;
    private MeshRenderer rend;
    private Material mat;
    private int seed;

    private void Start()
    {
        seed = System.Environment.TickCount;

        cam = FindObjectOfType<Camera>();
        cam.depthTextureMode |= DepthTextureMode.Depth;

        transform.position = new Vector3((int)(cam.transform.position.x / transform.localScale.x) * transform.localScale.x, transform.position.y, (int)(cam.transform.position.z / transform.localScale.z) * transform.localScale.z);

        MeshFilter mf = GetComponent<MeshFilter>();
        Mesh m = new Mesh();
        mf.mesh = m;

        List<Vector3> verts = new List<Vector3>();
        verts.Add(new Vector3(-1, 1, 0));
        verts.Add(new Vector3(1, 1, 0));
        verts.Add(new Vector3(-1, -1, 0));
        verts.Add(new Vector3(1, -1, 0));
        m.SetVertices(verts);
        m.SetIndices(new int[] { 0, 3, 1, 0, 2, 3 }, MeshTopology.Triangles, 0);
        m.bounds = new Bounds(Vector3.zero, transform.localScale);

        tex3D = new RenderTexture(volumeRes, volumeRes, 0, RenderTextureFormat.R8);
        tex3D.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;
        tex3D.useMipMap = true;
        tex3D.enableRandomWrite = true;
        tex3D.volumeDepth = volumeRes;
        tex3D.filterMode = FilterMode.Trilinear;
        tex3D.wrapMode = TextureWrapMode.Repeat;
        tex3D.autoGenerateMips = true;
        tex3D.Create();

        volumeGenerator.SetVector("g_VolumeOffset", new Vector4(Random.Range(0, 256.0F), Random.Range(0, 256.0F), Random.Range(0, 256.0F)));
        volumeGenerator.SetVector("g_VolumeRes", new Vector4(volumeRes, volumeRes, volumeRes));
        volumeGenerator.SetTexture(0, "Result", tex3D);
        volumeGenerator.Dispatch(0, volumeRes / 8, volumeRes / 8, volumeRes / 8);

        RenderTargetIdentifier shadowmap = BuiltinRenderTextureType.CurrentActive;
        texShadow = new RenderTexture(shadowMapRes, shadowMapRes, 0);
        texShadow.filterMode = FilterMode.Point;
        CommandBuffer cb = new CommandBuffer();
        cb.SetShadowSamplingMode(shadowmap, ShadowSamplingMode.RawDepth);
        cb.Blit(shadowmap, new RenderTargetIdentifier(texShadow));
        sun.AddCommandBuffer(LightEvent.AfterShadowMap, cb);

        mat = new Material(volumeShader);
        mat.renderQueue = renderQueue;
        mat.SetTexture("g_CloudVolume", tex3D);
        mat.SetTexture("g_ShadowTex", texShadow);

        rend = GetComponent<MeshRenderer>();
        rend.material = mat;
    }

    private float playerSpeed;
    private Vector3 prevPlayerPosition;
    private void LateUpdate()
    {
        if (procedualWeather)
        {
            fogDensity = 0.01F + 0.5F * Mathf.PerlinNoise(Time.time * 0.02F, 0.2F + seed);
            cloudDensity = 0.5F + 4.5F * Mathf.PerlinNoise(Time.time * 0.05F, 9.4F + seed);
        }
        
        Vector3 playerPos = player.position;
        playerSpeed = Mathf.Lerp(playerSpeed, (playerPos - prevPlayerPosition).magnitude / Time.deltaTime, Time.deltaTime * 3);
        playerSpeed = Mathf.Min(10, playerSpeed);
        prevPlayerPosition = playerPos;
    }

    public void OnWillRenderObject()
    {
        if (mat == null)
            return;

        Camera activeCam = Camera.current;

        transform.position = new Vector3((int)(cam.transform.position.x / transform.localScale.x) * transform.localScale.x, transform.position.y, (int)(cam.transform.position.z / transform.localScale.z) * transform.localScale.z);
        Vector4[] lightVecs = new Vector4[8];
        float[] dist = new float[]{ 8, 16, 32, 48, 64, 96, 128, 256 };
        Vector3 dir = -sun.transform.forward.normalized;
        float prevLen = 0;
        for (int i = 0; i < lightVecs.Length; ++i)
        {
            Vector3 v = dir * dist[i] / 32.0F;
            v = transform.InverseTransformVector(v);

            float len = v.magnitude;
            lightVecs[i] = new Vector4(v.x, v.y, v.z, len - prevLen);
            prevLen = len;
        }

        Vector3 playerPos = transform.InverseTransformPoint(player.position);
        mat.SetVector("g_Player", new Vector4(playerPos.x, playerPos.y, playerPos.z, playerSpeed));
        mat.SetVector("g_LocalCamera", transform.InverseTransformPoint(activeCam.transform.position));
        mat.SetMatrix("g_InvWorldViewProjection", (activeCam.projectionMatrix * activeCam.worldToCameraMatrix * transform.localToWorldMatrix).inverse);
        mat.SetMatrix("g_WorldView", activeCam.worldToCameraMatrix * transform.localToWorldMatrix);
        mat.SetMatrix("g_World", transform.localToWorldMatrix);
        mat.SetVector("g_Density", new Vector3(cloudDensity, fogDensity, 0));
        mat.SetVectorArray("g_LightVecs", lightVecs);
    }
}
