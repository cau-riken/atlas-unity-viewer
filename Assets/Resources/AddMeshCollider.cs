using UnityEngine;

public class AddMeshCollider : MonoBehaviour
{
    void Reset()
    {
        Mesh m = null;
        var mf = GetComponentInChildren<MeshFilter>();
        if (mf != null)
            m = mf.sharedMesh;
        var smr = GetComponentInChildren<SkinnedMeshRenderer>();
        if (smr != null)
            m = smr.sharedMesh;
        if (m != null)
        {
            float quality = 0.5f;
            var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
            meshSimplifier.Initialize(m);
            meshSimplifier.SimplifyMesh(quality);
            var dstMesh = meshSimplifier.ToMesh();

            var col = GetComponentInChildren<MeshCollider>();
            if (col == null)
                col = gameObject.AddComponent<MeshCollider>();
            col.sharedMesh = dstMesh;
        }
    }
}
