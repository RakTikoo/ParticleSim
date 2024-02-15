Shader "Custom/Particle2D"
{
    //Properties
    //{
    //    _Color ("Color", Color) = (1,1,1,1)
    //    _MainTex ("Albedo (RGB)", 2D) = "white" {}
    //    _Glossiness ("Smoothness", Range(0,1)) = 0.5
    //    _Metallic ("Metallic", Range(0,1)) = 0.0
    //}
    SubShader
    {
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface ConfigureSurface Standard fullforwardshadows
        #pragma instancing_options assumeuniformscaling procedural:ConfigureProcedural 
        #pragma editor_sync_compilation
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 4.5

        struct Input {
            float3 worldPos; // float3 is shader equivanet of vect3. worldPos contains worldPos of what is rendered
        };
        
        struct Particle {
            float2 position;
            float2 velocity;
            int type; // Will be color too
            float rad;
        };


        //float _Step;
        
        int type;

        #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
            StructuredBuffer<Particle> particles; // Only to read and render. Structured buffer. 
        #endif

        
        void ConfigureProcedural () {
            #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
                float2 position = particles[unity_InstanceID].position; // We can access the compute buffer using the unity_InstanceID which is universally defined
                unity_ObjectToWorld._m03_m13_m23_m33 = float4(position, 1.0, 1.0); // The transformation matrix is a 4x4 matrix. The 4th col has the positions. 
                unity_ObjectToWorld._m00_m11_m22 = particles[unity_InstanceID].rad; // The diagonal has the scale
                type = particles[unity_InstanceID].type;
            #endif
        }


        // surface configuration data of type inout to  indicate that it's both passed to the function and used for the result of the function
        void ConfigureSurface (Input input, inout SurfaceOutputStandard surface) {
            //surface.Albedo = input.worldPos*_AlbedoFactor; // Albedo tells the color of the points
            if(type == 0)
                surface.Albedo = float3(1.0, 0.0, 0.0); 
            if(type == 1)
                surface.Albedo = float3(0.0, 1.0, 0.0); 
            if(type == 2)
                surface.Albedo = float3(0.0, 0.0, 1.0); 
            if(type == 3)
                surface.Albedo = float3(1.0, 1.0, 0.0); 
            //surface.Smoothness = _Smoothness; // No need to add f suffix for floats
            
        }
        ENDCG
    }
    FallBack "Diffuse"
}
