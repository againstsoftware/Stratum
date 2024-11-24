using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlinedMesh : MonoBehaviour
{
    [SerializeField] private Material _outlineMaterial;

    private GameObject _outlineGO;
    private Material[] _materials;
    public void Awake()
    {
        _outlineGO = new GameObject("Outline");
        _outlineGO.transform.SetPositionAndRotation(transform.position, transform.rotation);
        _outlineGO.transform.SetParent(transform);

        foreach(var meshRenderer in GetComponentsInChildren<Renderer>())
        {
            var newMeshRenderer = Instantiate(meshRenderer, _outlineGO.transform);

            var newMaterials = new List<Material>();
            for(int i = 0; i < newMeshRenderer.materials.Length; i++)
            {
                newMaterials.Add(_outlineMaterial);
            }
            _materials = newMaterials.ToArray();

            newMeshRenderer.materials = _materials;
            while(newMeshRenderer.TryGetComponent<Collider>(out var collider))
            {
                Destroy(collider);
            }
        }
    }

    public void ToggleOutline(bool don) => _outlineGO.SetActive(don);
}
