using Assets.Scripts.Shared;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhysicsSimulator : MonoBehaviour
{
    public bool takeControl;
    private List<Rigidbody> rigidbodies = new List<Rigidbody>();

    void Start()
    {
        if (takeControl)
            Physics.autoSimulation = false;

        RescanSceneForRigidbodies();
        GigaNetGlobals.physics = this;
    }

    public void Simulate(Rigidbody sim)
    {
        IEnumerable<bool> kinematics = rigidbodies.Select(rb => rb.isKinematic);
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            if (rigidbody != sim)
            {
                rigidbody.isKinematic = true;
            }
        }

        Physics.Simulate(Time.fixedDeltaTime);

        foreach ((Rigidbody, bool) item in rigidbodies.Zip(kinematics, (rb, isKin) => (rb, isKin)))
        {
            Rigidbody rb = item.Item1;
            bool isKinematic = item.Item2;
            rb.isKinematic = isKinematic;
        }
    }

    public void RescanSceneForRigidbodies()
    {
        rigidbodies.Clear();

        Scene scene = SceneManager.GetActiveScene();
        IEnumerable<Transform> roots = scene.GetRootGameObjects().Select(go => go.transform);
        foreach (Transform root in roots)
        {
            RecursivePopulateRigidbodies(root, rigidbodies);
        }
    }

    private void RecursivePopulateRigidbodies(Transform parentTransform, List<Rigidbody> rbs)
    {
        if (parentTransform.gameObject.TryGetComponent(out Rigidbody rb))
        {
            rbs.Add(rb);
        }

        foreach (Transform child in parentTransform)
        {
            RecursivePopulateRigidbodies(child, rbs);
        }
    }
}
