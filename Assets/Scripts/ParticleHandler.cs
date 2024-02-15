using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Particle {
    public Vector2 position;
    public Vector2 velocity;
    public int type; // Will be color too
    public float rad;
}

public class ParticleHandler : MonoBehaviour
{

    [SerializeField, Range(10, 100000)]  // To make the option appear in the field
	int particleCnt = 10;

    [SerializeField, Range(0.01f, 1.0f)]
    float particleRad = 0.1f;



    //[SerializeField]
	//GameObject particlePrefab;
    //
    //GameObject[] particlePos;

    Particle[] particles;

    float bound = 10.0f;
    

    void InitParticles() {
        particles = new Particle[particleCnt];
        //particlePos = new GameObject[particleCnt];
        for(int i = 0; i < particleCnt; i++) {
            // Particle struct set
            Particle particle = new Particle();
            particle.position = new Vector2(Random.Range(-1f*bound, 1f*bound), Random.Range(-1f*bound, 1f*bound));
            particle.velocity = new Vector2(0.0f, 0.0f);
            particle.rad = particleRad;


            //public enum Types {RED, GREEN, BLUE, YELLOW}


            particle.type = Random.Range(0, 4); // Only total 4 types supported
            particles[i] = particle;
            //GameObject obj;
            //
//
            //obj = Instantiate(particlePrefab, new Vector3(0.1f, 0.1f, 0.1f), Quaternion.identity);
            ////GameObject currParticle.transform = Instantiate(particlePrefab);
            //particlePos[i] = obj;
//
            //if(particles[i].type == 0)
            //    particlePos[i].GetComponent<Renderer>().material.color = Color.red; // Very slow
            //if(particles[i].type == 1)
            //    particlePos[i].GetComponent<Renderer>().material.color = Color.blue; // Very slow
            //if(particles[i].type == 2)
            //    particlePos[i].GetComponent<Renderer>().material.color = Color.green; // Very slow
            //if(particles[i].type == 3)
            //    particlePos[i].GetComponent<Renderer>().material.color = Color.yellow; // Very slow
            //
            //particlePos[i].transform.localScale = new Vector3(0.09f, 0.09f, 0.09f);


        }
    }

    [SerializeField]
	ComputeShader computeShader;

    static readonly int particlesId = Shader.PropertyToID("particles"); // Get the ID to the compute buffer
    static readonly int resolutionId = Shader.PropertyToID("numParticles"); // Get the ID to the resolution
    static readonly int boundId = Shader.PropertyToID("bound"); // Get the ID to the resolution
    ComputeBuffer particlesBuffer;

    void CallComputeGPU() {
        int totalsize = sizeof(float) * 2 + sizeof(float) * 2 + sizeof(int) + sizeof(float); 
        particlesBuffer = new ComputeBuffer(particleCnt, totalsize); // Total elements, and size of each element
        particlesBuffer.SetData(particles); // Pass the particles data to the buffer
        computeShader.SetBuffer(0, particlesId, particlesBuffer); // Set to kernel 0, the ID to the buffer, the buffer data
        computeShader.SetInt(resolutionId, particleCnt);
        computeShader.SetFloat(boundId, bound);
        //computeShader.SetInt(resolutionId, particleCnt);

        
    } 


    [SerializeField]
	Material material;

	[SerializeField]
	Mesh mesh;

    void CallGraphicsGPU() {
        material.SetBuffer(particlesId, particlesBuffer);
    }

    // Start is called before the first frame update
    void Start()
    {
        InitParticles();
        CallComputeGPU(); // Sets data and call GPU 
        CallGraphicsGPU();
    }

    void OnDestroy() {
        particlesBuffer.Release();
        particlesBuffer = null;
    }
    


    


    // Update is called once per frame
    void Update() // This loop is too slow
    {
        
        computeShader.Dispatch(0, (int)(particleCnt%65535), 1, 1); // Call Kernel 0
        var bounds = new Bounds(Vector3.zero, Vector3.one * bound * 2); // Around the origin

        // TODO: Figure out how to turn a Quad mesh into a circle
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, particleCnt); // Draw using subshader 0

        
        // Get Data
        //particlesBuffer.GetData(particles);  // Gets GPU data
        ////Debug.Log(particles[0].position);
        //// Render here
        //for(int i = 0; i < particleCnt; i++) {
        //    // Call material/render material
        //    particlePos[i].transform.position = particles[i].position;
        //}
    }
}
