using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class UIParticleSetup : MonoBehaviour
{
    [Range(0.01f, 0.5f)]
    public float particleSizeMultiplier = 0.1f;
    [Range(1, 100)]
    public int emissionRate = 10;
    [Range(0.5f, 5f)]
    public float particleLifetime = 2f;
    private ParticleSystem particles;
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        SetupUIParticles();
    }

    void SetupUIParticles()
    {
        // Check if ParticleContainer already exists
        Transform existingContainer = transform.Find("ParticleContainer");
        GameObject particleContainer;

        if (existingContainer != null)
        {
            particleContainer = existingContainer.gameObject;
            particles = particleContainer.GetComponentInChildren<ParticleSystem>();
            if (particles != null)
            {
                // ParticleSystem already exists, just update its properties
                UpdateParticleProperties();
                Debug.Log("ParticleSystem already exists and updated");
                return;
            }
        }
        else
        {
            // Create ParticleContainer if it doesn't exist
            particleContainer = new GameObject("ParticleContainer");
            particleContainer.transform.SetParent(transform, false);
            particleContainer.transform.SetSiblingIndex(1); // After Image, before Text
            Debug.Log("ParticleContainer created");
        }

        // Setup Mask component if it doesn't exist
        Mask mask = particleContainer.GetComponent<Mask>();
        if (mask == null)
        {
            mask = particleContainer.AddComponent<Mask>();
            mask.showMaskGraphic = false;
            Debug.Log("Mask component created");
        }

        // Setup Image component if it doesn't exist
        Image maskImage = particleContainer.GetComponent<Image>();
        if (maskImage == null)
        {
            maskImage = particleContainer.AddComponent<Image>();
            Image targetImage = GetComponent<Image>();
            if (targetImage != null)
            {
                maskImage.sprite = targetImage.sprite;
                maskImage.type = Image.Type.Sliced; // Use sliced type for better shape conformity
                Debug.Log("Mask Image component created and assigned sprite");
            }
        }

        // Create and setup Particle System if it doesn't exist
        if (particles == null)
        {
            particles = new GameObject("UIParticles").AddComponent<ParticleSystem>();
            particles.transform.SetParent(particleContainer.transform, false);
            particles.transform.localPosition = Vector3.zero;

            // Configure particle system
            var main = particles.main;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            // Adjust renderer
            var renderer = particles.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
            renderer.sortingOrder = GetComponent<Canvas>().sortingOrder + 1;
            Debug.Log("Particle system renderer configured");

            // Add Canvas Renderer to make particles work with UI
            particles.gameObject.AddComponent<CanvasRenderer>();
            Debug.Log("CanvasRenderer added to particle system");

            // Setup basic particle properties
            SetupBasicParticles();
        }

        // Ensure Text is on top
        Text buttonText = GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            buttonText.transform.SetAsLastSibling();
        }

        // Start particle emission
        particles.Play();
        Debug.Log("Particles started playing");
    }

    void SetupBasicParticles()
    {
        var main = particles.main;
        main.startLifetime = particleLifetime;
        main.startSpeed = 0.1f; // Reduced speed for less movement

        UpdateParticleProperties();
    }

    void UpdateParticleProperties()
    {
        var main = particles.main;
        // Calculate particle size based on UI element size
        float minDimension = Mathf.Min(rectTransform.rect.width, rectTransform.rect.height);
        float particleSize = minDimension * particleSizeMultiplier;
        main.startSize = particleSize;

        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = particles.emission;
        emission.rateOverTime = emissionRate;

        var shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Rectangle;
        shape.scale = new Vector3(rectTransform.rect.width, rectTransform.rect.height, 1);

        // Use a sprite renderer instead of a standard particle material
        var renderer = particles.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.material = new Material(Shader.Find("Sprites/Default"));

        Debug.Log("Particle properties updated");
    }

    void LateUpdate()
    {
        // Update particle system shape to match UI element size
        if (particles != null)
        {
            var shape = particles.shape;
            shape.scale = new Vector3(rectTransform.rect.width, rectTransform.rect.height, 1);
        }
    }
}
